# 訂餐功能系統 - 快速入門指南

**Feature Branch**: `002-order-food`  
**Created**: 2025年11月23日  
**Status**: Complete

## 概述

本指南幫助開發人員快速理解訂餐功能系統的架構、實作步驟和關鍵技術決策。閱讀本文件後，您應該能夠：

1. 理解訂餐流程的完整生命週期
2. 掌握關鍵元件和服務的職責
3. 了解前後端資料流動方式
4. 知道如何執行、測試和擴展功能

---

## 功能概述

訂餐功能系統讓使用者可以：

1. 瀏覽餐廳列表並選擇餐廳
2. 查看餐廳菜單並將菜品加入訂單
3. 在訂單摘要中即時查看已選菜品和總金額
4. 填寫訂餐者資訊並完成結帳
5. 取得唯一訂單編號
6. 查看訂單歷史紀錄（最近 5 天）

**技術棧**:
- ASP.NET Core 8.0 MVC
- C# 13
- Razor Views + Bootstrap 5
- JavaScript (ES6) + Session Storage
- JSON 檔案儲存
- Serilog 結構化日誌
- xUnit 測試框架

---

## 架構概覽

### 分層架構

```text
┌─────────────────────────────────────────┐
│          Presentation Layer             │
│  (Views, Controllers, JavaScript)       │
│  - Order/SelectRestaurant.cshtml        │
│  - Order/Menu.cshtml                    │
│  - Order/Checkout.cshtml                │
│  - wwwroot/js/order.js                  │
└──────────────────┬──────────────────────┘
                   │
┌──────────────────▼──────────────────────┐
│         Business Logic Layer            │
│         (Services, Models)              │
│  - OrderService                         │
│  - StoreService                         │
│  - Order, OrderItem Models              │
└──────────────────┬──────────────────────┘
                   │
┌──────────────────▼──────────────────────┐
│          Data Access Layer              │
│    (IFileStorage, JsonFileStorage)      │
│  - orders.json                          │
│  - stores.json                          │
└─────────────────────────────────────────┘
```

### 關鍵元件

| 元件 | 職責 | 檔案位置 |
|------|------|---------|
| `OrderController` | 處理訂餐流程的所有 HTTP 請求 | `Controllers/OrderController.cs` |
| `IOrderService` | 訂單業務邏輯介面 | `Services/IOrderService.cs` |
| `OrderService` | 訂單業務邏輯實作（建立訂單、查詢訂單、清理舊訂單） | `Services/OrderService.cs` |
| `Order` | 訂單模型 | `Models/Order.cs` |
| `OrderItem` | 訂單項目模型 | `Models/OrderItem.cs` |
| `OrderStatus` | 訂單狀態列舉 | `Models/OrderStatus.cs` |
| `IFileStorage` | 檔案儲存介面（現有） | `Data/IFileStorage.cs` |
| `JsonFileStorage` | JSON 檔案儲存實作（現有） | `Data/JsonFileStorage.cs` |
| `order.js` | 前端購物車管理和 Session Storage 操作 | `wwwroot/js/order.js` |

---

## 快速開始

### 前置條件

- .NET 8.0 SDK
- Visual Studio 2022 或 VS Code
- 基本的 C# 和 ASP.NET Core MVC 知識
- 基本的 JavaScript 和 Bootstrap 知識

### 執行專案

```bash
# 1. 切換至功能分支
git checkout 002-order-food

# 2. 還原相依套件
cd OrderLunchWeb
dotnet restore

# 3. 建置專案
dotnet build

# 4. 執行專案
dotnet run

# 5. 開啟瀏覽器
# 導向 https://localhost:5001（或顯示的 URL）
```

### 執行測試

```bash
# 執行所有測試
cd OrderLunchWeb.Tests
dotnet test

# 執行特定測試類別
dotnet test --filter "FullyQualifiedName~OrderServiceTests"

# 執行測試並產生覆蓋率報告
dotnet test /p:CollectCoverage=true /p:CoverageReportsFormat=cobertura
```

---

## 實作步驟摘要

### Phase 1: 資料模型和服務層

1. **建立 Models**:
   - `Order.cs`: 訂單模型（訂單編號、餐廳資訊、訂餐者資訊、訂單項目、狀態、建立時間）
   - `OrderItem.cs`: 訂單項目模型（菜品資訊快照、數量、小計）
   - `OrderStatus.cs`: 訂單狀態列舉（Pending, Confirmed, Preparing, Completed, Cancelled）

2. **建立 Services**:
   - `IOrderService.cs`: 定義訂單服務介面
     - `CreateOrderAsync(Order order)`: 建立訂單
     - `GetOrderByIdAsync(string orderId)`: 查詢訂單
     - `GetRecentOrdersAsync(int days)`: 取得最近 N 天的訂單
     - `GetPendingOrdersAsync()`: 取得進行中的訂單
     - `CleanupOldOrdersAsync(int days)`: 清理舊訂單
   - `OrderService.cs`: 實作訂單服務邏輯

3. **更新 Program.cs**:
   - 註冊 `IOrderService` 和 `OrderService` 至 DI 容器
   - 在應用程式啟動時執行舊訂單清理邏輯

### Phase 2: Controller 和 Views

4. **建立 OrderController**:
   - `SelectRestaurant()`: 餐廳列表頁面
   - `Menu(string storeId)`: 菜單頁面
   - `Checkout(string cartData)`: 結帳頁面
   - `Submit(CheckoutViewModel model)`: 提交訂單
   - `Confirmation(string orderId)`: 訂單確認頁面
   - `History()`: 訂單紀錄頁面
   - `Details(string orderId)`: 訂單詳情頁面

5. **建立 Views**:
   - `Views/Order/SelectRestaurant.cshtml`: 餐廳列表
   - `Views/Order/Menu.cshtml`: 菜單頁面（含訂單摘要區塊）
   - `Views/Order/Checkout.cshtml`: 結帳頁面（訂單明細 + 訂餐者資訊表單）
   - `Views/Order/Confirmation.cshtml`: 訂單確認頁面
   - `Views/Order/History.cshtml`: 訂單紀錄頁面
   - `Views/Order/Details.cshtml`: 訂單詳情頁面

6. **更新 Home/Index.cshtml**:
   - 新增「訂購餐點」按鈕，連結至 `/Order/SelectRestaurant`

### Phase 3: 前端 JavaScript

7. **建立 wwwroot/js/order.js**:
   - `CartStorage`: 封裝 Session Storage 操作（save, load, clear）
   - `addToCart()`: 加入菜品至購物車
   - `updateOrderSummary()`: 更新訂單摘要 UI
   - `goToCheckout()`: 前往結帳頁面（傳遞購物車資料）
   - `checkTimeout()`: 結帳逾時檢查（30 分鐘）

### Phase 4: 測試

8. **單元測試**:
   - `OrderServiceTests.cs`: 測試訂單業務邏輯
     - 建立訂單
     - 查詢訂單
     - 取得最近訂單
     - 清理舊訂單
     - 訂單編號唯一性
     - 金額計算正確性

9. **整合測試**:
   - `OrderControllerTests.cs`: 測試完整的訂餐流程
     - 餐廳列表載入
     - 菜單顯示
     - 訂單提交（含驗證）
     - 訂單確認
     - 訂單紀錄查詢

---

## 關鍵概念

### 1. 購物車狀態管理（Session Storage）

**為什麼使用 Session Storage?**
- 減輕伺服器負擔（狀態儲存在客戶端）
- 簡化架構（無需設定 ASP.NET Core Session）
- 分頁關閉自動清空（符合需求）

**資料結構**:
```javascript
{
  storeId: "STR001",
  storeName: "美味餐廳",
  items: [
    { menuItemId: "MENU001", name: "炸雞套餐", price: 150.00, quantity: 2 },
    { menuItemId: "MENU002", name: "珍珠奶茶", price: 65.00, quantity: 1 }
  ]
}
```

**操作流程**:
1. 加入菜品 → 更新 Session Storage → 更新 UI
2. 前往結帳 → 讀取 Session Storage → 傳遞至後端
3. 訂單成功 → 清除 Session Storage

---

### 2. 快照機制（避免歷史資料失效）

**問題**: 餐廳改名或菜品調價後，歷史訂單該如何顯示？

**解決方案**: 儲存快照資料

- `Order.StoreName`: 儲存訂單建立時的餐廳名稱
- `OrderItem.MenuItemName`: 儲存訂單建立時的菜品名稱
- `OrderItem.Price`: 儲存訂單建立時的價格

**實作**:
```csharp
// 建立訂單時，從 Store 和 MenuItem 複製快照資料
var order = new Order
{
    OrderId = GenerateOrderId(),
    StoreId = store.StoreId,
    StoreName = store.Name,  // 快照
    Items = cartItems.Select(ci => new OrderItem
    {
        MenuItemId = ci.MenuItemId,
        MenuItemName = ci.Name,  // 快照
        Price = ci.Price,        // 快照
        Quantity = ci.Quantity
    }).ToList()
};
```

---

### 3. 訂單編號產生策略

**格式**: `ORD{yyyyMMddHHmmssfff}`

**範例**: `ORD20251123143025123`（2025年11月23日 14:30:25.123）

**優點**:
- 唯一性高（毫秒級時間戳記）
- 可讀性強（包含日期時間）
- 易於排序（字串排序 = 時間排序）

**實作**:
```csharp
public string GenerateOrderId()
{
    return $"ORD{DateTime.Now:yyyyMMddHHmmssfff}";
}
```

---

### 4. 金額計算與精確度

**使用 `decimal` 型別**:
```csharp
public class OrderItem
{
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    
    // 小計計算：四捨五入到小數點第 2 位
    public decimal Subtotal => Math.Round(Price * Quantity, 2);
}

public class Order
{
    public List<OrderItem> Items { get; set; } = new();
    
    // 訂單總金額：所有小計的加總
    public decimal TotalAmount => Items.Sum(i => i.Subtotal);
}
```

**顯示格式**:
```csharp
// Razor View
@Model.TotalAmount.ToString("N2")  // 輸出: 1,234.56
@String.Format("NT$ {0:N2}", Model.TotalAmount)  // 輸出: NT$ 1,234.56
```

---

### 5. 舊訂單自動清理

**觸發時機**: 應用程式啟動時

**實作位置**: `Program.cs`

**邏輯**:
```csharp
// Program.cs
var app = builder.Build();

// 應用程式啟動時清理舊訂單
try
{
    using var scope = app.Services.CreateScope();
    var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
    var deletedCount = await orderService.CleanupOldOrdersAsync(days: 5);
    Log.Information("清理了 {Count} 筆超過 5 天的舊訂單", deletedCount);
}
catch (Exception ex)
{
    Log.Error(ex, "清理舊訂單時發生錯誤");
}

app.Run();
```

---

## 資料流動範例

### 完整訂餐流程的資料流動

```text
1. 選擇餐廳
   Browser ─GET /Order/SelectRestaurant─> OrderController
                                           ↓
                                      StoreService
                                           ↓
                                      IFileStorage (stores.json)
                                           ↓
                                      ← List<Store> ─
                                           ↓
   Browser ← SelectRestaurant.cshtml View ─┘

2. 瀏覽菜單
   Browser ─GET /Order/Menu/STR001─> OrderController
                                           ↓
                                      StoreService.GetStoreById()
                                           ↓
   Browser ← Menu.cshtml (Store + MenuItems) ─┘

3. 加入訂單（前端）
   User clicks "加入訂單"
      ↓
   JavaScript reads current cart from Session Storage
      ↓
   JavaScript adds/updates item in cart
      ↓
   JavaScript saves cart to Session Storage
      ↓
   JavaScript updates UI (Order Summary)

4. 前往結帳
   User clicks "前往結帳"
      ↓
   JavaScript reads cart from Session Storage
      ↓
   JavaScript redirects to /Order/Checkout?cartData={json}
      ↓
   OrderController receives cartData
      ↓
   Deserializes cartData into CheckoutViewModel
      ↓
   Browser ← Checkout.cshtml (Order Summary + Customer Info Form) ─┘

5. 提交訂單
   User fills form and clicks "確認訂單"
      ↓
   Browser ─POST /Order/Submit (CustomerName, CustomerPhone, CartData)─> OrderController
                                                                             ↓
                                                                        Validate ModelState
                                                                             ↓
                                                                        OrderService.CreateOrderAsync()
                                                                             ↓
                                                                        Generate OrderId
                                                                             ↓
                                                                        IFileStorage.SaveAsync(orders.json)
                                                                             ↓
   Browser ← Redirect to /Order/Confirmation/{orderId} ──────────────────────┘

6. 訂單確認
   Browser ─GET /Order/Confirmation/ORD20251123143025123─> OrderController
                                                               ↓
                                                          OrderService.GetOrderByIdAsync()
                                                               ↓
   Browser ← Confirmation.cshtml (Order Details) ─────────────┘
```

---

## 常見問題

### Q1: 為什麼使用 JSON 檔案而非資料庫？

**A**: 練習專案聚焦於 MVC 架構和業務邏輯，JSON 檔案簡化了資料存取層的複雜度。未來可輕鬆遷移至 Entity Framework Core + SQL Server。

---

### Q2: 購物車資料會在伺服器端儲存嗎？

**A**: 不會。購物車資料僅儲存於瀏覽器的 Session Storage，直到使用者提交訂單後才儲存至伺服器的 `orders.json`。

---

### Q3: 如何處理訂單編號衝突？

**A**: 使用毫秒級時間戳記（`yyyyMMddHHmmssfff`）產生訂單編號，在小型專案中衝突機率極低。若仍有疑慮，可在訂單編號後加入 3 位隨機數字。

---

### Q4: 訂單狀態會自動變更嗎？

**A**: 本版本（v1）不實作訂單狀態變更功能，所有訂單建立後維持 `Pending` 狀態。未來版本可加入狀態流轉邏輯（Pending → Confirmed → Preparing → Completed）。

---

### Q5: 如何處理中文亂碼問題？

**A**: 使用 `System.Text.Json` 序列化時，設定 `Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping`，並確保檔案以 UTF-8 編碼儲存。

```csharp
var options = new JsonSerializerOptions
{
    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    WriteIndented = true
};
```

---

### Q6: 結帳逾時後購物車會清空嗎？

**A**: 不會。結帳逾時僅顯示提示訊息「訂單已逾時，請重新選擇菜品」，但不強制清空購物車，使用者仍可繼續結帳。

---

### Q7: 如何測試舊訂單清理功能？

**A**: 手動修改 `orders.json` 中的 `createdAt` 欄位（設為 6 天前的日期），然後重啟應用程式，觀察日誌確認清理成功。

---

## 擴展指南

### 未來可能的功能擴展

1. **使用者認證系統**:
   - 加入 `UserId` 欄位至 Order 模型
   - 實作登入/註冊功能（JWT 或 Cookie Authentication）
   - 訂單僅能由建立者查看和修改

2. **訂單狀態管理**:
   - 實作訂單狀態變更 API
   - 加入訂單狀態歷史記錄
   - 餐廳管理員可確認和準備訂單

3. **通知機制**:
   - 訂單確認後發送 Email 或 SMS 通知
   - 訂單狀態變更時推送通知

4. **支付整合**:
   - 整合第三方支付（如綠界、藍新）
   - 加入支付狀態和交易記錄

5. **評論系統**:
   - 訂單完成後允許使用者評論餐廳和菜品
   - 顯示餐廳評分和評論

6. **資料庫遷移**:
   - 使用 Entity Framework Core 取代 JSON 檔案
   - 實作 Migration 和 Seeding
   - 加入資料庫索引優化查詢效能

---

## 參考資源

### 官方文件

- [ASP.NET Core MVC](https://learn.microsoft.com/zh-tw/aspnet/core/mvc/overview)
- [Dependency Injection](https://learn.microsoft.com/zh-tw/aspnet/core/fundamentals/dependency-injection)
- [Model Validation](https://learn.microsoft.com/zh-tw/aspnet/core/mvc/models/validation)
- [Session Storage API](https://developer.mozilla.org/zh-TW/docs/Web/API/Window/sessionStorage)

### 專案文件

- [Feature Specification](./spec.md): 功能規格和使用者故事
- [Implementation Plan](./plan.md): 實作計畫和憲章檢查
- [Research](./research.md): 技術研究和決策理由
- [Data Model](./data-model.md): 資料模型定義
- [API Endpoints](./contracts/api-endpoints.md): HTTP 端點契約

---

## 下一步

1. **閱讀完整規格**: 詳細閱讀 [spec.md](./spec.md) 了解所有使用者故事和驗收標準
2. **查看資料模型**: 詳細閱讀 [data-model.md](./data-model.md) 了解實體定義和關聯
3. **研究 API 契約**: 詳細閱讀 [contracts/api-endpoints.md](./contracts/api-endpoints.md) 了解所有端點定義
4. **開始實作**: 按照 Phase 1 → Phase 2 → Phase 3 → Phase 4 的順序實作功能
5. **執行測試**: 使用 TDD 方法，先寫測試再實作功能
6. **提交 Pull Request**: 實作完成後提交 PR，等待程式碼審查

---

## 聯絡資訊

如有任何問題或建議，請在 GitHub Issue 中討論或聯絡專案維護者。

**Happy Coding! 🚀**
