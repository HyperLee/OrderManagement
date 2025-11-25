using Serilog;
using OrderLunchWeb.Data;
using OrderLunchWeb.Services;

namespace OrderLunchWeb;

/// <summary>
/// 應用程式主類別
/// </summary>
public class Program
{
    /// <summary>
    /// 應用程式入口點
    /// </summary>
    /// <param name="args">命令列參數</param>
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // 設定 Serilog 結構化日誌
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(
                path: "Logs/log-.txt",
                rollingInterval: RollingInterval.Day,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
                encoding: System.Text.Encoding.UTF8)
            .CreateLogger();

        builder.Host.UseSerilog();

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        // 註冊相依性注入
        builder.Services.AddSingleton<IFileStorage, JsonFileStorage>();
        builder.Services.AddScoped<IStoreService, StoreService>();
        builder.Services.AddScoped<IOrderService, OrderService>();

        var app = builder.Build();

        // 應用程式啟動時清理超過 5 天的舊訂單
        await CleanupOldOrdersAsync(app.Services);

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }

    /// <summary>
    /// 清理超過指定天數的舊訂單
    /// </summary>
    /// <param name="services">服務提供者</param>
    private static async Task CleanupOldOrdersAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            var removedCount = await orderService.CleanupOldOrdersAsync(days: 5);
            if (removedCount > 0)
            {
                logger.LogInformation("應用程式啟動：已清理 {RemovedCount} 筆超過 5 天的舊訂單", removedCount);
            }
            else
            {
                logger.LogInformation("應用程式啟動：沒有需要清理的舊訂單");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "應用程式啟動時清理舊訂單失敗");
        }
    }
}
