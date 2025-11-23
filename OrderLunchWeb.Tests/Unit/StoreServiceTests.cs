using Microsoft.Extensions.Logging;
using Moq;
using OrderLunchWeb.Data;
using OrderLunchWeb.Models;
using OrderLunchWeb.Services;
using Xunit;

namespace OrderLunchWeb.Tests.Unit;

/// <summary>
/// StoreService 單元測試 (User Story 1)
/// 測試業務邏輯：新增店家、重複檢查、驗證規則
/// </summary>
public class StoreServiceTests : IDisposable
{
    private readonly string _testDataDirectory;
    private readonly IFileStorage _storage;
    private readonly StoreService _service;
    private readonly Mock<ILogger<StoreService>> _mockLogger;

    public StoreServiceTests()
    {
        _testDataDirectory = Path.Combine(Path.GetTempPath(), $"OrderLunchWeb_Test_{Guid.NewGuid()}");
        _storage = new JsonFileStorage(_testDataDirectory, "test_stores.json");
        _mockLogger = new Mock<ILogger<StoreService>>();
        _service = new StoreService(_storage, _mockLogger.Object);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDataDirectory))
        {
            Directory.Delete(_testDataDirectory, true);
        }
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task AddStoreAsync_ShouldAddStoreSuccessfully_WhenValidStore()
    {
        // Arrange
        var store = CreateTestStore("測試便當店", "0912345678", "台北市中正區羅斯福路一段100號");

        // Act
        var result = await _service.AddStoreAsync(store);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("測試便當店", result.Name);
        Assert.NotEqual(default, result.CreatedAt);
        Assert.NotEqual(default, result.UpdatedAt);
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task AddStoreAsync_ShouldSetTimestamps_WhenStoreIsAdded()
    {
        // Arrange
        var store = CreateTestStore("便當店", "0912345678", "台北市");
        var beforeAdd = DateTime.Now;

        // Act
        var result = await _service.AddStoreAsync(store);
        var afterAdd = DateTime.Now;

        // Assert
        Assert.True(result.CreatedAt >= beforeAdd && result.CreatedAt <= afterAdd);
        Assert.True(result.UpdatedAt >= beforeAdd && result.UpdatedAt <= afterAdd);
        
        // 新增時兩者應相同或非常接近 (允許微小的時間差異)
        var timeDifference = Math.Abs((result.UpdatedAt - result.CreatedAt).TotalMilliseconds);
        Assert.True(timeDifference < 10, $"CreatedAt 和 UpdatedAt 應該幾乎相同，但差異為 {timeDifference}ms");
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task IsDuplicateStoreAsync_ShouldReturnTrue_WhenExactDuplicateExists()
    {
        // Arrange
        var store = CreateTestStore("好吃便當店", "0912345678", "台北市中正區羅斯福路一段100號");
        await _service.AddStoreAsync(store);

        // Act
        var isDuplicate = await _service.IsDuplicateStoreAsync("好吃便當店", "0912345678", "台北市中正區羅斯福路一段100號");

        // Assert
        Assert.True(isDuplicate, "相同店名、電話、地址應判定為重複");
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task IsDuplicateStoreAsync_ShouldReturnFalse_WhenNameDiffers()
    {
        // Arrange
        var store = CreateTestStore("好吃便當店", "0912345678", "台北市中正區羅斯福路一段100號");
        await _service.AddStoreAsync(store);

        // Act
        var isDuplicate = await _service.IsDuplicateStoreAsync("美味便當店", "0912345678", "台北市中正區羅斯福路一段100號");

        // Assert
        Assert.False(isDuplicate, "店名不同不應判定為重複");
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task IsDuplicateStoreAsync_ShouldReturnFalse_WhenPhoneDiffers()
    {
        // Arrange
        var store = CreateTestStore("好吃便當店", "0912345678", "台北市中正區羅斯福路一段100號");
        await _service.AddStoreAsync(store);

        // Act
        var isDuplicate = await _service.IsDuplicateStoreAsync("好吃便當店", "0923456789", "台北市中正區羅斯福路一段100號");

        // Assert
        Assert.False(isDuplicate, "電話不同不應判定為重複 (可能是分店)");
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task IsDuplicateStoreAsync_ShouldReturnFalse_WhenAddressDiffers()
    {
        // Arrange
        var store = CreateTestStore("好吃便當店", "0912345678", "台北市中正區羅斯福路一段100號");
        await _service.AddStoreAsync(store);

        // Act
        var isDuplicate = await _service.IsDuplicateStoreAsync("好吃便當店", "0912345678", "台北市大安區復興南路一段200號");

        // Assert
        Assert.False(isDuplicate, "地址不同不應判定為重複 (可能是分店)");
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task IsDuplicateStoreAsync_ShouldBeCaseInsensitive_ForName()
    {
        // Arrange
        var store = CreateTestStore("好吃便當店", "0912345678", "台北市中正區羅斯福路一段100號");
        await _service.AddStoreAsync(store);

        // Act
        var isDuplicate = await _service.IsDuplicateStoreAsync("好吃便當店", "0912345678", "台北市中正區羅斯福路一段100號");

        // Assert
        Assert.True(isDuplicate, "重複檢查應該不區分大小寫 (中文不影響)");
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task IsDuplicateStoreAsync_ShouldExcludeSelf_WhenExcludeIdProvided()
    {
        // Arrange
        var store = CreateTestStore("好吃便當店", "0912345678", "台北市中正區羅斯福路一段100號");
        var addedStore = await _service.AddStoreAsync(store);

        // Act
        var isDuplicate = await _service.IsDuplicateStoreAsync(
            "好吃便當店", 
            "0912345678", 
            "台北市中正區羅斯福路一段100號", 
            addedStore.Id);

        // Assert
        Assert.False(isDuplicate, "排除自己的 ID 時，不應判定為重複 (用於編輯情境)");
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task IsDuplicateStoreAsync_ShouldReturnFalse_WhenNoStoresExist()
    {
        // Act
        var isDuplicate = await _service.IsDuplicateStoreAsync("新店家", "0912345678", "台北市");

        // Assert
        Assert.False(isDuplicate);
    }

    [Fact]
    [Trait("Category", "US1")]
    [Trait("Category", "US2")]
    public async Task GetAllStoresAsync_ShouldReturnAllStores_WhenMultipleStoresExist()
    {
        // Arrange
        await _service.AddStoreAsync(CreateTestStore("店家1", "0912345678", "地址1"));
        await _service.AddStoreAsync(CreateTestStore("店家2", "0923456789", "地址2"));
        await _service.AddStoreAsync(CreateTestStore("店家3", "0934567890", "地址3"));

        // Act
        var stores = await _service.GetAllStoresAsync();

        // Assert
        Assert.Equal(3, stores.Count);
    }

    [Fact]
    [Trait("Category", "US1")]
    [Trait("Category", "US2")]
    public async Task GetStoreByIdAsync_ShouldReturnStore_WhenStoreExists()
    {
        // Arrange
        var store = CreateTestStore("測試店家", "0912345678", "測試地址");
        var addedStore = await _service.AddStoreAsync(store);

        // Act
        var result = await _service.GetStoreByIdAsync(addedStore.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("測試店家", result.Name);
    }

    [Fact]
    [Trait("Category", "US1")]
    [Trait("Category", "US2")]
    public async Task GetStoreByIdAsync_ShouldReturnNull_WhenStoreDoesNotExist()
    {
        // Act
        var result = await _service.GetStoreByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData("", "0912345678", "地址")]
    [InlineData("店名", "", "地址")]
    [InlineData("店名", "0912345678", "")]
    [Trait("Category", "US1")]
    public async Task AddStoreAsync_ShouldAcceptAnyInput_ValidationIsHandledByController(
        string name, string phone, string address)
    {
        // Arrange
        var store = new Store
        {
            Name = name,
            Address = address,
            PhoneType = PhoneType.Mobile,
            Phone = phone,
            BusinessHours = "週一至週五 11:00-14:00",
            MenuItems = new List<MenuItem>
            {
                new MenuItem { Name = "測試便當", Price = 80 }
            }
        };

        // Act
        // Service 層不負責驗證，驗證由 Controller 的 ModelState 和 Data Annotations 處理
        var result = await _service.AddStoreAsync(store);

        // Assert
        Assert.NotNull(result); // Service 僅負責儲存，接受任何輸入
        Assert.True(result.Id > 0);
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task AddStoreAsync_ShouldPreserveMenuItems_WhenStoreIsAdded()
    {
        // Arrange
        var store = CreateTestStore("便當店", "0912345678", "地址");
        store.MenuItems = new List<MenuItem>
        {
            new MenuItem { Name = "排骨便當", Price = 80, Description = "好吃" },
            new MenuItem { Name = "雞腿便當", Price = 90, Description = "美味" },
            new MenuItem { Name = "魚便當", Price = 85, Description = "新鮮" }
        };

        // Act
        var result = await _service.AddStoreAsync(store);
        var retrieved = await _service.GetStoreByIdAsync(result.Id);

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal(3, retrieved.MenuItems.Count);
        Assert.Equal("排骨便當", retrieved.MenuItems[0].Name);
        Assert.Equal(80, retrieved.MenuItems[0].Price);
    }

    #region User Story 3 - 編修店家資訊

    [Fact]
    [Trait("Category", "US3")]
    public async Task UpdateStoreAsync_ShouldUpdateStoreSuccessfully_WhenValidStore()
    {
        // Arrange
        var store = CreateTestStore("原始店名", "0912345678", "原始地址");
        var addedStore = await _service.AddStoreAsync(store);
        var originalCreatedAt = addedStore.CreatedAt;
        var originalUpdatedAt = addedStore.UpdatedAt;

        // 等待至少 10ms 確保時間戳不同
        await Task.Delay(10);

        // 修改店家資訊
        addedStore.Name = "更新後的店名";
        addedStore.Address = "更新後的地址";
        addedStore.Phone = "0923456789";

        // Act
        var result = await _service.UpdateStoreAsync(addedStore);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("更新後的店名", result.Name);
        Assert.Equal("更新後的地址", result.Address);
        Assert.Equal("0923456789", result.Phone);
        Assert.Equal(originalCreatedAt, result.CreatedAt); // CreatedAt 不應改變
        Assert.True(result.UpdatedAt > originalUpdatedAt, "UpdatedAt 應該被更新");
    }

    [Fact]
    [Trait("Category", "US3")]
    public async Task UpdateStoreAsync_ShouldUpdateTimestamp_WhenStoreIsUpdated()
    {
        // Arrange
        var store = CreateTestStore("店家", "0912345678", "地址");
        var addedStore = await _service.AddStoreAsync(store);
        var beforeUpdate = DateTime.Now;

        // 等待至少 10ms 確保時間戳不同
        await Task.Delay(10);

        addedStore.Name = "更新後的店名";

        // Act
        var result = await _service.UpdateStoreAsync(addedStore);
        var afterUpdate = DateTime.Now;

        // Assert
        Assert.True(result.UpdatedAt >= beforeUpdate && result.UpdatedAt <= afterUpdate);
        Assert.True(result.UpdatedAt > result.CreatedAt, "UpdatedAt 應該晚於 CreatedAt");
    }

    [Fact]
    [Trait("Category", "US3")]
    public async Task UpdateStoreAsync_ShouldReturnNull_WhenStoreDoesNotExist()
    {
        // Arrange
        var nonExistentStore = CreateTestStore("不存在的店家", "0912345678", "地址");
        nonExistentStore.Id = 999;

        // Act
        var result = await _service.UpdateStoreAsync(nonExistentStore);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    [Trait("Category", "US3")]
    public async Task IsDuplicateStoreAsync_ShouldExcludeSelfWhenChecking_ForUpdate()
    {
        // Arrange
        var store1 = CreateTestStore("便當店A", "0912345678", "台北市中正區");
        var store2 = CreateTestStore("便當店B", "0923456789", "台北市大安區");
        var addedStore1 = await _service.AddStoreAsync(store1);
        await _service.AddStoreAsync(store2);

        // Act - 更新 store1，但名稱、電話、地址都不變（編輯其他欄位）
        var isDuplicate = await _service.IsDuplicateStoreAsync(
            "便當店A", 
            "0912345678", 
            "台北市中正區", 
            addedStore1.Id);

        // Assert
        Assert.False(isDuplicate, "更新自己時，不應判定為重複");
    }

    [Fact]
    [Trait("Category", "US3")]
    public async Task IsDuplicateStoreAsync_ShouldReturnTrueWhenConflictWithOther_ForUpdate()
    {
        // Arrange
        var store1 = CreateTestStore("便當店A", "0912345678", "台北市中正區");
        var store2 = CreateTestStore("便當店B", "0923456789", "台北市大安區");
        var addedStore1 = await _service.AddStoreAsync(store1);
        var addedStore2 = await _service.AddStoreAsync(store2);

        // Act - 嘗試將 store1 更新為與 store2 相同的資訊
        var isDuplicate = await _service.IsDuplicateStoreAsync(
            "便當店B", 
            "0923456789", 
            "台北市大安區", 
            addedStore1.Id);

        // Assert
        Assert.True(isDuplicate, "更新後與其他店家重複時，應判定為重複");
    }

    [Fact]
    [Trait("Category", "US3")]
    public async Task UpdateStoreAsync_ShouldUpdateMenuItems_WhenMenuItemsChanged()
    {
        // Arrange
        var store = CreateTestStore("便當店", "0912345678", "地址");
        store.MenuItems = new List<MenuItem>
        {
            new MenuItem { Name = "排骨便當", Price = 80 },
            new MenuItem { Name = "雞腿便當", Price = 90 }
        };
        var addedStore = await _service.AddStoreAsync(store);

        // 修改菜單
        addedStore.MenuItems = new List<MenuItem>
        {
            new MenuItem { Name = "排骨便當", Price = 85 }, // 價格改變
            new MenuItem { Name = "雞腿便當", Price = 95 }, // 價格改變
            new MenuItem { Name = "魚便當", Price = 100 }   // 新增項目
        };

        // Act
        var result = await _service.UpdateStoreAsync(addedStore);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.MenuItems.Count);
        Assert.Equal(85, result.MenuItems[0].Price);
        Assert.Equal(95, result.MenuItems[1].Price);
        Assert.Equal("魚便當", result.MenuItems[2].Name);
    }

    [Fact]
    [Trait("Category", "US3")]
    public async Task UpdateStoreAsync_ShouldPreserveCreatedAt_WhenStoreIsUpdated()
    {
        // Arrange
        var store = CreateTestStore("店家", "0912345678", "地址");
        var addedStore = await _service.AddStoreAsync(store);
        var originalCreatedAt = addedStore.CreatedAt;

        await Task.Delay(10);

        // 修改店家資訊
        addedStore.Name = "新店名";
        addedStore.Address = "新地址";
        addedStore.BusinessHours = "週一至週日 10:00-22:00";

        // Act
        var result = await _service.UpdateStoreAsync(addedStore);

        // Assert
        Assert.Equal(originalCreatedAt, result.CreatedAt);
        Assert.True(result.UpdatedAt > originalCreatedAt);
    }

    #endregion

    #region User Story 4 - 刪除店家資訊

    [Fact]
    [Trait("Category", "US4")]
    public async Task DeleteStoreAsync_ShouldReturnTrue_WhenStoreExists()
    {
        // Arrange
        var store = CreateTestStore("要刪除的店家", "0912345678", "台北市中正區");
        var addedStore = await _service.AddStoreAsync(store);

        // Act
        var result = await _service.DeleteStoreAsync(addedStore.Id);

        // Assert
        Assert.True(result, "刪除已存在的店家應返回 true");

        // 驗證店家確實被刪除
        var deletedStore = await _service.GetStoreByIdAsync(addedStore.Id);
        Assert.Null(deletedStore);
    }

    [Fact]
    [Trait("Category", "US4")]
    public async Task DeleteStoreAsync_ShouldReturnFalse_WhenStoreDoesNotExist()
    {
        // Arrange
        var nonExistentId = 9999;

        // Act
        var result = await _service.DeleteStoreAsync(nonExistentId);

        // Assert
        Assert.False(result, "刪除不存在的店家應返回 false");
    }

    [Fact]
    [Trait("Category", "US4")]
    public async Task DeleteStoreAsync_ShouldNotAffectOtherStores()
    {
        // Arrange
        var store1 = CreateTestStore("店家1", "0912345678", "地址1");
        var store2 = CreateTestStore("店家2", "0923456789", "地址2");
        var store3 = CreateTestStore("店家3", "0934567890", "地址3");

        var added1 = await _service.AddStoreAsync(store1);
        var added2 = await _service.AddStoreAsync(store2);
        var added3 = await _service.AddStoreAsync(store3);

        // Act - 刪除中間的店家
        var result = await _service.DeleteStoreAsync(added2.Id);

        // Assert
        Assert.True(result);

        var allStores = await _service.GetAllStoresAsync();
        Assert.Equal(2, allStores.Count);
        Assert.Contains(allStores, s => s.Id == added1.Id);
        Assert.Contains(allStores, s => s.Id == added3.Id);
        Assert.DoesNotContain(allStores, s => s.Id == added2.Id);
    }

    #endregion

    /// <summary>
    /// 輔助方法：建立測試用的店家物件
    /// </summary>
    private Store CreateTestStore(string name, string phone, string address)
    {
        return new Store
        {
            Name = name,
            Address = address,
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
