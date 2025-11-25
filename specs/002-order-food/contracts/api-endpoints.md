# 訂餐功能系統 - API 端點契約

**Feature Branch**: `002-order-food`  
**Created**: 2025年11月23日  
**Status**: Complete

## 概述

本文件定義訂餐功能系統的所有 HTTP 端點（Controller Actions），包含路由、HTTP 方法、請求參數、回應格式和錯誤處理。本專案使用 ASP.NET Core MVC 架構，端點返回 HTML 視圖或 JSON 資料。

---

## Controller: OrderController

**基礎路由**: `/Order`

### 1. 餐廳列表頁面

**功能**: 顯示所有可訂餐的餐廳清單，若有進行中訂單則顯示提示區塊。

**路由**: `GET /Order/SelectRestaurant`

**請求參數**: 無

**回應**:
- **成功** (200 OK): 返回 `SelectRestaurant.cshtml` 視圖
  - 視圖模型: `List<Store>`
  - 進行中訂單提示: 若有 `Status = Pending` 的訂單，顯示提示區塊

**錯誤處理**:
- 無餐廳資料: 顯示「目前沒有可訂餐的餐廳」訊息
- 檔案讀取錯誤: 記錄日誌，顯示錯誤頁面

**C# Action 簽名**:
```csharp
/// <summary>
/// 顯示餐廳列表頁面
/// </summary>
[HttpGet]
public async Task<IActionResult> SelectRestaurant()
```

---

### 2. 菜單頁面

**功能**: 顯示指定餐廳的菜單，包含訂單摘要區塊（從 Session Storage 載入）。

**路由**: `GET /Order/Menu/{storeId}`

**請求參數**:
- `storeId` (路由參數, string, 必填): 餐廳ID

**回應**:
- **成功** (200 OK): 返回 `Menu.cshtml` 視圖
  - 視圖模型: `Store`（包含 `MenuItems`）
  - 訂單摘要: 由前端 JavaScript 從 Session Storage 載入並渲染

**錯誤處理**:
- 餐廳不存在: 返回 404，顯示「找不到指定的餐廳」錯誤頁面
- 菜單為空: 顯示「此餐廳目前沒有可訂購的菜品」訊息
- 檔案讀取錯誤: 記錄日誌，顯示錯誤頁面

**C# Action 簽名**:
```csharp
/// <summary>
/// 顯示餐廳菜單頁面
/// </summary>
/// <param name="storeId">餐廳ID</param>
[HttpGet("Menu/{storeId}")]
public async Task<IActionResult> Menu(string storeId)
```

---

### 3. 結帳頁面

**功能**: 顯示訂單明細和訂餐者資訊表單，接收購物車資料（從前端 Session Storage 傳遞）。

**路由**: `GET /Order/Checkout`

**請求參數**:
- `cartData` (查詢字串, JSON string, 必填): 購物車資料（前端從 Session Storage 讀取後傳遞）
  - 格式: `{ storeId, storeName, items: [{menuItemId, name, price, quantity}] }`

**回應**:
- **成功** (200 OK): 返回 `Checkout.cshtml` 視圖
  - 視圖模型: `CheckoutViewModel`（包含訂單明細和空白的訂餐者資訊表單）

**錯誤處理**:
- 購物車為空: 重定向至餐廳列表頁面，顯示「訂單為空，請先選擇菜品」訊息
- 購物車資料格式錯誤: 記錄日誌，顯示錯誤頁面

**C# Action 簽名**:
```csharp
/// <summary>
/// 顯示結帳頁面
/// </summary>
/// <param name="cartData">購物車資料（JSON 字串）</param>
[HttpGet]
public IActionResult Checkout([FromQuery] string cartData)
```

---

### 4. 提交訂單

**功能**: 驗證訂餐者資訊，建立訂單並儲存至 JSON 檔案，產生唯一訂單編號。

**路由**: `POST /Order/Submit`

**請求參數** (表單資料):
- `CustomerName` (string, 必填): 訂餐者姓名
- `CustomerPhone` (string, 必填): 訂餐者聯絡電話（僅數字）
- `CartData` (JSON string, 必填): 購物車資料（從前端 Session Storage 傳遞）
  - 格式: `{ storeId, storeName, items: [{menuItemId, name, price, quantity}] }`

**回應**:
- **成功** (200 OK): 重定向至訂單確認頁面 `/Order/Confirmation/{orderId}`
- **驗證失敗** (200 OK): 返回 `Checkout.cshtml` 視圖，顯示驗證錯誤訊息
  - ModelState 包含錯誤訊息（如「姓名為必填欄位」、「電話號碼僅能輸入數字」）

**錯誤處理**:
- 購物車為空: 返回錯誤訊息「訂單必須至少包含一個菜品」
- 訂單編號產生失敗: 記錄日誌，顯示「訂單建立失敗，請稍後再試」
- 檔案寫入錯誤: 記錄日誌，顯示「訂單儲存失敗，請聯絡客服」

**C# Action 簽名**:
```csharp
/// <summary>
/// 提交訂單
/// </summary>
/// <param name="model">結帳視圖模型（包含訂餐者資訊和購物車資料）</param>
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Submit(CheckoutViewModel model)
```

---

### 5. 訂單確認頁面

**功能**: 顯示訂單提交成功頁面，包含訂單編號和快速連結。

**路由**: `GET /Order/Confirmation/{orderId}`

**請求參數**:
- `orderId` (路由參數, string, 必填): 訂單編號

**回應**:
- **成功** (200 OK): 返回 `Confirmation.cshtml` 視圖
  - 視圖模型: `Order`（包含訂單編號和訂單摘要）
  - 快速連結: 「返回首頁」、「查看訂單紀錄」

**錯誤處理**:
- 訂單不存在: 返回 404，顯示「找不到指定的訂單」錯誤頁面

**C# Action 簽名**:
```csharp
/// <summary>
/// 顯示訂單確認頁面
/// </summary>
/// <param name="orderId">訂單編號</param>
[HttpGet("Confirmation/{orderId}")]
public async Task<IActionResult> Confirmation(string orderId)
```

---

### 6. 訂單紀錄頁面

**功能**: 顯示最近 5 天內的訂單清單（最新的在最上方）。

**路由**: `GET /Order/History`

**請求參數**: 無

**回應**:
- **成功** (200 OK): 返回 `History.cshtml` 視圖
  - 視圖模型: `OrderHistoryViewModel`（包含訂單清單）
  - 訂單清單欄位: 訂單編號、日期、餐廳名稱、總金額、狀態、「查看詳情」按鈕

**錯誤處理**:
- 無訂單紀錄: 顯示「目前沒有訂單紀錄」訊息
- 檔案讀取錯誤: 記錄日誌，顯示錯誤頁面

**C# Action 簽名**:
```csharp
/// <summary>
/// 顯示訂單紀錄頁面（最近 5 天）
/// </summary>
[HttpGet]
public async Task<IActionResult> History()
```

---

### 7. 訂單詳情頁面

**功能**: 顯示指定訂單的完整資訊（訂單明細、訂餐者資訊、訂單狀態）。

**路由**: `GET /Order/Details/{orderId}`

**請求參數**:
- `orderId` (路由參數, string, 必填): 訂單編號

**回應**:
- **成功** (200 OK): 返回 `Details.cshtml` 視圖
  - 視圖模型: `Order`（包含完整訂單資訊）
  - 顯示內容: 訂單編號、建立時間、餐廳名稱、訂餐者資訊、訂單項目明細、總金額、訂單狀態

**錯誤處理**:
- 訂單不存在: 返回 404，顯示「找不到指定的訂單」錯誤頁面

**C# Action 簽名**:
```csharp
/// <summary>
/// 顯示訂單詳情頁面
/// </summary>
/// <param name="orderId">訂單編號</param>
[HttpGet("Details/{orderId}")]
public async Task<IActionResult> Details(string orderId)
```

---

## Controller: HomeController（更新）

**基礎路由**: `/` 或 `/Home`

### 8. 首頁（更新）

**功能**: 在首頁新增「訂購餐點」按鈕，點擊後導向餐廳列表頁面。

**路由**: `GET /` 或 `GET /Home/Index`

**請求參數**: 無

**回應**:
- **成功** (200 OK): 返回 `Index.cshtml` 視圖（新增「訂購餐點」按鈕）

**變更說明**:
- 在現有 `Views/Home/Index.cshtml` 中新增按鈕或卡片
- 按鈕文字: 「訂購餐點」
- 按鈕連結: `/Order/SelectRestaurant`

**C# Action 簽名** (現有):
```csharp
/// <summary>
/// 首頁
/// </summary>
[HttpGet]
public IActionResult Index()
```

---

## 資料傳輸格式

### CartData JSON 格式

前端從 Session Storage 讀取購物車資料後，透過查詢字串或表單資料傳遞至後端：

```json
{
  "storeId": "STR001",
  "storeName": "美味餐廳",
  "items": [
    {
      "menuItemId": "MENU001",
      "name": "炸雞套餐",
      "price": 150.00,
      "quantity": 2
    },
    {
      "menuItemId": "MENU002",
      "name": "珍珠奶茶",
      "price": 65.00,
      "quantity": 1
    }
  ]
}
```

---

## 錯誤回應格式

### ModelState 驗證錯誤

當表單驗證失敗時，`ModelState` 包含錯誤訊息，視圖中使用 `asp-validation-for` 顯示：

```html
<!-- 範例：結帳頁面驗證錯誤 -->
<div class="form-group">
    <label asp-for="CustomerName" class="control-label">姓名</label>
    <input asp-for="CustomerName" class="form-control" />
    <span asp-validation-for="CustomerName" class="text-danger"></span>
</div>

<div class="form-group">
    <label asp-for="CustomerPhone" class="control-label">聯絡電話</label>
    <input asp-for="CustomerPhone" class="form-control" />
    <span asp-validation-for="CustomerPhone" class="text-danger"></span>
</div>
```

**驗證錯誤範例**:
- `CustomerName` 為空: 「姓名為必填欄位」
- `CustomerPhone` 為空: 「聯絡電話為必填欄位」
- `CustomerPhone` 包含非數字: 「電話號碼僅能輸入數字」
- `CustomerPhone` 超過 20 位數: 「電話號碼長度不可超過20位數」

### HTTP 錯誤頁面

- **404 Not Found**: 餐廳或訂單不存在時，重定向至 `/Home/Error` 並傳遞錯誤訊息
- **500 Internal Server Error**: 檔案讀寫錯誤、系統異常時，記錄日誌並顯示通用錯誤頁面

---

## 路由摘要表

| HTTP 方法 | 路由 | Controller Action | 功能說明 |
|----------|------|------------------|---------|
| GET | `/` | `HomeController.Index` | 首頁（新增「訂購餐點」按鈕） |
| GET | `/Order/SelectRestaurant` | `OrderController.SelectRestaurant` | 餐廳列表頁面 |
| GET | `/Order/Menu/{storeId}` | `OrderController.Menu` | 菜單頁面 |
| GET | `/Order/Checkout?cartData={json}` | `OrderController.Checkout` | 結帳頁面 |
| POST | `/Order/Submit` | `OrderController.Submit` | 提交訂單 |
| GET | `/Order/Confirmation/{orderId}` | `OrderController.Confirmation` | 訂單確認頁面 |
| GET | `/Order/History` | `OrderController.History` | 訂單紀錄頁面 |
| GET | `/Order/Details/{orderId}` | `OrderController.Details` | 訂單詳情頁面 |

---

## 前端互動流程

### 完整訂餐流程

```text
1. 首頁 (/)
   ↓ 點擊「訂購餐點」
2. 餐廳列表 (/Order/SelectRestaurant)
   ↓ 選擇餐廳
3. 菜單頁面 (/Order/Menu/{storeId})
   ↓ 加入菜品至購物車（Session Storage）
   ↓ 點擊「前往結帳」
4. 結帳頁面 (/Order/Checkout?cartData={json})
   ↓ 填寫訂餐者資訊
   ↓ 點擊「確認訂單」（POST /Order/Submit）
5. 訂單確認頁面 (/Order/Confirmation/{orderId})
   ↓ 點擊「查看訂單紀錄」
6. 訂單紀錄頁面 (/Order/History)
   ↓ 點擊「查看詳情」
7. 訂單詳情頁面 (/Order/Details/{orderId})
```

### 購物車管理（JavaScript + Session Storage）

**加入菜品流程**:
```text
1. 使用者在菜單頁面選擇菜品和數量
2. 點擊「加入訂單」
3. JavaScript 讀取 Session Storage 中的購物車資料
4. 若相同菜品已存在，累加數量；否則新增項目
5. 更新 Session Storage
6. 更新 UI 的訂單摘要區塊（菜品清單、數量、小計、總金額）
7. 啟用「前往結帳」按鈕
```

**前往結帳流程**:
```text
1. 使用者點擊「前往結帳」
2. JavaScript 讀取 Session Storage 中的購物車資料
3. 將購物車資料序列化為 JSON 字串
4. 重定向至 /Order/Checkout?cartData={json}
5. 後端解析 cartData 並渲染結帳頁面
```

**提交訂單流程**:
```text
1. 使用者填寫訂餐者資訊
2. 點擊「確認訂單」（表單 POST 提交）
3. 後端驗證表單資料和購物車資料
4. 建立訂單並儲存至 orders.json
5. 產生唯一訂單編號
6. 清除 Session Storage 中的購物車資料（JavaScript）
7. 重定向至訂單確認頁面
```

---

## 安全性考量

### CSRF 防護

所有 POST 請求必須包含 Anti-Forgery Token：

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Submit(CheckoutViewModel model)
```

對應的視圖中包含：

```html
<form method="post" asp-action="Submit">
    @Html.AntiForgeryToken()
    <!-- 表單欄位 -->
</form>
```

### 輸入驗證

- 前端驗證: JavaScript 即時驗證（姓名必填、電話格式、數量範圍）
- 後端驗證: Data Annotations + ModelState 驗證
- 雙重驗證: 永遠不信任客戶端輸入

### XSS 防護

- 使用 Razor 語法自動 HTML 編碼（`@Model.CustomerName`）
- 避免使用 `@Html.Raw()` 除非必要且已驗證安全性

---

## 效能考量

### 快取策略

- **靜態資源**: CSS, JS, Bootstrap 檔案使用瀏覽器快取
- **餐廳資料**: 考慮實作記憶體快取（`IMemoryCache`），避免每次請求都讀取 JSON 檔案
- **訂單資料**: 不快取（確保資料即時性）

### 非同步操作

所有檔案 I/O 操作使用 async/await 模式：

```csharp
public async Task<IActionResult> SelectRestaurant()
{
    var stores = await _storeService.GetAllStoresAsync();
    return View(stores);
}
```

---

## 測試建議

### 整合測試

使用 `WebApplicationFactory` 測試完整的請求-回應流程：

```csharp
[Fact]
public async Task SelectRestaurant_ReturnsViewWithStores()
{
    // Arrange
    var client = _factory.CreateClient();

    // Act
    var response = await client.GetAsync("/Order/SelectRestaurant");

    // Assert
    response.EnsureSuccessStatusCode();
    var content = await response.Content.ReadAsStringAsync();
    Assert.Contains("餐廳列表", content);
}
```

### 單元測試

測試 Controller Action 的邏輯和驗證：

```csharp
[Fact]
public async Task Submit_WithInvalidModelState_ReturnsView()
{
    // Arrange
    var controller = new OrderController(_orderService, _storeService);
    controller.ModelState.AddModelError("CustomerName", "姓名為必填欄位");
    var model = new CheckoutViewModel { /* invalid data */ };

    // Act
    var result = await controller.Submit(model);

    // Assert
    var viewResult = Assert.IsType<ViewResult>(result);
    Assert.False(controller.ModelState.IsValid);
}
```

---

## 總結

本 API 契約定義了訂餐功能系統的所有 HTTP 端點，遵循 ASP.NET Core MVC 最佳實踐：

1. **RESTful 路由**: 使用語意化的 URL（`/Order/SelectRestaurant`, `/Order/Menu/{storeId}`）
2. **驗證機制**: 雙重驗證（前端 + 後端）
3. **錯誤處理**: 清晰的錯誤訊息和 HTTP 狀態碼
4. **安全性**: CSRF 防護、輸入驗證、XSS 防護
5. **效能**: 非同步操作、快取策略
6. **可測試性**: 支援單元測試和整合測試

所有端點均包含詳細的文件註解，符合專案憲章的程式碼品質要求。
