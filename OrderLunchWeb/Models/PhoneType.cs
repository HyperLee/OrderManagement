using System.ComponentModel.DataAnnotations;

namespace OrderLunchWeb.Models;

/// <summary>
/// 聯絡電話類型
/// </summary>
public enum PhoneType
{
    /// <summary>
    /// 市話
    /// </summary>
    [Display(Name = "市話")]
    Landline = 1,

    /// <summary>
    /// 行動電話
    /// </summary>
    [Display(Name = "行動電話")]
    Mobile = 2
}
