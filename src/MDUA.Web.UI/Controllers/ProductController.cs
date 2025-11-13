using MDUA.Entities;
using MDUA.Facade.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO.Pipelines;
using System.Net;

namespace MDUA.Web.UI.Controllers
{  
    public class ProductController : BaseController
    {
        private readonly IProductFacade _productFacade;

        public ProductController(IProductFacade productFacade)
        {
            _productFacade = productFacade;
        }

        [HttpGet("product/{slug}")]
        public IActionResult Index(string slug)
        {
            ProductDetailsModel model = _productFacade.GetProductDetails(slug);

            if (model.Product == null)
                return NotFound();

            return View(model);
        }


    }
}