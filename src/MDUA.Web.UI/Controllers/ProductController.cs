using MDUA.Entities;
using MDUA.Facade;
using MDUA.Facade.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public ProductController(
            IProductFacade productFacade,
            IProductAttributeFacade productAttributeFacade,
            IAttributeNameFacade attributeNameFacade,
            IAttributeValueFacade attributeValueFacade,
            IProductVariantFacade productVariantFacade,
            IVariantPriceStockFacade variantPriceStockFacade,
            IVariantAttributeValueFacade variantAttributeValueFacade,
            IProductDiscountFacade productDiscountFacade)
        {
            _productFacade = productFacade;
            _productAttributeFacade = productAttributeFacade;
            _attributeNameFacade = attributeNameFacade;
            _attributeValueFacade = attributeValueFacade;
            _productVariantFacade = productVariantFacade;
            _variantPriceStockFacade = variantPriceStockFacade;
            _variantAttributeValueFacade = variantAttributeValueFacade;
            _productDiscountFacade = productDiscountFacade;
        }

        [HttpGet("product/{slug}")]
        public IActionResult Index(string slug)
        {
            ProductDetailsModel model = _productFacade.GetProductDetails(slug);

            if (model.Product == null)
                return NotFound();

            model.Options = new List<ProductAttributeOptionModel>();

            var productAttributes = _productAttributeFacade.GetByProductId(model.Product.Id);

            foreach (var attr in productAttributes.OrderBy(a => a.DisplayOrder))
            {
                var attrName = _attributeNameFacade.Get(attr.AttributeId);
                var attrValues = _attributeValueFacade.GetByAttributeId(attr.AttributeId);

                if (attrName != null && attrValues != null && attrValues.Count > 0)
                {
                    model.Options.Add(new ProductAttributeOptionModel
                    {
                        AttributeId = attr.AttributeId,
                        AttributeName = attrName.Name,
                        Values = attrValues
                    });
                }
            }

            return View(model);
        }

        
    }
}
