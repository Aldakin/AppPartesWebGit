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


        //private readonly IWriteDataBase _iWriteDataBase;
        private readonly IWorkPartInformation _iWorkPartInformation;
        private readonly ILoadIndexController _iILoadIndexController;
        private readonly IApplicationUserAldakin _iApplicationUserAldakin;
        private int _idAldakinUser;

        public WeekReviewController(/*IWriteDataBase iWriteDataBase,*/ IWorkPartInformation iWorkPartInformation, ILoadIndexController iLoadIndexController, IApplicationUserAldakin iApplicationUserAldakin)
        {
            //_iWriteDataBase = iWriteDataBase;
            _iWorkPartInformation = iWorkPartInformation;
            _iILoadIndexController = iLoadIndexController;
            _iApplicationUserAldakin = iApplicationUserAldakin;
        }
        public async Task<IActionResult> Index(string strMessage = "", string strDate = "", string strAction = "", string strId = "")
        {
            ViewBag.Message = strMessage;
            _idAldakinUser = await _iApplicationUserAldakin.GetIdUserAldakin(HttpContext.User);
            var oView = await _iILoadIndexController.LoadWeekControllerAsync(_idAldakinUser, strDate, strAction, strId);
            if (!(string.IsNullOrEmpty(oView.strError)))
            {
                ViewBag.Message = oView.strError;
            }
            if (!(oView.Mensaje is null))
            {
                ViewBag.Message = oView.Mensaje;
            }

            return View(oView);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLine(string strEntidad, string strOt, string strPresupuesto, string strNivel1, string strNivel2, string strNivel3, string strNivel4, string strNivel5, string strNivel6, string strNivel7, string strCalendario, string strHoraInicio, string strMinutoInicio, string strHoraFin, string strMinutoFin, string bHorasViaje, string bGastos, string strParte, string strPernoctacion, string strObservaciones, string strPreslin, string strGastos, string strMessage, string strIdLinea, string strIdUser, string SaveAndValidate, string Save, string Validate)
        {

            _idAldakinUser = await _iApplicationUserAldakin.GetIdUserAldakin(HttpContext.User);
            string strAction=string.Empty;
            var dataEditLine = new WorkerLineData
            {
                iIdUsuario = _idAldakinUser,
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
                strGastos = strGastos,
                strMensaje = strMessage,
                strIdlineaAntigua = strIdLinea,
                strAction = strAction
            };
            //var oReturn = await _IWriteDataBase.EditWorkerLineAsync(dataEditLine, _idAldakinUser);

            var strReturn = await _iWorkPartInformation.PrepareWorkLineAsync(dataEditLine, _idAldakinUser, _idAldakinUser, "edit");



            if (string.IsNullOrEmpty(strReturn))
            {
                return RedirectToAction("Index", new { strMessage = strReturn, strDate = strCalendario, strAction = "" });
            }
            else
            {
                return RedirectToAction("Index", new { strMessage = strReturn, strDate = strCalendario, strAction = "loadWeek" });
            }
        }
    }

}