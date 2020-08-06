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
        private readonly ILoadIndexController _ILoadIndexController;
        private readonly IApplicationUserAldakin _IApplicationUserAldakin;
        private readonly IWorkPartInformation _IWorkPartInformation;
        private readonly IWriteDataBase _IWriteDataBase;
        int _idAldakinUser;
        public SearchController(IApplicationUserAldakin iApplicationUserAldakin, ILoadIndexController iLoadIndexController, IWorkPartInformation iWorkPartInformation, IWriteDataBase iWriteDataBase)
        {
            _IApplicationUserAldakin = iApplicationUserAldakin;
            _ILoadIndexController = iLoadIndexController;
            _IWriteDataBase = iWriteDataBase;
        }
        public async Task<IActionResult> Index(string strMessage = "", string strDate = "", string strDate1 = "", string strEntity = "", string strAction = "", string strOt = "", string strWorker = "", string strListValidation = "")
        {
            ViewBag.Message = strMessage;
            _idAldakinUser = await _IApplicationUserAldakin.GetIdUserAldakin(HttpContext.User);
            var oView = await _ILoadIndexController.LoadSearchControllerAsync(_idAldakinUser, strDate, strDate1, strEntity, strAction, strOt, strWorker, strListValidation);
            if (!(string.IsNullOrEmpty(oView.strError)))
            {
                ViewBag.Message = oView.strError;
            }
            return View(oView);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchValidate(string idLine)
        {
            string strAction, strMessage;
            _idAldakinUser = await _IApplicationUserAldakin.GetIdUserAldakin(HttpContext.User);
            var oReturn = await _IWriteDataBase.ValidateWorkerLineAsync(idLine, _idAldakinUser);
            if (oReturn.iValue == 0)
            {
                strAction = string.Empty;
                strMessage = oReturn.strText;
            }
            else
            {
                strAction = "StatusResume";
                strMessage = oReturn.strText;
            }
            return RedirectToAction("Index", new { strMessage = strMessage, action = strAction, strDate1 = oReturn.strValue, strEntity = Convert.ToString(oReturn.iValue) });
        }

        public async Task<IActionResult> SearchWeek(string strMessage = "", string strDate1 = "", string strEntity = "", string strAction = "", string strOt = "", string strWorker = "")
        {
            
            return RedirectToAction("Index", new { strMessage = strMessage, strAction = strAction, strDate1 = strDate1, strEntity = strEntity, strOt= strOt, strWorker= strWorker });
        }
    }
}
