using OrderLunchWeb.Models;

namespace OrderLunchWeb.Services;

/// <summary>
/// 店家服務介面，定義業務邏輯層的操作
/// </summary>
public interface IStoreService
{
    /// <summary>
    /// 取得所有店家資料
    /// </summary>
    /// <returns>店家清單</returns>
    Task<List<Store>> GetAllStoresAsync();

    /// <summary>
    /// 根據 ID 取得特定店家資料
    /// </summary>
    /// <param name="id">店家 ID</param>
    /// <returns>店家物件，若找不到則返回 null</returns>
    Task<Store?> GetStoreByIdAsync(int id);

    /// <summary>
    /// 新增店家資料
    /// </summary>
    /// <param name="store">要新增的店家物件</param>
    /// <returns>新增後的店家物件</returns>
    Task<Store> AddStoreAsync(Store store);

    /// <summary>
    /// 更新店家資料
    /// </summary>
    /// <param name="store">要更新的店家物件</param>
    /// <returns>更新後的店家物件，若找不到則返回 null</returns>
    Task<Store?> UpdateStoreAsync(Store store);

    /// <summary>
    /// 刪除店家資料
    /// </summary>
    /// <param name="id">要刪除的店家 ID</param>
    /// <returns>刪除成功返回 true，否則返回 false</returns>
    Task<bool> DeleteStoreAsync(int id);

    /// <summary>
    /// 檢查是否為重複的店家 (店名 + 電話 + 地址組合)
    /// </summary>
    /// <param name="name">店家名稱</param>
    /// <param name="phone">聯絡電話</param>
    /// <param name="address">店家地址</param>
    /// <param name="excludeId">排除的店家 ID (用於編輯時排除自己)，預設為 null</param>
    /// <returns>若為重複店家返回 true，否則返回 false</returns>
    Task<bool> IsDuplicateStoreAsync(string name, string phone, string address, int? excludeId = null);
}
