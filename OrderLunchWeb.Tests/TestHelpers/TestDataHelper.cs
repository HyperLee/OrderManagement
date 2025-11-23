namespace OrderLunchWeb.Tests.TestHelpers;

/// <summary>
/// 提供測試資料產生的輔助工具
/// </summary>
public static class TestDataHelper
{
    /// <summary>
    /// 建立有效的測試店家資料
    /// </summary>
    /// <param name="id">店家 ID</param>
    /// <param name="name">店家名稱</param>
    /// <param name="menuItemCount">菜單項目數量（預設 3 筆）</param>
    /// <returns>店家物件</returns>
    public static object CreateValidStore(
        int id = 1,
        string? name = null,
        int menuItemCount = 3)
    {
        var storeName = name ?? $"測試店家{id}";
        var menuItems = new List<object>();

        for (int i = 1; i <= menuItemCount; i++)
        {
            menuItems.Add(CreateValidMenuItem(i, $"餐點{i}"));
        }

        return new
        {
            Id = id,
            Name = storeName,
            Address = $"台北市測試區測試路{id}號",
            PhoneType = "Mobile",
            Phone = $"0912345{id:D3}",
            BusinessHours = "週一至週五 11:00-14:00, 17:00-21:00",
            MenuItems = menuItems,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
    }

    /// <summary>
    /// 建立有效的測試菜單項目
    /// </summary>
    /// <param name="id">項目 ID</param>
    /// <param name="name">項目名稱</param>
    /// <param name="price">價格（預設 100）</param>
    /// <param name="description">描述</param>
    /// <returns>菜單項目物件</returns>
    public static object CreateValidMenuItem(
        int id = 1,
        string? name = null,
        int price = 100,
        string? description = null)
    {
        return new
        {
            Id = id,
            Name = name ?? $"測試餐點{id}",
            Price = price,
            Description = description ?? $"這是測試餐點{id}的描述"
        };
    }

    /// <summary>
    /// 建立包含指定數量店家的 JSON 字串
    /// </summary>
    /// <param name="count">店家數量</param>
    /// <returns>JSON 字串</returns>
    public static string CreateStoresJson(int count)
    {
        var stores = new List<object>();
        for (int i = 1; i <= count; i++)
        {
            stores.Add(CreateValidStore(i));
        }

        return System.Text.Json.JsonSerializer.Serialize(stores, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });
    }

    /// <summary>
    /// 建立空的店家列表 JSON 字串
    /// </summary>
    /// <returns>空陣列的 JSON 字串</returns>
    public static string CreateEmptyStoresJson()
    {
        return "[]";
    }

    /// <summary>
    /// 建立損毀的 JSON 字串（用於錯誤測試）
    /// </summary>
    /// <returns>無效的 JSON 字串</returns>
    public static string CreateInvalidJson()
    {
        return "{ invalid json content";
    }

    /// <summary>
    /// 產生隨機的店家名稱
    /// </summary>
    /// <returns>店家名稱</returns>
    public static string GenerateRandomStoreName()
    {
        var prefixes = new[] { "美味", "好吃", "香濃", "道地", "特色" };
        var suffixes = new[] { "小吃店", "餐廳", "飲食店", "食堂", "美食" };
        var random = new Random();

        return $"{prefixes[random.Next(prefixes.Length)]}{suffixes[random.Next(suffixes.Length)]}";
    }

    /// <summary>
    /// 產生隨機的電話號碼（行動電話格式）
    /// </summary>
    /// <returns>電話號碼</returns>
    public static string GenerateRandomPhone()
    {
        var random = new Random();
        return $"09{random.Next(10000000, 99999999)}";
    }

    /// <summary>
    /// 產生隨機的地址
    /// </summary>
    /// <returns>地址</returns>
    public static string GenerateRandomAddress()
    {
        var cities = new[] { "台北市", "新北市", "桃園市", "台中市", "台南市", "高雄市" };
        var districts = new[] { "中正區", "大安區", "信義區", "松山區", "中山區" };
        var random = new Random();

        var city = cities[random.Next(cities.Length)];
        var district = districts[random.Next(districts.Length)];
        var streetNumber = random.Next(1, 200);

        return $"{city}{district}測試路{streetNumber}號";
    }

    /// <summary>
    /// 建立測試用的表單資料字典
    /// </summary>
    /// <param name="store">店家資料物件</param>
    /// <returns>表單資料字典</returns>
    public static Dictionary<string, string> CreateFormData(dynamic store)
    {
        var formData = new Dictionary<string, string>
        {
            ["Name"] = store.Name,
            ["Address"] = store.Address,
            ["PhoneType"] = store.PhoneType,
            ["Phone"] = store.Phone,
            ["BusinessHours"] = store.BusinessHours
        };

        // 新增菜單項目
        if (store.MenuItems is not null)
        {
            for (int i = 0; i < store.MenuItems.Count; i++)
            {
                var item = store.MenuItems[i];
                formData[$"MenuItems[{i}].Name"] = item.Name;
                formData[$"MenuItems[{i}].Price"] = item.Price.ToString();
                formData[$"MenuItems[{i}].Description"] = item.Description ?? string.Empty;
            }
        }

        return formData;
    }

    /// <summary>
    /// 驗證字串是否為有效的 JSON 格式
    /// </summary>
    /// <param name="json">JSON 字串</param>
    /// <returns>是否為有效 JSON</returns>
    public static bool IsValidJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return false;
        }

        try
        {
            System.Text.Json.JsonDocument.Parse(json);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 建立包含特殊字元的店家名稱（用於驗證測試）
    /// </summary>
    /// <returns>包含特殊字元的店家名稱</returns>
    public static string CreateStoreNameWithSpecialChars()
    {
        return "測試店家<>&\"'";
    }

    /// <summary>
    /// 建立超長字串（用於長度驗證測試）
    /// </summary>
    /// <param name="length">字串長度</param>
    /// <returns>指定長度的字串</returns>
    public static string CreateLongString(int length)
    {
        return new string('測', length);
    }

    /// <summary>
    /// 建立包含最小數量菜單項目的店家（1 筆）
    /// </summary>
    /// <returns>店家物件</returns>
    public static object CreateStoreWithMinimumMenuItems()
    {
        return CreateValidStore(menuItemCount: 1);
    }

    /// <summary>
    /// 建立包含最大數量菜單項目的店家（20 筆）
    /// </summary>
    /// <returns>店家物件</returns>
    public static object CreateStoreWithMaximumMenuItems()
    {
        return CreateValidStore(menuItemCount: 20);
    }

    /// <summary>
    /// 建立價格為零的菜單項目（用於邊界測試）
    /// </summary>
    /// <returns>菜單項目物件</returns>
    public static object CreateMenuItemWithZeroPrice()
    {
        return CreateValidMenuItem(price: 0);
    }
}
