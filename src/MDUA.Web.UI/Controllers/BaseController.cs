using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims; 

namespace MDUA.Web.UI.Controllers
{
    public class BaseController : Controller
    {
 
        public bool IsPermitted(int permissionId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return false;
            }

            // 2. Claim of type "Permission" with the matching value
            // In AccountController,  claims.Add(new Claim("Permission", permId.ToString()));
            return User.HasClaim(c => c.Type == "Permission" && c.Value == permissionId.ToString());
        }
    }
}