using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppPartes.Logic;
using AppPartes.Web.Controllers.Api;
using Microsoft.AspNetCore.Mvc;

namespace AppPartes.Web.Controllers
{
    public class UdObraPresuController : Controller
    {

        private readonly ILoadIndexController _ILoadIndexController;
        private readonly IApplicationUserAldakin _IApplicationUserAldakin;
        private readonly IWriteDataBase _IWriteDataBase;
        private int _idAldakinUser;

        public UdObraPresuController(IApplicationUserAldakin iApplicationUser, ILoadIndexController iLoadIndexController,IWriteDataBase iWriteDataBase)
        {
            _ILoadIndexController = iLoadIndexController;
            _IApplicationUserAldakin = iApplicationUser;
            _IWriteDataBase = iWriteDataBase;
        }

        public async Task<IActionResult> Index(string strMessage = "")
        {
            ViewBag.Message = strMessage;
            _idAldakinUser = await _IApplicationUserAldakin.GetIdUserAldakin(HttpContext.User);
            var oReturn = await _ILoadIndexController.LoadUdObraPresuAsync(_idAldakinUser);
            return View(oReturn);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> InsertLine( string strDescription = "", string strRef = "", string strEntidad = "")
        {
            string strReturn = await _IWriteDataBase.WritetUdObrePresuNewAsync(strDescription, strRef, strEntidad);
            return RedirectToAction("Index", new { strMessage = strReturn });
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteLine(string strId = "")
        {
            string strReturn = await _IWriteDataBase.DeletetUdObrePresuNewAsync(strId);

            return RedirectToAction("Index", new { strMessage = strReturn });
        }
        
    }
}