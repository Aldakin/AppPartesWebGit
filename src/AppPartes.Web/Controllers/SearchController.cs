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
    //[Route("api/[controller]")]
    //[ApiController]
    [Authorize]
    public class SearchController : Controller
    {
        private readonly ILoadIndexController _ILoadIndexController;
        private readonly IApplicationUserAldakin _IApplicationUserAldakin;
        private readonly IWorkPartInformation _IWorkPartInformation;
        int _idAldakinUser;
        public SearchController(IApplicationUserAldakin iApplicationUserAldakin, ILoadIndexController iLoadIndexController, IWorkPartInformation iWorkPartInformation)
        {
            _IApplicationUserAldakin = iApplicationUserAldakin;
            _ILoadIndexController = iLoadIndexController;
        }
        public async Task<IActionResult> Index(string strMessage = "", string strDate = "", string strDate1 = "", string strEntity = "", string action = "", string strOt = "", string strWorker = "",string strListValidation="")
        {
            ViewBag.Message = strMessage;
            _idAldakinUser = await _IApplicationUserAldakin.GetIdUserAldakin(HttpContext.User);
            var oView = await _ILoadIndexController.LoadSearchControllerAsync(_idAldakinUser, strDate, strDate1, strEntity, action, strOt, strWorker, strListValidation);
            if (!(string.IsNullOrEmpty(oView.strError)))
            {
                ViewBag.Message = oView.strError;
            }
            return View(oView);
        }
        //[HttpPost, ValidateAntiForgeryToken]
        //public async Task<IActionResult> SearchEntityData(string strEntidad, string strDate, string strMes, string strSemana)
        //{

        //    return RedirectToAction("Index", new { strMessage = "ojo al manojo" });
        //}
    }
}