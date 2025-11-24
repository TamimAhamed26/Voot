using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Facade.Interface;
using MDUA.Web.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using static MDUA.Entities.ProductDiscount;
using System.IO;                       // For Path, FileStream, Directory
using System.Threading.Tasks;          // For Task, async, await
using Microsoft.AspNetCore.Hosting;    // For IWebHostEnvironment
using Microsoft.AspNetCore.Http;       // For IFormFile

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
        private readonly IProductAttributeFacade _productAttributeFacade;
        private readonly IAttributeNameFacade _attributeNameFacade;
        private readonly IAttributeValueFacade _attributeValueFacade;
        private readonly IVariantAttributeValueFacade _variantAttributeValueFacade;
        private readonly IProductDiscountFacade _productDiscountFacade;
        private readonly IWebHostEnvironment _webHostEnvironment; // NEW: For file saving

        public AdminProductController(
            IProductFacade productFacade,
            IProductCategoryFacade productCategoryFacade,
            IProductVariantFacade productVariantFacade,
            ILogger<AdminProductController> logger,
            IProductAttributeFacade productAttributeFacade,
            IAttributeNameFacade attributeNameFacade,
            IAttributeValueFacade attributeValueFacade,
            IVariantPriceStockFacade variantPriceStockFacade,
            IVariantAttributeValueFacade variantAttributeValueFacade,
            IProductDiscountFacade productDiscountFacade,
            IWebHostEnvironment webHostEnvironment)
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
            _webHostEnvironment = webHostEnvironment;
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
        #endregion

        #region File Helper
        private async Task<string> SaveImageFile(IFormFile file, string targetFolder)
        {
            if (file == null || file.Length == 0)
                return null;

            if (!Directory.Exists(targetFolder))
                Directory.CreateDirectory(targetFolder);

            string ext = Path.GetExtension(file.FileName);
            string fileName = $"{Guid.NewGuid():N}{ext}";

            string filePath = Path.Combine(targetFolder, fileName);

            using (var fs = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fs);
            }

            return fileName;
        }

        #endregion

        #region Product Shell & List 
        public IActionResult Index()
        {
            if (!IsPermitted(Permission.Product.View))
            {
                ViewData["TaskName"] = "View Products";
                return View("AccessDenied", "Account");
            }
            return View();
        }

        [HttpGet]
        public IActionResult GetProducts()
        {
            if (!IsPermitted(Permission.Product.View)) return StatusCode(403, new { success = false });

            try
            {
                var products = _productFacade.GetAll();
                var resultList = new List<ProductListViewModel>();

                foreach (var p in products)
                {
                    // Use unified facade to compute selling price & active discount for product
                    Product productWithPrice = _productFacade.GetProductWithPrice(p.Id);

                    var vm = new ProductListViewModel
                    {
                        Id = p.Id,
                        ProductName = p.ProductName,
                        Slug = p.Slug,
                        IsVariantBased = p.IsVariantBased ?? false,
                        IsActive = p.IsActive,
                        SellingPrice = productWithPrice?.SellingPrice ?? (p.BasePrice ?? 0),
                        OriginalPrice = p.BasePrice ?? 0,
                        HasDiscount = (productWithPrice?.SellingPrice ?? (p.BasePrice ?? 0)) < (p.BasePrice ?? 0)
                    };

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
        [HttpGet]
        public IActionResult GetProduct(int id)
        {
            if (!IsPermitted(Permission.Product.View))
                return StatusCode(403, new { success = false, message = "Access Denied" });

            var product = _productFacade.GetProductWithPrice(id); // returns product with SellingPrice & ActiveDiscount
            if (product == null)
                return StatusCode(404, new { success = false, message = "Product not found" });
            var images = _productFacade.GetImages(id);
            // additionally include variants in the product payload if needed by UI
            return Json(new { success = true, product = product, images = images });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductCreateViewModel model)
        {
            if (!IsPermitted(Permission.Product.Create))
                return StatusCode(403, new { success = false, message = "Access Denied" });

            // Note: SelectedAttributeIds validation is optional, so ModelState is likely fine
            if (!ModelState.IsValid)
                return StatusCode(400, GetModelStateErrors());

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
                // 1. Insert Product
                _productFacade.Insert(product);

                // 2. Insert Attributes (if any selected)
                if (model.IsVariantBased && model.SelectedAttributeIds != null && model.SelectedAttributeIds.Any())
                {
                    foreach (var attrId in model.SelectedAttributeIds)
                    {
                        var pa = new ProductAttribute
                        {
                            ProductId = (int)product.Id,
                            AttributeId = attrId,
                            DisplayOrder = 0
                        };
                        _productAttributeFacade.Insert(pa);
                    }
                }

                // 3. Return result
                var created = _productFacade.GetProductWithPrice((int)product.Id);
                return Json(new { success = true, product = created ?? product });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product {ProductName}", model.ProductName);
                return StatusCode(500, new { success = false, message = "An error occurred while saving the product." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProductEditViewModel model)
        {
            if (!IsPermitted(Permission.Product.Edit))
                return StatusCode(403, new { success = false, message = "Access Denied" });
            if (!ModelState.IsValid)
                return StatusCode(400, GetModelStateErrors());

            var product = _productFacade.Get(model.Id);
            if (product == null)
                return StatusCode(404, new { success = false, message = "Product not found." });

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
                var updated = _productFacade.GetProductWithPrice(product.Id);
                return Json(new { success = true, product = updated ?? product });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product {ProductName}", model.ProductName);
                return StatusCode(500, new { success = false, message = "An error occurred while saving the product." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteProduct(int id)
        {
            if (!IsPermitted(Permission.Product.Delete))
                return StatusCode(403, new { success = false, message = "Access Denied" });
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

        #region Attribute Management 

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

        #region Category Quick-Add 
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
                return StatusCode(403, new { success = false, message = "Access Denied" });
            if (string.IsNullOrWhiteSpace(categoryName))
                return StatusCode(400, new { success = false, message = "Category name cannot be empty." });
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

        #region DYNAMIC VARIANT ACTIONS 
        [HttpGet]
        public IActionResult GetProductOptions(int productId)
        {
            if (!IsPermitted(Permission.Product.View))
                return StatusCode(403, new { success = false, message = "Access Denied" });
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
                return Json(new { success = true, options = options });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product options for ProductId {ProductId}", productId);
                return StatusCode(500, new { success = false, message = "Could not load product options." });
            }
        }

        [HttpGet]
        public IActionResult GetVariants(int productId)
        {
            if (!IsPermitted(Permission.Product.View))
                return StatusCode(403, new { success = false, message = "Access Denied" });

            // Get variants
            var variants = _productVariantFacade.GetByProductId(productId);
            var variantsWithStock = variants.Select(v =>
            {
                var priceStock = _variantPriceStockFacade.Get(v.Id);

                // compute original price (variant-specific if available)
                decimal originalPrice = priceStock?.Price ?? v.VariantPrice ?? 0;
                decimal sellingPrice = originalPrice;

                // Use the product-discount facade to choose the single best discount and calculate the new price
                var bestDiscount = _productDiscountFacade.GetBestDiscount(productId, originalPrice);
                if (bestDiscount != null)
                {
                    sellingPrice = _productDiscountFacade.CalculateNewPrice(originalPrice, bestDiscount.DiscountType, bestDiscount.DiscountValue);
                }

                bool hasDiscount = sellingPrice < originalPrice;

                return new
                {
                    id = v.Id,
                    productId = v.ProductId,
                    variantName = v.VariantName,
                    sku = v.SKU,
                    price = Math.Max(sellingPrice, 0), // final price shown in UI
                    compareAtPrice = hasDiscount ? originalPrice : (decimal?)null,
                    variantPrice = originalPrice, // raw stored price
                    isActive = v.IsActive,
                    stockQty = priceStock?.StockQty ?? 0,
                    trackInventory = priceStock?.TrackInventory ?? true,
                    allowBackorder = priceStock?.AllowBackorder ?? false
                };
            }).ToList();

            return Json(new { success = true, variants = variantsWithStock });
        }

        [HttpGet]
        public IActionResult GetVariant(int id)
        {
            if (!IsPermitted(Permission.Product.View))
                return StatusCode(403, new { success = false, message = "Access Denied" });
            var variant = _productVariantFacade.Get(id);
            if (variant == null)
                return StatusCode(404, new { success = false, message = "Variant not found" });
            var images = _productVariantFacade.GetImages(id);

            var links = _variantAttributeValueFacade.GetByVariantId(id);
            var selectedValueIds = links.Select(l => l.AttributeValueId).ToList();
            var priceStock = _variantPriceStockFacade.Get(id);

            // compute price & compareAtPrice for this variant
            decimal originalPrice = priceStock?.Price ?? variant.VariantPrice ?? 0;
            var bestDiscount = _productDiscountFacade.GetBestDiscount(variant.ProductId, originalPrice);
            decimal sellingPrice = originalPrice;
            if (bestDiscount != null)
            {
                sellingPrice = _productDiscountFacade.CalculateNewPrice(originalPrice, bestDiscount.DiscountType, bestDiscount.DiscountValue);
            }
            bool hasDiscount = sellingPrice < originalPrice;

            return Json(new
            {

                success = true,
                variant = variant,
                selectedValueIds = selectedValueIds,
                priceStock = priceStock,
                images = images,        // 🔥 Add this

                price = Math.Max(sellingPrice, 0),
                compareAtPrice = hasDiscount ? originalPrice : (decimal?)null
            });
        }

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

        #region Discounts Management 
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
                return Json(new { success = true, message = "Discount removed." });
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
        #endregion


        #region Image Management (Product & Variant)

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SetPrimaryProductImage(int imageId)
        {
            try
            {
                var image = _productFacade.GetImage(imageId);
                if (image == null)
                    return Json(new { success = false, message = "Image not found" });

                var productId = image.ProductId;

                // Get all images for this product
                var allImages = _productFacade.GetImages(productId);

                // Set all to non-primary
                foreach (var img in allImages)
                {
                    img.IsPrimary = false;
                    _productFacade.UpdateImage(img);
                }

                // Set the selected one as primary
                image.IsPrimary = true;
                _productFacade.UpdateImage(image);

                return Json(new { success = true, message = "Primary image updated" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ============================================
        // UPDATE SINGLE IMAGE ORDER
        // ============================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UpdateProductImageOrder(int imageId, int order)
        {
            try
            {
                var image = _productFacade.GetImage(imageId);
                if (image == null)
                    return Json(new { success = false, message = "Image not found" });

                image.SortOrder = order;
                _productFacade.UpdateImage(image);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UpdateVariantImageOrder(int imageId, int order)
        {
            try
            {
                var image = _productVariantFacade.GetImage(imageId);
                if (image == null)
                    return Json(new { success = false, message = "Image not found" });

                image.DisplayOrder = order;
                _productVariantFacade.UpdateImage(image);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ============================================
        // UPDATE MULTIPLE IMAGES ORDER (BULK)
        // ============================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UpdateProductImagesOrder([FromBody] ImageOrderUpdateModel model)
        {
            try
            {
                foreach (var item in model.Images)
                {
                    var image = _productFacade.GetImage(item.Id);
                    if (image != null)
                    {
                        image.SortOrder = item.Order;
                        _productFacade.UpdateImage(image);
                    }
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UpdateVariantImagesOrder([FromBody] ImageOrderUpdateModel model)
        {
            try
            {
                foreach (var item in model.Images)
                {
                    var image = _productVariantFacade.GetImage(item.Id);
                    if (image != null)
                    {
                        image.DisplayOrder = item.Order;
                        _productVariantFacade.UpdateImage(image);
                    }
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }


        }

        #endregion
    }
    public class ImageOrderUpdateModel
    {
        public List<ImageOrderItem> Images { get; set; }
    }

    public class ImageOrderItem
    {
        public int Id { get; set; }
        public int Order { get; set; }
    }
}
