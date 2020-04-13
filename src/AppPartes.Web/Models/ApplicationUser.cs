using Microsoft.AspNetCore.Identity;

namespace AppPartes.Web.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int IdAldakin { get; set; }
    }
}
