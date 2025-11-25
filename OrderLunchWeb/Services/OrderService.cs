using System.Text;
using System.Text.Json;
using OrderLunchWeb.Models;

namespace OrderLunchWeb.Services;

/// <summary>
/// 訂單服務實作，處理訂單相關的業務邏輯
/// </summary>
public class OrderService : IOrderService
{
    private readonly ILogger<OrderService> _logger;
    private readonly string _ordersFilePath;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    /// <summary>
    /// 建構子，設定檔案路徑和 JSON 序列化選項
    /// </summary>
    /// <param name="logger">日誌記錄器</param>
    public OrderService(ILogger<OrderService> logger)
    {
        _logger = logger;
        _ordersFilePath = Path.Combine("Data", "orders.json");
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNameCaseInsensitive = true
        };

        EnsureFileExists();
    }

    /// <summary>
    /// 確保訂單 JSON 檔案存在
    /// </summary>
    private void EnsureFileExists()
    {
        var directory = Path.GetDirectoryName(_ordersFilePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        if (!File.Exists(_ordersFilePath))
        {
            File.WriteAllText(_ordersFilePath, "[]", Encoding.UTF8);
        }
    }

    /// <inheritdoc />
    public async Task<Order> CreateOrderAsync(Order order)
    {
        if (order is null)
        {
            throw new ArgumentNullException(nameof(order));
        }

        if (order.Items is null || order.Items.Count == 0)
        {
            throw new ArgumentException("訂單必須至少包含一個菜品", nameof(order));
        }

        order.OrderId = GenerateOrderId();
        order.CreatedAt = DateTime.Now;
        order.Status = OrderStatus.Pending;

        var orders = await GetAllOrdersAsync();
        orders.Add(order);

        await SaveOrdersAsync(orders);

        _logger.LogInformation(
            "訂單建立成功：OrderId={OrderId}, StoreId={StoreId}, CustomerName={CustomerName}, TotalAmount={TotalAmount}",
            order.OrderId, order.StoreId, order.CustomerName, order.TotalAmount);

        return order;
    }

    /// <inheritdoc />
    public async Task<Order?> GetOrderByIdAsync(string orderId)
    {
        if (string.IsNullOrWhiteSpace(orderId))
        {
            return null;
        }

        var orders = await GetAllOrdersAsync();
        return orders.FirstOrDefault(o => o.OrderId == orderId);
    }

    /// <inheritdoc />
    public async Task<List<Order>> GetRecentOrdersAsync(int days = 5)
    {
        var cutoffDate = DateTime.Now.AddDays(-days);
        var orders = await GetAllOrdersAsync();

        return orders
            .Where(o => o.CreatedAt >= cutoffDate)
            .OrderByDescending(o => o.CreatedAt)
            .ToList();
    }

    /// <inheritdoc />
    public async Task<List<Order>> GetPendingOrdersAsync()
    {
        var orders = await GetAllOrdersAsync();

        return orders
            .Where(o => o.Status == OrderStatus.Pending)
            .OrderByDescending(o => o.CreatedAt)
            .ToList();
    }

    /// <inheritdoc />
    public async Task<int> CleanupOldOrdersAsync(int days = 5)
    {
        var cutoffDate = DateTime.Now.AddDays(-days);
        var orders = await GetAllOrdersAsync();
        var originalCount = orders.Count;

        var recentOrders = orders.Where(o => o.CreatedAt >= cutoffDate).ToList();
        var removedCount = originalCount - recentOrders.Count;

        if (removedCount > 0)
        {
            await SaveOrdersAsync(recentOrders);
            _logger.LogInformation(
                "舊訂單清理完成：刪除 {RemovedCount} 筆超過 {Days} 天的訂單",
                removedCount, days);
        }

        return removedCount;
    }

    /// <inheritdoc />
    public async Task<List<Order>> GetAllOrdersAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            if (!File.Exists(_ordersFilePath))
            {
                return new List<Order>();
            }

            var json = await File.ReadAllTextAsync(_ordersFilePath, Encoding.UTF8);

            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<Order>();
            }

            return JsonSerializer.Deserialize<List<Order>>(json, _jsonOptions) ?? new List<Order>();
        }
        catch (FileNotFoundException)
        {
            _logger.LogWarning("訂單檔案不存在，返回空清單：{FilePath}", _ordersFilePath);
            return new List<Order>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "讀取訂單檔案失敗：{FilePath}", _ordersFilePath);
            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <inheritdoc />
    public string GenerateOrderId()
    {
        return $"ORD{DateTime.Now:yyyyMMddHHmmssfff}";
    }

    /// <summary>
    /// 儲存訂單清單到 JSON 檔案
    /// </summary>
    /// <param name="orders">訂單清單</param>
    private async Task SaveOrdersAsync(List<Order> orders)
    {
        await _semaphore.WaitAsync();
        try
        {
            var json = JsonSerializer.Serialize(orders, _jsonOptions);
            await File.WriteAllTextAsync(_ordersFilePath, json, Encoding.UTF8);
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
