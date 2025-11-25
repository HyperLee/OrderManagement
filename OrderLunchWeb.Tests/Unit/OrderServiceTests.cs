using Microsoft.Extensions.Logging;
using Moq;
using OrderLunchWeb.Models;
using OrderLunchWeb.Services;
using Xunit;

namespace OrderLunchWeb.Tests.Unit;

/// <summary>
/// OrderService 單元測試 (User Story 1)
/// 測試業務邏輯：訂單建立、查詢、清理
/// </summary>
public class OrderServiceTests : IDisposable
{
    private readonly string _testDataDirectory;
    private readonly string _originalDirectory;
    private readonly OrderService _service;
    private readonly Mock<ILogger<OrderService>> _mockLogger;

    public OrderServiceTests()
    {
        _testDataDirectory = Path.Combine(Path.GetTempPath(), $"OrderLunchWeb_OrderTest_{Guid.NewGuid()}");
        Directory.CreateDirectory(_testDataDirectory);

        // 保存原始工作目錄
        _originalDirectory = Directory.GetCurrentDirectory();
        
        // 設定測試環境的工作目錄
        Directory.SetCurrentDirectory(_testDataDirectory);

        _mockLogger = new Mock<ILogger<OrderService>>();
        _service = new OrderService(_mockLogger.Object);
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

    private static Order CreateTestOrder(string storeName = "測試餐廳", string customerName = "測試客戶", string customerPhone = "0912345678")
    {
        return new Order
        {
            StoreId = "1",
            StoreName = storeName,
            CustomerName = customerName,
            CustomerPhone = customerPhone,
            Items = new List<OrderItem>
            {
                new OrderItem
                {
                    MenuItemId = "1",
                    MenuItemName = "招牌便當",
                    Price = 100,
                    Quantity = 2
                }
            }
        };
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task CreateOrderAsync_ShouldCreateOrderSuccessfully_WhenValidOrder()
    {
        var order = CreateTestOrder();

        var result = await _service.CreateOrderAsync(order);

        Assert.NotNull(result);
        Assert.StartsWith("ORD", result.OrderId);
        Assert.Equal("測試餐廳", result.StoreName);
        Assert.Equal("測試客戶", result.CustomerName);
        Assert.Equal(OrderStatus.Pending, result.Status);
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task CreateOrderAsync_ShouldGenerateOrderId_WhenOrderCreated()
    {
        var order = CreateTestOrder();

        var result = await _service.CreateOrderAsync(order);

        Assert.NotNull(result.OrderId);
        Assert.Matches(@"^ORD\d{17}$", result.OrderId);
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task CreateOrderAsync_ShouldSetCreatedAt_WhenOrderCreated()
    {
        var order = CreateTestOrder();
        var beforeCreate = DateTime.Now;

        var result = await _service.CreateOrderAsync(order);
        var afterCreate = DateTime.Now;

        Assert.True(result.CreatedAt >= beforeCreate.AddSeconds(-1) && result.CreatedAt <= afterCreate.AddSeconds(1));
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task CreateOrderAsync_ShouldSetPendingStatus_WhenOrderCreated()
    {
        var order = CreateTestOrder();

        var result = await _service.CreateOrderAsync(order);

        Assert.Equal(OrderStatus.Pending, result.Status);
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task CreateOrderAsync_ShouldThrowArgumentException_WhenOrderHasNoItems()
    {
        var order = new Order
        {
            StoreId = "1",
            StoreName = "測試餐廳",
            CustomerName = "測試客戶",
            CustomerPhone = "0912345678",
            Items = new List<OrderItem>()
        };

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateOrderAsync(order));
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task CreateOrderAsync_ShouldThrowArgumentNullException_WhenOrderIsNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateOrderAsync(null!));
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task CreateOrderAsync_ShouldCalculateTotalAmount_WhenOrderHasMultipleItems()
    {
        var order = new Order
        {
            StoreId = "1",
            StoreName = "測試餐廳",
            CustomerName = "測試客戶",
            CustomerPhone = "0912345678",
            Items = new List<OrderItem>
            {
                new OrderItem { MenuItemId = "1", MenuItemName = "便當A", Price = 100, Quantity = 2 },
                new OrderItem { MenuItemId = "2", MenuItemName = "便當B", Price = 80, Quantity = 3 }
            }
        };

        var result = await _service.CreateOrderAsync(order);

        // TotalAmount = (100 * 2) + (80 * 3) = 200 + 240 = 440
        Assert.Equal(440, result.TotalAmount);
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task GetOrderByIdAsync_ShouldReturnOrder_WhenOrderExists()
    {
        var order = CreateTestOrder();
        var createdOrder = await _service.CreateOrderAsync(order);

        var result = await _service.GetOrderByIdAsync(createdOrder.OrderId);

        Assert.NotNull(result);
        Assert.Equal(createdOrder.OrderId, result.OrderId);
        Assert.Equal("測試餐廳", result.StoreName);
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task GetOrderByIdAsync_ShouldReturnNull_WhenOrderNotExists()
    {
        var result = await _service.GetOrderByIdAsync("ORD99999999999999999");

        Assert.Null(result);
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task GetOrderByIdAsync_ShouldReturnNull_WhenOrderIdIsEmpty()
    {
        var result = await _service.GetOrderByIdAsync("");

        Assert.Null(result);
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task GetRecentOrdersAsync_ShouldReturnOrdersWithinDays_WhenOrdersExist()
    {
        var order = CreateTestOrder();
        await _service.CreateOrderAsync(order);

        var result = await _service.GetRecentOrdersAsync(5);

        Assert.Single(result);
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task GetRecentOrdersAsync_ShouldReturnEmptyList_WhenNoOrders()
    {
        var result = await _service.GetRecentOrdersAsync(5);

        Assert.Empty(result);
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task GetRecentOrdersAsync_ShouldOrderByCreatedAtDescending()
    {
        var order1 = CreateTestOrder("餐廳A");
        var order2 = CreateTestOrder("餐廳B");

        await _service.CreateOrderAsync(order1);
        await Task.Delay(10); // 確保時間不同
        await _service.CreateOrderAsync(order2);

        var result = await _service.GetRecentOrdersAsync(5);

        Assert.Equal(2, result.Count);
        Assert.Equal("餐廳B", result[0].StoreName); // 最新的在前面
        Assert.Equal("餐廳A", result[1].StoreName);
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task GetPendingOrdersAsync_ShouldReturnPendingOrders_WhenPendingOrdersExist()
    {
        var order = CreateTestOrder();
        await _service.CreateOrderAsync(order);

        var result = await _service.GetPendingOrdersAsync();

        Assert.Single(result);
        Assert.Equal(OrderStatus.Pending, result[0].Status);
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task GetPendingOrdersAsync_ShouldReturnEmptyList_WhenNoPendingOrders()
    {
        var result = await _service.GetPendingOrdersAsync();

        Assert.Empty(result);
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task GetAllOrdersAsync_ShouldReturnAllOrders_WhenOrdersExist()
    {
        await _service.CreateOrderAsync(CreateTestOrder("餐廳A"));
        await _service.CreateOrderAsync(CreateTestOrder("餐廳B"));

        var result = await _service.GetAllOrdersAsync();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task GetAllOrdersAsync_ShouldReturnEmptyList_WhenNoOrders()
    {
        var result = await _service.GetAllOrdersAsync();

        Assert.Empty(result);
    }

    [Fact]
    [Trait("Category", "US1")]
    public void GenerateOrderId_ShouldReturnValidFormat()
    {
        var orderId = _service.GenerateOrderId();

        Assert.StartsWith("ORD", orderId);
        Assert.Matches(@"^ORD\d{17}$", orderId);
    }

    [Fact]
    [Trait("Category", "US1")]
    public void GenerateOrderId_ShouldGenerateUniqueIds()
    {
        var orderId1 = _service.GenerateOrderId();
        Thread.Sleep(1); // 確保時間戳不同
        var orderId2 = _service.GenerateOrderId();

        Assert.NotEqual(orderId1, orderId2);
    }

    [Fact]
    [Trait("Category", "US1")]
    public async Task CleanupOldOrdersAsync_ShouldReturnZero_WhenNoOldOrders()
    {
        await _service.CreateOrderAsync(CreateTestOrder());

        var result = await _service.CleanupOldOrdersAsync(5);

        Assert.Equal(0, result);
    }
}
