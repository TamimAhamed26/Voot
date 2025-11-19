#nullable disable
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Facade;
using MDUA.Facade.Interface;
using MDUA.Web.UI.Models; // <-- Contains your DTO and SaveModel
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MDUA.Web.UI.Controllers
{
    [Authorize]
    public class AdminProductController : BaseController
    {
        private readonly IProductFacade _productFacade;
        private readonly IProductCategoryFacade _productCategoryFacade;
        private readonly IProductVariantFacade _productVariantFacade;
        private readonly ILogger<AdminProductController> _logger;
        private readonly IVariantPriceStockFacade _variantPriceStockFacade;

        // --- 1. Inject all new Facades ---
        private readonly IProductAttributeFacade _productAttributeFacade;
        private readonly IAttributeNameFacade _attributeNameFacade;
        private readonly IAttributeValueFacade _attributeValueFacade;
        private readonly IVariantAttributeValueFacade _variantAttributeValueFacade;

        public AdminProductController(
            IProductFacade productFacade,
            IProductCategoryFacade productCategoryFacade,
            IProductVariantFacade productVariantFacade,
            ILogger<AdminProductController> logger,
            // --- 2. Add new Facades to constructor ---
            IProductAttributeFacade productAttributeFacade,
            IAttributeNameFacade attributeNameFacade,
            IAttributeValueFacade attributeValueFacade,
            IVariantPriceStockFacade variantPriceStockFacade,

            IVariantAttributeValueFacade variantAttributeValueFacade)
        {
            _productFacade = productFacade;
            _productCategoryFacade = productCategoryFacade;
            _productVariantFacade = productVariantFacade;
            _logger = logger;
            // --- 3. Assign new Facades ---
            _productAttributeFacade = productAttributeFacade;
            _attributeNameFacade = attributeNameFacade;
            _attributeValueFacade = attributeValueFacade;
            _variantPriceStockFacade = variantPriceStockFacade;

            _variantAttributeValueFacade = variantAttributeValueFacade;
        }

        #region Helper: GetModelStateErrors
        private object GetModelStateErrors()
        {
            var errors = ModelState
                .Where(kvp => kvp.Value.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors
                                .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage)
                                    ? e.Exception?.Message
                                    : e.ErrorMessage)
                                .Where(msg => !string.IsNullOrWhiteSpace(msg))
                                .ToArray()
                );

            return new
            {
                success = false,
                message = "Invalid data. Please check errors.",
                errors = errors
            };
        }

        /// <summary>
        /// Checks if a variant with the same attribute combination already exists
        /// </summary>
        private bool VariantExists(int productId, List<int> attributeValueIds, int? excludeVariantId = null)
        {
            // Get all variants for this product
            var existingVariants = _productVariantFacade.GetByProductId(productId);

            foreach (var variant in existingVariants)
            {
                // Skip the variant being edited
                if (excludeVariantId.HasValue && variant.Id == excludeVariantId.Value)
                    continue;

                // Get attribute values for this variant
                var variantAttributes = _variantAttributeValueFacade.GetByVariantId(variant.Id);
                var variantValueIds = variantAttributes.Select(va => va.AttributeValueId).OrderBy(id => id).ToList();

                // Compare sorted lists
                var inputValueIds = attributeValueIds.OrderBy(id => id).ToList();

                if (variantValueIds.SequenceEqual(inputValueIds))
                {
                    return true; // Duplicate found
                }
            }

            return false; // No duplicate
        }
        #endregion

        #region --- Attribute Management  ---

        //
        // GET: /AdminProduct/GetProductAttributes?productId=1
        // Gets the attributes *currently linked* to a product
        //
        [HttpGet]
        public IActionResult GetProductAttributes(int productId)
        {
            if (!IsPermitted(Permission.Product.View))
                return StatusCode(403, new { success = false, message = "Access Denied" });

            try
            {
                var productAttributes = _productAttributeFacade.GetByProductId(productId);

                // We need to get the names for them
                var result = productAttributes.Select(pa => new {
                    productAttributeId = pa.Id, // This is the ID of the *link* (for deleting)
                    attributeId = pa.AttributeId,
                    attributeName = _attributeNameFacade.Get(pa.AttributeId)?.Name
                }).ToList();

                return Json(new { success = true, attributes = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product attributes for {ProductId}", productId);
                return StatusCode(500, new { success = false, message = "Error loading attributes." });
            }
        }

        //
        // GET: /AdminProduct/GetAvailableAttributes
        // Gets *all* attributes in the system (for the dropdown)
        //
        [HttpGet]
        public IActionResult GetAvailableAttributes()
        {
            if (!IsPermitted(Permission.Product.View))
                return StatusCode(403, new { success = false, message = "Access Denied" });

            try
            {
                var attributes = _attributeNameFacade.GetAll();
                return Json(new { success = true, attributes = attributes });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all attributes");
                return StatusCode(500, new { success = false, message = "Error loading attributes." });
            }
        }

        //
        // POST: /AdminProduct/AddProductAttribute
        // Links an attribute (like "Color") to a product
        //
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddProductAttribute(int productId, int attributeId)
        {
            if (!IsPermitted(Permission.Product.Edit))
                return StatusCode(403, new { success = false, message = "Access Denied" });

            try
            {
                // Check if it already exists
                var existing = _productAttributeFacade.GetByProductId(productId)
                    .FirstOrDefault(pa => pa.AttributeId == attributeId);

                if (existing != null)
                {
                    return StatusCode(400, new { success = false, message = "This attribute is already added to the product." });
                }

                var productAttribute = new ProductAttribute
                {
                    ProductId = productId,
                    AttributeId = attributeId,
                    DisplayOrder = 0 // You can make this editable later
                };

                _productAttributeFacade.Insert(productAttribute); // This populates productAttribute.Id

                // Get the name to send back to the UI
                var attrName = _attributeNameFacade.Get(attributeId)?.Name;

                return Json(new
                {
                    success = true,
                    newAttribute = new
                    {
                        productAttributeId = productAttribute.Id,
                        attributeId = productAttribute.AttributeId,
                        attributeName = attrName
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding attribute {AttributeId} to product {ProductId}", attributeId, productId);
                return StatusCode(500, new { success = false, message = "Error adding attribute." });
            }
        }

        //
        // POST: /AdminProduct/DeleteProductAttribute
        // Unlinks an attribute from a product
        //
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteProductAttribute(int productAttributeId) // This is the ID of the LINK, not the attribute
        {
            if (!IsPermitted(Permission.Product.Edit))
                return StatusCode(403, new { success = false, message = "Access Denied" });

            try
            {
                _productAttributeFacade.Delete(productAttributeId);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product attribute {ProductAttributeId}", productAttributeId);
                return StatusCode(500, new { success = false, message = "Error deleting attribute." });
            }
        }

        #endregion

        #region Product Shell & List 
        //
        // GET: /AdminProduct/
        //
        public IActionResult Index()
        {
            if (!IsPermitted(Permission.Product.View))
            {
                ViewData["TaskName"] = "View Products";
                return View("AccessDenied", "Account");
            }
            return View();
        }

        //
        // GET: /AdminProduct/GetProducts 
        //
        [HttpGet]
        public IActionResult GetProducts()
        {
            if (!IsPermitted(Permission.Product.View))
            {
                return StatusCode(403, new { success = false, message = "Access Denied" });
            }
            var productList = _productFacade.GetAll();
            return Json(new { success = true, products = productList });
        }
        #endregion

        #region Product CRUD 
        //
        // GET: /AdminProduct/GetProduct/5 
        //
        [HttpGet]
        public IActionResult GetProduct(int id)
        {
            if (!IsPermitted(Permission.Product.View))
            {
                return StatusCode(403, new { success = false, message = "Access Denied" });
            }
            var product = _productFacade.Get(id);
            if (product == null)
            {
                return StatusCode(404, new { success = false, message = "Product not found" });
            }
            return Json(new { success = true, product = product });
        }

        //
        // POST: /AdminProduct/Create 
        //
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductCreateViewModel model)
        {
            if (!IsPermitted(Permission.Product.Create))
            {
                return StatusCode(403, new { success = false, message = "Access Denied" });
            }
            if (!ModelState.IsValid)
            {
                return StatusCode(400, GetModelStateErrors());
            }
            Product product = new Product
            {
                ProductName = model.ProductName,
                Slug = model.Slug,
                Description = model.Description,
                BasePrice = model.BasePrice,
                Barcode = model.Barcode,
                ReorderLevel = model.ReorderLevel,
                IsVariantBased = model.IsVariantBased,
                CategoryId = model.CategoryId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = User.Identity.Name,
                CompanyId = 1
            };
            try
            {
                _productFacade.Insert(product);
                return Json(new { success = true, product = product });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product {ProductName}", model.ProductName);
                return StatusCode(500, new { success = false, message = "An error occurred while saving the product." });
            }
        }

        //
        // POST: /AdminProduct/Edit 
        //
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProductEditViewModel model)
        {
            if (!IsPermitted(Permission.Product.Edit))
            {
                return StatusCode(403, new { success = false, message = "Access Denied" });
            }
            if (!ModelState.IsValid)
            {
                return StatusCode(400, GetModelStateErrors());
            }
            var product = _productFacade.Get(model.Id);
            if (product == null)
            {
                return StatusCode(404, new { success = false, message = "Product not found." });
            }
            product.ProductName = model.ProductName;
            product.Slug = model.Slug;
            product.Description = model.Description;
            product.BasePrice = model.BasePrice;
            product.Barcode = model.Barcode;
            product.ReorderLevel = model.ReorderLevel;
            product.IsVariantBased = model.IsVariantBased;
            product.CategoryId = model.CategoryId;
            product.IsActive = model.IsActive;
            product.UpdatedAt = DateTime.UtcNow;
            product.UpdatedBy = User.Identity.Name;
            try
            {
                _productFacade.Update(product);
                return Json(new { success = true, product = product });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product {ProductName}", model.ProductName);
                return StatusCode(500, new { success = false, message = "An error occurred while saving the product." });
            }
        }

        //
        // POST: /AdminProduct/Delete 
        //
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteProduct(int id)
        {
            if (!IsPermitted(Permission.Product.Delete))
            {
                return StatusCode(403, new { success = false, message = "Access Denied" });
            }
            try
            {
                _productFacade.Delete(id);
                return Json(new { success = true, message = "Product deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product {ProductId}", id);
                return StatusCode(500, new { success = false, message = "An error occurred. The product might be in use on an order." });
            }
        }
        #endregion

        #region Category Quick-Add 
        //
        // GET: /AdminProduct/GetCategories 
        //
        [HttpGet]
        public IActionResult GetCategories()
        {
            var categories = _productCategoryFacade.GetAll();
            return Json(new { success = true, categories = categories });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateCategory(string categoryName)
        {
            if (!IsPermitted(Permission.Product.Category_Create))
            {
                return StatusCode(403, new { success = false, message = "Access Denied" });
            }
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                return StatusCode(400, new { success = false, message = "Category name cannot be empty." });
            }
            var category = new ProductCategory { Name = categoryName };
            try
            {
                _productCategoryFacade.Insert(category);
                return Json(new { success = true, newCategory = category });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category {CategoryName}", categoryName);
                return StatusCode(500, new { success = false, message = "Error saving category." });
            }
        }
        #endregion

        #region --- DYNAMIC VARIANT ACTIONS  ---

        //
        // "FETCH DATA" (NEW ACTION)
        // GET: /AdminProduct/GetProductOptions?productId=1
        //
        [HttpGet]
        public IActionResult GetProductOptions(int productId)
        {
            if (!IsPermitted(Permission.Product.View))
            {
                return StatusCode(403, new { success = false, message = "Access Denied" });
            }

            try
            {
                var options = new List<ProductOptionDTO>();

                // 1. Get all attributes linked to this product (e.g., AttrID 1, AttrID 2)
                var productAttributes = _productAttributeFacade.GetByProductId(productId);

                foreach (var attr in productAttributes.OrderBy(a => a.DisplayOrder))
                {
                    // 2. Get the name (e.g., "Color")
                    var attrName = _attributeNameFacade.Get(attr.AttributeId);

                    // 3. Get all values for this attribute (e.g., "Black", "M")
                    var attrValues = _attributeValueFacade.GetByAttributeId(attr.AttributeId);

                    if (attrName != null && attrValues.Count > 0)
                    {
                        options.Add(new ProductOptionDTO
                        {
                            AttributeId = attr.AttributeId,
                            AttributeName = attrName.Name,
                            Values = attrValues
                        });
                    }
                }

                // 4. Send the complete list of options to the JavaScript
                return Json(new { success = true, options = options });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product options for ProductId {ProductId}", productId);
                return StatusCode(500, new { success = false, message = "Could not load product options." });
            }
        }

        //
        // GET: /AdminProduct/GetVariants?productId=1
        //
        [HttpGet]
        public IActionResult GetVariants(int productId)
        {
            if (!IsPermitted(Permission.Product.View))
                return StatusCode(403, new { success = false, message = "Access Denied" });

            var variants = _productVariantFacade.GetByProductId(productId);

            // Enhance variants with stock data
            var variantsWithStock = variants.Select(v =>
            {
                var priceStock = _variantPriceStockFacade.Get(v.Id);
                return new
                {
                    id = v.Id,
                    productId = v.ProductId,
                    variantName = v.VariantName,
                    sku = v.SKU,
                    variantPrice = v.VariantPrice,
                    isActive = v.IsActive,
                    stockQty = priceStock?.StockQty ?? 0,
                    trackInventory = priceStock?.TrackInventory ?? true,
                    allowBackorder = priceStock?.AllowBackorder ?? false
                };
            }).ToList();

            return Json(new { success = true, variants = variantsWithStock });
        }


        //
        // GET: /AdminProduct/GetVariant/5
        //
        [HttpGet]
        public IActionResult GetVariant(int id)
        {
            if (!IsPermitted(Permission.Product.View))
                return StatusCode(403, new { success = false, message = "Access Denied" });

            var variant = _productVariantFacade.Get(id);
            if (variant == null)
                return StatusCode(404, new { success = false, message = "Variant not found" });

            var links = _variantAttributeValueFacade.GetByVariantId(id);
            var selectedValueIds = links.Select(l => l.AttributeValueId).ToList();

            // NEW: Get price/stock data
            var priceStock = _variantPriceStockFacade.Get(id);

            return Json(new
            {
                success = true,
                variant = variant,
                selectedValueIds = selectedValueIds,
                priceStock = priceStock // NEW
            });
        }

        //
        // "SAVE DATA" - CREATE (UPDATED ACTION)
        //
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateVariant(ProductVariantSaveModel model)
        {
            if (!IsPermitted(Permission.Product.Create))
                return StatusCode(403, new { success = false, message = "Access Denied" });

            if (!ModelState.IsValid || model.SelectedAttributeValueIds == null || model.SelectedAttributeValueIds.Count == 0)
                return StatusCode(400, GetModelStateErrors());

            try
            {
                // *** NEW: Check for duplicate variant ***
                if (VariantExists(model.ProductId, model.SelectedAttributeValueIds))
                {
                    return StatusCode(400, new
                    {
                        success = false,
                        message = "A variant with this combination of attributes already exists. Please choose different attributes."
                    });
                }

                // 1. Create main variant
                var variant = new ProductVariant
                {
                    ProductId = model.ProductId,
                    VariantName = model.VariantName,
                    SKU = model.SKU,
                    VariantPrice = model.VariantPrice,
                    IsActive = model.IsActive,
                    CreatedBy = User.Identity!.Name,
                    CreatedAt = DateTime.UtcNow
                };

                _productVariantFacade.Insert(variant);

                // 2. Create VariantPriceStock record
                var priceStock = new VariantPriceStock
                {
                    Id = variant.Id,
                    Price = model.Price,
                    CompareAtPrice = model.CompareAtPrice,
                    CostPrice = model.CostPrice,
                    StockQty = model.StockQty,
                    TrackInventory = model.TrackInventory,
                    AllowBackorder = model.AllowBackorder,
                    WeightGrams = model.WeightGrams
                };

                _variantPriceStockFacade.Insert((VariantPriceStockBase)priceStock);

                // 3. Save attribute links
                int displayOrder = 0;
                foreach (var valueId in model.SelectedAttributeValueIds)
                {
                    var attrValue = _attributeValueFacade.Get(valueId);
                    if (attrValue == null)
                        return StatusCode(400, new { success = false, message = $"Invalid attributeValueId: {valueId}" });

                    _variantAttributeValueFacade.Insert(new VariantAttributeValue
                    {
                        VariantId = variant.Id,
                        AttributeId = attrValue.AttributeId,
                        AttributeValueId = valueId,
                        DisplayOrder = displayOrder++
                    });
                }

                return Json(new
                {
                    success = true,
                    variant = new
                    {
                        id = variant.Id,
                        productId = variant.ProductId,
                        variantName = variant.VariantName,
                        sku = variant.SKU,
                        variantPrice = variant.VariantPrice,
                        isActive = variant.IsActive,
                        stockQty = model.StockQty,
                        trackInventory = model.TrackInventory,
                        allowBackorder = model.AllowBackorder
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating dynamic variant for ProductId {ProductId}", model.ProductId);
                return StatusCode(500, new { success = false, message = "Error saving variant.", exception = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateVariant(ProductVariantSaveModel model)
        {
            if (!IsPermitted(Permission.Product.Edit))
                return StatusCode(403, new { success = false, message = "Access Denied" });

            if (!ModelState.IsValid || model.Id == 0 || model.SelectedAttributeValueIds == null || model.SelectedAttributeValueIds.Count == 0)
                return StatusCode(400, GetModelStateErrors());

            var variant = _productVariantFacade.Get(model.Id);
            if (variant == null)
                return StatusCode(404, new { success = false, message = "Variant not found." });

            try
            {
                //
                // Detect whether attributes actually changed
                //
                var oldAttributeLinks = _variantAttributeValueFacade.GetByVariantId(model.Id);
                var oldValueIds = oldAttributeLinks.Select(a => a.AttributeValueId).OrderBy(id => id).ToList();
                var newValueIds = model.SelectedAttributeValueIds.OrderBy(id => id).ToList();

                bool attributesChanged = !oldValueIds.SequenceEqual(newValueIds);

                //
                // Check duplicate variant only if attributes changed
                //
                if (attributesChanged)
                {
                    if (VariantExists(model.ProductId, model.SelectedAttributeValueIds, model.Id))
                    {
                        return StatusCode(400, new
                        {
                            success = false,
                            message = "A variant with this combination of attributes already exists. Please choose different attributes."
                        });
                    }
                }

                //
                //Update main variant table
                //
                variant.VariantName = model.VariantName;
                variant.SKU = model.SKU;
                variant.VariantPrice = model.VariantPrice;
                variant.IsActive = model.IsActive;
                variant.UpdatedBy = User.Identity!.Name;
                variant.UpdatedAt = DateTime.UtcNow;

                _productVariantFacade.Update(variant);

                //
                //Update VariantPriceStock (create if missing)
                //
                var priceStock = _variantPriceStockFacade.Get(model.Id);

                if (priceStock != null)
                {
                    priceStock.Price = model.Price;
                    priceStock.CompareAtPrice = model.CompareAtPrice;
                    priceStock.CostPrice = model.CostPrice;
                    priceStock.StockQty = model.StockQty;
                    priceStock.TrackInventory = model.TrackInventory;
                    priceStock.AllowBackorder = model.AllowBackorder;
                    priceStock.WeightGrams = model.WeightGrams;

                    _variantPriceStockFacade.Update((VariantPriceStockBase)priceStock);
                }
                else
                {
                    var newPriceStock = new VariantPriceStock
                    {
                        Id = model.Id,
                        Price = model.Price,
                        CompareAtPrice = model.CompareAtPrice,
                        CostPrice = model.CostPrice,
                        StockQty = model.StockQty,
                        TrackInventory = model.TrackInventory,
                        AllowBackorder = model.AllowBackorder,
                        WeightGrams = model.WeightGrams
                    };

                    _variantPriceStockFacade.Insert((VariantPriceStockBase)newPriceStock);
                }

                //
                //  Update attribute links ONLY if changed
                //
                if (attributesChanged)
                {
                    // Delete old attribute links
                    foreach (var link in oldAttributeLinks)
                        _variantAttributeValueFacade.Delete(link.Id);

                    // Insert new attribute links
                    int displayOrder = 0;
                    foreach (var valueId in model.SelectedAttributeValueIds)
                    {
                        var attrValue = _attributeValueFacade.Get(valueId);
                        if (attrValue == null)
                            return StatusCode(400, new { success = false, message = $"Invalid attributeValueId: {valueId}" });

                        _variantAttributeValueFacade.Insert(new VariantAttributeValue
                        {
                            VariantId = variant.Id,
                            AttributeId = attrValue.AttributeId,
                            AttributeValueId = valueId,
                            DisplayOrder = displayOrder++
                        });
                    }
                }

                var updatedPriceStock = _variantPriceStockFacade.Get(model.Id);

                //
                //  Final response
                //
                return Json(new
                {
                    success = true,
                    variant = new
                    {
                        id = variant.Id,
                        productId = variant.ProductId,
                        variantName = variant.VariantName,
                        sku = variant.SKU,
                        variantPrice = variant.VariantPrice,
                        isActive = variant.IsActive,
                        stockQty = updatedPriceStock?.StockQty ?? 0,
                        trackInventory = updatedPriceStock?.TrackInventory ?? true,
                        allowBackorder = updatedPriceStock?.AllowBackorder ?? false,
                        weightGrams = updatedPriceStock?.WeightGrams ?? 0
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating variant {VariantId}", model.Id);
                return StatusCode(500, new { success = false, message = "Error saving variant.", exception = ex.Message });
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteVariant(int id)
        {
            if (!IsPermitted(Permission.Product.Delete))
            {
                return StatusCode(403, new { success = false, message = "Access Denied" });
            }
            try
            {
                _productVariantFacade.Delete(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting variant {VariantId}", id);
                return StatusCode(500, new { success = false, message = "Error deleting variant." });
            }
        }
        #endregion
    }
}