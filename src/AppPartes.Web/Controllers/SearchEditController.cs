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
        private readonly ILoadIndexController _ILoadIndexController;
        private readonly IApplicationUserAldakin _IApplicationUserAldakin;
        private readonly IWorkPartInformation _IWorkPartInformation;
        int _idAldakinUser;
        public SearchEditController(IApplicationUserAldakin iApplicationUserAldakin, ILoadIndexController iLoadIndexController, IWorkPartInformation iWorkPartInformation)
        {
            _IApplicationUserAldakin = iApplicationUserAldakin;
            _ILoadIndexController = iLoadIndexController;
        }
        public async Task< IActionResult> Index(string strMessage = "",string strLineId = "",string strAction = "")
        {
            ViewBag.Message = strMessage;
            _idAldakinUser = await _IApplicationUserAldakin.GetIdUserAldakin(HttpContext.User);
            var oView = await _ILoadIndexController.LoadSearchEditControllerAsync(_idAldakinUser, strLineId, strAction);
            return View(oView);
        }
    }
}