using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppPartes.Logic;
using AppPartes.Web.Controllers.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppPartes.Web.Controllers
{
    [Authorize]
    public class MessageController : Controller
    {
        private readonly ILoadIndexController _ILoadIndexController;
        private readonly IApplicationUserAldakin _IApplicationUserAldakin;
        private readonly IWriteDataBase _IWriteDataBase;
        private int _idAldakinUser;

        public MessageController(ILoadIndexController iLoadIndexController, IApplicationUserAldakin iApplicationUserAldakin, IWriteDataBase iWriteDataBase)
        {
            _ILoadIndexController = iLoadIndexController;
            _IApplicationUserAldakin = iApplicationUserAldakin;
            _IWriteDataBase = iWriteDataBase;
        }
        public async Task<IActionResult> Index(string strMessage = "", int idMessage = 0)
        {
            ViewBag.Message = strMessage;
            _idAldakinUser = await _IApplicationUserAldakin.GetIdUserAldakin(HttpContext.User);
            var oView = await _ILoadIndexController.LoadMessageControllerAsync(_idAldakinUser, idMessage);
            if (!(string.IsNullOrEmpty(oView.strError)))
            {
                ViewBag.Message = oView.strError;
            }
            return View(oView);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Answer(int idOriginal, int iAddress, string strAnswer, string strSubject, int iIdLinea)
        {
            string strReturn = string.Empty;
            int iIdLineaNew = 0;
            try
            {
                if (iIdLinea < 1)
                {
                    iIdLineaNew = -2;
                }
                else
                {
                    iIdLineaNew = iIdLinea;
                }
                var answer = new LineMessage
                {
                    Inicial = idOriginal,
                    A = iAddress,
                    De = await _IApplicationUserAldakin.GetIdUserAldakin(HttpContext.User),
                    Asunto = strSubject,
                    Mensaje = strAnswer.ToUpper(),
                    Idlinea = iIdLineaNew
                };
                strReturn = await _IWriteDataBase.AnswerMessageAsync(answer);
            }
            catch (Exception ex)
            {
                strReturn = "Ocurrio un error al contestar el email";
            }
            return RedirectToAction("Index", new { strMessage = strReturn });
        }
    }
}