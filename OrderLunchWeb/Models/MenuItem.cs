using System.ComponentModel.DataAnnotations;

namespace OrderLunchWeb.Models;

/// <summary>
/// 代表店家菜單中的單一餐點項目
/// </summary>
public class MenuItem
{
    /// <summary>
    /// 菜單項目唯一識別碼
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 菜名 (最多 50 字元)
    /// </summary>
    [Required(ErrorMessage = "菜名為必填欄位")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "菜名長度必須介於 1 到 50 字元")]
    [Display(Name = "菜名")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 價格 (正整數或零)
    /// </summary>
    [Required(ErrorMessage = "價格為必填欄位")]
    [Range(0, int.MaxValue, ErrorMessage = "價格必須為正整數或零")]
    [Display(Name = "價格")]
    public int Price { get; set; }

    /// <summary>
    /// 菜品描述 (選填，最多 200 字元)
    /// </summary>
    [StringLength(200, ErrorMessage = "描述最多 200 字元")]
    [Display(Name = "描述")]
    public string Description { get; set; } = string.Empty;
}
