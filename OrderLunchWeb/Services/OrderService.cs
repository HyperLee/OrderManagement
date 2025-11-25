using OrderLunchWeb.Data;
using OrderLunchWeb.Models;

namespace OrderLunchWeb.Services;

/// <summary>
/// 訂單服務實作，處理訂單相關的業務邏輯
/// </summary>
public class OrderService : IOrderService
{
    private readonly IFileStorage _fileStorage;
    private readonly ILogger<OrderService> _logger;
    private const string OrdersFileName = "orders.json";

    /// <summary>
    /// 建構子，透過相依性注入取得檔案儲存服務和日誌記錄器
    /// </summary>
    /// <param name="fileStorage">檔案儲存服務</param>
    /// <param name="logger">日誌記錄器</param>
    public OrderService(IFileStorage fileStorage, ILogger<OrderService> logger)
    {
        _fileStorage = fileStorage;
        _logger = logger;
    }

    // 服務方法將在 Phase 2 (Foundational) 實作
}
