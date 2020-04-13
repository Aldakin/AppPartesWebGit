using AppPartes.Data.Models;
using AppPartes.Logic;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

//https://programacion.net/articulo/como_resaltar_fechas_especificas_en_jquery_ui_datepicker_1731

namespace AppPartes.Web.Controllers
{
    public class WeekReviewController : Controller
    {
        //apaño para usuario con claims
        private int iUserCondEntO = 0;
        private int iUserId = 0;
        private string strUserName = "";
        private string stUserrDni = "";
        private List<Lineas> listPartes = new List<Lineas>();
        //private readonly AldakinDbContext aldakinDbContext;


        private readonly IWriteDataBase _IWriteDataBase;
        private readonly IWorkPartInformation _IWorkPartInformation;
        private readonly ILoadIndexController _ILoadIndexController;

        public WeekReviewController(IWriteDataBase iWriteDataBase, IWorkPartInformation iWorkPartInformation, ILoadIndexController iLoadIndexController)
        {
            _IWriteDataBase = iWriteDataBase;
            _IWorkPartInformation = iWorkPartInformation;
            _ILoadIndexController = iLoadIndexController;
        }
        public IActionResult Index(string strMessage = "", string strDate = "", string strAction = "", string strId = "")
        {
            ViewBag.Message = strMessage;
            var oView = _ILoadIndexController.LoadWeekController(strDate, strAction, strId);
            if (!(oView.Mensaje is null))
            {
                ViewBag.Message = "ocurrio un error!!!";
            }
            return View(oView);
        }
        [HttpPost]
        public async Task<JsonResult> DeleteLineFunction(int cantidad)
        {
            var lReturn = new List<SelectData>();
            lReturn = await _IWriteDataBase.DeleteWorkerLineAsync(cantidad);
            return Json(lReturn);// RedirectToAction("Index", new { strMessage = "Parte Borrado Satisfactoriamente;", strDate = strReturn, strAction = "loadWeek" });
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CloseWeek(string strDataSelected)
        {
            var oReturn = await _IWriteDataBase.CloseWorkerWeekAsync(strDataSelected);
            if (oReturn.iValue == 0)
            {
                return RedirectToAction("Index", new { strMessage = oReturn.strText, strDate = oReturn.strValue, strAction = "" });
            }
            else
            {
                return RedirectToAction("Index", new { strMessage = oReturn.strText, strDate = oReturn.strValue, strAction = "loadWeek" });
            }
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
        public async Task<IActionResult> EditLine(string strIdLinea, string ot, string strCalendario, string strHoraInicio, string strMinutoInicio, string strHoraFin, string strMinutoFin, string bHorasViaje, string bGastos, string strParte, string strPernoctacion, string strObservaciones, string strGastos)
        {
            var dataEditLine = new WorkerLineData
            {
                strIdLinea = strIdLinea,
                strOt = ot,
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
                strGastos = strGastos
            };
            var oReturn = await _IWriteDataBase.EditWorkerLineAsync(dataEditLine);
            if (oReturn.iValue == 0)
            {
                return RedirectToAction("Index", new { strMessage = oReturn.strText, strDate = oReturn.strValue, strAction = "" });
            }
            else
            {
                return RedirectToAction("Index", new { strMessage = oReturn.strText, strDate = oReturn.strValue, strAction = "loadWeek" });
            }
        }
    }

}