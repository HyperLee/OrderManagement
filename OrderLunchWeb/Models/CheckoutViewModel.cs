using System.ComponentModel.DataAnnotations;

namespace OrderLunchWeb.Models;

/// <summary>
/// 結帳視圖模型，用於在結帳頁面收集訂餐者資訊和訂單明細
/// </summary>
public class CheckoutViewModel
{
    /// <summary>
    /// 訂餐者姓名
    /// </summary>
    [Required(ErrorMessage = "姓名為必填欄位")]
    [MaxLength(100)]
    [Display(Name = "姓名")]
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// 訂餐者聯絡電話
    /// </summary>
    [Required(ErrorMessage = "聯絡電話為必填欄位")]
    [RegularExpression(@"^\d+$", ErrorMessage = "電話號碼僅能輸入數字")]
    [MaxLength(20, ErrorMessage = "電話號碼長度不可超過20位數")]
    [Display(Name = "聯絡電話")]
    public string CustomerPhone { get; set; } = string.Empty;

    /// <summary>
    /// 訂單項目清單（從 Session Storage 載入）
    /// </summary>
    public List<OrderItem> Items { get; set; } = new();

    /// <summary>
    /// 餐廳 ID
    /// </summary>
    public string StoreId { get; set; } = string.Empty;

    /// <summary>
    /// 餐廳名稱
    /// </summary>
    public string StoreName { get; set; } = string.Empty;

    /// <summary>
    /// 購物車資料（JSON 字串，用於表單提交）
    /// </summary>
    public string CartData { get; set; } = string.Empty;

    /// <summary>
    /// 訂單總金額（計算欄位）
    /// </summary>
    public decimal TotalAmount => Items.Sum(i => i.Subtotal);
}
