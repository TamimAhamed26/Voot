// Global variables
var productModal, deleteModal, categoryModal, variantModal, attributeModal, appToast, discountModal, imageModal;
var categoryCache = [];
var allAttributesCache = [];
var cropper = null;
var cropMode = false;
var tempSelectedAttributes = [];

function getCsrfToken() {
    return $('input[name="__RequestVerificationToken"]').val();
}

$(function () {
    // Initialize Modals
    productModal = new bootstrap.Modal(document.getElementById('productModal'));
    deleteModal = new bootstrap.Modal(document.getElementById('deleteModal'));
    appToast = new bootstrap.Toast(document.getElementById('appToast'));
    categoryModal = new bootstrap.Modal(document.getElementById('categoryModal'));
    variantModal = new bootstrap.Modal(document.getElementById('variantModal'));
    attributeModal = new bootstrap.Modal(document.getElementById('attributeModal'));
    discountModal = new bootstrap.Modal(document.getElementById('discountModal'));
    imageModal = new bootstrap.Modal(document.getElementById('imageUploadModal'));

    // Handle modal backdrop cleanup
    $('#categoryModal').on('hidden.bs.modal', function () {
        if ($('.modal.show').length > 0) $('body').addClass('modal-open');
    });

    $('#attributeModal').on('hidden.bs.modal', function () {
        if ($('.modal.show').length > 0) $('body').addClass('modal-open');
    });

    $('#imageUploadModal').on('hidden.bs.modal', function () {
        cancelCrop();
        $("#imageUploadForm")[0].reset();
        if ($('.modal.show').length > 0) $('body').addClass('modal-open');
    });

    $('#IsVariantBased').on('change', function () {
        toggleManageAttributesButton();
    });

    // Image Handling Events
    $("#btnToggleCrop").on('click', function () {
        cropMode = !cropMode;
        $("#cropStatus").text(cropMode ? "ON" : "OFF");
        $(this).toggleClass("btn-outline-secondary btn-success");
        $("#imageFiles").val('');
        cancelCrop();
    });

    $("#imageFiles").on('change', function () {
        if (this.files && this.files.length > 0) {
            if (cropMode) {
                startCropper(this.files[0]);
            }
        }
    });

    $("#imageUploadForm").on('submit', function (e) {
        e.preventDefault();
        if (!cropMode) uploadImagesBulk();
    });

    // Initial Data Load
    loadProducts();
    loadCategories();
    loadAllAttributes();

    // Form Submissions
    $("#productForm").on('submit', function (e) { e.preventDefault(); saveProduct(); });
    $("#btnConfirmDelete").on('click', function () { deleteProduct($(this).data('id')); });
    $("#categoryForm").on('submit', function (e) { e.preventDefault(); saveCategory(); });
    $("#variantForm").on('submit', function (e) { e.preventDefault(); saveVariant(); });
    $("#discountForm").on('submit', function (e) { e.preventDefault(); saveDiscount(); });
    $("#attributeAddForm").on('submit', function (e) { e.preventDefault(); saveProductAttribute(); });
});

// ==========================================
// IMAGE MANAGEMENT LOGIC
// ==========================================

function openProductImageModal(productId) {
    configureImageModal("Product", productId, "Product Images");
}

function openVariantImageModal(variantId) {
    configureImageModal("Variant", variantId, "Variant Images");
}

function configureImageModal(type, id, title) {
    $("#imgModalTitle").text(title);
    $("#imageUploadForm")[0].reset();
    $("#img_Type").val(type);
    $("#img_ParentId").val(id);
    $("#existingImagesContainer").empty();
    $("#noImagesMessage").addClass('d-none');
    $("#existingImagesLoader").removeClass('d-none');

    cropMode = false;
    $("#btnToggleCrop").removeClass("btn-success").addClass("btn-outline-secondary");
    $("#cropStatus").text("OFF");
    cancelCrop();

    imageModal.show();
    loadExistingImages(type, id);
}

function loadExistingImages(type, id) {
    var url = type === "Product" ? "/AdminProduct/GetProduct?id=" + id : "/AdminProduct/GetVariant?id=" + id;

    $.ajax({
        url: url,
        type: "GET",
        success: function (response) {
            $("#existingImagesLoader").addClass('d-none');
            if (response.success && response.images) {
                renderImageGrid(response.images, type);
            } else {
                $("#noImagesMessage").removeClass('d-none');
            }
        },
        error: function () {
            $("#existingImagesLoader").addClass('d-none');
            showToast("Error", "Failed to load images", true);
        }
    });
}

function renderImageGrid(images, type) {
    var container = $("#existingImagesContainer");
    container.empty();

    if (!images || images.length === 0) {
        $("#noImagesMessage").removeClass('d-none');
        return;
    }

    $("#noImagesMessage").addClass('d-none');

    images.sort((a, b) => {
        var orderA = type === "Product" ? (a.sortOrder || 0) : (a.displayOrder || 0);
        var orderB = type === "Product" ? (b.sortOrder || 0) : (b.displayOrder || 0);
        return orderA - orderB;
    });

    images.forEach((img, index) => {
        var deleteFn = type === "Product" ? `deleteProductImage(${img.id})` : `deleteVariantImage(${img.id})`;
        var imgUrl = `/images/products/${img.imageUrl}`;

        var primaryBtn = '';
        if (type === "Product") {
            var primaryClass = img.isPrimary ? "btn-success" : "btn-outline-secondary";
            primaryBtn = `
                <button class="btn ${primaryClass} btn-sm w-100 mb-1"
                    onclick="setPrimaryProductImage(${img.id})">
                    <i class="bi bi-star${img.isPrimary ? '-fill' : ''}"></i> Primary
                </button>`;
        }

        var orderField = type === "Product" ? img.sortOrder : img.displayOrder;
        var primaryBadge = (type === "Product" && img.isPrimary)
            ? '<span class="primary-badge badge bg-success"><i class="bi bi-star-fill"></i> Primary</span>'
            : '';

        var html = `
<div class="col-6 col-md-3 col-lg-2 text-center image-card-container" data-image-id="${img.id}">
    <div class="card h-100 border shadow-sm position-relative">
        <div class="ratio ratio-1x1 position-relative">
            ${primaryBadge}
            <span class="order-badge">#${orderField}</span>
            <img src="${imgUrl}" class="card-img-top object-fit-cover" alt="Image" 
                onerror="this.onerror=null; this.src='data:image/svg+xml,%3Csvg xmlns=%22http://www.w3.org/2000/svg%22 width=%22200%22 height=%22200%22%3E%3Crect fill=%22%23ddd%22 width=%22200%22 height=%22200%22/%3E%3Ctext fill=%22%23999%22 x=%2250%25%22 y=%2250%25%22 text-anchor=%22middle%22 dy=%22.3em%22%3ENo Image%3C/text%3E%3C/svg%3E';">
        </div>
        <div class="card-body p-2">
            ${primaryBtn}
            
            <div class="input-group input-group-sm mb-1">
                <span class="input-group-text">Order</span>
                <input type="number" class="form-control form-control-sm" 
                    value="${orderField}" 
                    min="0"
                    onchange="updateImageOrder(${img.id}, this.value, '${type}')">
            </div>

            <div class="btn-group w-100 mb-1">
                <button class="btn btn-outline-secondary btn-sm" 
                    onclick="moveImage(${img.id}, 'up', '${type}')" 
                    title="Move Up"
                    ${index === 0 ? 'disabled' : ''}>
                    <i class="bi bi-arrow-up"></i>
                </button>
                <button class="btn btn-outline-secondary btn-sm" 
                    onclick="moveImage(${img.id}, 'down', '${type}')" 
                    title="Move Down"
                    ${index === images.length - 1 ? 'disabled' : ''}>
                    <i class="bi bi-arrow-down"></i>
                </button>
            </div>

            <button class="btn btn-danger btn-sm w-100" onclick="${deleteFn}">
                <i class="bi bi-trash"></i> Delete
            </button>
        </div>
    </div>
</div>`;

        container.append(html);
    });
}

function setPrimaryProductImage(imageId) {
    var productId = $("#img_ParentId").val();

    $.ajax({
        url: "/AdminProduct/SetPrimaryProductImage",
        type: "POST",
        data: { imageId: imageId },
        headers: { 'RequestVerificationToken': getCsrfToken() },
        success: function (response) {
            if (response.success) {
                showToast("Success", "Primary image updated");
                loadExistingImages("Product", productId);
            } else {
                showToast("Error", response.message, true);
            }
        },
        error: function (xhr) {
            showToast("Error", xhr.responseJSON?.message || "Failed to set primary image", true);
        }
    });
}

function updateImageOrder(imageId, newOrder, type) {
    var url = type === "Product"
        ? "/AdminProduct/UpdateProductImageOrder"
        : "/AdminProduct/UpdateVariantImageOrder";

    var parentId = $("#img_ParentId").val();

    $.ajax({
        url: url,
        type: "POST",
        data: { imageId: imageId, order: parseInt(newOrder) },
        headers: { 'RequestVerificationToken': getCsrfToken() },
        success: function (response) {
            if (response.success) {
                showToast("Success", "Order updated");
                loadExistingImages(type, parentId);
            } else {
                showToast("Error", response.message, true);
            }
        },
        error: function (xhr) {
            showToast("Error", xhr.responseJSON?.message || "Failed to update order", true);
        }
    });
}

function moveImage(imageId, direction, type) {
    var container = $("#existingImagesContainer");
    var currentCard = container.find(`[data-image-id="${imageId}"]`);

    if (direction === 'up') {
        var prevCard = currentCard.prev();
        if (prevCard.length) {
            currentCard.insertBefore(prevCard);
        }
    } else {
        var nextCard = currentCard.next();
        if (nextCard.length) {
            currentCard.insertAfter(nextCard);
        }
    }

    saveCurrentDisplayOrder(type);
}

function saveCurrentDisplayOrder(type) {
    var parentId = $("#img_ParentId").val();
    var imageIds = [];

    $("#existingImagesContainer [data-image-id]").each(function (index) {
        imageIds.push({
            id: $(this).data('image-id'),
            order: index + 1
        });
    });

    var url = type === "Product"
        ? "/AdminProduct/UpdateProductImagesOrder"
        : "/AdminProduct/UpdateVariantImagesOrder";

    $.ajax({
        url: url,
        type: "POST",
        data: JSON.stringify({ images: imageIds }),
        contentType: 'application/json',
        headers: { 'RequestVerificationToken': getCsrfToken() },
        success: function (response) {
            if (response.success) {
                showToast("Success", "Display order updated");
            }
        },
        error: function (xhr) {
            showToast("Error", "Failed to update display order", true);
            loadExistingImages(type, parentId);
        }
    });
}

function deleteProductImage(id) {
    if (!confirm("Delete this image?")) return;
    $.ajax({
        url: "/AdminProduct/DeleteProductImage",
        type: "POST",
        data: { imageId: id },
        headers: { 'RequestVerificationToken': getCsrfToken() },
        success: function (response) {
            if (response.success) {
                var productId = $("#img_ParentId").val();
                showToast("Success", "Image deleted");
                loadExistingImages("Product", productId);
            } else {
                showToast("Error", response.message, true);
            }
        }
    });
}

function deleteVariantImage(id) {
    if (!confirm("Delete this image?")) return;
    $.ajax({
        url: "/AdminProduct/DeleteVariantImage",
        type: "POST",
        data: { imageId: id },
        headers: { 'RequestVerificationToken': getCsrfToken() },
        success: function (response) {
            if (response.success) {
                var variantId = $("#img_ParentId").val();
                showToast("Success", "Image deleted");
                loadExistingImages("Variant", variantId);
            } else {
                showToast("Error", response.message, true);
            }
        }
    });
}

function startCropper(file) {
    var reader = new FileReader();
    reader.onload = function (e) {
        $("#cropperImage").attr('src', e.target.result);
        $("#uploadContainer").addClass('d-none');
        $("#cropperContainer").removeClass('d-none');

        if (cropper) cropper.destroy();

        var image = document.getElementById('cropperImage');
        cropper = new Cropper(image, {
            viewMode: 1,
            autoCropArea: 1,
        });
    };
    reader.readAsDataURL(file);
}

function cancelCrop() {
    $("#cropperContainer").addClass('d-none');
    $("#uploadContainer").removeClass('d-none');
    $("#imageFiles").val('');
    if (cropper) {
        cropper.destroy();
        cropper = null;
    }
}

function uploadCroppedImage() {
    if (!cropper) return;

    var w = parseInt($("#cropWidth").val()) || undefined;
    var h = parseInt($("#cropHeight").val()) || undefined;

    var canvas = cropper.getCroppedCanvas({ width: w, height: h });
    if (!canvas) return;

    canvas.toBlob(function (blob) {
        var formData = new FormData();
        var type = $("#img_Type").val();
        var id = $("#img_ParentId").val();
        var idParam = type === "Product" ? "productId" : "variantId";

        formData.append(idParam, id);
        formData.append("files", blob, "cropped-image.jpg");

        performUpload(formData, type, id);
    }, 'image/jpeg', 0.9);
}

function uploadImagesBulk() {
    var type = $("#img_Type").val();
    var id = $("#img_ParentId").val();
    var idParam = type === "Product" ? "productId" : "variantId";

    var formData = new FormData();
    formData.append(idParam, id);

    var files = $('#imageFiles')[0].files;
    if (files.length === 0) {
        alert("Please select a file first.");
        return;
    }

    for (var i = 0; i < files.length; i++) {
        formData.append("files", files[i]);
    }

    performUpload(formData, type, id);
}

function performUpload(formData, type, id) {
    var url = type === "Product"
        ? "/AdminProduct/UploadProductImages"
        : "/AdminProduct/UploadVariantImages";

    var uploadBtn = $("#btnDirectUpload");
    uploadBtn.prop("disabled", true)
        .html('<span class="spinner-border spinner-border-sm"></span> Uploading...');

    $.ajax({
        url: url,
        type: "POST",
        data: formData,
        processData: false,
        contentType: false,
        headers: { 'RequestVerificationToken': getCsrfToken() },
        success: function (response) {
            uploadBtn.prop("disabled", false)
                .html('<i class="bi bi-cloud-upload"></i> Upload');

            if (response.success) {
                showToast("Success", "Uploaded successfully.");
                $("#imageFiles").val('');
                cancelCrop();
                loadExistingImages(type, id);
            } else {
                showToast("Error", response.message, true);
            }
        },
        error: function (xhr) {
            uploadBtn.prop("disabled", false)
                .html('<i class="bi bi-cloud-upload"></i> Upload');
            showToast("Error", "Upload failed: " + xhr.statusText, true);
        }
    });
}

// ==========================================
// CRUD & BUSINESS LOGIC
// ==========================================

function showCreateModal() {
    $("#productForm")[0].reset();
    $("#productModalLabel").text("Add New Product");
    $("#Id").val("");
    tempSelectedAttributes = [];
    renderTempAttributes();
    $("#modalErrorAlert").addClass('d-none');
    $("#IsActive").prop('checked', true);
    $('#manageAttributesContainer').hide();
    populateCategoryDropdown(categoryCache);
    productModal.show();
}

function showEditModal(id) {
    $("#productForm")[0].reset();
    $("#productModalLabel").text("Edit Product");
    $("#modalErrorAlert").addClass('d-none');

    $.ajax({
        url: "/AdminProduct/GetProduct?id=" + id,
        type: "GET",
        success: function (response) {
            if (response.success) {
                var p = response.product;
                $("#Id").val(p.id);
                $("#ProductName").val(p.productName);
                $("#Slug").val(p.slug);
                $("#Description").val(p.description);
                $("#BasePrice").val(p.basePrice);
                $("#Barcode").val(p.barcode);
                $("#ReorderLevel").val(p.reorderLevel);
                populateCategoryDropdown(categoryCache, p.categoryId);
                $("#IsVariantBased").prop('checked', p.isVariantBased);
                $("#IsActive").prop('checked', p.isActive);
                toggleManageAttributesButton();
                productModal.show();
            }
        },
        error: function (xhr) {
            showToast("Error", "Could not load product. " + xhr.responseJSON?.message, true);
        }
    });
}

function saveProduct() {
    var isCreate = $("#Id").val() === "";
    var url = isCreate ? "/AdminProduct/Create" : "/AdminProduct/Edit";
    var formData = $("#productForm").serializeArray();

    if (!$("#IsVariantBased").is(":checked")) formData.push({ name: "IsVariantBased", value: "false" });
    if (!$("#IsActive").is(":checked")) formData.push({ name: "IsActive", value: "false" });

    if (isCreate && $("#IsVariantBased").is(":checked") && tempSelectedAttributes.length > 0) {
        tempSelectedAttributes.forEach(attr => {
            formData.push({ name: "SelectedAttributeIds", value: attr.id });
        });
    }

    $.ajax({
        url: url,
        type: "POST",
        data: $.param(formData),
        headers: { 'RequestVerificationToken': getCsrfToken() },
        success: function (response) {
            if (response.success) {
                productModal.hide();
                showToast("Success", "Product saved successfully.");
                loadProducts();
            }
        },
        error: function (xhr) {
            $("#modalErrorAlert").text("An error occurred. " + (xhr.responseJSON?.message || "Check console.")).removeClass('d-none');
        }
    });
}

function toggleManageAttributesButton() {
    var isVariant = $('#IsVariantBased').is(':checked');
    var productId = $("#Id").val();

    if (isVariant) {
        $('#manageAttributesContainer').slideDown(200);

        if (productId) {
            $("#editModeAttributes").removeClass('d-none');
            $("#createModeAttributes").addClass('d-none');
        } else {
            $("#editModeAttributes").addClass('d-none');
            $("#createModeAttributes").removeClass('d-none');

            if ($("#createAttributeSelect option").length <= 1) {
                var dropdown = $("#createAttributeSelect");
                allAttributesCache.forEach(attr => {
                    dropdown.append(`<option value="${attr.id}" data-name="${escapeHTML(attr.name)}">${escapeHTML(attr.name)}</option>`);
                });
            }
        }
    } else {
        $('#manageAttributesContainer').slideUp(200);
    }
}

function addTempAttribute() {
    var select = $("#createAttributeSelect");
    var id = select.val();
    var name = select.find('option:selected').data('name');

    if (!id) return;

    if (tempSelectedAttributes.some(x => x.id == id)) {
        alert("Attribute already added.");
        return;
    }

    tempSelectedAttributes.push({ id: id, name: name });
    renderTempAttributes();
    select.val("");
}

function removeTempAttribute(id) {
    tempSelectedAttributes = tempSelectedAttributes.filter(x => x.id != id);
    renderTempAttributes();
}

function renderTempAttributes() {
    var list = $("#tempAttributeList");
    list.empty();

    if (tempSelectedAttributes.length === 0) {
        list.append('<li class="list-group-item bg-transparent text-muted fst-italic p-1">No attributes selected</li>');
        return;
    }

    tempSelectedAttributes.forEach(attr => {
        list.append(`
            <li class="list-group-item bg-transparent d-flex justify-content-between align-items-center p-1">
                <span>${escapeHTML(attr.name)}</span>
                <button type="button" class="btn btn-xs btn-link text-danger p-0" onclick="removeTempAttribute(${attr.id})">
                    <i class="bi bi-x-circle"></i>
                </button>
            </li>
        `);
    });
}

function loadProducts() {
    var tableBody = $("#productTableBody");
    tableBody.html('<tr><td colspan="7" class="text-center"><div class="spinner-border spinner-border-sm"></div> Loading products...</td></tr>');

    $.ajax({
        url: "/AdminProduct/GetProducts",
        type: "GET",
        success: function (response) {
            tableBody.empty();
            if (response.success && response.products.length > 0) {
                response.products.forEach(p => tableBody.append(renderProductRow(p)));
            } else {
                tableBody.append('<tr><td colspan="7" class="text-center">No products found. Click "Add New Product" to begin.</td></tr>');
            }
        },
        error: function (xhr) {
            var msg = xhr.responseJSON?.message || "An unknown error occurred.";
            tableBody.empty().append(`<tr><td colspan="7" class="text-center text-danger">Error loading products: ${msg}</td></tr>`);
            showToast("Error", "Could not load products. " + msg, true);
        }
    });
}

function deleteProduct(id) {
    $.ajax({
        url: "/AdminProduct/DeleteProduct",
        type: "POST",
        data: { id: id },
        headers: { 'RequestVerificationToken': getCsrfToken() },
        success: function (response) {
            if (response.success) {
                deleteModal.hide();
                showToast("Success", "Product deleted successfully.");
                $("#row-" + id).fadeOut(500, function () {
                    $(this).remove();
                    if ($("#productTableBody tr").length === 0) {
                        $("#productTableBody").append('<tr><td colspan="7" class="text-center">No products found.</td></tr>');
                    }
                });
            }
        },
        error: function (xhr) {
            $("#deleteErrorAlert").text("An error occurred. " + xhr.responseJSON?.message).removeClass('d-none');
        }
    });
}

function saveCategory() {
    var categoryName = $("#categoryName").val();
    $.ajax({
        url: "/AdminProduct/CreateCategory",
        type: "POST",
        data: { categoryName: categoryName },
        headers: { 'RequestVerificationToken': getCsrfToken() },
        success: function (response) {
            if (response.success) {
                categoryCache.push(response.newCategory);
                populateCategoryDropdown(categoryCache, response.newCategory.id);
                categoryModal.hide();
                showToast("Success", "Category created.");
            }
        },
        error: function (xhr) {
            $("#categoryErrorAlert").text(xhr.responseJSON?.message).removeClass('d-none');
        }
    });
}

function loadCategories() {
    if (categoryCache.length > 0) {
        populateCategoryDropdown(categoryCache);
        return;
    }
    $.ajax({
        url: "/AdminProduct/GetCategories",
        type: "GET",
        success: function (response) {
            if (response.success) {
                categoryCache = response.categories;
                populateCategoryDropdown(categoryCache);
            }
        },
        error: function () {
            $("#CategoryId").html('<option value="">Error loading categories</option>');
        }
    });
}

function loadAllAttributes() {
    $.ajax({
        url: "/AdminProduct/GetAvailableAttributes",
        type: "GET",
        success: function (response) {
            if (response.success) allAttributesCache = response.attributes;
        }
    });
}

function showCategoryModal() {
    $("#categoryForm")[0].reset();
    $("#categoryErrorAlert").addClass('d-none');
    categoryModal.show();
}

function showDeleteModal(id, name) {
    $("#deleteProductName").text(name);
    $("#btnConfirmDelete").data('id', id);
    $("#deleteErrorAlert").addClass('d-none');
    deleteModal.show();
}

function populateCategoryDropdown(categories, selectedId) {
    var dropdown = $("#CategoryId");
    dropdown.empty().append('<option value="">Select Category</option>');
    categories.forEach(cat => {
        var selected = (cat.id === selectedId) ? "selected" : "";
        dropdown.append(`<option value="${cat.id}" ${selected}>${escapeHTML(cat.name)}</option>`);
    });
}

// VARIANT, ATTRIBUTE, DISCOUNT FUNCTIONS CONTINUE...
// (Rest of your functions remain the same - I'll include them for completeness)

function saveVariant() {
    var isCreate = $("#v_VariantId").val() === "";
    var url = isCreate ? "/AdminProduct/CreateVariant" : "/AdminProduct/UpdateVariant";

    var formData = {
        Id: $("#v_VariantId").val() || 0,
        ProductId: $("#v_ProductId").val(),
        SKU: $("#v_SKU").val(),
        VariantPrice: $("#v_VariantPrice").val(),
        IsActive: $("#v_IsActive").val() === "true",
        Price: $("#v_Price").val() || 0,
        CompareAtPrice: $("#v_CompareAtPrice").val() || null,
        CostPrice: $("#v_CostPrice").val() || null,
        StockQty: $("#v_StockQty").val() || 0,
        TrackInventory: $("#v_TrackInventory").is(":checked"),
        AllowBackorder: $("#v_AllowBackorder").is(":checked"),
        WeightGrams: $("#v_WeightGrams").val() || null,
        SelectedAttributeValueIds: []
    };

    var selectedTexts = [];
    var allSelected = true;

    $(".dynamic-attr-select").each(function () {
        var valueId = $(this).val();
        if (valueId) {
            formData.SelectedAttributeValueIds.push(parseInt(valueId));
            selectedTexts.push($(this).find('option:selected').text());
        } else {
            allSelected = false;
        }
    });

    if (!allSelected && $(".dynamic-attr-select").length > 0) {
        $("#variantErrorAlert").html("Please select a value for all attributes.").removeClass('d-none');
        return;
    }

    formData.VariantName = `${$("#variantModal").data("productName")} - ${selectedTexts.join(" / ")}`;

    $.ajax({
        url: url,
        type: "POST",
        data: formData,
        headers: { 'RequestVerificationToken': getCsrfToken() },
        success: function (response) {
            if (response.success) {
                clearVariantForm();
                showToast("Success", "Variant saved.");
                $("#variantErrorAlert").addClass('d-none');
                loadVariantsTable($("#v_ProductId").val());
            }
        },
        error: function (xhr) {
            var msg = "An unknown error occurred.";
            if (xhr.responseJSON) {
                msg = `<strong>${xhr.responseJSON.message}</strong>`;
                if (xhr.responseJSON.errors) {
                    var list = "<ul>";
                    for (var key in xhr.responseJSON.errors) {
                        if (key.includes("SelectedAttributeValueIds")) {
                            list += `<li>Attributes: Please select a value for all options.</li>`;
                        } else {
                            xhr.responseJSON.errors[key].forEach(err => list += `<li>${escapeHTML(key)}: ${escapeHTML(err)}</li>`);
                        }
                    }
                    msg += list + "</ul>";
                }
            }
            $("#variantErrorAlert").html(msg).removeClass('d-none');
        }
    });
}

function editVariant(id) {
    var container = $("#dynamic-variant-form-container");
    container.html('<div class="col-12 text-center">Loading options...</div>');
    var productId = $("#v_ProductId").val();

    $.ajax({
        url: "/AdminProduct/GetVariant?id=" + id,
        type: "GET",
        success: function (response) {
            if (response.success) {
                var v = response.variant;
                $("#v_VariantId").val(v.id);
                $("#v_SKU").val(v.sku);
                $("#v_VariantPrice").val(v.variantPrice);
                $("#v_IsActive").val(v.isActive.toString());

                if (response.priceStock) {
                    $("#v_Price").val(response.priceStock.price);
                    $("#v_CompareAtPrice").val(response.priceStock.compareAtPrice || "");
                    $("#v_CostPrice").val(response.priceStock.costPrice || "");
                    $("#v_StockQty").val(response.priceStock.stockQty);
                    $("#v_TrackInventory").prop('checked', response.priceStock.trackInventory);
                    $("#v_AllowBackorder").prop('checked', response.priceStock.allowBackorder);
                    $("#v_WeightGrams").val(response.priceStock.weightGrams || "");
                }

                $.ajax({
                    url: `/AdminProduct/GetProductOptions?productId=${productId}`,
                    type: "GET",
                    success: function (optResp) {
                        container.empty();
                        $("#noAttributesWarning").addClass('d-none');

                        if (optResp.success && optResp.options.length > 0) {
                            optResp.options.forEach(opt => {
                                var selectId = `attr-val-${opt.attributeId}`;
                                var html = `<div class="col-sm-3 mb-3">
                                    <label for="${selectId}" class="form-label">${escapeHTML(opt.attributeName)}</label>
                                    <select id="${selectId}" class="form-select dynamic-attr-select" required>
                                        <option value="">-- Select ${escapeHTML(opt.attributeName)} --</option>`;
                                opt.values.forEach(val => html += `<option value="${val.id}">${escapeHTML(val.value)}</option>`);
                                html += `</select></div>`;
                                container.append(html);
                            });

                            response.selectedValueIds?.forEach(valId => {
                                container.find(`option[value='${valId}']`).prop('selected', true);
                            });

                            $("#btnCancelEditVariant").removeClass("d-none");
                        } else {
                            $("#noAttributesWarning").removeClass('d-none');
                        }
                    },
                    error: function (xhr) {
                        container.html(`<div class="col-12 text-danger">Error: ${xhr.responseJSON?.message || "Unknown"}</div>`);
                    }
                });
            }
        },
        error: function (xhr) {
            $("#variantErrorAlert").text(xhr.responseJSON?.message).removeClass('d-none');
        }
    });
}

function clearVariantForm() {
    var productId = $("#v_ProductId").val();
    $("#variantForm")[0].reset();
    $("#v_ProductId").val(productId);
    $("#v_VariantId").val("");
    $("#v_Price").val("");
    $("#v_CompareAtPrice").val("");
    $("#v_CostPrice").val("");
    $("#v_StockQty").val("0");
    $("#v_TrackInventory").prop('checked', true);
    $("#v_AllowBackorder").prop('checked', false);
    $("#v_WeightGrams").val("");
    $("#dynamic-variant-form-container").empty();
    $("#btnCancelEditVariant").addClass("d-none");
    $("#variantErrorAlert").addClass('d-none');
    $("#noAttributesWarning").addClass('d-none');
    loadDynamicVariantForm(productId);
}

function deleteVariant(id, btn) {
    if (!confirm("Are you sure you want to delete this variant?")) return;

    $.ajax({
        url: "/AdminProduct/DeleteVariant",
        type: "POST",
        data: { id: id },
        headers: { 'RequestVerificationToken': getCsrfToken() },
        success: function (response) {
            if (response.success) {
                $(btn).closest('tr').fadeOut(300, function () {
                    $(this).remove();
                    if ($("#variantTableBody tr").length === 0) {
                        $("#variantTableBody").append('<tr><td colspan="6" class="text-center">No variants found.</td></tr>');
                    }
                });
                showToast("Success", "Variant deleted.");
            }
        },
        error: function (xhr) {
            $("#variantErrorAlert").text(xhr.responseJSON?.message).removeClass('d-none');
        }
    });
}

function loadDynamicVariantForm(productId, callback) {
    var container = $("#dynamic-variant-form-container");
    container.html('<div class="col-12 text-center">Loading options...</div>');

    $.ajax({
        url: `/AdminProduct/GetProductOptions?productId=${productId}`,
        type: "GET",
        success: function (response) {
            container.empty();
            $("#noAttributesWarning").addClass('d-none');

            if (response.success && response.options.length > 0) {
                response.options.forEach(opt => {
                    var selectId = `attr-val-${opt.attributeId}`;
                    var html = `<div class="col-sm-3 mb-3">
                        <label for="${selectId}" class="form-label">${escapeHTML(opt.attributeName)}</label>
                        <select id="${selectId}" class="form-select dynamic-attr-select" required>
                            <option value="">-- Select ${escapeHTML(opt.attributeName)} --</option>`;
                    opt.values.forEach(val => html += `<option value="${val.id}">${escapeHTML(val.value)}</option>`);
                    html += `</select></div>`;
                    container.append(html);
                });
            } else {
                $("#noAttributesWarning").removeClass('d-none');
            }
            if (callback) callback();
        },
        error: function (xhr) {
            container.html(`<div class="col-12 text-danger">Error: ${xhr.responseJSON?.message || "Unknown"}</div>`);
        }
    });
}

function loadVariantsTable(productId) {
    var tableBody = $("#variantTableBody");
    tableBody.html('<tr><td colspan="6" class="text-center">Loading variants...</td></tr>');

    $.ajax({
        url: `/AdminProduct/GetVariants?productId=${productId}`,
        type: "GET",
        success: function (response) {
            tableBody.empty();
            if (response.success && response.variants.length > 0) {
                response.variants.forEach(v => tableBody.append(renderVariantRow(v)));
            } else {
                tableBody.append('<tr><td colspan="6" class="text-center">No variants found.</td></tr>');
            }
        },
        error: function (xhr) {
            var msg = xhr.responseJSON?.message || "An unknown error.";
            tableBody.html(`<tr><td colspan="6" class="text-danger">Error: ${msg}</td></tr>`);
        }
    });
}

function showVariantModal(productId, productName) {
    $("#variantModalLabel").text(`Manage Variants for: ${productName}`);
    $("#v_ProductId").val(productId);
    $("#variantModal").data("productName", productName);

    loadDynamicVariantForm(productId, () => {
        clearVariantForm();
        variantModal.show();
    });

    loadVariantsTable(productId);
}

function showAttributeModalFromProduct() {
    var productId = $('#Id').val();

    if (!productId) {
        alert('Please save the product first before managing attributes.');
        return;
    }

    productModal.hide();

    $("#attr_ProductId").val(productId);
    $("#attributeModalLabel").text(`Manage Attributes for Product ID: ${productId}`);
    $("#attributeErrorAlert").addClass('d-none');

    attributeModal.show();

    var tableBody = $("#productAttributeTableBody");
    tableBody.html('<tr><td>Loading...</td></tr>');

    $.ajax({
        url: `/AdminProduct/GetProductAttributes?productId=${productId}`,
        type: "GET",
        success: function (response) {
            tableBody.empty();
            if (response.success) {
                if (response.attributes.length > 0) {
                    response.attributes.forEach(attr => tableBody.append(renderProductAttributeRow(attr)));
                } else {
                    tableBody.append('<tr><td>No attributes assigned.</td></tr>');
                }
                populateAvailableAttributes(response.attributes);
            }
        },
        error: function (xhr) {
            tableBody.html(`<tr><td class="text-danger">Error: ${xhr.responseJSON?.message}</td></tr>`);
        }
    });

    $('#attributeModal').one('hidden.bs.modal', function () {
        productModal.show();
    });
}

function showAttributeModal() {
    var productId = $("#v_ProductId").val();
    var productName = $("#variantModal").data("productName");
    $("#attributeModalLabel").text(`Manage Attributes for: ${productName}`);
    $("#attr_ProductId").val(productId);
    $("#attributeErrorAlert").addClass('d-none');

    attributeModal.show();

    var tableBody = $("#productAttributeTableBody");
    tableBody.html('<tr><td>Loading...</td></tr>');

    $.ajax({
        url: `/AdminProduct/GetProductAttributes?productId=${productId}`,
        type: "GET",
        success: function (response) {
            tableBody.empty();
            if (response.success) {
                if (response.attributes.length > 0) {
                    response.attributes.forEach(attr => tableBody.append(renderProductAttributeRow(attr)));
                } else {
                    tableBody.append('<tr><td>No attributes assigned.</td></tr>');
                }
                populateAvailableAttributes(response.attributes);
            }
        },
        error: function (xhr) {
            tableBody.html(`<tr><td class="text-danger">Error: ${xhr.responseJSON?.message}</td></tr>`);
        }
    });
}

function populateAvailableAttributes(currentAttributes) {
    var dropdown = $("#availableAttributesDropdown");
    dropdown.empty().append('<option value="">-- Select attribute to add --</option>');

    var currentIds = currentAttributes.map(a => a.attributeId);
    var available = allAttributesCache.filter(attr => !currentIds.includes(attr.id));

    available.forEach(attr => {
        dropdown.append(`<option value="${attr.id}">${escapeHTML(attr.name)}</option>`);
    });
}

function saveProductAttribute() {
    var productId = $("#attr_ProductId").val();
    var attributeId = $("#availableAttributesDropdown").val();
    if (!attributeId) {
        $("#attributeErrorAlert").text("Please select an attribute.").removeClass('d-none');
        return;
    }

    $.ajax({
        url: "/AdminProduct/AddProductAttribute",
        type: "POST",
        data: { productId: productId, attributeId: attributeId },
        headers: { 'RequestVerificationToken': getCsrfToken() },
        success: function (response) {
            if (response.success) {
                var tableBody = $("#productAttributeTableBody");
                if (tableBody.find('td').length === 1) tableBody.empty();
                tableBody.append(renderProductAttributeRow(response.newAttribute));
                $("#availableAttributesDropdown option[value='" + attributeId + "']").remove();
                $("#availableAttributesDropdown").val("");
                $("#attributeErrorAlert").addClass('d-none');
                showToast("Success", "Attribute added successfully.");
                loadDynamicVariantForm(productId);
            }
        },
        error: function (xhr) {
            $("#attributeErrorAlert").text(xhr.responseJSON?.message).removeClass('d-none');
        }
    });
}

function deleteProductAttribute(productAttributeId, attributeId, btn) {
    if (!confirm("Are you sure you want to remove this attribute from the product?")) return;

    $.ajax({
        url: "/AdminProduct/DeleteProductAttribute",
        type: "POST",
        data: { productAttributeId: productAttributeId },
        headers: { 'RequestVerificationToken': getCsrfToken() },
        success: function (response) {
            if (response.success) {
                $(btn).closest('tr').fadeOut(300, function () {
                    $(this).remove();
                    if ($("#productAttributeTableBody tr").length === 0) {
                        $("#productAttributeTableBody").append('<tr><td>No attributes assigned.</td></tr>');
                    }
                });

                var attr = allAttributesCache.find(a => a.id === attributeId);
                if (attr) {
                    $("#availableAttributesDropdown").append(`<option value="${attr.id}">${escapeHTML(attr.name)}</option>`);
                }
                loadDynamicVariantForm($("#attr_ProductId").val());
            }
        },
        error: function (xhr) {
            $("#attributeErrorAlert").text(xhr.responseJSON?.message).removeClass('d-none');
        }
    });
}

function showDiscountModal(productId, productName) {
    $("#discountForm")[0].reset();
    $("#disc_ProductId").val(productId);
    $("#discountForm #ProductId").val(productId);
    $("#discountErrorAlert").addClass("d-none");

    var now = new Date();
    now.setMinutes(now.getMinutes() - now.getTimezoneOffset());
    $("#EffectiveFrom").val(now.toISOString().slice(0, 16));

    discountModal.show();
    loadDiscountsList(productId);
}

function loadDiscountsList(productId) {
    var tbody = $("#discountTableBody");
    tbody.html('<tr><td colspan="3" class="text-center">Loading...</td></tr>');

    $.ajax({
        url: `/AdminProduct/GetDiscounts?productId=${productId}`,
        type: "GET",
        success: function (response) {
            tbody.empty();
            if (response.success && response.discounts.length > 0) {
                response.discounts.forEach(d => {
                    var valueDisplay = d.discountType === "Percentage" ? `${d.discountValue}%` : `${d.discountValue}`;
                    var fromDate = new Date(d.effectiveFrom).toLocaleDateString();

                    var row = `
                        <tr>
                            <td><span class="badge bg-info text-dark">${valueDisplay}</span></td>
                            <td><small>${fromDate}</small></td>
                            <td class="text-center">
                                <button class="btn btn-sm btn-outline-danger py-0" onclick="deleteDiscount(${d.id})">
                                    <i class="bi bi-trash"></i>
                                </button>
                            </td>
                        </tr>`;
                    tbody.append(row);
                });
            } else {
                tbody.append('<tr><td colspan="3" class="text-center text-muted small">No active discounts.</td></tr>');
            }
        }
    });
}

function saveDiscount() {
    var formData = $("#discountForm").serializeArray();

    $.ajax({
        url: "/AdminProduct/ApplyDiscount",
        type: "POST",
        data: $.param(formData),
        headers: { 'RequestVerificationToken': getCsrfToken() },
        success: function (response) {
            if (response.success) {
                showToast("Success", response.message);
                $("#discountForm")[0].reset();
                loadDiscountsList($("#disc_ProductId").val());
                loadProducts();

                if ($("#variantModal").hasClass('show')) {
                    var variantProductId = $("#v_ProductId").val();
                    if (variantProductId) {
                        loadVariantsTable(variantProductId);
                    }
                }
            }
        },
        error: function (xhr) {
            $("#discountErrorAlert").text(xhr.responseJSON?.message || "Error").removeClass('d-none');
        }
    });
}

function deleteDiscount(id) {
    if (!confirm("Remove this discount? Prices will update immediately.")) return;

    $.ajax({
        url: "/AdminProduct/DeleteDiscount",
        type: "POST",
        data: { id: id },
        headers: { 'RequestVerificationToken': getCsrfToken() },
        success: function (response) {
            if (response.success) {
                showToast("Deleted", response.message);
                loadDiscountsList($("#disc_ProductId").val());
                loadProducts();

                if ($("#variantModal").hasClass('show')) {
                    var variantProductId = $("#v_ProductId").val();
                    if (variantProductId) {
                        loadVariantsTable(variantProductId);
                    }
                }
            }
        },
        error: function (xhr) {
            alert(xhr.responseJSON?.message || "Error deleting discount");
        }
    });
}

function renderProductRow(product) {
    var priceHtml = '';

    if (product.hasDiscount && product.originalPrice > product.sellingPrice) {
        priceHtml = `
        <div class="d-flex align-items-center">
            <span class="original-price">${product.originalPrice.toFixed(2)}</span>
            <span class="discounted-price">${product.sellingPrice.toFixed(2)}</span>
        </div>`;
    } else {
        var display = (product.sellingPrice > 0) ? product.sellingPrice : product.originalPrice;
        priceHtml = `<span class="fw-bold">${display.toFixed(2)}</span>`;
    }

    var activeBadge = product.isActive ?
        '<span class="badge bg-success">Active</span>' : '<span class="badge bg-secondary">Inactive</span>';
    var variantBadge = product.isVariantBased ? '<span class="badge bg-info">Yes</span>' : '<span class="badge bg-light text-dark">No</span>';

    var variantButton = product.isVariantBased ?
        `<button class="btn btn-sm btn-outline-info" onclick="showVariantModal(${product.id}, '${escapeQuote(product.productName)}')">Variants</button>` : '';

    return `
        <tr id="row-${product.id}">
            <td>${product.id}</td>
            <td>${escapeHTML(product.productName)}</td>
            <td>${escapeHTML(product.slug)}</td>
            <td>${priceHtml}</td>
            <td>${variantBadge}</td>
            <td>${activeBadge}</td>
            <td class="text-end">
                <button class="btn btn-sm btn-outline-secondary me-1" onclick="openProductImageModal(${product.id})" title="Manage Images">
                    <i class="bi bi-images"></i>
                </button>
                
                <button class="btn btn-sm btn-outline-warning me-1" onclick="showDiscountModal(${product.id}, '${escapeQuote(product.productName)}')">
                    <i class="bi bi-tag"></i> Discount
                </button>
                ${variantButton}
                <button class="btn btn-sm btn-outline-primary" onclick="showEditModal(${product.id})">Edit</button>
                <button class="btn btn-sm btn-outline-danger" onclick="showDeleteModal(${product.id}, '${escapeQuote(product.productName)}')">Delete</button>
            </td>
        </tr>`;
}

function renderProductAttributeRow(attr) {
    return `
        <tr id="product-attr-row-${attr.productAttributeId}">
            <td>${escapeHTML(attr.attributeName)}</td>
            <td class="text-end">
                <button class="btn btn-sm btn-outline-danger" onclick="deleteProductAttribute(${attr.productAttributeId}, ${attr.attributeId}, this)">
                    Remove
                </button>
            </td>
        </tr>`;
}

function renderVariantRow(variant) {
    var sellingPrice = parseFloat(variant.price) || 0;
    var comparePrice = variant.compareAtPrice;

    var priceHtml = '';

    if (comparePrice != null && comparePrice > sellingPrice) {
        priceHtml = `
            <div class="d-flex align-items-center justify-content-end">
                <span class="original-price">
                    ${Number(comparePrice).toFixed(2)}
                </span>
                <span class="discounted-price">
                    ${sellingPrice.toFixed(2)}
                </span>
            </div>`;
    } else {
        priceHtml = `<span class="fw-bold">${sellingPrice.toFixed(2)}</span>`;
    }

    var activeBadge = variant.isActive
        ? '<span class="badge bg-success">Active</span>'
        : '<span class="badge bg-secondary">Inactive</span>';

    var stockBadge = variant.stockQty > 0
        ? `<span class="badge bg-success">${variant.stockQty}</span>`
        : `<span class="badge bg-danger">0</span>`;

    return `
        <tr id="variant-row-${variant.id}">
            <td>${escapeHTML(variant.variantName)}</td>
            <td>${escapeHTML(variant.sku)}</td>
            <td class="text-end">${priceHtml}</td>
            <td class="text-center">${stockBadge}</td>
            <td class="text-center">${activeBadge}</td>
            <td class="text-end">
                <button class="btn btn-sm btn-outline-secondary me-1" onclick="openVariantImageModal(${variant.id})" title="Manage Images">
                    <i class="bi bi-images"></i>
                </button>

                <button class="btn btn-sm btn-outline-primary" onclick="editVariant(${variant.id})">Edit</button>
                <button class="btn btn-sm btn-outline-danger" onclick="deleteVariant(${variant.id}, this)">Delete</button>
            </td>
        </tr>`;
}

function showToast(title, message, isError = false) {
    $("#toastTitle").text(title);
    $("#toastMessage").text(message);
    $("#appToast").removeClass('bg-danger bg-success text-white');
    $("#appToast").addClass(isError ? 'bg-danger text-white' : 'bg-success text-white');
    appToast.show();
}

function escapeHTML(str) {
    if (str === null || str === undefined) return "";
    return str.toString().replace(/[&<>"']/g, m => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' })[m]);
}

function escapeQuote(str) {
    if (str === null || str === undefined) return "";
    return str.toString().replace(/'/g, "\\'").replace(/"/g, "&quot;");
}