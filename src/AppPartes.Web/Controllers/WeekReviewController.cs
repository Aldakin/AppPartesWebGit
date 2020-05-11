using AppPartes.Logic;
using AppPartes.Web.Controllers.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

//https://programacion.net/articulo/como_resaltar_fechas_especificas_en_jquery_ui_datepicker_1731

namespace AppPartes.Web.Controllers
{
    [Authorize]
    public class WeekReviewController : Controller
    {
        //apaño para usuario con claims
        //private int iUserCondEntO = 0;
        //private int iUserId = 0;
        //private string strUserName = "";
        //private string stUserrDni = "";
        //private List<Lineas> listPartes = new List<Lineas>();
        //private readonly AldakinDbContext aldakinDbContext;


        private readonly IWriteDataBase _IWriteDataBase;
        private readonly IWorkPartInformation _IWorkPartInformation;
        private readonly ILoadIndexController _ILoadIndexController;
        private readonly IApplicationUserAldakin _IApplicationUserAldakin;
        private int _idAldakinUser;

        public WeekReviewController(IWriteDataBase iWriteDataBase, IWorkPartInformation iWorkPartInformation, ILoadIndexController iLoadIndexController, IApplicationUserAldakin iApplicationUserAldakin)
        {
            _IWriteDataBase = iWriteDataBase;
            _IWorkPartInformation = iWorkPartInformation;
            _ILoadIndexController = iLoadIndexController;
            _IApplicationUserAldakin = iApplicationUserAldakin;
        }
        public async Task<IActionResult> Index(string strMessage = "", string strDate = "", string strAction = "", string strId = "")
        {
            ViewBag.Message = strMessage;
            _idAldakinUser = await _IApplicationUserAldakin.GetIdUserAldakin(HttpContext.User);
            var oView = await _ILoadIndexController.LoadWeekControllerAsync(_idAldakinUser, strDate, strAction, strId);
            if (!(oView.Mensaje is null))
            {
                ViewBag.Message = "ocurrio un error!!!";
            }
            return View(oView);
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
            _idAldakinUser = await _IApplicationUserAldakin.GetIdUserAldakin(HttpContext.User);
            var oReturn = await _IWriteDataBase.EditWorkerLineAsync(dataEditLine, _idAldakinUser);
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