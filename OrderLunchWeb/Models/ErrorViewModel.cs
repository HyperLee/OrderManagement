namespace OrderLunchWeb.Models;

/// <summary>
/// 錯誤視圖模型
/// </summary>
public class ErrorViewModel
{
    /// <summary>
    /// 請求識別碼
    /// </summary>
    public string? RequestId { get; set; }

    /// <summary>
    /// 是否顯示請求識別碼
    /// </summary>
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
