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
        // 更新 UI（如果有 updateOrderSummaryUI 函式）
        if (typeof updateOrderSummaryUI === 'function') {
            updateOrderSummaryUI();
        }
        
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
