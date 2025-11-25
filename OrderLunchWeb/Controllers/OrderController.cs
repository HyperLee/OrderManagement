using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using OrderLunchWeb.Models;
using OrderLunchWeb.Services;

namespace OrderLunchWeb.Controllers;

/// <summary>
/// 訂單控制器 - 負責訂餐流程、結帳、訂單確認和歷史紀錄
/// </summary>
public class OrderController : Controller
{
    private readonly IStoreService _storeService;
    private readonly IOrderService _orderService;
    private readonly ILogger<OrderController> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// 初始化 OrderController
    /// </summary>
    /// <param name="storeService">店家服務</param>
    /// <param name="orderService">訂單服務</param>
    /// <param name="logger">日誌記錄器</param>
    public OrderController(
        IStoreService storeService,
        IOrderService orderService,
        ILogger<OrderController> logger)
    {
        _storeService = storeService;
        _orderService = orderService;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    /// <summary>
    /// GET: /Order/SelectRestaurant - 顯示餐廳列表頁面
    /// </summary>
    /// <returns>餐廳列表視圖</returns>
    [HttpGet]
    public async Task<IActionResult> SelectRestaurant()
    {
        _logger.LogInformation("顯示餐廳列表頁面");

        try
        {
            var stores = await _storeService.GetAllStoresAsync();
            var pendingOrders = await _orderService.GetPendingOrdersAsync();

            ViewBag.PendingOrders = pendingOrders;

            return View(stores);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "讀取餐廳列表時發生錯誤");
            TempData["ErrorMessage"] = "讀取餐廳列表時發生錯誤，請稍後再試。";
            return View(new List<Store>());
        }
    }

    /// <summary>
    /// GET: /Order/Menu/{storeId} - 顯示餐廳菜單頁面
    /// </summary>
    /// <param name="storeId">餐廳 ID</param>
    /// <returns>菜單視圖</returns>
    [HttpGet("Order/Menu/{storeId}")]
    public async Task<IActionResult> Menu(string storeId)
    {
        _logger.LogInformation("顯示餐廳菜單頁面: StoreId={StoreId}", storeId);

        if (!int.TryParse(storeId, out var id))
        {
            _logger.LogWarning("無效的餐廳 ID: {StoreId}", storeId);
            return NotFound();
        }

        try
        {
            var store = await _storeService.GetStoreByIdAsync(id);

            if (store is null)
            {
                _logger.LogWarning("找不到餐廳: StoreId={StoreId}", storeId);
                return NotFound();
            }

            return View(store);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "讀取餐廳菜單時發生錯誤: StoreId={StoreId}", storeId);
            TempData["ErrorMessage"] = "讀取菜單時發生錯誤，請稍後再試。";
            return RedirectToAction(nameof(SelectRestaurant));
        }
    }

    /// <summary>
    /// GET: /Order/Checkout - 顯示結帳頁面
    /// </summary>
    /// <param name="cartData">購物車資料（JSON 字串）</param>
    /// <returns>結帳視圖</returns>
    [HttpGet]
    public IActionResult Checkout([FromQuery] string cartData)
    {
        _logger.LogInformation("顯示結帳頁面");

        if (string.IsNullOrWhiteSpace(cartData))
        {
            _logger.LogWarning("購物車資料為空");
            TempData["ErrorMessage"] = "訂單為空，請先選擇菜品。";
            return RedirectToAction(nameof(SelectRestaurant));
        }

        try
        {
            var cart = JsonSerializer.Deserialize<CartDto>(cartData, _jsonOptions);

            if (cart is null || cart.Items is null || cart.Items.Count == 0)
            {
                _logger.LogWarning("購物車資料無效或為空");
                TempData["ErrorMessage"] = "訂單為空，請先選擇菜品。";
                return RedirectToAction(nameof(SelectRestaurant));
            }

            var viewModel = new CheckoutViewModel
            {
                StoreId = cart.StoreId,
                StoreName = cart.StoreName,
                CartData = cartData,
                Items = cart.Items.Select(item => new OrderItem
                {
                    MenuItemId = item.MenuItemId,
                    MenuItemName = item.Name,
                    Price = item.Price,
                    Quantity = item.Quantity
                }).ToList()
            };

            return View(viewModel);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "解析購物車資料失敗");
            TempData["ErrorMessage"] = "購物車資料格式錯誤，請重新選擇菜品。";
            return RedirectToAction(nameof(SelectRestaurant));
        }
    }

    /// <summary>
    /// POST: /Order/Submit - 提交訂單
    /// </summary>
    /// <param name="model">結帳視圖模型</param>
    /// <returns>成功則重定向到確認頁面，失敗則返回結帳頁面</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(CheckoutViewModel model)
    {
        _logger.LogInformation("接收提交訂單請求: CustomerName={CustomerName}", model.CustomerName);

        // 解析購物車資料以恢復 Items
        if (!string.IsNullOrWhiteSpace(model.CartData))
        {
            try
            {
                var cart = JsonSerializer.Deserialize<CartDto>(model.CartData, _jsonOptions);
                if (cart?.Items is not null)
                {
                    model.Items = cart.Items.Select(item => new OrderItem
                    {
                        MenuItemId = item.MenuItemId,
                        MenuItemName = item.Name,
                        Price = item.Price,
                        Quantity = item.Quantity
                    }).ToList();
                }
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "解析購物車資料失敗");
                ModelState.AddModelError("", "購物車資料格式錯誤。");
            }
        }

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("訂單資料驗證失敗: CustomerName={CustomerName}", model.CustomerName);
            return View("Checkout", model);
        }

        if (model.Items is null || model.Items.Count == 0)
        {
            _logger.LogWarning("訂單項目為空");
            ModelState.AddModelError("", "訂單必須至少包含一個菜品。");
            return View("Checkout", model);
        }

        try
        {
            var order = new Order
            {
                StoreId = model.StoreId,
                StoreName = model.StoreName,
                CustomerName = model.CustomerName,
                CustomerPhone = model.CustomerPhone,
                Items = model.Items
            };

            var createdOrder = await _orderService.CreateOrderAsync(order);

            _logger.LogInformation(
                "訂單提交成功: OrderId={OrderId}, CustomerName={CustomerName}, TotalAmount={TotalAmount}",
                createdOrder.OrderId, createdOrder.CustomerName, createdOrder.TotalAmount);

            return RedirectToAction(nameof(Confirmation), new { orderId = createdOrder.OrderId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "建立訂單時發生錯誤");
            ModelState.AddModelError("", "訂單建立失敗，請稍後再試。");
            return View("Checkout", model);
        }
    }

    /// <summary>
    /// GET: /Order/Confirmation/{orderId} - 顯示訂單確認頁面
    /// </summary>
    /// <param name="orderId">訂單編號</param>
    /// <returns>訂單確認視圖</returns>
    [HttpGet("Order/Confirmation/{orderId}")]
    public async Task<IActionResult> Confirmation(string orderId)
    {
        _logger.LogInformation("顯示訂單確認頁面: OrderId={OrderId}", orderId);

        try
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);

            if (order is null)
            {
                _logger.LogWarning("找不到訂單: OrderId={OrderId}", orderId);
                return NotFound();
            }

            return View(order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "讀取訂單時發生錯誤: OrderId={OrderId}", orderId);
            TempData["ErrorMessage"] = "讀取訂單時發生錯誤，請稍後再試。";
            return RedirectToAction(nameof(SelectRestaurant));
        }
    }

    /// <summary>
    /// GET: /Order/History - 顯示訂單紀錄頁面（最近 5 天）
    /// </summary>
    /// <returns>訂單紀錄視圖</returns>
    [HttpGet]
    public async Task<IActionResult> History()
    {
        _logger.LogInformation("顯示訂單紀錄頁面");

        try
        {
            var recentOrders = await _orderService.GetRecentOrdersAsync(5);

            var viewModel = new OrderHistoryViewModel
            {
                Orders = recentOrders.Select(o => new OrderHistoryViewModel.OrderSummary
                {
                    OrderId = o.OrderId,
                    CreatedAt = o.CreatedAt,
                    StoreName = o.StoreName,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    ItemCount = o.Items.Count
                }).ToList()
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "讀取訂單紀錄時發生錯誤");
            TempData["ErrorMessage"] = "讀取訂單紀錄時發生錯誤，請稍後再試。";
            return View(new OrderHistoryViewModel());
        }
    }

    /// <summary>
    /// GET: /Order/Details/{orderId} - 顯示訂單詳情頁面
    /// </summary>
    /// <param name="orderId">訂單編號</param>
    /// <returns>訂單詳情視圖</returns>
    [HttpGet("Order/Details/{orderId}")]
    public async Task<IActionResult> Details(string orderId)
    {
        _logger.LogInformation("顯示訂單詳情頁面: OrderId={OrderId}", orderId);

        if (string.IsNullOrWhiteSpace(orderId))
        {
            _logger.LogWarning("訂單編號為空");
            return BadRequest();
        }

        try
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);

            if (order is null)
            {
                _logger.LogWarning("找不到訂單: OrderId={OrderId}", orderId);
                return NotFound();
            }

            return View(order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "讀取訂單詳情時發生錯誤: OrderId={OrderId}", orderId);
            TempData["ErrorMessage"] = "讀取訂單詳情時發生錯誤，請稍後再試。";
            return RedirectToAction(nameof(History));
        }
    }
}
