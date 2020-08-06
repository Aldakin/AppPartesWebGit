using AppPartes.Logic;
using AppPartes.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppPartes.Web.Controllers.Api
{
    public class SearchDataApi : ControllerBase
    {
        private readonly IWriteDataBase _IWriteDataBase;
        private readonly IApplicationUserAldakin _manager;
        private readonly IWorkPartInformation _IWorkPartInformation;
        public SearchDataApi( IWriteDataBase iWriteDataBase, IApplicationUserAldakin manager, IWorkPartInformation iWorkPartInformation)
        {
            _IWriteDataBase = iWriteDataBase;
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
        public async Task<List<SelectData>> GetWorker(int cantidad)
        {
            var listaSelect = new List<SelectData>();
            try
            {
                int idAldakinUser = await GetIdUserAldakinAsync();
                listaSelect = await _IWorkPartInformation.GetWorkerValidationAsnc(idAldakinUser, cantidad);
            }
            catch (Exception)
            {
                return null;
            }
            return listaSelect;
        }
        public async Task<List<SelectData>> GetOt(int cantidad)
        {
            var listaSelect = new List<SelectData>();
            try
            {
                int idAldakinUser = await GetIdUserAldakinAsync();
                listaSelect = await _IWorkPartInformation.GetOtValidationAsync(idAldakinUser, cantidad);
            }
            catch (Exception)
            {
                return null;
            }
            return listaSelect;
        }
        //public async Task<SelectData> validateLineFunc(string strLine)
        //{
        //    var lReturn = new SelectData();
        //    int idAldakin = await GetIdUserAldakinAsync();
        //    lReturn = await _IWriteDataBase.ValidateWorkerLineAsync(strLine, idAldakin);
        //    return lReturn;
        //}
    }
}