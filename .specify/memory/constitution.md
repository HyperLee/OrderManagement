<!--
Sync Impact Report
==================
Version Change: N/A → 1.0.0
Change Type: MAJOR (Initial Constitution)
Modified Principles: All principles newly created
Added Sections: All sections newly created (including Documentation Language Requirements)
Removed Sections: None

Key Features:
- Six core principles: Code Quality, Test-First, UX Consistency, Performance, Observability, Security
- Documentation MUST be in Traditional Chinese (zh-TW)
- Technical standards: ASP.NET Core 8.0 + C# 13
- Quality gates and governance rules

Templates Status:
✅ plan-template.md - Aligned with constitution principles (Test-First, Code Quality gates)
⚠️  plan-template.md - REQUIRES localization to zh-TW for user-facing sections
✅ spec-template.md - User story prioritization matches UX Consistency principle
⚠️  spec-template.md - REQUIRES localization to zh-TW for user-facing sections
✅ tasks-template.md - Task organization supports Test-First and independent testing
⚠️  tasks-template.md - REQUIRES localization to zh-TW for user-facing sections
⚠️  No command files found in .specify/templates/commands/ - Manual verification needed if added later

Follow-up TODOs:
1. Localize all template files (.specify/templates/*.md) to Traditional Chinese (zh-TW)
2. Ensure all generated specs, plans, and documentation use zh-TW
3. Update .github/instructions/csharp.instructions.md to include zh-TW documentation requirements
4. Create sample spec/plan documents in zh-TW to serve as references
-->

# OrderManagement 訂餐管理系統憲章

## 核心原則

### I. 程式碼品質至上 (NON-NEGOTIABLE)

所有程式碼必須符合以下品質標準:

- **可維護性**: 程式碼必須清晰、有註解,設計決策必須文件化
- **C# 最佳實踐**: 使用 C# 13 最新功能、檔案範圍命名空間、模式匹配、null 安全性 (`is null`/`is not null`)
- **命名規範**: PascalCase 用於公開成員和方法,camelCase 用於私有欄位,介面前綴 "I"
- **XML 文件註解**: 所有公開 API 必須包含 XML 文件註解,包含 `<example>` 和 `<code>` 區段
- **錯誤處理**: 必須處理邊界情況並提供清晰的例外處理
- **程式碼格式化**: 遵循 `.editorconfig` 定義的格式規範

**理由**: 高品質程式碼減少技術債務,提升團隊生產力,降低維護成本,確保長期專案健康度。

### II. 測試優先開發 (NON-NEGOTIABLE)

嚴格執行測試驅動開發 (TDD) 流程:

- **紅-綠-重構週期**: 必須先寫測試 → 使用者批准 → 測試失敗 → 實作功能 → 測試通過 → 重構
- **關鍵路徑測試**: 所有關鍵業務邏輯必須有單元測試覆蓋
- **整合測試**: API 端點、認證授權邏輯、資料存取層必須有整合測試
- **測試命名**: 遵循現有檔案的命名風格和大小寫規範,不使用 "Arrange"、"Act"、"Assert" 註解
- **相依性模擬**: 有效使用 Mock 測試隔離的單元
- **獨立可測試**: 每個使用者故事必須能獨立測試,作為可交付的 MVP 增量

**理由**: 測試優先確保需求正確理解、減少缺陷、提供重構安全網、作為活文件說明系統行為。

### III. 使用者體驗一致性

提供一致且優質的使用者體驗:

- **UI/UX 標準化**: 使用統一的設計語言、元件庫 (Bootstrap)、樣式指南
- **回應式設計**: 所有介面必須在不同裝置和螢幕尺寸下正常運作
- **錯誤訊息**: 提供清晰、可操作的錯誤訊息 (使用 RFC 7807 Problem Details 格式)
- **驗證回饋**: 即時驗證回饋,明確指出問題和修正方法
- **無障礙設計**: 遵循 WCAG 2.1 標準,確保可及性
- **使用者故事優先級**: 按業務價值排序 (P1, P2, P3...),每個故事獨立可交付

**理由**: 一致的 UX 降低學習成本、提升使用者滿意度、減少支援請求,獨立可測試的故事確保持續交付價值。

### IV. 效能與延展性

系統必須符合效能標準並能擴展:

- **回應時間**: API 端點 p95 回應時間 < 200ms (正常負載下)
- **資料庫查詢最佳化**: 避免 N+1 查詢問題、使用適當索引、實施查詢計畫分析
- **記憶體管理**: 單一請求記憶體使用 < 100MB,適當使用資源清理和 `IDisposable`
- **非同步程式設計**: I/O 密集操作必須使用 async/await 模式
- **快取策略**: 對靜態或低變化率資料實施適當快取機制
- **效能監控**: 使用 Application Insights 或類似工具追蹤效能指標、錯誤率、使用模式

**理由**: 良好效能直接影響使用者體驗和系統可用性,早期建立效能意識避免後期昂貴的效能調校。

### V. 可觀察性與監控

系統必須提供完整的可觀察性:

- **結構化日誌**: 使用 Serilog 或類似提供者實施結構化日誌
- **日誌層級**: 正確使用日誌層級 (Trace/Debug/Information/Warning/Error/Critical)
- **遙測收集**: 整合 Application Insights 收集自訂遙測和關聯 ID 進行請求追蹤
- **關鍵事件記錄**: 所有安全事件、業務關鍵操作必須記錄
- **監控儀表板**: 建立監控面板追蹤 API 效能、錯誤、使用模式
- **告警機制**: 對關鍵錯誤和效能降級設定告警

**理由**: 可觀察性是生產環境問題診斷的基礎,主動監控能在使用者受影響前發現問題。

### VI. 安全優先

安全性必須內建於每個功能:

- **認證機制**: 使用 JWT Bearer Tokens 或 OAuth 2.0/OpenID Connect
- **授權控制**: 實施基於角色 (Role-Based) 和策略 (Policy-Based) 的授權
- **輸入驗證**: 所有使用者輸入必須驗證 (使用 Data Annotations 或 FluentValidation)
- **SQL 注入防護**: 使用 Entity Framework Core 參數化查詢,避免字串拼接 SQL
- **敏感資料保護**: 密碼、金鑰、連線字串等必須使用 Secret Manager 或 Azure Key Vault
- **HTTPS Only**: 生產環境強制使用 HTTPS,啟用 HSTS

**理由**: 安全漏洞造成的損害遠超過預防成本,內建安全性比事後補強更有效且成本更低。

## 技術標準

### 技術堆疊

- **Framework**: ASP.NET Core 8.0 或更新版本
- **語言**: C# 13 (啟用最新功能)
- **資料庫**: Entity Framework Core (支援 SQL Server、SQLite、In-Memory)
- **前端**: Razor Pages/MVC + Bootstrap 5 + jQuery
- **API 文件**: Swagger/OpenAPI
- **日誌**: Serilog
- **測試**: xUnit/NUnit + Moq (單元測試) + WebApplicationFactory (整合測試)

### 專案結構

- **關注點分離**: Models、Services、Controllers/Pages、Data Access 明確分層
- **功能資料夾**: 大型功能使用 Feature Folders 或領域驅動設計原則組織
- **設定管理**: 使用 `appsettings.json` + 環境特定設定檔 (Development、Production)
- **相依性注入**: 使用內建 DI 容器管理服務生命週期

### 資料存取模式

- **Repository Pattern**: 視情況實施 Repository 模式封裝資料存取
- **Migration 管理**: 使用 EF Core Migrations 管理資料庫架構變更
- **資料種子**: 為開發和測試環境提供資料播種機制

## 開發工作流程

### 文件語言要求

**所有使用者面向文件必須使用繁體中文 (zh-TW)**:

- 功能規格 (`spec.md`)
- 實作計畫 (`plan.md`)
- 研究文件 (`research.md`)
- 資料模型 (`data-model.md`)
- 快速入門指南 (`quickstart.md`)
- 任務清單 (`tasks.md`)
- API 文件和註解

**程式碼內部可使用英文**:

- 變數名稱、函式名稱、類別名稱
- 程式碼註解可使用英文或中文
- Git commit 訊息建議使用英文 (選擇性)

### 功能開發流程

1. **規格定義**: 在 `/specs/[###-feature-name]/spec.md` 以繁體中文定義使用者故事和驗收標準
2. **計畫制定**: 使用 `/speckit.plan` 產生 `plan.md`、`research.md`、`data-model.md` (繁體中文)
3. **憲章檢查**: 驗證設計符合本憲章所有原則
4. **測試先行**: 撰寫失敗測試 (contract tests → integration tests)
5. **實作**: 按使用者故事優先級實作功能
6. **測試通過**: 確保所有測試通過
7. **程式碼審查**: 通過審查確認符合品質標準和憲章原則

### 品質閘門

每個 Pull Request 必須通過:

- ✅ 所有自動化測試通過 (單元 + 整合)
- ✅ 程式碼覆蓋率 > 80% (關鍵業務邏輯)
- ✅ 無編譯警告或 linter 錯誤
- ✅ XML 文件註解完整 (公開 API)
- ✅ 效能基準測試通過 (如適用)
- ✅ 安全性掃描無高危漏洞
- ✅ 至少一位團隊成員審查通過

### API 版本控制與文件

- **版本策略**: 使用 URL 版本控制 (`/api/v1/...`) 或 Header 版本控制
- **語意化版本**: MAJOR.MINOR.PATCH 格式
- **中斷性變更**: MAJOR 版本變更必須提供遷移指南和棄用通知期
- **API 文件**: 使用 Swagger/OpenAPI 自動產生,包含端點、參數、回應、認證說明

## 治理規則

### 憲章優先級

本憲章優先於所有其他開發實踐和指南。當發生衝突時:

1. 本憲章原則為最高優先
2. `.github/instructions/csharp.instructions.md` 為實施細節指南
3. 團隊慣例和個人偏好為最低優先

### 修訂程序

修訂本憲章需要:

1. **提案文件**: 說明修訂理由、影響範圍、替代方案
2. **團隊審查**: 至少 2/3 團隊成員同意
3. **遷移計畫**: 對現有程式碼的影響評估和遷移時程
4. **版本更新**: 按語意化版本規則更新版本號
5. **相依文件更新**: 同步更新所有相關範本和指南檔案

### 版本控制規則

- **MAJOR**: 移除或重新定義核心原則、不相容的治理變更
- **MINOR**: 新增原則、區段或實質擴充現有指導
- **PATCH**: 釐清說明、文字修正、非語意的細化

### 合規審查

- 所有 Pull Request 必須驗證憲章合規性
- 每季度進行憲章遵循審計
- 複雜度增加必須有明確的業務價值理由

### 執行指引

開發期間參考 `.github/instructions/csharp.instructions.md` 以獲取具體實施指導,但該檔案必須與本憲章原則保持一致。

**版本**: 1.0.0 | **批准日期**: 2025-11-22 | **最後修訂**: 2025-11-22
