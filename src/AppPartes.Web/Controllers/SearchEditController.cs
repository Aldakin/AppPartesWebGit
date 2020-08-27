using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppPartes.Logic;
using AppPartes.Web.Controllers.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppPartes.Web.Controllers
{
    public class SearchEditController : Controller
    {
        private readonly ILoadIndexController _iLoadIndexController;
        private readonly IApplicationUserAldakin _iApplicationUserAldakin;
        private readonly IWorkPartInformation _iWorkPartInformation;
        //private readonly IWriteDataBase _IWriteDataBase;
        int _idAldakinUser;
        public SearchEditController(IApplicationUserAldakin iApplicationUserAldakin, ILoadIndexController iLoadIndexController, IWorkPartInformation iWorkPartInformation/*, IWriteDataBase iWriteDataBase*/)
        {
            _iApplicationUserAldakin = iApplicationUserAldakin;
            _iLoadIndexController = iLoadIndexController;
            _iWorkPartInformation = iWorkPartInformation;
            //_IWriteDataBase = iWriteDataBase;
        }
        public async Task<IActionResult> Index(string strMessage = "", string strLineId = "", string strAction = "")
        {
            ViewBag.Message = strMessage;
            _idAldakinUser = await _iApplicationUserAldakin.GetIdUserAldakin(HttpContext.User);
            var oView = await _iLoadIndexController.LoadSearchEditControllerAsync(_idAldakinUser, strLineId, strAction);
            if (oView.bLevelError) return RedirectToAction("Index", "Home", new { strMessage = "No tiene permiso de acceso a la página" });
            return View(oView);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLine(string strEntidad, string strOt, string strPresupuesto, string strNivel1, string strNivel2, string strNivel3, string strNivel4, string strNivel5, string strNivel6, string strNivel7, string strCalendario, string strHoraInicio, string strMinutoInicio, string strHoraFin, string strMinutoFin, string bHorasViaje, string bGastos, string strParte, string strPernoctacion, string strObservaciones, string strPreslin, string strGastos, string strMessage, string strIdLinea, string strIdUser, string SaveAndValidate, string Save, string Validate)
        {
            var strReturn = string.Empty;
            _idAldakinUser = await _iApplicationUserAldakin.GetIdUserAldakin(HttpContext.User);
            string strAction = string.Empty;
            // SaveAndValidate,string Save,string Validate
            if (!(string.IsNullOrEmpty(SaveAndValidate)))
            {
                strAction = "SaveAndValidate";
            }
            else
            {
                if (!(string.IsNullOrEmpty(Save)))
                {
                    strAction = "Save";
                }
                else
                {
                    if (!(string.IsNullOrEmpty(Validate)))
                    {
                        strAction = "Validate";
                    }
                    else
                    {
                        strAction = string.Empty;
                    }
                }
            }
            var dataEditLine = new WorkerLineData
            {
                iIdUsuario = Convert.ToInt32(strIdUser),
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

            strReturn = await _iWorkPartInformation.PrepareWorkLineAsync(dataEditLine, Convert.ToInt32(strIdUser), _idAldakinUser, "edit");
            // strReturn = await _IWriteDataBase.EditWorkerLineAdminAsync(dataToInsertLine);
            return RedirectToAction("Index", "Search", new { strMessage = strReturn, strAction = "StatusResume", strDate1 = strCalendario, strWorker = strIdUser, strEntity = strEntidad, strOt = 0 });



            //return RedirectToAction("Index", "Search", new { strMessage = "Parte editado salisfactoriamente",action="StatusResume",strDate1= strCalendario,strWorker= strIdUser });
            //return RedirectToAction("Index", new { strMessage = "aqui" });
            //return RedirectToAction("Index", new { strMessage = strMessage, action = strAction, strDate1 = oReturn.strValue, strEntity = Convert.ToString(oReturn.iValue) });
        }
    }
}