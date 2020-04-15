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

namespace AppPartes.Web.Controllers.Api
{
    public class ApplicationUserAldakin : ControllerBase
    {
        private readonly AldakinDbContext _aldakinDbContext;
        private readonly UserManager<ApplicationUser> _manager;
        private Usuarios _user;
        public  ApplicationUserAldakin(UserManager<ApplicationUser> manager, AldakinDbContext aldakinDbContext)
        {
            _aldakinDbContext = aldakinDbContext;
            _manager = manager;
        }
        private async Task<int> GetIdUserAldakin()
        {
            //TODO Asi recuperamos los datos de aldakin
            var user = await _manager.GetUserAsync(HttpContext.User);
            var idAldakin = user.IdAldakin;
            if (idAldakin < 1) idAldakin = 0;
            return idAldakin;
        }
    }
}
