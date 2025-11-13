using MDUA.Entities; 
using MDUA.Entities.Bases; 
using MDUA.Facade.Interface;
using MDUA.Web.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;



namespace MDUA.Web.UI.Controllers
{
     [Authorize]  
    public class AdminProductController : BaseController
    {
        private readonly IProductFacade _productFacade;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AdminProductController(IProductFacade productFacade, IHttpContextAccessor httpContextAccessor)
        {
            _productFacade = productFacade;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Create()
        {
            if (!IsPermitted(Permission.Product.Create))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var model = new ProductCreateViewModel();
         
            return View(model);
        }

   
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
         
                return View(model);
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

         
                CreatedBy = "admin", 

             
                CompanyId = 1
            };

            try
            {
                _productFacade.Insert(product);

                TempData["SuccessMessage"] = "Product created successfully!";
                return RedirectToAction("Edit", new { id = product.Id }); 
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while saving the product.");
                return View(model);
            }
        }
    }
}