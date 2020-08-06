using AppPartes.Logic;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppPartes.Web.Controllers.Api
{
    public class SearchEditApi : ControllerBase
    {
        private readonly IWorkPartInformation _IWorkPartInformation;
        private readonly IApplicationUserAldakin _manager;
        public SearchEditApi(IWorkPartInformation iWorkPartInformation, IApplicationUserAldakin manager)
        {
            _IWorkPartInformation = iWorkPartInformation;
            _manager = manager;
        }
        private async Task<int> GetIdUserAldakinAsync()
        {
            //TODO Asi recuperamos los datos de aldakin
            var idAldakin = await _manager.GetIdUserAldakin(HttpContext.User);
            if (idAldakin < 1) idAldakin = 0;
            return idAldakin;
        }
    
    }
}
