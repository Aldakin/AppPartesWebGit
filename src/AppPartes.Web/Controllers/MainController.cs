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

//https://developer.mozilla.org/es/docs/Web/JavaScript/Referencia/Objetos_globales/Object/values

//https://developer.mozilla.org/es/docs/Web/JavaScript/Referencia/Objetos_globales/JSON/stringify

//https://geeks.ms/santypr/2015/08/19/asp-net-mvc-cmo-enviar-modelo-con-javascript-desde-un-formulario-a-una-accin/
//https://www.youtube.com/watch?v=6kgYrpugYJA
namespace AppPartes.Web.Controllers
{
    //[Authorize]
    public class MainController : Controller
    {
        private readonly IWorkPartInformation _IWorkPartInformation;
        private readonly IWriteDataBase _IWriteDataBase;
        private readonly ILoadIndexController _ILoadIndexController;
        //private readonly UserManager<ApplicationUser> _manager;
        public MainController(IWorkPartInformation iWorkPartInformation, IWriteDataBase iWriteDataBase, ILoadIndexController iLoadIndexController/*,UserManager<ApplicationUser> manager*/)
        {
            _IWorkPartInformation = iWorkPartInformation;
            _IWriteDataBase = iWriteDataBase;
            _ILoadIndexController = iLoadIndexController;
            //_manager = manager;
        }
        public async Task<IActionResult> Index(string strMessage = "")
        {
            //TODO Asi recuperamos los datos de aldakin
            //var user = await _manager.GetUserAsync(HttpContext.User);
            //var idAldakin = user?.IdAldakin;
            ViewBag.Message = strMessage;
            var oView = _ILoadIndexController.LoadMainController();
            return View(oView);
        }
    }
    //.Replace(',', '.')
    //aldakinDbContext.Lineas.Remove //para borrar dato
    //throw new ArgumentException();//forzar exception
}