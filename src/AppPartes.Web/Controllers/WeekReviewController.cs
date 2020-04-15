﻿using AppPartes.Data.Models;
using AppPartes.Logic;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using AppPartes.Web.Models;
using Microsoft.AspNetCore.Authorization;

//https://programacion.net/articulo/como_resaltar_fechas_especificas_en_jquery_ui_datepicker_1731

namespace AppPartes.Web.Controllers
{
    [Authorize]
    public class WeekReviewController : Controller
    {
        //apaño para usuario con claims
        private int iUserCondEntO = 0;
        private int iUserId = 0;
        private string strUserName = "";
        private string stUserrDni = "";
        private List<Lineas> listPartes = new List<Lineas>();
        //private readonly AldakinDbContext aldakinDbContext;
        private readonly UserManager<ApplicationUser> _manager;


        private readonly IWriteDataBase _IWriteDataBase;
        private readonly IWorkPartInformation _IWorkPartInformation;
        private readonly ILoadIndexController _ILoadIndexController;

        public WeekReviewController(IWriteDataBase iWriteDataBase, IWorkPartInformation iWorkPartInformation, ILoadIndexController iLoadIndexController, UserManager<ApplicationUser> manager)
        {
            _IWriteDataBase = iWriteDataBase;
            _IWorkPartInformation = iWorkPartInformation;
            _ILoadIndexController = iLoadIndexController;
            _manager = manager;
        }
        public async Task<IActionResult> Index(string strMessage = "", string strDate = "", string strAction = "", string strId = "")
        {
            ViewBag.Message = strMessage;
            var user = await _manager.GetUserAsync(HttpContext.User);
            var idAldakin = user.IdAldakin;
            var oView = _ILoadIndexController.LoadWeekController(idAldakin,strDate, strAction, strId);
            if (!(oView.Mensaje is null))
            {
                ViewBag.Message = "ocurrio un error!!!";
            }
            return View(oView);
        }
       
    }

}