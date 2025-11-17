using System.ComponentModel.DataAnnotations;

namespace MDUA.Web.UI.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; } = false;
        public string? ReturnUrl { get; set; }
    }
}