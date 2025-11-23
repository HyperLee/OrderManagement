// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// ==========================================
// 動態菜單項目管理 (User Story 1 - T028)
// ==========================================
$(document).ready(function () {
    let menuItemIndex = $('#menuItemsContainer .menu-item-row').length;
    const maxMenuItems = 20;
    const minMenuItems = 1;

    // 新增菜單項目按鈕點擊事件
    $('#addMenuItemBtn').on('click', function () {
        if (menuItemIndex >= maxMenuItems) {
            alert(`已達菜單項目上限 (${maxMenuItems} 筆)！`);
            return;
        }

        const newMenuItem = `
            <div class="menu-item-row mb-3 p-3 border rounded" data-index="${menuItemIndex}">
                <div class="row">
                    <div class="col-md-4">
                        <label class="form-label">
                            菜名 <span class="text-danger">*</span>
                        </label>
                        <input name="MenuItems[${menuItemIndex}].Name" class="form-control" 
                               placeholder="例如：排骨便當" maxlength="50" required />
                        <span class="text-danger field-validation-valid" 
                              data-valmsg-for="MenuItems[${menuItemIndex}].Name" 
                              data-valmsg-replace="true"></span>
                    </div>
                    <div class="col-md-3">
                        <label class="form-label">
                            價格 (元) <span class="text-danger">*</span>
                        </label>
                        <input name="MenuItems[${menuItemIndex}].Price" class="form-control" 
                               type="number" min="0" step="1" placeholder="例如：80" required />
                        <span class="text-danger field-validation-valid" 
                              data-valmsg-for="MenuItems[${menuItemIndex}].Price" 
                              data-valmsg-replace="true"></span>
                    </div>
                    <div class="col-md-4">
                        <label class="form-label">
                            描述 (選填)
                        </label>
                        <input name="MenuItems[${menuItemIndex}].Description" class="form-control" 
                               placeholder="例如：香酥排骨配三菜一飯" maxlength="200" />
                        <span class="text-danger field-validation-valid" 
                              data-valmsg-for="MenuItems[${menuItemIndex}].Description" 
                              data-valmsg-replace="true"></span>
                    </div>
                    <div class="col-md-1 d-flex align-items-end">
                        <button type="button" class="btn btn-danger btn-sm remove-menu-item-btn">
                            <i class="bi bi-trash"></i>
                        </button>
                    </div>
                </div>
            </div>
        `;

        $('#menuItemsContainer').append(newMenuItem);
        menuItemIndex++;
        updateRemoveButtonsState();

        // 重新初始化驗證 (如果使用 jQuery Validation)
        if (typeof $.validator !== 'undefined') {
            var form = $('#createStoreForm');
            form.removeData('validator');
            form.removeData('unobtrusiveValidation');
            $.validator.unobtrusive.parse(form);
        }
    });

    // 移除菜單項目按鈕點擊事件 (使用事件委派)
    $('#menuItemsContainer').on('click', '.remove-menu-item-btn', function () {
        const currentCount = $('#menuItemsContainer .menu-item-row').length;

        if (currentCount <= minMenuItems) {
            alert(`至少需要保留 ${minMenuItems} 個菜單項目！`);
            return;
        }

        $(this).closest('.menu-item-row').remove();
        updateRemoveButtonsState();
    });

    // 更新移除按鈕狀態（只有一個項目時禁用移除按鈕）
    function updateRemoveButtonsState() {
        const currentCount = $('#menuItemsContainer .menu-item-row').length;
        const removeButtons = $('.remove-menu-item-btn');

        if (currentCount <= minMenuItems) {
            removeButtons.prop('disabled', true).addClass('disabled');
        } else {
            removeButtons.prop('disabled', false).removeClass('disabled');
        }

        // 更新新增按鈕狀態
        if (currentCount >= maxMenuItems) {
            $('#addMenuItemBtn').prop('disabled', true);
        } else {
            $('#addMenuItemBtn').prop('disabled', false);
        }
    }

    // 初始化時更新按鈕狀態
    updateRemoveButtonsState();
});

// ==========================================
// 防重複提交機制 (User Story 1 - T029)
// ==========================================
$(document).ready(function () {
    const form = $('#createStoreForm');
    const submitBtn = $('#submitBtn');
    let isSubmitting = false;

    form.on('submit', function (e) {
        // 檢查是否正在提交
        if (isSubmitting) {
            e.preventDefault();
            return false;
        }

        // 執行表單驗證
        if (!form.valid()) {
            return false;
        }

        // 標記為正在提交
        isSubmitting = true;

        // 禁用提交按鈕並顯示載入狀態
        submitBtn.prop('disabled', true)
            .html('<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>處理中...');

        // 設定逾時保護（10 秒後自動恢復按鈕）
        setTimeout(function () {
            if (isSubmitting) {
                isSubmitting = false;
                submitBtn.prop('disabled', false)
                    .html('<i class="bi bi-check-circle"></i> 確認新增');
            }
        }, 10000);

        return true;
    });
});
