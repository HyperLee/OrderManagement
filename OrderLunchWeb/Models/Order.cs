using System.ComponentModel.DataAnnotations;

namespace OrderLunchWeb.Models;

/// <summary>
/// 訂單模型，代表一筆完整的訂單
/// </summary>
public class Order
{
    /// <summary>
    /// 唯一訂單編號（格式：ORD{yyyyMMddHHmmssfff}）
    /// </summary>
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// 餐廳ID
    /// </summary>
    public string StoreId { get; set; } = string.Empty;

    /// <summary>
    /// 餐廳名稱快照（儲存訂單建立時的餐廳名稱）
    /// </summary>
    public string StoreName { get; set; } = string.Empty;

    /// <summary>
    /// 訂餐者姓名
    /// </summary>
    [Required(ErrorMessage = "姓名為必填欄位")]
    [MaxLength(100)]
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// 訂餐者聯絡電話（僅數字）
    /// </summary>
    [Required(ErrorMessage = "聯絡電話為必填欄位")]
    [RegularExpression(@"^\d+$", ErrorMessage = "電話號碼僅能輸入數字")]
    [MaxLength(20, ErrorMessage = "電話號碼長度不可超過20位數")]
    public string CustomerPhone { get; set; } = string.Empty;

    /// <summary>
    /// 訂單項目清單
    /// </summary>
    public List<OrderItem> Items { get; set; } = new();

    /// <summary>
    /// 訂單總金額（計算欄位：所有 OrderItem.Subtotal 的加總）
    /// </summary>
    public decimal TotalAmount => Items.Sum(i => i.Subtotal);

    /// <summary>
    /// 訂單狀態
    /// </summary>
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    /// <summary>
    /// 訂單建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
