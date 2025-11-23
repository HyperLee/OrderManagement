# 訂餐功能設計規格

**功能代號**: 002-order-food  
**建立日期**: 2025年11月23日  
**狀態**: 設計階段  
**前置功能**: 001-store-menu-management (店家與菜單管理系統)

## 功能概述

提供用戶選擇餐廳並訂購餐點的完整流程,包含餐廳選擇、菜品瀏覽、購物車管理、訂單結帳與訂單歷史查詢功能。

## 使用者故事

### 故事 1: 訂購餐點入口 (優先級: P1)

**描述**: 用戶從首頁進入訂餐功能  
**角色**: 訂餐用戶  
**目標**: 快速啟動訂餐流程

**驗收條件**:
1. **Given** 用戶在首頁,**When** 查看主功能選單,**Then** 顯示「訂購餐點」按鈕
2. **Given** 用戶點擊「訂購餐點」按鈕,**When** 按鈕可用,**Then** 跳轉至餐廳列表頁面

**實作位置**: `OrderLunchWeb/Views/Home/Index.cshtml`

**目前狀態**:
```csharp
<button class="btn btn-lg btn-secondary" disabled>
    <i class="bi bi-cart"></i> 訂購餐點 (即將開放)
</button>
```

**更新為**:
```csharp
<a href="@Url.Action("Index", "Order")" class="btn btn-lg btn-success">
    <i class="bi bi-cart"></i> 訂購餐點
</a>
```

---

### 故事 2: 瀏覽餐廳列表 (優先級: P1)

**描述**: 用戶查看可訂購的餐廳清單  
**角色**: 訂餐用戶  
**目標**: 選擇想要訂餐的餐廳

**頁面**: `/Order/Index` (餐廳列表頁面)

**資料來源**: 從 `Data/stores.json` 讀取所有餐廳資料

**顯示內容**:
- 餐廳名稱
- 餐廳地址
- 聯絡電話
- 營業時間
- 進行中的訂單提示 (如果存在)

**驗收條件**:
1. **Given** 系統中有餐廳資料,**When** 用戶進入餐廳列表頁面,**Then** 顯示所有可訂餐的餐廳卡片
2. **Given** 用戶當前有進行中的訂單,**When** 進入餐廳列表,**Then** 頁面頂部顯示「您有進行中的訂單」提醒區塊,包含:
   - 訂單編號
   - 餐廳名稱
   - 訂單狀態 (待確認/已確認/準備中)
   - 「查看訂單詳情」按鈕
   - 提示文字:「已有訂單進行中,是否繼續點餐?」
3. **Given** 用戶點擊餐廳卡片,**When** 選擇餐廳,**Then** 跳轉至該餐廳的菜品列表頁面
4. **Given** 系統中無餐廳資料,**When** 用戶進入列表頁面,**Then** 顯示「目前沒有可訂餐的餐廳」訊息

**UI要求**:
- 使用卡片式佈局展示餐廳
- 每張卡片包含餐廳資訊和「選擇此餐廳」按鈕
- 採用食物主題配色 (暖色調,如橙色、黃色、紅色)

---

### 故事 3: 瀏覽餐廳菜單並加入訂單 (優先級: P1)

**描述**: 用戶查看餐廳菜品並選擇要訂購的項目  
**角色**: 訂餐用戶  
**目標**: 選擇菜品並加入購物車

**頁面**: `/Order/Menu?storeId={id}` (餐廳菜單頁面)

**資料來源**: 從 `Data/stores.json` 讀取指定餐廳的菜單資料

**顯示內容**:
- 餐廳名稱 (頁面標題)
- 每個菜品顯示:
  - 菜品名稱
  - 價格 (整數)
  - 數量選擇器 (+/- 按鈕 + 數字輸入框)
  - 「加入訂單」按鈕
- 當前訂單摘要區塊 (固定在頁面右側或底部):
  - 已選菜品清單
  - 每項菜品的數量與小計
  - 訂單總金額 (顯示至小數點後兩位)
  - 「前往結帳」按鈕

**驗收條件**:
1. **Given** 用戶在菜單頁面,**When** 查看菜品,**Then** 顯示該餐廳所有菜品的名稱和價格
2. **Given** 用戶點擊 `+` 按鈕,**When** 增加數量,**Then** 數字輸入框數值 +1
3. **Given** 用戶點擊 `-` 按鈕,**When** 減少數量且當前數量 > 1,**Then** 數字輸入框數值 -1
4. **Given** 數量為 1,**When** 用戶點擊 `-` 按鈕,**Then** 數量維持為 1 (不可小於 1)
5. **Given** 用戶直接在輸入框輸入數字,**When** 輸入 0 或負數,**Then** 自動修正為 1
6. **Given** 用戶直接在輸入框輸入數字,**When** 輸入非整數 (小數或文字),**Then** 顯示驗證錯誤訊息
7. **Given** 用戶選擇數量並點擊「加入訂單」,**When** 數量為正整數,**Then** 將菜品加入當前訂單摘要
8. **Given** 用戶重複加入相同菜品,**When** 該菜品已在訂單中,**Then** 累加數量而非新增重複項目
9. **Given** 訂單摘要中有菜品,**When** 用戶修改或新增菜品,**Then** 即時更新訂單總金額 (格式: `NT$ 1,234.00`)
10. **Given** 訂單摘要為空,**When** 用戶嘗試點擊「前往結帳」,**Then** 按鈕為 disabled 狀態
11. **Given** 訂單摘要有菜品,**When** 用戶點擊「前往結帳」,**Then** 跳轉至結帳頁面

**數量驗證規則**:
- 必須為正整數
- 不可為 0 或負數
- 不可包含小數點
- 預設值為 1

**UI要求**:
- 菜品清單使用表格或清單呈現
- 數量選擇器緊鄰每個菜品
- 訂單摘要使用固定區塊,方便用戶隨時查看
- 價格數字使用千分位逗號格式化

---

### 故事 4: 訂單結帳 (優先級: P1)

**描述**: 用戶確認訂單內容並提交訂單  
**角色**: 訂餐用戶  
**目標**: 完成訂單提交並取得訂單編號

**頁面**: `/Order/Checkout` (結帳頁面)

**顯示內容**:
- 訂單明細表格:
  - 菜品名稱
  - 單價
  - 數量
  - 小計 (單價 × 數量)
- 訂單總金額 (顯示至小數點後兩位)
- 訂餐者資訊輸入表單:
  - 姓名 (必填)
  - 聯絡電話 (必填,純數字)
- 「確認訂單」按鈕
- 「返回修改」按鈕

**驗收條件**:
1. **Given** 用戶在結帳頁面,**When** 查看訂單,**Then** 顯示完整的訂單明細表格
2. **Given** 用戶未填寫姓名,**When** 點擊「確認訂單」,**Then** 顯示「姓名為必填欄位」錯誤訊息
3. **Given** 用戶未填寫電話,**When** 點擊「確認訂單」,**Then** 顯示「聯絡電話為必填欄位」錯誤訊息
4. **Given** 用戶填寫的電話包含非數字字元,**When** 點擊「確認訂單」,**Then** 顯示「電話號碼僅能輸入數字」錯誤訊息
5. **Given** 用戶正確填寫所有資訊,**When** 點擊「確認訂單」,**Then**:
   - 產生唯一訂單編號 (格式: `ORD{yyyyMMddHHmmss}`)
   - 訂單資料寫入 `Data/orders.json`
   - 顯示訂單提交成功頁面,包含訂單編號
   - 提供「返回首頁」和「查看訂單紀錄」連結
6. **Given** 用戶點擊「返回修改」,**When** 返回菜單頁面,**Then** 保留當前已選擇的菜品

**資料儲存位置**: `Data/orders.json` (新建檔案)

**訂單資料結構** (參考下方資料模型)

---

### 故事 5: 訂單紀錄查詢 (優先級: P2)

**描述**: 用戶查看歷史訂單紀錄  
**角色**: 訂餐用戶  
**目標**: 追蹤過往訂單

**頁面**: `/Order/History` (訂單紀錄頁面)

**資料來源**: 從 `Data/orders.json` 讀取訂單資料 (僅顯示最近 5 天)

**顯示內容**:
- 訂單清單 (最新的在最上方):
  - 訂單編號
  - 訂購日期時間
  - 餐廳名稱
  - 訂單總金額
  - 訂單狀態
  - 「查看詳情」按鈕

**驗收條件**:
1. **Given** 系統中有訂單紀錄,**When** 用戶進入訂單紀錄頁面,**Then** 顯示最近 5 天內的訂單清單
2. **Given** 訂單超過 5 天,**When** 系統執行自動清理,**Then** 從 `orders.json` 中移除舊訂單
3. **Given** 用戶點擊「查看詳情」,**When** 選擇訂單,**Then** 顯示該訂單的完整資訊
4. **Given** 系統中無訂單紀錄,**When** 用戶進入頁面,**Then** 顯示「目前沒有訂單紀錄」訊息

**首頁整合**:
在 `OrderLunchWeb/Views/Home/Index.cshtml` 新增第三個功能按鈕:

```csharp
<a href="@Url.Action("History", "Order")" class="btn btn-lg btn-info">
    <i class="bi bi-clock-history"></i> 訂餐紀錄
</a>
```

目前狀態為:
```csharp
<button class="btn btn-lg btn-secondary" disabled>
    <i class="bi bi-clock-history"></i> 訂餐紀錄 (即將開放)
</button>
```

---

## 資料模型設計

### Order (訂單)

**檔案位置**: `OrderLunchWeb/Models/Order.cs` (新建)

**屬性定義**:

| 屬性名稱 | 型別 | 必填 | 說明 |
|---------|------|------|------|
| `OrderId` | `string` | ✅ | 訂單唯一編號,格式: `ORD{yyyyMMddHHmmss}` |
| `StoreId` | `int` | ✅ | 餐廳 ID (關聯至 Store) |
| `StoreName` | `string` | ✅ | 餐廳名稱 (冗餘儲存,避免店家刪除後資料遺失) |
| `CustomerName` | `string` | ✅ | 訂餐者姓名 (最多 50 字元) |
| `CustomerPhone` | `string` | ✅ | 訂餐者電話 (純數字) |
| `OrderItems` | `List<OrderItem>` | ✅ | 訂單項目清單 (至少 1 項) |
| `TotalAmount` | `decimal` | ✅ | 訂單總金額 (精確至小數點後兩位) |
| `OrderStatus` | `OrderStatus` (enum) | ✅ | 訂單狀態 |
| `CreatedAt` | `DateTime` | ✅ | 訂單建立時間 |

**驗證規則**:
- `OrderId`: 格式必須為 `ORD` + 14 位數字
- `CustomerName`: 不可空白,長度 1-50 字元
- `CustomerPhone`: 僅允許數字字元
- `OrderItems`: 至少包含 1 項菜品
- `TotalAmount`: 必須 ≥ 0,自動計算 (不可手動輸入)

**C# 類別定義**:

```csharp
namespace OrderLunchWeb.Models
{
    /// <summary>
    /// 代表一筆訂單
    /// </summary>
    public class Order
    {
        /// <summary>
        /// 訂單唯一編號 (格式: ORDyyyyMMddHHmmss)
        /// </summary>
        [Required]
        [RegularExpression(@"^ORD\d{14}$", ErrorMessage = "訂單編號格式錯誤")]
        public string OrderId { get; set; } = string.Empty;

        /// <summary>
        /// 餐廳 ID
        /// </summary>
        [Required]
        public int StoreId { get; set; }

        /// <summary>
        /// 餐廳名稱 (快照)
        /// </summary>
        [Required]
        [StringLength(100)]
        public string StoreName { get; set; } = string.Empty;

        /// <summary>
        /// 訂餐者姓名
        /// </summary>
        [Required(ErrorMessage = "姓名為必填欄位")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "姓名長度必須介於 1 到 50 字元")]
        [Display(Name = "訂餐者姓名")]
        public string CustomerName { get; set; } = string.Empty;

        /// <summary>
        /// 訂餐者聯絡電話
        /// </summary>
        [Required(ErrorMessage = "聯絡電話為必填欄位")]
        [RegularExpression(@"^\d+$", ErrorMessage = "電話號碼僅能輸入數字")]
        [Display(Name = "聯絡電話")]
        public string CustomerPhone { get; set; } = string.Empty;

        /// <summary>
        /// 訂單項目清單
        /// </summary>
        [Required]
        [MinLength(1, ErrorMessage = "訂單必須至少包含一個項目")]
        public List<OrderItem> OrderItems { get; set; } = new();

        /// <summary>
        /// 訂單總金額
        /// </summary>
        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 訂單狀態
        /// </summary>
        [Required]
        public OrderStatus OrderStatus { get; set; }

        /// <summary>
        /// 訂單建立時間
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
```

---

### OrderItem (訂單項目)

**檔案位置**: `OrderLunchWeb/Models/OrderItem.cs` (新建)

**屬性定義**:

| 屬性名稱 | 型別 | 必填 | 說明 |
|---------|------|------|------|
| `MenuItemId` | `int` | ✅ | 菜品 ID (關聯至 MenuItem) |
| `MenuItemName` | `string` | ✅ | 菜品名稱 (快照) |
| `UnitPrice` | `int` | ✅ | 單價 (快照,避免價格變動影響歷史訂單) |
| `Quantity` | `int` | ✅ | 訂購數量 (必須為正整數) |
| `Subtotal` | `int` | ✅ | 小計 (UnitPrice × Quantity) |

**驗證規則**:
- `Quantity`: 必須 ≥ 1
- `UnitPrice`: 必須 ≥ 0
- `Subtotal`: 自動計算,必須等於 `UnitPrice × Quantity`

**C# 類別定義**:

```csharp
namespace OrderLunchWeb.Models
{
    /// <summary>
    /// 代表訂單中的單一菜品項目
    /// </summary>
    public class OrderItem
    {
        /// <summary>
        /// 菜品 ID
        /// </summary>
        [Required]
        public int MenuItemId { get; set; }

        /// <summary>
        /// 菜品名稱 (快照)
        /// </summary>
        [Required]
        [StringLength(50)]
        public string MenuItemName { get; set; } = string.Empty;

        /// <summary>
        /// 單價 (快照)
        /// </summary>
        [Required]
        [Range(0, int.MaxValue)]
        public int UnitPrice { get; set; }

        /// <summary>
        /// 訂購數量
        /// </summary>
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "數量必須為正整數")]
        public int Quantity { get; set; }

        /// <summary>
        /// 小計 (自動計算)
        /// </summary>
        [Required]
        public int Subtotal { get; set; }
    }
}
```

---

### OrderStatus (訂單狀態列舉)

**檔案位置**: `OrderLunchWeb/Models/OrderStatus.cs` (新建)

```csharp
namespace OrderLunchWeb.Models
{
    /// <summary>
    /// 訂單狀態
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// 待確認
        /// </summary>
        Pending = 0,

        /// <summary>
        /// 已確認
        /// </summary>
        Confirmed = 1,

        /// <summary>
        /// 準備中
        /// </summary>
        Preparing = 2,

        /// <summary>
        /// 已完成
        /// </summary>
        Completed = 3,

        /// <summary>
        /// 已取消
        /// </summary>
        Cancelled = 4
    }
}
```

---

## 技術架構

### Controller

**新增檔案**: `OrderLunchWeb/Controllers/OrderController.cs`

**Actions**:
- `Index()`: 顯示餐廳列表
- `Menu(int storeId)`: 顯示餐廳菜單
- `Checkout()`: 顯示結帳頁面
- `ConfirmOrder(Order order)`: 處理訂單提交
- `History()`: 顯示訂單紀錄
- `Details(string orderId)`: 顯示訂單詳情

### Service

**新增檔案**: `OrderLunchWeb/Services/IOrderService.cs`

**方法**:
```csharp
public interface IOrderService
{
    /// <summary>
    /// 取得所有訂單 (最近 5 天)
    /// </summary>
    Task<List<Order>> GetRecentOrdersAsync();

    /// <summary>
    /// 根據訂單編號取得訂單
    /// </summary>
    Task<Order?> GetOrderByIdAsync(string orderId);

    /// <summary>
    /// 建立新訂單
    /// </summary>
    Task<string> CreateOrderAsync(Order order);

    /// <summary>
    /// 取得進行中的訂單 (狀態為 Pending, Confirmed, Preparing)
    /// </summary>
    Task<List<Order>> GetActiveOrdersAsync();

    /// <summary>
    /// 刪除超過 5 天的訂單
    /// </summary>
    Task CleanupOldOrdersAsync();
}
```

**新增檔案**: `OrderLunchWeb/Services/OrderService.cs`

實作 `IOrderService` 介面,使用 `JsonFileStorage` 讀寫 `Data/orders.json`

### Views

**新增檔案**:
- `OrderLunchWeb/Views/Order/Index.cshtml` (餐廳列表)
- `OrderLunchWeb/Views/Order/Menu.cshtml` (餐廳菜單)
- `OrderLunchWeb/Views/Order/Checkout.cshtml` (結帳頁面)
- `OrderLunchWeb/Views/Order/Success.cshtml` (訂單成功)
- `OrderLunchWeb/Views/Order/History.cshtml` (訂單紀錄)
- `OrderLunchWeb/Views/Order/Details.cshtml` (訂單詳情)

---

## UI/UX 設計需求

### 主題風格

採用「食物訂餐」主題設計,區別於店家管理功能的商務風格。

**配色方案**:
- 主色: `#FF6B35` (橙紅色,象徵食慾與活力)
- 次色: `#F7931E` (橙黃色,溫暖感)
- 背景色: `#FFF8F0` (米白色,柔和不刺眼)
- 文字色: `#333333` (深灰色)
- 成功色: `#4CAF50` (綠色,確認動作)
- 警告色: `#FFC107` (黃色,提示訊息)

**元素設計**:
- 餐廳卡片: 使用圓角、陰影效果,呈現立體感
- 按鈕: 大型、明顯,易於點擊 (行動裝置友善)
- 圖示: 使用 Bootstrap Icons 的食物相關圖示 (如 `bi-shop`, `bi-cart`, `bi-receipt`)
- 字體: 使用較大的字級,提升可讀性

**參考網站**:
- Foodpanda
- Uber Eats
- 街口支付訂餐

### 響應式設計

- 桌面版 (≥992px): 餐廳卡片 3 欄排列,訂單摘要固定在右側
- 平板版 (768-991px): 餐廳卡片 2 欄排列,訂單摘要固定在右側
- 行動版 (<768px): 餐廳卡片 1 欄排列,訂單摘要固定在頁面底部

---

## 資料管理規則

### 訂單資料保留期限

訂單資料僅保留最近 5 天,超過期限的訂單自動刪除。

**實作方式**:
- 每次應用程式啟動時執行 `CleanupOldOrdersAsync()`
- 或使用排程任務 (如 Hangfire) 每日凌晨執行清理

**清理邏輯**:
```csharp
// 刪除 CreatedAt < (今天 - 5 天) 的訂單
var cutoffDate = DateTime.Now.AddDays(-5);
orders = orders.Where(o => o.CreatedAt >= cutoffDate).ToList();
```

### 資料完整性

**店家刪除時**:
- 訂單中的店家資訊 (StoreName) 為快照資料,即使店家被刪除,歷史訂單仍可正常顯示
- 但新訂單不可選擇已刪除的店家

**菜品價格變動時**:
- 訂單中的菜品資訊 (MenuItemName, UnitPrice) 為快照資料,不受後續價格調整影響

---

## 測試場景

### 功能測試

1. **訂餐流程測試**:
   - 從首頁進入 → 選擇餐廳 → 加入菜品 → 結帳 → 確認訂單 → 顯示成功訊息
   
2. **數量驗證測試**:
   - 輸入 0: 自動修正為 1
   - 輸入負數: 顯示錯誤訊息
   - 輸入小數: 顯示錯誤訊息
   - 輸入文字: 顯示錯誤訊息

3. **訂單總金額計算測試**:
   - 單一菜品: 價格 × 數量
   - 多個菜品: 所有小計加總
   - 數量變更時即時更新

4. **必填欄位驗證測試**:
   - 姓名空白: 顯示錯誤
   - 電話空白: 顯示錯誤
   - 電話包含非數字: 顯示錯誤

5. **訂單紀錄測試**:
   - 顯示最近 5 天的訂單
   - 超過 5 天的訂單不顯示
   - 無訂單時顯示提示訊息

### 整合測試

1. **與店家管理功能整合**:
   - 確認可正確讀取 `stores.json` 中的餐廳資料
   - 確認可正確讀取菜單資料

2. **資料持久化測試**:
   - 訂單成功提交後,檢查 `orders.json` 是否正確寫入
   - 重新整理頁面後,訂單紀錄仍可正常顯示

3. **進行中訂單提示測試**:
   - 有進行中訂單時,餐廳列表頁面顯示提示
   - 點擊「查看訂單詳情」可正確跳轉

---

## 實作優先順序

### Phase 1 (P1 - 核心功能)
1. 建立資料模型 (Order, OrderItem, OrderStatus)
2. 實作 OrderService (建立訂單、查詢訂單)
3. 實作 OrderController (餐廳列表、菜單、結帳)
4. 建立基本 Views (Index, Menu, Checkout, Success)
5. 實作數量選擇器與訂單摘要 (JavaScript)
6. 實作必填欄位驗證

### Phase 2 (P2 - 進階功能)
7. 實作訂單紀錄功能 (History, Details)
8. 實作進行中訂單提示
9. 實作自動清理舊訂單 (5 天規則)
10. 更新首頁按鈕狀態

### Phase 3 (P3 - UI 優化)
11. 套用食物主題 UI 設計
12. 實作響應式佈局
13. 新增載入動畫與過渡效果
14. 改善錯誤訊息顯示方式

---

## 相依性

**前置需求**:
- 001-store-menu-management (店家與菜單管理系統) 必須完成
- `Data/stores.json` 必須存在且包含至少一筆餐廳資料

**技術相依**:
- ASP.NET Core 8.0 MVC
- System.Text.Json (JSON 序列化)
- Bootstrap 5.3
- Bootstrap Icons
- jQuery (用於前端互動)

---

## 未來擴充建議

以下功能不在本次範圍內,但可作為未來版本的參考:

1. **訂單狀態管理**: 店家可更新訂單狀態 (已確認、準備中、已完成)
2. **訂單通知**: Email 或 SMS 通知訂單狀態變更
3. **訂單取消**: 用戶可取消尚未確認的訂單
4. **訂單搜尋**: 根據訂單編號、日期、餐廳名稱搜尋
5. **訂單統計**: 顯示訂單總數、總金額、熱門餐廳等統計資訊
6. **收藏功能**: 用戶可收藏常點的餐廳或菜品
7. **評論功能**: 用戶可對餐廳或菜品留下評論

---

## 附錄: JSON 資料範例

### orders.json 結構

```json
[
  {
    "orderId": "ORD20251123143022",
    "storeId": 1,
    "storeName": "美味便當店",
    "customerName": "王小明",
    "customerPhone": "0912345678",
    "orderItems": [
      {
        "menuItemId": 1,
        "menuItemName": "排骨便當",
        "unitPrice": 80,
        "quantity": 2,
        "subtotal": 160
      },
      {
        "menuItemId": 3,
        "menuItemName": "雞腿便當",
        "unitPrice": 85,
        "quantity": 1,
        "subtotal": 85
      }
    ],
    "totalAmount": 245.00,
    "orderStatus": 0,
    "createdAt": "2025-11-23T14:30:22"
  }
]
```

---

**文件版本**: 1.0  
**最後更新**: 2025年11月23日  
**作者**: 系統設計團隊
