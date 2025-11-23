# API Contracts: 店家與菜單管理系統

**Feature**: 001-store-menu-management  
**Date**: 2025-11-22  
**API Style**: ASP.NET Core MVC (HTTP POST/GET)  
**Base Path**: `/Store`

## 概述

本文件定義店家與菜單管理系統的所有 HTTP 端點、請求/回應格式和操作規範。系統使用標準 ASP.NET Core MVC 模式，不使用 RESTful API，而是傳統的 Controller Action 方式。

---

## 端點列表

| 端點 | HTTP 方法 | 用途 | 認證 |
|------|----------|------|------|
| `/Store/Index` | GET | 顯示店家列表頁面 | ❌ |
| `/Store/Create` | GET | 顯示新增店家表單 | ❌ |
| `/Store/Create` | POST | 提交新增店家資料 | ❌ |
| `/Store/Details/{id}` | GET | 顯示店家詳細資訊 | ❌ |
| `/Store/Edit/{id}` | GET | 顯示編輯店家表單 | ❌ |
| `/Store/Edit/{id}` | POST | 提交更新店家資料 | ❌ |
| `/Store/Delete/{id}` | GET | 顯示刪除確認頁面 | ❌ |
| `/Store/Delete/{id}` | POST | 確認刪除店家 | ❌ |
| `/` (Home/Index) | GET | 系統首頁 (含即時時間) | ❌ |

---

## 端點詳細規格

### 1. GET /Store/Index

**用途**: 顯示所有店家列表，支援客戶端搜尋

**請求**:

- **Method**: GET
- **Parameters**: 無
- **Headers**: 無特殊要求

**回應**:

- **Content-Type**: `text/html`
- **Status Code**: 200 OK
- **Body**: 渲染 Razor View (`Views/Store/Index.cshtml`)，包含店家列表和搜尋功能

**View Model**:

```csharp
// 傳遞 List<Store> 到 View
@model List<Store>
```

**View 內容**:

- 顯示所有店家的基本資訊 (店名、地址、電話類型、電話、營業時間)
- 提供搜尋框 (客戶端 JavaScript 即時篩選)
- 提供「新增店家」按鈕
- 每個店家項目提供「查看詳情」、「編輯」、「刪除」連結

**範例 UI 結構**:

```html
<div class="container">
    <h1>店家列表</h1>
    <a href="/Store/Create" class="btn btn-primary">新增店家</a>
    <input type="text" id="search-input" placeholder="搜尋店家名稱..." />
    
    <div class="store-list">
        <!-- 每個店家卡片 -->
        <div class="store-item" data-name="好吃便當店">
            <h3>好吃便當店</h3>
            <p>地址: 台北市中正區...</p>
            <p>電話: 行動電話 0912345678</p>
            <p>營業時間: 週一至週五 11:00-14:00</p>
            <a href="/Store/Details/1">查看詳情</a>
            <a href="/Store/Edit/1">編輯</a>
            <a href="/Store/Delete/1">刪除</a>
        </div>
    </div>
</div>
```

**錯誤處理**:

- 若無店家資料，顯示空狀態訊息: "目前沒有店家資料，請點擊上方按鈕新增店家"

---

### 2. GET /Store/Create

**用途**: 顯示新增店家的表單

**請求**:

- **Method**: GET
- **Parameters**: 無

**回應**:

- **Content-Type**: `text/html`
- **Status Code**: 200 OK
- **Body**: 渲染 Razor View (`Views/Store/Create.cshtml`)

**View Model**:

```csharp
@model Store
```

**表單欄位**:

- 店家名稱 (文字輸入，必填，最多 100 字元)
- 店家地址 (文字輸入，必填，最多 200 字元)
- 電話類型 (下拉選單: 市話/行動電話，必填)
- 聯絡電話 (文字輸入，必填，僅數字)
- 營業時間 (文字輸入，必填，最多 100 字元)
- 菜單項目 (動態列表，至少 1 項，最多 20 項)
  - 菜名 (文字輸入，必填，最多 50 字元)
  - 價格 (數字輸入，必填，≥ 0)
  - 描述 (文字輸入，選填，最多 200 字元)

**UI 功能**:

- 「新增菜單項目」按鈕 (JavaScript 動態新增輸入欄位)
- 「移除」按鈕 (移除菜單項目，但至少保留 1 項)
- 客戶端驗證 (jQuery Validation)
- 防重複提交 (按鈕禁用 + PRG 模式)

---

### 3. POST /Store/Create

**用途**: 提交新增店家資料

**請求**:

- **Method**: POST
- **Content-Type**: `application/x-www-form-urlencoded`
- **Anti-Forgery Token**: 必須包含 `__RequestVerificationToken`

**Request Body** (表單資料):

```
Name=好吃便當店
Address=台北市中正區羅斯福路一段100號
PhoneType=2
Phone=0912345678
BusinessHours=週一至週五 11:00-14:00, 17:00-20:00
MenuItems[0].Name=排骨便當
MenuItems[0].Price=80
MenuItems[0].Description=香酥排骨配上三菜一飯
MenuItems[1].Name=雞腿便當
MenuItems[1].Price=90
MenuItems[1].Description=大雞腿配上三菜一飯
```

**驗證規則**:

1. 所有必填欄位不可為空
2. 電話號碼僅含數字
3. 菜單價格 ≥ 0
4. 菜單項目數量 1-20 筆
5. 店家不重複 (Name + Phone + Address 唯一)
6. 欄位長度限制

**成功回應**:

- **Status Code**: 302 Found (Redirect)
- **Location**: `/Store/Index`
- **TempData**: `SuccessMessage = "店家新增成功"`

**驗證失敗回應**:

- **Status Code**: 200 OK
- **Body**: 重新渲染 `Create.cshtml`，顯示錯誤訊息
- **ModelState**: 包含驗證錯誤

**錯誤訊息範例**:

```csharp
ModelState.AddModelError("Name", "店家名稱為必填欄位");
ModelState.AddModelError("Phone", "電話號碼僅能輸入數字");
ModelState.AddModelError("", "此店家已存在（店名、電話、地址完全相同）");
```

---

### 4. GET /Store/Details/{id}

**用途**: 顯示店家詳細資訊和完整菜單

**請求**:

- **Method**: GET
- **Parameters**:
  - `id` (路由參數): 店家 ID (int)

**範例**: `/Store/Details/1`

**回應**:

- **Content-Type**: `text/html`
- **Status Code**: 200 OK (若找到)
- **Body**: 渲染 Razor View (`Views/Store/Details.cshtml`)

**View Model**:

```csharp
@model Store
```

**顯示內容**:

- 店家完整資訊 (名稱、地址、電話類型、電話、營業時間、建立時間、修改時間)
- 完整菜單列表 (菜名、價格、描述)
- 「返回列表」、「編輯」、「刪除」按鈕

**錯誤處理**:

- **Status Code**: 404 Not Found (若店家不存在)
- **Body**: 顯示錯誤訊息 "找不到指定的店家"

---

### 5. GET /Store/Edit/{id}

**用途**: 顯示編輯店家的表單，預先填入現有資料

**請求**:

- **Method**: GET
- **Parameters**:
  - `id` (路由參數): 店家 ID (int)

**範例**: `/Store/Edit/1`

**回應**:

- **Content-Type**: `text/html`
- **Status Code**: 200 OK
- **Body**: 渲染 Razor View (`Views/Store/Edit.cshtml`)，表單欄位預先填入店家現有資料

**View Model**:

```csharp
@model Store
```

**表單欄位**: 與 Create 相同，但包含隱藏欄位 `Id`

**UI 功能**:

- 所有欄位預先填入現有值
- 菜單項目顯示現有清單，可新增/移除/修改
- 「儲存」和「取消」按鈕

---

### 6. POST /Store/Edit/{id}

**用途**: 提交更新店家資料

**請求**:

- **Method**: POST
- **Content-Type**: `application/x-www-form-urlencoded`
- **Anti-Forgery Token**: 必須包含

**Request Body**: 與 Create 類似，但包含 `Id` 欄位

**驗證規則**: 與 Create 相同，額外檢查:

- 店家 ID 必須存在
- 重複性檢查排除自己 (excludeId = store.Id)

**成功回應**:

- **Status Code**: 302 Found
- **Location**: `/Store/Index`
- **TempData**: `SuccessMessage = "店家資料更新成功"`

**驗證失敗回應**: 與 Create 相同

**錯誤處理**:

- **404 Not Found**: 若店家不存在

---

### 7. GET /Store/Delete/{id}

**用途**: 顯示刪除確認頁面

**請求**:

- **Method**: GET
- **Parameters**:
  - `id` (路由參數): 店家 ID (int)

**範例**: `/Store/Delete/1`

**回應**:

- **Content-Type**: `text/html`
- **Status Code**: 200 OK
- **Body**: 渲染 Razor View (`Views/Store/Delete.cshtml`)

**View Model**:

```csharp
@model Store
```

**顯示內容**:

- 店家基本資訊
- 確認訊息: "確定要刪除此店家嗎？此操作無法復原。"
- 「確認刪除」和「取消」按鈕

---

### 8. POST /Store/Delete/{id}

**用途**: 確認刪除店家

**請求**:

- **Method**: POST
- **Parameters**:
  - `id` (路由參數): 店家 ID (int)
- **Anti-Forgery Token**: 必須包含

**成功回應**:

- **Status Code**: 302 Found
- **Location**: `/Store/Index`
- **TempData**: `SuccessMessage = "店家刪除成功"`

**錯誤處理**:

- **404 Not Found**: 若店家不存在

---

### 9. GET / (Home/Index)

**用途**: 系統首頁，顯示訂餐系統標題和即時時間

**請求**:

- **Method**: GET
- **Parameters**: 無

**回應**:

- **Content-Type**: `text/html`
- **Status Code**: 200 OK
- **Body**: 渲染 Razor View (`Views/Home/Index.cshtml`)

**顯示內容**:

- 標題: "訂餐系統"
- 即時時間顯示 (格式: `yyyy/MM/dd HH:mm:ss`)
- 主選單:
  - 「店家列表」連結 → `/Store/Index`
  - 「訂購餐點」按鈕 (佔位，未實作)

**JavaScript 功能**:

- 使用 `setInterval` 每秒更新時間顯示

---

## Controller 結構

### StoreController

```csharp
namespace OrderLunchWeb.Controllers
{
    public class StoreController : Controller
    {
        private readonly IStoreService _storeService;
        private readonly ILogger<StoreController> _logger;

        public StoreController(IStoreService storeService, ILogger<StoreController> logger)
        {
            _storeService = storeService;
            _logger = logger;
        }

        // GET: /Store/Index
        public async Task<IActionResult> Index()
        {
            var stores = await _storeService.GetAllStoresAsync();
            return View(stores);
        }

        // GET: /Store/Create
        public IActionResult Create()
        {
            return View(new Store());
        }

        // POST: /Store/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Store store)
        {
            // 自訂驗證: 檢查重複
            if (await _storeService.IsDuplicateStoreAsync(store.Name, store.Phone, store.Address))
            {
                ModelState.AddModelError("", "此店家已存在（店名、電話、地址完全相同）");
            }

            if (!ModelState.IsValid)
            {
                return View(store);
            }

            try
            {
                await _storeService.AddStoreAsync(store);
                TempData["SuccessMessage"] = "店家新增成功";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "新增店家失敗");
                ModelState.AddModelError("", "新增店家失敗，請稍後再試");
                return View(store);
            }
        }

        // GET: /Store/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var store = await _storeService.GetStoreByIdAsync(id);
            if (store is null)
            {
                return NotFound("找不到指定的店家");
            }
            return View(store);
        }

        // GET: /Store/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var store = await _storeService.GetStoreByIdAsync(id);
            if (store is null)
            {
                return NotFound("找不到指定的店家");
            }
            return View(store);
        }

        // POST: /Store/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Store store)
        {
            if (id != store.Id)
            {
                return BadRequest();
            }

            // 檢查重複 (排除自己)
            if (await _storeService.IsDuplicateStoreAsync(
                store.Name, store.Phone, store.Address, store.Id))
            {
                ModelState.AddModelError("", "此店家已存在（店名、電話、地址完全相同）");
            }

            if (!ModelState.IsValid)
            {
                return View(store);
            }

            try
            {
                await _storeService.UpdateStoreAsync(store);
                TempData["SuccessMessage"] = "店家資料更新成功";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新店家失敗: {StoreId}", id);
                ModelState.AddModelError("", "更新店家失敗，請稍後再試");
                return View(store);
            }
        }

        // GET: /Store/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var store = await _storeService.GetStoreByIdAsync(id);
            if (store is null)
            {
                return NotFound("找不到指定的店家");
            }
            return View(store);
        }

        // POST: /Store/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var result = await _storeService.DeleteStoreAsync(id);
                if (!result)
                {
                    return NotFound("找不到指定的店家");
                }

                TempData["SuccessMessage"] = "店家刪除成功";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "刪除店家失敗: {StoreId}", id);
                TempData["ErrorMessage"] = "刪除店家失敗，請稍後再試";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
```

---

## 錯誤處理

### HTTP 狀態碼

| 狀態碼 | 情境 | 回應 |
|--------|------|------|
| 200 OK | 成功取得頁面 | 渲染 View |
| 302 Found | 操作成功後重導向 (PRG) | 重導向到 Index |
| 400 Bad Request | 請求參數錯誤 | 錯誤訊息 |
| 404 Not Found | 找不到指定店家 | 錯誤頁面 |
| 500 Internal Server Error | 伺服器錯誤 | 錯誤頁面 |

### ModelState 錯誤訊息格式

```csharp
// 欄位特定錯誤
ModelState.AddModelError("Name", "店家名稱為必填欄位");

// 全域錯誤
ModelState.AddModelError("", "此店家已存在（店名、電話、地址完全相同）");
```

### TempData 訊息

```csharp
// 成功訊息
TempData["SuccessMessage"] = "操作成功訊息";

// 錯誤訊息
TempData["ErrorMessage"] = "操作失敗訊息";
```

---

## 安全性

### Anti-Forgery Token

所有 POST 請求必須包含 Anti-Forgery Token，防止 CSRF 攻擊。

```html
@using (Html.BeginForm())
{
    @Html.AntiForgeryToken()
    <!-- 表單欄位 -->
}
```

### 輸入驗證

- **客戶端驗證**: jQuery Validation (即時回饋)
- **伺服器端驗證**: Data Annotations + ModelState (安全性保證)
- **雙重驗證**: 確保安全性和 UX

---

## 總結

API Contracts 定義了完整的 HTTP 端點、請求/回應格式、驗證規則和錯誤處理機制。採用標準 ASP.NET Core MVC 模式，簡單易懂，適合練習用專案。

**下一步**: 建立 quickstart.md 快速入門指南。
