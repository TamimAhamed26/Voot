using MDUA.Entities;
using MDUA.Facade.Interface;
using Microsoft.AspNetCore.Mvc;

namespace MDUA.Web.UI.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductFacade _productFacade;

        // 1. The IProductFacade is injected here, thanks to your DependencyInjection.cs
        public ProductController(IProductFacade productFacade)
        {
            _productFacade = productFacade;
        }

        // 2. This action will handle requests like /Product/1
        [HttpGet("Product/{id}")]
        public IActionResult Index(int id)
        {
            // 3. Call the facade to get all product data in one go
            ProductDetailsModel model = _productFacade.GetProductDetails(id);

            // 4. Check if the product was found
            if (model.Product == null)
            {
                return NotFound(); // Shows a 404 page
            }

            // 5. Pass the complete model to the View
            return View(model);
        }
    }
}