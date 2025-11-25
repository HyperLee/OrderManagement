/**
 * 訂單相關 JavaScript 功能
 * 包含購物車 Session Storage 管理、訂單摘要更新等功能
 */

/**
 * 購物車儲存物件
 * 使用 Session Storage 管理購物車狀態
 */
const CartStorage = {
    /**
     * Session Storage 鍵名
     */
    STORAGE_KEY: 'orderLunchCart',

    /**
     * 取得購物車資料
     * @returns {Object} 購物車資料，若不存在則回傳空購物車
     */
    getCart: function() {
        try {
            const cartJson = sessionStorage.getItem(this.STORAGE_KEY);
            if (!cartJson) {
                return this._createEmptyCart();
            }
            const cart = JSON.parse(cartJson);
            return this._validateCart(cart) ? cart : this._createEmptyCart();
        } catch (error) {
            console.error('讀取購物車失敗:', error);
            return this._createEmptyCart();
        }
    },

    /**
     * 儲存購物車資料
     * @param {Object} cart 購物車資料
     */
    saveCart: function(cart) {
        try {
            if (!this._validateCart(cart)) {
                console.error('購物車資料格式無效');
                return false;
            }
            sessionStorage.setItem(this.STORAGE_KEY, JSON.stringify(cart));
            return true;
        } catch (error) {
            console.error('儲存購物車失敗:', error);
            return false;
        }
    },

    /**
     * 清除購物車
     */
    clearCart: function() {
        try {
            sessionStorage.removeItem(this.STORAGE_KEY);
            return true;
        } catch (error) {
            console.error('清除購物車失敗:', error);
            return false;
        }
    },

    /**
     * 初始化購物車（設定餐廳資訊）
     * @param {string} storeId 餐廳 ID
     * @param {string} storeName 餐廳名稱
     */
    initCart: function(storeId, storeName) {
        const cart = this.getCart();
        
        // 如果是不同餐廳，清空購物車
        if (cart.storeId && cart.storeId !== storeId) {
            this.clearCart();
        }

        const newCart = {
            storeId: storeId,
            storeName: storeName,
            items: cart.storeId === storeId ? cart.items : []
        };
        
        this.saveCart(newCart);
        return newCart;
    },

    /**
     * 加入菜品到購物車
     * @param {Object} item 菜品資訊 { menuItemId, name, price, quantity }
     * @returns {Object} 更新後的購物車
     */
    addItem: function(item) {
        if (!this._validateItem(item)) {
            console.error('菜品資料格式無效');
            return null;
        }

        const cart = this.getCart();
        const existingIndex = cart.items.findIndex(i => i.menuItemId === item.menuItemId);

        if (existingIndex >= 0) {
            // 更新現有項目的數量
            cart.items[existingIndex].quantity += item.quantity;
            // 確保數量在有效範圍內
            cart.items[existingIndex].quantity = Math.min(Math.max(cart.items[existingIndex].quantity, 1), 100);
        } else {
            // 新增項目
            cart.items.push({
                menuItemId: item.menuItemId,
                name: item.name,
                price: item.price,
                quantity: Math.min(Math.max(item.quantity, 1), 100)
            });
        }

        this.saveCart(cart);
        return cart;
    },

    /**
     * 更新菜品數量
     * @param {string} menuItemId 菜品 ID
     * @param {number} quantity 新數量
     * @returns {Object} 更新後的購物車
     */
    updateQuantity: function(menuItemId, quantity) {
        const cart = this.getCart();
        const itemIndex = cart.items.findIndex(i => i.menuItemId === menuItemId);

        if (itemIndex < 0) {
            console.error('找不到指定的菜品');
            return null;
        }

        quantity = parseInt(quantity, 10);
        if (isNaN(quantity) || quantity < 1) {
            // 數量小於 1 時移除項目
            cart.items.splice(itemIndex, 1);
        } else {
            // 更新數量（限制在 1-100 之間）
            cart.items[itemIndex].quantity = Math.min(Math.max(quantity, 1), 100);
        }

        this.saveCart(cart);
        return cart;
    },

    /**
     * 從購物車移除菜品
     * @param {string} menuItemId 菜品 ID
     * @returns {Object} 更新後的購物車
     */
    removeItem: function(menuItemId) {
        const cart = this.getCart();
        cart.items = cart.items.filter(i => i.menuItemId !== menuItemId);
        this.saveCart(cart);
        return cart;
    },

    /**
     * 計算購物車總金額
     * @returns {number} 總金額
     */
    getTotalAmount: function() {
        const cart = this.getCart();
        return cart.items.reduce((total, item) => {
            return total + Math.round(item.price * item.quantity * 100) / 100;
        }, 0);
    },

    /**
     * 取得購物車項目數量
     * @returns {number} 項目數量
     */
    getItemCount: function() {
        const cart = this.getCart();
        return cart.items.reduce((count, item) => count + item.quantity, 0);
    },

    /**
     * 購物車是否為空
     * @returns {boolean} 是否為空
     */
    isEmpty: function() {
        const cart = this.getCart();
        return cart.items.length === 0;
    },

    /**
     * 取得購物車資料的 JSON 字串（用於表單提交）
     * @returns {string} JSON 字串
     */
    toJson: function() {
        return JSON.stringify(this.getCart());
    },

    /**
     * 建立空購物車
     * @private
     */
    _createEmptyCart: function() {
        return {
            storeId: '',
            storeName: '',
            items: []
        };
    },

    /**
     * 驗證購物車資料格式
     * @param {Object} cart 購物車資料
     * @returns {boolean} 是否有效
     * @private
     */
    _validateCart: function(cart) {
        if (!cart || typeof cart !== 'object') {
            return false;
        }
        if (typeof cart.storeId !== 'string' || typeof cart.storeName !== 'string') {
            return false;
        }
        if (!Array.isArray(cart.items)) {
            return false;
        }
        return true;
    },

    /**
     * 驗證菜品資料格式
     * @param {Object} item 菜品資料
     * @returns {boolean} 是否有效
     * @private
     */
    _validateItem: function(item) {
        if (!item || typeof item !== 'object') {
            return false;
        }
        if (typeof item.menuItemId !== 'string' || !item.menuItemId) {
            return false;
        }
        if (typeof item.name !== 'string' || !item.name) {
            return false;
        }
        if (typeof item.price !== 'number' || item.price <= 0) {
            return false;
        }
        if (typeof item.quantity !== 'number' || item.quantity < 1 || item.quantity > 100) {
            return false;
        }
        return true;
    }
};

/**
 * 格式化金額為新臺幣格式
 * @param {number} amount 金額
 * @returns {string} 格式化後的金額字串（如：NT$ 1,234）
 */
function formatCurrency(amount) {
    return 'NT$ ' + amount.toLocaleString('zh-TW', {
        minimumFractionDigits: 0,
        maximumFractionDigits: 0
    });
}

/**
 * 將菜品加入訂單
 * @param {string} menuItemId 菜品 ID
 * @param {string} name 菜品名稱
 * @param {number} price 單價
 * @param {number} quantity 數量
 */
function addToOrder(menuItemId, name, price, quantity) {
    if (!menuItemId || !name || price <= 0 || quantity < 1) {
        console.error('加入訂單參數無效');
        return;
    }

    const item = {
        menuItemId: menuItemId,
        name: name,
        price: price,
        quantity: Math.min(Math.max(parseInt(quantity, 10) || 1, 1), 100)
    };

    const result = CartStorage.addItem(item);
    
    if (result) {
        // 更新訂單摘要 UI
        updateOrderSummary();
        
        // 顯示成功訊息（可選）
        console.log('已加入訂單:', name, 'x', quantity);
    }
}

/**
 * 前往結帳頁面
 */
function goToCheckout() {
    if (CartStorage.isEmpty()) {
        alert('購物車是空的，請先選擇菜品。');
        return;
    }

    const cartJson = CartStorage.toJson();
    const checkoutUrl = '/Order/Checkout?cartData=' + encodeURIComponent(cartJson);
    
    window.location.href = checkoutUrl;
}

/**
 * 返回菜單頁面修改訂單
 * 此函式會保留購物車內容，導航回指定餐廳的菜單頁面
 * @param {string} storeId 餐廳 ID
 */
function goBackToMenu(storeId) {
    if (!storeId) {
        console.error('餐廳 ID 未提供');
        return;
    }

    // 購物車資料保留在 Session Storage 中，不需要額外處理
    // Menu 頁面載入時會透過 CartStorage.initCart() 恢復購物車內容
    const menuUrl = '/Order/Menu?storeId=' + encodeURIComponent(storeId);
    
    window.location.href = menuUrl;
}

/**
 * HTML 轉義函式
 * @param {string} text 要轉義的文字
 * @returns {string} 轉義後的文字
 */
function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

/**
 * 更新訂單摘要 UI
 * 根據購物車內容即時更新訂單摘要區塊的顯示
 */
function updateOrderSummary() {
    const cart = CartStorage.getCart();
    const emptyDiv = document.getElementById('order-summary-empty');
    const itemsDiv = document.getElementById('order-summary-items');
    const itemsList = document.getElementById('order-items-list');
    const totalSpan = document.getElementById('order-total');
    const checkoutBtn = document.getElementById('btn-checkout');
    const itemCountSpan = document.getElementById('order-item-count');
    
    // 檢查必要的 DOM 元素是否存在
    if (!emptyDiv || !itemsDiv || !itemsList || !totalSpan || !checkoutBtn) {
        return;
    }
    
    // 更新項目數量顯示
    const totalItemCount = CartStorage.getItemCount();
    if (itemCountSpan) {
        itemCountSpan.textContent = totalItemCount + ' 項';
    }
    
    if (cart.items.length === 0) {
        // 購物車為空時顯示空狀態
        emptyDiv.classList.remove('d-none');
        itemsDiv.classList.add('d-none');
        totalSpan.textContent = formatCurrency(0);
        updateCheckoutButtonState(false);
        return;
    }
    
    // 購物車有項目時顯示項目列表
    emptyDiv.classList.add('d-none');
    itemsDiv.classList.remove('d-none');
    updateCheckoutButtonState(true);
    
    // 建立項目列表 HTML
    itemsList.innerHTML = '';
    cart.items.forEach(function(item) {
        const subtotal = Math.round(item.price * item.quantity * 100) / 100;
        const li = document.createElement('li');
        li.className = 'list-group-item';
        li.innerHTML = `
            <div class="d-flex justify-content-between align-items-start mb-2">
                <div class="fw-bold">${escapeHtml(item.name)}</div>
                <button type="button" class="btn btn-sm btn-outline-danger btn-remove-item" 
                        data-menu-item-id="${item.menuItemId}"
                        title="移除此項目">
                    <i class="bi bi-x"></i>
                </button>
            </div>
            <div class="d-flex justify-content-between align-items-center">
                <div class="d-flex align-items-center gap-1">
                    <button type="button" class="btn btn-sm btn-outline-secondary btn-summary-minus" 
                            data-menu-item-id="${item.menuItemId}"
                            title="減少數量">
                        <i class="bi bi-dash"></i>
                    </button>
                    <span class="badge bg-light text-dark mx-1" style="min-width: 30px;">${item.quantity}</span>
                    <button type="button" class="btn btn-sm btn-outline-secondary btn-summary-plus" 
                            data-menu-item-id="${item.menuItemId}"
                            title="增加數量">
                        <i class="bi bi-plus"></i>
                    </button>
                </div>
                <div class="text-end">
                    <small class="text-muted d-block">${formatCurrency(item.price)} × ${item.quantity}</small>
                    <span class="badge bg-primary">${formatCurrency(subtotal)}</span>
                </div>
            </div>
        `;
        itemsList.appendChild(li);
    });
    
    // 綁定刪除按鈕事件
    bindRemoveItemButtons();
    
    // 綁定訂單摘要中的數量調整按鈕
    bindSummaryQuantityButtons();
    
    // 更新總金額顯示
    totalSpan.textContent = formatCurrency(CartStorage.getTotalAmount());
}

/**
 * 綁定移除項目按鈕的事件處理
 */
function bindRemoveItemButtons() {
    document.querySelectorAll('.btn-remove-item').forEach(function(btn) {
        btn.addEventListener('click', function() {
            const menuItemId = this.dataset.menuItemId;
            CartStorage.removeItem(menuItemId);
            updateOrderSummary();
        });
    });
}

/**
 * 更新「前往結帳」按鈕狀態
 * @param {boolean} enabled 是否啟用按鈕
 */
function updateCheckoutButtonState(enabled) {
    const checkoutBtn = document.getElementById('btn-checkout');
    if (checkoutBtn) {
        checkoutBtn.disabled = !enabled;
        
        // 更新按鈕樣式以提供視覺回饋
        if (enabled) {
            checkoutBtn.classList.remove('btn-secondary');
            checkoutBtn.classList.add('btn-success');
        } else {
            checkoutBtn.classList.remove('btn-success');
            checkoutBtn.classList.add('btn-secondary');
        }
    }
}

/**
 * 綁定訂單摘要中數量調整按鈕的事件處理
 */
function bindSummaryQuantityButtons() {
    // 綁定減少數量按鈕
    document.querySelectorAll('.btn-summary-minus').forEach(function(btn) {
        btn.addEventListener('click', function() {
            const menuItemId = this.dataset.menuItemId;
            const cart = CartStorage.getCart();
            const item = cart.items.find(i => i.menuItemId === menuItemId);
            
            if (item) {
                if (item.quantity <= 1) {
                    // 數量為 1 時，移除項目
                    CartStorage.removeItem(menuItemId);
                } else {
                    // 減少數量
                    CartStorage.updateQuantity(menuItemId, item.quantity - 1);
                }
                updateOrderSummary();
            }
        });
    });
    
    // 綁定增加數量按鈕
    document.querySelectorAll('.btn-summary-plus').forEach(function(btn) {
        btn.addEventListener('click', function() {
            const menuItemId = this.dataset.menuItemId;
            const cart = CartStorage.getCart();
            const item = cart.items.find(i => i.menuItemId === menuItemId);
            
            if (item && item.quantity < 100) {
                CartStorage.updateQuantity(menuItemId, item.quantity + 1);
                updateOrderSummary();
            }
        });
    });
}

/**
 * 為了向後相容，保留 updateOrderSummaryUI 別名
 */
function updateOrderSummaryUI() {
    updateOrderSummary();
}

// ============================================================================
// 數量選擇與驗證功能 (User Story 3)
// ============================================================================

/**
 * 數量輸入驗證常數
 */
const QuantityValidation = {
    MIN_VALUE: 1,
    MAX_VALUE: 100,
    DEFAULT_VALUE: 1
};

/**
 * 驗證並修正數量值
 * @param {number|string} value 輸入的數量值
 * @returns {Object} 驗證結果 { isValid: boolean, value: number, message: string }
 */
function validateQuantity(value) {
    // 處理空值
    if (value === null || value === undefined || value === '') {
        return {
            isValid: false,
            value: QuantityValidation.DEFAULT_VALUE,
            message: '數量不能為空，已自動設為 1'
        };
    }

    // 轉換為數字
    const numValue = Number(value);

    // 檢查是否為有效數字
    if (isNaN(numValue)) {
        return {
            isValid: false,
            value: QuantityValidation.DEFAULT_VALUE,
            message: '數量必須為數字，已自動設為 1'
        };
    }

    // 檢查是否為正整數
    if (!Number.isInteger(numValue)) {
        const correctedValue = Math.round(numValue);
        const finalValue = Math.min(Math.max(correctedValue, QuantityValidation.MIN_VALUE), QuantityValidation.MAX_VALUE);
        return {
            isValid: false,
            value: finalValue,
            message: `數量必須為整數，已自動修正為 ${finalValue}`
        };
    }

    // 檢查是否小於最小值
    if (numValue < QuantityValidation.MIN_VALUE) {
        return {
            isValid: false,
            value: QuantityValidation.MIN_VALUE,
            message: `數量不能小於 ${QuantityValidation.MIN_VALUE}，已自動修正`
        };
    }

    // 檢查是否大於最大值
    if (numValue > QuantityValidation.MAX_VALUE) {
        return {
            isValid: false,
            value: QuantityValidation.MAX_VALUE,
            message: `數量不能超過 ${QuantityValidation.MAX_VALUE}，已自動修正`
        };
    }

    // 驗證通過
    return {
        isValid: true,
        value: numValue,
        message: ''
    };
}

/**
 * 自動修正無效數量並更新輸入框
 * @param {HTMLInputElement} input 數量輸入框元素
 * @returns {number} 修正後的數量值
 */
function autoCorrectQuantity(input) {
    if (!input) {
        console.error('數量輸入框元素不存在');
        return QuantityValidation.DEFAULT_VALUE;
    }

    const result = validateQuantity(input.value);
    
    // 更新輸入框的值
    input.value = result.value;

    // 如果無效，顯示提示訊息
    if (!result.isValid && result.message) {
        // 更新輸入框狀態
        showQuantityFeedback(input, result.message, false);
    } else {
        // 清除錯誤狀態
        clearQuantityFeedback(input);
    }

    return result.value;
}

/**
 * 顯示數量驗證回饋訊息
 * @param {HTMLInputElement} input 數量輸入框元素
 * @param {string} message 回饋訊息
 * @param {boolean} isSuccess 是否為成功訊息
 */
function showQuantityFeedback(input, message, isSuccess) {
    if (!input) return;

    // 移除之前的樣式
    input.classList.remove('is-valid', 'is-invalid');
    
    // 添加新的樣式
    input.classList.add(isSuccess ? 'is-valid' : 'is-invalid');

    // 取得或建立回饋元素
    let feedbackDiv = input.parentElement.querySelector('.quantity-feedback');
    if (!feedbackDiv) {
        feedbackDiv = document.createElement('div');
        feedbackDiv.className = 'quantity-feedback invalid-feedback';
        feedbackDiv.style.cssText = 'position: absolute; font-size: 0.75rem; white-space: nowrap;';
        input.parentElement.style.position = 'relative';
        input.parentElement.appendChild(feedbackDiv);
    }

    feedbackDiv.textContent = message;
    feedbackDiv.classList.remove('valid-feedback', 'invalid-feedback');
    feedbackDiv.classList.add(isSuccess ? 'valid-feedback' : 'invalid-feedback');
    feedbackDiv.style.display = 'block';

    // 3 秒後自動清除回饋
    setTimeout(function() {
        clearQuantityFeedback(input);
    }, 3000);
}

/**
 * 清除數量驗證回饋
 * @param {HTMLInputElement} input 數量輸入框元素
 */
function clearQuantityFeedback(input) {
    if (!input) return;

    input.classList.remove('is-valid', 'is-invalid');
    
    const feedbackDiv = input.parentElement.querySelector('.quantity-feedback');
    if (feedbackDiv) {
        feedbackDiv.style.display = 'none';
    }
}

/**
 * 減少數量
 * @param {string} menuItemId 菜品 ID
 * @returns {number} 更新後的數量
 */
function decreaseQuantity(menuItemId) {
    const input = document.getElementById('quantity-' + menuItemId);
    if (!input) {
        console.error('找不到數量輸入框: quantity-' + menuItemId);
        return QuantityValidation.DEFAULT_VALUE;
    }

    // 先驗證並修正當前值
    const currentValue = autoCorrectQuantity(input);
    
    // 計算新值
    const newValue = Math.max(QuantityValidation.MIN_VALUE, currentValue - 1);
    input.value = newValue;

    // 觸發 change 事件以便其他監聽器可以處理
    input.dispatchEvent(new Event('change', { bubbles: true }));

    return newValue;
}

/**
 * 增加數量
 * @param {string} menuItemId 菜品 ID
 * @returns {number} 更新後的數量
 */
function increaseQuantity(menuItemId) {
    const input = document.getElementById('quantity-' + menuItemId);
    if (!input) {
        console.error('找不到數量輸入框: quantity-' + menuItemId);
        return QuantityValidation.DEFAULT_VALUE;
    }

    // 先驗證並修正當前值
    const currentValue = autoCorrectQuantity(input);
    
    // 計算新值
    const newValue = Math.min(QuantityValidation.MAX_VALUE, currentValue + 1);
    input.value = newValue;

    // 如果已達上限，顯示提示
    if (newValue === QuantityValidation.MAX_VALUE && currentValue === QuantityValidation.MAX_VALUE) {
        showQuantityFeedback(input, `數量已達上限 ${QuantityValidation.MAX_VALUE}`, false);
    }

    // 觸發 change 事件以便其他監聯器可以處理
    input.dispatchEvent(new Event('change', { bubbles: true }));

    return newValue;
}

/**
 * 直接設定數量
 * @param {string} menuItemId 菜品 ID
 * @param {number|string} quantity 數量值
 * @returns {number} 驗證並修正後的數量
 */
function setQuantity(menuItemId, quantity) {
    const input = document.getElementById('quantity-' + menuItemId);
    if (!input) {
        console.error('找不到數量輸入框: quantity-' + menuItemId);
        return QuantityValidation.DEFAULT_VALUE;
    }

    input.value = quantity;
    return autoCorrectQuantity(input);
}

/**
 * 取得當前數量值
 * @param {string} menuItemId 菜品 ID
 * @returns {number} 當前數量值（已驗證）
 */
function getQuantity(menuItemId) {
    const input = document.getElementById('quantity-' + menuItemId);
    if (!input) {
        console.error('找不到數量輸入框: quantity-' + menuItemId);
        return QuantityValidation.DEFAULT_VALUE;
    }

    const result = validateQuantity(input.value);
    return result.value;
}

/**
 * 綁定數量輸入框的驗證事件
 * @param {HTMLInputElement} input 數量輸入框元素
 */
function bindQuantityInputValidation(input) {
    if (!input) return;

    // 輸入時即時驗證（只允許數字）
    input.addEventListener('input', function(e) {
        // 移除非數字字元
        const originalValue = this.value;
        const sanitizedValue = originalValue.replace(/[^0-9]/g, '');
        
        if (originalValue !== sanitizedValue) {
            this.value = sanitizedValue;
        }
    });

    // 失去焦點時進行完整驗證和修正
    input.addEventListener('blur', function(e) {
        autoCorrectQuantity(this);
    });

    // 變更時驗證
    input.addEventListener('change', function(e) {
        autoCorrectQuantity(this);
    });

    // 按下 Enter 鍵時驗證
    input.addEventListener('keydown', function(e) {
        if (e.key === 'Enter') {
            e.preventDefault();
            autoCorrectQuantity(this);
            this.blur();
        }
    });

    // 防止透過滾輪修改（避免意外修改）
    input.addEventListener('wheel', function(e) {
        if (document.activeElement === this) {
            e.preventDefault();
        }
    }, { passive: false });
}

/**
 * 綁定菜單頁面的數量控制按鈕
 * 此函式應在 DOMContentLoaded 後呼叫
 */
function initQuantityControls() {
    // 綁定減少數量按鈕
    document.querySelectorAll('.btn-quantity-minus').forEach(function(btn) {
        btn.addEventListener('click', function(e) {
            e.preventDefault();
            const menuItemId = this.dataset.menuItemId;
            decreaseQuantity(menuItemId);
        });
    });

    // 綁定增加數量按鈕
    document.querySelectorAll('.btn-quantity-plus').forEach(function(btn) {
        btn.addEventListener('click', function(e) {
            e.preventDefault();
            const menuItemId = this.dataset.menuItemId;
            increaseQuantity(menuItemId);
        });
    });

    // 綁定所有數量輸入框的驗證
    document.querySelectorAll('.quantity-input').forEach(function(input) {
        bindQuantityInputValidation(input);
    });
}

// ============================================================================
// 結帳表單驗證功能 (User Story 4)
// ============================================================================

/**
 * 表單驗證規則常數
 */
const CheckoutValidation = {
    /** 姓名最小長度 */
    NAME_MIN_LENGTH: 1,
    /** 姓名最大長度 */
    NAME_MAX_LENGTH: 50,
    /** 電話最小長度 */
    PHONE_MIN_LENGTH: 8,
    /** 電話最大長度 */
    PHONE_MAX_LENGTH: 15
};

/**
 * 驗證電話號碼格式
 * @param {string} phone 電話號碼
 * @returns {Object} 驗證結果 { isValid: boolean, message: string }
 */
function validatePhoneNumber(phone) {
    // 檢查空值
    if (!phone || phone.trim() === '') {
        return {
            isValid: false,
            message: '聯絡電話為必填欄位'
        };
    }

    const trimmedPhone = phone.trim();

    // 檢查是否僅包含數字
    if (!/^\d+$/.test(trimmedPhone)) {
        return {
            isValid: false,
            message: '電話號碼僅能輸入數字'
        };
    }

    // 檢查長度是否在有效範圍內
    if (trimmedPhone.length < CheckoutValidation.PHONE_MIN_LENGTH) {
        return {
            isValid: false,
            message: `電話號碼長度至少需要 ${CheckoutValidation.PHONE_MIN_LENGTH} 碼`
        };
    }

    if (trimmedPhone.length > CheckoutValidation.PHONE_MAX_LENGTH) {
        return {
            isValid: false,
            message: `電話號碼長度不能超過 ${CheckoutValidation.PHONE_MAX_LENGTH} 碼`
        };
    }

    return {
        isValid: true,
        message: ''
    };
}

/**
 * 驗證客戶姓名
 * @param {string} name 客戶姓名
 * @returns {Object} 驗證結果 { isValid: boolean, message: string }
 */
function validateCustomerName(name) {
    // 檢查空值
    if (!name || name.trim() === '') {
        return {
            isValid: false,
            message: '姓名為必填欄位'
        };
    }

    const trimmedName = name.trim();

    // 檢查長度是否超過限制
    if (trimmedName.length > CheckoutValidation.NAME_MAX_LENGTH) {
        return {
            isValid: false,
            message: `姓名長度不能超過 ${CheckoutValidation.NAME_MAX_LENGTH} 個字元`
        };
    }

    return {
        isValid: true,
        message: ''
    };
}

/**
 * 驗證結帳表單
 * @returns {Object} 驗證結果 { isValid: boolean, errors: { name: string, phone: string } }
 */
function validateCheckoutForm() {
    const nameInput = document.querySelector('input[name="CustomerName"]');
    const phoneInput = document.querySelector('input[name="CustomerPhone"]');
    
    const errors = {
        name: '',
        phone: ''
    };

    let isValid = true;

    // 驗證姓名
    if (nameInput) {
        const nameResult = validateCustomerName(nameInput.value);
        if (!nameResult.isValid) {
            isValid = false;
            errors.name = nameResult.message;
            setFieldValidationState(nameInput, false, nameResult.message);
        } else {
            setFieldValidationState(nameInput, true);
        }
    }

    // 驗證電話
    if (phoneInput) {
        const phoneResult = validatePhoneNumber(phoneInput.value);
        if (!phoneResult.isValid) {
            isValid = false;
            errors.phone = phoneResult.message;
            setFieldValidationState(phoneInput, false, phoneResult.message);
        } else {
            setFieldValidationState(phoneInput, true);
        }
    }

    return {
        isValid: isValid,
        errors: errors
    };
}

/**
 * 設定表單欄位的驗證狀態
 * @param {HTMLInputElement} input 輸入欄位
 * @param {boolean} isValid 是否有效
 * @param {string} message 錯誤訊息（可選）
 */
function setFieldValidationState(input, isValid, message) {
    if (!input) return;

    // 移除之前的驗證樣式
    input.classList.remove('is-valid', 'is-invalid');

    // 添加新的驗證樣式
    input.classList.add(isValid ? 'is-valid' : 'is-invalid');

    // 取得或建立回饋訊息元素
    const feedbackSelector = 'span[data-valmsg-for="' + input.name + '"]';
    let feedbackSpan = document.querySelector(feedbackSelector);
    
    // 如果找不到 asp-validation-for 產生的 span，嘗試找 .invalid-feedback 或建立一個
    if (!feedbackSpan) {
        feedbackSpan = input.parentElement.querySelector('.field-validation-error, .field-validation-valid, .invalid-feedback');
    }
    
    if (!feedbackSpan) {
        feedbackSpan = document.createElement('span');
        feedbackSpan.className = 'invalid-feedback';
        feedbackSpan.setAttribute('data-valmsg-for', input.name);
        input.parentElement.appendChild(feedbackSpan);
    }

    // 更新回饋訊息
    if (!isValid && message) {
        feedbackSpan.textContent = message;
        feedbackSpan.className = 'text-danger invalid-feedback d-block';
    } else if (isValid) {
        feedbackSpan.textContent = '';
        feedbackSpan.className = 'valid-feedback';
    }
}

/**
 * 清除表單欄位的驗證狀態
 * @param {HTMLInputElement} input 輸入欄位
 */
function clearFieldValidationState(input) {
    if (!input) return;

    input.classList.remove('is-valid', 'is-invalid');

    const feedbackSelector = 'span[data-valmsg-for="' + input.name + '"]';
    const feedbackSpan = document.querySelector(feedbackSelector);
    
    if (feedbackSpan) {
        feedbackSpan.textContent = '';
        feedbackSpan.className = '';
    }
}

/**
 * 初始化結帳表單驗證
 * 此函式應在結帳頁面載入後呼叫
 */
function initCheckoutFormValidation() {
    const form = document.getElementById('checkout-form');
    const nameInput = document.querySelector('input[name="CustomerName"]');
    const phoneInput = document.querySelector('input[name="CustomerPhone"]');

    if (!form) return;

    // 電話輸入即時過濾非數字
    if (phoneInput) {
        phoneInput.addEventListener('input', function() {
            // 移除非數字字元
            const originalValue = this.value;
            const sanitizedValue = originalValue.replace(/[^0-9]/g, '');
            
            if (originalValue !== sanitizedValue) {
                this.value = sanitizedValue;
            }
        });

        // 失去焦點時驗證
        phoneInput.addEventListener('blur', function() {
            const result = validatePhoneNumber(this.value);
            setFieldValidationState(this, result.isValid, result.message);
        });
    }

    // 姓名失去焦點時驗證
    if (nameInput) {
        nameInput.addEventListener('blur', function() {
            const result = validateCustomerName(this.value);
            setFieldValidationState(this, result.isValid, result.message);
        });
    }

    // 表單提交時驗證
    form.addEventListener('submit', function(e) {
        const validationResult = validateCheckoutForm();
        
        if (!validationResult.isValid) {
            e.preventDefault();
            
            // 聚焦到第一個錯誤欄位
            if (validationResult.errors.name && nameInput) {
                nameInput.focus();
            } else if (validationResult.errors.phone && phoneInput) {
                phoneInput.focus();
            }
            
            return false;
        }
        
        return true;
    });
}

/**
 * 訂單成功後清除 Session Storage
 * 應在訂單確認頁面呼叫此函式
 */
function clearCartAfterOrderSuccess() {
    try {
        CartStorage.clearCart();
        console.log('訂單成功，購物車已清除');
        return true;
    } catch (error) {
        console.error('清除購物車時發生錯誤:', error);
        return false;
    }
}

/**
 * 檢查是否在訂單確認頁面並自動清除購物車
 * 此函式會檢查 URL 是否包含 Confirmation 路徑
 */
function checkAndClearCartOnConfirmation() {
    const currentPath = window.location.pathname.toLowerCase();
    if (currentPath.includes('/order/confirmation')) {
        clearCartAfterOrderSuccess();
    }
}

/**
 * 頁面載入時的初始化邏輯
 */
document.addEventListener('DOMContentLoaded', function() {
    // 初始化數量控制（如果在菜單頁面）
    if (document.querySelector('.quantity-input')) {
        initQuantityControls();
    }

    // 初始化結帳表單驗證（如果在結帳頁面）
    if (document.getElementById('checkout-form')) {
        initCheckoutFormValidation();
    }

    // 檢查是否在確認頁面並清除購物車
    checkAndClearCartOnConfirmation();
});
