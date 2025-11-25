using OrderLunchWeb.Data;
using OrderLunchWeb.Models;
using System.Text;
using System.Text.Json;
using Xunit;

namespace OrderLunchWeb.Tests.Unit;

/// <summary>
/// JsonFileStorage 單元測試 (User Story 1)
/// 測試 JSON 檔案儲存的核心功能：初始化、新增、ID 管理、UTF-8 編碼
/// </summary>
public class JsonFileStorageTests : IDisposable
{
    private readonly string _testDataDirectory;
    private readonly string _testFileName;
    private readonly JsonFileStorage _storage;

    public JsonFileStorageTests()
    {
        // 為每個測試建立獨立的臨時目錄
        _testDataDirectory = Path.Combine(Path.GetTempPath(), $"OrderLunchWeb_Test_{Guid.NewGuid()}");
        _testFileName = "test_stores.json";
        _storage = new JsonFileStorage(_testDataDirectory, _testFileName);
    }

    public void Dispose()
    {
        // 清理測試資料
        if (Directory.Exists(_testDataDirectory))
        {
            Directory.Delete(_testDataDirectory, true);
        }
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task Constructor_ShouldCreateDataDirectoryAndFile_WhenNotExists()
    {
        // Arrange & Act
        // (已在建構函式中執行)
        await Task.CompletedTask;

        // Assert
        Assert.True(Directory.Exists(_testDataDirectory), "資料目錄應該被建立");
        Assert.True(File.Exists(Path.Combine(_testDataDirectory, _testFileName)), "JSON 檔案應該被建立");
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task Constructor_ShouldInitializeEmptyJsonArray_WhenFileNotExists()
    {
        // Arrange
        var filePath = Path.Combine(_testDataDirectory, _testFileName);

        // Act
        var fileContent = await File.ReadAllTextAsync(filePath, Encoding.UTF8);
        var stores = JsonSerializer.Deserialize<List<Store>>(fileContent);

        // Assert
        Assert.NotNull(stores);
        Assert.Empty(stores);
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task AddAsync_ShouldAddStoreWithAutoIncrementId_WhenStoreIsValid()
    {
        // Arrange
        var store = new Store
        {
            Name = "測試便當店",
            Address = "台北市中正區羅斯福路一段100號",
            PhoneType = PhoneType.Mobile,
            Phone = "0912345678",
            BusinessHours = "週一至週五 11:00-14:00",
            MenuItems = new List<MenuItem>
            {
                new MenuItem { Name = "排骨便當", Price = 80, Description = "好吃的排骨" }
            }
        };

        // Act
        await _storage.AddAsync(store);
        var allStores = await _storage.GetAllAsync();

        // Assert
        Assert.Single(allStores);
        var savedStore = allStores[0];
        Assert.Equal(1, savedStore.Id); // 第一筆資料 ID 應為 1
        Assert.Equal("測試便當店", savedStore.Name);
        Assert.NotEqual(default, savedStore.CreatedAt);
        Assert.NotEqual(default, savedStore.UpdatedAt);
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task AddAsync_ShouldAutoIncrementId_WhenMultipleStoresAdded()
    {
        // Arrange
        var store1 = CreateTestStore("店家1", "0912345678");
        var store2 = CreateTestStore("店家2", "0923456789");
        var store3 = CreateTestStore("店家3", "0934567890");

        // Act
        await _storage.AddAsync(store1);
        await _storage.AddAsync(store2);
        await _storage.AddAsync(store3);
        var allStores = await _storage.GetAllAsync();

        // Assert
        Assert.Equal(3, allStores.Count);
        Assert.Equal(1, allStores[0].Id);
        Assert.Equal(2, allStores[1].Id);
        Assert.Equal(3, allStores[2].Id);
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task AddAsync_ShouldNotReuseDeletedIds_WhenStoreIsDeleted()
    {
        // Arrange
        var store1 = CreateTestStore("店家1", "0912345678");
        var store2 = CreateTestStore("店家2", "0923456789");
        await _storage.AddAsync(store1);
        await _storage.AddAsync(store2);

        // Act
        await _storage.DeleteAsync(1); // 刪除 ID=1
        var store3 = CreateTestStore("店家3", "0934567890");
        await _storage.AddAsync(store3);
        var allStores = await _storage.GetAllAsync();

        // Assert
        Assert.Equal(2, allStores.Count); // 只剩 2 筆
        Assert.DoesNotContain(allStores, s => s.Id == 1); // ID 1 已刪除
        Assert.Equal(3, allStores.Max(s => s.Id)); // 新增的 ID 為 3，不重複使用 1
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task AddAsync_ShouldSaveFileWithUtf8Encoding_WhenStoreContainsChineseCharacters()
    {
        // Arrange
        var store = CreateTestStore("好吃便當店", "0912345678");
        store.MenuItems[0].Description = "香酥排骨配上三菜一飯，美味可口！";

        // Act
        await _storage.AddAsync(store);
        var filePath = Path.Combine(_testDataDirectory, _testFileName);
        var fileContent = await File.ReadAllTextAsync(filePath, Encoding.UTF8);

        // Assert
        Assert.Contains("好吃便當店", fileContent);
        Assert.Contains("香酥排骨配上三菜一飯，美味可口！", fileContent);
        Assert.DoesNotContain("\\u", fileContent); // 確認中文沒有被轉義為 Unicode
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task GetByIdAsync_ShouldReturnStore_WhenStoreExists()
    {
        // Arrange
        var store = CreateTestStore("測試店家", "0912345678");
        await _storage.AddAsync(store);

        // Act
        var result = await _storage.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("測試店家", result.Name);
        Assert.Equal("0912345678", result.Phone);
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task GetByIdAsync_ShouldReturnNull_WhenStoreDoesNotExist()
    {
        // Act
        var result = await _storage.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoStores()
    {
        // Act
        var result = await _storage.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task GetAllAsync_ShouldReturnAllStores_WhenMultipleStoresExist()
    {
        // Arrange
        await _storage.AddAsync(CreateTestStore("店家1", "0912345678"));
        await _storage.AddAsync(CreateTestStore("店家2", "0923456789"));
        await _storage.AddAsync(CreateTestStore("店家3", "0934567890"));

        // Act
        var result = await _storage.GetAllAsync();

        // Assert
        Assert.Equal(3, result.Count);
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task UpdateAsync_ShouldUpdateStoreAndModifiedTime_WhenStoreExists()
    {
        // Arrange
        var store = CreateTestStore("原始店家", "0912345678");
        await _storage.AddAsync(store);
        var originalUpdatedAt = store.UpdatedAt;

        await Task.Delay(100); // 確保時間差異

        // Act
        var updatedStore = await _storage.GetByIdAsync(1);
        updatedStore!.Name = "更新後店家";
        await _storage.UpdateAsync(updatedStore);
        var result = await _storage.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("更新後店家", result.Name);
        Assert.True(result.UpdatedAt > originalUpdatedAt, "UpdatedAt 應該被更新");
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task DeleteAsync_ShouldRemoveStore_WhenStoreExists()
    {
        // Arrange
        var store = CreateTestStore("待刪除店家", "0912345678");
        await _storage.AddAsync(store);

        // Act
        var deleteResult = await _storage.DeleteAsync(1);
        var getResult = await _storage.GetByIdAsync(1);

        // Assert
        Assert.True(deleteResult);
        Assert.Null(getResult);
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task DeleteAsync_ShouldReturnFalse_WhenStoreDoesNotExist()
    {
        // Act
        var result = await _storage.DeleteAsync(999);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// 輔助方法：建立測試用的店家物件
    /// </summary>
    private Store CreateTestStore(string name, string phone)
    {
        return new Store
        {
            Name = name,
            Address = "台北市中正區羅斯福路一段100號",
            PhoneType = PhoneType.Mobile,
            Phone = phone,
            BusinessHours = "週一至週五 11:00-14:00",
            MenuItems = new List<MenuItem>
            {
                new MenuItem { Name = "測試便當", Price = 80, Description = "測試描述" }
            }
        };
    }
}
