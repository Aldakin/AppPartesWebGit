using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppPartes.Data.Models;
using AppPartes.Logic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppPartes.Web.Controllers.Api
{
    public class SearchDataApi : ControllerBase
    {
        private readonly IWorkPartInformation _IWorkPartInformation;
        private readonly IWriteDataBase _IWriteDataBase;
        private readonly IApplicationUserAldakin _manager;
        public SearchDataApi(IWorkPartInformation iWorkPartInformation, IWriteDataBase iWriteDataBase, IApplicationUserAldakin manager)
        {
            _IWorkPartInformation = iWorkPartInformation;
            _IWriteDataBase = iWriteDataBase;
            _manager = manager;
        }
        private async Task<int> GetIdUserAldakinAsync()
        {
            //TODO Asi recuperamos los datos de aldakin
            var idAldakin = await _manager.GetIdUserAldakin(HttpContext.User);
            if (idAldakin < 1) idAldakin = 0;
            return idAldakin;
        }
        ////[HttpPost]
        //public async Task<List<string>> statusEntityApi(string strDate,  string strEntity)
        //{
        //    var lReturn = new List<string>();
        //    try
        //    {
        //        int idAldakinUser = await GetIdUserAldakinAsync();
        //        lReturn = await _IWorkPartInformation.StatusEntityAsync(idAldakinUser,strDate,  strEntity);
        //    }
        //    catch (Exception ex)
        //    {
        //        lReturn = null;
        //    }
        //    return lReturn;
        //}
    }
}
