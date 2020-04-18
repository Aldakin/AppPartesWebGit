using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AppPartes.Web.Controllers.Api
{
    public interface IApplicationUserAldakin
    {
        Task<int> GetIdUserAldakin(ClaimsPrincipal httpUser);
    }
}
