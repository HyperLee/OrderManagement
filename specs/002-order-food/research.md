# 訂餐功能系統 - 研究文件

**Feature Branch**: `002-order-food`  
**Created**: 2025年11月23日  
**Status**: Complete

## 概述

本文件記錄訂餐功能系統的技術研究和決策理由。由於本專案為練習專案，所有技術決策都已在規格和計畫階段明確定義，本文件主要說明關鍵技術選擇的最佳實踐和實作考量。

## 技術決策

### 1. JSON 檔案儲存 vs 資料庫

**Decision**: 使用 JSON 檔案儲存訂單資料

**Rationale**:
- **簡化開發**: 無需設定 Entity Framework Core、資料庫連線字串和 Migration
- **版本控制友善**: JSON 檔案可直接納入版本控制，易於檢視資料變更
- **練習導向**: 聚焦於業務邏輯和 MVC 架構，避免資料存取層的額外複雜度
- **現有模式**: 專案已使用 JSON 檔案儲存餐廳資料（stores.json），保持一致性

**Alternatives Considered**:
- **Entity Framework Core + SQLite**: 更接近生產環境，但需要額外的 Migration 管理和資料庫設定
- **In-Memory Database**: 適合測試，但資料不持久化，不符合需求
- **SQL Server**: 過於重量級，不適合小型練習專案

**Implementation Considerations**:
- 使用 UTF-8 編碼避免中文亂碼（`Encoding.UTF8`）
- 實作檔案讀寫的執行緒安全機制（使用 `SemaphoreSlim` 或檔案鎖）
- 提供清晰的錯誤處理和日誌記錄（檔案不存在、權限錯誤、JSON 格式錯誤）
- 訂單檔案路徑：`Data/orders.json`

**Code Pattern**:
```csharp
// 使用現有的 IFileStorage 和 JsonFileStorage
// 確保 JSON 序列化設定支援中文
var options = new JsonSerializerOptions
{
    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    WriteIndented = true
};
```

---

### 2. Session Storage vs Server-Side Session 管理購物車

**Decision**: 使用瀏覽器 Session Storage 管理購物車狀態

**Rationale**:
- **減輕伺服器負擔**: 購物車資料儲存在客戶端，伺服器無需維護 Session 狀態
- **簡化架構**: 無需設定 ASP.NET Core Session 中介軟體和分散式快取
- **練習前端技能**: 學習使用 JavaScript 管理客戶端狀態
- **分頁關閉自動清理**: Session Storage 在瀏覽器分頁關閉時自動清空，符合需求

**Alternatives Considered**:
- **ASP.NET Core Session**: 需要設定 Session 中介軟體、記憶體快取或分散式快取，增加複雜度
- **Local Storage**: 資料持久化，不符合「關閉分頁即清空」的需求
- **Cookie**: 有大小限制（4KB），不適合儲存多個訂單項目

**Implementation Considerations**:
- 購物車資料結構：`{ storeId: string, storeName: string, items: [{menuItemId, name, price, quantity}] }`
- 使用 JavaScript 函式封裝 Session Storage 操作（`saveCart()`, `loadCart()`, `clearCart()`）
- 在每次加入菜品或修改數量時同步更新 Session Storage 和 UI
- 結帳時將 Session Storage 資料傳送至伺服器建立訂單

**Code Pattern**:
```javascript
// order.js
const CartStorage = {
    key: 'orderCart',
    
    save(cart) {
        sessionStorage.setItem(this.key, JSON.stringify(cart));
    },
    
    load() {
        const data = sessionStorage.getItem(this.key);
        return data ? JSON.parse(data) : null;
    },
    
    clear() {
        sessionStorage.removeItem(this.key);
    }
};
```

---

### 3. 訂單編號產生策略

**Decision**: 使用時間戳記格式 `ORD{yyyyMMddHHmmss}`

**Rationale**:
- **唯一性**: 時間戳記精確到秒，在小型練習專案中足以保證唯一性
- **可讀性**: 訂單編號包含建立日期時間，易於人工識別和排序
- **簡單性**: 無需資料庫自動遞增 ID 或 GUID 產生器

**Alternatives Considered**:
- **GUID**: 保證唯一性但不易讀，訂單編號過長（如 `ORD-3f2504e0-4f89-11d3-9a0c-0305e82c3301`）
- **資料庫自動遞增 ID**: 需要資料庫支援，不適合 JSON 檔案儲存
- **雪花演算法**: 過於複雜，不適合小型專案

**Implementation Considerations**:
- 格式範例：`ORD20251123143025`（2025年11月23日 14:30:25）
- 在極少數情況下（同一秒內建立多筆訂單），可能產生重複，解決方案：
  1. 加入毫秒：`ORD{yyyyMMddHHmmssfff}`
  2. 加入隨機後綴：`ORD{yyyyMMddHHmmss}{3位隨機數字}`
- 建議使用方案 1（加入毫秒）以保證唯一性

**Code Pattern**:
```csharp
public string GenerateOrderId()
{
    return $"ORD{DateTime.Now:yyyyMMddHHmmssfff}";
}
```

---

### 4. 訂單資料自動清理機制

**Decision**: 應用程式啟動時執行清理（刪除超過 5 天的訂單）

**Rationale**:
- **簡單可靠**: 每次應用程式啟動時自動執行，無需額外的排程器或背景服務
- **練習導向**: 避免引入 Quartz.NET 或 Hangfire 等排程套件
- **足夠頻繁**: 開發階段應用程式重啟頻繁，生產環境可改用背景服務

**Alternatives Considered**:
- **背景服務 (IHostedService)**: 更專業，但增加複雜度，需要管理服務生命週期和錯誤處理
- **手動清理**: 不可靠，容易忘記執行
- **定時排程 (Quartz.NET)**: 過於重量級，不適合練習專案

**Implementation Considerations**:
- 在 `Program.cs` 的 `app.Run()` 之前執行清理邏輯
- 計算日期差異時使用 `DateTime.Now.AddDays(-5)`
- 清理失敗不應影響應用程式啟動（使用 try-catch 並記錄錯誤）
- 記錄清理結果（刪除的訂單數量）

**Code Pattern**:
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

### 5. 訂單總金額計算與精確度

**Decision**: 使用 `decimal` 型別儲存金額，計算小計時四捨五入到小數點第 2 位

**Rationale**:
- **精確性**: `decimal` 適合金融計算，避免浮點數誤差
- **C# 最佳實踐**: 金額計算應使用 `decimal` 而非 `double` 或 `float`
- **顯示格式**: 使用 `{0:C}` 或 `{0:N2}` 格式化為貨幣或小數點 2 位

**Alternatives Considered**:
- **`double` 或 `float`**: 有浮點數精確度問題，不適合金額計算
- **整數 (分為單位)**: 避免小數點問題，但顯示時需要除以 100，增加複雜度

**Implementation Considerations**:
- 單價和總金額使用 `decimal` 型別
- 小計計算：`decimal subtotal = Math.Round(price * quantity, 2)`
- 訂單總金額：`decimal total = orderItems.Sum(i => i.Subtotal)`
- 顯示格式：`NT$ {total:N2}` 或使用 `string.Format("{0:C}", total)`

**Code Pattern**:
```csharp
public class OrderItem
{
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    
    public decimal Subtotal => Math.Round(Price * Quantity, 2);
}

public class Order
{
    public List<OrderItem> Items { get; set; } = new();
    
    public decimal TotalAmount => Items.Sum(i => i.Subtotal);
}
```

---

### 6. 前端驗證 vs 後端驗證

**Decision**: 前端和後端都實作驗證（雙重驗證）

**Rationale**:
- **使用者體驗**: 前端驗證提供即時回饋，避免不必要的伺服器請求
- **安全性**: 後端驗證防止繞過前端驗證的惡意請求
- **最佳實踐**: 永遠不信任客戶端輸入

**Implementation Considerations**:
- **前端驗證** (JavaScript):
  - 姓名和電話必填檢查
  - 電話號碼格式驗證（正則表達式：`/^\d+$/`）
  - 數量正整數驗證（最小值 1）
  - 即時顯示錯誤訊息
  
- **後端驗證** (C# Data Annotations + FluentValidation):
  - 使用 `[Required]`, `[RegularExpression]`, `[Range]` 等屬性
  - 在 Controller Action 中檢查 `ModelState.IsValid`
  - 返回清晰的驗證錯誤訊息

**Code Pattern**:
```csharp
// 後端驗證
public class CheckoutViewModel
{
    [Required(ErrorMessage = "姓名為必填欄位")]
    public string CustomerName { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "聯絡電話為必填欄位")]
    [RegularExpression(@"^\d+$", ErrorMessage = "電話號碼僅能輸入數字")]
    [MaxLength(20, ErrorMessage = "電話號碼長度不可超過20位數")]
    public string CustomerPhone { get; set; } = string.Empty;
}

// 前端驗證 (JavaScript)
function validateCheckoutForm() {
    const name = document.getElementById('customerName').value.trim();
    const phone = document.getElementById('customerPhone').value.trim();
    
    if (!name) {
        showError('nameError', '姓名為必填欄位');
        return false;
    }
    
    if (!phone) {
        showError('phoneError', '聯絡電話為必填欄位');
        return false;
    }
    
    if (!/^\d+$/.test(phone)) {
        showError('phoneError', '電話號碼僅能輸入數字');
        return false;
    }
    
    return true;
}
```

---

### 7. Razor Views vs Razor Pages

**Decision**: 使用 Razor Views + MVC Controllers

**Rationale**:
- **現有架構**: 專案已使用 MVC 模式（HomeController, StoreController）
- **一致性**: 保持與現有程式碼的一致性，降低學習曲線
- **關注點分離**: Controller 處理邏輯，View 處理顯示

**Alternatives Considered**:
- **Razor Pages**: 更適合頁面導向的應用程式，但需要改變現有架構
- **Blazor**: 過於現代且複雜，不適合練習專案

**Implementation Considerations**:
- 訂單相關的所有頁面放置於 `Views/Order/` 資料夾
- 使用 ViewModel 在 Controller 和 View 之間傳遞資料
- 共用版面配置和部分視圖放置於 `Views/Shared/`

---

### 8. 結帳逾時處理

**Decision**: 僅顯示提示訊息，不強制清空購物車或導向

**Rationale**:
- **使用者友善**: 使用者可能因故離開後返回，強制清空會造成不良體驗
- **簡化實作**: 無需實作複雜的倒數計時器或自動重定向邏輯
- **練習導向**: 聚焦於核心功能，避免過度工程

**Implementation Considerations**:
- 在結帳頁面加載時記錄時間戳記
- 使用 JavaScript 計算停留時間
- 超過 30 分鐘時在頁面頂部顯示提示橫幅：「訂單已逾時，請重新選擇菜品」
- 不阻止使用者繼續結帳（資料仍在 Session Storage）

**Code Pattern**:
```javascript
// 結帳頁面逾時檢查
const checkoutStartTime = Date.now();
const timeoutMinutes = 30;

setInterval(() => {
    const elapsedMinutes = (Date.now() - checkoutStartTime) / 1000 / 60;
    if (elapsedMinutes > timeoutMinutes) {
        document.getElementById('timeoutBanner').style.display = 'block';
    }
}, 60000); // 每分鐘檢查一次
```

---

## 最佳實踐總結

### ASP.NET Core MVC
- 使用相依性注入管理服務生命週期（Singleton, Scoped, Transient）
- 遵循 RESTful 路由慣例（`/Order/SelectRestaurant`, `/Order/Menu/{storeId}`）
- 使用 ViewModel 分離業務模型和視圖模型
- 實作全域例外處理中介軟體

### C# 程式設計
- 使用檔案範圍命名空間（`namespace OrderLunchWeb;`）
- 使用 `is null` / `is not null` 而非 `== null`
- 公開 API 包含 XML 文件註解
- 使用 `async/await` 處理 I/O 操作（檔案讀寫）

### 前端開發
- 使用 Bootstrap 5 元件確保 UI 一致性
- JavaScript 程式碼模組化（使用物件或模組模式）
- 表單提交前進行前端驗證
- 使用 Session Storage API 管理暫存資料

### 測試策略
- 單元測試覆蓋核心業務邏輯（OrderService, 金額計算）
- 整合測試覆蓋 Controller 端點和完整流程
- 使用 Mock 隔離外部相依性（IFileStorage）
- 測試邊界情況（空訂單、無效輸入、檔案錯誤）

### 錯誤處理與日誌
- 使用 Serilog 結構化日誌
- 記錄關鍵業務事件（訂單建立、驗證失敗、檔案錯誤）
- 提供使用者友善的錯誤訊息
- 敏感資訊不記錄至日誌（電話號碼可記錄前 3 碼或遮罩）

---

## 未來擴展考量

雖然本版本不實作以下功能，但設計時應考慮未來擴展的可能性：

1. **使用者認證系統**
   - 預留 `UserId` 欄位於 Order 模型
   - 設計時考慮多租戶場景（不同使用者查看各自的訂單）

2. **訂單狀態流轉**
   - OrderStatus 列舉設計為可擴展（Pending, Confirmed, Preparing, Completed, Cancelled）
   - 預留狀態變更歷史記錄欄位

3. **資料庫遷移**
   - Order 和 OrderItem 模型設計應符合資料庫正規化
   - 考慮未來加入 Entity Framework Core 的相容性

4. **通知機制**
   - 預留通知發送的擴展點（訂單確認後發送 Email 或 SMS）

5. **支付整合**
   - 預留支付狀態和交易記錄欄位

---

## 參考資源

- [ASP.NET Core MVC 官方文件](https://learn.microsoft.com/zh-tw/aspnet/core/mvc/overview)
- [Serilog 最佳實踐](https://github.com/serilog/serilog/wiki/Configuration-Basics)
- [JSON 序列化 (System.Text.Json)](https://learn.microsoft.com/zh-tw/dotnet/standard/serialization/system-text-json-overview)
- [Session Storage API](https://developer.mozilla.org/zh-TW/docs/Web/API/Window/sessionStorage)
- [Bootstrap 5 文件](https://getbootstrap.com/docs/5.3/getting-started/introduction/)

---

**結論**: 本研究文件確認所有技術決策均已明確定義，無 NEEDS CLARIFICATION 項目。所有選擇都以簡化開發、保持一致性和練習導向為原則，符合專案憲章的核心原則。
