using AppPartes.Logic;
using AppPartes.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppPartes.Web.Controllers.Api
{
    public class WeekDataApi : ControllerBase
    {
        private readonly IWorkPartInformation _IWorkPartInformation;
        private readonly IWriteDataBase _IWriteDataBase;
        private readonly IApplicationUserAldakin _manager;
        public WeekDataApi(IWorkPartInformation iWorkPartInformation, IWriteDataBase iWriteDataBase, IApplicationUserAldakin manager)//UserManager<ApplicationUser>
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
        public async Task<List<SelectData>> SelectPayer(int cantidad, int cantidad2)
        {
            var listaSelect = new List<SelectData>();
            try
            {
                int idAldakin = await GetIdUserAldakinAsync();
                listaSelect = await _IWorkPartInformation.SelectedPayerAsync(cantidad, cantidad2, idAldakin);
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
            int idAldakin = await GetIdUserAldakinAsync();
            lReturn = await _IWriteDataBase.DeleteWorkerLineAsync(cantidad, idAldakin, idAldakin);
            return lReturn;
        }
        public async Task<SelectData> CloseFunction(string strDataSelected)
        {
            var lReturn = new SelectData();
            int idAldakin = await GetIdUserAldakinAsync();
            lReturn = await _IWriteDataBase.CloseWorkerWeekAsync(strDataSelected, idAldakin);
            return lReturn;
        }
        public async Task<List<SelectData>> WeekSummary(string cantidad)
        {
            var listaSelect = new List<SelectData>();
            DateTime dtSelected;
            try
            {
                dtSelected = Convert.ToDateTime(cantidad);
                int idAldakinUser = await GetIdUserAldakinAsync();
                listaSelect = await _IWorkPartInformation.WeekHourResume(dtSelected, idAldakinUser);
            }
            catch (Exception)
            {
                return null;
            }
            return listaSelect;
        }
        //public int iValue { set; get; }
        //public string strText { set; get; }
        //public string strValue { set; get; }
    }
}
