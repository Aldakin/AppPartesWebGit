using AppPartes.Logic;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using AppPartes.Web.Models;

namespace AppPartes.Web.Controllers.Api
{
    public class WeekDataApi : ControllerBase
    {
        private readonly IWorkPartInformation _IWorkPartInformation;
        private readonly IWriteDataBase _IWriteDataBase;
        private readonly ILoadIndexController _ILoadIndexController;
        private readonly UserManager<ApplicationUser> _manager;
        public WeekDataApi(IWorkPartInformation iWorkPartInformation, IWriteDataBase iWriteDataBase, ILoadIndexController iLoadIndexController,UserManager<ApplicationUser> manager)
        {
            _IWorkPartInformation = iWorkPartInformation;
            _IWriteDataBase = iWriteDataBase;
            _ILoadIndexController = iLoadIndexController;
            _manager = manager;
        }
        private async Task<int> GetIdUserAldakinAsync()
        {
            //TODO Asi recuperamos los datos de aldakin
            var user = await _manager.GetUserAsync(HttpContext.User);
            var idAldakin = user.IdAldakin;
            if (idAldakin < 1) idAldakin = 0;
            return idAldakin;
        }
        public async Task<List<SelectData>> SelectPayer(int cantidad, int cantidad2)
        {
            var listaSelect = new List<SelectData>();
            try
            {
                int idAldakin =await GetIdUserAldakinAsync();
                   listaSelect =await _IWorkPartInformation.SelectedPayer(cantidad, cantidad2, idAldakin);
            }
            catch (Exception)
            {
                return null;
            }
            return listaSelect;
        }
        public async Task<List<SelectData>> DeleteLineFunction(int cantidad)
        {
            var lReturn = new List<SelectData>();
            int idAldakin =await GetIdUserAldakinAsync();
            lReturn = await _IWriteDataBase.DeleteWorkerLineAsync(cantidad, idAldakin);
            return lReturn;
        }
        public async Task<SelectData> CloseFunction(string strDataSelected)
        {
            var lReturn = new SelectData();
            int idAldakin = await GetIdUserAldakinAsync();
            lReturn = await _IWriteDataBase.CloseWorkerWeekAsync(strDataSelected, idAldakin);
            return lReturn;
        }
        public int iValue { set; get; }
        public string strText { set; get; }
        public string strValue { set; get; }
    }
}
