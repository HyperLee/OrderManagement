# OrderManagement 訂餐系統

> 一個基於 ASP.NET Core MVC 的店家與菜單管理系統，支援完整的 CRUD 操作、即時搜尋和資料驗證功能。

[![.NET Version](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Build Status](https://img.shields.io/badge/build-passing-brightgreen.svg)](https://github.com/HyperLee/OrderManagement)

## 📋 目錄

- [專案簡介](#專案簡介)
- [核心功能](#核心功能)
- [技術堆疊](#技術堆疊)
- [系統架構](#系統架構)
- [快速開始](#快速開始)
- [專案結構](#專案結構)
- [資料模型](#資料模型)
- [API 端點](#api-端點)
- [業務規則](#業務規則)
- [測試說明](#測試說明)
- [開發指南](#開發指南)
- [部署說明](#部署說明)
- [常見問題](#常見問題)
- [授權資訊](#授權資訊)

---

## 專案簡介

**OrderManagement** 是一個輕量級的訂餐系統，專注於店家與菜單的管理功能。系統採用 **ASP.NET Core 8.0 MVC** 架構，使用 **JSON 檔案**作為資料儲存方式，適合中小型團隊的訂餐需求或作為學習 ASP.NET Core 的參考專案。

### 🎯 設計目標

- **簡單易用**: 直覺的使用者介面，無需複雜設定即可上手
- **輕量化部署**: 無需資料庫伺服器，使用 JSON 檔案儲存
- **完整測試**: TDD 開發流程，涵蓋單元測試與整合測試
- **可維護性**: 清晰的三層架構，易於擴充與維護
- **現代化 UI**: 響應式設計，支援桌面與行動裝置

### 🚀 適用場景

- 公司內部訂餐系統
- 學習 ASP.NET Core MVC 的實作範例
- 快速原型開發的起點
- 小型團隊的餐點管理工具

---

## 核心功能

### 1. 店家管理 (CRUD)

- ✅ **新增店家**: 建立店家基本資訊（店名、地址、電話、營業時間）
- ✅ **瀏覽列表**: 卡片式呈現所有店家，支援即時搜尋
- ✅ **查看詳情**: 顯示完整店家資訊與菜單列表
- ✅ **編輯資訊**: 更新店家資料與菜單項目
- ✅ **刪除店家**: 移除不再需要的店家（含確認機制）

### 2. 菜單管理

- ✅ **動態新增**: 即時新增菜單項目（最多 20 筆）
- ✅ **項目編輯**: 修改菜名、價格、描述
- ✅ **項目移除**: 刪除菜單項目（至少保留 1 筆）
- ✅ **即時驗證**: 前端表單驗證與後端業務規則檢查

### 3. 資料驗證

- 🔒 **唯一性檢查**: 防止重複店家（店名 + 電話 + 地址組合）
- 🔒 **欄位驗證**: 必填欄位、字數限制、格式驗證
- 🔒 **業務規則**: 菜單數量限制、價格範圍、電話號碼純數字
- 🔒 **防重複提交**: PRG 模式 + 客戶端按鈕禁用

### 4. 使用者體驗

- 🎨 **響應式設計**: Bootstrap 5 框架，支援各種螢幕尺寸
- 🎨 **即時搜尋**: 客戶端篩選，無需重新載入頁面
- 🎨 **即時時間**: 首頁顯示當前時間（每秒更新）
- 🎨 **友善訊息**: 操作成功/失敗的明確提示

### 5. 系統管理

- 📊 **結構化日誌**: Serilog 記錄所有關鍵操作
- 📊 **錯誤處理**: 統一的錯誤處理與友善錯誤頁面
- 📊 **資料備份**: JSON 檔案易於備份與還原
- 📊 **時間戳記**: 自動記錄建立與修改時間

---

## 技術堆疊

### 後端技術

| 技術 | 版本 | 用途 |
|------|------|------|
| .NET | 8.0 | 應用程式框架 |
| ASP.NET Core MVC | 8.0 | Web 應用程式架構 |
| C# | 12.0 | 程式設計語言 |
| Serilog | 3.1.1 | 結構化日誌記錄 |
| System.Text.Json | 內建 | JSON 序列化/反序列化 |

### 前端技術

| 技術 | 版本 | 用途 |
|------|------|------|
| Bootstrap | 5.1.0 | UI 框架 |
| jQuery | 3.5.1 | DOM 操作與事件處理 |
| jQuery Validation | 1.17.0 | 表單驗證 |
| Bootstrap Icons | 1.11.x | 圖示庫 |
| Razor Pages | - | 伺服器端範本引擎 |

### 測試框架

| 技術 | 版本 | 用途 |
|------|------|------|
| xUnit | 2.4.2 | 測試執行器 |
| Moq | 4.18.4 | Mock 物件框架 |
| Microsoft.AspNetCore.Mvc.Testing | 8.0 | 整合測試 |
| Coverlet | 6.0.0 | 程式碼覆蓋率分析 |

### 開發工具

- **IDE**: Visual Studio 2022 / VS Code / JetBrains Rider
- **版本控制**: Git
- **套件管理**: NuGet
- **平台**: macOS / Windows / Linux

---

## 系統架構

### 三層架構設計

```
┌─────────────────────────────────────────────────────────────┐
│                     Presentation Layer                      │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐     │
│  │ HomeController│  │StoreController│  │  Razor Views │     │
│  └──────┬───────┘  └──────┬───────┘  └──────────────┘     │
│         │                  │                                 │
└─────────┼──────────────────┼─────────────────────────────────┘
          │                  │
          ▼                  ▼
┌─────────────────────────────────────────────────────────────┐
│                      Business Layer                         │
│         ┌────────────────────────────────┐                  │
│         │      IStoreService             │                  │
│         │      StoreService              │                  │
│         │  - GetAllStoresAsync()         │                  │
│         │  - GetStoreByIdAsync()         │                  │
│         │  - AddStoreAsync()             │                  │
│         │  - UpdateStoreAsync()          │                  │
│         │  - DeleteStoreAsync()          │                  │
│         │  - IsDuplicateStoreAsync()     │                  │
│         └────────────┬───────────────────┘                  │
└──────────────────────┼──────────────────────────────────────┘
                       │
                       ▼
┌─────────────────────────────────────────────────────────────┐
│                       Data Layer                            │
│         ┌────────────────────────────────┐                  │
│         │      IFileStorage              │                  │
│         │      JsonFileStorage           │                  │
│         │  - GetAllAsync()               │                  │
│         │  - GetByIdAsync()              │                  │
│         │  - AddAsync()                  │                  │
│         │  - UpdateAsync()               │                  │
│         │  - DeleteAsync()               │                  │
│         └────────────┬───────────────────┘                  │
│                      │                                       │
│                      ▼                                       │
│         ┌────────────────────────────────┐                  │
│         │   Data/stores.json (檔案)     │                  │
│         └────────────────────────────────┘                  │
└─────────────────────────────────────────────────────────────┘
```

### 資料流程

1. **使用者請求** → Controller 接收 HTTP 請求
2. **驗證層** → ModelState 驗證 + Data Annotations
3. **業務邏輯** → Service 層執行業務規則檢查
4. **資料存取** → FileStorage 層讀寫 JSON 檔案
5. **回應** → View 渲染 HTML 或 RedirectToAction

### 相依性注入配置

```csharp
// Program.cs
builder.Services.AddSingleton<IFileStorage, JsonFileStorage>();  // 單例模式
builder.Services.AddScoped<IStoreService, StoreService>();        // 範圍模式
```

**設計模式**:
- **Repository Pattern**: `IFileStorage` 抽象化資料存取
- **Dependency Injection**: ASP.NET Core 內建 DI 容器
- **Post-Redirect-Get (PRG)**: 防止表單重複提交
- **Factory Pattern**: 自動 ID 生成邏輯

---

## 快速開始

### 前置需求

- ✅ [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) 或更新版本
- ✅ 文字編輯器或 IDE (VS Code / Visual Studio 2022 / Rider)
- ✅ Git (選用，用於版本控制)

### 安裝步驟

#### 1. 複製專案

```bash
git clone https://github.com/HyperLee/OrderManagement.git
cd OrderManagement
```

#### 2. 還原套件

```bash
dotnet restore
```

#### 3. 執行應用程式

```bash
cd OrderLunchWeb
dotnet run
```

應用程式將在以下網址啟動:
- **HTTPS**: https://localhost:5001
- **HTTP**: http://localhost:5000

#### 4. 開啟瀏覽器

訪問 `https://localhost:5001`，您將看到首頁顯示「訂餐系統」標題和即時時間。

### 執行測試

```bash
# 執行所有測試
cd OrderLunchWeb.Tests
dotnet test

# 執行特定分類的測試
dotnet test --filter "Category=US1"

# 產生程式碼覆蓋率報告
dotnet test --collect:"XPlat Code Coverage"
```

### Docker 部署 (選用)

```bash
# 建置 Docker 映像
docker build -t ordermanagement:latest .

# 執行容器
docker run -d -p 8080:80 --name orderlunch ordermanagement:latest
```

---

## 專案結構

```
OrderManagement/
├── OrderLunchWeb/                 # 主要 Web 應用程式
│   ├── Controllers/               # MVC 控制器
│   │   ├── HomeController.cs      # 首頁控制器
│   │   └── StoreController.cs     # 店家 CRUD 控制器
│   ├── Models/                    # 資料模型
│   │   ├── Store.cs               # 店家實體
│   │   ├── MenuItem.cs            # 菜單項目實體
│   │   ├── PhoneType.cs           # 電話類型列舉
│   │   └── ErrorViewModel.cs     # 錯誤視圖模型
│   ├── Services/                  # 業務邏輯層
│   │   ├── IStoreService.cs       # 店家服務介面
│   │   └── StoreService.cs        # 店家服務實作
│   ├── Data/                      # 資料存取層
│   │   ├── IFileStorage.cs        # 檔案儲存介面
│   │   ├── JsonFileStorage.cs     # JSON 檔案儲存實作
│   │   └── stores.json            # 店家資料檔案 (執行時自動建立)
│   ├── Views/                     # Razor 視圖
│   │   ├── Home/                  # 首頁視圖
│   │   │   └── Index.cshtml       # 首頁 (即時時間 + 主選單)
│   │   ├── Store/                 # 店家視圖
│   │   │   ├── Index.cshtml       # 店家列表 (卡片式 + 搜尋)
│   │   │   ├── Create.cshtml      # 新增店家表單
│   │   │   ├── Details.cshtml     # 店家詳情
│   │   │   ├── Edit.cshtml        # 編輯店家表單
│   │   │   └── Delete.cshtml      # 刪除確認頁面
│   │   └── Shared/                # 共用視圖
│   │       ├── _Layout.cshtml     # 主版面配置
│   │       └── Error.cshtml       # 錯誤頁面
│   ├── wwwroot/                   # 靜態資源
│   │   ├── css/site.css           # 自訂樣式
│   │   ├── js/site.js             # 自訂 JavaScript
│   │   └── lib/                   # 前端函式庫
│   ├── Logs/                      # 日誌檔案目錄
│   │   └── log-YYYYMMDD.txt       # 每日日誌檔案
│   ├── Program.cs                 # 應用程式進入點
│   ├── appsettings.json           # 應用程式設定
│   └── appsettings.Development.json # 開發環境設定
│
├── OrderLunchWeb.Tests/           # 測試專案
│   ├── Unit/                      # 單元測試
│   │   ├── JsonFileStorageTests.cs  # 資料存取層測試
│   │   └── StoreServiceTests.cs     # 業務邏輯層測試
│   ├── Integration/               # 整合測試
│   │   └── StoreControllerTests.cs  # 控制器整合測試
│   └── TestHelpers/               # 測試輔助工具
│       ├── TestDataHelper.cs      # 測試資料產生器
│       └── TestEnvironment.cs     # 測試環境設定
│
├── specs/                         # 專案規格文件
│   └── 001-store-menu-management/ # Feature 001 規格
│       ├── spec.md                # 功能規格書
│       ├── plan.md                # 開發計畫
│       ├── tasks.md               # 工作項目
│       └── ...
│
├── OrderManagement.sln            # Visual Studio 方案檔
└── README.md                      # 本文件
```

### 關鍵目錄說明

| 目錄 | 用途 | 重要檔案 |
|------|------|----------|
| `Controllers/` | 處理 HTTP 請求與回應 | `StoreController.cs` |
| `Services/` | 業務邏輯與規則驗證 | `StoreService.cs` |
| `Data/` | 資料存取與 JSON 檔案操作 | `JsonFileStorage.cs`, `stores.json` |
| `Models/` | 資料模型與驗證規則 | `Store.cs`, `MenuItem.cs` |
| `Views/` | Razor 視圖範本 | `Store/Index.cshtml` |
| `wwwroot/` | 靜態資源 (CSS/JS/Images) | `site.js`, `site.css` |
| `Logs/` | Serilog 日誌輸出 | `log-YYYYMMDD.txt` |

---

## 資料模型

### Store (店家實體)

```csharp
public class Store
{
    public int Id { get; set; }  // 自動遞增 ID (從 1 開始)
    
    [Required(ErrorMessage = "店家名稱為必填欄位")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "店家名稱長度必須在 1-100 字元之間")]
    public string Name { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "店家地址為必填欄位")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "地址長度必須在 1-200 字元之間")]
    public string Address { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "電話類型為必填欄位")]
    public PhoneType PhoneType { get; set; }
    
    [Required(ErrorMessage = "聯絡電話為必填欄位")]
    [RegularExpression(@"^\d+$", ErrorMessage = "電話號碼僅能輸入數字")]
    public string Phone { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "營業時間為必填欄位")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "營業時間長度必須在 1-100 字元之間")]
    public string BusinessHours { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "至少需要新增一個菜單項目")]
    [MinLength(1, ErrorMessage = "至少需要新增一個菜單項目")]
    [MaxLength(20, ErrorMessage = "菜單項目已達上限 (20 筆)")]
    public List<MenuItem> MenuItems { get; set; } = new();
    
    public DateTime CreatedAt { get; set; }   // 系統自動生成
    public DateTime UpdatedAt { get; set; }   // 系統自動維護
}
```

**屬性說明**:

| 屬性 | 類型 | 必填 | 驗證規則 | 說明 |
|------|------|------|----------|------|
| `Id` | `int` | ✅ (自動) | 自動遞增，從 1 開始 | 店家唯一識別碼 |
| `Name` | `string` | ✅ | 1-100 字元 | 店家名稱 |
| `Address` | `string` | ✅ | 1-200 字元 | 店家地址 |
| `PhoneType` | `PhoneType` | ✅ | 列舉值 (1=市話, 2=行動) | 電話類型 |
| `Phone` | `string` | ✅ | 純數字，不含空格/符號 | 聯絡電話 |
| `BusinessHours` | `string` | ✅ | 1-100 字元 | 營業時間 (自由格式) |
| `MenuItems` | `List<MenuItem>` | ✅ | 1-20 筆 | 菜單項目清單 |
| `CreatedAt` | `DateTime` | ✅ (自動) | ISO 8601 格式 | 建立時間戳記 |
| `UpdatedAt` | `DateTime` | ✅ (自動) | ISO 8601 格式 | 最後更新時間戳記 |

**唯一性約束**: `Name` + `Phone` + `Address` 組合必須唯一 (不分大小寫)

---

### MenuItem (菜單項目實體)

```csharp
public class MenuItem
{
    public int Id { get; set; }  // 菜單項目 ID (從 1 開始編號)
    
    [Required(ErrorMessage = "菜名為必填欄位")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "菜名長度必須在 1-50 字元之間")]
    public string Name { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "價格為必填欄位")]
    [Range(0, int.MaxValue, ErrorMessage = "價格必須為正整數或零")]
    public int Price { get; set; }
    
    [StringLength(200, ErrorMessage = "描述長度不可超過 200 字元")]
    public string Description { get; set; } = string.Empty;
}
```

**屬性說明**:

| 屬性 | 類型 | 必填 | 驗證規則 | 說明 |
|------|------|------|----------|------|
| `Id` | `int` | ✅ (自動) | 1-based，店家內部編號 | 菜單項目識別碼 |
| `Name` | `string` | ✅ | 1-50 字元 | 菜品名稱 |
| `Price` | `int` | ✅ | ≥ 0 | 價格 (新台幣，整數) |
| `Description` | `string` | ❌ | 0-200 字元 | 菜品描述 (選填) |

---

### PhoneType (電話類型列舉)

```csharp
public enum PhoneType
{
    [Display(Name = "市話")]
    Landline = 1,
    
    [Display(Name = "行動電話")]
    Mobile = 2
}
```

---

### JSON 儲存格式範例

```json
[
  {
    "Id": 1,
    "Name": "好味便當店",
    "Address": "台北市中正區羅斯福路一段100號",
    "PhoneType": 2,
    "Phone": "0912345678",
    "BusinessHours": "週一~週五 10:00~18:00, 週末公休",
    "MenuItems": [
      {
        "Id": 1,
        "Name": "雞腿便當",
        "Price": 120,
        "Description": "主菜 + 三道配菜 + 白飯"
      },
      {
        "Id": 2,
        "Name": "排骨便當",
        "Price": 110,
        "Description": "主菜 + 三道配菜 + 白飯"
      },
      {
        "Id": 3,
        "Name": "素食便當",
        "Price": 90,
        "Description": "豆腐 + 四道配菜 + 白飯"
      }
    ],
    "CreatedAt": "2025-11-23T09:58:10.257576+08:00",
    "UpdatedAt": "2025-11-23T10:37:20.273586+08:00"
  },
  {
    "Id": 2,
    "Name": "快樂小吃部",
    "Address": "新北市板橋區中山路二段88號",
    "PhoneType": 1,
    "Phone": "0226543210",
    "BusinessHours": "週一~週日 11:00~14:00, 17:00~20:00",
    "MenuItems": [
      {
        "Id": 1,
        "Name": "滷肉飯",
        "Price": 50,
        "Description": "經典滷肉飯"
      }
    ],
    "CreatedAt": "2025-11-23T11:15:30.123456+08:00",
    "UpdatedAt": "2025-11-23T11:15:30.123456+08:00"
  }
]
```

---

## API 端點

### HomeController

#### GET /Home/Index

**用途**: 顯示系統首頁

**回應**: HTML 頁面
- 標題: "訂餐系統"
- 即時時間顯示 (每秒更新)
- 主選單: 「店家列表」、「訂購餐點」

**範例**:

```http
GET https://localhost:5001/Home/Index
```

---

### StoreController

#### GET /Store/Index

**用途**: 顯示所有店家列表

**回應**: HTML 頁面
- 卡片式呈現所有店家
- 搜尋框 (即時客戶端篩選)
- 每個店家顯示: 店名、地址、電話、營業時間
- 操作按鈕: 「查看詳情」、「編輯」、「刪除」

**範例**:

```http
GET https://localhost:5001/Store/Index
```

---

#### GET /Store/Details/{id}

**用途**: 顯示指定店家的詳細資訊

**參數**:
- `id` (int, required): 店家 ID

**回應**: HTML 頁面
- 店家完整資訊
- 菜單項目表格 (ID、菜名、價格、描述)
- 操作按鈕: 「返回列表」、「編輯」、「刪除」

**範例**:

```http
GET https://localhost:5001/Store/Details/1
```

**錯誤處理**:
- 404 Not Found: 店家 ID 不存在

---

#### GET /Store/Create

**用途**: 顯示新增店家表單

**回應**: HTML 頁面
- 店家基本資訊表單 (店名、地址、電話類型、電話、營業時間)
- 菜單項目動態表單 (預設 1 筆空白項目)
- 操作按鈕: 「新增菜單項目」、「提交」、「取消」

**範例**:

```http
GET https://localhost:5001/Store/Create
```

---

#### POST /Store/Create

**用途**: 提交新增店家資料

**請求 Body** (Form Data):

```csharp
Store {
    Name: "便當店名稱",
    Address: "店家地址",
    PhoneType: 2,  // 1=市話, 2=行動
    Phone: "0912345678",
    BusinessHours: "週一~週五 10:00~18:00",
    MenuItems: [
        { Name: "雞腿便當", Price: 120, Description: "主菜 + 三配菜" },
        { Name: "排骨便當", Price: 110, Description: "主菜 + 三配菜" }
    ]
}
```

**成功回應**:
- HTTP 302 Redirect → `/Store/Index`
- TempData["SuccessMessage"] = "店家「XXX」新增成功！"

**失敗回應**:
- HTTP 200 (返回表單頁面)
- ModelState 錯誤訊息

**驗證規則**:
1. 必填欄位檢查 (店名、地址、電話、營業時間)
2. 欄位長度驗證
3. 電話純數字檢查
4. 菜單項目 1-20 筆限制
5. 重複店家檢查 (店名 + 電話 + 地址)

**範例**:

```http
POST https://localhost:5001/Store/Create
Content-Type: application/x-www-form-urlencoded

Name=好味便當店&Address=台北市...&Phone=0912345678&...
```

---

#### GET /Store/Edit/{id}

**用途**: 顯示編輯店家表單

**參數**:
- `id` (int, required): 店家 ID

**回應**: HTML 頁面
- 預填當前店家資料的表單
- 菜單項目列表 (可新增/移除)
- 操作按鈕: 「新增菜單項目」、「儲存」、「取消」

**範例**:

```http
GET https://localhost:5001/Store/Edit/1
```

**錯誤處理**:
- 404 Not Found: 店家 ID 不存在

---

#### POST /Store/Edit/{id}

**用途**: 提交編輯後的店家資料

**參數**:
- `id` (int, required): 店家 ID

**請求 Body**: 同 POST /Store/Create

**成功回應**:
- HTTP 302 Redirect → `/Store/Index`
- TempData["SuccessMessage"] = "店家「XXX」更新成功！"

**失敗回應**:
- HTTP 200 (返回編輯表單頁面)
- ModelState 錯誤訊息

**驗證規則**: 同新增，但重複檢查時排除當前店家自身

**範例**:

```http
POST https://localhost:5001/Store/Edit/1
Content-Type: application/x-www-form-urlencoded

Name=好味便當店&Address=台北市...&...
```

---

#### GET /Store/Delete/{id}

**用途**: 顯示刪除確認頁面

**參數**:
- `id` (int, required): 店家 ID

**回應**: HTML 頁面
- 顯示要刪除的店家完整資訊
- 警告訊息: "確定要刪除此店家嗎？此操作無法復原。"
- 操作按鈕: 「確認刪除」、「取消」

**範例**:

```http
GET https://localhost:5001/Store/Delete/1
```

---

#### POST /Store/Delete/{id}

**用途**: 執行刪除操作

**參數**:
- `id` (int, required): 店家 ID

**成功回應**:
- HTTP 302 Redirect → `/Store/Index`
- TempData["SuccessMessage"] = "店家「XXX」已成功刪除。"

**範例**:

```http
POST https://localhost:5001/Store/Delete/1
```

---

## 業務規則

### 1. ID 管理規則

#### 店家 ID 生成
- **起始值**: 1
- **遞增規則**: 取得當前最大 ID + 1
- **刪除後**: 已刪除的 ID **不會重複使用**
- **實作位置**: `JsonFileStorage.GenerateNewId()`

**範例**:
```
初始狀態: []
新增店家 A → ID = 1
新增店家 B → ID = 2
刪除店家 A (ID=1)
新增店家 C → ID = 3 (不會使用 ID=1)
```

#### 菜單項目 ID 生成
- **起始值**: 1
- **遞增規則**: 店家內部順序編號 (1, 2, 3...)
- **重新編號**: 每次儲存時重新從 1 開始編號
- **實作位置**: `JsonFileStorage.AddAsync()`, `UpdateAsync()`

---

### 2. 唯一性檢查規則

#### 重複店家定義
店家被視為重複，當且僅當以下**三個欄位同時相同** (不分大小寫):
1. 店家名稱 (`Name`)
2. 聯絡電話 (`Phone`)
3. 店家地址 (`Address`)

#### 檢查時機
- **新增店家**: POST /Store/Create 提交時
- **編輯店家**: POST /Store/Edit 提交時 (排除自身)

#### 實作邏輯

```csharp
public async Task<bool> IsDuplicateStoreAsync(
    string name, string phone, string address, int? excludeId = null)
{
    var allStores = await _fileStorage.GetAllAsync();
    return allStores.Any(s => 
        s.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
        s.Phone.Equals(phone, StringComparison.OrdinalIgnoreCase) &&
        s.Address.Equals(address, StringComparison.OrdinalIgnoreCase) &&
        s.Id != excludeId);  // 編輯時排除自己
}
```

**錯誤訊息**: "此店家已存在（店名、電話、地址完全相同）"

---

### 3. 菜單項目限制

#### 數量限制
- **最少**: 1 筆 (必須至少有一個菜單項目)
- **最多**: 20 筆

#### 驗證點
1. **伺服器端**: ModelState 驗證 (`[MinLength(1), MaxLength(20)]`)
2. **客戶端**: JavaScript 即時檢查 (達到 20 筆時禁用「新增菜單項目」按鈕)

#### 移除限制
- 當菜單項目數量 = 1 時，無法移除最後一項
- 客戶端顯示警告: "至少需要保留 1 個菜單項目！"

---

### 4. 時間戳記規則

#### CreatedAt (建立時間)
- **設定時機**: 新增店家時
- **格式**: ISO 8601 (含時區)
- **後續操作**: **永不變更**

#### UpdatedAt (更新時間)
- **設定時機**: 新增或編輯店家時
- **格式**: ISO 8601 (含時區)
- **後續操作**: 每次編輯時更新為當前時間

**實作位置**: `JsonFileStorage.AddAsync()`, `UpdateAsync()`

```csharp
// 新增時
store.CreatedAt = DateTime.Now;
store.UpdatedAt = DateTime.Now;

// 編輯時
existingStore.UpdatedAt = DateTime.Now;
// CreatedAt 保持不變
```

---

### 5. 資料驗證規則總覽

| 欄位 | 必填 | 格式 | 長度限制 | 其他規則 |
|------|------|------|----------|----------|
| 店家名稱 | ✅ | 文字 | 1-100 字元 | - |
| 店家地址 | ✅ | 文字 | 1-200 字元 | - |
| 電話類型 | ✅ | 列舉 | - | 1=市話, 2=行動 |
| 聯絡電話 | ✅ | 純數字 | 不限 | 不含空格/符號 |
| 營業時間 | ✅ | 文字 | 1-100 字元 | 自由格式 |
| 菜單項目 | ✅ | 陣列 | 1-20 筆 | - |
| 菜名 | ✅ | 文字 | 1-50 字元 | - |
| 價格 | ✅ | 整數 | - | ≥ 0 |
| 菜品描述 | ❌ | 文字 | 0-200 字元 | - |

---

### 6. 防重複提交機制

#### 客戶端防護
```javascript
let isSubmitting = false;
form.on('submit', function (e) {
    if (isSubmitting) {
        e.preventDefault();
        return false;
    }
    isSubmitting = true;
    submitBtn.prop('disabled', true)
        .html('<span class="spinner-border spinner-border-sm"></span> 處理中...');
});
```

#### 伺服器端防護 (PRG 模式)
```csharp
// Post-Redirect-Get Pattern
[HttpPost]
public async Task<IActionResult> Create(Store store)
{
    // 處理邏輯...
    TempData["SuccessMessage"] = "新增成功！";
    return RedirectToAction(nameof(Index));  // 重定向，防止 F5 重複提交
}
```

---

### 7. 錯誤處理規則

#### 驗證錯誤
- 顯示位置: 表單頂部 + 欄位旁邊
- 樣式: 紅色警告框 (Bootstrap `.alert-danger`)
- 訊息格式: 明確指出問題欄位與修正方式

#### 業務邏輯錯誤
- 重複店家: "此店家已存在（店名、電話、地址完全相同）"
- 檔案讀寫錯誤: "系統錯誤，請稍後再試"

#### 系統錯誤
- 顯示通用錯誤頁面 (`Error.cshtml`)
- 記錄詳細錯誤到日誌檔案
- 不洩漏敏感資訊給使用者

---

## 測試說明

### 測試架構

專案採用 **測試驅動開發 (TDD)** 方法，包含兩類測試:

#### 1. 單元測試 (Unit Tests)
- **目錄**: `OrderLunchWeb.Tests/Unit/`
- **測試對象**: 獨立的類別與方法
- **Mock 依賴**: 使用 Moq 框架
- **涵蓋範圍**: Services 層、Data 層

#### 2. 整合測試 (Integration Tests)
- **目錄**: `OrderLunchWeb.Tests/Integration/`
- **測試對象**: Controllers 與完整的請求/回應流程
- **測試環境**: `WebApplicationFactory<Program>`
- **涵蓋範圍**: 端對端場景、業務流程

---

### 測試統計

| 測試類型 | 檔案數 | 測試方法數 | 覆蓋率 |
|---------|--------|-----------|--------|
| 單元測試 | 2 | 34 | ~85% |
| 整合測試 | 1 | 27 | ~90% |
| **總計** | **3** | **61** | **~87%** |

---

### 執行測試

#### 執行所有測試

```bash
cd OrderLunchWeb.Tests
dotnet test
```

#### 執行特定 User Story 測試

```bash
# US1: 新增店家與菜單
dotnet test --filter "Category=US1"

# US2: 瀏覽店家列表
dotnet test --filter "Category=US2"

# US3: 編修店家資訊
dotnet test --filter "Category=US3"

# US4: 刪除店家資訊
dotnet test --filter "Category=US4"
```

#### 執行特定測試類別

```bash
dotnet test --filter "FullyQualifiedName~JsonFileStorageTests"
dotnet test --filter "FullyQualifiedName~StoreServiceTests"
dotnet test --filter "FullyQualifiedName~StoreControllerTests"
```

#### 產生程式碼覆蓋率報告

```bash
# 產生覆蓋率資料
dotnet test --collect:"XPlat Code Coverage"

# 覆蓋率報告位置
# TestResults/{GUID}/coverage.cobertura.xml
```

#### 使用 ReportGenerator 產生 HTML 報告

```bash
# 安裝 ReportGenerator (全域工具)
dotnet tool install -g dotnet-reportgenerator-globaltool

# 產生 HTML 報告
reportgenerator \
  -reports:"TestResults/*/coverage.cobertura.xml" \
  -targetdir:"TestResults/CoverageReport" \
  -reporttypes:Html

# 開啟報告
open TestResults/CoverageReport/index.html
```

---

### 測試範例

#### 單元測試範例 - JsonFileStorageTests.cs

```csharp
[Fact]
[Trait("Category", "US1")]
public async Task AddAsync_ShouldAutoIncrementId_WhenMultipleStoresAdded()
{
    // Arrange
    var store1 = TestDataHelper.CreateValidStore("店家1", "0912345678", "地址1");
    var store2 = TestDataHelper.CreateValidStore("店家2", "0923456789", "地址2");
    
    // Act
    var added1 = await _storage.AddAsync(store1);
    var added2 = await _storage.AddAsync(store2);
    
    // Assert
    Assert.Equal(1, added1.Id);  // 第一筆 ID = 1
    Assert.Equal(2, added2.Id);  // 第二筆 ID = 2
}

[Fact]
[Trait("Category", "US1")]
public async Task AddAsync_ShouldNotReuseDeletedIds_WhenStoreIsDeleted()
{
    // Arrange
    var store1 = TestDataHelper.CreateValidStore("店家1", "0912345678", "地址1");
    var store2 = TestDataHelper.CreateValidStore("店家2", "0923456789", "地址2");
    var store3 = TestDataHelper.CreateValidStore("店家3", "0934567890", "地址3");
    
    // Act
    await _storage.AddAsync(store1);  // ID = 1
    await _storage.AddAsync(store2);  // ID = 2
    await _storage.DeleteAsync(1);    // 刪除 ID = 1
    var added3 = await _storage.AddAsync(store3);
    
    // Assert
    Assert.Equal(3, added3.Id);  // ID = 3，不會重複使用 1
}
```

#### 整合測試範例 - StoreControllerTests.cs

```csharp
[Fact]
[Trait("Category", "US1")]
public async Task PostCreate_ShouldReturnRedirect_WhenValidStoreIsProvided()
{
    // Arrange
    var store = TestDataHelper.CreateValidStore(
        "便當店", "0912345678", "台北市中正區");
    
    // Act
    var result = await _controller.Create(store);
    
    // Assert
    var redirectResult = Assert.IsType<RedirectToActionResult>(result);
    Assert.Equal("Index", redirectResult.ActionName);
    Assert.Contains("新增成功", _controller.TempData["SuccessMessage"]?.ToString());
}

[Fact]
[Trait("Category", "US1")]
public async Task PostCreate_ShouldReturnViewWithError_WhenDuplicateStoreExists()
{
    // Arrange
    var store1 = TestDataHelper.CreateValidStore(
        "便當店", "0912345678", "台北市");
    var store2 = TestDataHelper.CreateValidStore(
        "便當店", "0912345678", "台北市");  // 重複
    
    await _controller.Create(store1);  // 先新增第一筆
    
    // Act
    var result = await _controller.Create(store2);  // 嘗試新增重複
    
    // Assert
    var viewResult = Assert.IsType<ViewResult>(result);
    Assert.False(_controller.ModelState.IsValid);
    Assert.Contains("已存在", 
        _controller.ModelState[""]?.Errors[0].ErrorMessage);
}
```

---

### 測試輔助工具

#### TestDataHelper.cs - 測試資料產生器

```csharp
public static class TestDataHelper
{
    public static Store CreateValidStore(
        string name, string phone, string address)
    {
        return new Store
        {
            Name = name,
            Address = address,
            PhoneType = PhoneType.Mobile,
            Phone = phone,
            BusinessHours = "週一至週五 11:00-14:00",
            MenuItems = new List<MenuItem>
            {
                new MenuItem 
                { 
                    Name = "排骨便當", 
                    Price = 80,
                    Description = "主菜 + 三配菜"
                }
            }
        };
    }
    
    public static Store CreateStoreWithMultipleMenuItems(int itemCount)
    {
        var store = CreateValidStore("測試店家", "0912345678", "測試地址");
        store.MenuItems.Clear();
        
        for (int i = 1; i <= itemCount; i++)
        {
            store.MenuItems.Add(new MenuItem
            {
                Name = $"菜品{i}",
                Price = 50 + i * 10,
                Description = $"描述{i}"
            });
        }
        
        return store;
    }
}
```

---

## 開發指南

### 開發環境設定

#### 1. 複製專案並安裝相依套件

```bash
git clone https://github.com/HyperLee/OrderManagement.git
cd OrderManagement
dotnet restore
```

#### 2. 開啟 IDE

**Visual Studio 2022**:
```bash
open OrderManagement.sln
```

**VS Code**:
```bash
code .
```

**JetBrains Rider**:
```bash
rider OrderManagement.sln
```

#### 3. 設定 appsettings.Development.json (選用)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information"
    }
  }
}
```

---

### TDD 開發工作流程

專案遵循 **Red-Green-Refactor** 循環:

```
1. 撰寫測試 (Red)
   ↓
2. 執行測試 → 確認失敗
   ↓
3. 實作最少程式碼使測試通過 (Green)
   ↓
4. 執行測試 → 確認通過
   ↓
5. 重構程式碼 (Refactor)
   ↓
6. 執行測試 → 確認仍然通過
   ↓
7. 提交變更
```

#### 範例: 新增功能的 TDD 流程

**步驟 1: 撰寫失敗測試**

```csharp
[Fact]
public async Task SearchStores_ShouldReturnFilteredResults()
{
    // Arrange
    await AddTestStore("便當店", "0912345678");
    await AddTestStore("小吃部", "0923456789");
    
    // Act
    var results = await _service.SearchStoresAsync("便當");
    
    // Assert
    Assert.Single(results);
    Assert.Equal("便當店", results[0].Name);
}
```

**步驟 2: 執行測試 → 紅燈 (失敗)**

```bash
dotnet test
# 結果: SearchStoresAsync 方法不存在
```

**步驟 3: 實作功能**

```csharp
public async Task<List<Store>> SearchStoresAsync(string keyword)
{
    var allStores = await _fileStorage.GetAllAsync();
    return allStores
        .Where(s => s.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase))
        .ToList();
}
```

**步驟 4: 執行測試 → 綠燈 (通過)**

```bash
dotnet test
# 結果: All tests passed
```

**步驟 5: 重構 (提升程式碼品質)**

```csharp
public async Task<List<Store>> SearchStoresAsync(string keyword)
{
    if (string.IsNullOrWhiteSpace(keyword))
        return await _fileStorage.GetAllAsync();
    
    var allStores = await _fileStorage.GetAllAsync();
    return allStores
        .Where(s => s.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase))
        .ToList();
}
```

**步驟 6: 再次執行測試 → 確認綠燈**

**步驟 7: 提交變更**

```bash
git add .
git commit -m "feat: 新增店家搜尋功能 (US2-Scenario3)"
```

---

### 程式碼結構導覽

#### Controllers (控制器層)

**職責**: 處理 HTTP 請求、驗證、調用 Service、返回 View/Redirect

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(Store store)
{
    // 1. ModelState 驗證
    if (!ModelState.IsValid)
        return View(store);
    
    // 2. 業務規則檢查
    if (await _storeService.IsDuplicateStoreAsync(...))
    {
        ModelState.AddModelError("", "重複店家");
        return View(store);
    }
    
    // 3. 執行業務邏輯
    var addedStore = await _storeService.AddStoreAsync(store);
    
    // 4. 重定向 (PRG 模式)
    TempData["SuccessMessage"] = "新增成功！";
    return RedirectToAction(nameof(Index));
}
```

#### Services (業務邏輯層)

**職責**: 業務規則驗證、資料轉換、協調多個資料存取操作

```csharp
public class StoreService : IStoreService
{
    private readonly IFileStorage _fileStorage;
    private readonly ILogger<StoreService> _logger;
    
    public async Task<Store> AddStoreAsync(Store store)
    {
        _logger.LogInformation("新增店家: {StoreName}", store.Name);
        
        // 呼叫資料存取層
        var addedStore = await _fileStorage.AddAsync(store);
        
        _logger.LogInformation("成功新增店家，ID: {StoreId}", addedStore.Id);
        return addedStore;
    }
    
    public async Task<bool> IsDuplicateStoreAsync(
        string name, string phone, string address, int? excludeId = null)
    {
        var allStores = await _fileStorage.GetAllAsync();
        return allStores.Any(s => 
            s.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
            s.Phone.Equals(phone, StringComparison.OrdinalIgnoreCase) &&
            s.Address.Equals(address, StringComparison.OrdinalIgnoreCase) &&
            s.Id != excludeId);
    }
}
```

#### Data (資料存取層)

**職責**: 檔案讀寫、ID 生成、執行緒安全

```csharp
public class JsonFileStorage : IFileStorage
{
    private readonly string _filePath = "Data/stores.json";
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    
    public async Task<Store> AddAsync(Store store)
    {
        await _semaphore.WaitAsync();
        try
        {
            var stores = await GetAllAsync();
            
            // ID 自動生成
            store.Id = GenerateNewId(stores);
            
            // 時間戳記
            store.CreatedAt = DateTime.Now;
            store.UpdatedAt = DateTime.Now;
            
            // 菜單項目 ID 重新編號
            for (int i = 0; i < store.MenuItems.Count; i++)
                store.MenuItems[i].Id = i + 1;
            
            stores.Add(store);
            await SaveAllAsync(stores);
            
            return store;
        }
        finally
        {
            _semaphore.Release();
        }
    }
    
    private int GenerateNewId(List<Store> stores)
        => stores.Count == 0 ? 1 : stores.Max(s => s.Id) + 1;
}
```

---

### 日誌記錄

#### Serilog 配置 (Program.cs)

```csharp
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File(
        path: "Logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        encoding: Encoding.UTF8)
    .CreateLogger();
```

#### 日誌使用範例

```csharp
// Information 層級 - 正常業務流程
_logger.LogInformation("接收新增店家請求: {StoreName}", store.Name);
_logger.LogInformation("成功新增店家，ID: {StoreId}, 名稱: {StoreName}", 
    addedStore.Id, addedStore.Name);

// Warning 層級 - 業務規則違反
_logger.LogWarning("發現重複店家: 名稱={StoreName}, 電話={Phone}", 
    store.Name, store.Phone);

// Error 層級 - 系統錯誤
_logger.LogError(ex, "新增店家時發生錯誤: {StoreName}", store.Name);
```

#### 日誌輸出範例

```text
[2025-11-23 10:37:20 INF] 接收新增店家請求: 好味便當店
[2025-11-23 10:37:20 INF] 成功新增店家，ID: 1, 名稱: 好味便當店
[2025-11-23 10:38:15 WRN] 發現重複店家: 名稱=好味便當店, 電話=0912345678
```

---

### Git 工作流程

#### 分支策略

- `main`: 穩定版本 (生產環境)
- `001-store-menu-management`: Feature 分支 (目前開發)
- `feature/*`: 其他功能分支

#### Commit 訊息規範

遵循 [Conventional Commits](https://www.conventionalcommits.org/):

```
<type>(<scope>): <subject>

<body>

<footer>
```

**Type 類型**:
- `feat`: 新增功能
- `fix`: 修復 Bug
- `test`: 新增或修改測試
- `refactor`: 重構程式碼
- `docs`: 文件更新
- `style`: 程式碼格式調整
- `chore`: 建置工具或相依套件更新

**範例**:

```bash
# 新增功能
git commit -m "feat(store): 新增店家搜尋功能 (US2-Scenario3)"

# 修復 Bug
git commit -m "fix(validation): 修正電話號碼驗證正則表達式"

# 新增測試
git commit -m "test(store): 新增重複店家檢查測試案例"

# 重構
git commit -m "refactor(storage): 提取 ID 生成邏輯為獨立方法"
```

---

## 部署說明

### 本機部署

#### 1. 發佈應用程式

```bash
cd OrderLunchWeb
dotnet publish -c Release -o ./publish
```

#### 2. 執行已發佈的應用程式

```bash
cd publish
dotnet OrderLunchWeb.dll
```

---

### IIS 部署 (Windows Server)

#### 1. 安裝 .NET 8.0 Hosting Bundle

下載並安裝: [.NET 8.0 Hosting Bundle](https://dotnet.microsoft.com/download/dotnet/8.0)

#### 2. 發佈應用程式

```bash
dotnet publish -c Release -o C:\inetpub\wwwroot\OrderLunchWeb
```

#### 3. 設定 IIS

1. 開啟 **IIS Manager**
2. 建立新的應用程式集區:
   - 名稱: `OrderLunchWebAppPool`
   - .NET CLR 版本: **無受控程式碼**
3. 建立新網站:
   - 網站名稱: `OrderLunchWeb`
   - 實體路徑: `C:\inetpub\wwwroot\OrderLunchWeb`
   - 應用程式集區: `OrderLunchWebAppPool`
   - 繫結: `http://*:80`
4. 設定應用程式集區身分識別的檔案權限

#### 4. 設定 web.config

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <system.webServer>
    <handlers>
      <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" />
    </handlers>
    <aspNetCore processPath="dotnet"
                arguments=".\OrderLunchWeb.dll"
                stdoutLogEnabled="true"
                stdoutLogFile=".\logs\stdout"
                hostingModel="inprocess" />
  </system.webServer>
</configuration>
```

---

### Docker 部署

#### 1. 建立 Dockerfile

```dockerfile
# 建置階段
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["OrderLunchWeb/OrderLunchWeb.csproj", "OrderLunchWeb/"]
RUN dotnet restore "OrderLunchWeb/OrderLunchWeb.csproj"

COPY . .
WORKDIR "/src/OrderLunchWeb"
RUN dotnet build "OrderLunchWeb.csproj" -c Release -o /app/build

# 發佈階段
FROM build AS publish
RUN dotnet publish "OrderLunchWeb.csproj" -c Release -o /app/publish

# 執行階段
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# 建立資料目錄
RUN mkdir -p /app/Data

EXPOSE 80
ENTRYPOINT ["dotnet", "OrderLunchWeb.dll"]
```

#### 2. 建立 .dockerignore

```text
**/bin/
**/obj/
**/out/
**/.vs/
**/.vscode/
**/node_modules/
**/TestResults/
**/Logs/
```

#### 3. 建置與執行

```bash
# 建置映像
docker build -t ordermanagement:1.0 .

# 執行容器
docker run -d \
  -p 8080:80 \
  --name orderlunch \
  -v orderlunch-data:/app/Data \
  ordermanagement:1.0

# 查看日誌
docker logs orderlunch

# 停止容器
docker stop orderlunch

# 移除容器
docker rm orderlunch
```

#### 4. Docker Compose (選用)

**docker-compose.yml**:

```yaml
version: '3.8'

services:
  web:
    build: .
    ports:
      - "8080:80"
    volumes:
      - orderlunch-data:/app/Data
      - orderlunch-logs:/app/Logs
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
    restart: unless-stopped

volumes:
  orderlunch-data:
  orderlunch-logs:
```

執行:

```bash
docker-compose up -d
docker-compose logs -f
docker-compose down
```

---

### Azure App Service 部署

#### 1. 安裝 Azure CLI

```bash
brew install azure-cli  # macOS
# 或從 https://aka.ms/installazurecliwindows 下載 (Windows)
```

#### 2. 登入 Azure

```bash
az login
```

#### 3. 建立資源群組

```bash
az group create --name OrderLunchWebRG --location eastasia
```

#### 4. 建立 App Service Plan

```bash
az appservice plan create \
  --name OrderLunchWebPlan \
  --resource-group OrderLunchWebRG \
  --sku B1 \
  --is-linux
```

#### 5. 建立 Web App

```bash
az webapp create \
  --name orderlunchweb \
  --resource-group OrderLunchWebRG \
  --plan OrderLunchWebPlan \
  --runtime "DOTNETCORE:8.0"
```

#### 6. 部署應用程式

```bash
# 發佈為 ZIP
dotnet publish -c Release -o ./publish
cd publish
zip -r ../app.zip *

# 部署到 Azure
az webapp deployment source config-zip \
  --resource-group OrderLunchWebRG \
  --name orderlunchweb \
  --src ../app.zip
```

#### 7. 開啟網站

```bash
az webapp browse --name orderlunchweb --resource-group OrderLunchWebRG
```

---

## 常見問題

### Q1: 為什麼使用 JSON 檔案而不是資料庫？

**A**: 本專案是學習用途，著重於 ASP.NET Core MVC 架構與 TDD 實踐。JSON 檔案具有以下優勢:
- ✅ 無需安裝資料庫伺服器
- ✅ 易於備份與版本控制
- ✅ 適合小型資料量 (< 1000 筆)
- ✅ 簡化部署流程

**未來擴充**: 可輕鬆替換為 Entity Framework Core + SQL Server/PostgreSQL，只需實作 `IFileStorage` 介面。

---

### Q2: 如何備份資料？

**A**: 複製 `Data/stores.json` 檔案即可:

```bash
# 備份
cp Data/stores.json Data/stores_backup_20251123.json

# 還原
cp Data/stores_backup_20251123.json Data/stores.json
```

**自動備份 (cron job)**:

```bash
# 每天凌晨 2 點備份
0 2 * * * cp /path/to/Data/stores.json /path/to/backups/stores_$(date +\%Y\%m\%d).json
```

---

### Q3: 如何清空所有資料？

**A**: 刪除 `Data/stores.json` 檔案，系統會自動重新建立空檔案:

```bash
rm Data/stores.json
# 重新啟動應用程式，stores.json 會自動建立
```

---

### Q4: 如何修改日誌設定？

**A**: 編輯 `Program.cs` 中的 Serilog 配置:

```csharp
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()  // 修改最低日誌層級
    .WriteTo.Console()
    .WriteTo.File(
        path: "Logs/log-.txt",
        rollingInterval: RollingInterval.Hour,  // 修改為每小時滾動
        retainedFileCountLimit: 7)  // 保留 7 個日誌檔案
    .CreateLogger();
```

---

### Q5: 如何新增客製化驗證規則？

**A**: 建立自訂 ValidationAttribute:

```csharp
public class PhoneNumberAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(
        object value, ValidationContext validationContext)
    {
        var phone = value as string;
        if (string.IsNullOrEmpty(phone))
            return ValidationResult.Success;
        
        // 自訂驗證邏輯
        if (phone.StartsWith("09") && phone.Length == 10)
            return ValidationResult.Success;
        
        return new ValidationResult("行動電話格式錯誤 (應為 09 開頭的 10 碼數字)");
    }
}

// 使用
public class Store
{
    [PhoneNumber]
    public string Phone { get; set; }
}
```

---

### Q6: 如何切換到資料庫儲存？

**A**: 實作 `IFileStorage` 介面並替換 DI 註冊:

```csharp
// 新增 EntityFramework Storage
public class EfCoreStorage : IFileStorage
{
    private readonly AppDbContext _context;
    
    public async Task<List<Store>> GetAllAsync()
        => await _context.Stores.Include(s => s.MenuItems).ToListAsync();
    
    // ... 實作其他方法
}

// Program.cs - 替換註冊
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddScoped<IFileStorage, EfCoreStorage>();  // 替換實作
```

---

### Q7: 多人同時編輯同一筆資料怎麼辦？

**A**: 目前採用「最後寫入者勝出 (Last Write Wins)」策略。若需樂觀鎖定，可:

1. 在 `Store` 模型新增 `RowVersion` 屬性
2. 更新時檢查版本號是否一致
3. 版本不一致時顯示衝突訊息

```csharp
public class Store
{
    public int RowVersion { get; set; }  // 版本號
}

// UpdateAsync 檢查
if (existingStore.RowVersion != store.RowVersion)
    throw new DbUpdateConcurrencyException("資料已被其他使用者修改");
```

---

### Q8: 如何新增權限控制？

**A**: 使用 ASP.NET Core Identity:

```bash
# 安裝套件
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore

# 新增 [Authorize] 屬性
[Authorize(Roles = "Admin")]
public async Task<IActionResult> Create(Store store)
{
    // 僅管理員可新增店家
}
```

---

## 授權資訊

本專案採用 **MIT License** 授權。

```text
MIT License

Copyright (c) 2025 HyperLee

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

---

## 聯絡資訊

- **作者**: HyperLee
- **GitHub**: [https://github.com/HyperLee/OrderManagement](https://github.com/HyperLee/OrderManagement)
- **Email**: (請在 GitHub 專案頁面提交 Issue)

---

## 貢獻指南

歡迎提交 Pull Request 或回報 Issue！

### 如何貢獻

1. Fork 本專案
2. 建立功能分支 (`git checkout -b feature/AmazingFeature`)
3. 撰寫測試並確保通過 (`dotnet test`)
4. 提交變更 (`git commit -m 'feat: 新增超棒功能'`)
5. 推送到分支 (`git push origin feature/AmazingFeature`)
6. 開啟 Pull Request

### 程式碼風格

- 遵循 C# 官方命名慣例
- 使用有意義的變數與方法名稱
- 保持方法簡短 (< 30 行)
- 撰寫 XML 文件註解

---

## 更新日誌

### Version 1.0.0 (2025-11-23)

#### ✨ 新增功能
- 完整的店家 CRUD 操作
- 菜單項目動態管理 (1-20 筆)
- 即時搜尋功能 (客戶端篩選)
- 重複店家檢查機制
- 防重複提交機制 (PRG 模式)
- Serilog 結構化日誌
- 完整的單元測試與整合測試 (61 個測試案例)

#### 📚 文件
- 詳細的 README 文件
- 功能規格書 (specs/001-store-menu-management/)
- TDD 開發計畫
- API 端點文件

---

**🎉 感謝您使用 OrderManagement 訂餐系統！如有任何問題或建議，歡迎在 GitHub 上提出 Issue。**
