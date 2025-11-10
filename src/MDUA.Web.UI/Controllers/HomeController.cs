using MDUA.Entities;
using MDUA.Facade;
using MDUA.Facade.Interface;
using MDUA.Framework;
using MDUA.Web.UI.Models;
using Microsoft.AspNetCore.Mvc; 
using System.Diagnostics;

namespace MDUA.Web.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IUserLoginFacade _userLoginFacade;
        public HomeController(
                 IUserLoginFacade userLoginFacade, ILogger<HomeController> logger)
        {
            _userLoginFacade = userLoginFacade;
            _logger = logger;
        }

        public IActionResult Index()
        {
            DateTime.Now.ToStringDDMMYYYY(); 
            Account ac = new Account();
            ac.AccountTitle = string.Empty;

            //_userLoginFacade.GetUserLoginBy("email.com", "sss");

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
