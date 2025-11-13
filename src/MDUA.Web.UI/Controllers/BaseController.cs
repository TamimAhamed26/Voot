using Microsoft.AspNetCore.Mvc;

namespace MDUA.Web.UI.Controllers
{
    public class BaseController : Controller
    {

        public bool IsPermitted(int permissionid)
        {
            // Placeholder logic for permission check
            // In a real application, this would check the user's permissions
            return true; // Assume all permissions are granted for this example
        }
    }
}
