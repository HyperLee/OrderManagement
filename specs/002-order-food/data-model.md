# 訂餐功能系統 - 資料模型

**Feature Branch**: `002-order-food`  
**Created**: 2025年11月23日  
**Status**: Complete

## 概述

本文件定義訂餐功能系統的資料模型，包含實體、欄位、關聯性、驗證規則和狀態轉換。所有模型設計遵循 C# 13 最佳實踐和專案憲章要求。

## 核心實體

### 1. Order（訂單）

**用途**: 代表一筆完整的訂單，包含訂餐者資訊、訂單項目和訂單狀態。

**欄位定義**:

| 欄位名稱 | 型別 | 必填 | 說明 | 驗證規則 |
|---------|------|------|------|---------|
| `OrderId` | `string` | ✅ | 唯一訂單編號 | 格式：`ORD{yyyyMMddHHmmssfff}`，由系統自動產生 |
| `StoreId` | `string` | ✅ | 餐廳ID | 外鍵關聯至 Store.StoreId |
| `StoreName` | `string` | ✅ | 餐廳名稱（快照） | 最大長度 200，儲存訂單建立時的餐廳名稱 |
| `CustomerName` | `string` | ✅ | 訂餐者姓名 | 最大長度 100，不可空白 |
| `CustomerPhone` | `string` | ✅ | 訂餐者聯絡電話 | 最大長度 20，僅能包含數字 |
| `Items` | `List<OrderItem>` | ✅ | 訂單項目清單 | 至少包含 1 個項目 |
| `TotalAmount` | `decimal` | ✅ | 訂單總金額 | 計算欄位：所有 OrderItem.Subtotal 的加總，保留小數點 2 位 |
| `Status` | `OrderStatus` | ✅ | 訂單狀態 | 列舉值，預設為 `Pending` |
| `CreatedAt` | `DateTime` | ✅ | 訂單建立時間 | 系統自動設定為 `DateTime.Now` |

**關聯性**:
- **Order → Store** (Many-to-One): 一筆訂單屬於一間餐廳（使用 StoreId 關聯，但儲存 StoreName 快照避免餐廳刪除後影響歷史訂單）
- **Order → OrderItem** (One-to-Many): 一筆訂單包含多個訂單項目

**索引建議**:
- 主鍵：`OrderId`
- 索引欄位：`CreatedAt`（用於訂單紀錄查詢和舊訂單清理）
- 索引欄位：`Status`（用於查詢進行中訂單）

**C# 模型定義**:

```csharp
namespace OrderLunchWeb.Models;

/// <summary>
/// 訂單模型
/// </summary>
public class Order
{
    /// <summary>
    /// 唯一訂單編號（格式：ORD{yyyyMMddHHmmssfff}）
    /// </summary>
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// 餐廳ID
    /// </summary>
    public string StoreId { get; set; } = string.Empty;

    /// <summary>
    /// 餐廳名稱快照（儲存訂單建立時的餐廳名稱）
    /// </summary>
    public string StoreName { get; set; } = string.Empty;

    /// <summary>
    /// 訂餐者姓名
    /// </summary>
    [Required(ErrorMessage = "姓名為必填欄位")]
    [MaxLength(100)]
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// 訂餐者聯絡電話（僅數字）
    /// </summary>
    [Required(ErrorMessage = "聯絡電話為必填欄位")]
    [RegularExpression(@"^\d+$", ErrorMessage = "電話號碼僅能輸入數字")]
    [MaxLength(20, ErrorMessage = "電話號碼長度不可超過20位數")]
    public string CustomerPhone { get; set; } = string.Empty;

    /// <summary>
    /// 訂單項目清單
    /// </summary>
    public List<OrderItem> Items { get; set; } = new();

    /// <summary>
    /// 訂單總金額（計算欄位）
    /// </summary>
    public decimal TotalAmount => Items.Sum(i => i.Subtotal);

    /// <summary>
    /// 訂單狀態
    /// </summary>
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    /// <summary>
    /// 訂單建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
```

---

### 2. OrderItem（訂單項目）

**用途**: 代表訂單中的單一菜品項目，包含菜品資訊快照、數量和小計。

**欄位定義**:

| 欄位名稱 | 型別 | 必填 | 說明 | 驗證規則 |
|---------|------|------|------|---------|
| `MenuItemId` | `string` | ✅ | 菜品ID | 外鍵關聯至 MenuItem.MenuItemId |
| `MenuItemName` | `string` | ✅ | 菜品名稱（快照） | 最大長度 200，儲存訂單建立時的菜品名稱 |
| `Price` | `decimal` | ✅ | 單價（快照） | 大於 0，儲存訂單建立時的價格 |
| `Quantity` | `int` | ✅ | 訂購數量 | 最小值 1，最大值 100 |
| `Subtotal` | `decimal` | ✅ | 小計 | 計算欄位：`Math.Round(Price * Quantity, 2)` |

**關聯性**:
- **OrderItem → MenuItem** (Many-to-One): 一個訂單項目對應一個菜品（使用 MenuItemId 關聯，但儲存 MenuItemName 和 Price 快照避免菜品價格變動影響歷史訂單）

**C# 模型定義**:

```csharp
namespace OrderLunchWeb.Models;

/// <summary>
/// 訂單項目模型
/// </summary>
public class OrderItem
{
    /// <summary>
    /// 菜品ID
    /// </summary>
    public string MenuItemId { get; set; } = string.Empty;

    /// <summary>
    /// 菜品名稱快照（儲存訂單建立時的菜品名稱）
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string MenuItemName { get; set; } = string.Empty;

    /// <summary>
    /// 單價快照（儲存訂單建立時的價格）
    /// </summary>
    [Range(0.01, double.MaxValue, ErrorMessage = "單價必須大於 0")]
    public decimal Price { get; set; }

    /// <summary>
    /// 訂購數量
    /// </summary>
    [Range(1, 100, ErrorMessage = "數量必須介於 1 到 100 之間")]
    public int Quantity { get; set; }

    /// <summary>
    /// 小計（計算欄位：單價 × 數量）
    /// </summary>
    public decimal Subtotal => Math.Round(Price * Quantity, 2);
}
```

---

### 3. OrderStatus（訂單狀態）

**用途**: 定義訂單的狀態列舉值。

**列舉值定義**:

| 值 | 說明 | 使用時機 |
|----|------|---------|
| `Pending` | 待確認 | 訂單建立後的預設狀態，本版本所有訂單維持此狀態 |
| `Confirmed` | 已確認 | （未來版本）餐廳確認訂單後 |
| `Preparing` | 準備中 | （未來版本）餐廳開始準備餐點 |
| `Completed` | 已完成 | （未來版本）訂單完成並交付 |
| `Cancelled` | 已取消 | （未來版本）使用者或餐廳取消訂單 |

**狀態轉換圖**:

```text
本版本（v1）:
  [Pending] ──────────────────> (不變更狀態)

未來版本（v2+）:
  [Pending] ──> [Confirmed] ──> [Preparing] ──> [Completed]
      │                                               ^
      └───────────> [Cancelled] ─────────────────────┘
```

**C# 模型定義**:

```csharp
namespace OrderLunchWeb.Models;

/// <summary>
/// 訂單狀態列舉
/// </summary>
public enum OrderStatus
{
    /// <summary>
    /// 待確認
    /// </summary>
    Pending = 0,

    /// <summary>
    /// 已確認（未來版本）
    /// </summary>
    Confirmed = 1,

    /// <summary>
    /// 準備中（未來版本）
    /// </summary>
    Preparing = 2,

    /// <summary>
    /// 已完成（未來版本）
    /// </summary>
    Completed = 3,

    /// <summary>
    /// 已取消（未來版本）
    /// </summary>
    Cancelled = 4
}
```

---

## 現有實體（參考）

以下實體已在專案中定義，訂單功能將與之整合：

### 4. Store（餐廳）

**欄位定義**（現有）:

| 欄位名稱 | 型別 | 說明 |
|---------|------|------|
| `StoreId` | `string` | 唯一餐廳編號 |
| `Name` | `string` | 餐廳名稱 |
| `Address` | `string` | 餐廳地址 |
| `Phone` | `string` | 聯絡電話 |
| `OpeningHours` | `string` | 營業時間 |
| `MenuItems` | `List<MenuItem>` | 菜單項目清單 |

**與訂單的關聯**:
- Order.StoreId → Store.StoreId（外鍵關聯）
- Order.StoreName 儲存 Store.Name 的快照值

---

### 5. MenuItem（菜品）

**欄位定義**（現有）:

| 欄位名稱 | 型別 | 說明 |
|---------|------|------|
| `MenuItemId` | `string` | 唯一菜品編號 |
| `Name` | `string` | 菜品名稱 |
| `Price` | `decimal` | 菜品價格 |

**與訂單的關聯**:
- OrderItem.MenuItemId → MenuItem.MenuItemId（外鍵關聯）
- OrderItem.MenuItemName 和 OrderItem.Price 儲存 MenuItem 的快照值

---

## ViewModel（視圖模型）

以下 ViewModel 用於在 Controller 和 View 之間傳遞資料：

### 6. CheckoutViewModel（結帳視圖模型）

**用途**: 在結帳頁面收集訂餐者資訊和訂單明細。

```csharp
namespace OrderLunchWeb.Models;

/// <summary>
/// 結帳視圖模型
/// </summary>
public class CheckoutViewModel
{
    /// <summary>
    /// 訂餐者姓名
    /// </summary>
    [Required(ErrorMessage = "姓名為必填欄位")]
    [MaxLength(100)]
    [Display(Name = "姓名")]
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// 訂餐者聯絡電話
    /// </summary>
    [Required(ErrorMessage = "聯絡電話為必填欄位")]
    [RegularExpression(@"^\d+$", ErrorMessage = "電話號碼僅能輸入數字")]
    [MaxLength(20, ErrorMessage = "電話號碼長度不可超過20位數")]
    [Display(Name = "聯絡電話")]
    public string CustomerPhone { get; set; } = string.Empty;

    /// <summary>
    /// 訂單項目清單（從 Session Storage 載入）
    /// </summary>
    public List<OrderItem> Items { get; set; } = new();

    /// <summary>
    /// 餐廳ID
    /// </summary>
    public string StoreId { get; set; } = string.Empty;

    /// <summary>
    /// 餐廳名稱
    /// </summary>
    public string StoreName { get; set; } = string.Empty;

    /// <summary>
    /// 訂單總金額
    /// </summary>
    public decimal TotalAmount => Items.Sum(i => i.Subtotal);
}
```

---

### 7. OrderHistoryViewModel（訂單紀錄視圖模型）

**用途**: 在訂單紀錄頁面顯示訂單清單。

```csharp
namespace OrderLunchWeb.Models;

/// <summary>
/// 訂單紀錄視圖模型
/// </summary>
public class OrderHistoryViewModel
{
    /// <summary>
    /// 訂單清單（最近 5 天）
    /// </summary>
    public List<OrderSummary> Orders { get; set; } = new();

    /// <summary>
    /// 訂單摘要（用於列表顯示）
    /// </summary>
    public class OrderSummary
    {
        public string OrderId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string StoreName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
    }
}
```

---

### 8. CartItem（購物車項目）

**用途**: 在瀏覽器 Session Storage 中儲存購物車狀態的 JavaScript 物件結構。

**JavaScript 物件結構**:

```javascript
// Session Storage 中的購物車資料結構
const cart = {
    storeId: "STR001",           // 餐廳ID
    storeName: "美味餐廳",         // 餐廳名稱
    items: [                      // 訂單項目陣列
        {
            menuItemId: "MENU001",
            name: "炸雞套餐",
            price: 150.00,
            quantity: 2
        },
        {
            menuItemId: "MENU002",
            name: "珍珠奶茶",
            price: 65.00,
            quantity: 1
        }
    ]
};
```

**對應的 C# DTO（資料傳輸物件）**:

```csharp
namespace OrderLunchWeb.Models;

/// <summary>
/// 購物車資料傳輸物件（用於接收前端 Session Storage 資料）
/// </summary>
public class CartDto
{
    public string StoreId { get; set; } = string.Empty;
    public string StoreName { get; set; } = string.Empty;
    public List<CartItemDto> Items { get; set; } = new();
}

/// <summary>
/// 購物車項目資料傳輸物件
/// </summary>
public class CartItemDto
{
    public string MenuItemId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    
    [Range(1, 100)]
    public int Quantity { get; set; }
}
```

---

## 資料驗證規則摘要

### Order 驗證規則

| 欄位 | 驗證規則 | 錯誤訊息 |
|------|---------|---------|
| `CustomerName` | Required, MaxLength(100) | 「姓名為必填欄位」 |
| `CustomerPhone` | Required, RegularExpression(@"^\d+$"), MaxLength(20) | 「聯絡電話為必填欄位」、「電話號碼僅能輸入數字」、「電話號碼長度不可超過20位數」 |
| `Items` | 至少 1 個項目 | 「訂單必須至少包含一個菜品」 |

### OrderItem 驗證規則

| 欄位 | 驗證規則 | 錯誤訊息 |
|------|---------|---------|
| `MenuItemName` | Required, MaxLength(200) | 「菜品名稱為必填欄位」 |
| `Price` | Range(0.01, double.MaxValue) | 「單價必須大於 0」 |
| `Quantity` | Range(1, 100) | 「數量必須介於 1 到 100 之間」 |

---

## 資料儲存格式

### orders.json 結構範例

```json
{
  "orders": [
    {
      "orderId": "ORD20251123143025123",
      "storeId": "STR001",
      "storeName": "美味餐廳",
      "customerName": "張三",
      "customerPhone": "0912345678",
      "items": [
        {
          "menuItemId": "MENU001",
          "menuItemName": "炸雞套餐",
          "price": 150.00,
          "quantity": 2
        },
        {
          "menuItemId": "MENU002",
          "menuItemName": "珍珠奶茶",
          "price": 65.00,
          "quantity": 1
        }
      ],
      "status": 0,
      "createdAt": "2025-11-23T14:30:25.123Z"
    }
  ]
}
```

**編碼注意事項**:
- 使用 UTF-8 編碼儲存，確保中文正確顯示
- 使用 `System.Text.Json` 序列化時設定 `Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping`
- `WriteIndented = true` 以便人工閱讀和版本控制

---

## 實體關聯圖（ERD）

```text
┌─────────────────┐         ┌──────────────────┐
│     Store       │         │      Order       │
├─────────────────┤         ├──────────────────┤
│ StoreId (PK)    │◄────────│ StoreId (FK)     │
│ Name            │         │ StoreName (快照)  │
│ Address         │         │ OrderId (PK)     │
│ Phone           │         │ CustomerName     │
│ OpeningHours    │         │ CustomerPhone    │
│ MenuItems       │         │ Items            │
└─────────────────┘         │ TotalAmount      │
                            │ Status           │
                            │ CreatedAt        │
                            └──────────────────┘
                                     │
                                     │ 1
                                     │
                                     │ *
                            ┌──────────────────┐
                            │   OrderItem      │
                            ├──────────────────┤
                            │ MenuItemId (FK)  │
                            │ MenuItemName(快照)│
                            │ Price (快照)      │
                            │ Quantity         │
                            │ Subtotal         │
                            └──────────────────┘
                                     │
                                     │ *
                                     │
                                     │ 1
┌─────────────────┐                 │
│   MenuItem      │◄────────────────┘
├─────────────────┤
│ MenuItemId (PK) │
│ Name            │
│ Price           │
└─────────────────┘
```

**關聯說明**:
- **Store ↔ Order**: 一對多（一間餐廳可以有多筆訂單）
- **Order ↔ OrderItem**: 一對多（一筆訂單包含多個訂單項目）
- **MenuItem ↔ OrderItem**: 一對多（一個菜品可以出現在多個訂單項目中）

**快照機制**:
- Order 儲存 StoreName 快照（避免餐廳改名或刪除影響歷史訂單）
- OrderItem 儲存 MenuItemName 和 Price 快照（避免菜品改名或調價影響歷史訂單）

---

## 資料完整性與約束

### 主鍵約束

- `Order.OrderId`: 唯一，由系統自動產生（格式：`ORD{yyyyMMddHHmmssfff}`）
- `Store.StoreId`: 唯一（現有資料）
- `MenuItem.MenuItemId`: 唯一（現有資料）

### 外鍵約束（邏輯層面）

由於使用 JSON 檔案儲存而非資料庫，外鍵約束在應用程式邏輯層面實施：

- 建立訂單時驗證 `StoreId` 存在於 `stores.json`
- 加入訂單項目時驗證 `MenuItemId` 存在於對應餐廳的菜單中
- 快照機制確保即使參考的餐廳或菜品被刪除，歷史訂單仍可正常顯示

### 業務規則約束

- 訂單必須至少包含 1 個訂單項目
- 訂單項目數量必須介於 1 到 100 之間
- 訂單總金額必須等於所有訂單項目小計的加總
- 訂單建立後狀態預設為 `Pending`（本版本不允許變更狀態）
- 訂單建立時間自動設定為當前時間
- 超過 5 天的訂單在應用程式啟動時自動清理

---

## 未來擴展考量

### v2.0 可能的資料模型擴展

1. **User（使用者）實體**
   - UserId, Username, Email, PasswordHash, Role
   - Order.UserId 外鍵（關聯訂單與使用者）

2. **OrderStatusHistory（訂單狀態歷史）實體**
   - OrderId, Status, ChangedAt, ChangedBy, Reason
   - 記錄訂單狀態變更軌跡

3. **Payment（支付）實體**
   - PaymentId, OrderId, Amount, Method, Status, TransactionId, PaidAt
   - 記錄支付資訊和交易記錄

4. **Review（評論）實體**
   - ReviewId, OrderId, UserId, StoreId, Rating, Comment, CreatedAt
   - 訂單完成後允許使用者評論

5. **Notification（通知）實體**
   - NotificationId, UserId, OrderId, Type, Message, IsSent, SentAt
   - 訂單狀態變更時發送通知（Email, SMS, Push）

### 資料庫遷移考量

若未來遷移至資料庫（Entity Framework Core），建議：

1. **訂單編號策略**: 改用資料庫自動遞增 ID + 顯示用訂單編號
2. **索引優化**: 在 CreatedAt, Status, UserId 等欄位建立索引
3. **關聯定義**: 使用 Navigation Properties 和 Foreign Key Constraints
4. **正規化**: 考慮將 Store 和 MenuItem 快照資料獨立成 Snapshot 表
5. **遷移計畫**: 撰寫 JSON → SQL 的資料遷移腳本

---

## 總結

本資料模型設計遵循以下原則：

1. **簡單性**: 使用 JSON 檔案儲存，避免資料庫複雜度
2. **完整性**: 欄位定義完整，驗證規則清晰
3. **可維護性**: 使用快照機制確保歷史訂單不受參考資料變動影響
4. **可擴展性**: 預留未來擴展空間（訂單狀態、使用者關聯、支付整合）
5. **一致性**: 遵循現有專案的模型設計風格（Store, MenuItem）

所有模型均包含 XML 文件註解，符合專案憲章的程式碼品質要求。
