using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using OrderLunchWeb.Controllers;
using OrderLunchWeb.Data;
using OrderLunchWeb.Models;
using OrderLunchWeb.Services;
using Xunit;

namespace OrderLunchWeb.Tests.Integration;

/// <summary>
/// StoreController 整合測試 (User Story 1)
/// 測試 Controller Actions: GET Create, POST Create (成功、驗證失敗、重複店家)
/// </summary>
public class StoreControllerTests : IDisposable
{
    private readonly string _testDataDirectory;
    private readonly IFileStorage _storage;
    private readonly IStoreService _service;
    private readonly StoreController _controller;

    public StoreControllerTests()
    {
        _testDataDirectory = Path.Combine(Path.GetTempPath(), $"OrderLunchWeb_Test_{Guid.NewGuid()}");
        _storage = new JsonFileStorage(_testDataDirectory, "test_stores.json");
        
        // 建立真實的 Logger (整合測試使用實際相依性)
        var serviceProvider = new ServiceCollection()
            .AddLogging()
            .BuildServiceProvider();
        
        var logger = serviceProvider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<StoreService>>();
        _service = new StoreService(_storage, logger);
        
        var controllerLogger = serviceProvider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<StoreController>>();
        _controller = new StoreController(_service, controllerLogger);
        
        // 初始化 TempData
        _controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(
            new Microsoft.AspNetCore.Http.DefaultHttpContext(),
            Moq.Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDataDirectory))
        {
            Directory.Delete(_testDataDirectory, true);
        }
    }

    #region GET Create Tests

    [Fact]
    [Trait("Category", "US1")]
    public void GetCreate_ShouldReturnViewResult_WithEmptyStoreModel()
    {
        // Act
        var result = _controller.Create();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Null(viewResult.ViewName); // 使用預設 View 名稱
        Assert.NotNull(viewResult.Model);
        Assert.IsType<Store>(viewResult.Model);
    }

    [Fact]
    [Trait("Category", "US1")]
    public void GetCreate_ShouldReturnStoreWithEmptyMenuItems()
    {
        // Act
        var result = _controller.Create();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Store>(viewResult.Model);
        Assert.NotNull(model.MenuItems);
        Assert.Single(model.MenuItems); // 應預設包含一個空菜單項目
    }

    #endregion

    #region POST Create - Success Tests

    [Fact]
    [Trait("Category", "US1")]
    public async Task PostCreate_ShouldRedirectToIndex_WhenModelIsValid()
    {
        // Arrange
        var store = CreateValidStore("測試便當店", "0912345678", "台北市中正區羅斯福路一段100號");
        TriggerModelValidation(store);

        // Act
        var result = await _controller.Create(store);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Null(redirectResult.ControllerName); // 同一個 Controller
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task PostCreate_ShouldSaveStoreToStorage_WhenModelIsValid()
    {
        // Arrange
        var store = CreateValidStore("好吃便當店", "0912345678", "台北市");
        TriggerModelValidation(store);

        // Act
        await _controller.Create(store);
        var allStores = await _service.GetAllStoresAsync();

        // Assert
        Assert.Single(allStores);
        Assert.Equal("好吃便當店", allStores[0].Name);
        Assert.Equal("0912345678", allStores[0].Phone);
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task PostCreate_ShouldSetSuccessMessage_WhenStoreIsCreated()
    {
        // Arrange
        var store = CreateValidStore("測試店家", "0912345678", "地址");
        TriggerModelValidation(store);

        // Act
        await _controller.Create(store);

        // Assert
        Assert.True(_controller.TempData.ContainsKey("SuccessMessage"));
        var message = _controller.TempData["SuccessMessage"]?.ToString();
        Assert.Contains("成功", message);
    }

    #endregion

    #region POST Create - Validation Failure Tests

    [Fact]
    [Trait("Category", "US1")]
    public async Task PostCreate_ShouldReturnView_WhenModelStateIsInvalid()
    {
        // Arrange
        var store = CreateValidStore("", "0912345678", "地址"); // 店名為空
        _controller.ModelState.AddModelError("Name", "店家名稱為必填欄位");

        // Act
        var result = await _controller.Create(store);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(_controller.ModelState.IsValid);
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task PostCreate_ShouldNotSaveStore_WhenModelStateIsInvalid()
    {
        // Arrange
        var store = CreateValidStore("", "", ""); // 所有必填欄位為空
        _controller.ModelState.AddModelError("Name", "必填");
        _controller.ModelState.AddModelError("Phone", "必填");
        _controller.ModelState.AddModelError("Address", "必填");

        // Act
        await _controller.Create(store);
        var allStores = await _service.GetAllStoresAsync();

        // Assert
        Assert.Empty(allStores); // 不應儲存無效資料
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task PostCreate_ShouldReturnViewWithModel_WhenValidationFails()
    {
        // Arrange
        var store = CreateValidStore("測試", "abc123", "地址"); // 電話包含非數字
        _controller.ModelState.AddModelError("Phone", "電話號碼僅能輸入數字");

        // Act
        var result = await _controller.Create(store);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Store>(viewResult.Model);
        Assert.Equal("測試", model.Name);
    }

    [Theory]
    [InlineData("", "0912345678", "地址", "Name")] // 店名為空
    [InlineData("店名", "", "地址", "Phone")] // 電話為空
    [InlineData("店名", "0912345678", "", "Address")] // 地址為空
    [InlineData("店名", "0912345678", "地址", "BusinessHours")] // 營業時間為空
    [Trait("Category", "US1")]
    public async Task PostCreate_ShouldFailValidation_WhenRequiredFieldMissing(
        string name, string phone, string address, string expectedErrorField)
    {
        // Arrange
        var store = new Store
        {
            Name = name,
            Address = address,
            PhoneType = PhoneType.Mobile,
            Phone = phone,
            BusinessHours = expectedErrorField == "BusinessHours" ? "" : "週一至週五 11:00-14:00",
            MenuItems = new List<MenuItem>
            {
                new MenuItem { Name = "測試", Price = 80 }
            }
        };
        
        // 模擬 Model Validation
        var context = new System.ComponentModel.DataAnnotations.ValidationContext(store);
        var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        System.ComponentModel.DataAnnotations.Validator.TryValidateObject(store, context, results, true);

        foreach (var validationResult in results)
        {
            foreach (var memberName in validationResult.MemberNames)
            {
                _controller.ModelState.AddModelError(memberName, validationResult.ErrorMessage ?? "驗證失敗");
            }
        }

        // Act
        var result = await _controller.Create(store);

        // Assert
        Assert.False(_controller.ModelState.IsValid);
        var viewResult = Assert.IsType<ViewResult>(result);
    }

    #endregion

    #region POST Create - Duplicate Store Tests

    [Fact]
    [Trait("Category", "US1")]
    public async Task PostCreate_ShouldReturnViewWithError_WhenDuplicateStoreExists()
    {
        // Arrange
        var existingStore = CreateValidStore("好吃便當店", "0912345678", "台北市中正區羅斯福路一段100號");
        await _service.AddStoreAsync(existingStore);

        var duplicateStore = CreateValidStore("好吃便當店", "0912345678", "台北市中正區羅斯福路一段100號");

        // Act
        var result = await _controller.Create(duplicateStore);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(_controller.ModelState.IsValid);
        Assert.True(_controller.ModelState.ContainsKey("") || _controller.ModelState.ContainsKey("DuplicateStore"));
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task PostCreate_ShouldNotCreateDuplicateStore_WhenAllThreeFieldsMatch()
    {
        // Arrange
        var store1 = CreateValidStore("便當店", "0912345678", "台北市");
        await _service.AddStoreAsync(store1);

        var store2 = CreateValidStore("便當店", "0912345678", "台北市");

        // Act
        await _controller.Create(store2);
        var allStores = await _service.GetAllStoresAsync();

        // Assert
        Assert.Single(allStores); // 應該只有一筆，重複的不應被新增
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task PostCreate_ShouldAllowStore_WhenOnlyNameMatches()
    {
        // Arrange
        var store1 = CreateValidStore("便當店", "0912345678", "台北市");
        await _service.AddStoreAsync(store1);

        var store2 = CreateValidStore("便當店", "0923456789", "新北市"); // 不同電話和地址
        TriggerModelValidation(store2);

        // Act
        var result = await _controller.Create(store2);
        var allStores = await _service.GetAllStoresAsync();

        // Assert
        Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(2, allStores.Count); // 分店情況，應允許新增
    }

    #endregion

    #region POST Create - Menu Items Validation Tests

    [Fact]
    [Trait("Category", "US1")]
    public async Task PostCreate_ShouldRequireAtLeastOneMenuItem()
    {
        // Arrange
        var store = CreateValidStore("測試店", "0912345678", "地址");
        store.MenuItems = new List<MenuItem>(); // 空菜單

        // 模擬驗證
        var context = new System.ComponentModel.DataAnnotations.ValidationContext(store);
        var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        System.ComponentModel.DataAnnotations.Validator.TryValidateObject(store, context, results, true);

        foreach (var validationResult in results)
        {
            foreach (var memberName in validationResult.MemberNames)
            {
                _controller.ModelState.AddModelError(memberName, validationResult.ErrorMessage ?? "驗證失敗");
            }
        }

        // Act
        var result = await _controller.Create(store);

        // Assert
        Assert.False(_controller.ModelState.IsValid);
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task PostCreate_ShouldAcceptUpTo20MenuItems()
    {
        // Arrange
        var store = CreateValidStore("測試店", "0912345678", "地址");
        store.MenuItems = Enumerable.Range(1, 20)
            .Select(i => new MenuItem { Name = $"菜單{i}", Price = 80 + i, Description = "描述" })
            .ToList();
        TriggerModelValidation(store);

        // Act
        var result = await _controller.Create(store);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        var allStores = await _service.GetAllStoresAsync();
        Assert.Single(allStores);
        Assert.Equal(20, allStores[0].MenuItems.Count);
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task PostCreate_ShouldRejectMoreThan20MenuItems()
    {
        // Arrange
        var store = CreateValidStore("測試店", "0912345678", "地址");
        store.MenuItems = Enumerable.Range(1, 21) // 21 筆，超過限制
            .Select(i => new MenuItem { Name = $"菜單{i}", Price = 80, Description = "描述" })
            .ToList();

        // 模擬驗證
        var context = new System.ComponentModel.DataAnnotations.ValidationContext(store);
        var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        System.ComponentModel.DataAnnotations.Validator.TryValidateObject(store, context, results, true);

        foreach (var validationResult in results)
        {
            foreach (var memberName in validationResult.MemberNames)
            {
                _controller.ModelState.AddModelError(memberName, validationResult.ErrorMessage ?? "驗證失敗");
            }
        }

        // Act
        var result = await _controller.Create(store);

        // Assert
        Assert.False(_controller.ModelState.IsValid);
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task PostCreate_ShouldAcceptMenuItemWithZeroPrice()
    {
        // Arrange
        var store = CreateValidStore("測試店", "0912345678", "地址");
        store.MenuItems[0].Price = 0; // 價格為 0 (免費品項)
        TriggerModelValidation(store);

        // Act
        var result = await _controller.Create(store);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        var allStores = await _service.GetAllStoresAsync();
        Assert.Equal(0, allStores[0].MenuItems[0].Price);
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// 建立有效的店家物件用於測試
    /// </summary>
    private Store CreateValidStore(string name, string phone, string address)
    {
        return new Store
        {
            Name = name,
            Address = address,
            PhoneType = PhoneType.Mobile,
            Phone = phone,
            BusinessHours = "週一至週五 11:00-14:00, 17:00-21:00",
            MenuItems = new List<MenuItem>
            {
                new MenuItem 
                { 
                    Name = "排骨便當", 
                    Price = 80, 
                    Description = "香酥排骨配上三菜一飯" 
                }
            }
        };
    }

    /// <summary>
    /// 手動觸發模型驗證 (因為單元測試中不會自動執行)
    /// </summary>
    private void TriggerModelValidation(object model)
    {
        var context = new System.ComponentModel.DataAnnotations.ValidationContext(model, null, null);
        var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        System.ComponentModel.DataAnnotations.Validator.TryValidateObject(model, context, results, true);

        foreach (var validationResult in results)
        {
            foreach (var memberName in validationResult.MemberNames)
            {
                _controller.ModelState.AddModelError(memberName, validationResult.ErrorMessage ?? "驗證失敗");
            }
        }

        // 驗證巢狀物件 (MenuItems)
        if (model is Store store && store.MenuItems != null)
        {
            for (int i = 0; i < store.MenuItems.Count; i++)
            {
                var menuItemContext = new System.ComponentModel.DataAnnotations.ValidationContext(store.MenuItems[i], null, null);
                var menuItemResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                System.ComponentModel.DataAnnotations.Validator.TryValidateObject(store.MenuItems[i], menuItemContext, menuItemResults, true);

                foreach (var validationResult in menuItemResults)
                {
                    foreach (var memberName in validationResult.MemberNames)
                    {
                        _controller.ModelState.AddModelError($"MenuItems[{i}].{memberName}", validationResult.ErrorMessage ?? "驗證失敗");
                    }
                }
            }
        }
    }

    #endregion

    #region POST Create - Anti-Duplicate Submission Tests (T024-1)

    [Fact]
    [Trait("Category", "US1")]
    public async Task PostCreate_ShouldPreventDuplicateSubmission_WhenSubmittedTwiceQuickly()
    {
        // Arrange
        var store = CreateValidStore("測試店", "0912345678", "地址");
        TriggerModelValidation(store);

        // Act
        // 模擬快速連續提交兩次
        var firstResult = await _controller.Create(store);
        var secondResult = await _controller.Create(store);

        // Assert
        var allStores = await _service.GetAllStoresAsync();
        Assert.Single(allStores); // 只應儲存一筆

        // 第一次應成功 (Redirect)
        Assert.IsType<RedirectToActionResult>(firstResult);

        // 第二次應失敗 (View with error) - 因為重複檢查會發現相同店家
        var secondViewResult = Assert.IsType<ViewResult>(secondResult);
        Assert.False(_controller.ModelState.IsValid);
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task PostCreate_ShouldUsePRGPattern_ToPreventFormResubmission()
    {
        // Arrange
        var store = CreateValidStore("測試店", "0912345678", "地址");
        TriggerModelValidation(store);

        // Act
        var result = await _controller.Create(store);

        // Assert
        // 確認使用 Post-Redirect-Get (PRG) 模式
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        
        // 確認 TempData 包含成功訊息 (PRG 模式的一部分)
        Assert.True(_controller.TempData.ContainsKey("SuccessMessage"));
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task PostCreate_ShouldDisplayErrorOnSecondSubmit_WhenStoreAlreadyExists()
    {
        // Arrange
        var store1 = CreateValidStore("好吃店", "0912345678", "台北市");
        var store2 = CreateValidStore("好吃店", "0912345678", "台北市"); // 完全相同

        // Act
        await _controller.Create(store1); // 第一次提交
        var result = await _controller.Create(store2); // 第二次提交相同資料

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(_controller.ModelState.IsValid);
        
        var allStores = await _service.GetAllStoresAsync();
        Assert.Single(allStores); // 確認只有一筆
    }

    #endregion

    #region GET Index Tests (T032 - User Story 2)

    [Fact]
    [Trait("Category", "US2")]
    public async Task GetIndex_ShouldReturnViewResult_WithEmptyList_WhenNoStores()
    {
        // Act
        var result = await _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<Store>>(viewResult.Model);
        Assert.NotNull(model);
        Assert.Empty(model);
    }

    [Fact]
    [Trait("Category", "US2")]
    public async Task GetIndex_ShouldReturnViewResult_WithStoreList_WhenStoresExist()
    {
        // Arrange
        var store1 = CreateValidStore("便當店A", "0912345678", "台北市");
        var store2 = CreateValidStore("便當店B", "0923456789", "新北市");
        var store3 = CreateValidStore("便當店C", "0934567890", "桃園市");

        await _service.AddStoreAsync(store1);
        await _service.AddStoreAsync(store2);
        await _service.AddStoreAsync(store3);

        // Act
        var result = await _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<Store>>(viewResult.Model);
        Assert.NotNull(model);
        Assert.Equal(3, model.Count);
        Assert.Contains(model, s => s.Name == "便當店A");
        Assert.Contains(model, s => s.Name == "便當店B");
        Assert.Contains(model, s => s.Name == "便當店C");
    }

    [Fact]
    [Trait("Category", "US2")]
    public async Task GetIndex_ShouldReturnStoresWithCompleteData_IncludingMenuItems()
    {
        // Arrange
        var store = CreateValidStore("測試餐廳", "0912345678", "測試地址");
        store.MenuItems = new List<MenuItem>
        {
            new MenuItem { Name = "餐點1", Price = 80, Description = "描述1" },
            new MenuItem { Name = "餐點2", Price = 90, Description = "描述2" }
        };
        await _service.AddStoreAsync(store);

        // Act
        var result = await _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<Store>>(viewResult.Model);
        Assert.Single(model);
        Assert.Equal(2, model[0].MenuItems.Count);
    }

    #endregion

    #region GET Details Tests (T032 - User Story 2)

    [Fact]
    [Trait("Category", "US2")]
    public async Task GetDetails_ShouldReturnNotFound_WhenStoreDoesNotExist()
    {
        // Act
        var result = await _controller.Details(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    [Trait("Category", "US2")]
    public async Task GetDetails_ShouldReturnViewResult_WithStore_WhenStoreExists()
    {
        // Arrange
        var store = CreateValidStore("好吃便當店", "0912345678", "台北市中正區羅斯福路一段100號");
        var added = await _service.AddStoreAsync(store);

        // Act
        var result = await _controller.Details(added.Id);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Store>(viewResult.Model);
        Assert.Equal(added.Id, model.Id);
        Assert.Equal("好吃便當店", model.Name);
        Assert.Equal("0912345678", model.Phone);
        Assert.Equal("台北市中正區羅斯福路一段100號", model.Address);
    }

    [Fact]
    [Trait("Category", "US2")]
    public async Task GetDetails_ShouldReturnStoreWithMenuItems_WhenStoreHasMenu()
    {
        // Arrange
        var store = CreateValidStore("美味餐廳", "0923456789", "新北市板橋區");
        store.MenuItems = new List<MenuItem>
        {
            new MenuItem { Name = "牛肉麵", Price = 120, Description = "香濃好吃" },
            new MenuItem { Name = "排骨飯", Price = 90, Description = "肉質鮮嫩" },
            new MenuItem { Name = "雞腿飯", Price = 100, Description = "外酥內嫩" }
        };
        var added = await _service.AddStoreAsync(store);

        // Act
        var result = await _controller.Details(added.Id);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Store>(viewResult.Model);
        Assert.Equal(3, model.MenuItems.Count);
        Assert.Equal("牛肉麵", model.MenuItems[0].Name);
        Assert.Equal(120, model.MenuItems[0].Price);
        Assert.Equal("香濃好吃", model.MenuItems[0].Description);
    }

    [Fact]
    [Trait("Category", "US2")]
    public async Task GetDetails_ShouldReturnStoreWithTimestamps()
    {
        // Arrange
        var store = CreateValidStore("時間測試店", "0912345678", "測試地址");
        var added = await _service.AddStoreAsync(store);

        // Act
        var result = await _controller.Details(added.Id);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Store>(viewResult.Model);
        Assert.NotEqual(default, model.CreatedAt);
        Assert.NotEqual(default, model.UpdatedAt);
    }

    #endregion

    #region GET Edit Tests (T042 - User Story 3)

    [Fact]
    [Trait("Category", "US3")]
    public async Task GetEdit_ShouldReturnNotFound_WhenStoreDoesNotExist()
    {
        // Act
        var result = await _controller.Edit(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    [Trait("Category", "US3")]
    public async Task GetEdit_ShouldReturnViewResult_WithStore_WhenStoreExists()
    {
        // Arrange
        var store = CreateValidStore("測試店家", "0912345678", "測試地址");
        var added = await _service.AddStoreAsync(store);

        // Act
        var result = await _controller.Edit(added.Id);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Store>(viewResult.Model);
        Assert.Equal(added.Id, model.Id);
        Assert.Equal("測試店家", model.Name);
        Assert.Equal("0912345678", model.Phone);
        Assert.Equal("測試地址", model.Address);
    }

    [Fact]
    [Trait("Category", "US3")]
    public async Task GetEdit_ShouldReturnStoreWithMenuItems()
    {
        // Arrange
        var store = CreateValidStore("有菜單的店", "0912345678", "地址");
        store.MenuItems = new List<MenuItem>
        {
            new MenuItem { Name = "套餐A", Price = 100 },
            new MenuItem { Name = "套餐B", Price = 120 }
        };
        var added = await _service.AddStoreAsync(store);

        // Act
        var result = await _controller.Edit(added.Id);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Store>(viewResult.Model);
        Assert.Equal(2, model.MenuItems.Count);
        Assert.Equal("套餐A", model.MenuItems[0].Name);
        Assert.Equal(100, model.MenuItems[0].Price);
    }

    #endregion

    #region POST Edit - Success Tests (T042 - User Story 3)

    [Fact]
    [Trait("Category", "US3")]
    public async Task PostEdit_ShouldRedirectToIndex_WhenUpdateIsSuccessful()
    {
        // Arrange
        var store = CreateValidStore("原始店名", "0912345678", "原始地址");
        var added = await _service.AddStoreAsync(store);

        // 修改店家資訊
        added.Name = "更新後的店名";
        added.Address = "更新後的地址";
        added.Phone = "0923456789";
        TriggerModelValidation(added);

        // Act
        var result = await _controller.Edit(added.Id, added);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
    }

    [Fact]
    [Trait("Category", "US3")]
    public async Task PostEdit_ShouldUpdateStore_WhenModelIsValid()
    {
        // Arrange
        var store = CreateValidStore("舊店名", "0912345678", "舊地址");
        var added = await _service.AddStoreAsync(store);

        // 修改店家資訊
        added.Name = "新店名";
        added.Address = "新地址";
        added.BusinessHours = "週一至週日 10:00-22:00";
        TriggerModelValidation(added);

        // Act
        await _controller.Edit(added.Id, added);

        // Assert - 驗證資料確實被更新
        var updated = await _service.GetStoreByIdAsync(added.Id);
        Assert.NotNull(updated);
        Assert.Equal("新店名", updated.Name);
        Assert.Equal("新地址", updated.Address);
        Assert.Equal("週一至週日 10:00-22:00", updated.BusinessHours);
    }

    [Fact]
    [Trait("Category", "US3")]
    public async Task PostEdit_ShouldSetSuccessMessage_WhenUpdateSucceeds()
    {
        // Arrange
        var store = CreateValidStore("店家", "0912345678", "地址");
        var added = await _service.AddStoreAsync(store);

        added.Name = "新店名";
        TriggerModelValidation(added);

        // Act
        await _controller.Edit(added.Id, added);

        // Assert
        Assert.True(_controller.TempData.ContainsKey("SuccessMessage"));
        Assert.Contains("新店名", _controller.TempData["SuccessMessage"]?.ToString());
    }

    #endregion

    #region POST Edit - Validation Failure Tests (T042 - User Story 3)

    [Fact]
    [Trait("Category", "US3")]
    public async Task PostEdit_ShouldReturnView_WhenModelStateIsInvalid()
    {
        // Arrange
        var store = CreateValidStore("店家", "0912345678", "地址");
        var added = await _service.AddStoreAsync(store);

        // 設定無效資料（空店名）
        added.Name = "";
        _controller.ModelState.AddModelError("Name", "店家名稱為必填欄位");

        // Act
        var result = await _controller.Edit(added.Id, added);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(_controller.ModelState.IsValid);
        Assert.IsType<Store>(viewResult.Model);
    }

    [Fact]
    [Trait("Category", "US3")]
    public async Task PostEdit_ShouldReturnView_WhenPhoneIsInvalid()
    {
        // Arrange
        var store = CreateValidStore("店家", "0912345678", "地址");
        var added = await _service.AddStoreAsync(store);

        // 設定無效的電話號碼（包含非數字字元）
        added.Phone = "0912-345-678";
        TriggerModelValidation(added);

        // Act
        var result = await _controller.Edit(added.Id, added);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(_controller.ModelState.IsValid);
    }

    [Fact]
    [Trait("Category", "US3")]
    public async Task PostEdit_ShouldReturnView_WhenMenuItemsExceedLimit()
    {
        // Arrange
        var store = CreateValidStore("店家", "0912345678", "地址");
        var added = await _service.AddStoreAsync(store);

        // 新增超過 20 個菜單項目
        added.MenuItems = Enumerable.Range(1, 21)
            .Select(i => new MenuItem { Name = $"菜單{i}", Price = 100 })
            .ToList();
        TriggerModelValidation(added);

        // Act
        var result = await _controller.Edit(added.Id, added);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(_controller.ModelState.IsValid);
    }

    #endregion

    #region POST Edit - 404 Tests (T042 - User Story 3)

    [Fact]
    [Trait("Category", "US3")]
    public async Task PostEdit_ShouldReturnNotFound_WhenStoreDoesNotExist()
    {
        // Arrange
        var store = CreateValidStore("不存在的店", "0912345678", "地址");
        store.Id = 999;
        TriggerModelValidation(store);

        // Act
        var result = await _controller.Edit(999, store);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    [Trait("Category", "US3")]
    public async Task PostEdit_ShouldReturnBadRequest_WhenIdMismatch()
    {
        // Arrange
        var store = CreateValidStore("店家", "0912345678", "地址");
        var added = await _service.AddStoreAsync(store);
        TriggerModelValidation(added);

        // Act - URL ID 與 Model ID 不一致
        var result = await _controller.Edit(999, added);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    #endregion

    #region POST Edit - Duplicate Store Tests (T042 - User Story 3)

    [Fact]
    [Trait("Category", "US3")]
    public async Task PostEdit_ShouldReturnView_WhenUpdatingToDuplicateStore()
    {
        // Arrange - 新增兩家不同的店
        var store1 = CreateValidStore("便當店A", "0912345678", "台北市中正區");
        var store2 = CreateValidStore("便當店B", "0923456789", "台北市大安區");
        var added1 = await _service.AddStoreAsync(store1);
        var added2 = await _service.AddStoreAsync(store2);

        // 嘗試將 store1 更新為與 store2 相同的資訊
        added1.Name = "便當店B";
        added1.Phone = "0923456789";
        added1.Address = "台北市大安區";
        TriggerModelValidation(added1);

        // Act
        var result = await _controller.Edit(added1.Id, added1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(_controller.ModelState.IsValid);
        Assert.True(_controller.ModelState.ContainsKey(""));
    }

    [Fact]
    [Trait("Category", "US3")]
    public async Task PostEdit_ShouldSucceed_WhenUpdatingOwnInformationWithoutChangingUniqueFields()
    {
        // Arrange
        var store = CreateValidStore("便當店", "0912345678", "台北市中正區");
        var added = await _service.AddStoreAsync(store);

        // 只修改營業時間，名稱、電話、地址保持不變
        added.BusinessHours = "週一至週日 09:00-21:00";
        TriggerModelValidation(added);

        // Act
        var result = await _controller.Edit(added.Id, added);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
    }

    [Fact]
    [Trait("Category", "US3")]
    public async Task PostEdit_ShouldUpdateMenuItems_WhenMenuChanged()
    {
        // Arrange
        var store = CreateValidStore("便當店", "0912345678", "地址");
        store.MenuItems = new List<MenuItem>
        {
            new MenuItem { Name = "排骨便當", Price = 80 },
            new MenuItem { Name = "雞腿便當", Price = 90 }
        };
        var added = await _service.AddStoreAsync(store);

        // 修改菜單
        added.MenuItems = new List<MenuItem>
        {
            new MenuItem { Name = "排骨便當", Price = 85 },    // 價格改變
            new MenuItem { Name = "雞腿便當", Price = 95 },    // 價格改變
            new MenuItem { Name = "魚便當", Price = 100 }      // 新增項目
        };
        TriggerModelValidation(added);

        // Act
        await _controller.Edit(added.Id, added);

        // Assert
        var updated = await _service.GetStoreByIdAsync(added.Id);
        Assert.NotNull(updated);
        Assert.Equal(3, updated.MenuItems.Count);
        Assert.Equal(85, updated.MenuItems[0].Price);
        Assert.Equal("魚便當", updated.MenuItems[2].Name);
    }

    #endregion

    #region User Story 4 - 刪除店家資訊

    [Fact]
    [Trait("Category", "US4")]
    public async Task GetDelete_ShouldReturnView_WhenStoreExists()
    {
        // Arrange
        var store = CreateValidStore("要刪除的店家", "0912345678", "台北市中正區");
        var added = await _service.AddStoreAsync(store);

        // Act
        var result = await _controller.Delete(added.Id);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Store>(viewResult.Model);
        Assert.Equal(added.Id, model.Id);
        Assert.Equal("要刪除的店家", model.Name);
    }

    [Fact]
    [Trait("Category", "US4")]
    public async Task GetDelete_ShouldReturnNotFound_WhenStoreDoesNotExist()
    {
        // Arrange
        var nonExistentId = 9999;

        // Act
        var result = await _controller.Delete(nonExistentId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    [Trait("Category", "US4")]
    public async Task PostDeleteConfirmed_ShouldRedirectToIndex_WhenStoreExists()
    {
        // Arrange
        var store = CreateValidStore("要刪除的店家", "0912345678", "台北市中正區");
        var added = await _service.AddStoreAsync(store);

        // Act
        var result = await _controller.DeleteConfirmed(added.Id);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        
        // 驗證 TempData 包含成功訊息
        Assert.True(_controller.TempData.ContainsKey("SuccessMessage"));
        
        // 驗證店家確實被刪除
        var deletedStore = await _service.GetStoreByIdAsync(added.Id);
        Assert.Null(deletedStore);
    }

    [Fact]
    [Trait("Category", "US4")]
    public async Task PostDeleteConfirmed_ShouldReturnNotFound_WhenStoreDoesNotExist()
    {
        // Arrange
        var nonExistentId = 9999;

        // Act
        var result = await _controller.DeleteConfirmed(nonExistentId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    [Trait("Category", "US4")]
    public async Task PostDeleteConfirmed_ShouldNotAffectOtherStores()
    {
        // Arrange
        var store1 = CreateValidStore("店家1", "0912345678", "地址1");
        var store2 = CreateValidStore("店家2", "0923456789", "地址2");
        var store3 = CreateValidStore("店家3", "0934567890", "地址3");

        var added1 = await _service.AddStoreAsync(store1);
        var added2 = await _service.AddStoreAsync(store2);
        var added3 = await _service.AddStoreAsync(store3);

        // Act - 刪除中間的店家
        await _controller.DeleteConfirmed(added2.Id);

        // Assert
        var allStores = await _service.GetAllStoresAsync();
        Assert.Equal(2, allStores.Count);
        Assert.Contains(allStores, s => s.Id == added1.Id);
        Assert.Contains(allStores, s => s.Id == added3.Id);
        Assert.DoesNotContain(allStores, s => s.Id == added2.Id);
    }

    #endregion
}
