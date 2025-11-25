# OrderManagement 技術文件

> 本文件包含 OrderManagement 訂餐系統的詳細技術說明，包括 API 端點、業務規則、測試說明、開發指南和部署說明。

---

## 📋 目錄

- [API 端點](#api-端點)
  - [HomeController](#homecontroller)
  - [StoreController](#storecontroller)
  - [OrderController](#ordercontroller)
- [業務規則](#業務規則)
- [測試說明](#測試說明)
- [開發指南](#開發指南)
- [部署說明](#部署說明)
- [常見問題](#常見問題)

---

## API 端點

### HomeController

#### GET /Home/Index

**用途**: 顯示系統首頁

**回應**: HTML 頁面
- 標題: "訂餐系統"
- 即時時間顯示 (每秒更新)
- 主選單: 「店家列表」、「訂購餐點」、「訂單紀錄」

---

### StoreController

#### GET /Store/Index

**用途**: 顯示所有店家列表

**回應**: HTML 頁面
- 卡片式呈現所有店家
- 搜尋框 (即時客戶端篩選)
- 每個店家顯示: 店名、地址、電話、營業時間
- 操作按鈕: 「查看詳情」、「編輯」、「刪除」

---

#### GET /Store/Details/{id}

**用途**: 顯示指定店家的詳細資訊

**參數**:
- `id` (int, required): 店家 ID

**回應**: HTML 頁面
- 店家完整資訊
- 菜單項目表格 (ID、菜名、價格、描述)
- 操作按鈕: 「返回列表」、「編輯」、「刪除」

**錯誤處理**:
- 404 Not Found: 店家 ID 不存在

---

#### GET /Store/Create

**用途**: 顯示新增店家表單

**回應**: HTML 頁面
- 店家基本資訊表單 (店名、地址、電話類型、電話、營業時間)
- 菜單項目動態表單 (預設 1 筆空白項目)
- 操作按鈕: 「新增菜單項目」、「提交」、「取消」

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

---

#### GET /Store/Edit/{id}

**用途**: 顯示編輯店家表單

**參數**:
- `id` (int, required): 店家 ID

**回應**: HTML 頁面
- 預填當前店家資料的表單
- 菜單項目列表 (可新增/移除)
- 操作按鈕: 「新增菜單項目」、「儲存」、「取消」

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

---

#### GET /Store/Delete/{id}

**用途**: 顯示刪除確認頁面

**參數**:
- `id` (int, required): 店家 ID

**回應**: HTML 頁面
- 顯示要刪除的店家完整資訊
- 警告訊息: "確定要刪除此店家嗎？此操作無法復原。"
- 操作按鈕: 「確認刪除」、「取消」

---

#### POST /Store/Delete/{id}

**用途**: 執行刪除操作

**參數**:
- `id` (int, required): 店家 ID

**成功回應**:
- HTTP 302 Redirect → `/Store/Index`
- TempData["SuccessMessage"] = "店家「XXX」已成功刪除。"

---

### OrderController

#### GET /Order/SelectRestaurant

**用途**: 顯示餐廳列表頁面（訂餐入口）

**回應**: HTML 頁面
- 卡片式呈現所有可訂餐的餐廳
- 進行中訂單提示區塊（若有 Pending 狀態訂單）
- 每個餐廳顯示: 名稱、地址、電話、營業時間
- 操作按鈕:「選擇此餐廳」

**特殊處理**:
- 若無餐廳資料，顯示「目前沒有可訂餐的餐廳」訊息
- 若有進行中訂單，頁面頂部顯示提醒

---

#### GET /Order/Menu/{storeId}

**用途**: 顯示指定餐廳的菜單頁面

**參數**:
- `storeId` (int, required): 餐廳 ID

**回應**: HTML 頁面
- 餐廳資訊區塊
- 菜單項目列表（含名稱、價格、數量選擇器）
- 購物車側邊欄（即時更新）
- 操作按鈕:「加入訂單」、「前往結帳」

**特殊處理**:
- 購物車資料儲存於 Session Storage
- 菜品數量限制: 1-100
- 購物車為空時「前往結帳」按鈕禁用

**錯誤處理**:
- 404 Not Found: 餐廳 ID 不存在
- 若菜單為空，顯示「此餐廳目前沒有可訂購的菜品」

---

#### GET /Order/Checkout

**用途**: 顯示結帳頁面

**查詢參數**:
- `cartData` (string, required): 購物車 JSON 資料（Base64 編碼）

**回應**: HTML 頁面
- 訂單明細表格（菜品、單價、數量、小計）
- 訂單總金額
- 訂餐者資訊表單（姓名、電話）
- 逾時提示（停留超過 30 分鐘）
- 操作按鈕:「返回修改」、「確認訂單」

**特殊處理**:
- 從 Session Storage 讀取購物車資料
- 訂單金額上限: NT$ 100,000

---

#### POST /Order/Checkout

**用途**: 提交訂單

**請求 Body** (Form Data):

```csharp
CheckoutViewModel {
    CustomerName: "訂餐者姓名",
    CustomerPhone: "0912345678",
    StoreId: "1",
    StoreName: "餐廳名稱",
    CartData: "購物車 JSON 字串"
}
```

**成功回應**:
- HTTP 302 Redirect → `/Order/Confirmation/{orderId}`

**失敗回應**:
- HTTP 200 (返回結帳頁面)
- ModelState 錯誤訊息

**驗證規則**:
1. 姓名必填
2. 電話必填且僅能為數字
3. 電話長度不超過 20 位
4. 購物車不可為空
5. 訂單金額不超過 100,000

---

#### GET /Order/Confirmation/{orderId}

**用途**: 顯示訂單確認頁面

**參數**:
- `orderId` (string, required): 訂單編號

**回應**: HTML 頁面
- 成功訊息與訂單編號
- 訂單摘要資訊
- 操作按鈕:「返回首頁」、「查看訂單紀錄」

**錯誤處理**:
- 404 Not Found: 訂單編號不存在

---

#### GET /Order/History

**用途**: 顯示訂單紀錄頁面

**回應**: HTML 頁面
- 最近 5 天內的訂單清單
- 表格顯示: 訂單編號、日期時間、餐廳名稱、總金額、狀態
- 操作按鈕:「查看詳情」

**特殊處理**:
- 若無訂單紀錄，顯示「目前沒有訂單紀錄」
- 訂單依建立時間降序排列（最新在前）

---

#### GET /Order/Details/{orderId}

**用途**: 顯示訂單詳情頁面

**參數**:
- `orderId` (string, required): 訂單編號

**回應**: HTML 頁面
- 訂單完整資訊
- 訂單明細表格
- 訂餐者資訊
- 訂單狀態
- 麵包屑導航

**錯誤處理**:
- 404 Not Found: 訂單編號不存在

---

## 業務規則

### 1. ID 管理規則

#### 店家 ID 生成
- **起始值**: 1
- **遞增規則**: 取得當前最大 ID + 1
- **刪除後**: 已刪除的 ID **不會重複使用**

#### 訂單編號生成
- **格式**: `ORD{yyyyMMddHHmmssfff}`
- **範例**: `ORD20251125212308620`
- **唯一性**: 時間戳記保證唯一性

#### 菜單項目 ID 生成
- **起始值**: 1
- **遞增規則**: 店家內部順序編號 (1, 2, 3...)
- **重新編號**: 每次儲存時重新從 1 開始編號

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

**錯誤訊息**: "此店家已存在（店名、電話、地址完全相同）"

---

### 3. 菜單項目限制

#### 數量限制
- **最少**: 1 筆 (必須至少有一個菜單項目)
- **最多**: 20 筆

#### 驗證點
1. **伺服器端**: ModelState 驗證 (`[MinLength(1), MaxLength(20)]`)
2. **客戶端**: JavaScript 即時檢查

---

### 4. 訂單業務規則

#### 訂單金額限制
- **最大金額**: NT$ 100,000
- **超過限制**: 顯示「訂單金額過高，請聯絡客服」

#### 菜品數量限制
- **單一菜品最小**: 1
- **單一菜品最大**: 100

#### 訂單狀態
- **初始狀態**: Pending（待確認）
- **狀態變更**: 此版本不實作狀態變更功能

#### 訂單保留期限
- **保留天數**: 5 天
- **清理時機**: 應用程式啟動時自動執行
- **清理規則**: 超過 5 天的訂單從 `orders.json` 中移除

#### 購物車逾時提示
- **逾時時間**: 30 分鐘
- **處理方式**: 僅顯示提示訊息，不強制清空或阻止結帳

---

### 5. 時間戳記規則

#### CreatedAt (建立時間)
- **設定時機**: 新增店家/訂單時
- **格式**: ISO 8601 (含時區)
- **後續操作**: **永不變更**

#### UpdatedAt (更新時間)
- **設定時機**: 新增或編輯時
- **格式**: ISO 8601 (含時區)
- **後續操作**: 每次編輯時更新為當前時間

---

### 6. 資料驗證規則總覽

#### 店家資料

| 欄位 | 必填 | 格式 | 長度限制 | 其他規則 |
|------|------|------|----------|----------|
| 店家名稱 | ✅ | 文字 | 1-100 字元 | - |
| 店家地址 | ✅ | 文字 | 1-200 字元 | - |
| 電話類型 | ✅ | 列舉 | - | 1=市話, 2=行動 |
| 聯絡電話 | ✅ | 純數字 | 不限 | 不含空格/符號 |
| 營業時間 | ✅ | 文字 | 1-100 字元 | 自由格式 |
| 菜單項目 | ✅ | 陣列 | 1-20 筆 | - |

#### 訂單資料

| 欄位 | 必填 | 格式 | 驗證規則 |
|------|------|------|----------|
| 訂餐者姓名 | ✅ | 文字 | 1-50 字元 |
| 聯絡電話 | ✅ | 純數字 | 1-20 位數 |
| 訂單項目 | ✅ | 陣列 | 至少 1 筆 |
| 菜品數量 | ✅ | 正整數 | 1-100 |
| 訂單金額 | ✅ | 數值 | 0-100,000 |

---

### 7. 防重複提交機制

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
- 表單提交後執行重定向，防止 F5 重複提交
- 使用 `[ValidateAntiForgeryToken]` 防止 CSRF 攻擊

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

### 執行測試

#### 執行所有測試

```bash
cd OrderLunchWeb.Tests
dotnet test
```

#### 執行特定 User Story 測試

```bash
# 店家管理測試
dotnet test --filter "Category=US1"  # 新增店家
dotnet test --filter "Category=US2"  # 瀏覽列表
dotnet test --filter "Category=US3"  # 編修店家
dotnet test --filter "Category=US4"  # 刪除店家

# 訂餐功能測試
dotnet test --filter "FullyQualifiedName~OrderServiceTests"
dotnet test --filter "FullyQualifiedName~OrderControllerTests"
```

#### 產生程式碼覆蓋率報告

```bash
# 產生覆蓋率資料
dotnet test --collect:"XPlat Code Coverage"

# 使用 ReportGenerator 產生 HTML 報告
reportgenerator \
  -reports:"TestResults/*/coverage.cobertura.xml" \
  -targetdir:"TestResults/CoverageReport" \
  -reporttypes:Html

# 開啟報告
open TestResults/CoverageReport/index.html
```

---

### 測試輔助工具

#### TestDataHelper.cs

提供測試資料產生方法:
- `CreateValidStore()` - 建立有效的店家資料
- `CreateStoreWithMultipleMenuItems()` - 建立含多個菜單項目的店家
- `CreateValidOrder()` - 建立有效的訂單資料

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

---

### 程式碼結構導覽

#### Controllers (控制器層)

**職責**: 處理 HTTP 請求、驗證、調用 Service、返回 View/Redirect

#### Services (業務邏輯層)

**職責**: 業務規則驗證、資料轉換、協調多個資料存取操作

#### Data (資料存取層)

**職責**: 檔案讀寫、ID 生成、執行緒安全

---

### 日誌記錄

#### Serilog 配置

```csharp
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File(
        path: "Logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        encoding: Encoding.UTF8)
    .CreateLogger();
```

#### 日誌層級
- **Information**: 正常業務流程
- **Warning**: 業務規則違反
- **Error**: 系統錯誤

---

### Git 工作流程

#### 分支策略

- `main`: 穩定版本 (生產環境)
- `001-store-menu-management`: 店家管理功能分支
- `002-order-food`: 訂餐功能分支

#### Commit 訊息規範

遵循 [Conventional Commits](https://www.conventionalcommits.org/):

```
<type>(<scope>): <subject>
```

**Type 類型**:
- `feat`: 新增功能
- `fix`: 修復 Bug
- `test`: 新增或修改測試
- `refactor`: 重構程式碼
- `docs`: 文件更新

---

## 部署說明

### 本機部署

```bash
cd OrderLunchWeb
dotnet publish -c Release -o ./publish
cd publish
dotnet OrderLunchWeb.dll
```

---

### Docker 部署

#### Dockerfile

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["OrderLunchWeb/OrderLunchWeb.csproj", "OrderLunchWeb/"]
RUN dotnet restore "OrderLunchWeb/OrderLunchWeb.csproj"
COPY . .
WORKDIR "/src/OrderLunchWeb"
RUN dotnet publish "OrderLunchWeb.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN mkdir -p /app/Data
EXPOSE 80
ENTRYPOINT ["dotnet", "OrderLunchWeb.dll"]
```

#### 執行

```bash
docker build -t ordermanagement:1.0 .
docker run -d -p 8080:80 --name orderlunch ordermanagement:1.0
```

---

### IIS 部署 (Windows Server)

1. 安裝 [.NET 8.0 Hosting Bundle](https://dotnet.microsoft.com/download/dotnet/8.0)
2. 發佈應用程式: `dotnet publish -c Release`
3. 設定 IIS 應用程式集區 (.NET CLR 版本: 無受控程式碼)
4. 建立網站並設定繫結

---

### Azure App Service 部署

```bash
# 登入 Azure
az login

# 建立資源群組
az group create --name OrderLunchWebRG --location eastasia

# 建立 App Service
az webapp create \
  --name orderlunchweb \
  --resource-group OrderLunchWebRG \
  --plan OrderLunchWebPlan \
  --runtime "DOTNETCORE:8.0"

# 部署
dotnet publish -c Release -o ./publish
cd publish && zip -r ../app.zip *
az webapp deployment source config-zip \
  --resource-group OrderLunchWebRG \
  --name orderlunchweb \
  --src ../app.zip
```

---

## 常見問題

### Q1: 為什麼使用 JSON 檔案而不是資料庫？

**A**: 本專案是學習用途，JSON 檔案具有以下優勢:
- ✅ 無需安裝資料庫伺服器
- ✅ 易於備份與版本控制
- ✅ 適合小型資料量

**未來擴充**: 可輕鬆替換為 Entity Framework Core + SQL Server，只需實作 `IFileStorage` 介面。

---

### Q2: 如何備份資料？

```bash
# 備份店家資料
cp Data/stores.json Data/stores_backup_$(date +%Y%m%d).json

# 備份訂單資料
cp Data/orders.json Data/orders_backup_$(date +%Y%m%d).json
```

---

### Q3: 訂單超過 5 天會怎樣？

**A**: 應用程式啟動時會自動清理超過 5 天的訂單。清理後的訂單將從 `orders.json` 中移除，無法再查詢。

---

### Q4: 購物車資料儲存在哪裡？

**A**: 購物車資料儲存在瀏覽器的 **Session Storage**，具有以下特性:
- 關閉分頁即清空
- 不同分頁不共享
- 重新整理頁面時保留

---

### Q5: 如何新增訂單狀態變更功能？

**A**: 在 `IOrderService` 介面新增方法:

```csharp
Task<Order> UpdateOrderStatusAsync(string orderId, OrderStatus newStatus);
```

然後在 `OrderService` 中實作，並在 Controller 新增對應的 Action。

---

**📖 返回 [README.md](README.md) 查看專案概覽**
