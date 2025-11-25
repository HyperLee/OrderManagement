using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using OrderLunchWeb.Controllers;
using OrderLunchWeb.Data;
using OrderLunchWeb.Models;
using OrderLunchWeb.Services;
using System.Text.Json;
using Xunit;

namespace OrderLunchWeb.Tests.Integration;

/// <summary>
/// OrderController 整合測試 (User Story 1)
/// 測試 Controller Actions: SelectRestaurant, Menu, Checkout, Submit, Confirmation
/// </summary>
public class OrderControllerTests : IDisposable
{
    private readonly string _testDataDirectory;
    private readonly string _originalDirectory;
    private readonly IFileStorage _storage;
    private readonly IStoreService _storeService;
    private readonly IOrderService _orderService;
    private readonly OrderController _controller;

    public OrderControllerTests()
    {
        _testDataDirectory = Path.Combine(Path.GetTempPath(), $"OrderLunchWeb_OrderControllerTest_{Guid.NewGuid()}");
        Directory.CreateDirectory(_testDataDirectory);

        // 保存原始工作目錄
        _originalDirectory = Directory.GetCurrentDirectory();

        _storage = new JsonFileStorage(_testDataDirectory, "test_stores.json");

        var serviceProvider = new ServiceCollection()
            .AddLogging()
            .BuildServiceProvider();

        var storeLogger = serviceProvider.GetRequiredService<ILogger<StoreService>>();
        _storeService = new StoreService(_storage, storeLogger);

        // 設定 OrderService 的工作目錄
        Directory.SetCurrentDirectory(_testDataDirectory);

        var orderLogger = serviceProvider.GetRequiredService<ILogger<OrderService>>();
        _orderService = new OrderService(orderLogger);

        var controllerLogger = serviceProvider.GetRequiredService<ILogger<OrderController>>();
        _controller = new OrderController(_storeService, _orderService, controllerLogger);

        // 初始化 TempData
        _controller.TempData = new TempDataDictionary(
            new DefaultHttpContext(),
            Mock.Of<ITempDataProvider>());
    }

    public void Dispose()
    {
        // 恢復原始工作目錄
        Directory.SetCurrentDirectory(_originalDirectory);
        
        if (Directory.Exists(_testDataDirectory))
        {
            try
            {
                Directory.Delete(_testDataDirectory, true);
            }
            catch
            {
                // 忽略清理失敗
            }
        }
    }

    private async Task<Store> CreateTestStore(string name = "測試餐廳")
    {
        var store = new Store
        {
            Name = name,
            Address = "台北市中正區",
            Phone = "0912345678",
            PhoneType = PhoneType.Mobile,
            BusinessHours = "11:00-20:00",
            MenuItems = new List<MenuItem>
            {
                new MenuItem { Name = "招牌便當", Price = 100, Description = "美味招牌" },
                new MenuItem { Name = "雞腿便當", Price = 120, Description = "酥脆雞腿" }
            }
        };

        return await _storeService.AddStoreAsync(store);
    }

    private static string CreateCartDataJson(int storeId, string storeName)
    {
        var cart = new
        {
            storeId = storeId.ToString(),
            storeName = storeName,
            items = new[]
            {
                new { menuItemId = "1", name = "招牌便當", price = 100, quantity = 2 }
            }
        };
        return JsonSerializer.Serialize(cart);
    }

    #region SelectRestaurant Tests

    [Fact]
    [Trait("Category", "US1")]
    public async Task SelectRestaurant_ShouldReturnViewResult_WithStoreList()
    {
        await CreateTestStore("餐廳A");
        await CreateTestStore("餐廳B");

        var result = await _controller.SelectRestaurant();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<Store>>(viewResult.Model);
        Assert.Equal(2, model.Count);
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task SelectRestaurant_ShouldReturnEmptyList_WhenNoStores()
    {
        var result = await _controller.SelectRestaurant();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<Store>>(viewResult.Model);
        Assert.Empty(model);
    }

    #endregion

    #region Menu Tests

    [Fact]
    [Trait("Category", "US1")]
    public async Task Menu_ShouldReturnViewResult_WhenStoreExists()
    {
        var store = await CreateTestStore();

        var result = await _controller.Menu(store.Id.ToString());

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Store>(viewResult.Model);
        Assert.Equal("測試餐廳", model.Name);
        Assert.Equal(2, model.MenuItems.Count);
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task Menu_ShouldReturnNotFound_WhenStoreNotExists()
    {
        var result = await _controller.Menu("999");

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task Menu_ShouldReturnNotFound_WhenStoreIdIsInvalid()
    {
        var result = await _controller.Menu("invalid");

        Assert.IsType<NotFoundResult>(result);
    }

    #endregion

    #region Checkout Tests

    [Fact]
    [Trait("Category", "US1")]
    public void Checkout_ShouldReturnViewResult_WhenCartDataIsValid()
    {
        var cartData = CreateCartDataJson(1, "測試餐廳");

        var result = _controller.Checkout(cartData);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<CheckoutViewModel>(viewResult.Model);
        Assert.Equal("1", model.StoreId);
        Assert.Equal("測試餐廳", model.StoreName);
        Assert.Single(model.Items);
    }

    [Fact]
    [Trait("Category", "US1")]
    public void Checkout_ShouldRedirectToSelectRestaurant_WhenCartDataIsEmpty()
    {
        var result = _controller.Checkout(string.Empty);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("SelectRestaurant", redirectResult.ActionName);
    }

    [Fact]
    [Trait("Category", "US1")]
    public void Checkout_ShouldRedirectToSelectRestaurant_WhenCartDataIsNull()
    {
        var result = _controller.Checkout(null!);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("SelectRestaurant", redirectResult.ActionName);
    }

    [Fact]
    [Trait("Category", "US1")]
    public void Checkout_ShouldSetTempDataError_WhenCartDataIsInvalid()
    {
        var result = _controller.Checkout("{invalid json}");

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.True(_controller.TempData.ContainsKey("ErrorMessage"));
    }

    #endregion

    #region Submit Tests

    [Fact]
    [Trait("Category", "US1")]
    public async Task Submit_ShouldRedirectToConfirmation_WhenOrderIsValid()
    {
        var cartData = CreateCartDataJson(1, "測試餐廳");
        var model = new CheckoutViewModel
        {
            CustomerName = "測試客戶",
            CustomerPhone = "0912345678",
            StoreId = "1",
            StoreName = "測試餐廳",
            CartData = cartData
        };

        var result = await _controller.Submit(model);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Confirmation", redirectResult.ActionName);
        Assert.NotNull(redirectResult.RouteValues?["orderId"]);
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task Submit_ShouldReturnView_WhenModelStateIsInvalid()
    {
        var model = new CheckoutViewModel
        {
            CustomerName = "",
            CustomerPhone = "0912345678",
            StoreId = "1",
            StoreName = "測試餐廳",
            CartData = CreateCartDataJson(1, "測試餐廳")
        };
        _controller.ModelState.AddModelError("CustomerName", "姓名為必填欄位");

        var result = await _controller.Submit(model);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Checkout", viewResult.ViewName);
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task Submit_ShouldReturnView_WhenPhoneIsInvalid()
    {
        var model = new CheckoutViewModel
        {
            CustomerName = "測試客戶",
            CustomerPhone = "abc123",
            StoreId = "1",
            StoreName = "測試餐廳",
            CartData = CreateCartDataJson(1, "測試餐廳")
        };
        _controller.ModelState.AddModelError("CustomerPhone", "電話號碼僅能輸入數字");

        var result = await _controller.Submit(model);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Checkout", viewResult.ViewName);
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task Submit_ShouldCreateOrder_WhenOrderIsValid()
    {
        var cartData = CreateCartDataJson(1, "測試餐廳");
        var model = new CheckoutViewModel
        {
            CustomerName = "測試客戶",
            CustomerPhone = "0912345678",
            StoreId = "1",
            StoreName = "測試餐廳",
            CartData = cartData
        };

        await _controller.Submit(model);

        var orders = await _orderService.GetAllOrdersAsync();
        Assert.Single(orders);
        Assert.Equal("測試客戶", orders[0].CustomerName);
    }

    #endregion

    #region Confirmation Tests

    [Fact]
    [Trait("Category", "US1")]
    public async Task Confirmation_ShouldReturnViewResult_WhenOrderExists()
    {
        var order = new Order
        {
            StoreId = "1",
            StoreName = "測試餐廳",
            CustomerName = "測試客戶",
            CustomerPhone = "0912345678",
            Items = new List<OrderItem>
            {
                new OrderItem { MenuItemId = "1", MenuItemName = "招牌便當", Price = 100, Quantity = 2 }
            }
        };
        var createdOrder = await _orderService.CreateOrderAsync(order);

        var result = await _controller.Confirmation(createdOrder.OrderId);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Order>(viewResult.Model);
        Assert.Equal(createdOrder.OrderId, model.OrderId);
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task Confirmation_ShouldReturnNotFound_WhenOrderNotExists()
    {
        var result = await _controller.Confirmation("ORD99999999999999999");

        Assert.IsType<NotFoundResult>(result);
    }

    #endregion
}
