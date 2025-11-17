// Global variables
var productModal, deleteModal, categoryModal, variantModal, appToast;
var categoryCache = [];

function getCsrfToken() {
    return $('input[name="__RequestVerificationToken"]').val();
}

// Use modern jQuery document ready
$(function () {
    // Initialize Bootstrap components
    productModal = new bootstrap.Modal(document.getElementById('productModal'));
    deleteModal = new bootstrap.Modal(document.getElementById('deleteModal'));
    appToast = new bootstrap.Toast(document.getElementById('appToast'));
    categoryModal = new bootstrap.Modal(document.getElementById('categoryModal'));
    variantModal = new bootstrap.Modal(document.getElementById('variantModal'));

    // Load initial data
    loadProducts();
    loadCategories();

    // --- Form Handlers ---
    $("#productForm").on('submit', function (e) { e.preventDefault(); saveProduct(); });
    $("#btnConfirmDelete").on('click', function () { deleteProduct($(this).data('id')); });
    $("#categoryForm").on('submit', function (e) { e.preventDefault(); saveCategory(); });
    $("#variantForm").on('submit', function (e) { e.preventDefault(); saveVariant(); });
});

//
// --- Data Loading Functions ---
//
function loadProducts() {
    var tableBody = $("#productTableBody");
    tableBody.html('<tr><td colspan="7" class="text-center"><div class="spinner-border spinner-border-sm" role="status"></div> &nbsp; Loading products...</td></tr>');

    $.ajax({
        url: "/AdminProduct/GetProducts",
        type: "GET",
        success: function (response) {
            tableBody.empty();
            if (response.success && response.products.length > 0) {
                response.products.forEach(function (product) {
                    tableBody.append(renderProductRow(product));
                });
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
    if (categoryCache.length > 0) { populateCategoryDropdown(categoryCache); return; }
    $.ajax({
        url: "/AdminProduct/GetCategories",
        type: "GET",
        success: function (response) {
            if (response.success) {
                categoryCache = response.categories;
                populateCategoryDropdown(categoryCache);
            }
        },
        error: function () { $("#CategoryId").html('<option value="">Error loading categories</option>'); }
    });
}

//
// --- Modal Functions (Product, Delete, Category) ---
//
function showCreateModal() {
    $("#productForm")[0].reset();
    $("#productModalLabel").text("Add New Product");
    $("#Id").val("");
    $("#modalErrorAlert").addClass('d-none');
    $("#IsActive").prop('checked', true);
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
                productModal.show();
            }
        },
        error: function (xhr) { showToast("Error", "Could not load product. " + xhr.responseJSON?.message, true); }
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
    var productModalElement = document.getElementById('productModal');
    productModalElement.style.zIndex = 1040;
    categoryModal.show();
    $('.modal-backdrop').last().css('z-index', 1050);
}

//
// --- "FETCH DATA": This function loads dynamic dropdowns and variant list ---
//
function showVariantModal(productId, productName) {
    $("#variantModalLabel").text(`Manage Variants for: ${productName}`);
    $("#v_ProductId").val(productId);

    // Store the product name in the modal's data for later use
    $("#variantModal").data("productName", productName);

    var tableBody = $("#variantTableBody");
    tableBody.html('<tr><td colspan="5" class="text-center">Loading variants...</td></tr>');

    // Call the helper to load the dynamic form
    loadDynamicVariantForm(productId, function () {
        // This callback runs AFTER the form is built
        clearVariantForm(); // Reset form for 'Add'
        variantModal.show();
    });

    // Load existing variants
    $.ajax({
        url: `/AdminProduct/GetVariants?productId=${productId}`,
        type: "GET",
        success: function (response) {
            tableBody.empty();
            if (response.success && response.variants.length > 0) {
                response.variants.forEach(function (v) { tableBody.append(renderVariantRow(v)); });
            } else {
                tableBody.append('<tr><td colspan="5" class="text-center">No variants found.</td></tr>');
            }
        },
        error: function (xhr) {
            var msg = xhr.responseJSON?.message || "An unknown error.";
            tableBody.html(`<tr><td colspan="5" class="text-danger">Error: ${msg}</td></tr>`);
        }
    });
}


//
// --- CRUD Functions ---
//
function saveProduct() {
    var isCreate = $("#Id").val() === "";
    var url = isCreate ? "/AdminProduct/Create" : "/AdminProduct/Edit";
    var formData = $("#productForm").serializeArray();
    if (!$("#IsVariantBased").is(":checked")) { formData.push({ name: "IsVariantBased", value: "false" }); }
    if (!$("#IsActive").is(":checked")) { formData.push({ name: "IsActive", value: "false" }); }

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
                    var existingRow = $("#productTableBody").find('tr[id^="row-"]').length;
                    if (existingRow === 0) { $("#productTableBody").empty(); }
                    $("#productTableBody").append(newRow);
                } else {
                    $("#row-" + response.product.id).replaceWith(newRow);
                }
            }
        },
        error: function (xhr) {
            var msg = "An error occurred. " + (xhr.responseJSON?.message || "Check console.");
            $("#modalErrorAlert").text(msg).removeClass('d-none');
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
                    if ($("#productTableBody").find('tr').length === 0) {
                        $("#productTableBody").append('<tr><td colspan="7" class="text-center">No products found.</td></tr>');
                    }
                });
            }
        },
        error: function (xhr) {
            var msg = "An error occurred. " + xhr.responseJSON?.message;
            $("#deleteErrorAlert").text(msg).removeClass('d-none');
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
                document.getElementById('productModal').style.zIndex = 1055;
            }
        },
        error: function (xhr) {
            $("#categoryErrorAlert").text(xhr.responseJSON?.message).removeClass('d-none');
        }
    });
}

//
// --- ⭐️⭐️ "SAVE DATA" - THIS IS THE CORRECT, WORKING FUNCTION ⭐️⭐️ ---
//
function saveVariant() {
    var isCreate = $("#v_VariantId").val() === "";
    var url = isCreate ? "/AdminProduct/CreateVariant" : "/AdminProduct/UpdateVariant";

    // --- 1. Manually build the data object ---
    // This fixes the "Id: '' is invalid" error.
    var formData = {
        ProductId: $("#v_ProductId").val(),
        SKU: $("#v_SKU").val(),
        VariantPrice: $("#v_VariantPrice").val(),
        IsActive: $("#v_IsActive").val() === "true", // Convert string to boolean
        SelectedAttributeValueIds: []
    };

    // --- 2. Auto-generate VariantName and fill SelectedAttributeValueIds ---
    var selectedTexts = [];
    $(".dynamic-attr-select").each(function () {
        var valueId = $(this).val();
        if (valueId) {
            formData.SelectedAttributeValueIds.push(parseInt(valueId));
            selectedTexts.push($(this).find('option:selected').text());
        }
    });

    var productName = $("#variantModal").data("productName");
    formData.VariantName = `${productName} - ${selectedTexts.join(" / ")}`;

    // --- 3. ONLY add the Id property if we are EDITING ---
    if (!isCreate) {
        formData.Id = $("#v_VariantId").val();
    }
    // If we are CREATING, we do not send "Id" at all.

    $.ajax({
        url: url,
        type: "POST",
        data: formData, // Send the clean, manually-built object
        headers: { 'RequestVerificationToken': getCsrfToken() },
        success: function (response) {
            if (response.success) {
                var newRow = renderVariantRow(response.variant);
                if (isCreate) {
                    var existingRows = $("#variantTableBody").find('tr[id^="variant-row-"]').length;
                    if (existingRows === 0) { $("#variantTableBody").empty(); }
                    $("#variantTableBody").append(newRow);
                } else {
                    // This will now work, because renderVariantRow exists
                    $("#variant-row-" + response.variant.id).replaceWith(newRow);
                }
                clearVariantForm();
                showToast("Success", "Variant saved.");
                $("#variantErrorAlert").addClass('d-none');
            }
        },
        error: function (xhr) {
            // Your error helper logic
            var errorResponse = xhr.responseJSON;
            var message = "An unknown error occurred.";
            if (errorResponse) {
                message = `<strong>${errorResponse.message}</strong>`;
                if (errorResponse.errors) {
                    var errorList = "<ul>";
                    for (var key in errorResponse.errors) {
                        if (key.includes("SelectedAttributeValueIds")) {
                            errorList += `<li>Attributes: Please select a value for all options.</li>`;
                        } else {
                            errorResponse.errors[key].forEach(function (err) {
                                errorList += `<li>${escapeHTML(key)}: ${escapeHTML(err)}</li>`;
                            });
                        }
                    }
                    errorList += "</ul>";
                    message = errorList;
                }
            }
            $("#variantErrorAlert").html(message).removeClass('d-none');
        }
    });
}

//
// --- ⭐️⭐️ "FETCH DATA" for Edit - THIS IS THE CORRECT, WORKING FUNCTION ⭐️⭐️ ---
//
function editVariant(id, btn) {
    var formContainer = $("#dynamic-variant-form-container");
    formContainer.html('<div class="col-12 text-center">Loading options...</div>');
    var productId = $("#v_ProductId").val();

    // --- AJAX CALL 1: Get the variant's main data (GetVariant) ---
    $.ajax({
        url: "/AdminProduct/GetVariant?id=" + id,
        type: "GET",
        success: function (response) {
            if (response.success) {
                var v = response.variant;
                // Populate the simple form fields
                $("#v_VariantId").val(v.id);
                $("#v_ProductId").val(v.productId);
                $("#v_SKU").val(v.sku);
                $("#v_VariantPrice").val(v.variantPrice);
                $("#v_IsActive").val(v.isActive.toString());

                // --- AJAX CALL 2: Get the attribute dropdowns (GetProductOptions) ---
                $.ajax({
                    url: `/AdminProduct/GetProductOptions?productId=${productId}`,
                    type: "GET",
                    success: function (optionsResponse) {
                        formContainer.empty();
                        if (optionsResponse.success && optionsResponse.options.length > 0) {

                            // Build the dropdowns
                            optionsResponse.options.forEach(function (option) {
                                var selectId = `attr-val-${option.attributeId}`;
                                var html = `<div class="col-sm-3 mb-3">
                                              <label for="${selectId}" class="form-label">${escapeHTML(option.attributeName)}</label>
                                              <select id="${selectId}" name="SelectedAttributeValueIds" class="form-select dynamic-attr-select" required>
                                                <option value="">-- Select ${escapeHTML(option.attributeName)} --</option>`;
                                option.values.forEach(function (val) {
                                    html += `<option value="${val.id}">${escapeHTML(val.value)}</option>`;
                                });
                                html += `</select></div>`;
                                formContainer.append(html);
                            });

                            // --- 8. NOW we can set the selected values ---
                            if (response.selectedValueIds) {
                                response.selectedValueIds.forEach(function (valueId) {
                                    var select = $(`#dynamic-variant-form-container select option[value='${valueId}']`).closest('select');
                                    if (select.length > 0) {
                                        select.val(valueId);
                                    }
                                });
                            }

                            $("#btnCancelEditVariant").removeClass("d-none");

                        } else {
                            formContainer.html('<div class="col-12"><p>No attributes are set for this product.</p></div>');
                        }
                    },
                    error: function (xhr) {
                        var msg = xhr.responseJSON?.message || "An unknown error.";
                        formContainer.html(`<div class="col-12 text-danger">Error: ${msg}</div>`);
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

    // Clear dynamic dropdowns and show "Add" button
    $("#dynamic-variant-form-container").empty();
    $("#btnCancelEditVariant").addClass("d-none");
    $("#variantErrorAlert").addClass("d-none");

    // Re-build the dynamic form for creating a new variant
    loadDynamicVariantForm(productId);
}

function deleteVariant(id, btn) {
    if (!confirm("Are you sure you want to delete this variant?")) { return; }
    $.ajax({
        url: "/AdminProduct/DeleteVariant",
        type: "POST",
        data: { id: id },
        headers: { 'RequestVerificationToken': getCsrfToken() },
        success: function (response) {
            if (response.success) {
                $(btn).closest('tr').fadeOut(300, function () {
                    $(this).remove();
                    if ($("#variantTableBody").find('tr').length === 0) {
                        $("#variantTableBody").append('<tr><td colspan="5" class="text-center">No variants found.</td></tr>');
                    }
                });
                showToast("Success", "Variant deleted.");
            }
        },
        error: function (xhr) { $("#variantErrorAlert").text(xhr.responseJSON?.message).removeClass('d-none'); }
    });
}

// --- NEW HELPER FUNCTION ---
function loadDynamicVariantForm(productId, callback) {
    var formContainer = $("#dynamic-variant-form-container");
    formContainer.html('<div class="col-12 text-center">Loading options...</div>');

    $.ajax({
        url: `/AdminProduct/GetProductOptions?productId=${productId}`,
        type: "GET",
        success: function (response) {
            formContainer.empty();
            if (response.success && response.options.length > 0) {
                response.options.forEach(function (option) {
                    var selectId = `attr-val-${option.attributeId}`;
                    var html = `<div class="col-sm-3 mb-3">
                                  <label for="${selectId}" class="form-label">${escapeHTML(option.attributeName)}</label>
                                  <select id="${selectId}" name="SelectedAttributeValueIds" class="form-select dynamic-attr-select" required>
                                    <option value="">-- Select ${escapeHTML(option.attributeName)} --</option>`;

                    option.values.forEach(function (val) {
                        html += `<option value="${val.id}">${escapeHTML(val.value)}</option>`;
                    });

                    html += `</select></div>`;
                    formContainer.append(html);
                });
            } else {
                formContainer.html('<div class="col-12"><p>This product is not set up for dynamic variants. Please edit the product and add attributes.</p></div>');
            }

            // Run the callback function if it exists
            if (callback) {
                callback();
            }
        },
        error: function (xhr) {
            var msg = xhr.responseJSON?.message || "An unknown error.";
            formContainer.html(`<div class="col-12 text-danger">Error: ${msg}</div>`);
        }
    });
}


//
// --- ⭐️⭐️ UI Helper Functions - THESE ARE REQUIRED ⭐️⭐️ ---
//
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

// --- This function IS INCLUDED ---
function renderVariantRow(variant) {
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
    var dropdown = $("#CategoryId");
    dropdown.empty();
    dropdown.append('<option value="">-- Select Category --</option>');
    categories.forEach(function (category) {
        var selected = (category.id === selectedId) ? "selected" : "";
        dropdown.append(`<option value="${category.id}" ${selected}>${escapeHTML(category.name)}</option>`);
    });
}

function showToast(title, message, isError = false) {
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

// Security helpers
function escapeHTML(str) {
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
    if (str === null || str === undefined) return "";
    return str.toString().replace(/'/g, '\\\'').replace(/"/g, '&quot;');
}