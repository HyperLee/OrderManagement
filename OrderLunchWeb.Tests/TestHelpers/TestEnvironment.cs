namespace OrderLunchWeb.Tests.TestHelpers;

/// <summary>
/// 提供測試環境檢查與臨時檔案管理功能
/// </summary>
public static class TestEnvironment
{
    private static readonly string TestDataDirectory = Path.Combine(
        Path.GetTempPath(),
        "OrderLunchWeb.Tests",
        Guid.NewGuid().ToString()
    );

    /// <summary>
    /// 取得測試資料目錄路徑
    /// </summary>
    public static string GetTestDataDirectory()
    {
        if (!Directory.Exists(TestDataDirectory))
        {
            Directory.CreateDirectory(TestDataDirectory);
        }

        return TestDataDirectory;
    }

    /// <summary>
    /// 取得測試用的 JSON 檔案路徑
    /// </summary>
    /// <param name="fileName">檔案名稱（例如: stores.json）</param>
    /// <returns>完整的檔案路徑</returns>
    public static string GetTestFilePath(string fileName)
    {
        var directory = GetTestDataDirectory();
        return Path.Combine(directory, fileName);
    }

    /// <summary>
    /// 清理測試資料目錄
    /// </summary>
    public static void CleanupTestData()
    {
        if (Directory.Exists(TestDataDirectory))
        {
            try
            {
                Directory.Delete(TestDataDirectory, recursive: true);
            }
            catch (Exception)
            {
                // 忽略清理失敗（可能被其他測試使用）
            }
        }
    }

    /// <summary>
    /// 建立包含指定內容的測試檔案
    /// </summary>
    /// <param name="fileName">檔案名稱</param>
    /// <param name="content">檔案內容</param>
    /// <returns>建立的檔案路徑</returns>
    public static string CreateTestFile(string fileName, string content)
    {
        var filePath = GetTestFilePath(fileName);
        var directory = Path.GetDirectoryName(filePath);

        if (directory is not null && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(filePath, content, System.Text.Encoding.UTF8);
        return filePath;
    }

    /// <summary>
    /// 檢查測試環境是否正常
    /// </summary>
    /// <returns>環境檢查結果</returns>
    public static bool IsEnvironmentHealthy()
    {
        try
        {
            // 檢查能否建立測試目錄
            var testDir = GetTestDataDirectory();
            if (!Directory.Exists(testDir))
            {
                return false;
            }

            // 檢查能否寫入檔案
            var testFile = Path.Combine(testDir, "health_check.txt");
            File.WriteAllText(testFile, "test", System.Text.Encoding.UTF8);
            File.Delete(testFile);

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 取得唯一的測試檔案名稱（避免測試間衝突）
    /// </summary>
    /// <param name="baseFileName">基礎檔案名稱（例如: stores.json）</param>
    /// <returns>唯一的檔案名稱</returns>
    public static string GetUniqueFileName(string baseFileName)
    {
        var extension = Path.GetExtension(baseFileName);
        var nameWithoutExt = Path.GetFileNameWithoutExtension(baseFileName);
        return $"{nameWithoutExt}_{Guid.NewGuid():N}{extension}";
    }
}
