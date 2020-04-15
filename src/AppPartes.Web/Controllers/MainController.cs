using AppPartes.Data.Models;
using AppPartes.Logic;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppPartes.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using AppPartes.Web.Controllers.Api;


namespace AppPartes.Web.Controllers
{
    [Authorize]
    public class MainController : Controller
    {
        private readonly IWorkPartInformation _IWorkPartInformation;
        private readonly IWriteDataBase _IWriteDataBase;
        private readonly ILoadIndexController _ILoadIndexController;
        private readonly UserManager<ApplicationUser> _manager;
        public   MainController(IWorkPartInformation iWorkPartInformation, IWriteDataBase iWriteDataBase, ILoadIndexController iLoadIndexController, UserManager<ApplicationUser> manager)
        {
            _IWorkPartInformation = iWorkPartInformation;
            _IWriteDataBase = iWriteDataBase;
            _ILoadIndexController = iLoadIndexController;
        }
        public async Task<IActionResult> Index(string strMessage = "")
        {
            //TODO Asi recuperamos los datos de aldakin
           var user = await _manager.GetUserAsync(HttpContext.User);
            var idAldakin = user.IdAldakin;

            ViewBag.Message = strMessage;
            var oView = _ILoadIndexController.LoadMainController(idAldakin);
            return View(oView);
        }
    }
    //.Replace(',', '.')
    //aldakinDbContext.Lineas.Remove //para borrar dato
    //throw new ArgumentException();//forzar exception
}