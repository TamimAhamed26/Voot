using MDUA.Entities;
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

        //
        // GET: /Home/Index
        // This method simply displays the login page.
        // It passes a 'null' model so the "Login Result" area is hidden.
        [HttpGet]
        public IActionResult Index()
        {
            return View(null);
        }

        //
        // POST: /Home/Index
        // This method handles the form submission when the user clicks "Log In".
        // The 'username' and 'password' parameters must match the 'name' attributes in your HTML form.
        [HttpPost]
        [ValidateAntiForgeryToken] // Important for security
        public IActionResult Index(string username, string password)
        {
            UserLoginResult loginResult;

            // Basic validation
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                loginResult = new UserLoginResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Please enter both username and password."
                };
            }
            else
            {
                // Call the facade with the user's input
                loginResult = _userLoginFacade.GetUserLoginBy(username, password);
            }

            return View(loginResult);
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