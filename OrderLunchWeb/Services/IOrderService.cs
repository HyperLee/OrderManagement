using OrderLunchWeb.Models;

namespace OrderLunchWeb.Services;

/// <summary>
/// 訂單服務介面，定義訂單相關的業務邏輯操作
/// </summary>
public interface IOrderService
{
    /// <summary>
    /// 建立新訂單
    /// </summary>
    /// <param name="order">訂單資料</param>
    /// <returns>建立成功的訂單（含訂單編號）</returns>
    Task<Order> CreateOrderAsync(Order order);

    /// <summary>
    /// 根據訂單編號取得訂單
    /// </summary>
    /// <param name="orderId">訂單編號</param>
    /// <returns>訂單資料，若不存在則回傳 null</returns>
    Task<Order?> GetOrderByIdAsync(string orderId);

    /// <summary>
    /// 取得最近幾天內的所有訂單
    /// </summary>
    /// <param name="days">天數（預設 5 天）</param>
    /// <returns>訂單清單，按建立時間降序排列</returns>
    Task<List<Order>> GetRecentOrdersAsync(int days = 5);

    /// <summary>
    /// 取得進行中的訂單（狀態為 Pending）
    /// </summary>
    /// <returns>進行中的訂單清單</returns>
    Task<List<Order>> GetPendingOrdersAsync();

    /// <summary>
    /// 清理超過指定天數的舊訂單
    /// </summary>
    /// <param name="days">保留天數（預設 5 天）</param>
    /// <returns>被清理的訂單數量</returns>
    Task<int> CleanupOldOrdersAsync(int days = 5);

    /// <summary>
    /// 取得所有訂單
    /// </summary>
    /// <returns>所有訂單清單</returns>
    Task<List<Order>> GetAllOrdersAsync();

    /// <summary>
    /// 產生唯一訂單編號
    /// </summary>
    /// <returns>格式為 ORD{yyyyMMddHHmmssfff} 的訂單編號</returns>
    string GenerateOrderId();
}
