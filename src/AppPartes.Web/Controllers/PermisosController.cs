using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AppPartes.Logic;
using AppPartes.Web.Controllers.Api;

namespace AppPartes.Web.Controllers
{
    [Authorize]
    public class PermisosController : Controller
    {
        private readonly ILoadIndexController _iLoadIndexController;
        private readonly IApplicationUserAldakin _iApplicationUserAldakin;
        private readonly IWorkPartInformation _iWorkPartInformation;
        private readonly IWriteDataBase _iWriteDataBase;
        int _idAldakinUser;
        public PermisosController(IApplicationUserAldakin iApplicationUserAldakin, ILoadIndexController iLoadIndexController, IWorkPartInformation iWorkPartInformation, IWriteDataBase iWriteDataBase)
        {
            _iApplicationUserAldakin = iApplicationUserAldakin;
            _iLoadIndexController = iLoadIndexController;
            _iWriteDataBase = iWriteDataBase;
        }

        public async Task<IActionResult> Index(string strUsuario = "", string strEntidad = "", string strFiltro = "", string strMessage="" )
        {
            ViewBag.Message = strMessage;
            _idAldakinUser = await _iApplicationUserAldakin.GetIdUserAldakin(HttpContext.User);
            var oView = new PermisosViewLogic();
            oView = await _iLoadIndexController.PermisosMainControllerAsync(_idAldakinUser,strUsuario, strEntidad, strFiltro);
            if(!(string.IsNullOrEmpty(oView.strError)))
            {
                ViewBag.Message = oView.strError;
            }

            if (oView.bLevelError) return RedirectToAction("Index", "Home", new { strMessage = "No tiene permiso de acceso a la página" });
            return View(oView);
        }

    }
}