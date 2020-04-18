using AppPartes.Logic;
using AppPartes.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace AppPartes.Web.Controllers.Api
{

    public class MainDataApi : ControllerBase
    {
        private readonly IWorkPartInformation _IWorkPartInformation;
        private readonly IWriteDataBase _IWriteDataBase;
        private readonly ILoadIndexController _ILoadIndexController;
        private readonly UserManager<ApplicationUser> _manager;
        public MainDataApi(IWorkPartInformation iWorkPartInformation, IWriteDataBase iWriteDataBase, ILoadIndexController iLoadIndexController, UserManager<ApplicationUser> manager)
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
        public async Task<MainDataViewLogic> LoadMainView()
        {
            int idAldakinUser =await GetIdUserAldakinAsync();
            var oView =await _ILoadIndexController.LoadMainController(idAldakinUser);
            return oView;
        }
        //[HttpPost]
        //[Route("week-summary")]
        public async Task<List<SelectData>> WeekSummary(string cantidad)
        {
            var listaSelect = new List<SelectData>();
            DateTime dtSelected;
            try
            {
                dtSelected = Convert.ToDateTime(cantidad);
                int idAldakinUser =await GetIdUserAldakinAsync();
                listaSelect =await _IWorkPartInformation.WeekHourResume(dtSelected, idAldakinUser);
            }
            catch (Exception)
            {
                return null;
            }
            return listaSelect;
        }
        public async Task<List<SelectData>> SelectedEntityOt(int cantidad)
        {
            var listaSelect = new List<SelectData>();
            try
            {
                int idAldakinUser =await GetIdUserAldakinAsync();
                listaSelect =await _IWorkPartInformation.SelectedCompanyReadOt(cantidad, idAldakinUser);
            }
            catch (Exception)
            {
                return null;
            }
            return listaSelect;
        }
        public async Task<List<SelectData>> SelectedEntityClient(int cantidad)
        {
            var listaSelect = new List<SelectData>();
            try
            {
                int idAldakinUser =await GetIdUserAldakinAsync();
                listaSelect = await _IWorkPartInformation.SelectedCompanyReadClient(cantidad, idAldakinUser);
            }
            catch (Exception)
            {
                return null;
            }
            return listaSelect;
        }
        public async Task<List<SelectData>> ClientSelected(int cantidad)
        {
            var listaSelect = new List<SelectData>();
            try
            {
                int idAldakinUser =await GetIdUserAldakinAsync();
                listaSelect = await _IWorkPartInformation.SelectedClient(cantidad, idAldakinUser);
            }
            catch (Exception)
            {
                return null;
            }
            return listaSelect;
        }
        public async Task<List<SelectData>> OtSelected(int cantidad)
        {
            var listaSelect = new List<SelectData>();
            try
            {
                int idAldakinUser =await GetIdUserAldakinAsync();
                listaSelect = await _IWorkPartInformation.SelectedOt(cantidad, idAldakinUser);
            }
            catch (Exception)
            {
                return null;
            }
            return listaSelect;
        }
        public async Task<List<SelectData>> GetLevel(int cantidad)
        {
            var listaSelect = new List<SelectData>();
            try
            {
                int idAldakinUser =await GetIdUserAldakinAsync();
                listaSelect = await _IWorkPartInformation.ReadLevelGeneral(cantidad, idAldakinUser);
            }
            catch (Exception)
            {
                return null;
            }
            return listaSelect;
        }
        public async Task<List<SelectData>> GetLevel1(int cantidad)
        {
            var listaSelect = new List<SelectData>();
            try
            {
                int idAldakinUser =await GetIdUserAldakinAsync();
                listaSelect =await _IWorkPartInformation.ReadLevel1(cantidad, idAldakinUser);
            }
            catch (Exception)
            {
                return null;
            }
            return listaSelect;
        }
        public async Task<List<SelectData>> GetLevel2(int cantidad, int cantidad2)
        {
            var listaSelect = new List<SelectData>();
            try
            {
                int idAldakinUser =await GetIdUserAldakinAsync();
                listaSelect =await _IWorkPartInformation.ReadLevel2(cantidad, cantidad2, idAldakinUser);
            }
            catch (Exception)
            {
                return null;
            }
            return listaSelect;
        }
        public async Task<List<SelectData>> SelectPayer(int cantidad, int cantidad2)
        {
            var listaSelect = new List<SelectData>();
            try
            {
                int idAldakinUser = await GetIdUserAldakinAsync();
                listaSelect = await _IWorkPartInformation.SelectedPayer(cantidad, cantidad2, idAldakinUser);
            }
            catch (Exception)
            {
                return null;
            }
            return listaSelect;
        }
        //[HttpPost, ValidateAntiForgeryToken]
        //public async Task<string> InsertLine(WorkerLineData dataToInsertLine)
        //{
        //    var strReturn = string.Empty;
        //    var dataToInsertLine = new WorkerLineData
        //    {
        //        strEntidad = strEntidad,
        //        strOt = strOt,
        //        strPresupuesto = strPresupuesto,
        //        strNivel1 = strNivel1,
        //        strNivel2 = strNivel2,
        //        strNivel3 = strNivel3,
        //        strNivel4 = strNivel4,
        //        strNivel5 = strNivel5,
        //        strNivel6 = strNivel6,
        //        strNivel7 = strNivel7,
        //        strCalendario = strCalendario,
        //        strHoraInicio = strHoraInicio,
        //        strMinutoInicio = strMinutoInicio,
        //        strHoraFin = strHoraFin,
        //        strMinutoFin = strMinutoFin,
        //        bHorasViaje = bHorasViaje,
        //        bGastos = bGastos,
        //        strParte = strParte,
        //        strPernoctacion = strPernoctacion,
        //        strObservaciones = strObservaciones,
        //        strPreslin = strPreslin,
        //        strGastos = strGastos
        //    };
        //    int idAldakinUser = await GetIdUserAldakinAsync();
        //    strReturn = await _IWriteDataBase.InsertWorkerLineAsync(dataToInsertLine, idAldakinUser);

        //    return strReturn;
        //}
    }
}
