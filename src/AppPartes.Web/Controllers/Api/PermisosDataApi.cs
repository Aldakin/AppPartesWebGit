using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppPartes.Logic;
using Microsoft.AspNetCore.Mvc;

namespace AppPartes.Web.Controllers.Api
{
    public class PermisosDataApi : ControllerBase
    {
        private readonly IWorkPartInformation _IWorkPartInformation;
        private readonly IApplicationUserAldakin _IApplicationUserAldakin;
        private int _idAldakinUser;
        public PermisosDataApi(IWorkPartInformation iWorkPartInformation, IApplicationUserAldakin manager)
        {
            _IWorkPartInformation = iWorkPartInformation;
            _IApplicationUserAldakin = manager;
        }
        private async Task<int> GetIdUserAldakinAsync()
        {
            //TODO Asi recuperamos los datos de aldakin
            var idAldakin = await _IApplicationUserAldakin.GetIdUserAldakin(HttpContext.User);
            if (idAldakin < 1) idAldakin = 0;
            return idAldakin;
        }
        public async Task<string> SaveUsers(string users, string worker)
        {
            //var listaSelect = new List<SelectData>();
            var strReturn = string.Empty;
            try
            {
                _idAldakinUser = await _IApplicationUserAldakin.GetIdUserAldakin(HttpContext.User);
                int idAldakinUser = await GetIdUserAldakinAsync();
                strReturn=await _IWorkPartInformation.NewValidationUsersAsync(idAldakinUser, users , worker);

                //listaSelect = await _IWorkPartInformation.WeekHourResume(dtSelected, idAldakinUser);
            }
            catch (Exception)
            {
                strReturn = "Error en el procesamientos de los datos del formulario;";
            }
            return strReturn;
        }
        
        public async Task<string> SaveOTs(string listots, string worker)
        {
            //var listaSelect = new List<SelectData>();
            var strReturn = string.Empty;
            try
            {
                _idAldakinUser = await _IApplicationUserAldakin.GetIdUserAldakin(HttpContext.User);
                int idAldakinUser = await GetIdUserAldakinAsync();
                strReturn = await _IWorkPartInformation.NewValidationOtAsync(idAldakinUser, listots, worker);

                //listaSelect = await _IWorkPartInformation.WeekHourResume(dtSelected, idAldakinUser);
            }
            catch (Exception)
            {
                strReturn = "Error en el procesamientos de los datos del formulario;";
            }
            return strReturn;
        }
    }
}