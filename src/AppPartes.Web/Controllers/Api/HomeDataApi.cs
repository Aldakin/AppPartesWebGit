using AppPartes.Logic;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppPartes.Web.Controllers.Api
{
    public class HomeDataApi : ControllerBase
    {
        private readonly IWriteDataBase _IWriteDataBase;
        private readonly IApplicationUserAldakin _manager;
        public HomeDataApi(IWriteDataBase iWriteDataBase, IApplicationUserAldakin manager)
        {
            _IWriteDataBase = iWriteDataBase;
            _manager = manager;
        }
        public async Task<string> UpdateEntityData(int iEntity)
        {
            string strReturn = "Ha ocurrido un error en la orden de actualizar la Entidad";
            var idAldakin = await _manager.GetIdUserAldakin(HttpContext.User);
            if (iEntity > 0) strReturn = await _IWriteDataBase.UpdateEntityDataOrCsvAsync(iEntity, idAldakin, "AC");
            return strReturn;
        }
        public async Task<string> GenerateCsvData(int iEntity)
        {
            string strReturn = "Ha ocurrido un error en la orden de actualizar la Entidad";
            var idAldakin = await _manager.GetIdUserAldakin(HttpContext.User);
            if (iEntity > 0) strReturn = await _IWriteDataBase.UpdateEntityDataOrCsvAsync(iEntity, idAldakin, "CS");
            return strReturn;
        }

    }
}
