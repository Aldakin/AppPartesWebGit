using AppPartes.Logic;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using AppPartes.Web.Models;
using AppPartes.Data.Models;
using Microsoft.AspNetCore.Http;
using System.Web;
using System.Security.Claims;

namespace AppPartes.Web.Controllers.Api
{
    public class ApplicationUserAldakin : IApplicationUserAldakin
    {
        private readonly UserManager<ApplicationUser> _manager;
        public  ApplicationUserAldakin( UserManager<ApplicationUser> manager)//(UserManager<ApplicationUser> manager, AldakinDbContext aldakinDbContext)
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
