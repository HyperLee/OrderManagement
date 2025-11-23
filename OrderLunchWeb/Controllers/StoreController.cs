using Microsoft.AspNetCore.Mvc;
using OrderLunchWeb.Models;
using OrderLunchWeb.Services;

namespace OrderLunchWeb.Controllers;

/// <summary>
/// 店家管理控制器 - 負責店家 CRUD 操作
/// </summary>
public class StoreController : Controller
{
    private readonly IStoreService _storeService;
    private readonly ILogger<StoreController> _logger;

    /// <summary>
    /// 初始化 StoreController
    /// </summary>
    /// <param name="storeService">店家服務</param>
    /// <param name="logger">日誌記錄器</param>
    public StoreController(IStoreService storeService, ILogger<StoreController> logger)
    {
        _storeService = storeService;
        _logger = logger;
    }

    /// <summary>
    /// GET: /Store/Create - 顯示新增店家表單
    /// </summary>
    /// <returns>新增店家的 View</returns>
    [HttpGet]
    public IActionResult Create()
    {
        _logger.LogInformation("顯示新增店家表單");

        // 建立預設的 Store 物件，包含一個空的菜單項目
        var store = new Store
        {
            MenuItems = new List<MenuItem>
            {
                new MenuItem() // 預設一個空菜單項目
            }
        };

        return View(store);
    }

    /// <summary>
    /// POST: /Store/Create - 提交新增店家資料
    /// </summary>
    /// <param name="store">店家資料</param>
    /// <returns>成功則重定向到 Index，失敗則返回 View 顯示錯誤</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Store store)
    {
        _logger.LogInformation("接收新增店家請求: {StoreName}", store.Name);

        // 檢查 ModelState 驗證
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("店家資料驗證失敗: {StoreName}", store.Name);
            return View(store);
        }

        // 檢查是否為重複店家 (店名 + 電話 + 地址)
        var isDuplicate = await _storeService.IsDuplicateStoreAsync(
            store.Name, 
            store.Phone, 
            store.Address);

        if (isDuplicate)
        {
            _logger.LogWarning("嘗試新增重複店家: 名稱={StoreName}, 電話={Phone}, 地址={Address}", 
                store.Name, store.Phone, store.Address);
            
            ModelState.AddModelError("", "此店家已存在（店名、電話、地址完全相同）。若為分店請修改地址或電話。");
            return View(store);
        }

        try
        {
            // 新增店家
            var addedStore = await _storeService.AddStoreAsync(store);
            
            _logger.LogInformation("成功新增店家，ID: {StoreId}, 名稱: {StoreName}", 
                addedStore.Id, addedStore.Name);

            // 使用 PRG (Post-Redirect-Get) 模式防止重複提交
            TempData["SuccessMessage"] = $"店家「{addedStore.Name}」新增成功！";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "新增店家時發生錯誤: {StoreName}", store.Name);
            ModelState.AddModelError("", "新增店家時發生錯誤，請稍後再試。");
            return View(store);
        }
    }

    /// <summary>
    /// GET: /Store/Index - 顯示所有店家列表
    /// </summary>
    /// <returns>店家列表 View</returns>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        _logger.LogInformation("顯示店家列表");

        try
        {
            var stores = await _storeService.GetAllStoresAsync();
            return View(stores);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得店家列表時發生錯誤");
            return View(new List<Store>()); // 返回空列表避免錯誤
        }
    }

    /// <summary>
    /// GET: /Store/Details/{id} - 顯示店家詳細資訊
    /// </summary>
    /// <param name="id">店家 ID</param>
    /// <returns>店家詳情 View 或 404</returns>
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        _logger.LogInformation("顯示店家詳情，ID: {StoreId}", id);

        var store = await _storeService.GetStoreByIdAsync(id);

        if (store is null)
        {
            _logger.LogWarning("找不到店家，ID: {StoreId}", id);
            return NotFound();
        }

        return View(store);
    }

    /// <summary>
    /// GET: /Store/Edit/{id} - 顯示編輯店家表單
    /// </summary>
    /// <param name="id">店家 ID</param>
    /// <returns>編輯表單 View 或 404</returns>
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        _logger.LogInformation("顯示編輯店家表單，ID: {StoreId}", id);

        var store = await _storeService.GetStoreByIdAsync(id);

        if (store is null)
        {
            _logger.LogWarning("找不到店家，ID: {StoreId}", id);
            return NotFound();
        }

        return View(store);
    }

    /// <summary>
    /// POST: /Store/Edit/{id} - 提交更新店家資料
    /// </summary>
    /// <param name="id">店家 ID</param>
    /// <param name="store">更新後的店家資料</param>
    /// <returns>成功則重定向到 Index，失敗則返回 View 顯示錯誤</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Store store)
    {
        _logger.LogInformation("接收更新店家請求，ID: {StoreId}, 名稱: {StoreName}", id, store.Name);

        if (id != store.Id)
        {
            _logger.LogWarning("店家 ID 不匹配: URL ID={UrlId}, 模型 ID={ModelId}", id, store.Id);
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("店家資料驗證失敗，ID: {StoreId}", id);
            return View(store);
        }

        // 檢查是否為重複店家 (排除自己)
        var isDuplicate = await _storeService.IsDuplicateStoreAsync(
            store.Name, 
            store.Phone, 
            store.Address, 
            store.Id);

        if (isDuplicate)
        {
            _logger.LogWarning("嘗試更新為重複店家，ID: {StoreId}", id);
            ModelState.AddModelError("", "此店家資訊與其他店家重複（店名、電話、地址完全相同）。");
            return View(store);
        }

        try
        {
            var result = await _storeService.UpdateStoreAsync(store);

            if (!result)
            {
                _logger.LogWarning("找不到要更新的店家，ID: {StoreId}", id);
                return NotFound();
            }

            _logger.LogInformation("成功更新店家，ID: {StoreId}, 名稱: {StoreName}", id, store.Name);

            TempData["SuccessMessage"] = $"店家「{store.Name}」更新成功！";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新店家時發生錯誤，ID: {StoreId}", id);
            ModelState.AddModelError("", "更新店家時發生錯誤，請稍後再試。");
            return View(store);
        }
    }

    /// <summary>
    /// GET: /Store/Delete/{id} - 顯示刪除確認頁面
    /// </summary>
    /// <param name="id">店家 ID</param>
    /// <returns>刪除確認 View 或 404</returns>
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("顯示刪除確認頁面，ID: {StoreId}", id);

        var store = await _storeService.GetStoreByIdAsync(id);

        if (store is null)
        {
            _logger.LogWarning("找不到店家，ID: {StoreId}", id);
            return NotFound();
        }

        return View(store);
    }

    /// <summary>
    /// POST: /Store/Delete/{id} - 確認刪除店家
    /// </summary>
    /// <param name="id">店家 ID</param>
    /// <returns>成功則重定向到 Index，失敗則返回 404</returns>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        _logger.LogInformation("接收刪除店家請求，ID: {StoreId}", id);

        try
        {
            var result = await _storeService.DeleteStoreAsync(id);

            if (!result)
            {
                _logger.LogWarning("找不到要刪除的店家，ID: {StoreId}", id);
                return NotFound();
            }

            _logger.LogInformation("成功刪除店家，ID: {StoreId}", id);

            TempData["SuccessMessage"] = "店家刪除成功！";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刪除店家時發生錯誤，ID: {StoreId}", id);
            TempData["ErrorMessage"] = "刪除店家時發生錯誤，請稍後再試。";
            return RedirectToAction(nameof(Index));
        }
    }
}
