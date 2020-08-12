using AppPartes.Logic;
using AppPartes.Web.Controllers.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AppPartes.Web.Controllers
{
    [Authorize]
    public class MainController : Controller
    {
        //private readonly IWriteDataBase _IWriteDataBase;
        private readonly IWorkPartInformation _IWorkPartInformation;
        private readonly ILoadIndexController _ILoadIndexController;
        private readonly IApplicationUserAldakin _IApplicationUserAldakin;
        private int _idAldakinUser;
        public MainController(IWorkPartInformation iWorkPartInformation, /*IWriteDataBase iWriteDataBase*/ ILoadIndexController iLoadIndexController, IApplicationUserAldakin iApplicationUserAldakin)
        {
            //_IWriteDataBase = iWriteDataBase;
            _IApplicationUserAldakin = iApplicationUserAldakin;
            _ILoadIndexController = iLoadIndexController;
            _IWorkPartInformation = iWorkPartInformation;
        }
        public async Task<IActionResult> Index(string strMessage = "")
        {
            ViewBag.Message = strMessage;
            _idAldakinUser = await _IApplicationUserAldakin.GetIdUserAldakin(HttpContext.User);
            var oView = await _ILoadIndexController.LoadMainControllerAsync(_idAldakinUser);
            if (!(string.IsNullOrEmpty(oView.strError)))
            {
                ViewBag.Message = oView.strError;
            }
            return View(oView);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> InsertLine(string strEntidad, string strOt, string strPresupuesto, string strNivel1, string strNivel2, string strNivel3, string strNivel4, string strNivel5, string strNivel6, string strNivel7, string strCalendario, string strHoraInicio, string strMinutoInicio, string strHoraFin, string strMinutoFin, string bHorasViaje, string bGastos, string strParte, string strPernoctacion, string strObservaciones, string strPreslin, string strGastos)
        {
            var strReturn = string.Empty;
            _idAldakinUser = await _IApplicationUserAldakin.GetIdUserAldakin(HttpContext.User);
            var dataToInsertLine = new WorkerLineData
            {
                strIdLinea="0",
                strEntidad = strEntidad,
                strOt = strOt,
                strPresupuesto = strPresupuesto,
                iIdUsuario= _idAldakinUser,
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
                strIdlineaAntigua = "0",
                strGastos = strGastos
            };
            //strReturn = await MainDataApi.InsertLine(dataToInsertLine);
            strReturn = await _IWorkPartInformation.PrepareWorkLineAsync(dataToInsertLine, _idAldakinUser, 0, "insert");
            //strReturn = await _IWriteDataBase.InsertWorkerLineAsync(dataToInsertLine, _idAldakinUser);
            return RedirectToAction("Index", new { strMessage = strReturn });
        }
    }
    //.Replace(',', '.')
    //aldakinDbContext.Lineas.Remove //para borrar dato
    //throw new ArgumentException();//forzar exception
}