# Tasks: 店家與菜單管理系統

**Feature**: 001-store-menu-management  
**Input**: 設計文件來自 `/specs/001-store-menu-management/`  
**Prerequisites**: plan.md, spec.md, data-model.md, contracts/api-endpoints.md, research.md, quickstart.md

**測試**: 本功能規格明確要求 TDD 開發方式，所有測試任務將先於實作任務執行

**組織方式**: 任務按使用者故事分組，使每個故事可以獨立實作和測試

## 格式: `- [ ] [ID] [Story?] Description`

- **[Story]**: 所屬使用者故事 (例如: US1, US2, US3)
- 描述中包含明確的檔案路徑

---

## Phase 1: Setup (專案初始化)

**目的**: 專案結構建立和基礎設定

- [X] T001 建立測試專案 OrderLunchWeb.Tests 並新增測試專案參考到方案
- [X] T002 安裝測試相依套件到 OrderLunchWeb.Tests: xUnit, xUnit.runner.visualstudio, Microsoft.NET.Test.Sdk, Microsoft.AspNetCore.Mvc.Testing
- [X] T003 安裝 Serilog 套件到 OrderLunchWeb: Serilog.AspNetCore, Serilog.Sinks.Console, Serilog.Sinks.File
- [X] T004 建立測試專案目錄結構: OrderLunchWeb.Tests/Unit/, OrderLunchWeb.Tests/Integration/, OrderLunchWeb.Tests/TestHelpers/
- [X] T005 建立 xunit.runner.json 設定檔在 OrderLunchWeb.Tests/ (單執行緒、逾時控制)
- [X] T006 建立 TestHelpers/TestEnvironment.cs (測試環境檢查與臨時檔案管理)
- [X] T007 建立 TestHelpers/TestDataHelper.cs (測試資料產生輔助工具)

---

## Phase 2: Foundational (基礎建設 - 阻塞所有使用者故事)

**目的**: 核心基礎設施，必須在任何使用者故事實作前完成

**⚠️ 重要**: 此階段未完成前，任何使用者故事都無法開始實作

- [X] T008 建立 PhoneType 列舉在 OrderLunchWeb/Models/PhoneType.cs
- [X] T009 建立 MenuItem 實體類別在 OrderLunchWeb/Models/MenuItem.cs (包含 Data Annotations 驗證)
- [X] T010 建立 Store 實體類別在 OrderLunchWeb/Models/Store.cs (包含 Data Annotations 驗證和關聯)
- [X] T011 建立 IFileStorage 介面在 OrderLunchWeb/Data/IFileStorage.cs
- [X] T012 建立 JsonFileStorage 實作在 OrderLunchWeb/Data/JsonFileStorage.cs (包含 UTF-8 編碼、ID 管理、async/await)
- [X] T013 建立 IStoreService 介面在 OrderLunchWeb/Services/IStoreService.cs
- [X] T014 在 Program.cs 設定 Serilog (主控台和檔案輸出、UTF-8 編碼)
- [X] T015 在 Program.cs 註冊相依性注入: IFileStorage (Singleton), IStoreService (Scoped)
- [X] T016 確保 JsonFileStorage 實作中包含自動建立 Data 資料夾和 stores.json 的邏輯（若不存在則建立，符合 FR-008-1）

**Checkpoint**: 基礎建設完成 - 使用者故事實作現在可以開始

---

## Phase 3: User Story 5 - 首頁顯示與導航 (Priority: P1) 🎯 MVP 基礎

**目標**: 使用者進入系統時清楚了解這是訂餐系統，能看到當前時間和主要功能選單

**獨立測試**: 開啟系統首頁，確認訂餐系統標題、即時時間顯示、以及主選單都正確呈現

### 實作 User Story 5

- [X] T017 [US5] 修改 Views/Home/Index.cshtml 顯示「訂餐系統」標題和即時時間區塊
- [X] T018 [US5] 在 Views/Home/Index.cshtml 新增主選單區塊 (店家列表連結、訂購餐點按鈕占位)
- [X] T019 [US5] 在 Views/Home/Index.cshtml 的 Scripts section 實作即時時間更新 JavaScript (setInterval, 格式 yyyy/MM/dd HH:mm:ss)
- [X] T020 [US5] 修改 Controllers/HomeController.cs Index Action 傳遞伺服器時間到 ViewBag
- [X] T021 [US5] 修改 Views/Shared/_Layout.cshtml 導航列，新增「店家列表」選單項目

**Checkpoint**: 首頁功能完整，可獨立測試 (開啟首頁確認標題、時間、選單)

---

## Phase 4: User Story 1 - 新增店家與菜單資訊 (Priority: P1) 🎯 MVP 核心

**目標**: 系統管理員能夠新增新的餐廳店家資訊到系統中，包含店家基本資料和菜單項目

**獨立測試**: 開啟首頁，點擊「新增店家」，填寫完整表單並提交，確認資料成功儲存並顯示確認訊息

### 測試 User Story 1 (TDD - 先寫測試)

> **重要: 先撰寫這些測試，確保它們失敗，然後再實作功能**

- [X] T022 [US1] 建立 Unit/JsonFileStorageTests.cs 測試檔案並撰寫測試: 初始化、新增店家、ID 自動遞增、UTF-8 編碼
- [X] T023 [US1] 建立 Unit/StoreServiceTests.cs 測試檔案並撰寫測試: 新增店家、重複店家檢查、驗證規則
- [X] T024 [US1] 建立 Integration/StoreControllerTests.cs 測試檔案並撰寫測試: GET Create、POST Create (成功、驗證失敗、重複店家)
- [X] T024-1 [US1] 在 Integration/StoreControllerTests.cs 新增測試: 驗證防重複提交機制（快速連續提交兩次，僅第一次成功儲存，第二次被 PRG 模式阻擋或返回已存在錯誤）

### 實作 User Story 1

- [X] T025 [US1] 實作 StoreService 在 OrderLunchWeb/Services/StoreService.cs: AddStoreAsync, IsDuplicateStoreAsync 方法
- [X] T026 [US1] 建立 StoreController 在 OrderLunchWeb/Controllers/StoreController.cs: Create GET 和 POST Actions (含驗證、防重複提交、PRG 模式)
- [X] T027 [US1] 建立 Views/Store/Create.cshtml 新增店家表單 (包含所有欄位、客戶端驗證、動態菜單項目)
- [X] T028 [US1] 在 wwwroot/js/site.js 實作動態菜單項目管理 JavaScript (新增、移除、最少 1 筆、最多 20 筆限制)
- [X] T029 [US1] 在 wwwroot/js/site.js 實作防重複提交 JavaScript (按鈕禁用)
- [X] T030 [US1] 執行所有 User Story 1 測試確認通過 (dotnet test --filter US1)

**Checkpoint**: 新增店家功能完整，可獨立測試 (新增店家 → 確認儲存成功)

---

## Phase 5: User Story 2 - 瀏覽店家列表 (Priority: P1) 🎯 MVP 核心

**目標**: 使用者能夠查看所有已新增的店家清單，並透過搜尋快速找到想要的店家

**獨立測試**: 在系統中新增數筆店家資料後，開啟店家列表頁面，確認所有店家都正確顯示，並測試搜尋功能

### 測試 User Story 2 (TDD - 先寫測試)

- [ ] T031 [US2] 在 Unit/StoreServiceTests.cs 新增測試: GetAllStoresAsync, GetStoreByIdAsync, 搜尋功能
- [ ] T032 [US2] 在 Integration/StoreControllerTests.cs 新增測試: GET Index (有資料、無資料), GET Details (成功、404)

### 實作 User Story 2

- [ ] T033 [US2] 在 StoreService.cs 實作方法: GetAllStoresAsync, GetStoreByIdAsync
- [ ] T034 [US2] 在 StoreController.cs 新增 Index Action (GET, 回傳所有店家)
- [ ] T035 [US2] 在 StoreController.cs 新增 Details Action (GET, 依 ID 查詢, 404 處理)
- [ ] T036 [US2] 建立 Views/Store/Index.cshtml 店家列表頁面 (顯示所有店家、搜尋框、新增按鈕、操作連結)
- [ ] T037 [US2] 在 wwwroot/js/site.js 實作客戶端即時搜尋 JavaScript (根據店家名稱篩選)
- [ ] T038 [US2] 建立 Views/Store/Details.cshtml 店家詳情頁面 (顯示完整資訊、菜單列表、操作按鈕)
- [ ] T039 [US2] 在 wwwroot/css/site.css 新增店家列表和詳情頁面樣式
- [ ] T040 [US2] 執行所有 User Story 2 測試確認通過 (dotnet test --filter US2)

**Checkpoint**: 瀏覽店家功能完整，可獨立測試 (列表顯示 → 搜尋 → 查看詳情)

---

## Phase 6: User Story 3 - 編修店家資訊 (Priority: P2)

**目標**: 系統管理員能夠更新已存在的店家資訊，以保持資料的正確性和即時性

**獨立測試**: 從店家列表選擇一筆資料，進入編輯模式，修改部分欄位後儲存，確認資料更新成功且修改時間正確記錄

### 測試 User Story 3 (TDD - 先寫測試)

- [ ] T041 [US3] 在 Unit/StoreServiceTests.cs 新增測試: UpdateStoreAsync, 修改時間更新, 重複檢查排除自己
- [ ] T042 [US3] 在 Integration/StoreControllerTests.cs 新增測試: GET Edit (成功、404), POST Edit (成功、驗證失敗、404、重複店家)

### 實作 User Story 3

- [ ] T043 [US3] 在 StoreService.cs 實作方法: UpdateStoreAsync (含修改時間更新、重複檢查排除自己)
- [ ] T044 [US3] 在 StoreController.cs 新增 Edit Actions (GET 和 POST, 含驗證、404 處理、PRG 模式)
- [ ] T045 [US3] 建立 Views/Store/Edit.cshtml 編輯店家表單 (預填資料、動態菜單項目、客戶端驗證)
- [ ] T046 [US3] 執行所有 User Story 3 測試確認通過 (dotnet test --filter US3)

**Checkpoint**: 編修店家功能完整，可獨立測試 (編輯店家 → 修改資料 → 確認更新成功)

---

## Phase 7: User Story 4 - 刪除店家資訊 (Priority: P3)

**目標**: 系統管理員能夠刪除不再營業或錯誤建立的店家資料，以維持系統資料的整潔性

**獨立測試**: 從店家列表選擇一筆測試資料，執行刪除操作，確認資料從列表中移除且 JSON 檔案中的對應記錄已刪除

### 測試 User Story 4 (TDD - 先寫測試)

- [ ] T047 [US4] 在 Unit/StoreServiceTests.cs 新增測試: DeleteStoreAsync (成功、不存在的 ID)
- [ ] T048 [US4] 在 Integration/StoreControllerTests.cs 新增測試: GET Delete (成功、404), POST Delete (成功、404)

### 實作 User Story 4

- [ ] T049 [US4] 在 StoreService.cs 實作方法: DeleteStoreAsync
- [ ] T050 [US4] 在 StoreController.cs 新增 Delete Actions (GET 和 POST, 含確認頁面、404 處理、PRG 模式)
- [ ] T051 [US4] 建立 Views/Store/Delete.cshtml 刪除確認頁面 (顯示店家資訊、警告訊息、確認和取消按鈕)
- [ ] T052 [US4] 執行所有 User Story 4 測試確認通過 (dotnet test --filter US4)

**Checkpoint**: 刪除店家功能完整，可獨立測試 (刪除店家 → 確認 → 資料移除)

---

## Phase 8: Polish & Cross-Cutting Concerns (最佳化與跨功能改善)

**目的**: 影響多個使用者故事的改善項目

- [ ] T053 在所有 Controller Actions 新增業務操作日誌 (使用 ILogger): 記錄 CRUD 操作的開始/成功/失敗、使用者輸入驗證失敗、業務規則違反（如重複店家）、檔案 I/O 錯誤。注意: Serilog 基礎設定已在 T014 完成，此任務僅新增業務層日誌呼叫。
- [ ] T054 在所有 Views 新增 TempData 成功/錯誤訊息顯示區塊 (Bootstrap alerts)功後顯示「成功訊息」(success alert)，操作失敗顯示「錯誤訊息」(danger alert)
- [ ] T055 改善 wwwroot/css/site.css 整體樣式 (回應式設計、間距、顏色一致性)
- [ ] T056 重構重複的驗證邏輯，確保 DRY 原則
- [ ] T057 執行完整測試套件確認所有測試通過 (dotnet test)
- [ ] T058 檢查程式碼格式符合 .editorconfig 規範 (格式化所有檔案)
- [ ] T059 新增 XML 文件註解到所有公開類別、介面、方法
- [ ] T060 依照 quickstart.md 執行完整使用者流程驗證 (手動測試所有場景)
- [ ] T061 產生測試覆蓋率報告並確認關鍵路徑 > 80% 覆蓋率

---

## Dependencies & Execution Order (相依性與執行順序)

### 階段相依性

- **Setup (Phase 1)**: 無相依性 - 可立即開始
- **Foundational (Phase 2)**: 相依於 Setup 完成 - **阻塞所有使用者故事**
- **User Stories (Phase 3-7)**: 全部相依於 Foundational (Phase 2) 完成
  - 使用者故事可以平行進行 (若有足夠人力)
  - 或依優先順序循序執行 (P1 → P2 → P3)
- **Polish (Phase 8)**: 相依於所有需要的使用者故事完成

### 使用者故事相依性

- **User Story 5 (P1) - 首頁**: Foundational 完成後可開始 - 無其他故事相依性
- **User Story 1 (P1) - 新增店家**: Foundational 完成後可開始 - 無其他故事相依性
- **User Story 2 (P1) - 瀏覽列表**: Foundational 完成後可開始 - 建議在 US1 之後 (需要測試資料)
- **User Story 3 (P2) - 編修店家**: Foundational 完成後可開始 - 建議在 US1 和 US2 之後 (需要已存在的資料)
- **User Story 4 (P3) - 刪除店家**: Foundational 完成後可開始 - 建議在 US1 和 US2 之後 (需要已存在的資料)

### 每個使用者故事內部

- **測試必須先撰寫且失敗** (TDD 紅燈階段)
- Models → Services → Controllers → Views
- 核心實作 → 整合 → 測試通過 (TDD 綠燈階段)
- 故事完成後才進行下一個優先級

### 執行順序建議

- Phase 1 和 Phase 2 依序完成
- Phase 2 完成後，依優先順序循序執行使用者故事 (P1 → P2 → P3)
- 每個使用者故事內的任務依序執行
- 若有多位開發者，不同使用者故事可由不同團隊成員開發

---

## Implementation Strategy (實作策略)

### MVP 優先 (僅 User Story 5 + 1 + 2 的 P1 功能)

1. 完成 Phase 1: Setup
2. 完成 Phase 2: Foundational (重要 - 阻塞所有故事)
3. 完成 Phase 3: User Story 5 (首頁)
4. 完成 Phase 4: User Story 1 (新增店家)
5. 完成 Phase 5: User Story 2 (瀏覽列表)
6. **停止並驗證**: 獨立測試 US5 + US1 + US2
7. 準備就緒時部署/展示

### 增量交付 (建議順序)

1. 完成 Setup + Foundational → 基礎就緒
2. 新增 User Story 5 → 獨立測試 → 部署/展示 (首頁)
3. 新增 User Story 1 → 獨立測試 → 部署/展示 (新增功能)
4. 新增 User Story 2 → 獨立測試 → 部署/展示 (完整 CRUD 的 R)
5. 新增 User Story 3 → 獨立測試 → 部署/展示 (完整 CRUD 的 U)
6. 新增 User Story 4 → 獨立測試 → 部署/展示 (完整 CRUD 的 D)
7. Polish → 最佳化
8. 每個故事都增加價值且不破壞先前的故事

### 平行團隊策略 (若有多位開發者)

1. 團隊一起完成 Setup + Foundational
2. Foundational 完成後:
   - Developer A: User Story 5 (首頁)
   - Developer B: User Story 1 (新增店家)
   - Developer C: User Story 2 (瀏覽列表)
   - Developer D: User Story 3 (編修店家)
   - Developer E: User Story 4 (刪除店家)
3. 各故事獨立完成並整合

---

## Task Summary (任務摘要)

**總任務數**: 62 個任務 (新增 T024-1 防重複提交測試)

**每個使用者故事的任務數**:

- Phase 1 (Setup): 7 個任務
- Phase 2 (Foundational): 9 個任務 ⚠️ **阻塞所有故事**
- Phase 3 (User Story 5 - 首頁): 5 個任務
- Phase 4 (User Story 1 - 新增店家): 10 個任務 (含 4 個測試任務，包括防重複提交測試)
- Phase 5 (User Story 2 - 瀏覽列表): 10 個任務 (含 2 個測試任務)
- Phase 6 (User Story 3 - 編修店家): 6 個任務 (含 2 個測試任務)
- Phase 7 (User Story 4 - 刪除店家): 6 個任務 (含 2 個測試任務)
- Phase 8 (Polish): 9 個任務



**建議 MVP 範圍**:

- Phase 1 + 2 (基礎建設)
- Phase 3 (首頁)
- Phase 4 (新增店家)
- Phase 5 (瀏覽列表)
- **= 共 41 個任務，涵蓋完整的新增和查看功能**

**獨立測試標準**:

- User Story 5: 開啟首頁 → 確認標題、時間、選單
- User Story 1: 新增店家 → 填寫表單 → 確認儲存成功
- User Story 2: 瀏覽列表 → 搜尋 → 查看詳情
- User Story 3: 編輯店家 → 修改資料 → 確認更新成功
- User Story 4: 刪除店家 → 確認 → 資料移除

**格式驗證**: ✅ 所有任務遵循檢查清單格式 (checkbox, ID, 標籤, 檔案路徑)

---

## Notes (注意事項)

- **[Story] 標籤** = 將任務對應到特定使用者故事，便於追蹤
- **每個使用者故事應該可以獨立完成和測試**
- **TDD 流程**: 撰寫測試 → 確認失敗 (紅燈) → 實作功能 → 測試通過 (綠燈) → 重構
- **每個任務或邏輯群組後提交程式碼**
- **在任何 Checkpoint 停下來獨立驗證故事**
- **避免**: 模糊的任務、相同檔案衝突、破壞故事獨立性的跨故事相依性
- **UTF-8 編碼**: 所有 JSON 檔案和日誌檔案使用 UTF-8 編碼確保中文正確顯示
- **測試逾時**: 注意測試執行逾時問題，參考 quickstart.md 的測試逾時與容錯章節
