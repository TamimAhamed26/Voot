// Global variables
var productModal, deleteModal, categoryModal, variantModal, attributeModal, appToast;
var categoryCache = [];
var allAttributesCache = [];

function getCsrfToken() {
    return $('input[name="__RequestVerificationToken"]').val();
}

$(function () {
    productModal = new bootstrap.Modal(document.getElementById('productModal'));
    deleteModal = new bootstrap.Modal(document.getElementById('deleteModal'));
    appToast = new bootstrap.Toast(document.getElementById('appToast'));
    categoryModal = new bootstrap.Modal(document.getElementById('categoryModal'));
    variantModal = new bootstrap.Modal(document.getElementById('variantModal'));
    attributeModal = new bootstrap.Modal(document.getElementById('attributeModal'));

    // NEW: Handle modal backdrop cleanup
    $('#categoryModal').on('hidden.bs.modal', function () {
        if ($('.modal.show').length > 0) {
            $('body').addClass('modal-open');
        }
    });

    $('#attributeModal').on('hidden.bs.modal', function () {
        if ($('.modal.show').length > 0) {
            $('body').addClass('modal-open');
        }
    });

    // NEW: Toggle Manage Attributes button visibility based on IsVariantBased checkbox
    $('#IsVariantBased').on('change', function () {
        toggleManageAttributesButton();
    });

    loadProducts();
    loadCategories();
    loadAllAttributes();

    $("#productForm").on('submit', function (e) { e.preventDefault(); saveProduct(); });
    $("#btnConfirmDelete").on('click', function () { deleteProduct($(this).data('id')); });
    $("#categoryForm").on('submit', function (e) { e.preventDefault(); saveCategory(); });
    $("#variantForm").on('submit', function (e) { e.preventDefault(); saveVariant(); });
    $("#attributeAddForm").on('submit', function (e) { e.preventDefault(); saveProductAttribute(); });
});

// NEW: Toggle Manage Attributes button visibility
function toggleManageAttributesButton() {
    if ($('#IsVariantBased').is(':checked')) {
        $('#manageAttributesContainer').slideDown(200);
    } else {
        $('#manageAttributesContainer').slideUp(200);
    }
}

// NEW: Show attribute modal from product modal (for existing products only)
function showAttributeModalFromProduct() {
    var productId = $('#Id').val();

    if (!productId) {
        alert('Please save the product first before managing attributes.');
        return;
    }

    // Hide product modal temporarily
    productModal.hide();

    // Set up the attribute modal with product ID
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

    // When attribute modal closes, reopen product modal
    $('#attributeModal').one('hidden.bs.modal', function () {
        productModal.show();
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

function showCreateModal() {
    $("#productForm")[0].reset();
    $("#productModalLabel").text("Add New Product");
    $("#Id").val("");
    $("#modalErrorAlert").addClass('d-none');
    $("#IsActive").prop('checked', true);
    $('#manageAttributesContainer').hide(); // Hide manage attributes for new products
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

                // Show/hide manage attributes button based on variant status
                toggleManageAttributesButton();

                productModal.show();
            }
        },
        error: function (xhr) {
            showToast("Error", "Could not load product. " + xhr.responseJSON?.message, true);
        }
    });
}

function showDeleteModal(id, name) {
    $("#deleteProductName").text(name);
    $("#btnConfirmDelete").data('id', id);
    $("#deleteErrorAlert").addClass('d-none');
    deleteModal.show();
}

function showCategoryModal() {
    $("#categoryForm")[0].reset();
    $("#categoryErrorAlert").addClass('d-none');
    categoryModal.show();
}

function showVariantModal(productId, productName) {
    $("#variantModalLabel").text(`Manage Variants for: ${productName}`);
    $("#v_ProductId").val(productId);
    $("#variantModal").data("productName", productName);

    var tableBody = $("#variantTableBody");
    tableBody.html('<tr><td colspan="6" class="text-center">Loading variants...</td></tr>');

    loadDynamicVariantForm(productId, () => {
        clearVariantForm();
        variantModal.show();
    });

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

function saveProduct() {
    var isCreate = $("#Id").val() === "";
    var url = isCreate ? "/AdminProduct/Create" : "/AdminProduct/Edit";
    var formData = $("#productForm").serializeArray();

    if (!$("#IsVariantBased").is(":checked")) formData.push({ name: "IsVariantBased", value: "false" });
    if (!$("#IsActive").is(":checked")) formData.push({ name: "IsActive", value: "false" });

    $.ajax({
        url: url,
        type: "POST",
        data: $.param(formData),
        headers: { 'RequestVerificationToken': getCsrfToken() },
        success: function (response) {
            if (response.success) {
                productModal.hide();
                showToast("Success", "Product saved successfully.");
                var newRow = renderProductRow(response.product);
                if (isCreate) {
                    if (!$("#productTableBody").find('tr[id^="row-"]').length) $("#productTableBody").empty();
                    $("#productTableBody").append(newRow);
                } else {
                    $("#row-" + response.product.id).replaceWith(newRow);
                }
            }
        },
        error: function (xhr) {
            $("#modalErrorAlert").text("An error occurred. " + (xhr.responseJSON?.message || "Check console.")).removeClass('d-none');
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

function saveVariant() {
    var isCreate = $("#v_VariantId").val() === "";
    var url = isCreate ? "/AdminProduct/CreateVariant" : "/AdminProduct/UpdateVariant";

    var formData = {
        Id: $("#v_VariantId").val() || 0,
        ProductId: $("#v_ProductId").val(),
        SKU: $("#v_SKU").val(),
        VariantPrice: $("#v_VariantPrice").val(),
        IsActive: $("#v_IsActive").val() === "true",
        // VariantPriceStock fields
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
                var newRow = renderVariantRow(response.variant);
                if (isCreate) {
                    if (!$("#variantTableBody").find('tr[id^="variant-row-"]').length) $("#variantTableBody").empty();
                    $("#variantTableBody").append(newRow);
                } else {
                    $("#variant-row-" + response.variant.id).replaceWith(newRow);
                }
                clearVariantForm();
                showToast("Success", "Variant saved.");
                $("#variantErrorAlert").addClass('d-none');
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

                // Populate VariantPriceStock fields
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

    // Reset VariantPriceStock fields to defaults
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

function renderProductRow(product) {
    var price = product.basePrice ? '$' + product.basePrice.toFixed(2) : 'N/A';
    var activeBadge = product.isActive ? '<span class="badge bg-success">Active</span>' : '<span class="badge bg-secondary">Inactive</span>';
    var variantBadge = product.isVariantBased ? '<span class="badge bg-info">Yes</span>' : '<span class="badge bg-light text-dark">No</span>';
    var variantButton = product.isVariantBased ? `<button class="btn btn-sm btn-outline-info" onclick="showVariantModal(${product.id}, '${escapeQuote(product.productName)}')">Variants</button>` : '';

    return `
        <tr id="row-${product.id}">
            <td>${product.id}</td>
            <td>${escapeHTML(product.productName)}</td>
            <td>${escapeHTML(product.slug)}</td>
            <td>${price}</td>
            <td>${variantBadge}</td>
            <td>${activeBadge}</td>
            <td class="text-end">
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
    var price = variant.variantPrice ? '$' + variant.variantPrice.toFixed(2) : 'N/A';
    var activeBadge = variant.isActive ? '<span class="badge bg-success">Active</span>' : '<span class="badge bg-secondary">Inactive</span>';

    // Display stock quantity with color coding
    var stockQty = variant.stockQty !== undefined ? variant.stockQty : 'N/A';
    var stockBadge = '';

    if (variant.stockQty !== undefined) {
        if (variant.stockQty > 10) {
            stockBadge = `<span class="badge bg-success">${stockQty}</span>`;
        } else if (variant.stockQty > 0) {
            stockBadge = `<span class="badge bg-warning text-dark">${stockQty}</span>`;
        } else {
            stockBadge = `<span class="badge bg-danger">${stockQty}</span>`;
        }
    } else {
        stockBadge = '<span class="text-muted">N/A</span>';
    }

    return `
        <tr id="variant-row-${variant.id}">
            <td>${escapeHTML(variant.variantName)}</td>
            <td>${escapeHTML(variant.sku)}</td>
            <td>${price}</td>
            <td>${stockBadge}</td>
            <td>${activeBadge}</td>
            <td class="text-end">
                <button class="btn btn-sm btn-outline-primary" onclick="editVariant(${variant.id})">Edit</button>
                <button class="btn btn-sm btn-outline-danger" onclick="deleteVariant(${variant.id}, this)">Delete</button>
            </td>
        </tr>`;
}

function populateCategoryDropdown(categories, selectedId) {
    var dropdown = $("#CategoryId");
    dropdown.empty().append('<option value="">Select Category</option>');
    categories.forEach(cat => {
        var selected = (cat.id === selectedId) ? "selected" : "";
        dropdown.append(`<option value="${cat.id}" ${selected}>${escapeHTML(cat.name)}</option>`);
    });
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