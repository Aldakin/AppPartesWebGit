using AppPartes.Data.Models;
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

        public MainController(AldakinDbContext aldakinDbContext)
        {
            this.aldakinDbContext = aldakinDbContext;
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
            //pruebas con claims
            //var identity = ((ClaimsIdentity)Thread.CurrentPrincipal.Identity);
            //var claim = identity.Claims.Where(x => x.Type == ClaimTypes.Name).FirstOrDefault();
            //var name =User.Claims.FirstOrDefault(x => x.Type.ToString().Equals("id", StringComparison.InvariantCultureIgnoreCase));

            ViewBag.Message = strMessage;
            ajusteUsuario();
            //obtener ots
            var totalOts = aldakinDbContext.Ots.Where(x => x.CodEnt == iUserCondEntO && x.CodEntD == 0 && x.Codigorefot != "29" && x.Cierre == null);
            var totalType1Ots = totalOts.Where(x => x.Tipoot == 1);
            var totalType2Ots = totalOts.Join(aldakinDbContext.Presupuestos.Where(x => x.CodEnt == iUserCondEntO), o => o.Idots, i => i.Idot, (o, p) => o);//original
            listOts = totalType1Ots.Concat(totalType2Ots).Distinct().OrderBy(x => x.Numero).ToList();
            ViewBag.Ots = listOts;
            //consulta original
            //select * from Ots where  cod_ent = {0} and cod_ent_d = 0 and codigorefot != 29 and cierre IS NULL and (tipoot = 1 or idots in (select idot from presupuestos where cod_ent = {0})), cod_ent)

            //obtener empresas
            var aux = aldakinDbContext.Entidad.FirstOrDefault(x => x.CodEnt == iUserCondEntO);
            listadept = aldakinDbContext.Entidad.Where(x => x.CodEnt != iUserCondEntO).OrderByDescending(x => x.Nombre).ToList();
            listadept.Insert(0, aux);
            ViewBag.Departamentos = listadept;

            //obtener clientes
            //select distinct Clientes.* from Clientes, Ots where Clientes.idclientes = Ots.cliente and Ots.cierre IS NULL and Ots.codigorefot != 29 and Clientes.cod_ent = {0}", cod_ent  
            listaClient = (from c in aldakinDbContext.Clientes
                           from o in aldakinDbContext.Ots
                           where (
                           (c.Idclientes == o.Cliente)
                           && (o.Cierre == null)
                           && (o.Codigorefot != "29")
                           && (c.CodEnt == iUserCondEntO)
                           )
                           select c).Distinct().OrderBy(c => c.Nombre).ToList();

            ViewBag.Clientes = listaClient;

            //obtener pernoctaciones
            lPernoctas = aldakinDbContext.Pernoctaciones.Where(x => x.CodEnt == iUserCondEntO).ToList();
            ViewBag.Pernoctas = lPernoctas;

            return View();
        }
        [HttpPost]
        public JsonResult ResumenSemana(string cantidad)
        {
            ajusteUsuario();
            var iCont = 0;
            var listaSelect = new List<SelectData>();
            DateTime dtSelected, dtIniWeek, dtEndWeek;
            try
            {
                dtSelected = Convert.ToDateTime(cantidad);
            }
            catch (Exception)
            {
                return Json(null);
            }
            //int m_nSemana = System.Globalization.CultureInfo.CurrentUICulture.Calendar.GetWeekOfYear(dtSelected, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            //int m_nAño = dtSelected.Year;
            var dayWeek = dtSelected.DayOfWeek;
            switch (dayWeek)
            {
                case System.DayOfWeek.Sunday:
                    dtIniWeek = dtSelected.AddDays(-6);
                    dtEndWeek = dtSelected.AddDays(+1);
                    break;
                case System.DayOfWeek.Saturday:
                    dtIniWeek = dtSelected.AddDays(-5);
                    dtEndWeek = dtSelected.AddDays(+2);
                    break;
                case System.DayOfWeek.Friday:
                    dtIniWeek = dtSelected.AddDays(-4);
                    dtEndWeek = dtSelected.AddDays(+3);
                    break;
                case System.DayOfWeek.Thursday:
                    dtIniWeek = dtSelected.AddDays(-3);
                    dtEndWeek = dtSelected.AddDays(+4);
                    break;
                case System.DayOfWeek.Wednesday:
                    dtIniWeek = dtSelected.AddDays(-2);
                    dtEndWeek = dtSelected.AddDays(+5);
                    break;
                case System.DayOfWeek.Tuesday:
                    dtIniWeek = dtSelected.AddDays(-1);
                    dtEndWeek = dtSelected.AddDays(+6);
                    break;
                case System.DayOfWeek.Monday:
                    dtIniWeek = dtSelected.AddDays(0);
                    dtEndWeek = dtSelected.AddDays(+7);
                    break;
                default:
                    return Json(null);
            }
            for (var date = dtIniWeek; date < dtEndWeek; date = date.AddDays(1.0))
            {
                var strRangosHora = "";
                var listPartes = aldakinDbContext.Lineas.Where(x => x.Inicio.Date == date.Date && x.CodEnt == iUserCondEntO && x.Idusuario == iUserId).OrderBy(x => x.Inicio).ToList();
                foreach (var l in listPartes)
                {
                    var strTempIni = string.Empty;
                    var strTempFin = string.Empty;
                    if (l.Horas > 0 || l.Horasviaje > 0)
                    {
                        if (l.Inicio.Minute < 10)
                        {
                            strTempIni = l.Inicio.Hour.ToString() + ":0" + l.Inicio.Minute.ToString();
                        }
                        else
                        {
                            strTempIni = l.Inicio.Hour.ToString() + ":" + l.Inicio.Minute.ToString();
                        }
                        if (l.Fin.Minute < 10)
                        {
                            strTempFin = l.Fin.Hour.ToString() + ":0" + l.Fin.Minute.ToString();
                        }
                        else
                        {
                            strTempFin = l.Fin.Hour.ToString() + ":" + l.Fin.Minute.ToString();
                        }
                        strRangosHora = strRangosHora + strTempIni + " - " + strTempFin + "||";
                    }
                }
                listaSelect.Add(new SelectData { iValue = iCont, strText = strRangosHora });
                iCont++;
            }
            return Json(listaSelect);
        }
        [HttpPost]
        public JsonResult EntidadSelectedOt(int cantidad)//JsonResult
        {
            var listaSelect = new List<SelectData>();
            listOts = null;
            //listOts = aldakinDbContext.Ots.Where(x => x.CodEnt == cantidad && x.CodEntD == 0 && x.Codigorefot != "29" && x.Cierre == null).OrderByDescending(x => x.Idots).ToList();
            if (cantidad < 1)
            {
                return Json(null);
            }
            var totalOts = aldakinDbContext.Ots.Where(x => x.CodEnt == cantidad && x.CodEntD == 0 && x.Codigorefot != "29" && x.Cierre == null);
            var totalType1Ots = totalOts.Where(x => x.Tipoot == 1);
            var totalType2Ots = totalOts.Join(aldakinDbContext.Presupuestos.Where(x => x.CodEnt == cantidad), o => o.Idots, i => i.Idot, (o, p) => o);//original
            listOts = totalType1Ots.Concat(totalType2Ots).Distinct().OrderBy(x => x.Numero).ToList();
            foreach (var p in listOts)
            {
                var strTemp = p.Numero + "||" + p.Nombre;
                listaSelect.Add(new SelectData { iValue = p.Idots, strText = strTemp });
            }
            return Json(listaSelect);
        }
        [HttpPost]
        public JsonResult EntidadSelectedCliente(int cantidad)//JsonResult
        {
            var listaSelect = new List<SelectData>();
            listaClient = null;
            if (cantidad < 1)
            {
                return Json(null);
            }
            listaClient = (from c in aldakinDbContext.Clientes
                           from o in aldakinDbContext.Ots
                           where (
                           (c.Idclientes == o.Cliente)
                           && (o.Cierre == null)
                           && (o.Codigorefot != "29")
                           && (c.CodEnt == cantidad)
                           )
                           select c).Distinct().OrderBy(c => c.Nombre).ToList();
            //select distinct Clientes.* from Clientes, Ots where Clientes.idclientes = Ots.cliente and Ots.cierre IS NULL and Ots.codigorefot != 29 and Clientes.cod_ent = { 0}", cod_ent)
            foreach (var p in listaClient)
            {
                var strTemp = p.Nombre;
                listaSelect.Add(new SelectData { iValue = p.Idclientes, strText = strTemp });
            }
            return Json(listaSelect);
        }
        [HttpPost]
        public JsonResult ClienteSelected(int cantidad)//JsonResult
        {
            ajusteUsuario();
            var listaSelect = new List<SelectData>();
            if (cantidad != 0)
            {
                listOts = null;
                listOts = aldakinDbContext.Ots.Where(x => x.Cierre == null && x.Cliente == cantidad && x.Codigorefot != "29" && x.CodEntD != -1).OrderByDescending(x => x.Idots).ToList();
                foreach (var p in listOts)
                {
                    var strTemp = p.Numero + "||" + p.Nombre;
                    listaSelect.Add(new SelectData { iValue = p.Idots, strText = strTemp });
                }
            }
            else
            {
                listOts = null;
                listOts = aldakinDbContext.Ots.Where(x => x.CodEnt == iUserCondEntO && x.CodEntD == 0 && x.Codigorefot != "29" && x.Cierre == null).OrderByDescending(x => x.Idots).ToList();
                ViewBag.Ots = listOts;

                foreach (var p in listOts)
                {
                    var strTemp = p.Numero + "||" + p.Nombre;
                    listaSelect.Add(new SelectData { iValue = p.Idots, strText = strTemp });
                }

            }
            return Json(listaSelect);
        }

        [HttpPost]
        public JsonResult OtSelected(int cantidad)//JsonResult
        {
            var listaSelect = new List<SelectData>();
            lPresupuestos = null;
            if (cantidad > 0)
            {
                var lTempOts = aldakinDbContext.Ots.Where(x => x.Idots == cantidad).OrderByDescending(x => x.Idots).ToList();
                if (lTempOts[0].Tipoot == 1)
                {
                    lPresupuestos = null;
                }
                else if (lTempOts[0].Tipoot == 2)
                {
                    //obtener presupuestos
                    lPresupuestos = aldakinDbContext.Presupuestos.Where(x => x.Idot == cantidad).ToList();
                    //devuelvo los presupuestos y los evaluo en front para ver si mostrar presupuestos o nivel1
                    //app anterior
                    //si presupuestos son mas que 1 se muestra lb presupuesto para  mostrar
                    //si no se obtiene y se muestra nivel 1

                    //if (ObtenerPresupuestos(CurrentOT.idOT))
                    //{
                    //    if (lspresupuestos.Items.Count > 1)
                    //    {
                    //        lspresupuestos.Visible = true;
                    //        lblpresupuestos.Visible = true;
                    //    }
                    //    else
                    //    {
                    //        lspresupuestos.Visible = false;
                    //        lblpresupuestos.Visible = false;
                    //        ObtenerNivel1(MyArrayPresupuestos[0].idPresupuesto);
                    //    }
                    //}
                }
                //return Json(lPresupuestos);
                if (lPresupuestos == null)
                {
                    listaSelect = null;
                }
                else
                {
                    foreach (var p in lPresupuestos)
                    {
                        var strTemp = p.Numero + "||" + p.Nombre;
                        listaSelect.Add(new SelectData { iValue = p.Idpresupuestos, strText = strTemp });

                    }
                }
                return Json(listaSelect);
                //listOts = context.Ots.Where(x => x.CodEnt == 7 && x.CodEntD == 0 && x.Codigorefot != "29" && x.Cierre == null).OrderByDescending(x => x.Idots).ToList();
                //return Json(listOts);
            }
            else
            {
                listaSelect = null;
                return Json(listaSelect);
            }

        }
        [HttpPost]
        public JsonResult ObtenerNivel1(int cantidad)
        {
            List<Preslin> lPreslin = null;
            var listaSelect = new List<SelectData>();
            if (cantidad < 1)
            {
                return Json(null);
            }
            lPreslin = aldakinDbContext.Preslin.Where(x => x.Idpresupuesto == cantidad && x.Horas != 0 && x.Nivel == 1).ToList();
            if (lPreslin == null)
            {
                listaSelect = null;
            }
            else
            {
                foreach (var p in lPreslin)
                {
                    var strTemp = p.Nombre;
                    listaSelect.Add(new SelectData { iValue = p.Idpreslin, strText = strTemp });
                }
            }
            return Json(listaSelect);

        }
        [HttpPost]
        public JsonResult ObtenerNivel2(int cantidad, int cantidad2)
        {
            List<Preslin> lPreslin = null;
            var listaSelect = new List<SelectData>();
            if (cantidad < 1 || cantidad2 < 1)
            {
                return Json(null);
            }
            Nivel2 = null;
            lPreslin = aldakinDbContext.Preslin.Where(x => x.Idpresupuesto == cantidad2 && x.Horas != 0 && x.Nivel == 1).ToList();
            //List<Preslin> lNivelTemp = lPreslin.Where(x => x.Idpreslin == cantidad).ToList();
            var lNivelTemp = lPreslin.FirstOrDefault(x => x.Idpreslin == cantidad);
            if (!(lNivelTemp is null))
            {
                lPreslin = aldakinDbContext.Preslin.Where(x => x.Horas != 0 && x.Idpresupuesto == lNivelTemp.Idpresupuesto && x.CodpPes == lNivelTemp.CodhPes && x.Version == lNivelTemp.Version && x.Anexo == lNivelTemp.Anexo).ToList();
            }
            else
            {
                lPreslin = null;
            }
            //cmd = new MySqlCommand(String.Format("SELECT idpreslin, idpresupuesto, codh_pes, codp_pes, nivel,nombre, horas, version, anexo FROM preslin where horas != 0 and idpresupuesto ={0} and codp_pes = {1} and version = {2} and anexo = {3} ORDER BY nombre", pres.idPresupuesto, pres.codh_pes, pres.Version, pres.Anexo), conexionBD);//ORDER BY codh_pes
            if (lPreslin == null)
            {
                listaSelect = null;
            }
            else
            {
                foreach (var p in lPreslin)
                {
                    var strTemp = p.Nombre;
                    listaSelect.Add(new SelectData { iValue = p.Idpreslin, strText = strTemp });
                }
            }
            return Json(listaSelect);

        }
        [HttpPost]
        public JsonResult ObtenerNivel3(int cantidad, int cantidad2, int cantidad3)
        {
            List<Preslin> lPreslin = null;
            var listaSelect = new List<SelectData>();
            if (cantidad < 1)
            {
                return Json(null);
            }
            Nivel2 = null;
            var lNivelTemp = aldakinDbContext.Preslin.FirstOrDefault(x => x.Horas != 0 && x.Idpreslin == cantidad);
            if (!(lNivelTemp is null))
            {
                lPreslin = aldakinDbContext.Preslin.Where(x => x.Horas != 0 && x.Idpresupuesto == lNivelTemp.Idpresupuesto && x.CodpPes == lNivelTemp.CodhPes && x.Version == lNivelTemp.Version && x.Anexo == lNivelTemp.Anexo).ToList();
            }
            else
            {
                lPreslin = null;
            }
            if (lPreslin == null)
            {
                listaSelect = null;
            }
            else
            {
                foreach (var p in lPreslin)
                {
                    var strTemp = p.Nombre;
                    listaSelect.Add(new SelectData { iValue = p.Idpreslin, strText = strTemp });
                }
            }
            return Json(listaSelect);
        }
        [HttpPost]
        public JsonResult ObtenerNivel(int cantidad)
        {
            List<Preslin> lPreslin = null;
            var listaSelect = new List<SelectData>();
            Nivel2 = null;
            if (cantidad < 1)
            {
                return Json(null);
            }
            var lNivelTemp = aldakinDbContext.Preslin.Where(x => x.Horas != 0 && x.Idpreslin == cantidad).ToList();
            lPreslin = aldakinDbContext.Preslin.Where(x => x.Horas != 0 && x.Idpresupuesto == lNivelTemp.Last().Idpresupuesto && x.CodpPes == lNivelTemp.Last().CodhPes && x.Version == lNivelTemp.Last().Version && x.Anexo == lNivelTemp.Last().Anexo).ToList();
            if (lPreslin == null)
            {
                listaSelect = null;
            }
            else
            {
                foreach (var p in lPreslin)
                {
                    var strTemp = p.Nombre;
                    listaSelect.Add(new SelectData { iValue = p.Idpreslin, strText = strTemp });
                }
            }
            return Json(listaSelect);

        }
        [HttpPost]
        public JsonResult PagadorSelect(int cantidad, int cantidad2)
        {
            var listaSelect = new List<SelectData>();
            if (cantidad2 < 1)
            {
                return Json(null);
            }

            var tipoGasto = aldakinDbContext.Tipogastos.Where(x => x.Pagador == cantidad && x.CodEnt == aldakinDbContext.Ots.FirstOrDefault(o => o.Idots == cantidad2).CodEnt).ToList();
            if (!(tipoGasto == null))
            {
                foreach (var p in tipoGasto)
                {
                    var strTemp = p.Tipo.ToUpper();
                    listaSelect.Add(new SelectData { strValue = strTemp, strText = strTemp });
                }
            }
            return Json(listaSelect);

        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> InsertLine(string strEntidad, string strOt, string strPresupuesto, string strNivel1, string strNivel2, string strNivel3, string strNivel4, string strNivel5, string strNivel6, string strNivel7, string strCalendario, string strHoraInicio, string strMinutoInicio, string strHoraFin, string strMinutoFin, string bHorasViaje, string bGastos, string strParte, string strPernoctacion, string strObservaciones, string strPreslin, string strGastos)
        {
            //datos provisionles
            ajusteUsuario();
            var datosLinea = new Lineas();
            var bHorasViajeTemp = false;
            var iPernoctacion = 0;
            DateTime day, dtInicio, dtFin;
            //Estado dia
            //"select estado from estadodias where idusuario = {0} and date(dia) = '{1}'", user.idUsuario, dia.ToString("yyyy-MM-dd")
            try
            {
                if (string.IsNullOrEmpty(strObservaciones))
                {
                    strObservaciones = string.Empty;
                }

                if (string.IsNullOrEmpty(strParte))
                {
                    strParte = string.Empty;
                }

                if (string.IsNullOrEmpty(bHorasViaje))
                {
                    bHorasViajeTemp = false;
                }
                else
                {
                    bHorasViajeTemp = true;
                }
                if (string.IsNullOrEmpty(bGastos))
                {
                }
                else
                {
                    strHoraInicio = "00";
                    strMinutoInicio = "00";
                    strHoraFin = "00";
                    strMinutoFin = "00";
                }
                if (string.IsNullOrEmpty(strPernoctacion))
                {
                    iPernoctacion = 0;
                }
                else
                {
                    iPernoctacion = Convert.ToInt32(strPernoctacion);
                }

                day = Convert.ToDateTime(strCalendario);
                dtInicio = Convert.ToDateTime(strCalendario + " " + strHoraInicio + ":" + strMinutoInicio + ":00");
                dtFin = Convert.ToDateTime(strCalendario + " " + strHoraFin + ":" + strMinutoFin + ":00");
            }
            catch (Exception)
            {
                return RedirectToAction("Index", new { strMessage = "Se ha producido un error en el procesamiento de los datos;" });
            }
            var lEstadoDia = aldakinDbContext.Estadodias.Where(x => x.Idusuario == iUserId && DateTime.Compare(x.Dia, day) == 0).ToList();//
            if (lEstadoDia.Count > 0)
            {
                return RedirectToAction("Index", new { strMessage = "La semana esta cerrada, habla con tu responsable para reabirla;" });
            }
            if (DateTime.Compare(dtInicio, dtFin) > 0)
            {
                return RedirectToAction("Index", new { strMessage = "Hora de Fin de Parte anterior a la Hora de inicio de Parte;" });
            }
            //rango usado
            var lLineas = aldakinDbContext.Lineas.Where(x => DateTime.Compare(x.Inicio.Date, day.Date) == 0 && x.Idusuario == iUserId && x.Validado == 0 && x.Registrado == 0).ToList();
            if (lLineas != null)
            {
                foreach (var x in lLineas)
                {
                    if (DateTime.Compare(dtFin, x.Inicio) > 0 && DateTime.Compare(dtFin, x.Fin) < 0)
                    {
                        return RedirectToAction("Index", new { strMessage = "Rango de Horas ya utilizado;" });
                    }
                    if (DateTime.Compare(dtInicio, x.Inicio) > 0 && DateTime.Compare(dtInicio, x.Fin) < 0)
                    {
                        return RedirectToAction("Index", new { strMessage = "Rango de Horas ya utilizado;" });
                    }
                }
            }
            //gastos
            float dGastos = 0;
            float dKilometros = 0;
            var iCodEntOt = aldakinDbContext.Ots.FirstOrDefault(t => t.Idots == Convert.ToInt32(strOt)).CodEnt;
            var lGastos = new List<Gastos>();
            if (!(string.IsNullOrEmpty(strGastos)))
            {
                string line;
                string[] substring;
                var strReader = new StringReader(strGastos);
                while ((line = strReader.ReadLine()) != null)
                {
                    if (line != null)
                    {
                        substring = line.Split('|');
                        if (substring.Length == 5)
                        {
                            if (string.Equals(substring[1], "ALDAKIN"))
                            {
                                substring[1] = "1";
                            }
                            else
                            {
                                if (string.Equals(substring[1], "TRABAJADOR"))
                                {
                                    substring[1] = "0";
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            try
                            {
                                var pagador = aldakinDbContext.Tipogastos.FirstOrDefault(x => x.CodEnt == iCodEntOt && string.Equals(x.Tipo, substring[2]) && x.Pagador == Convert.ToInt32(substring[1]));
                                if (pagador is null)
                                {
                                    //si hay error no hace nada con la lineapara que siga con la siguiente
                                    //return RedirectToAction("Index", new { strMessage = "Los gastos son erroneos, repita el parte;" });
                                }
                                else
                                {
                                    lGastos.Add(new Gastos
                                    {
                                        Pagador = Convert.ToInt32(substring[1]),
                                        Tipo = pagador.Idtipogastos,
                                        Cantidad = (float)Convert.ToDouble(substring[3].Replace('.', ',')),
                                        Observacion = substring[4]
                                    });

                                    if (substring[2] != "KILOMETROS")
                                    {
                                        dGastos = dGastos + (float)Convert.ToDouble(substring[3].Replace('.', ','));
                                    }
                                    else
                                    {
                                        dKilometros = dKilometros + (float)Convert.ToDouble(substring[3].Replace('.', ','));
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                //si hay error no hace nada con la lineapara que siga con la siguiente
                                //return RedirectToAction("Index", new { strMessage = "Los gastos son erroneos, repita el parte;" });
                            }
                        }
                        else
                        {
                            //si hay error no hace nada con la lineapara que siga con la siguiente
                            //return RedirectToAction("Index", new { strMessage = "Los gastos son erroneos, repita el parte;" });
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            //tiempo de viaje
            float TiempoViaje = 0;
            if (bHorasViajeTemp)
            {
                TiempoViaje = Convert.ToSingle((dtFin - dtInicio).TotalHours);
            }

            //trabajos realizados

            var otSel = aldakinDbContext.Ots.FirstOrDefault(x => x.Idots == Convert.ToInt32(strOt) && x.Codigorefot != "29" && x.Cierre == null);
            if (otSel is null)
            {
                return RedirectToAction("Index", new { strMessage = "En la Ot que esta usande se ha encontrado un problema, recargue la pagina;" });
            }

            if (otSel.Nombre.Length > 10 && otSel.Nombre.Substring(0, 20) == "TRABAJOS REALIZADOS ")
            {
                try
                {
                    var otoriginal = Convert.ToInt32(strObservaciones.Substring(0, 1));
                }
                catch (Exception)
                {
                    return RedirectToAction("Index", new { strMessage = "En las OTs de trabajos para otras delegaciones, lo primero que debe aparecer en las observaciones debe ser la OT de la delegacion de origen;" });
                }
            }
            if (string.IsNullOrEmpty(strPreslin))
            {
                datosLinea.Idpreslin = null;
            }
            else
            {
                if (strPreslin.Equals("-1"))
                {
                    datosLinea.Idpreslin = null;
                }
                else
                {
                    datosLinea.Idpreslin = Convert.ToInt32(strPreslin);
                }
            }
            datosLinea.Idot = Convert.ToInt32(strOt);
            datosLinea.Dietas = dGastos;
            datosLinea.Km = dKilometros;
            datosLinea.Observaciones = strObservaciones.ToUpper();
            datosLinea.Horasviaje = TiempoViaje;
            datosLinea.Inicio = dtInicio;
            datosLinea.Fin = dtFin;
            datosLinea.Horas = Convert.ToSingle((dtFin - dtInicio).TotalHours) - TiempoViaje;
            datosLinea.Idusuario = iUserId;
            datosLinea.Facturable = iPernoctacion;
            datosLinea.Npartefirmado = strParte.ToUpper();
            datosLinea.CodEnt = iUserCondEntO;
            datosLinea.Idoriginal = 0;
            datosLinea.Validador = string.Empty;
            datosLinea.Validado = 0;
            //inserccion segun codent...
            if (otSel.CodEnt == iUserCondEntO)
            {

                var transaction = await aldakinDbContext.Database.BeginTransactionAsync();
                try
                {
                    var linea = new Lineas
                    {
                        Idot = datosLinea.Idot,
                        Idpreslin = datosLinea.Idpreslin,
                        Dietas = datosLinea.Dietas,
                        Km = datosLinea.Km,
                        Observaciones = datosLinea.Observaciones,
                        Horasviaje = datosLinea.Horasviaje,
                        Inicio = datosLinea.Inicio,
                        Fin = datosLinea.Fin,
                        Horas = datosLinea.Horas,
                        Idusuario = datosLinea.Idusuario,
                        Facturable = datosLinea.Facturable,
                        Npartefirmado = datosLinea.Npartefirmado,
                        CodEnt = datosLinea.CodEnt,
                        Idoriginal = 0,
                        Validador = string.Empty,
                        Validado = 0
                    };
                    //var linea = new Lineas();
                    //linea.Idot = datosLinea.Idot;
                    //linea.Idpreslin = datosLinea.Idpreslin;
                    //linea.Dietas = datosLinea.Dietas;
                    //linea.Km = datosLinea.Km;
                    //linea.Observaciones = datosLinea.Observaciones;
                    //linea.Horasviaje = datosLinea.Horasviaje;
                    //linea.Inicio = datosLinea.Inicio;
                    //linea.Fin = datosLinea.Fin;
                    //linea.Horas = datosLinea.Horas;
                    //linea.Idusuario = datosLinea.Idusuario;
                    //linea.Facturable = datosLinea.Facturable;
                    //linea.Npartefirmado = datosLinea.Npartefirmado;
                    //linea.CodEnt = datosLinea.CodEnt;
                    //linea.Idoriginal = 0;
                    //linea.Validador = string.Empty;
                    //linea.Validado = 0;

                    aldakinDbContext.Lineas.Add(linea);
                    await aldakinDbContext.SaveChangesAsync();
                    foreach (var g in lGastos)
                    {
                        var gasto = new Gastos
                        {
                            Pagador = g.Pagador,
                            Tipo = g.Tipo,
                            Cantidad = g.Cantidad,
                            Observacion = g.Observacion.ToUpper(),
                            Idlinea = linea.Idlinea
                        };
                        aldakinDbContext.Gastos.Add(gasto);
                    }
                    await aldakinDbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    return RedirectToAction("Index", new { strMessage = "Ha ocurrido un problema durante el proceso de guardar el parte de trabajo.;" });
                }
            }
            else
            {
                var user = aldakinDbContext.Usuarios.FirstOrDefault(x => x.Name == stUserrDni && x.CodEnt == Convert.ToInt16(strEntidad));
                if (!(user is null))
                {
                    var transaction = await aldakinDbContext.Database.BeginTransactionAsync();
                    try
                    {
                        var linea = new Lineas
                        {
                            Idot = datosLinea.Idot,
                            Idpreslin = datosLinea.Idpreslin,
                            Dietas = datosLinea.Dietas,
                            Km = datosLinea.Km,
                            Observaciones = datosLinea.Observaciones,
                            Horasviaje = datosLinea.Horasviaje,
                            Inicio = datosLinea.Inicio,
                            Fin = datosLinea.Fin,
                            Horas = datosLinea.Horas,
                            Idusuario = datosLinea.Idusuario,
                            Facturable = datosLinea.Facturable,
                            Npartefirmado = datosLinea.Npartefirmado,
                            CodEnt = iCodEntOt,//codigo entidad de la ot
                            Idoriginal = 0,
                            Validador = string.Empty,
                            Validado = 0
                        };
                        aldakinDbContext.Lineas.Add(linea);
                        await aldakinDbContext.SaveChangesAsync();
                        foreach (var g in lGastos)
                        {
                            var gasto = new Gastos
                            {
                                Pagador = g.Pagador,
                                Tipo = g.Tipo,
                                Cantidad = g.Cantidad,
                                Observacion = g.Observacion.ToUpper(),
                                Idlinea = linea.Idlinea
                            };
                            aldakinDbContext.Gastos.Add(gasto);
                        }
                        await aldakinDbContext.SaveChangesAsync();
                        string Salida = otSel.Numero.ToString(), Primerdigito, Resto;
                        var Cero = Convert.ToChar("0"); ;
                        Primerdigito = otSel.Numero.ToString().Substring(0, 1);
                        Resto = otSel.Numero.ToString().Substring(2, otSel.Numero.ToString().Length - 2);
                        Resto = Resto.TrimStart(Cero);
                        Salida = string.Format("{0}|{1}", Primerdigito, Resto);
                        var observaciones = Salida + " " + datosLinea.Observaciones.ToUpper();

                        //from ots where cierre is null and year(apertura) = year(iinicio) and  cod_ent = icod_ent and cod_ent_d = (select cod_ent from lineas where idlinea = iidoriginal)
                        var iOt = aldakinDbContext.Ots.FirstOrDefault(x => x.Cierre == null && x.Apertura.Year == datosLinea.Inicio.Year && x.CodEnt == iUserCondEntO && x.CodEntD == aldakinDbContext.Lineas.FirstOrDefault(y => y.Idlinea == linea.Idlinea).CodEnt).Idots;
                        var lineaSecundaria = new Lineas
                        {
                            Idot = iOt,
                            Idpreslin = null,
                            Dietas = datosLinea.Dietas,
                            Km = datosLinea.Km,
                            Observaciones = observaciones.ToUpper(),
                            Horasviaje = datosLinea.Horasviaje,
                            Inicio = datosLinea.Inicio,
                            Fin = datosLinea.Fin,
                            Horas = datosLinea.Horas,
                            Idusuario = datosLinea.Idusuario,
                            Facturable = datosLinea.Facturable,
                            Npartefirmado = datosLinea.Npartefirmado,
                            CodEnt = iUserCondEntO,//codigo entidad del usuario
                            Idoriginal = linea.Idlinea,
                            Validador = string.Empty,
                            Validado = 0
                        };
                        aldakinDbContext.Lineas.Add(lineaSecundaria);
                        await aldakinDbContext.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        return RedirectToAction("Index", new { strMessage = "Ha ocurrido un problema durante el proceso de guardar el parte de trabajo.;" });
                    }
                }
                else
                {
                    return RedirectToAction("Index", new { strMessage = "¡¡¡No estas dado de alta en la delegacion para la que quieres registrar el parte!!!Informe a admisntracion de lo ocurrido;" });
                }
            }
            return RedirectToAction("Index", new { strMessage = "Parte rellenado satisfactortiamente" });
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