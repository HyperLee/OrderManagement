namespace OrderLunchWeb.Models;

/// <summary>
/// 訂單紀錄視圖模型，用於在訂單紀錄頁面顯示訂單清單
/// </summary>
public class OrderHistoryViewModel
{
    /// <summary>
    /// 訂單清單（最近 5 天）
    /// </summary>
    public List<OrderSummary> Orders { get; set; } = new();

    /// <summary>
    /// 訂單摘要（用於列表顯示）
    /// </summary>
    public class OrderSummary
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public string OrderId { get; set; } = string.Empty;

        /// <summary>
        /// 訂單建立時間
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 餐廳名稱
        /// </summary>
        public string StoreName { get; set; } = string.Empty;

        /// <summary>
        /// 訂單總金額
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 訂單狀態
        /// </summary>
        public OrderStatus Status { get; set; }

        /// <summary>
        /// 訂單項目數量
        /// </summary>
        public int ItemCount { get; set; }
    }
}
