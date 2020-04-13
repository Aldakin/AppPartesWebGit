using AppPartes.Data.Models;
using AppPartes.Logic;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppPartes.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

//https://developer.mozilla.org/es/docs/Web/JavaScript/Referencia/Objetos_globales/Object/values

//https://developer.mozilla.org/es/docs/Web/JavaScript/Referencia/Objetos_globales/JSON/stringify

//https://geeks.ms/santypr/2015/08/19/asp-net-mvc-cmo-enviar-modelo-con-javascript-desde-un-formulario-a-una-accin/
//https://www.youtube.com/watch?v=6kgYrpugYJA
namespace AppPartes.Web.Controllers
{
    public class MainController : Controller
    {
        private readonly IWorkPartInformation _IWorkPartInformation;
        private readonly IWriteDataBase _IWriteDataBase;
        private readonly ILoadIndexController _ILoadIndexController;
        //private readonly UserManager<ApplicationUser> _manager;

        public MainController(IWorkPartInformation iWorkPartInformation, IWriteDataBase iWriteDataBase, ILoadIndexController iLoadIndexController/*,UserManager<ApplicationUser> manager*/)
        {
            _IWorkPartInformation = iWorkPartInformation;
            _IWriteDataBase = iWriteDataBase;
            _ILoadIndexController = iLoadIndexController;
            //_manager = manager;
        }
        public async Task<IActionResult> Index(string strMessage = "")
        {
            //TODO Asi recuperamos los datos de aldakin
            //var user = await _manager.GetUserAsync(HttpContext.User);
            //var idAldakin = user?.IdAldakin;
            ViewBag.Message = strMessage;
            var oView = _ILoadIndexController.LoadMainController();
            return View(oView);
        }
        [HttpPost]
        [Route("ResumenSemana")]
        public JsonResult ResumenSemana(string cantidad)
        {
            var listaSelect = new List<Logic.SelectData>();
            DateTime dtSelected;
            try
            {
                dtSelected = Convert.ToDateTime(cantidad);
                listaSelect = _IWorkPartInformation.WeekHourResume(dtSelected);
            }
            catch (Exception)
            {
                return Json(null);
            }
            return Json(listaSelect);
        }
        [HttpPost]
        public List<SelectData> EntidadSelectedOt(int cantidad)//JsonResult
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
        [HttpPost]
        public JsonResult EntidadSelectedCliente(int cantidad)//JsonResult
        {
            var listaSelect = new List<Logic.SelectData>();
            try
            {
                listaSelect = _IWorkPartInformation.SelectedCompanyReadClient(cantidad);
            }
            catch (Exception)
            {
                return Json(null);
            }
            return Json(listaSelect);
        }
        [HttpPost]
        public JsonResult ClienteSelected(int cantidad)//JsonResult
        {
            var listaSelect = new List<Logic.SelectData>();
            try
            {
                listaSelect = _IWorkPartInformation.SelectedClient(cantidad);
            }
            catch (Exception)
            {
                return Json(null);
            }
            return Json(listaSelect);
        }
        [HttpPost]
        public JsonResult OtSelected(int cantidad)//JsonResult
        {
            var listaSelect = new List<Logic.SelectData>();
            try
            {
                listaSelect = _IWorkPartInformation.SelectedOt(cantidad);
            }
            catch (Exception)
            {
                return Json(null);
            }
            return Json(listaSelect);
        }
        [HttpPost]
        public JsonResult ObtenerNivel1(int cantidad)
        {
            var listaSelect = new List<Logic.SelectData>();
            try
            {
                listaSelect = _IWorkPartInformation.ReadLevel1(cantidad);
            }
            catch (Exception)
            {
                return Json(null);
            }
            return Json(listaSelect);
        }
        [HttpPost]
        public JsonResult ObtenerNivel2(int cantidad, int cantidad2)
        {
            var listaSelect = new List<Logic.SelectData>();
            try
            {
                listaSelect = _IWorkPartInformation.ReadLevel2(cantidad,cantidad2);
            }
            catch (Exception)
            {
                return Json(null);
            }
            return Json(listaSelect);
        }
        [HttpPost]
        public JsonResult ObtenerNivel(int cantidad)
        {
            var listaSelect = new List<Logic.SelectData>();
            try
            {
                listaSelect = _IWorkPartInformation.ReadLevelGeneral(cantidad);
            }
            catch (Exception)
            {
                return Json(null);
            }
            return Json(listaSelect);
        }
        [HttpPost]
        public JsonResult PagadorSelect(int cantidad, int cantidad2)
        {
            var listaSelect = new List<Logic.SelectData>();
            try
            {
                listaSelect = _IWorkPartInformation.SelectedPayer(cantidad, cantidad2);
            }
            catch (Exception)
            {
                return Json(null);
            }
            return Json(listaSelect);
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

    //.Replace(',', '.')
    //aldakinDbContext.Lineas.Remove //para borrar dato
    //throw new ArgumentException();//forzar exception
}