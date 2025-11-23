# Implementation Plan: 訂餐功能系統

**Branch**: `002-order-food` | **Date**: 2025年11月23日 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/002-order-food/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/commands/plan.md` for the execution workflow.

## Summary

實作完整的訂餐功能系統，使用者可以從餐廳列表選擇餐廳、瀏覽菜單、將菜品加入訂單、完成結帳並查看訂單歷史紀錄。系統使用 ASP.NET Core 8.0 MVC 架構，採用 JSON 檔案儲存訂單資料，瀏覽器 Session Storage 管理購物車狀態，並在應用程式啟動時自動清理超過 5 天的舊訂單。

## Technical Context

**Language/Version**: C# 13 / .NET 8.0  
**Primary Dependencies**: ASP.NET Core 8.0 MVC, Serilog 9.0.0 (日誌), Bootstrap 5 (UI 元件庫)  
**Storage**: JSON 檔案儲存 (orders.json, 使用 UTF-8 編碼避免中文亂碼), 瀏覽器 Session Storage (購物車暫存)  
**Testing**: xUnit (單元測試和整合測試)  
**Target Platform**: Web 應用程式 (支援桌面和行動裝置瀏覽器)  
**Project Type**: Web MVC 應用程式  
**Performance Goals**: API 端點 p95 回應時間 < 200ms, 訂單摘要更新延遲 < 500ms, 訂單紀錄查詢 < 2 秒  
**Constraints**: 單一請求記憶體使用 < 100MB, 支援至少 100 筆訂單資料, 訂單資料保留 5 天  
**Scale/Scope**: 小型練習專案, 約 30 個使用者故事驗收場景, 7 個 MVC 頁面, 預估 100+ 筆訂單資料

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

根據 OrderManagement 訂餐管理系統憲章的六大核心原則評估本功能：

### I. 程式碼品質至上 ✅ 通過

- 將使用 C# 13 最新功能（檔案範圍命名空間、模式匹配、null 安全性）
- 所有公開 API 將包含 XML 文件註解
- 遵循命名規範（PascalCase 公開成員、camelCase 私有欄位、介面前綴 "I"）
- 錯誤處理覆蓋所有邊界情況（如空餐廳列表、數量驗證、電話格式驗證）

### II. 測試優先開發 ✅ 通過

- 規格中定義了 30 個獨立可測試的驗收場景（7 個使用者故事 × 平均 4-5 個場景）
- 每個使用者故事都設計為獨立可測試的 MVP 增量
- 將為關鍵業務邏輯（訂單計算、資料驗證、狀態管理）撰寫單元測試
- 將為 Controller 端點和訂單流程撰寫整合測試
- 遵循現有測試命名風格（不使用 "Arrange", "Act", "Assert" 註解）

### III. 使用者體驗一致性 ✅ 通過

- 使用現有的 Bootstrap 5 元件庫確保 UI 一致性
- 所有頁面將支援回應式設計（桌面和行動裝置）
- 提供即時驗證回饋（姓名必填、電話格式、數量驗證）
- 錯誤訊息清晰且可操作（如「電話號碼僅能輸入數字」）
- 使用者故事已按業務價值排序（P1: 核心訂餐流程, P2: 歷史紀錄和提示, P3: 返回修改）

### IV. 效能與延展性 ✅ 通過

- 目標回應時間符合憲章要求（API 端點 < 200ms, 訂單摘要更新 < 500ms）
- JSON 檔案讀寫將使用非同步 I/O（async/await 模式）
- 訂單資料限制在 100 筆以內（自動清理 5 天前的資料）
- 購物車使用 Session Storage 避免伺服器記憶體壓力
- 單一請求記憶體使用預估 < 100MB（符合憲章限制）

### V. 可觀察性與監控 ✅ 通過

- 將使用現有的 Serilog 結構化日誌記錄關鍵事件
- 記錄所有訂單建立、資料驗證錯誤、檔案讀寫異常
- 使用適當的日誌層級（Information: 訂單建立, Warning: 驗證失敗, Error: 檔案異常）
- 日誌將包含關聯 ID（訂單編號）以便追蹤
- 應用程式啟動時記錄舊訂單清理結果

### VI. 安全優先 ⚠️ 部分符合（可接受）

- **輸入驗證**: ✅ 所有使用者輸入將驗證（姓名必填、電話數字格式、數量正整數）
- **SQL 注入防護**: ✅ N/A（使用 JSON 檔案儲存，無 SQL 查詢）
- **認證/授權**: ⚠️ 本功能不實作認證授權（練習專案，降低複雜度）
- **HTTPS**: ✅ 使用 ASP.NET Core 預設 HTTPS 設定
- **敏感資料保護**: ✅ N/A（無密碼或金鑰儲存，訂餐者姓名和電話僅儲存於本機 JSON）

**安全性說明**: 本功能為練習專案，不實作使用者認證和授權系統。在生產環境中，應加入：

1. 使用者登入機制（JWT 或 Cookie Authentication）
2. 訂單僅能由建立者查看和修改
3. 管理員角色管理餐廳和菜單

### 治理規則檢查 ✅ 通過

- 所有文件使用繁體中文 (zh-TW)
- 遵循現有專案結構（Models, Services, Controllers, Data, Views）
- 不新增額外專案（單一 Web MVC 專案）
- 不引入複雜設計模式（使用現有的 Service + Repository 模式）

### 總結

**Gate Status**: ✅ **通過** - 本功能設計符合憲章要求，安全性部分因練習專案性質未實作認證授權，此決策已明確說明並記錄於 Complexity Tracking。

## Project Structure

### Documentation (this feature)

```text
specs/[###-feature]/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/           # Phase 1 output (/speckit.plan command)
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### Source Code (repository root)

```text
OrderLunchWeb/                    # 主要 ASP.NET Core MVC 專案
├── Controllers/
│   ├── HomeController.cs         # 現有：首頁控制器
│   ├── StoreController.cs        # 現有：餐廳管理控制器
│   └── OrderController.cs        # 新增：訂單控制器（訂餐流程、結帳、訂單紀錄）
├── Models/
│   ├── Store.cs                  # 現有：餐廳模型
│   ├── MenuItem.cs               # 現有：菜品模型
│   ├── Order.cs                  # 新增：訂單模型
│   ├── OrderItem.cs              # 新增：訂單項目模型
│   ├── OrderStatus.cs            # 新增：訂單狀態列舉
│   └── ErrorViewModel.cs         # 現有：錯誤視圖模型
├── Services/
│   ├── IStoreService.cs          # 現有：餐廳服務介面
│   ├── StoreService.cs           # 現有：餐廳服務實作
│   ├── IOrderService.cs          # 新增：訂單服務介面
│   └── OrderService.cs           # 新增：訂單服務實作
├── Data/
│   ├── IFileStorage.cs           # 現有：檔案儲存介面
│   ├── JsonFileStorage.cs        # 現有：JSON 檔案儲存實作
│   ├── stores.json               # 現有：餐廳資料
│   └── orders.json               # 新增：訂單資料（UTF-8 編碼）
├── Views/
│   ├── Order/                    # 新增：訂單視圖資料夾
│   │   ├── SelectRestaurant.cshtml   # 餐廳列表頁面
│   │   ├── Menu.cshtml               # 菜單頁面（含訂單摘要）
│   │   ├── Checkout.cshtml           # 結帳頁面
│   │   ├── Confirmation.cshtml       # 訂單確認頁面
│   │   ├── History.cshtml            # 訂單紀錄頁面
│   │   └── Details.cshtml            # 訂單詳情頁面
│   ├── Home/
│   │   └── Index.cshtml          # 現有：首頁（需新增「訂購餐點」按鈕）
│   └── Shared/
│       └── _Layout.cshtml        # 現有：共用版面配置
├── wwwroot/
│   ├── js/
│   │   ├── site.js               # 現有：共用 JavaScript
│   │   └── order.js              # 新增：訂單相關 JavaScript（購物車 Session Storage 管理）
│   └── css/
│       └── site.css              # 現有：共用樣式
└── Program.cs                    # 現有：應用程式入口點（需註冊 OrderService 和清理舊訂單）

OrderLunchWeb.Tests/              # 測試專案
├── Unit/
│   ├── JsonFileStorageTests.cs  # 現有：JSON 儲存單元測試
│   ├── StoreServiceTests.cs     # 現有：餐廳服務單元測試
│   └── OrderServiceTests.cs     # 新增：訂單服務單元測試
├── Integration/
│   ├── StoreControllerTests.cs  # 現有：餐廳控制器整合測試
│   └── OrderControllerTests.cs  # 新增：訂單控制器整合測試
└── TestHelpers/
    ├── TestDataHelper.cs         # 現有：測試資料輔助類別
    └── TestEnvironment.cs        # 現有：測試環境設定

specs/002-order-food/             # 本功能文件
├── spec.md                       # 功能規格
├── plan.md                       # 本檔案
├── research.md                   # 研究文件（Phase 0 產出）
├── data-model.md                 # 資料模型（Phase 1 產出）
├── quickstart.md                 # 快速入門（Phase 1 產出）
├── contracts/                    # API 契約（Phase 1 產出）
└── tasks.md                      # 任務清單（Phase 2 產出）
```

**Structure Decision**: 本功能採用單一 Web MVC 專案結構，擴展現有的 OrderLunchWeb 專案。選擇此結構的理由：

1. **一致性**: 延續現有的 MVC 架構和資料夾組織方式
2. **簡單性**: 不新增額外專案，符合練習專案的降低複雜度目標
3. **現有模式**: 複用現有的 Service + Repository 模式（IFileStorage + JsonFileStorage）
4. **測試分離**: 單元測試和整合測試分別放置於 Unit/ 和 Integration/ 資料夾
5. **前端簡化**: 不使用前端框架，使用 Razor Views + Bootstrap + jQuery + Session Storage

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| 未實作認證/授權 | 練習專案，降低複雜度，聚焦於訂餐流程核心功能 | 認證系統需要額外的 User 實體、JWT/Cookie 設定、密碼加密、Session 管理等，增加開發時間和學習曲線，不符合本階段練習目標 |
| 使用 JSON 檔案而非資料庫 | 簡化資料存取層，避免 Entity Framework Core 和資料庫設定 | 資料庫需要 Migration、Connection String、Entity Configuration 等設定，JSON 檔案更直接且易於版本控制和檢視資料內容 |

---

## Phase 1 設計審查 (Post-Design Constitution Check)

### 設計產出清單

✅ **research.md**: 完成技術研究，所有關鍵決策均已記錄並提供理由  
✅ **data-model.md**: 完成資料模型定義，包含 3 個核心實體（Order, OrderItem, OrderStatus）和 4 個 ViewModel  
✅ **contracts/api-endpoints.md**: 完成 8 個 HTTP 端點定義（7 個新增 + 1 個更新）  
✅ **quickstart.md**: 完成快速入門指南，提供架構概覽和實作步驟  
✅ **agent context**: 已更新 GitHub Copilot 上下文檔案

### 設計審查結果

**憲章符合性**: ✅ **維持通過**

Phase 1 設計過程中未發現新的憲章違規項目：

- **資料模型設計**: 符合 C# 最佳實踐，使用 Data Annotations 驗證，包含 XML 文件註解
- **API 端點設計**: 遵循 RESTful 慣例，使用 MVC 標準路由，包含 CSRF 防護和輸入驗證
- **快照機制**: 有效解決餐廳和菜品資料變動對歷史訂單的影響，符合資料完整性要求
- **購物車管理**: Session Storage 方案簡化伺服器端狀態管理，符合效能與延展性原則
- **錯誤處理**: 定義清晰的錯誤訊息和 HTTP 狀態碼，符合使用者體驗一致性原則

**技術債務評估**: 無新增技術債務

**下一步**: 進入 Phase 2（任務分解），使用 `/speckit.tasks` 命令產生 `tasks.md`

---

## 附錄：產出文件摘要

### research.md 重點

- 8 個關鍵技術決策的理由和最佳實踐
- JSON 檔案儲存 vs 資料庫的權衡分析
- Session Storage vs Server-Side Session 的比較
- 訂單編號產生策略（時間戳記 + 毫秒）
- 金額計算精確度（decimal 型別）
- 前後端雙重驗證機制

### data-model.md 重點

- 3 個核心實體：Order（訂單）、OrderItem（訂單項目）、OrderStatus（訂單狀態）
- 2 個現有實體參考：Store（餐廳）、MenuItem（菜品）
- 4 個 ViewModel：CheckoutViewModel、OrderHistoryViewModel、CartDto、CartItemDto
- 快照機制設計：StoreName, MenuItemName, Price
- 實體關聯圖（ERD）和資料驗證規則
- JSON 檔案結構範例（UTF-8 編碼）

### contracts/api-endpoints.md 重點

- 8 個 HTTP 端點定義（OrderController 7 個 + HomeController 1 個更新）
- 完整的請求參數和回應格式
- ModelState 驗證錯誤處理
- CSRF 防護、輸入驗證、XSS 防護
- 完整訂餐流程的資料流動圖
- 購物車管理流程（JavaScript + Session Storage）

### quickstart.md 重點

- 架構概覽（分層架構圖）
- 關鍵元件職責表
- 快速開始指南（執行專案、執行測試）
- 實作步驟摘要（Phase 1-4）
- 關鍵概念解說（購物車、快照機制、訂單編號、金額計算、舊訂單清理）
- 資料流動範例（完整訂餐流程）
- 常見問題解答（FAQ）
- 未來擴展指南

---

**規劃階段總結**: 本功能的規劃階段（Phase 0 + Phase 1）已完成，所有設計文件均已產出並符合專案憲章要求。設計審查確認無新增技術債務，可進入實作階段。
