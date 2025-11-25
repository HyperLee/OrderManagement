using System.ComponentModel.DataAnnotations;

namespace OrderLunchWeb.Models;

/// <summary>
/// 訂單項目模型，代表訂單中的單一菜品項目
/// </summary>
public class OrderItem
{
    /// <summary>
    /// 菜品ID
    /// </summary>
    public string MenuItemId { get; set; } = string.Empty;

    /// <summary>
    /// 菜品名稱快照（儲存訂單建立時的菜品名稱）
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string MenuItemName { get; set; } = string.Empty;

    /// <summary>
    /// 單價快照（儲存訂單建立時的價格）
    /// </summary>
    [Range(0.01, double.MaxValue, ErrorMessage = "單價必須大於 0")]
    public decimal Price { get; set; }

    /// <summary>
    /// 訂購數量
    /// </summary>
    [Range(1, 100, ErrorMessage = "數量必須介於 1 到 100 之間")]
    public int Quantity { get; set; }

    /// <summary>
    /// 小計（計算欄位：單價 × 數量）
    /// </summary>
    public decimal Subtotal => Math.Round(Price * Quantity, 2);
}
