# Implementation Plan: 店家與菜單管理系統

**Branch**: `001-store-menu-management` | **Date**: 2025-11-22 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/001-store-menu-management/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/commands/plan.md` for the execution workflow.

## Summary

實作訂餐系統的店家與菜單管理功能，提供新增、列表、編修和刪除店家資訊的完整 CRUD 操作。系統使用 ASP.NET Core 8.0 MVC 架構，採用 JSON 檔案作為資料儲存方式，不使用前端框架以降低複雜度。功能包含完整的表單驗證、搜尋篩選、以及即時時間顯示，確保資料完整性和使用者體驗。

## Technical Context

**Language/Version**: C# 13 / ASP.NET Core 8.0

**Primary Dependencies**:

- ASP.NET Core MVC 8.0 (內建)
- System.Text.Json (JSON 序列化/反序列化，UTF-8 編碼支援)
- Serilog.AspNetCore (結構化日誌)
- Serilog.Sinks.Console (主控台日誌輸出)
- Serilog.Sinks.File (檔案日誌輸出)
- Bootstrap 5 (已在專案中，用於 UI 樣式)
- jQuery (已在專案中，用於客戶端驗證)

**Storage**: JSON 檔案 (Data/stores.json)，使用 UTF-8 編碼防止中文亂碼

**Testing**: xUnit (單元測試) + WebApplicationFactory (整合測試)

**Target Platform**: 跨平台 Web 應用程式 (Windows/macOS/Linux)

**Project Type**: Web MVC 應用程式

**Performance Goals**:

- 頁面載入時間 < 2 秒 (100 筆店家資料)
- 搜尋回應時間 < 0.5 秒
- 即時時間更新每秒一次
- API 端點回應時間 < 200ms (p95)

**Constraints**:

- 不使用前端框架 (React/Vue/Angular) - 使用原生 Razor Views + jQuery
- 不使用資料庫 - 使用 JSON 檔案儲存
- 單一請求記憶體使用 < 100MB
- JSON 檔案必須使用 UTF-8 編碼 (避免中文亂碼)

**Scale/Scope**:

- 預期最多 100 筆店家資料
- 每個店家最多 20 筆菜單項目
- 單一使用者使用 (練習用專案，無併發控制需求)
- 約 10-15 個頁面/視圖

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### I. 程式碼品質至上

- ✅ **可維護性**: 將實作清晰的分層架構 (Models, Services, Controllers, Views)
- ✅ **C# 最佳實踐**: 使用 C# 13、檔案範圍命名空間、模式匹配、null 安全性
- ✅ **命名規範**: 遵循 PascalCase (公開成員)、camelCase (私有欄位)
- ✅ **XML 文件註解**: 所有公開 API 和服務方法將包含 XML 註解
- ✅ **錯誤處理**: 實作完整的驗證和例外處理機制
- ✅ **程式碼格式化**: 遵循專案的 `.editorconfig` 規範

### II. 測試優先開發

- ✅ **紅-綠-重構週期**: 先撰寫測試 → 實作功能 → 測試通過 → 重構
- ✅ **關鍵路徑測試**: 為所有 CRUD 操作和驗證邏輯撰寫單元測試
- ✅ **整合測試**: 為 Controller Actions 和 JSON 檔案存取撰寫整合測試
- ✅ **測試命名**: 遵循專案現有測試命名風格
- ✅ **獨立可測試**: 每個使用者故事按優先級 (P1→P2→P3) 獨立實作和測試

### III. 使用者體驗一致性

- ✅ **UI/UX 標準化**: 使用 Bootstrap 5 提供一致的設計語言
- ✅ **回應式設計**: 確保在不同裝置和螢幕尺寸下正常運作
- ✅ **錯誤訊息**: 提供清晰、可操作的中文錯誤訊息
- ✅ **驗證回饋**: 客戶端和伺服器端雙重驗證，即時回饋
- ✅ **使用者故事優先級**: 已按 P1 (新增、列表、首頁) → P2 (編修) → P3 (刪除) 排序

### IV. 效能與延展性

- ✅ **回應時間**: 目標 API 回應時間 < 200ms
- ✅ **記憶體管理**: 單一請求 < 100MB，適當使用 `IDisposable` 和資源清理
- ✅ **非同步程式設計**: JSON 檔案讀寫使用 async/await
- ⚠️ **快取策略**: JSON 檔案讀取無快取 (練習用專案，資料量小，不實作快取以降低複雜度)
- ✅ **效能監控**: 測試中驗證頁面載入 < 2 秒、搜尋 < 0.5 秒

### V. 可觀察性與監控

- ✅ **結構化日誌**: 使用 Serilog 實作結構化日誌，透過 ILogger 介面注入
- ✅ **日誌層級**: 正確使用 Information (正常操作)、Warning (驗證失敗)、Error (檔案存取錯誤)
- ✅ **日誌輸出**: 主控台 + 檔案 (Logs/log-.txt，按日期滾動)
- ✅ **關鍵事件記錄**: 記錄 CRUD 操作、驗證失敗、檔案 I/O 錯誤
- ⚠️ **監控儀表板**: 不實作 (練習用專案，使用主控台日誌即可)

### VI. 安全優先

- ⚠️ **認證機制**: 不實作 (此功能為管理後台，不涉及多使用者或敏感操作，符合風險導向安全原則；未來訂購功能將實作完整認證)
- ⚠️ **授權控制**: 不實作 (單一管理員操作，無角色區分需求；未來多角色功能將實作授權控制)
- ✅ **輸入驗證**: 使用 Data Annotations 和自訂驗證邏輯，防止惡意輸入
- ✅ **防注入攻擊**: 不使用資料庫，無 SQL 注入風險；使用 System.Text.Json 防止 JSON 注入；所有使用者輸入經過驗證和編碼
- ⚠️ **敏感資料保護**: 無敏感資料 (此功能僅儲存公開的店家資訊，無密碼、金鑰或個人識別資訊)
- ⚠️ **HTTPS Only**: 開發環境支援 HTTP 和 HTTPS，生產環境強制 HTTPS (標準 ASP.NET Core 設定)

**安全性簡化理由**: 本功能為練習用的店家管理後台，符合憲章「風險導向安全原則」：

- **無敏感資料**: 僅儲存公開的店家和菜單資訊，無使用者個人資料或金融資訊
- **單一管理員**: 預期單一管理員本地操作，無多使用者併發或權限衝突
- **學習目標**: 重點在 CRUD 操作和資料驗證，認證機制將在未來訂購功能中實作
- **已實作關鍵安全控制**: 輸入驗證 (防 XSS)、JSON 注入防護、HTTPS 支援

**評估結果**: ✅ **通過** - 所有核心原則符合憲章要求。安全性簡化基於風險評估且有明確理由，符合憲章「安全性必須內建於每個功能」的精神（已實作必要的輸入驗證和注入防護）。

## Project Structure

### Documentation (this feature)

```text
specs/001-store-menu-management/
├── plan.md              # 本檔案 (實作計畫)
├── research.md          # 技術研究報告
├── data-model.md        # 資料模型設計
├── quickstart.md        # 快速入門指南
├── contracts/           # API 合約
│   └── api-endpoints.md # HTTP 端點規格
└── tasks.md             # 任務清單 (待產生，使用 /speckit.tasks 命令)
```

### Source Code (repository root)

```text
OrderLunchWeb/                    # ASP.NET Core MVC 專案
├── Controllers/
│   ├── HomeController.cs         # 首頁 Controller (已存在)
│   └── StoreController.cs        # 店家管理 Controller (待實作)
├── Models/
│   ├── ErrorViewModel.cs         # 錯誤視圖模型 (已存在)
│   ├── Store.cs                  # 店家實體 (待實作)
│   ├── MenuItem.cs               # 菜單項目實體 (待實作)
│   └── PhoneType.cs              # 電話類型列舉 (待實作)
├── Services/
│   ├── IStoreService.cs          # 店家服務介面 (待實作)
│   └── StoreService.cs           # 店家服務實作 (待實作)
├── Data/
│   ├── IFileStorage.cs           # 檔案儲存介面 (待實作)
│   ├── JsonFileStorage.cs        # JSON 檔案儲存實作 (待實作)
│   └── stores.json               # 資料檔案 (自動建立)
├── Views/
│   ├── Home/
│   │   └── Index.cshtml          # 首頁視圖 (需修改)
│   ├── Store/                    # 店家視圖 (待建立)
│   │   ├── Index.cshtml          # 店家列表
│   │   ├── Create.cshtml         # 新增店家
│   │   ├── Details.cshtml        # 店家詳情
│   │   ├── Edit.cshtml           # 編輯店家
│   │   └── Delete.cshtml         # 刪除確認
│   └── Shared/
│       └── _Layout.cshtml        # 共用版面配置 (需修改)
├── wwwroot/
│   ├── css/
│   │   └── site.css              # 自訂樣式 (需修改)
│   ├── js/
│   │   └── site.js               # 自訂 JavaScript (需修改)
│   └── lib/                      # 第三方函式庫 (已存在)
└── Program.cs                    # 應用程式進入點 (需修改，註冊服務)

OrderLunchWeb.Tests/              # 測試專案 (待建立)
├── Unit/
│   ├── StoreServiceTests.cs      # StoreService 單元測試
│   └── JsonFileStorageTests.cs   # JsonFileStorage 單元測試
├── Integration/
│   └── StoreControllerTests.cs   # StoreController 整合測試
├── TestHelpers/
│   ├── TestEnvironment.cs        # 測試環境檢查與設定
│   └── TestDataHelper.cs         # 測試資料輔助工具
└── xunit.runner.json             # xUnit 執行設定 (逾時控制)
```

**Structure Decision**: 採用標準 ASP.NET Core MVC 單一專案結構，搭配獨立測試專案。選擇此結構的理由:

- **簡單明瞭**: 適合練習用專案，無需複雜的多層架構
- **關注點分離**: Controllers、Services、Data 分層清晰
- **可測試性**: Service 和 FileStorage 使用介面，方便單元測試
- **擴展性**: 未來可輕鬆遷移至資料庫或新增其他功能模組

## Complexity Tracking

> **無複雜度違規** - 本專案完全符合憲章要求，無需額外複雜度理由。

所有標記為 ⚠️ 的項目 (如未實作快取、認證、監控儀表板) 均為練習用專案的合理簡化，符合學習目標，不構成憲章違規。
