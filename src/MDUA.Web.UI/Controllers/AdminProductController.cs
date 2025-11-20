#nullable disable
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Facade;
using MDUA.Facade.Interface;
using MDUA.Web.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static MDUA.Entities.ProductDiscount;

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
        private readonly IProductDiscountFacade _productDiscountFacade;
        public AdminProductController(
            IProductFacade productFacade,
            IProductCategoryFacade productCategoryFacade,
            IProductVariantFacade productVariantFacade,
            ILogger<AdminProductController> logger,
            IProductAttributeFacade productAttributeFacade,
            IAttributeNameFacade attributeNameFacade,
            IAttributeValueFacade attributeValueFacade,
            IVariantPriceStockFacade variantPriceStockFacade,

            IVariantAttributeValueFacade variantAttributeValueFacade, IProductDiscountFacade productDiscountFacade)

        {
            _productFacade = productFacade;
            _productCategoryFacade = productCategoryFacade;
            _productVariantFacade = productVariantFacade;
            _logger = logger;
            _productAttributeFacade = productAttributeFacade;
            _attributeNameFacade = attributeNameFacade;
            _attributeValueFacade = attributeValueFacade;
            _variantPriceStockFacade = variantPriceStockFacade;
            _productDiscountFacade = productDiscountFacade;
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

                var result = productAttributes.Select(pa => new
                {
                    productAttributeId = pa.Id, // ID of the *link* (for deleting)
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
            if (!IsPermitted(Permission.Product.View)) return StatusCode(403, new { success = false });

            try
            {
                var products = _productFacade.GetAll();
                var allDiscounts = _productDiscountFacade.GetAll(); // Fetch all discounts once (Optimization)
                var resultList = new List<ProductListViewModel>();
                var now = DateTime.UtcNow;

                foreach (var p in products)
                {
                    var vm = new ProductListViewModel
                    {
                        Id = p.Id,
                        ProductName = p.ProductName,
                        Slug = p.Slug,
                        IsVariantBased = p.IsVariantBased ?? false,
                        IsActive = p.IsActive,
                        OriginalPrice = p.BasePrice ?? 0
                    };

                    // Only calculate discount for Simple Products for the main grid
                    if (!vm.IsVariantBased)
                    {
                        // Find active discounts for this product
                        var activeDiscounts = allDiscounts
                            .Where(d => d.ProductId == p.Id && d.IsActive && d.EffectiveFrom <= now && (d.EffectiveTo == null || d.EffectiveTo >= now))
                            .ToList();

                        decimal bestPrice = vm.OriginalPrice;
                        if (activeDiscounts.Any())
                        {
                            foreach (var d in activeDiscounts)
                            {
                                decimal calc = CalculateNewPrice(vm.OriginalPrice, d.DiscountType, d.DiscountValue);
                                if (calc < bestPrice) bestPrice = calc;
                            }
                        }

                        vm.SellingPrice = bestPrice;
                        vm.HasDiscount = bestPrice < vm.OriginalPrice;
                    }
                    else
                    {
                        // For variant products, just show the base price in grid usually
                        vm.SellingPrice = vm.OriginalPrice;
                        vm.HasDiscount = false;
                    }

                    resultList.Add(vm);
                }

                return Json(new { success = true, products = resultList });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
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
        // "FETCH DATA" 
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
                var options = new List<ProductOption1DTO>();

                var productAttributes = _productAttributeFacade.GetByProductId(productId);

                foreach (var attr in productAttributes.OrderBy(a => a.DisplayOrder))
                {
                    var attrName = _attributeNameFacade.Get(attr.AttributeId);

                    var attrValues = _attributeValueFacade.GetByAttributeId(attr.AttributeId);

                    if (attrName != null && attrValues.Count > 0)
                    {
                        options.Add(new ProductOption1DTO
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

            var priceStock = _variantPriceStockFacade.Get(id);

            return Json(new
            {
                success = true,
                variant = variant,
                selectedValueIds = selectedValueIds,
                priceStock = priceStock
            });
        }

        //
        // CREATE
        //
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateVariant(ProductVariantSaveModl model)
        {
            if (!IsPermitted(Permission.Product.Create))
                return StatusCode(403, new { success = false, message = "Access Denied" });

            if (!ModelState.IsValid || model.SelectedAttributeValueIds == null || !model.SelectedAttributeValueIds.Any())
                return StatusCode(400, GetModelStateErrors());

            try
            {
                var result = _productVariantFacade.CreateVariantWithAttributes(model, User.Identity!.Name);

                return Json(new { success = true, message = "Variant created successfully.", variant = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating variant");
                return StatusCode(400, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateVariant(ProductVariantSaveModl model)
        {
            if (!IsPermitted(Permission.Product.Edit))
                return StatusCode(403, new { success = false, message = "Access Denied" });

            if (!ModelState.IsValid || model.Id == 0)
                return StatusCode(400, GetModelStateErrors());

            try
            {
                var result = _productVariantFacade.UpdateVariantWithAttributes(model, User.Identity!.Name);

                return Json(new { success = true, message = "Variant updated successfully.", variant = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating variant {Id}", model.Id);
                return StatusCode(400, new { success = false, message = ex.Message });
            }
        }
        //
        // DELETE
        //
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteVariant(int id)
        {
            if (!IsPermitted(Permission.Product.Delete))
                return StatusCode(403, new { success = false, message = "Access Denied" });

            try
            {
                _productVariantFacade.Delete(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting variant {Id}", id);
                return StatusCode(500, new { success = false, message = "Error deleting variant." });
            }
        }

        #endregion

        #region -- Discounts Management --

      

        // Helper reused from Facade (or make this a shared utility)
        private decimal CalculateNewPrice(decimal original, string type, decimal value)
        {
            if (type.Equals("Percentage", StringComparison.OrdinalIgnoreCase)) return original - (original * (value / 100));
            else return Math.Max(0, original - value);
        }

        // -----------------------------------------------------------
        // 2. DISCOUNT ENDPOINTS
        // -----------------------------------------------------------

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApplyDiscount(ProductDiscountViewModel model)
        {
            if (!IsPermitted(Permission.Product.Edit)) return StatusCode(403);
            if (!ModelState.IsValid) return StatusCode(400, new { success = false, message = "Invalid data" });

            try
            {
                var discount = new ProductDiscount
                {
                    ProductId = model.ProductId,
                    DiscountType = model.DiscountType,
                    DiscountValue = model.DiscountValue,
                    MinQuantity = model.MinQuantity,
                    EffectiveFrom = model.EffectiveFrom,
                    EffectiveTo = model.EffectiveTo,
                    IsActive = true
                };

                _productDiscountFacade.ApplyDiscount(discount, User.Identity?.Name ?? "Admin");

                return Json(new { success = true, message = "Discount applied successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Discount Error");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteDiscount(int id)
        {
            if (!IsPermitted(Permission.Product.Edit)) return StatusCode(403);

            try
            {
                _productDiscountFacade.DeleteDiscount(id, User.Identity?.Name ?? "Admin");
                return Json(new { success = true, message = "Discount removed. Prices restored/recalculated." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetDiscounts(int productId)
        {
            var discounts = _productDiscountFacade.GetByProductId(productId);
            var result = discounts.OrderByDescending(d => d.CreatedAt).ToList();
            return Json(new { success = true, discounts = result });
        }
    }
    #endregion
}
