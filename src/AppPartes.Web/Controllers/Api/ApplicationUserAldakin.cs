using AppPartes.Web.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AppPartes.Web.Controllers.Api
{
    public class ApplicationUserAldakin : IApplicationUserAldakin
    {
        private readonly UserManager<ApplicationUser> _manager;
        public ApplicationUserAldakin(UserManager<ApplicationUser> manager)//(UserManager<ApplicationUser> manager, AldakinDbContext aldakinDbContext)
        {
            _manager = manager;
        }
        public async Task<int> GetIdUserAldakin(ClaimsPrincipal httpUser)
        {
            ////TODO Asi recuperamos los datos de aldakin
            var user = await _manager.GetUserAsync(httpUser);
            var idAldakin = user.IdAldakin;
            if (idAldakin < 1) idAldakin = 0;
            return idAldakin;
        }
    }
}
