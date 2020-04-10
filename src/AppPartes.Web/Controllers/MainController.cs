using AppPartes.Data.Models;
using AppPartes.Logic;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
//https://developer.mozilla.org/es/docs/Web/JavaScript/Referencia/Objetos_globales/Object/values

//https://developer.mozilla.org/es/docs/Web/JavaScript/Referencia/Objetos_globales/JSON/stringify

//https://geeks.ms/santypr/2015/08/19/asp-net-mvc-cmo-enviar-modelo-con-javascript-desde-un-formulario-a-una-accin/
//https://www.youtube.com/watch?v=6kgYrpugYJA
namespace AppPartes.Web.Controllers
{
    public class MainController : Controller
    {
        //apaño para usuario con claims
        private string strUserName = "";
        private string stUserrDni = "";
        private int iUserId = 0;
        private int iUserCondEntO = 0;

        //Datos

        private List<Ots> listOts;
        private List<Entidad> listadept;
        private List<Clientes> listaClient;
        private List<Presupuestos> lPresupuestos;
        private List<Pernoctaciones> lPernoctas;
        private readonly List<Preslin> Nivel1;
        private List<Preslin> Nivel2;
        private readonly List<Preslin> Nivel3;
        private readonly List<Preslin> Nivel4;
        private readonly List<Preslin> Nivel5;
        private readonly List<Preslin> Nivel6;
        private readonly List<Preslin> Nivel7;
        private readonly AldakinDbContext aldakinDbContext;

        //public MainController(AldakinDbContext aldakinDbContext)
        //{
        //    this.aldakinDbContext = aldakinDbContext;
        //}

        public MainController()
        {
        }

        private void ajusteUsuario()
        {
            var user = aldakinDbContext.Usuarios.FirstOrDefault(x => x.Name.Equals("460b244aa3e22b31a53018fc506f517f") && x.CodEnt == x.CodEntO);
            if (!(user is null))
            {
                strUserName = user.Nombrecompleto.ToString();
                iUserId = Convert.ToInt16(user.Idusuario);
                iUserCondEntO = Convert.ToInt16(user.CodEntO);
                stUserrDni = user.Name;
            }
        }
        public IActionResult Index(string strMessage = "")
        {
            Logic.DataLogic oView = new DataLogic();
            OperationLogic oper = new OperationLogic(aldakinDbContext);
            ViewBag.Message = strMessage;
            oView = oper.LoadMainController();
            return View(oView);
        }
        [HttpPost]
        public JsonResult ResumenSemana(string cantidad)
        {
            ajusteUsuario();
            var listaSelect = new List<Logic.SelectData>();
            OperationLogic oper = new OperationLogic(aldakinDbContext);
            DateTime dtSelected;
            try
            {
                dtSelected = Convert.ToDateTime(cantidad);
                listaSelect=oper.WeekHourResume(dtSelected);
            }
            catch (Exception)
            {
                return Json(null);
            }
            return Json(listaSelect);
        }
        [HttpPost]
        public JsonResult EntidadSelectedOt(int cantidad)//JsonResult
        {
            var listaSelect = new List<Logic.SelectData>();
            OperationLogic oper = new OperationLogic(aldakinDbContext);
            try
            {
                listaSelect = oper.SelectedCompanyReadOt(cantidad);
            }
            catch(Exception ex)
            {
                return Json(null);
            }
            return Json(listaSelect);
        }
        [HttpPost]
        public JsonResult EntidadSelectedCliente(int cantidad)//JsonResult
        {
            var listaSelect = new List<Logic.SelectData>();
            OperationLogic oper = new OperationLogic(aldakinDbContext);
            try
            {
                listaSelect = oper.SelectedCompanyReadClient(cantidad);
            }
            catch (Exception ex)
            {
                return Json(null);
            }
            return Json(listaSelect);
        }
        [HttpPost]
        public JsonResult ClienteSelected(int cantidad)//JsonResult
        {
            var listaSelect = new List<Logic.SelectData>();
            OperationLogic oper = new OperationLogic(aldakinDbContext);
            try
            {
                listaSelect = oper.SelectedClient(cantidad);
            }
            catch (Exception ex)
            {
                return Json(null);
            }
            return Json(listaSelect);
        }
        [HttpPost]
        public JsonResult OtSelected(int cantidad)//JsonResult
        {
            var listaSelect = new List<Logic.SelectData>();
            OperationLogic oper = new OperationLogic(aldakinDbContext);
            try
            {
                listaSelect = oper.SelectedOt(cantidad);
            }
            catch (Exception ex)
            {
                return Json(null);
            }
            return Json(listaSelect);
        }
        [HttpPost]
        public JsonResult ObtenerNivel1(int cantidad)
        {
            var listaSelect = new List<Logic.SelectData>();
            OperationLogic oper = new OperationLogic(aldakinDbContext);
            try
            {
                listaSelect = oper.ReadLevel1(cantidad);
            }
            catch (Exception ex)
            {
                return Json(null);
            }
            return Json(listaSelect);
        }
        [HttpPost]
        public JsonResult ObtenerNivel(int cantidad)
        {
            var listaSelect = new List<Logic.SelectData>();
            OperationLogic oper = new OperationLogic(aldakinDbContext);
            try
            {
                listaSelect = oper.ReadLevelGeneral(cantidad);
            }
            catch (Exception ex)
            {
                return Json(null);
            }
            return Json(listaSelect);
        }
        [HttpPost]
        public JsonResult PagadorSelect(int cantidad, int cantidad2)
        {
            var listaSelect = new List<Logic.SelectData>();
            OperationLogic oper = new OperationLogic(aldakinDbContext);
            try
            {
                listaSelect = oper.SelectedPayer(cantidad, cantidad2);
            }
            catch (Exception ex)
            {
                return Json(null);
            }
            return Json(listaSelect);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> InsertLine(string strEntidad, string strOt, string strPresupuesto, string strNivel1, string strNivel2, string strNivel3, string strNivel4, string strNivel5, string strNivel6, string strNivel7, string strCalendario, string strHoraInicio, string strMinutoInicio, string strHoraFin, string strMinutoFin, string bHorasViaje, string bGastos, string strParte, string strPernoctacion, string strObservaciones, string strPreslin, string strGastos)
        {
            string strReturn = string.Empty;
            OperationLogic oper = new OperationLogic(aldakinDbContext);
            strReturn = oper.InsertWorkerLine( strEntidad,  strOt,  strPresupuesto,  strNivel1,  strNivel2,  strNivel3,  strNivel4,  strNivel5,  strNivel6,  strNivel7,  strCalendario,  strHoraInicio,  strMinutoInicio,  strHoraFin,  strMinutoFin,  bHorasViaje,  bGastos,  strParte,  strPernoctacion,  strObservaciones,  strPreslin,  strGastos);

            return RedirectToAction("Index", new { strMessage = strReturn });
        }

    }
    public class SelectData
    {
        public int iValue { set; get; }
        public string strText { set; get; }
        public string strValue { set; get; }
    }

    //.Replace(',', '.')
    //aldakinDbContext.Lineas.Remove //para borrar dato
    //throw new ArgumentException();//forzar exception
}