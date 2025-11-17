console.log("🔥 JS FILE LOADED");
window.onerror = function (msg, url, line, col, error) {
    console.error("🔥 Uncaught JS Error:", msg, "at", line + ":" + col);
};
var productModal;
var deleteModal;
var categoryModal;
var variantModal;
var appToast;
var categoryCache = [];

function getCsrfToken() {
    return $('input[name="__RequestVerificationToken"]').val();
}

$(document).ready(function () {
    debugger;
    productModal = new bootstrap.Modal(document.getElementById('productModal'));
    deleteModal = new bootstrap.Modal(document.getElementById('deleteModal'));
    appToast = new bootstrap.Toast(document.getElementById('appToast'));
    categoryModal = new bootstrap.Modal(document.getElementById('categoryModal'));
    variantModal = new bootstrap.Modal(document.getElementById('variantModal'));

    loadProducts();
    loadCategories();

    $("#productForm").submit(function (e) { debugger; e.preventDefault(); saveProduct(); });
    $("#btnConfirmDelete").click(function () { debugger; deleteProduct($(this).data('id')); });
    $("#categoryForm").submit(function (e) { debugger; e.preventDefault(); saveCategory(); });
    $("#variantForm").submit(function (e) { debugger; e.preventDefault(); saveVariant(); });
});

//
// --- Data Loading Functions ---
//
function loadProducts() {
    debugger;
    var tableBody = $("#productTableBody");
    tableBody.html('<tr><td colspan="7" class="text-center"><div class="spinner-border spinner-border-sm" role="status"></div> &nbsp; Loading products...</td></tr>');

    $.ajax({
        url: "/AdminProduct/GetProducts",
        type: "GET",

        beforeSend: function () { debugger; },

        success: function (response) {
            debugger;
            tableBody.empty();
            if (response.success && response.products.length > 0) {
                response.products.forEach(function (product) {
                    debugger;
                    tableBody.append(renderProductRow(product));
                });
            } else {
                tableBody.append('<tr><td colspan="7" class="text-center">No products found. Click "Add New Product" to begin.</td></tr>');
            }
        },

        error: function (xhr) {
            debugger;
            var msg = xhr.responseJSON?.message || "An unknown error occurred.";
            tableBody.empty().append(`<tr><td colspan="7" class="text-center text-danger">Error loading products: ${msg}</td></tr>`);
            showToast("Error", "Could not load products. " + msg, true);
        }
    });
}

function loadCategories() {
    debugger;
    if (categoryCache.length > 0) { populateCategoryDropdown(categoryCache); return; }

    $.ajax({
        url: "/AdminProduct/GetCategories",
        type: "GET",

        beforeSend: function () { debugger; },

        success: function (response) {
            debugger;
            if (response.success) {
                categoryCache = response.categories;
                populateCategoryDropdown(categoryCache);
            }
        },

        error: function () {
            debugger;
            $("#CategoryId").html('<option value="">Error loading categories</option>');
        }
    });
}

//
// --- Modal Functions ---
//
function showCreateModal() {
    debugger;
    $("#productForm")[0].reset();
    $("#productModalLabel").text("Add New Product");
    $("#Id").val("");
    $("#modalErrorAlert").addClass('d-none');
    $("#IsActive").prop('checked', true);
    populateCategoryDropdown(categoryCache);
    productModal.show();
}

function showEditModal(id) {
    debugger;
    $("#productForm")[0].reset();
    $("#productModalLabel").text("Edit Product");
    $("#modalErrorAlert").addClass('d-none');

    $.ajax({
        url: "/AdminProduct/GetProduct?id=" + id,
        type: "GET",

        beforeSend: function () { debugger; },

        success: function (response) {
            debugger;
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
                productModal.show();
            }
        },

        error: function (xhr) {
            debugger;
            showToast("Error", "Could not load product. " + xhr.responseJSON?.message, true);
        }
    });
}

function showDeleteModal(id, name) {
    debugger;
    $("#deleteProductName").text(name);
    $("#btnConfirmDelete").data('id', id);
    $("#deleteErrorAlert").addClass('d-none');
    deleteModal.show();
}

function showCategoryModal() {
    debugger;
    $("#categoryForm")[0].reset();
    $("#categoryErrorAlert").addClass('d-none');
    var productModalElement = document.getElementById('productModal');
    productModalElement.style.zIndex = 1040;
    categoryModal.show();
    $('.modal-backdrop').last().css('z-index', 1050);
}

function showVariantModal(productId, productName) {
    debugger;
    $("#variantModalLabel").text(`Manage Variants for: ${productName}`);
    $("#v_ProductId").val(productId);
    clearVariantForm();

    var tableBody = $("#variantTableBody");
    tableBody.html('<tr><td colspan="5" class="text-center">Loading variants...</td></tr>');
    variantModal.show();

    $.ajax({
        url: `/AdminProduct/GetVariants?productId=${productId}`,
        type: "GET",

        beforeSend: function () { debugger; },

        success: function (response) {
            debugger;
            tableBody.empty();
            if (response.success && response.variants.length > 0) {
                response.variants.forEach(function (v) {
                    debugger;
                    tableBody.append(renderVariantRow(v));
                });
            } else {
                tableBody.append('<tr><td colspan="5" class="text-center">No variants found.</td></tr>');
            }
        },

        error: function (xhr) {
            debugger;
            var msg = xhr.responseJSON?.message || "An unknown error.";
            tableBody.html(`<tr><td colspan="5" class="text-danger">Error: ${msg}</td></tr>`);
        }
    });
}

//
// --- CRUD Functions ---
//
function saveProduct() {
    debugger;

    var isCreate = $("#Id").val() === "";
    var url = isCreate ? "/AdminProduct/Create" : "/AdminProduct/Edit";
    var formData = $("#productForm").serializeArray();

    if (!$("#IsVariantBased").is(":checked")) { formData.push({ name: "IsVariantBased", value: "false" }); }
    if (!$("#IsActive").is(":checked")) { formData.push({ name: "IsActive", value: "false" }); }

    debugger;

    $.ajax({
        url: url,
        type: "POST",
        traditional: true,   // FIXES ARRAY BINDING

        data: $.param(formData),
        headers: { 'RequestVerificationToken': getCsrfToken() },

        beforeSend: function () { debugger; },

        success: function (response) {
            debugger;
            if (response.success) {
                productModal.hide();
                showToast("Success", "Product saved successfully.");
                var newRow = renderProductRow(response.product);

                if (isCreate) {
                    var existingRow = $("#productTableBody").find('tr[id^="row-"]').length;

                    debugger;

                    if (existingRow === 0) { $("#productTableBody").empty(); }
                    $("#productTableBody").append(newRow);
                } else {
                    debugger;
                    $("#row-" + response.product.id).replaceWith(newRow);
                }
            }
        },

        error: function (xhr) {
            debugger;
            var msg = "An error occurred. " + (xhr.responseJSON?.message || "Check console.");
            $("#modalErrorAlert").text(msg).removeClass('d-none');
        }
    });
}

function deleteProduct(id) {
    debugger;

    $.ajax({
        url: "/AdminProduct/DeleteProduct",
        type: "POST",
        data: { id: id },
        headers: { 'RequestVerificationToken': getCsrfToken() },

        beforeSend: function () { debugger; },

        success: function (response) {
            debugger;
            if (response.success) {
                deleteModal.hide();
                showToast("Success", "Product deleted successfully.");
                $("#row-" + id).fadeOut(500, function () {
                    debugger;
                    $(this).remove();
                    if ($("#productTableBody").find('tr').length === 0) {
                        $("#productTableBody").append('<tr><td colspan="7" class="text-center">No products found.</td></tr>');
                    }
                });
            }
        },

        error: function (xhr) {
            debugger;
            var msg = "An error occurred. " + xhr.responseJSON?.message;
            $("#deleteErrorAlert").text(msg).removeClass('d-none');
        }
    });
}

function saveCategory() {
    debugger;

    var categoryName = $("#categoryName").val();

    $.ajax({
        url: "/AdminProduct/CreateCategory",
        type: "POST",
        data: { categoryName: categoryName },
        headers: { 'RequestVerificationToken': getCsrfToken() },

        beforeSend: function () { debugger; },

        success: function (response) {
            debugger;
            if (response.success) {
                categoryCache.push(response.newCategory);
                populateCategoryDropdown(categoryCache, response.newCategory.id);
                categoryModal.hide();
                showToast("Success", "Category created.");
                document.getElementById('productModal').style.zIndex = 1055;
            }
        },

        error: function (xhr) {
            debugger;
            $("#categoryErrorAlert").text(xhr.responseJSON?.message).removeClass('d-none');
        }
    });
}

//
// --- Variant Save (main bug zone) ---
//
function saveVariant() {
    var isCreate = $("#v_VariantId").val() === "";
    var url = isCreate ? "/AdminProduct/CreateVariant" : "/AdminProduct/UpdateVariant";

    // Manually build the data object to avoid duplicates
    var formData = {
        ProductId: $("#v_ProductId").val(),
        Id: $("#v_VariantId").val(),
        VariantName: $("#v_VariantName").val(),
        SKU: $("#v_SKU").val(),
        VariantPrice: $("#v_VariantPrice").val(),
        IsActive: $("#v_IsActive").val() === "true" // Convert string to boolean
    };

    $.ajax({
        url: url,
        type: "POST",
        data: formData,
        headers: { 'RequestVerificationToken': getCsrfToken() },
        success: function (response) {
            if (response.success) {
                var newRow = renderVariantRow(response.variant);
                if (isCreate) {
                    var existingRows = $("#variantTableBody").find('tr[id^="variant-row-"]').length;
                    if (existingRows === 0) { $("#variantTableBody").empty(); }
                    $("#variantTableBody").append(newRow);
                } else {
                    $("#variant-row-" + response.variant.id).replaceWith(newRow);
                }
                clearVariantForm();
                showToast("Success", "Variant saved.");
                $("#variantErrorAlert").addClass('d-none'); // Hide error on success
            }
        },
        error: function (xhr) {
            var errorResponse = xhr.responseJSON;
            var message = "An unknown error occurred.";

            if (errorResponse) {
                message = `<strong>${errorResponse.message}</strong>`;
                if (errorResponse.errors) {
                    var errorList = "<ul>";
                    for (var key in errorResponse.errors) {
                        errorResponse.errors[key].forEach(function (err) {
                            errorList += `<li>${escapeHTML(key)}: ${escapeHTML(err)}</li>`;
                        });
                    }
                    errorList += "</ul>";
                    message = errorList;
                }
            }

            $("#variantErrorAlert").html(message).removeClass('d-none');
        }
    });
}



function editVariant(id, btn) {
    debugger;

    $.ajax({
        url: "/AdminProduct/GetVariant?id=" + id,
        type: "GET",

        beforeSend: function () { debugger; },

        success: function (response) {
            debugger;

            if (response.success) {
                var v = response.variant;
                $("#v_VariantId").val(v.id);
                $("#v_ProductId").val(v.productId);
                $("#v_VariantName").val(v.variantName);
                $("#v_SKU").val(v.sku);
                $("#v_VariantPrice").val(v.variantPrice);
                $("#v_IsActive").val(v.isActive.toString());
                $("#btnCancelEditVariant").removeClass("d-none");
            }
        },

        error: function (xhr) {
            debugger;
            $("#variantErrorAlert").text(xhr.responseJSON?.message).removeClass('d-none');
        }
    });
}

function clearVariantForm() {
    debugger;
    var productId = $("#v_ProductId").val();
    $("#variantForm")[0].reset();
    $("#v_ProductId").val(productId);
    $("#v_VariantId").val("");
    $("#btnCancelEditVariant").addClass("d-none");
    $("#variantErrorAlert").addClass("d-none");
}

function deleteVariant(id, btn) {
    debugger;

    if (!confirm("Are you sure you want to delete this variant?")) { debugger; return; }

    $.ajax({
        url: "/AdminProduct/DeleteVariant",
        type: "POST",
        data: { id: id },
        headers: { 'RequestVerificationToken': getCsrfToken() },

        beforeSend: function () { debugger; },

        success: function (response) {
            debugger;

            if (response.success) {
                $(btn).closest('tr').fadeOut(300, function () {
                    debugger;
                    $(this).remove();
                    if ($("#variantTableBody").find('tr').length === 0) {
                        $("#variantTableBody").append('<tr><td colspan="5" class="text-center">No variants found.</td></tr>');
                    }
                });
                showToast("Success", "Variant deleted.");
            }
        },

        error: function (xhr) {
            debugger;
            $("#variantErrorAlert").text(xhr.responseJSON?.message).removeClass('d-none');
        }
    });
}

//
// --- UI Helpers ---
//
function renderProductRow(product) {
    debugger;
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

function renderVariantRow(variant) {
    debugger;
    var price = variant.variantPrice ? '$' + variant.variantPrice.toFixed(2) : 'N/A';
    var activeBadge = variant.isActive ? '<span class="badge bg-success">Active</span>' : '<span class="badge bg-secondary">Inactive</span>';

    return `
        <tr id="variant-row-${variant.id}">
            <td>${escapeHTML(variant.variantName)}</td>
            <td>${escapeHTML(variant.sku)}</td>
            <td>${price}</td>
            <td>${activeBadge}</td>
            <td class="text-end">
                <button class="btn btn-sm btn-outline-primary" onclick="editVariant(${variant.id}, this)">Edit</button>
                <button class="btn btn-sm btn-outline-danger" onclick="deleteVariant(${variant.id}, this)">Delete</button>
            </td>
        </tr>`;
}

function populateCategoryDropdown(categories, selectedId) {
    debugger;
    var dropdown = $("#CategoryId");
    dropdown.empty();
    dropdown.append('<option value="">-- Select Category --</option>');
    categories.forEach(function (category) {
        debugger;
        var selected = (category.id === selectedId) ? "selected" : "";
        dropdown.append(`<option value="${category.id}" ${selected}>${escapeHTML(category.name)}</option>`);
    });
}

function showToast(title, message, isError = false) {
    debugger;
    $("#toastTitle").text(title);
    $("#toastMessage").text(message);
    $("#appToast").removeClass('bg-danger bg-success text-white');

    if (isError) {
        $("#appToast").addClass('bg-danger text-white');
    } else {
        $("#appToast").addClass('bg-success text-white');
    }

    appToast.show();
}

function escapeHTML(str) {
    debugger;
    if (str === null || str === undefined) return "";
    return str.toString().replace(/[&<>"']/g, function (m) {
        return {
            '&': '&amp;',
            '<': '&lt;',
            '>': '&gt;',
            '"': '&quot;',
            "'": '&#39;'
        }[m];
    });
}

function escapeQuote(str) {
    debugger;
    if (str === null || str === undefined) return "";
    return str.toString().replace(/'/g, '\\\'').replace(/"/g, '&quot;');
}
