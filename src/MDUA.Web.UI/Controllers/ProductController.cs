using MDUA.Entities;
using MDUA.Entities.List;
using MDUA.Facade;
using MDUA.Facade.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting; // Required for accessing wwwroot
using Microsoft.AspNetCore.Mvc;
using System.IO; // Required for Path and File checks
using System.IO.Pipelines;
using System.Net;
using static MDUA.Entities.ProductDiscount;

namespace MDUA.Web.UI.Controllers
{
    public class ProductController : BaseController
    {
        private readonly IProductFacade _productFacade;
        private readonly IProductDiscountFacade _productDiscountFacade;
        private readonly IProductAttributeFacade _productAttributeFacade;
        private readonly IAttributeNameFacade _attributeNameFacade;
        private readonly IAttributeValueFacade _attributeValueFacade;
        private readonly IProductVariantFacade _productVariantFacade;
        private readonly IVariantPriceStockFacade _variantPriceStockFacade;
        private readonly IVariantAttributeValueFacade _variantAttributeValueFacade;
        private readonly ICompanyFacade _companyFacade;
        private readonly IWebHostEnvironment _webHostEnvironment; // Added to access physical paths
        private readonly IOrderFacade _orderFacade;
        public ProductController(
            IProductFacade productFacade,
            IProductAttributeFacade productAttributeFacade,
            IAttributeNameFacade attributeNameFacade,
            IAttributeValueFacade attributeValueFacade,
            IProductVariantFacade productVariantFacade,
            IOrderFacade orderFacade, // 2. Inject it here
            IVariantPriceStockFacade variantPriceStockFacade,
            IVariantAttributeValueFacade variantAttributeValueFacade,
            IProductDiscountFacade productDiscountFacade,
            ICompanyFacade companyFacade,
            IWebHostEnvironment webHostEnvironment) // Inject Environment
        {
            _orderFacade = orderFacade; // 3. Assign it
            _productFacade = productFacade;
            _productAttributeFacade = productAttributeFacade;
            _attributeNameFacade = attributeNameFacade;
            _attributeValueFacade = attributeValueFacade;
            _productVariantFacade = productVariantFacade;
            _variantPriceStockFacade = variantPriceStockFacade;
            _variantAttributeValueFacade = variantAttributeValueFacade;
            _productDiscountFacade = productDiscountFacade;
            _companyFacade = companyFacade;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("product/{slug}")]
        public IActionResult Index(string slug)
        {
            ProductDetailsModel model = _productFacade.GetProductDetails(slug);
            if (model.Product == null)
                return NotFound();

            // --- REAL-WORLD DYNAMIC LOGO LOGIC START ---

            // 1. Set Default Fallback
            string logoUrl = "/images/products/logo.png";
            string companyName = "Company";

            if (model.Product.CompanyId > 0)
            {
                var company = _companyFacade.Get(model.Product.CompanyId);
                if (company != null)
                {
                    companyName = company.CompanyName;

                    // 2. Get the physical root path (e.g., C:\Voot-master\src\MDUA.Web.UI\wwwroot)
                    string webRootPath = _webHostEnvironment.WebRootPath;

                    // 3. Construct the specific file path safely
                    // Path.Combine handles slashes correctly for Windows vs Linux servers
                    string pathToCheck = Path.Combine(webRootPath, "images", company.Id.ToString(), "logo.png");

                    // 4. Check if the file actually exists on the disk
                    if (System.IO.File.Exists(pathToCheck))
                    {
                        // If it exists, set the URL path
                        logoUrl = $"/images/{company.Id}/logo.png";
                    }
                }
            }

            ViewBag.CompanyLogoPath = logoUrl;
            ViewBag.CompanyName = companyName;

            // --- REAL-WORLD DYNAMIC LOGO LOGIC END ---

            model.Options = new List<ProductAttributeOptionModel>();

            // Get all variants for this product
            var variants = _productVariantFacade.GetByProductId(model.Product.Id);

            if (variants != null && variants.Count > 0)
            {
                // Dictionary to store unique attribute values per attribute
                var attributeValuesDict = new Dictionary<int, HashSet<int>>();

                // Collect all unique attribute values from all variants
                foreach (var variant in variants)
                {
                    var variantAttributeValues = _variantAttributeValueFacade.GetByVariantId(variant.Id);

                    foreach (var vav in variantAttributeValues)
                    {
                        if (!attributeValuesDict.ContainsKey(vav.AttributeId))
                        {
                            attributeValuesDict[vav.AttributeId] = new HashSet<int>();
                        }
                        attributeValuesDict[vav.AttributeId].Add(vav.AttributeValueId);
                    }
                }

                // Get product attributes for display order
                var productAttributes = _productAttributeFacade.GetByProductId(model.Product.Id);

                // Build the options list with only the values used in variants
                foreach (var attr in productAttributes.OrderBy(a => a.DisplayOrder))
                {
                    if (attributeValuesDict.ContainsKey(attr.AttributeId))
                    {
                        var attrName = _attributeNameFacade.Get(attr.AttributeId);

                        // Get only the attribute values that are actually used in variants
                        var usedValueIds = attributeValuesDict[attr.AttributeId];
                        var attrValues = new AttributeValueList();

                        foreach (var valueId in usedValueIds)
                        {
                            var attrValue = _attributeValueFacade.Get(valueId);
                            if (attrValue != null)
                            {
                                attrValues.Add(attrValue);
                            }
                        }

                        if (attrName != null && attrValues.Count > 0)
                        {
                            model.Options.Add(new ProductAttributeOptionModel
                            {
                                AttributeId = attr.AttributeId,
                                AttributeName = attrName.Name,
                                Values = attrValues
                            });
                        }
                    }
                }
            }

            return View(model);
        }
        

        [HttpPost("product/place-order")]
        public IActionResult PlaceOrder([FromBody] ProductPageOrderModel orderPayload)
        {
            if (orderPayload == null) return BadRequest("Invalid Order Data received (null).");

            try
            {
                // The Facade now handles CompanyId internally
                long orderId = _orderFacade.PlaceOnlineOrder(orderPayload);

                return Ok(new { success = true, message = "Order placed successfully!", orderId = orderId });
            }
            catch (Exception ex)
            {
                // Dig for the deepest exception message (usually the SQL error)
                string errorMessage = ex.Message;
                var inner = ex.InnerException;
                while (inner != null)
                {
                    errorMessage += " | " + inner.Message;
                    inner = inner.InnerException;
                }

                Console.WriteLine($"[ORDER ERROR]: {errorMessage}");

                return StatusCode(500, new
                {
                    success = false,
                    message = "Server Error: " + errorMessage
                });
            }
        }

        [HttpGet("product/check-customer/{phone}")]
        public IActionResult CheckCustomer(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return BadRequest();

            try
            {
                // Call Facade instead of DA directly
                var result = _orderFacade.CheckCustomer(phone);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { found = false, message = ex.Message });
            }
        }
    }
}