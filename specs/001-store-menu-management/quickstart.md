# 快速入門指南: 店家與菜單管理系統

**Feature**: 001-store-menu-management  
**Date**: 2025-11-22  
**Audience**: 開發人員、測試人員

## 目標

本指南幫助您快速建立和測試店家與菜單管理功能。完成本指南後，您將能夠:

- 設定開發環境
- 執行應用程式
- 理解專案結構
- 執行基本操作 (CRUD)
- 執行測試

**預計完成時間**: 15-20 分鐘

---

## 前置需求

### 必要工具

- **.NET 8.0 SDK** 或更新版本
  - 檢查版本: `dotnet --version`
  - 下載: [https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download)

- **程式碼編輯器**
  - Visual Studio 2022 (建議) 或
  - Visual Studio Code + C# 擴充套件

- **瀏覽器**
  - Chrome、Firefox、Edge 或 Safari (支援 JavaScript)

### 選用工具

- **Git** (用於版本控制)
- **Postman** (用於 API 測試，非必要)

---

## 步驟 1: 取得程式碼

### 選項 A: 從 Git 複製

```bash
git clone <repository-url>
cd OrderManagement
git checkout 001-store-menu-management
```

### 選項 B: 下載壓縮檔

1. 下載專案壓縮檔
2. 解壓縮到工作目錄
3. 開啟終端機，切換到專案目錄

---

## 步驟 2: 還原相依套件

```bash
cd OrderLunchWeb
dotnet restore
```

**預期輸出**:

```
Determining projects to restore...
Restored /path/to/OrderLunchWeb/OrderLunchWeb.csproj (in XXX ms).
```

---

## 步驟 3: 建構專案

```bash
dotnet build
```

**預期輸出**:

```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

**若遇到錯誤**: 檢查 .NET SDK 版本是否 ≥ 8.0

---

## 步驟 4: 執行應用程式

```bash
dotnet run
```

**預期輸出**:

```
Building...
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

**開啟瀏覽器**: 訪問 [http://localhost:5000](http://localhost:5000) 或 [https://localhost:5001](https://localhost:5001)

---

## 步驟 5: 探索應用程式

### 首頁 (/)

**應該看到**:

- 標題: "訂餐系統"
- 即時時間顯示 (每秒更新)
- 主選單:
  - 「店家列表」連結
  - 「訂購餐點」按鈕 (佔位)

**驗證**: 確認時間每秒更新，格式為 `yyyy/MM/dd HH:mm:ss`

---

### 店家列表 (/Store/Index)

點擊「店家列表」

**第一次執行 (無資料)**:

- 顯示: "目前沒有店家資料"
- 提供「新增店家」按鈕

**驗證**: JSON 檔案 `Data/stores.json` 自動建立 (若不存在)

---

### 新增店家 (/Store/Create)

點擊「新增店家」

**表單欄位**:

1. **店家名稱**: 輸入 "好吃便當店" (必填，最多 100 字元)
2. **店家地址**: 輸入 "台北市中正區羅斯福路一段 100 號" (必填，最多 200 字元)
3. **電話類型**: 選擇 "行動電話" (必填)
4. **聯絡電話**: 輸入 "0912345678" (必填，僅數字)
5. **營業時間**: 輸入 "週一至週五 11:00-14:00, 17:00-20:00" (必填，最多 100 字元)
6. **菜單項目 1**:
   - 菜名: "排骨便當" (必填，最多 50 字元)
   - 價格: "80" (必填，≥ 0)
   - 描述: "香酥排骨配上三菜一飯" (選填，最多 200 字元)
7. **菜單項目 2**:
   - 菜名: "雞腿便當"
   - 價格: "90"
   - 描述: "大雞腿配上三菜一飯"

**操作**:

- 點擊「新增菜單項目」按鈕可新增更多項目
- 點擊「移除」按鈕可刪除菜單項目 (至少保留 1 項)
- 點擊「提交」送出表單

**驗證**:

- 成功後重導向到店家列表
- 顯示成功訊息: "店家新增成功"
- 店家出現在列表中

**測試驗證**:

1. **必填欄位驗證**: 嘗試不填寫店名，應顯示 "店家名稱為必填欄位"
2. **電話驗證**: 輸入 "09-1234-5678"，應顯示 "電話號碼僅能輸入數字"
3. **價格驗證**: 輸入 "-10"，應顯示 "價格必須為正整數或零"
4. **重複店家驗證**: 嘗試新增相同店家 (名稱+電話+地址)，應顯示 "此店家已存在"

---

### 查看店家詳情 (/Store/Details/{id})

在店家列表點擊「查看詳情」

**應該看到**:

- 店家完整資訊
- 完整菜單列表 (名稱、價格、描述)
- 建立時間和修改時間
- 「返回列表」、「編輯」、「刪除」按鈕

---

### 編輯店家 (/Store/Edit/{id})

在店家列表點擊「編輯」

**應該看到**:

- 表單預先填入現有資料
- 可修改任何欄位
- 可新增/移除/修改菜單項目

**測試操作**:

1. 修改價格: 將 "排骨便當" 改為 85 元
2. 新增菜單: 新增 "雞排便當 - 100 元"
3. 點擊「儲存」

**驗證**:

- 成功後重導向到店家列表
- 顯示: "店家資料更新成功"
- 檢查詳情頁，確認修改已生效

---

### 刪除店家 (/Store/Delete/{id})

在店家列表點擊「刪除」

**應該看到**:

- 確認頁面顯示店家資訊
- 警告訊息: "確定要刪除此店家嗎？此操作無法復原。"
- 「確認刪除」和「取消」按鈕

**測試操作**:

1. 點擊「取消」→ 返回列表，資料未刪除
2. 再次點擊「刪除」→ 點擊「確認刪除」

**驗證**:

- 成功後重導向到店家列表
- 顯示: "店家刪除成功"
- 店家從列表中消失

---

### 搜尋功能

在店家列表頁面 (需先新增 2-3 筆店家)

**測試操作**:

1. 在搜尋框輸入 "便當"
2. 即時篩選顯示包含 "便當" 的店家
3. 清空搜尋框，顯示所有店家

**驗證**: 搜尋無延遲，即時回應 (< 0.5 秒)

---

## 步驟 6: 檢查 JSON 檔案

開啟 `OrderLunchWeb/Data/stores.json`

**預期內容**:

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
      "createdAt": "2025-11-22T14:30:00",
      "updatedAt": "2025-11-22T14:30:00"
    }
  ],
  "nextStoreId": 2,
  "nextMenuItemId": 3
}
```

**驗證**:

- JSON 格式正確
- 中文字元正確顯示 (無亂碼)
- ID 自動遞增

---

## 步驟 7: 執行測試 (實作後)

### 測試環境檢查

在執行測試前，確認環境已正確設定:

```bash
# 檢查 .NET 版本 (需 >= 8.0)
dotnet --version

# 切換到測試專案目錄
cd OrderLunchWeb.Tests

# 還原測試專案相依套件
dotnet restore
```

### 執行測試

```bash
# 執行所有測試 (建議加上逾時保護)
dotnet test --timeout 30000

# 執行特定測試類別
dotnet test --filter FullyQualifiedName~StoreServiceTests

# 執行測試並顯示詳細輸出
dotnet test --verbosity normal

# 執行測試並產生覆蓋率報告 (需安裝 coverlet)
dotnet test /p:CollectCoverage=true /p:CoverageReportFormat=opencover
```

**預期輸出**:

```text
Test run for OrderLunchWeb.Tests.dll (.NET 8.0)
Microsoft (R) Test Execution Command Line Tool Version X.X.X
Starting test execution, please wait...

Passed!  - Failed:     0, Passed:    XX, Skipped:     0, Total:    XX
Time: XX.XXXs
```

### 測試逾時與容錯

**測試配置** (`xunit.runner.json`):

- ⏱️ **逾時控制**: 長時間測試 (>10 秒) 會被標記
- 🔒 **單執行緒**: 避免 JSON 檔案併發寫入問題
- 🧹 **自動清理**: 測試完成後自動清理臨時檔案

**如果測試卡住**:

1. **檢查檔案鎖定**: 確認沒有其他程式開啟測試資料檔案
2. **清理測試資料**: 手動刪除 `OrderLunchWeb.Tests/bin/Debug/net8.0/TestData/` 目錄
3. **重新建置**: `dotnet clean && dotnet build`
4. **個別執行**: 使用 `--filter` 參數逐一執行測試，找出問題測試

**跳過有問題的測試**:

```csharp
[Fact(Skip = "暫時跳過，待環境問題解決")]
public async Task ProblematicTest()
{
    // 測試邏輯
}
```

---

## 專案結構說明

```text
OrderLunchWeb/
├── Controllers/
│   ├── HomeController.cs        # 首頁 Controller
│   └── StoreController.cs       # 店家管理 Controller
├── Models/
│   ├── Store.cs                 # 店家實體類別
│   ├── MenuItem.cs              # 菜單項目實體類別
│   └── PhoneType.cs             # 電話類型列舉
├── Services/
│   ├── IStoreService.cs         # 店家服務介面
│   └── StoreService.cs          # 店家服務實作
├── Data/
│   ├── IFileStorage.cs          # 檔案儲存介面
│   ├── JsonFileStorage.cs       # JSON 檔案儲存實作
│   └── stores.json              # 資料儲存檔案 (自動建立)
├── Views/
│   ├── Home/
│   │   └── Index.cshtml         # 首頁視圖
│   ├── Store/
│   │   ├── Index.cshtml         # 店家列表
│   │   ├── Create.cshtml        # 新增店家
│   │   ├── Details.cshtml       # 店家詳情
│   │   ├── Edit.cshtml          # 編輯店家
│   │   └── Delete.cshtml        # 刪除確認
│   └── Shared/
│       └── _Layout.cshtml       # 共用版面配置
├── wwwroot/
│   ├── css/
│   └── js/
└── Program.cs                   # 應用程式進入點
```

---

## 常見問題

### 1. 應用程式無法啟動

**問題**: `dotnet run` 執行後顯示錯誤

**解決方案**:

- 檢查 .NET SDK 版本: `dotnet --version` (需 ≥ 8.0)
- 清理專案: `dotnet clean && dotnet build`
- 檢查連接埠是否被占用: 更改 `Properties/launchSettings.json` 中的連接埠

### 2. JSON 檔案中文亂碼

**問題**: `stores.json` 中文字元顯示為 `\uXXXX`

**解決方案**:

- 確認 `JsonSerializerOptions` 設定包含 `Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping`
- 使用 UTF-8 編碼的編輯器開啟 JSON 檔案

### 3. 表單驗證無作用

**問題**: 提交空表單不顯示錯誤

**解決方案**:

- 確認 `_ValidationScriptsPartial.cshtml` 已包含在 View 中
- 檢查瀏覽器開發者工具，確認 jQuery Validation 腳本已載入

### 4. 搜尋功能無反應

**問題**: 輸入搜尋關鍵字後無篩選效果

**解決方案**:

- 檢查瀏覽器開發者工具的 Console，查看 JavaScript 錯誤
- 確認 `store-item` 元素包含 `data-name` 屬性

### 5. 刪除店家後 ID 重複使用

**問題**: 刪除 ID=1 的店家後，新增店家 ID 仍為 1

**解決方案**:

- 檢查 `JsonFileStorage.InitializeAsync()` 是否正確計算 `nextStoreId`
- 確認 `stores.json` 中的 `nextStoreId` 和 `nextMenuItemId` 正確儲存

### 6. 測試執行卡住或逾時

**問題**: `dotnet test` 執行時卡住，無回應或逾時

**解決方案**:

- **檢查檔案鎖定**: 關閉所有可能開啟測試資料檔案的程式 (如文字編輯器、檔案總管)
- **清理測試資料**: 刪除 `OrderLunchWeb.Tests/bin/Debug/net8.0/TestData/` 目錄
- **檢查逾時設定**: 確認 `xunit.runner.json` 存在且設定正確
- **單獨執行測試**: 使用 `--filter` 參數找出有問題的測試
- **重新建置**: 執行 `dotnet clean && dotnet build`
- **檢查 .NET 版本**: 確認版本 >= 8.0

```bash
# 清理並重新建置
dotnet clean
dotnet build

# 執行測試加上逾時保護
dotnet test --timeout 30000

# 個別執行測試類別
dotnet test --filter FullyQualifiedName~StoreServiceTests --verbosity normal
```

### 7. Serilog 日誌檔案無法寫入

**問題**: 應用程式啟動後無法建立或寫入日誌檔案

**解決方案**:

- **檢查權限**: 確認應用程式有權限寫入 `Logs/` 目錄
- **手動建立目錄**: 先手動建立 `Logs/` 目錄
- **檢查磁碟空間**: 確認磁碟有足夠空間
- **檢查檔案鎖定**: 確認日誌檔案未被其他程式開啟
- **檢查編碼設定**: 確認 Serilog 配置使用 UTF-8 編碼

```csharp
// 在 Program.cs 中確保目錄存在
var logsDir = Path.Combine(AppContext.BaseDirectory, "Logs");
if (!Directory.Exists(logsDir))
{
    Directory.CreateDirectory(logsDir);
}
```

---

## 下一步

完成快速入門後，您可以:

1. **閱讀詳細文件**:
   - [data-model.md](./data-model.md) - 資料模型設計
   - [contracts/api-endpoints.md](./contracts/api-endpoints.md) - API 端點規格
   - [research.md](./research.md) - 技術研究報告

2. **執行進階操作**:
   - 新增更多店家 (測試搜尋和列表效能)
   - 測試邊界情況 (20 筆菜單項目、100 字元店名)
   - 測試重複店家驗證

3. **開始開發**:
   - 參考 [tasks.md](./tasks.md) (Phase 2 產生) 開始實作
   - 遵循測試優先開發 (TDD) 流程
   - 定期執行測試確保品質

4. **探索擴展功能**:
   - 新增圖片上傳功能
   - 實作分頁功能 (當店家數量 > 20)
   - 新增匯出/匯入 JSON 功能

---

## 支援

若遇到問題或需要協助:

1. 檢查 [常見問題](#常見問題) 區段
2. 查看專案的 Issue Tracker
3. 聯繫專案維護者

**祝您開發順利！** 🎉
