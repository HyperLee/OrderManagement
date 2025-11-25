# Tasks: 訂餐功能系統

**Input**: Design documents from `/specs/002-order-food/`
**Prerequisites**: plan.md ✅, spec.md ✅, research.md ✅, data-model.md ✅, contracts/api-endpoints.md ✅, quickstart.md ✅

**Tests**: 規格中指定了測試要求，將為關鍵業務邏輯撰寫單元測試和整合測試。

**Organization**: 任務按使用者故事分組，以支援各故事的獨立實作和測試。

## Format: `[ID] [P?] [Story] Description`

- **[P]**: 可平行執行（不同檔案，無相依性）
- **[Story]**: 此任務所屬的使用者故事（如 US1, US2, US3）
- 描述中包含確切的檔案路徑

## Path Conventions

- **主專案**: `OrderLunchWeb/`
- **測試專案**: `OrderLunchWeb.Tests/`
- **規格文件**: `specs/002-order-food/`

---

## Phase 1: Setup (共用基礎設施)

**Purpose**: 專案初始化和基本結構設定

- [ ] T001 建立訂單相關模型檔案結構：建立 `OrderLunchWeb/Models/Order.cs`, `OrderLunchWeb/Models/OrderItem.cs`, `OrderLunchWeb/Models/OrderStatus.cs`
- [ ] T002 建立訂單服務介面和實作檔案結構：建立 `OrderLunchWeb/Services/IOrderService.cs`, `OrderLunchWeb/Services/OrderService.cs`
- [ ] T003 建立訂單視圖資料夾結構：建立 `OrderLunchWeb/Views/Order/` 資料夾
- [ ] T004 [P] 建立訂單資料儲存檔案：建立 `OrderLunchWeb/Data/orders.json`（空陣列初始內容）
- [ ] T005 [P] 建立前端 JavaScript 檔案：建立 `OrderLunchWeb/wwwroot/js/order.js`

---

## Phase 2: Foundational (阻塞性前置條件)

**Purpose**: 所有使用者故事開始前必須完成的核心基礎設施

**⚠️ 重要**: 本階段完成前，任何使用者故事都無法開始

- [ ] T006 實作 OrderStatus 列舉模型於 `OrderLunchWeb/Models/OrderStatus.cs`
- [ ] T007 [P] 實作 OrderItem 模型（含快照欄位和驗證）於 `OrderLunchWeb/Models/OrderItem.cs`
- [ ] T008 [P] 實作 Order 模型（含驗證屬性和計算屬性）於 `OrderLunchWeb/Models/Order.cs`
- [ ] T009 [P] 實作 CheckoutViewModel 視圖模型於 `OrderLunchWeb/Models/CheckoutViewModel.cs`
- [ ] T010 [P] 實作 OrderHistoryViewModel 視圖模型於 `OrderLunchWeb/Models/OrderHistoryViewModel.cs`
- [ ] T011 [P] 實作 CartDto 和 CartItemDto 資料傳輸物件於 `OrderLunchWeb/Models/CartDto.cs`
- [ ] T012 定義 IOrderService 介面（含所有方法簽名）於 `OrderLunchWeb/Services/IOrderService.cs`
- [ ] T013 實作 OrderService 服務（訂單建立、查詢、清理邏輯）於 `OrderLunchWeb/Services/OrderService.cs`
- [ ] T014 更新 Program.cs 註冊 IOrderService 和 OrderService 至 DI 容器於 `OrderLunchWeb/Program.cs`
- [ ] T015 實作應用程式啟動時的舊訂單清理邏輯於 `OrderLunchWeb/Program.cs`
- [ ] T016 實作前端購物車儲存基礎函式（CartStorage 物件）於 `OrderLunchWeb/wwwroot/js/order.js`

**Checkpoint**: 基礎設施完成 - 使用者故事實作可平行開始

---

## Phase 3: User Story 1 - 訂購餐點流程 (Priority: P1) 🎯 MVP

**Goal**: 使用者可以從首頁進入訂餐系統，瀏覽餐廳列表，選擇餐廳後查看菜單，將菜品加入訂單，完成結帳並取得訂單編號。

**Independent Test**: 可透過完整的使用者操作路徑測試：點擊首頁「訂購餐點」按鈕 → 選擇餐廳 → 瀏覽菜單並加入訂單 → 填寫個人資訊 → 確認訂單 → 取得訂單編號。

### Tests for User Story 1

- [ ] T017 [P] [US1] 建立 OrderService 單元測試於 `OrderLunchWeb.Tests/Unit/OrderServiceTests.cs`
- [ ] T018 [P] [US1] 建立 OrderController 整合測試於 `OrderLunchWeb.Tests/Integration/OrderControllerTests.cs`

### Implementation for User Story 1

- [ ] T019 [US1] 建立 OrderController 控制器骨架（含建構子和 DI）於 `OrderLunchWeb/Controllers/OrderController.cs`
- [ ] T020 [US1] 實作 SelectRestaurant Action（餐廳列表頁面）於 `OrderLunchWeb/Controllers/OrderController.cs`
- [ ] T021 [US1] 實作 Menu Action（菜單頁面）於 `OrderLunchWeb/Controllers/OrderController.cs`
- [ ] T022 [US1] 實作 Checkout Action（結帳頁面 GET）於 `OrderLunchWeb/Controllers/OrderController.cs`
- [ ] T023 [US1] 實作 Submit Action（提交訂單 POST）於 `OrderLunchWeb/Controllers/OrderController.cs`
- [ ] T024 [US1] 實作 Confirmation Action（訂單確認頁面）於 `OrderLunchWeb/Controllers/OrderController.cs`
- [ ] T025 [P] [US1] 建立 SelectRestaurant.cshtml 視圖（餐廳列表）於 `OrderLunchWeb/Views/Order/SelectRestaurant.cshtml`
- [ ] T026 [P] [US1] 建立 Menu.cshtml 視圖（菜單頁面含訂單摘要區塊）於 `OrderLunchWeb/Views/Order/Menu.cshtml`
- [ ] T027 [P] [US1] 建立 Checkout.cshtml 視圖（結帳頁面含表單驗證）於 `OrderLunchWeb/Views/Order/Checkout.cshtml`
- [ ] T028 [P] [US1] 建立 Confirmation.cshtml 視圖（訂單確認頁面）於 `OrderLunchWeb/Views/Order/Confirmation.cshtml`
- [ ] T029 [US1] 更新首頁新增「訂購餐點」按鈕於 `OrderLunchWeb/Views/Home/Index.cshtml`
- [ ] T030 [US1] 實作前端「加入訂單」功能函式於 `OrderLunchWeb/wwwroot/js/order.js`
- [ ] T031 [US1] 實作前端「前往結帳」功能函式於 `OrderLunchWeb/wwwroot/js/order.js`

**Checkpoint**: User Story 1 完成後，應可獨立測試完整的訂餐流程

---

## Phase 4: User Story 2 - 訂單摘要即時更新 (Priority: P1)

**Goal**: 使用者在瀏覽菜單時，可以在固定區塊查看當前訂單摘要，包含已選菜品清單、數量、小計和訂單總金額，並即時更新。

**Independent Test**: 在菜單頁面加入多個菜品，驗證訂單摘要區塊即時顯示所有已選菜品、數量和總金額。

### Implementation for User Story 2

- [ ] T032 [US2] 實作前端訂單摘要 UI 更新函式（updateOrderSummary）於 `OrderLunchWeb/wwwroot/js/order.js`
- [ ] T033 [US2] 實作前端訂單總金額格式化顯示（NT$ 格式）於 `OrderLunchWeb/wwwroot/js/order.js`
- [ ] T034 [US2] 實作前端「前往結帳」按鈕狀態管理（啟用/停用）於 `OrderLunchWeb/wwwroot/js/order.js`
- [ ] T035 [US2] 更新 Menu.cshtml 訂單摘要區塊樣式和互動於 `OrderLunchWeb/Views/Order/Menu.cshtml`

**Checkpoint**: User Story 2 完成後，訂單摘要應即時反映購物車狀態

---

## Phase 5: User Story 3 - 數量選擇與驗證 (Priority: P1)

**Goal**: 使用者可以透過 +/- 按鈕或直接輸入數字來調整菜品數量，系統會驗證數量必須為正整數（最小值為1）。

**Independent Test**: 測試各種數量輸入情境（點擊按鈕、直接輸入數字、輸入無效值），驗證系統是否正確處理並顯示錯誤訊息。

### Implementation for User Story 3

- [ ] T036 [US3] 實作前端數量增減按鈕邏輯（+/- 按鈕）於 `OrderLunchWeb/wwwroot/js/order.js`
- [ ] T037 [US3] 實作前端數量輸入驗證（正整數檢查、範圍限制）於 `OrderLunchWeb/wwwroot/js/order.js`
- [ ] T038 [US3] 實作前端無效數量自動修正邏輯於 `OrderLunchWeb/wwwroot/js/order.js`
- [ ] T039 [US3] 更新 Menu.cshtml 數量選擇器 UI 元件於 `OrderLunchWeb/Views/Order/Menu.cshtml`

**Checkpoint**: User Story 3 完成後，數量調整功能應完整且防呆

---

## Phase 6: User Story 4 - 訂單資訊驗證與提交 (Priority: P1)

**Goal**: 使用者在結帳頁面必須填寫姓名和聯絡電話，系統驗證欄位必填且電話僅能為數字，驗證通過後產生訂單編號並儲存訂單。

**Independent Test**: 測試各種填寫情境（空白欄位、無效電話格式、正確資訊），驗證系統是否正確驗證並提交訂單。

### Implementation for User Story 4

- [ ] T040 [US4] 實作前端表單驗證邏輯（validateCheckoutForm）於 `OrderLunchWeb/wwwroot/js/order.js`
- [ ] T041 [US4] 實作前端電話號碼格式驗證於 `OrderLunchWeb/wwwroot/js/order.js`
- [ ] T042 [US4] 更新 Checkout.cshtml 表單驗證訊息顯示於 `OrderLunchWeb/Views/Order/Checkout.cshtml`
- [ ] T043 [US4] 實作訂單成功後清除 Session Storage 於 `OrderLunchWeb/wwwroot/js/order.js`

**Checkpoint**: User Story 4 完成後，訂單驗證和提交流程應完整

---

## Phase 7: User Story 5 - 查看訂單歷史紀錄 (Priority: P2)

**Goal**: 使用者可以從首頁或訂單成功頁面進入訂單紀錄頁面，查看最近5天內的訂單清單，並可查看個別訂單的詳細資訊。

**Independent Test**: 建立多筆測試訂單後，進入訂單紀錄頁面，驗證是否正確顯示最近5天的訂單清單，並可點擊查看個別訂單詳情。

### Implementation for User Story 5

- [ ] T044 [US5] 實作 History Action（訂單紀錄頁面）於 `OrderLunchWeb/Controllers/OrderController.cs`
- [ ] T045 [US5] 實作 Details Action（訂單詳情頁面）於 `OrderLunchWeb/Controllers/OrderController.cs`
- [ ] T046 [P] [US5] 建立 History.cshtml 視圖（訂單紀錄清單）於 `OrderLunchWeb/Views/Order/History.cshtml`
- [ ] T047 [P] [US5] 建立 Details.cshtml 視圖（訂單詳情頁面）於 `OrderLunchWeb/Views/Order/Details.cshtml`
- [ ] T048 [US5] 更新 Confirmation.cshtml 新增「查看訂單紀錄」連結於 `OrderLunchWeb/Views/Order/Confirmation.cshtml`

**Checkpoint**: User Story 5 完成後，使用者應可查看訂單歷史紀錄

---

## Phase 8: User Story 6 - 進行中訂單提示 (Priority: P2)

**Goal**: 當使用者有進行中的訂單（狀態為待確認 Pending）時，在餐廳列表頁面頂部顯示提醒區塊。

**Independent Test**: 建立一筆進行中的訂單，進入餐廳列表頁面，驗證是否正確顯示訂單提示區塊並可點擊查看詳情。

### Implementation for User Story 6

- [ ] T049 [US6] 更新 SelectRestaurant Action 查詢進行中訂單於 `OrderLunchWeb/Controllers/OrderController.cs`
- [ ] T050 [US6] 更新 SelectRestaurant.cshtml 新增進行中訂單提示區塊於 `OrderLunchWeb/Views/Order/SelectRestaurant.cshtml`

**Checkpoint**: User Story 6 完成後，進行中訂單提示功能應正常運作

---

## Phase 9: User Story 7 - 返回修改訂單 (Priority: P3)

**Goal**: 使用者在結帳頁面可以點擊「返回修改」按鈕，回到菜單頁面修改訂單內容，系統會保留當前已選擇的菜品。

**Independent Test**: 在結帳頁面點擊「返回修改」按鈕，驗證是否返回菜單頁面並保留訂單內容。

### Implementation for User Story 7

- [ ] T051 [US7] 更新 Checkout.cshtml 新增「返回修改」按鈕於 `OrderLunchWeb/Views/Order/Checkout.cshtml`
- [ ] T052 [US7] 實作前端「返回修改」導航邏輯（保留購物車）於 `OrderLunchWeb/wwwroot/js/order.js`

**Checkpoint**: User Story 7 完成後，使用者應可從結帳頁面返回修改訂單

---

## Phase 10: Edge Cases & Polish

**Purpose**: 邊界情況處理和跨功能改善

- [ ] T053 [P] 實作無餐廳資料時的空狀態顯示於 `OrderLunchWeb/Views/Order/SelectRestaurant.cshtml`
- [ ] T054 [P] 實作空菜單時的空狀態顯示於 `OrderLunchWeb/Views/Order/Menu.cshtml`
- [ ] T055 [P] 實作無訂單紀錄時的空狀態顯示於 `OrderLunchWeb/Views/Order/History.cshtml`
- [ ] T056 實作結帳逾時提示功能（30 分鐘）於 `OrderLunchWeb/wwwroot/js/order.js`
- [ ] T057 [P] 新增訂單相關 Serilog 日誌記錄於 `OrderLunchWeb/Services/OrderService.cs`
- [ ] T058 更新共用版面配置新增「訂單紀錄」導航連結於 `OrderLunchWeb/Views/Shared/_Layout.cshtml`
- [ ] T059 驗證 quickstart.md 測試場景於 `specs/002-order-food/quickstart.md`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: 無相依性 - 可立即開始
- **Foundational (Phase 2)**: 相依 Setup 完成 - 阻塞所有使用者故事
- **User Stories (Phase 3-9)**: 全部相依 Foundational 完成
  - User Stories 可平行進行（若有多人）
  - 或按優先順序依序進行（P1 → P2 → P3）
- **Polish (Phase 10)**: 相依所有需要的使用者故事完成

### User Story Dependencies

- **User Story 1 (P1)**: Foundational 完成後可開始 - 無其他故事相依
- **User Story 2 (P1)**: Foundational 完成後可開始 - 可與 US1 平行
- **User Story 3 (P1)**: Foundational 完成後可開始 - 可與 US1, US2 平行
- **User Story 4 (P1)**: Foundational 完成後可開始 - 需要 US1 的結帳流程
- **User Story 5 (P2)**: 需要 US1 的訂單建立功能
- **User Story 6 (P2)**: 需要 US1 的訂單建立功能
- **User Story 7 (P3)**: 需要 US1 的結帳流程

### Within Each User Story

- 測試 MUST 在實作前撰寫並確認失敗
- Models → Services
- Services → Controllers
- Controllers → Views
- 核心實作 → 整合
- 故事完成後再進入下一個優先順序

### Parallel Opportunities

- 所有 Setup 階段標記 [P] 的任務可平行執行
- 所有 Foundational 階段標記 [P] 的任務可平行執行
- Foundational 完成後，所有 P1 使用者故事可平行開始
- 同一故事內標記 [P] 的視圖任務可平行執行
- 不同使用者故事可由不同團隊成員平行進行

---

## Parallel Example: User Story 1

```bash
# Launch all tests for User Story 1 together:
Task: T017 "建立 OrderService 單元測試"
Task: T018 "建立 OrderController 整合測試"

# Launch all views for User Story 1 together:
Task: T025 "建立 SelectRestaurant.cshtml 視圖"
Task: T026 "建立 Menu.cshtml 視圖"
Task: T027 "建立 Checkout.cshtml 視圖"
Task: T028 "建立 Confirmation.cshtml 視圖"
```

---

## Implementation Strategy

### MVP First (僅 User Story 1-4)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (CRITICAL - 阻塞所有故事)
3. Complete Phase 3-6: User Story 1-4 (P1 核心訂餐流程)
4. **STOP and VALIDATE**: 測試完整訂餐流程
5. Deploy/demo if ready (MVP 完成)

### Incremental Delivery

1. Complete Setup + Foundational → 基礎設施完成
2. Add User Story 1 → 測試獨立 → Deploy/Demo (訂餐流程 MVP!)
3. Add User Story 2-4 → 測試獨立 → Deploy/Demo (P1 完整)
4. Add User Story 5-6 → 測試獨立 → Deploy/Demo (P2 完整)
5. Add User Story 7 → 測試獨立 → Deploy/Demo (P3 完整)
6. Add Polish → 最終驗證

### Suggested MVP Scope

MVP = Phase 1-6 (User Stories 1-4)

- 完整的訂餐流程（選擇餐廳 → 瀏覽菜單 → 加入訂單 → 結帳 → 確認）
- 訂單摘要即時更新
- 數量選擇與驗證
- 訂單資訊驗證與提交

---

## Notes

- [P] 任務 = 不同檔案，無相依性
- [Story] 標籤對應任務至特定使用者故事以便追蹤
- 每個使用者故事應可獨立完成和測試
- 實作前先驗證測試失敗
- 每個任務或邏輯群組完成後提交
- 任何 checkpoint 都可停下來獨立驗證故事
- 避免：模糊任務、同檔案衝突、破壞獨立性的跨故事相依
