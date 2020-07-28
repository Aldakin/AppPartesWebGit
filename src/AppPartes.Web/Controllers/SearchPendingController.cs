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
    public class SearchPendingController : Controller
    {
        private readonly IWriteDataBase _IWriteDataBase;
        private readonly IWorkPartInformation _IWorkPartInformation;
        private readonly ILoadIndexController _ILoadIndexController;
        private readonly IApplicationUserAldakin _IApplicationUserAldakin;
        private int _idAldakinUser;
        public SearchPendingController(IWorkPartInformation iWorkPartInformation, IWriteDataBase iWriteDataBase, ILoadIndexController iLoadIndexController, IApplicationUserAldakin iApplicationUserAldakin)
        {
            _IWriteDataBase = iWriteDataBase;
            _IApplicationUserAldakin = iApplicationUserAldakin;
            _ILoadIndexController = iLoadIndexController;
            _IWorkPartInformation = iWorkPartInformation;
        }
        public async Task<IActionResult> Index(List<string> lSummary, string strMessage = "")
        {
            ViewBag.Message = strMessage;
            _idAldakinUser = await _IApplicationUserAldakin.GetIdUserAldakin(HttpContext.User);
            var oView = new SearchPendingViewLogic();
            oView = await _ILoadIndexController.SearchPendingControllerAsync(_idAldakinUser);
            if (lSummary.Count == 0)
            {
                oView.lSummary = null;
            }
            else
            {
                oView.lSummary = lSummary;
            }
            if (!(string.IsNullOrEmpty(oView.strError)))
            {
                ViewBag.Message = oView.strError;
            }
            return View(oView);
        }

    }
}