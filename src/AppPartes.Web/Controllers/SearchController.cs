using System;
using System.Threading.Tasks;
using AppPartes.Logic;
using AppPartes.Web.Controllers.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppPartes.Web.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    [Authorize]
    public class SearchController : Controller
    {
        private readonly ILoadIndexController _iLoadIndexController;
        private readonly IApplicationUserAldakin _iApplicationUserAldakin;
        private readonly IWorkPartInformation _iWorkPartInformation;
        private readonly IWriteDataBase _iWriteDataBase;
        int _idAldakinUser;
        public SearchController(IApplicationUserAldakin iApplicationUserAldakin, ILoadIndexController iLoadIndexController, IWorkPartInformation iWorkPartInformation, IWriteDataBase iWriteDataBase)
        {
            _iApplicationUserAldakin = iApplicationUserAldakin;
            _iLoadIndexController = iLoadIndexController;
            _iWriteDataBase = iWriteDataBase;
        }
        public async Task<IActionResult> Index(string strMessage = "", string strDate = "", string strDate1 = "", string strEntity = "", string strAction = "", string strOt = "", string strWorker = "", string strListValidation = "")
        {
            ViewBag.Message = strMessage;
            _idAldakinUser = await _iApplicationUserAldakin.GetIdUserAldakin(HttpContext.User);
            var oView = await _iLoadIndexController.LoadSearchControllerAsync(_idAldakinUser, strDate, strDate1, strEntity, strAction, strOt, strWorker, strListValidation);
            if (!(string.IsNullOrEmpty(oView.strError)))
            {
                ViewBag.Message = oView.strError;
            }
            if(oView.bLevelError) return RedirectToAction("Index", "Home", new { strMessage = "No tiene permiso de acceso a la página" });
            return View(oView);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchValidate(string idLine)
        {
            string strAction, strMessage;
            _idAldakinUser = await _iApplicationUserAldakin.GetIdUserAldakin(HttpContext.User);
            var oReturn = await _iWriteDataBase.ValidateWorkerLineAsync(idLine, _idAldakinUser,1);

            strMessage = oReturn.strError;
            strAction = "StatusResume";
            return RedirectToAction("Index", new { strMessage = strMessage, strAction = strAction, strDate1 = oReturn.strDate1, strEntity = oReturn.strEntity, strOt = "0", strWorker = oReturn.strWorker });
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchUnValidate(string idLine)
        {
            string strAction, strMessage;
            _idAldakinUser = await _iApplicationUserAldakin.GetIdUserAldakin(HttpContext.User);
            var oReturn = await _iWriteDataBase.ValidateWorkerLineAsync(idLine, _idAldakinUser, 0);
            strMessage = oReturn.strError;
            strAction = "StatusResume";
            return RedirectToAction("Index", new { strMessage = strMessage, strAction = strAction, strDate1 = oReturn.strDate1, strEntity = oReturn.strEntity, strOt="0",strWorker = oReturn.strWorker });
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> GlobalValidation(string strMessage = "",  string strDate1 = "", string strEntity = "", string strAction = "", string strOt = "", string strWorker = "", string strListValidation = "")
        {
            _idAldakinUser = await _iApplicationUserAldakin.GetIdUserAldakin(HttpContext.User);
            var oReturn = await _iWriteDataBase.ValidateGlobalLineAsync(_idAldakinUser, strListValidation, 1);
            strMessage = oReturn;
            strAction = "StatusResume";
            return RedirectToAction("Index", new { strMessage = strMessage, strAction = strAction, strDate1 = strDate1, strEntity = strEntity, strOt= strOt, strWorker= strWorker });
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> OpenWeek(string strMessage = "", string strDate1 = "", string strEntity = "", string strAction = "", string strOt = "", string strWorker = "", string strListValidation = "")
        {
            _idAldakinUser = await _iApplicationUserAldakin.GetIdUserAldakin(HttpContext.User);
            var strMessageO = await _iWriteDataBase.OpenWeek( strListValidation);
            strAction = "StatusResume";
            return RedirectToAction("Index", new { strMessage = strMessageO, strAction = strAction, strDate1 = strDate1, strEntity = strEntity, strOt = strOt, strWorker = strWorker });
        }
    }
}
