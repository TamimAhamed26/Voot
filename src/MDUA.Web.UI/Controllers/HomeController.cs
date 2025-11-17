using MDUA.Entities;
using MDUA.Facade.Interface;
using MDUA.Web.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MDUA.Web.UI.Controllers
{
    [Authorize] 
    public class HomeController : BaseController 
    {
        private readonly ILogger<HomeController> _logger;

   
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        //
        // GET: /
        // Admin Dashboard page
        //
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

     
    }
}