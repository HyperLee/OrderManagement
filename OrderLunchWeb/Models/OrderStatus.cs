namespace OrderLunchWeb.Models;

/// <summary>
/// 訂單狀態列舉
/// </summary>
public enum OrderStatus
{
    /// <summary>
    /// 待確認（訂單建立後的預設狀態）
    /// </summary>
    Pending,

    /// <summary>
    /// 已確認（未來版本：餐廳確認訂單後）
    /// </summary>
    Confirmed,

    /// <summary>
    /// 準備中（未來版本：餐廳開始準備餐點）
    /// </summary>
    Preparing,

    /// <summary>
    /// 已完成（未來版本：訂單完成並交付）
    /// </summary>
    Completed,

    /// <summary>
    /// 已取消（未來版本：使用者或餐廳取消訂單）
    /// </summary>
    Cancelled
}
