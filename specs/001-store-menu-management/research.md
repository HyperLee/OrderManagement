# 技術研究報告: 店家與菜單管理系統

**Feature**: 001-store-menu-management  
**Date**: 2025-11-22  
**Purpose**: 解決技術實作細節和最佳實踐

## 研究主題

### 1. JSON 檔案儲存與 UTF-8 編碼處理

**決策**: 使用 `System.Text.Json` 搭配 `JsonSerializerOptions` 進行 UTF-8 編碼的 JSON 序列化/反序列化

**理由**:

- `System.Text.Json` 是 .NET Core 內建的高效能 JSON 函式庫，從 .NET Core 3.0 開始取代 Newtonsoft.Json
- 預設使用 UTF-8 編碼，天然支援中文字元，無需額外設定
- 提供 `Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping` 選項避免中文被轉義為 Unicode 跳脫序列
- 支援 async/await 的檔案讀寫 API (`JsonSerializer.SerializeAsync` / `DeserializeAsync`)

**實作範例**:

```csharp
using System.Text.Encodings.Web;
using System.Text.Json;

// 設定選項
var options = new JsonSerializerOptions
{
    WriteIndented = true, // 格式化輸出
    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, // 避免中文轉義
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase // 使用 camelCase
};

// 序列化 (寫入)
await using var createStream = File.Create("Data/stores.json");
await JsonSerializer.SerializeAsync(createStream, stores, options);

// 反序列化 (讀取)
await using var openStream = File.OpenRead("Data/stores.json");
var stores = await JsonSerializer.DeserializeAsync<List<Store>>(openStream, options);
```

**考慮的替代方案**:

- **Newtonsoft.Json (Json.NET)**: 功能更豐富，但效能較低，需要額外套件
- **手動檔案讀寫 + 字串處理**: 容易出錯，不建議

**參考資料**:

- [System.Text.Json 官方文件](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-overview)
- [如何處理中文字元編碼](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-character-encoding)

---

### 2. ASP.NET Core MVC 架構最佳實踐

**決策**: 採用三層架構 - Controllers (展示層) → Services (業務邏輯層) → Repository/FileStorage (資料存取層)

**理由**:

- **關注點分離**: Controller 只負責 HTTP 請求/回應，Service 處理業務邏輯，FileStorage 處理 JSON 檔案存取
- **可測試性**: 每層可獨立進行單元測試，使用介面 (IStoreService, IFileStorage) 方便 Mock
- **可維護性**: 業務邏輯變更不影響 Controller，儲存方式變更 (如未來改用資料庫) 不影響 Service
- **符合 SOLID 原則**: 單一職責原則 (SRP)、依賴反轉原則 (DIP)

**架構圖**:

```text
Controllers (StoreController)
    ↓ 呼叫
Services (StoreService) ← 業務邏輯、驗證
    ↓ 呼叫
FileStorage (JsonFileStorage) ← JSON 讀寫
```

**實作模式**:

```csharp
// 1. 定義介面
public interface IStoreService
{
    Task<List<Store>> GetAllStoresAsync();
    Task<Store?> GetStoreByIdAsync(int id);
    Task<Store> AddStoreAsync(Store store);
    Task<Store> UpdateStoreAsync(Store store);
    Task<bool> DeleteStoreAsync(int id);
    Task<bool> IsDuplicateStoreAsync(string name, string phone, string address, int? excludeId = null);
}

// 2. Service 實作
public class StoreService : IStoreService
{
    private readonly IFileStorage _fileStorage;
    private readonly ILogger<StoreService> _logger;

    public StoreService(IFileStorage fileStorage, ILogger<StoreService> logger)
    {
        _fileStorage = fileStorage;
        _logger = logger;
    }
    
    // 實作方法...
}

// 3. 註冊到 DI 容器 (Program.cs)
builder.Services.AddScoped<IStoreService, StoreService>();
builder.Services.AddSingleton<IFileStorage, JsonFileStorage>();
```

**考慮的替代方案**:

- **Fat Controller**: 所有邏輯在 Controller 中 → 難以測試和維護
- **Repository Pattern**: 過度設計 (對於 JSON 檔案儲存過於複雜)

**參考資料**:

- [ASP.NET Core 架構指南](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection)
- [Clean Architecture in ASP.NET Core](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures)

---

### 3. 表單驗證最佳實踐

**決策**: 使用 Data Annotations + 自訂驗證屬性 + 客戶端驗證 (jQuery Validation)

**理由**:

- **Data Annotations**: 宣告式驗證，易讀易維護，與 ModelState 整合
- **自訂驗證屬性**: 處理複雜驗證邏輯 (如電話純數字、店家重複檢查)
- **雙重驗證**: 客戶端驗證提升 UX (即時回饋)，伺服器端驗證確保安全性
- **本地化錯誤訊息**: 自訂中文錯誤訊息，符合使用者體驗需求

**實作範例**:

```csharp
// Models/Store.cs
public class Store
{
    public int Id { get; set; }

    [Required(ErrorMessage = "店家名稱為必填欄位")]
    [StringLength(100, ErrorMessage = "店家名稱最多 100 字")]
    [Display(Name = "店家名稱")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "店家地址為必填欄位")]
    [StringLength(200, ErrorMessage = "店家地址最多 200 字")]
    [Display(Name = "店家地址")]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "聯絡電話為必填欄位")]
    [RegularExpression(@"^\d+$", ErrorMessage = "電話號碼僅能輸入數字")]
    [Display(Name = "聯絡電話")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "請選擇電話類型")]
    [Display(Name = "電話類型")]
    public PhoneType PhoneType { get; set; }

    [Required(ErrorMessage = "營業時間為必填欄位")]
    [StringLength(100, ErrorMessage = "營業時間最多 100 字")]
    [Display(Name = "營業時間")]
    public string BusinessHours { get; set; } = string.Empty;

    [Required]
    [MinLength(1, ErrorMessage = "至少需要新增一個菜單項目")]
    [MaxLength(20, ErrorMessage = "菜單項目已達上限 20 筆")]
    public List<MenuItem> MenuItems { get; set; } = new();

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

// 自訂驗證屬性 (進階驗證)
public class NoDuplicateStoreAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var service = validationContext.GetService<IStoreService>();
        var store = (Store)validationContext.ObjectInstance;
        
        if (service is not null)
        {
            var isDuplicate = service.IsDuplicateStoreAsync(
                store.Name, store.Phone, store.Address, store.Id).Result;
            
            if (isDuplicate)
            {
                return new ValidationResult("此店家已存在（店名、電話、地址完全相同）");
            }
        }
        
        return ValidationResult.Success;
    }
}
```

**客戶端驗證設定** (已在專案中使用 jQuery Validation):

```html
<!-- Views 中已包含 -->
@section Scripts {
    <partial name="_ValidationScriptsPartial" />
}
```

**考慮的替代方案**:

- **FluentValidation**: 更強大但增加複雜度，對練習用專案過度設計
- **僅客戶端驗證**: 不安全，可繞過
- **僅伺服器端驗證**: UX 較差，無即時回饋

**參考資料**:

- [ASP.NET Core Model Validation](https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation)
- [Custom Validation Attributes](https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation#custom-attributes)

---

### 4. 防重複提交機制

**決策**: 使用 PRG (Post-Redirect-Get) 模式 + 客戶端 JavaScript 按鈕禁用

**理由**:

- **PRG 模式**: POST 成功後 Redirect 到 GET 頁面，防止瀏覽器重新整理重複提交
- **按鈕禁用**: 表單提交時立即禁用提交按鈕，防止快速連續點擊
- **簡單有效**: 不需要複雜的 Token 機制 (如 CSRF Token 已由 ASP.NET Core 內建處理)

**實作範例**:

```csharp
// Controller
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(Store store)
{
    if (!ModelState.IsValid)
    {
        return View(store);
    }

    try
    {
        await _storeService.AddStoreAsync(store);
        TempData["SuccessMessage"] = "店家新增成功";
        return RedirectToAction(nameof(Index)); // PRG 模式
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "新增店家失敗");
        ModelState.AddModelError("", "新增店家失敗，請稍後再試");
        return View(store);
    }
}
```

**客戶端 JavaScript**:

```javascript
// Views/Store/Create.cshtml
<script>
document.querySelector('form').addEventListener('submit', function(e) {
    const submitBtn = this.querySelector('button[type="submit"]');
    if (submitBtn.disabled) {
        e.preventDefault();
        return false;
    }
    submitBtn.disabled = true;
    submitBtn.textContent = '處理中...';
});
</script>
```

**考慮的替代方案**:

- **Token-based**: 更安全但增加複雜度，對練習用專案過度設計
- **僅 JavaScript**: 可被繞過，需搭配伺服器端機制

**參考資料**:

- [Post-Redirect-Get Pattern](https://en.wikipedia.org/wiki/Post/Redirect/Get)
- [ASP.NET Core Anti-forgery](https://learn.microsoft.com/en-us/aspnet/core/security/anti-request-forgery)

---

### 5. 即時時間顯示實作

**決策**: 使用客戶端 JavaScript setInterval + DateTime.Now (伺服器時間初始化)

**理由**:

- **客戶端更新**: 每秒更新時間無需伺服器請求，減少網路負載
- **格式化**: 使用 JavaScript `toLocaleString` 或手動格式化為 `yyyy/MM/dd HH:mm:ss`
- **伺服器時間**: 初始時間從伺服器傳遞，確保時區正確

**實作範例**:

```csharp
// Controller (傳遞初始時間)
ViewBag.ServerTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
```

```html
<!-- View -->
<div class="text-center">
    <p>當前時間：<span id="current-time">@ViewBag.ServerTime</span></p>
</div>

<script>
function updateTime() {
    const now = new Date();
    const year = now.getFullYear();
    const month = String(now.getMonth() + 1).padStart(2, '0');
    const day = String(now.getDate()).padStart(2, '0');
    const hours = String(now.getHours()).padStart(2, '0');
    const minutes = String(now.getMinutes()).padStart(2, '0');
    const seconds = String(now.getSeconds()).padStart(2, '0');
    
    const timeString = `${year}/${month}/${day} ${hours}:${minutes}:${seconds}`;
    document.getElementById('current-time').textContent = timeString;
}

// 每秒更新
setInterval(updateTime, 1000);
</script>
```

**考慮的替代方案**:

- **SignalR 即時推送**: 過度設計，增加複雜度
- **定期 AJAX 請求**: 不必要的網路負載

**參考資料**:

- [JavaScript Date Object](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Date)
- [setInterval](https://developer.mozilla.org/en-US/docs/Web/API/setInterval)

---

### 6. 搜尋功能實作

**決策**: 使用客戶端 JavaScript 即時篩選 (小資料量) + 可選的伺服器端搜尋 (擴展性)

**理由**:

- **資料量小** (預期 < 100 筆): 客戶端篩選效能足夠，UX 更好 (即時回應)
- **簡單實作**: 使用 JavaScript `filter()` 和 DOM 操作
- **可擴展**: 若資料量增加，可輕鬆改為 AJAX 伺服器端搜尋

**實作範例 (客戶端)**:

```javascript
// Views/Store/Index.cshtml
<input type="text" id="search-input" class="form-control" placeholder="搜尋店家名稱...">

<script>
document.getElementById('search-input').addEventListener('input', function(e) {
    const searchTerm = e.target.value.toLowerCase();
    const storeItems = document.querySelectorAll('.store-item');
    
    storeItems.forEach(item => {
        const storeName = item.getAttribute('data-name').toLowerCase();
        if (storeName.includes(searchTerm)) {
            item.style.display = '';
        } else {
            item.style.display = 'none';
        }
    });
});
</script>
```

**考慮的替代方案**:

- **伺服器端搜尋 (AJAX)**: 適合大資料量，但增加複雜度
- **全文搜尋引擎**: 過度設計

**參考資料**:

- [JavaScript Array filter()](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Array/filter)

---

### 7. ID 自動遞增機制

**決策**: 在記憶體中維護 `nextId` 計數器，每次新增時遞增並持久化到 JSON

**理由**:

- **簡單可靠**: 不需要資料庫的 AUTO_INCREMENT，手動管理
- **唯一性**: 確保 ID 不重複
- **不重用已刪除 ID**: 符合需求規格

**實作範例**:

```csharp
public class JsonFileStorage
{
    private int _nextStoreId = 1;
    private int _nextMenuItemId = 1;
    
    public async Task InitializeAsync()
    {
        if (!File.Exists(_filePath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
            await SaveDataAsync(new DataContainer());
            return;
        }
        
        var data = await LoadDataAsync();
        
        // 計算下一個 ID
        if (data.Stores.Count > 0)
        {
            _nextStoreId = data.Stores.Max(s => s.Id) + 1;
            var allMenuItems = data.Stores.SelectMany(s => s.MenuItems);
            if (allMenuItems.Any())
            {
                _nextMenuItemId = allMenuItems.Max(m => m.Id) + 1;
            }
        }
    }
    
    public async Task<Store> AddStoreAsync(Store store)
    {
        var data = await LoadDataAsync();
        
        store.Id = _nextStoreId++;
        
        foreach (var menuItem in store.MenuItems)
        {
            menuItem.Id = _nextMenuItemId++;
        }
        
        store.CreatedAt = DateTime.Now;
        store.UpdatedAt = DateTime.Now;
        
        data.Stores.Add(store);
        await SaveDataAsync(data);
        
        return store;
    }
}
```

**考慮的替代方案**:

- **GUID**: 不符合需求 (要求整數 ID)
- **時間戳**: 可能重複

---

## 結論

所有技術決策都已明確，無未解決的 NEEDS CLARIFICATION。研究結果為 Phase 1 的實作提供了清晰的技術方向。

**關鍵技術選擇總結**:

1. **JSON 儲存**: System.Text.Json + UTF-8 編碼
2. **架構**: 三層架構 (Controller → Service → FileStorage)
3. **驗證**: Data Annotations + 自訂屬性 + 雙重驗證
4. **防重複提交**: PRG 模式 + 按鈕禁用
5. **即時時間**: 客戶端 JavaScript setInterval
6. **搜尋**: 客戶端即時篩選
7. **ID 管理**: 記憶體計數器 + JSON 持久化

下一步: 進入 Phase 1，生成 data-model.md 和 API contracts。
