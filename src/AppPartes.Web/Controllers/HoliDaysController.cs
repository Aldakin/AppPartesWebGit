using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AppPartes.Logic;
using AppPartes.Web.Controllers.Api;

namespace AppPartes.Web.Controllers
{
    public class HoliDaysController : Controller
    {
        private readonly ILoadIndexController _iLoadIndexController;
        private readonly IApplicationUserAldakin _iApplicationUserAldakin;
        private readonly IWriteDataBase _iWriteDataBase;
        private int _idAldakinUser;
        public HoliDaysController(ILoadIndexController iLoadIndexController, IApplicationUserAldakin iApplicationUserAldakin, IWriteDataBase iWriteDataBase)
        {
            _iLoadIndexController = iLoadIndexController;
            _iApplicationUserAldakin = iApplicationUserAldakin;
            _iWriteDataBase = iWriteDataBase;
        }
        public async Task<IActionResult> Index(string strMessage="", string strCalendarioIni = "", string strCalendarioFin = "", string strEntidad = "", string strAction = "")
        {
            ViewBag.Message = strMessage;
            var oReturn = new HoliDaysViewLogic();
            oReturn = await _iLoadIndexController.LoadHoliDaysAsync(strCalendarioIni, strCalendarioFin, strEntidad, strAction);
            oReturn.dtSelectedIni = strCalendarioIni;
            oReturn.dtSelectedFin = strCalendarioFin;
            oReturn.strEntidadSelec = strEntidad;
            return View(oReturn);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> InsertLine(string strCalendario = "", string strEntidad = "", string strJornada = "", string strAction = "")
        {
            var strReturn = string.Empty;
            //_idAldakinUser = await _iApplicationUserAldakin.GetIdUserAldakin(HttpContext.User);
            strReturn = await _iWriteDataBase.InsertHoliDayAsync(strCalendario, strEntidad, strJornada, strAction);
            return RedirectToAction("Index", new { strMessage = strReturn });
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteLine(string strId = "",string dtSelectedIni="", string dtSelectedFin = "", string strEntidad = "")
        {
            var strReturn = string.Empty;
            //_idAldakinUser = await _iApplicationUserAldakin.GetIdUserAldakin(HttpContext.User);
            strReturn = await _iWriteDataBase.DeleteHoliDayAsync(strId);
            return RedirectToAction("Index", new { strMessage = strReturn, strCalendarioIni= dtSelectedIni, strCalendarioFin= dtSelectedFin, strEntidad= strEntidad, strAction="list" });
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> InsertAllLine(string strlista = "")
        {
            var strReturn = string.Empty;
            //_idAldakinUser = await _iApplicationUserAldakin.GetIdUserAldakin(HttpContext.User);
            strReturn = await _iWriteDataBase.WriteAllHolidaysAsync(strlista);
            return RedirectToAction("Index", new { strMessage = strReturn });
        }
    }
}