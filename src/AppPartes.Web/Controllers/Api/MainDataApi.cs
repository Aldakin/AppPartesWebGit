using AppPartes.Logic;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppPartes.Web.Controllers.Api
{
    
    public class MainDataApi : ControllerBase
    {
        private readonly IWorkPartInformation _IWorkPartInformation;
        private readonly IWriteDataBase _IWriteDataBase;
        private readonly ILoadIndexController _ILoadIndexController;
        public MainDataApi(IWorkPartInformation iWorkPartInformation, IWriteDataBase iWriteDataBase, ILoadIndexController iLoadIndexController/*,UserManager<ApplicationUser> manager*/)
        {
            _IWorkPartInformation = iWorkPartInformation;
            _IWriteDataBase = iWriteDataBase;
            _ILoadIndexController = iLoadIndexController;
            //_manager = manager;
        }
        //[HttpPost]
        //[Route("week-summary")]
        public List<SelectData> WeekSummary(string cantidad)
        {
            var listaSelect = new List<SelectData>();
            DateTime dtSelected;
            try
            {
                dtSelected = Convert.ToDateTime(cantidad);
                listaSelect = _IWorkPartInformation.WeekHourResume(dtSelected);
            }
            catch (Exception)
            {
                return null;
            }
            return listaSelect;
        }
        public List<SelectData> SelectedEntityOt(int cantidad)//JsonResult
        {
            var listaSelect = new List<SelectData>();
            try
            {
                listaSelect = _IWorkPartInformation.SelectedCompanyReadOt(cantidad);
            }
            catch (Exception)
            {
                return null;
            }
            return listaSelect;
        }
        public List<SelectData> SelectedEntityClient(int cantidad)//JsonResult
        {
            var listaSelect = new List<SelectData>();
            try
            {
                listaSelect = _IWorkPartInformation.SelectedCompanyReadClient(cantidad);
            }
            catch (Exception)
            {
                return null;
            }
            return listaSelect;
        }
        public List<SelectData> ClientSelected(int cantidad)//JsonResult
        {
            var listaSelect = new List<SelectData>();
            try
            {
                listaSelect = _IWorkPartInformation.SelectedClient(cantidad);
            }
            catch (Exception)
            {
                return null;
            }
            return listaSelect;
        }
        public List<SelectData> OtSelected(int cantidad)//JsonResult
        {
            var listaSelect = new List<SelectData>();
            try
            {
                listaSelect = _IWorkPartInformation.SelectedOt(cantidad);
            }
            catch (Exception)
            {
                return null;
            }
            return listaSelect;
        }
        public List<SelectData> GetLevel(int cantidad)
        {
            var listaSelect = new List<SelectData>();
            try
            {
                listaSelect = _IWorkPartInformation.ReadLevelGeneral(cantidad);
            }
            catch (Exception)
            {
                return null;
            }
            return listaSelect;
        }
        public List<SelectData> GetLevel1(int cantidad)
        {
            var listaSelect = new List<SelectData>();
            try
            {
                listaSelect = _IWorkPartInformation.ReadLevel1(cantidad);
            }
            catch (Exception)
            {
                return null;
            }
            return listaSelect;
        }
        public List<SelectData> GetLevel2(int cantidad, int cantidad2)
        {
            var listaSelect = new List<SelectData>();
            try
            {
                listaSelect = _IWorkPartInformation.ReadLevel2(cantidad, cantidad2);
            }
            catch (Exception)
            {
                return null;
            }
            return listaSelect;
        }
        public List<SelectData> SelectPayer(int cantidad, int cantidad2)
        {
            var listaSelect = new List<SelectData>();
            try
            {
                listaSelect = _IWorkPartInformation.SelectedPayer(cantidad, cantidad2);
            }
            catch (Exception)
            {
                return null;
            }
            return listaSelect;
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> InsertLine(string strEntidad, string strOt, string strPresupuesto, string strNivel1, string strNivel2, string strNivel3, string strNivel4, string strNivel5, string strNivel6, string strNivel7, string strCalendario, string strHoraInicio, string strMinutoInicio, string strHoraFin, string strMinutoFin, string bHorasViaje, string bGastos, string strParte, string strPernoctacion, string strObservaciones, string strPreslin, string strGastos)
        {
            var strReturn = string.Empty;
            var dataToInsertLine = new WorkerLineData
            {
                strEntidad = strEntidad,
                strOt = strOt,
                strPresupuesto = strPresupuesto,
                strNivel1 = strNivel1,
                strNivel2 = strNivel2,
                strNivel3 = strNivel3,
                strNivel4 = strNivel4,
                strNivel5 = strNivel5,
                strNivel6 = strNivel6,
                strNivel7 = strNivel7,
                strCalendario = strCalendario,
                strHoraInicio = strHoraInicio,
                strMinutoInicio = strMinutoInicio,
                strHoraFin = strHoraFin,
                strMinutoFin = strMinutoFin,
                bHorasViaje = bHorasViaje,
                bGastos = bGastos,
                strParte = strParte,
                strPernoctacion = strPernoctacion,
                strObservaciones = strObservaciones,
                strPreslin = strPreslin,
                strGastos = strGastos
            };
            strReturn = await _IWriteDataBase.InsertWorkerLineAsync(dataToInsertLine);

            return RedirectToAction("Index", new { strMessage = strReturn });
        }
    }
}
