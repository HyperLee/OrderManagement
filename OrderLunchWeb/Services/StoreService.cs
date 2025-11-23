using OrderLunchWeb.Data;
using OrderLunchWeb.Models;

namespace OrderLunchWeb.Services;

/// <summary>
/// 店家服務實作，處理店家相關的業務邏輯
/// </summary>
public class StoreService : IStoreService
{
    private readonly IFileStorage _fileStorage;
    private readonly ILogger<StoreService> _logger;

    /// <summary>
    /// 初始化 StoreService
    /// </summary>
    /// <param name="fileStorage">檔案儲存服務</param>
    /// <param name="logger">日誌記錄器</param>
    public StoreService(IFileStorage fileStorage, ILogger<StoreService> logger)
    {
        _fileStorage = fileStorage;
        _logger = logger;
    }

    /// <summary>
    /// 取得所有店家資料
    /// </summary>
    public async Task<List<Store>> GetAllStoresAsync()
    {
        _logger.LogInformation("取得所有店家資料");
        return await _fileStorage.GetAllAsync();
    }

    /// <summary>
    /// 根據 ID 取得特定店家資料
    /// </summary>
    public async Task<Store?> GetStoreByIdAsync(int id)
    {
        _logger.LogInformation("取得店家資料，ID: {StoreId}", id);
        var store = await _fileStorage.GetByIdAsync(id);
        
        if (store is null)
        {
            _logger.LogWarning("找不到店家，ID: {StoreId}", id);
        }
        
        return store;
    }

    /// <summary>
    /// 新增店家資料
    /// </summary>
    public async Task<Store> AddStoreAsync(Store store)
    {
        _logger.LogInformation("開始新增店家: {StoreName}", store.Name);
        
        var addedStore = await _fileStorage.AddAsync(store);
        
        _logger.LogInformation("成功新增店家，ID: {StoreId}, 名稱: {StoreName}", addedStore.Id, addedStore.Name);
        
        return addedStore;
    }

    /// <summary>
    /// 更新店家資料
    /// </summary>
    public async Task<Store?> UpdateStoreAsync(Store store)
    {
        _logger.LogInformation("開始更新店家，ID: {StoreId}, 名稱: {StoreName}", store.Id, store.Name);
        
        var result = await _fileStorage.UpdateAsync(store);
        
        if (result is not null)
        {
            _logger.LogInformation("成功更新店家，ID: {StoreId}", store.Id);
        }
        else
        {
            _logger.LogWarning("更新店家失敗，找不到店家 ID: {StoreId}", store.Id);
        }
        
        return result;
    }

    /// <summary>
    /// 刪除店家資料
    /// </summary>
    public async Task<bool> DeleteStoreAsync(int id)
    {
        _logger.LogInformation("開始刪除店家，ID: {StoreId}", id);
        
        var result = await _fileStorage.DeleteAsync(id);
        
        if (result)
        {
            _logger.LogInformation("成功刪除店家，ID: {StoreId}", id);
        }
        else
        {
            _logger.LogWarning("刪除店家失敗，找不到店家 ID: {StoreId}", id);
        }
        
        return result;
    }

    /// <summary>
    /// 檢查是否為重複的店家 (店名 + 電話 + 地址組合)
    /// </summary>
    public async Task<bool> IsDuplicateStoreAsync(string name, string phone, string address, int? excludeId = null)
    {
        _logger.LogInformation("檢查重複店家: 名稱={StoreName}, 電話={Phone}, 地址={Address}, 排除ID={ExcludeId}", 
            name, phone, address, excludeId);
        
        var allStores = await _fileStorage.GetAllAsync();
        
        var duplicate = allStores.Any(s => 
            s.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
            s.Phone.Equals(phone, StringComparison.OrdinalIgnoreCase) &&
            s.Address.Equals(address, StringComparison.OrdinalIgnoreCase) &&
            s.Id != excludeId);
        
        if (duplicate)
        {
            _logger.LogWarning("發現重複店家: 名稱={StoreName}, 電話={Phone}, 地址={Address}", name, phone, address);
        }
        
        return duplicate;
    }
}
