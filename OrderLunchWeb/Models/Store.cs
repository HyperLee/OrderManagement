using System.ComponentModel.DataAnnotations;

namespace OrderLunchWeb.Models;

/// <summary>
/// 代表一個餐廳店家
/// </summary>
public class Store
{
    /// <summary>
    /// 店家唯一識別碼
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 店家名稱 (最多 100 字元)
    /// </summary>
    [Required(ErrorMessage = "店家名稱為必填欄位")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "店家名稱長度必須介於 1 到 100 字元")]
    [Display(Name = "店家名稱")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 店家地址 (最多 200 字元)
    /// </summary>
    [Required(ErrorMessage = "店家地址為必填欄位")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "店家地址長度必須介於 1 到 200 字元")]
    [Display(Name = "店家地址")]
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// 電話類型 (市話或行動電話)
    /// </summary>
    [Required(ErrorMessage = "請選擇電話類型")]
    [Display(Name = "電話類型")]
    public PhoneType PhoneType { get; set; }

    /// <summary>
    /// 聯絡電話號碼 (僅數字)
    /// </summary>
    [Required(ErrorMessage = "聯絡電話為必填欄位")]
    [RegularExpression(@"^\d+$", ErrorMessage = "電話號碼僅能輸入數字")]
    [Display(Name = "聯絡電話")]
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// 營業時間 (自由文字，最多 100 字元)
    /// </summary>
    [Required(ErrorMessage = "營業時間為必填欄位")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "營業時間長度必須介於 1 到 100 字元")]
    [Display(Name = "營業時間")]
    public string BusinessHours { get; set; } = string.Empty;

    /// <summary>
    /// 菜單項目清單 (1-20 項)
    /// </summary>
    [Required]
    [MinLength(1, ErrorMessage = "至少需要新增一個菜單項目")]
    [MaxLength(20, ErrorMessage = "菜單項目已達上限 20 筆")]
    public List<MenuItem> MenuItems { get; set; } = new();

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 最後修改時間
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
