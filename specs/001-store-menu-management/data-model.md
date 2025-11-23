# 資料模型設計: 店家與菜單管理系統

**Feature**: 001-store-menu-management  
**Date**: 2025-11-22  
**Version**: 1.0

## 概述

本文件定義店家與菜單管理系統的資料模型，包括實體定義、關係、驗證規則和狀態轉換。資料儲存於 JSON 檔案 (`Data/stores.json`)，使用 UTF-8 編碼。

## 實體定義

### 1. Store (店家)

代表一個餐廳或店家的完整資訊。

**屬性**:

| 屬性名稱 | 型別 | 必填 | 長度限制 | 說明 | 預設值 |
|---------|------|------|---------|------|--------|
| `Id` | `int` | ✅ | - | 店家唯一識別碼，自動遞增 | 系統生成 |
| `Name` | `string` | ✅ | 1-100 字元 | 店家名稱 | - |
| `Address` | `string` | ✅ | 1-200 字元 | 店家地址 | - |
| `PhoneType` | `PhoneType` (enum) | ✅ | - | 電話類型：市話或行動電話 | - |
| `Phone` | `string` | ✅ | 純數字 | 聯絡電話號碼 | - |
| `BusinessHours` | `string` | ✅ | 1-100 字元 | 營業時間 (自由文字) | - |
| `MenuItems` | `List<MenuItem>` | ✅ | 1-20 項 | 菜單項目清單 | 空清單 |
| `CreatedAt` | `DateTime` | ✅ | - | 建立時間 | 系統生成 |
| `UpdatedAt` | `DateTime` | ✅ | - | 最後修改時間 | 系統生成 |

**驗證規則**:

- `Name`: 不可空白，長度 1-100 字元
- `Address`: 不可空白，長度 1-200 字元
- `Phone`: 僅允許數字字元 (0-9)，不允許空格、破折號、括號
- `PhoneType`: 必須為有效的 `PhoneType` 列舉值
- `BusinessHours`: 不可空白，長度 1-100 字元，建議格式 "週一至週五 11:00-14:00, 17:00-21:00"
- `MenuItems`: 至少 1 項，最多 20 項
- **唯一性約束**: `Name` + `Phone` + `Address` 組合必須唯一 (不分大小寫)

**C# 類別定義**:

```csharp
namespace OrderLunchWeb.Models
{
    /// <summary>
    /// 代表一個餐廳店家
    /// </summary>
    public class Store
    {
        /// <summary>
        /// 店家唯一識別碼
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 店家名稱 (最多 100 字元)
        /// </summary>
        [Required(ErrorMessage = "店家名稱為必填欄位")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "店家名稱長度必須介於 1 到 100 字元")]
        [Display(Name = "店家名稱")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 店家地址 (最多 200 字元)
        /// </summary>
        [Required(ErrorMessage = "店家地址為必填欄位")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "店家地址長度必須介於 1 到 200 字元")]
        [Display(Name = "店家地址")]
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// 電話類型 (市話或行動電話)
        /// </summary>
        [Required(ErrorMessage = "請選擇電話類型")]
        [Display(Name = "電話類型")]
        public PhoneType PhoneType { get; set; }

        /// <summary>
        /// 聯絡電話號碼 (僅數字)
        /// </summary>
        [Required(ErrorMessage = "聯絡電話為必填欄位")]
        [RegularExpression(@"^\d+$", ErrorMessage = "電話號碼僅能輸入數字")]
        [Display(Name = "聯絡電話")]
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        /// 營業時間 (自由文字，最多 100 字元)
        /// </summary>
        [Required(ErrorMessage = "營業時間為必填欄位")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "營業時間長度必須介於 1 到 100 字元")]
        [Display(Name = "營業時間")]
        public string BusinessHours { get; set; } = string.Empty;

        /// <summary>
        /// 菜單項目清單 (1-20 項)
        /// </summary>
        [Required]
        [MinLength(1, ErrorMessage = "至少需要新增一個菜單項目")]
        [MaxLength(20, ErrorMessage = "菜單項目已達上限 20 筆")]
        public List<MenuItem> MenuItems { get; set; } = new();

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 最後修改時間
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}
```

---

### 2. MenuItem (菜單項目)

代表店家提供的單一餐點或商品。

**屬性**:

| 屬性名稱 | 型別 | 必填 | 長度限制 | 說明 | 預設值 |
|---------|------|------|---------|------|--------|
| `Id` | `int` | ✅ | - | 菜單項目唯一識別碼，自動遞增 | 系統生成 |
| `Name` | `string` | ✅ | 1-50 字元 | 菜名 | - |
| `Price` | `int` | ✅ | ≥ 0 | 價格 (正整數或零) | - |
| `Description` | `string` | ❌ | 0-200 字元 | 菜品描述 (選填) | 空字串 |

**驗證規則**:

- `Name`: 不可空白，長度 1-50 字元
- `Price`: 必須為正整數或零 (≥ 0)，不允許負數或小數
- `Description`: 選填，長度最多 200 字元

**C# 類別定義**:

```csharp
namespace OrderLunchWeb.Models
{
    /// <summary>
    /// 代表店家菜單中的單一餐點項目
    /// </summary>
    public class MenuItem
    {
        /// <summary>
        /// 菜單項目唯一識別碼
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 菜名 (最多 50 字元)
        /// </summary>
        [Required(ErrorMessage = "菜名為必填欄位")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "菜名長度必須介於 1 到 50 字元")]
        [Display(Name = "菜名")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 價格 (正整數或零)
        /// </summary>
        [Required(ErrorMessage = "價格為必填欄位")]
        [Range(0, int.MaxValue, ErrorMessage = "價格必須為正整數或零")]
        [Display(Name = "價格")]
        public int Price { get; set; }

        /// <summary>
        /// 菜品描述 (選填，最多 200 字元)
        /// </summary>
        [StringLength(200, ErrorMessage = "描述最多 200 字元")]
        [Display(Name = "描述")]
        public string Description { get; set; } = string.Empty;
    }
}
```

---

### 3. PhoneType (電話類型列舉)

定義聯絡電話的類型。

**列舉值**:

| 值 | 說明 |
|----|------|
| `Landline` | 市話 |
| `Mobile` | 行動電話 |

**C# 列舉定義**:

```csharp
namespace OrderLunchWeb.Models
{
    /// <summary>
    /// 聯絡電話類型
    /// </summary>
    public enum PhoneType
    {
        /// <summary>
        /// 市話
        /// </summary>
        [Display(Name = "市話")]
        Landline = 1,

        /// <summary>
        /// 行動電話
        /// </summary>
        [Display(Name = "行動電話")]
        Mobile = 2
    }
}
```

---

## 實體關係

### Store ↔ MenuItem (一對多)

- **關係**: 一個 `Store` 擁有 1 到 20 個 `MenuItem`
- **擁有權**: `Store` 是聚合根 (Aggregate Root)，刪除 `Store` 時連帶刪除所有 `MenuItem`
- **外鍵**: 隱式關係，`MenuItem` 透過 `Store.MenuItems` 集合關聯
- **級聯刪除**: 是

**關係圖**:

```text
┌─────────────────┐       1       ┌──────────────────┐
│     Store       │───────────────│    MenuItem      │
│                 │              *│                  │
│ - Id (PK)       │               │ - Id (PK)        │
│ - Name          │               │ - Name           │
│ - Address       │               │ - Price          │
│ - Phone         │               │ - Description    │
│ - PhoneType     │               └──────────────────┘
│ - BusinessHours │
│ - MenuItems[]   │
│ - CreatedAt     │
│ - UpdatedAt     │
└─────────────────┘
```

---

## 資料儲存結構 (JSON)

資料儲存在 `Data/stores.json` 檔案中，使用 UTF-8 編碼。

**JSON 結構範例**:

```json
{
  "stores": [
    {
      "id": 1,
      "name": "好吃便當店",
      "address": "台北市中正區羅斯福路一段 100 號",
      "phoneType": 2,
      "phone": "0912345678",
      "businessHours": "週一至週五 11:00-14:00, 17:00-20:00",
      "menuItems": [
        {
          "id": 1,
          "name": "排骨便當",
          "price": 80,
          "description": "香酥排骨配上三菜一飯"
        },
        {
          "id": 2,
          "name": "雞腿便當",
          "price": 90,
          "description": "大雞腿配上三菜一飯"
        }
      ],
      "createdAt": "2025-11-22T10:30:00",
      "updatedAt": "2025-11-22T10:30:00"
    },
    {
      "id": 2,
      "name": "麵食天堂",
      "address": "台北市大安區忠孝東路四段 200 號",
      "phoneType": 1,
      "phone": "0227001234",
      "businessHours": "每日 10:00-21:00",
      "menuItems": [
        {
          "id": 3,
          "name": "牛肉麵",
          "price": 120,
          "description": "大塊牛肉配上 Q 彈麵條"
        },
        {
          "id": 4,
          "name": "炸醬麵",
          "price": 80,
          "description": ""
        }
      ],
      "createdAt": "2025-11-22T11:00:00",
      "updatedAt": "2025-11-22T11:15:00"
    }
  ],
  "nextStoreId": 3,
  "nextMenuItemId": 5
}
```

**說明**:

- `stores`: 店家陣列
- `nextStoreId`: 下一個可用的店家 ID (用於自動遞增)
- `nextMenuItemId`: 下一個可用的菜單項目 ID (用於自動遞增)
- 使用 camelCase 命名規範 (與 C# 的 PascalCase 透過 `JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase` 轉換)

---

## 狀態轉換

### Store 生命週期

```text
┌──────────┐   Create    ┌────────┐   Update    ┌─────────┐
│  不存在   │───────────→│ 存在中  │←───────────│  存在中  │
└──────────┘             └────────┘             └─────────┘
                              │
                              │ Delete
                              ↓
                         ┌──────────┐
                         │  已刪除   │
                         └──────────┘
```

**狀態說明**:

1. **不存在**: 店家尚未建立
2. **存在中**: 店家已建立，可進行查詢、更新操作
3. **已刪除**: 店家從系統中移除，ID 不再重複使用

**狀態轉換規則**:

- **Create**: 新增店家，自動設定 `Id`、`CreatedAt`、`UpdatedAt`，為所有 `MenuItems` 設定 `Id`
- **Update**: 更新店家，重新設定 `UpdatedAt`
- **Delete**: 刪除店家及其所有 `MenuItems`，ID 不可重複使用

---

## ID 管理策略

### 自動遞增機制

- **店家 ID**: 從 1 開始自動遞增，存儲在 `nextStoreId`
- **菜單項目 ID**: 從 1 開始自動遞增，存儲在 `nextMenuItemId`
- **唯一性**: 系統保證 ID 在各自的類型中唯一
- **不重用**: 已刪除的 ID 不會被重複使用

**初始化邏輯**:

```csharp
// 系統啟動時計算下一個可用 ID
if (data.Stores.Count > 0)
{
    nextStoreId = data.Stores.Max(s => s.Id) + 1;
    
    var allMenuItems = data.Stores.SelectMany(s => s.MenuItems);
    if (allMenuItems.Any())
    {
        nextMenuItemId = allMenuItems.Max(m => m.Id) + 1;
    }
}
```

---

## 業務規則

### 1. 店家唯一性檢查

**規則**: 店家的 `Name`、`Phone`、`Address` 組合必須唯一 (不區分大小寫)

**檢查邏輯**:

```csharp
public async Task<bool> IsDuplicateStoreAsync(
    string name, 
    string phone, 
    string address, 
    int? excludeId = null)
{
    var stores = await GetAllStoresAsync();
    
    return stores.Any(s => 
        s.Id != excludeId && 
        string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase) &&
        s.Phone == phone &&
        string.Equals(s.Address, address, StringComparison.OrdinalIgnoreCase));
}
```

### 2. 菜單項目數量限制

**規則**: 每個店家的菜單項目數量必須介於 1 到 20 筆

**驗證時機**:

- 新增店家時
- 更新店家時 (若修改菜單項目數量)

### 3. 時間戳記自動更新

**規則**:

- `CreatedAt`: 新增店家時自動設定為當前時間
- `UpdatedAt`: 新增和更新店家時自動設定為當前時間

---

## 索引與查詢優化

由於使用 JSON 檔案儲存且資料量小 (< 100 筆)，不需要實體索引。所有查詢在記憶體中執行。

**常見查詢**:

1. **取得所有店家**: 載入完整 JSON
2. **依 ID 查詢**: `stores.FirstOrDefault(s => s.Id == id)`
3. **搜尋店家名稱**: `stores.Where(s => s.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase))`
4. **重複性檢查**: `stores.Any(s => ...)`

---

## 資料遷移考量

### 未來擴展至資料庫

當資料量增長或需要多使用者併發時，可遷移至資料庫 (如 SQL Server、SQLite)。

**遷移步驟**:

1. 使用 Entity Framework Core 建立相同的實體類別
2. 新增 `DbContext` 和 Migration
3. 實作新的 Repository 替代 `JsonFileStorage`
4. 透過 DI 切換 `IFileStorage` 實作

**資料庫結構 (參考)**:

```sql
CREATE TABLE Stores (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Address NVARCHAR(200) NOT NULL,
    PhoneType INT NOT NULL,
    Phone VARCHAR(20) NOT NULL,
    BusinessHours NVARCHAR(100) NOT NULL,
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NOT NULL,
    CONSTRAINT UQ_Store UNIQUE (Name, Phone, Address)
);

CREATE TABLE MenuItems (
    Id INT PRIMARY KEY IDENTITY(1,1),
    StoreId INT NOT NULL,
    Name NVARCHAR(50) NOT NULL,
    Price INT NOT NULL CHECK (Price >= 0),
    Description NVARCHAR(200),
    CONSTRAINT FK_MenuItem_Store FOREIGN KEY (StoreId) 
        REFERENCES Stores(Id) ON DELETE CASCADE
);
```

---

## 總結

資料模型設計完整定義了店家與菜單管理系統的核心實體、關係、驗證規則和業務邏輯。採用簡單的 JSON 檔案儲存，適合練習用專案，同時保留未來擴展至資料庫的彈性。

**下一步**: 定義 API contracts (HTTP 端點和操作)。
