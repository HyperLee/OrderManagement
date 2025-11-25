using System.ComponentModel.DataAnnotations;

namespace OrderLunchWeb.Models;

/// <summary>
/// 購物車資料傳輸物件（用於接收前端 Session Storage 資料）
/// </summary>
public class CartDto
{
    /// <summary>
    /// 餐廳 ID
    /// </summary>
    public string StoreId { get; set; } = string.Empty;

    /// <summary>
    /// 餐廳名稱
    /// </summary>
    public string StoreName { get; set; } = string.Empty;

    /// <summary>
    /// 購物車項目清單
    /// </summary>
    public List<CartItemDto> Items { get; set; } = new();

    /// <summary>
    /// 購物車總金額（計算欄位）
    /// </summary>
    public decimal TotalAmount => Items.Sum(i => i.Subtotal);
}

/// <summary>
/// 購物車項目資料傳輸物件
/// </summary>
public class CartItemDto
{
    /// <summary>
    /// 菜品 ID
    /// </summary>
    public string MenuItemId { get; set; } = string.Empty;

    /// <summary>
    /// 菜品名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 單價
    /// </summary>
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
