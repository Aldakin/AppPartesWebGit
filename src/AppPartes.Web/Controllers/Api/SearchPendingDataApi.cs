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
    //[Route("api/[controller]")]
    //[ApiController]
    public class SearchPendingDataApi : ControllerBase
    {
        private readonly IWorkPartInformation _IWorkPartInformation;
        private readonly IWriteDataBase _IWriteDataBase;
        private readonly IApplicationUserAldakin _manager;
        public SearchPendingDataApi(IWorkPartInformation iWorkPartInformation, IWriteDataBase iWriteDataBase, IApplicationUserAldakin manager)
        {
            _IWorkPartInformation = iWorkPartInformation;
            _IWriteDataBase = iWriteDataBase;
            _manager = manager;
        }
        private async Task<int> GetIdUserAldakinAsync()
        {
            var idAldakin = await _manager.GetIdUserAldakin(HttpContext.User);
            if (idAldakin < 1) idAldakin = 0;
            return idAldakin;
        }
        //[HttpPost]
        public async Task<List<string>> ReviewPending(string strDate, string strUser, string strEntity)
        {
            var lReturn = new List<string>();
            try
            {
                lReturn = await _IWorkPartInformation.PendingWorkPartApiAsync(strDate, strUser, strEntity);
            }
            catch (Exception ex)
            {
                lReturn = null;
            }
            return lReturn;
        }
    }
}