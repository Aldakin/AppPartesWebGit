using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using AppPartes.Data.Models;

//https://programacion.net/articulo/como_resaltar_fechas_especificas_en_jquery_ui_datepicker_1731

namespace AppPartes.Web.Controllers
{
    public class WeekReviewController : Controller
    {
        //apaño para usuario con claims
        int iUserCondEntO = 0;
        int iUserId = 0;
        string strUserName = "";
        string stUserrDni = "";
        List<Lineas> listPartes = new List<Lineas>();
        private readonly AldakinDbContext aldakinDbContext;

        public WeekReviewController(AldakinDbContext aldakinDbContext)
        {
            this.aldakinDbContext = aldakinDbContext;
        }
        private void ajusteUsuario()
        {
            using (var context = new AldakinDbContext())
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
        }
        public IActionResult Index(string strMessage = "", string strDate = "", string strAction = "", string strId = "")
        {
            //datos provisionles
            ajusteUsuario();
            ViewBag.Message = strMessage;
            DateTime dtIniWeek, dtEndWeek;
            if (string.IsNullOrEmpty(strDate) && string.IsNullOrEmpty(strAction))
            {
                return View();
            }
            else
            {
                List<LineaVisual> listVisual = new List<LineaVisual>();
                string NombreOt = string.Empty;
                string NombreCliente = string.Empty;
                string strPernocta = string.Empty;
                string strPreslin = string.Empty;
                if ((string.IsNullOrEmpty(strId))) strId = "0";
                var lSelect = aldakinDbContext.Lineas.FirstOrDefault(x => x.Idlinea == Convert.ToInt32(strId));
                DateTime day;
                List<Estadodias> lEstadoDia;
                //ViewBag.Semana = null;
                //ViewBag.Lineas = null;
                //ViewBag.DateSelected = null;
                switch (strAction)
                {
                    case "loadWeek":
                        try
                        {
                            DateTime dtSelected = Convert.ToDateTime(strDate);
                            //int m_nSemana = System.Globalization.CultureInfo.CurrentUICulture.Calendar.GetWeekOfYear(dtSelected, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
                            //int m_nAño = dtSelected.Year;
                            System.DayOfWeek dayWeek = dtSelected.DayOfWeek;
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
                                    return RedirectToAction("Index", new { strMessage = "Error en la seleccion de dia!!!" });
                            }
                            List<double> ldHorasdia = new List<double>();
                            for (DateTime date = dtIniWeek; date < dtEndWeek; date = date.AddDays(1.0))
                            {
                                double dHorasDia = 0;
                                listPartes = aldakinDbContext.Lineas.Where(x => x.Inicio.Date == date.Date && x.CodEnt == iUserCondEntO && x.Idusuario == iUserId).OrderBy(x => x.Inicio).ToList();
                                foreach (Lineas l in listPartes)
                                {
                                    dHorasDia = dHorasDia + (l.Fin - l.Inicio).TotalHours;
                                }
                                ldHorasdia.Add(dHorasDia);
                            }
                            ViewBag.Semana = ldHorasdia;
                            listPartes = null;
                            listPartes = aldakinDbContext.Lineas.Where(x => x.Inicio > dtIniWeek && x.Fin < dtEndWeek && x.Idusuario == iUserId && x.CodEnt == iUserCondEntO).OrderBy(x => x.Inicio).ToList();
                            //aqui tengo que recorrer lisPartes e ir volvandolo a otra clase nueva para nomstrar losnombres de las ots y empresas...
                            //ademas tengo que ver si es la linea original o la secundaria para que si es la secundaria la cambie por la principal
                            foreach (Lineas l in listPartes)
                            {
                                NombreOt = aldakinDbContext.Ots.FirstOrDefault(x => x.Idots == l.Idot).Nombre;
                                NombreCliente = aldakinDbContext.Clientes.FirstOrDefault(x => x.Idclientes == aldakinDbContext.Ots.FirstOrDefault(o => o.Idots == l.Idot).Cliente).Nombre;
                                if(l.Facturable==0)
                                {
                                    strPernocta = "NO";
                                }
                                else
                                {
                                    strPernocta = aldakinDbContext.Pernoctaciones.FirstOrDefault(x => x.Tipo == l.Facturable && x.CodEnt == iUserCondEntO).Descripcion;
                                }
                                if (l.Idoriginal == 0)
                                {
                                    listVisual.Add(new LineaVisual
                                    {
                                        Idlinea = l.Idlinea,
                                        Idot = l.Idot,
                                        NombreOt = NombreOt,
                                        NombreCliente = NombreCliente,
                                        NombrePreslin = strPreslin,
                                        Dietas = l.Dietas,
                                        Km = l.Km,
                                        Observaciones = l.Observaciones,
                                        Horasviaje = l.Horasviaje,
                                        Horas = l.Horas,
                                        Inicio = l.Inicio,
                                        Fin = l.Fin,
                                        Idusuario = l.Idusuario,
                                        strPernocta= strPernocta,
                                        Npartefirmado = l.Npartefirmado,
                                        Idoriginal = l.Idoriginal,
                                        Registrado = l.Registrado,
                                        CodEnt = l.CodEnt,
                                        Validado = l.Validado,
                                        Validador = l.Validador
                                    });
                                }
                                else
                                {
                                    var lineaOriginal = aldakinDbContext.Lineas.FirstOrDefault(x => x.Idlinea == l.Idoriginal);
                                    listVisual.Add(new LineaVisual
                                    {
                                        Idlinea = lineaOriginal.Idlinea,
                                        Idot = lineaOriginal.Idot,
                                        NombreOt = NombreOt,
                                        NombreCliente = NombreCliente,
                                        NombrePreslin = strPreslin,
                                        Dietas = lineaOriginal.Dietas,
                                        Km = lineaOriginal.Km,
                                        Observaciones = lineaOriginal.Observaciones,
                                        Horasviaje = lineaOriginal.Horasviaje,
                                        Horas = lineaOriginal.Horas,
                                        Inicio = lineaOriginal.Inicio,
                                        Fin = lineaOriginal.Fin,
                                        Idusuario = lineaOriginal.Idusuario,
                                        strPernocta = strPernocta,
                                        Npartefirmado = lineaOriginal.Npartefirmado,
                                        Idoriginal = lineaOriginal.Idoriginal,
                                        Registrado = lineaOriginal.Registrado,
                                        CodEnt = lineaOriginal.CodEnt,
                                        Validado = lineaOriginal.Validado,
                                        Validador = lineaOriginal.Validador
                                    });
                                }
                            }
                            ViewBag.Lineas = listVisual;
                            if (!(aldakinDbContext.Estadodias.FirstOrDefault(x => x.Dia == dtSelected.Date && x.Idusuario == iUserId) is null))
                            {
                                ViewBag.SemanaCerrada = true;
                            }
                            else
                            {
                                ViewBag.SemanaCerrada = false;
                            }
                            ViewBag.DateSelected = dtSelected.ToString("yyyy-MM-dd"); // dtSelected.Date;
                        }
                        catch (Exception ex)
                        {
                            return RedirectToAction("Index", new { strMessage = "ocurrio un error!!!" });
                        }
                        break;

                    case "editLine":
                        if (lSelect is null) return RedirectToAction("Index", new { strMessage = "Error en la seleccion de parte" });
                        day = lSelect.Inicio;
                        lEstadoDia = aldakinDbContext.Estadodias.Where(x => x.Idusuario == iUserId && DateTime.Compare(x.Dia, day.Date) == 0).ToList();//
                        if (lEstadoDia.Count > 0)
                        {
                            return RedirectToAction("Index", new { strMessage = "La semana esta cerrada, habla con tu responsable para reabirla;" });
                        }
                        NombreOt = aldakinDbContext.Ots.FirstOrDefault(x => x.Idots == lSelect.Idot).Nombre;
                        NombreCliente = aldakinDbContext.Clientes.FirstOrDefault(x => x.Idclientes == aldakinDbContext.Ots.FirstOrDefault(o => o.Idots == lSelect.Idot).Cliente).Nombre;
                        List<Gastos> lGastos = aldakinDbContext.Gastos.Where(x => x.Idlinea == lSelect.Idlinea).ToList();
                        int iCont = 0;
                        string strGastos = "";
                        foreach (Gastos g in lGastos)
                        {
                            string strPagador = "";
                            string strTipo = aldakinDbContext.Tipogastos.FirstOrDefault(x => x.Idtipogastos == g.Tipo).Tipo;
                            iCont++;
                            if (g.Pagador == 0)
                            {
                                strPagador = "TRABAJADOR";
                            }
                            else
                            {
                                strPagador = "ALDAKIN";
                            }
                            strGastos = strGastos + iCont + "|" + strPagador + "|" + strTipo + "|" + g.Cantidad + "|" + g.Observacion + "\r\n";
                        }
                        listVisual.Add(new LineaVisual
                        {
                            Idlinea = lSelect.Idlinea,
                            Idot = lSelect.Idot,
                            NombreOt = NombreOt,
                            NombreCliente = NombreCliente,
                            Idpreslin = lSelect.Idpreslin,
                            Dietas = lSelect.Dietas,
                            Km = lSelect.Km,
                            Observaciones = lSelect.Observaciones,
                            Horasviaje = lSelect.Horasviaje,
                            Horas = lSelect.Horas,
                            Inicio = lSelect.Inicio,
                            Fin = lSelect.Fin,
                            Idusuario = lSelect.Idusuario,
                            Facturable = lSelect.Facturable,
                            Npartefirmado = lSelect.Npartefirmado,
                            Idoriginal = lSelect.Idoriginal,
                            Registrado = lSelect.Registrado,
                            CodEnt = lSelect.CodEnt,
                            Validado = lSelect.Validado,
                            Validador = lSelect.Validador,
                            Gastos = strGastos,
                            ContGastos = iCont + 1
                        });
                        ViewBag.LineasSelect = listVisual;
                        //obtener pernoctaciones
                        List<Pernoctaciones> lPernoctas = aldakinDbContext.Pernoctaciones.Where(x => x.CodEnt == lSelect.CodEnt).ToList();
                        ViewBag.Pernoctas = lPernoctas;

                        ViewBag.DateSelected = lSelect.Inicio.Date.ToString("yyyy-MM-dd"); // dtSelected.Date;
                        break;
                    default:
                        return View();
                        break;
                }
            }
            return View();
        }
        //[HttpPost]
        //public async Task<IActionResult> DeleteLine(int cantidad)
        //{
        //    ajusteUsuario();

        //    List<SelectData> listaSelect = new List<SelectData>();
        //    listaSelect = null;
        //    string strReturn = string.Empty;
        //    int iIdLinea = 0;
        //    try
        //    {
        //        iIdLinea = Convert.ToInt32(cantidad);
        //    }
        //    catch (Exception ex)
        //    {
        //        return RedirectToAction("Index", new { strMessage = "proceso abortado, error en los datos;" });
        //    }
        //    if (iIdLinea == 0) return RedirectToAction("Index", new { strMessage = "proceso abortado, error en los datos;" });
        //    var lSelect = aldakinDbContext.Lineas.FirstOrDefault(x => x.Idlinea == Convert.ToInt32(iIdLinea));
        //    DateTime day;
        //    if (lSelect is null) return RedirectToAction("Index", new { strMessage = "Error en la seleccion de parte" });
        //    day = lSelect.Inicio;
        //    List<Estadodias> lEstadoDia = aldakinDbContext.Estadodias.Where(x => x.Idusuario == iUserId && DateTime.Compare(x.Dia, day.Date) == 0).ToList();//
        //    if (lEstadoDia.Count > 0)
        //    {
        //        strReturn = lSelect.Inicio.Year + "-" + lSelect.Inicio.Month + "-" + lSelect.Inicio.Day;
        //        return RedirectToAction("Index", new { strMessage = "La semana esta cerrada, habla con tu responsable para reabirla;", strDate = strReturn, strAction = "loadWeek" });
        //    }

        //    gestion borrado parte
        //    DDBB.EscribirDatos(string.Format("Delete from Lineas where idlinea = '{0}' or idoriginal = '{0}'", idLinea));
        //    DDBB.EscribirDatos(string.Format("Delete from gastos where idlinea = '{0}'", idLinea));
        //    var transaction = await aldakinDbContext.Database.BeginTransactionAsync();
        //    try
        //    {
        //        var linea = new Lineas { Idlinea = lSelect.Idlinea };
        //        var linea = new Lineas();
        //        linea = lSelect;
        //        aldakinDbContext.Lineas.Remove(linea);
        //        var lineaSecundaria = aldakinDbContext.Lineas.FirstOrDefault(x => x.Idoriginal == lSelect.Idlinea);
        //        if (!(lineaSecundaria is null)) aldakinDbContext.Lineas.Remove(lineaSecundaria);

        //        List<Gastos> gasto = aldakinDbContext.Gastos.Where(x => x.Idlinea == lSelect.Idlinea).ToList();
        //        if (!(gasto is null)) aldakinDbContext.Gastos.RemoveRange(gasto);

        //        await aldakinDbContext.SaveChangesAsync();

        //        await transaction.CommitAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        await transaction.RollbackAsync();
        //        strReturn = lSelect.Inicio.Year + "-" + lSelect.Inicio.Month + "-" + lSelect.Inicio.Day;
        //        return RedirectToAction("Index", new { strMessage = "Ha ocurrido un problema durante el proceso de borrado el parte de trabajo.;", strDate = strReturn, strAction = "loadWeek" });
        //    }
        //    string strDay = string.Empty;
        //    string strMonth = string.Empty;
        //    if (lSelect.Inicio.Month < 10)
        //    {
        //        strMonth = "0" + lSelect.Inicio.Month;
        //    }
        //    else
        //    {
        //        strMonth = lSelect.Inicio.Month.ToString();
        //    }
        //    if (lSelect.Inicio.Day < 10)
        //    {
        //        strDay = "0" + lSelect.Inicio.Day;
        //    }
        //    else
        //    {
        //        strDay = lSelect.Inicio.Day.ToString();
        //    }
        //    strReturn = lSelect.Inicio.Year + "-" + strMonth + "-" + strDay;
        //    return RedirectToAction("Index", new { strMessage = "Parte Borrado Satisfactoriamente;", strDate = strReturn, strAction = "loadWeek" });
        //}

        [HttpPost]
        public async Task<JsonResult> DeleteLineFunction(int cantidad)
        {
            ajusteUsuario();

            List<SelectData> listaSelect = new List<SelectData>();
            List<SelectData> lReturn = new List<SelectData>();
            lReturn.Add(new SelectData { iValue = 0 });
            listaSelect = null;
            string strReturn = string.Empty;
            int iIdLinea = 0;
            try
            {
                iIdLinea = Convert.ToInt32(cantidad);
            }
            catch (Exception ex)
            {
                return Json(lReturn);// RedirectToAction("Index", new { strMessage = "proceso abortado, error en los datos;" });
            }
            if (iIdLinea == 0) return Json(lReturn);// RedirectToAction("Index", new { strMessage = "proceso abortado, error en los datos;" });
            var lSelect = aldakinDbContext.Lineas.FirstOrDefault(x => x.Idlinea == Convert.ToInt32(iIdLinea));
            DateTime day;
            if (lSelect is null) return Json(lReturn);// RedirectToAction("Index", new { strMessage = "Error en la seleccion de parte" });
            day = lSelect.Inicio;
            List<Estadodias> lEstadoDia = aldakinDbContext.Estadodias.Where(x => x.Idusuario == iUserId && DateTime.Compare(x.Dia, day.Date) == 0).ToList();//
            if (lEstadoDia.Count > 0)
            {
                //strReturn = lSelect.Inicio.Year + "-" + lSelect.Inicio.Month + "-" + lSelect.Inicio.Day;
                return Json(lReturn);// RedirectToAction("Index", new { strMessage = "La semana esta cerrada, habla con tu responsable para reabirla;", strDate = strReturn, strAction = "loadWeek" });
            }

            //gestion borrado parte
            //DDBB.EscribirDatos(string.Format("Delete from Lineas where idlinea = '{0}' or idoriginal = '{0}'", idLinea));
            //DDBB.EscribirDatos(string.Format("Delete from gastos where idlinea = '{0}'", idLinea));
            var transaction = await aldakinDbContext.Database.BeginTransactionAsync();
            try
            {
                //var linea = new Lineas { Idlinea = lSelect.Idlinea };
                var linea = new Lineas();
                linea = lSelect;
                aldakinDbContext.Lineas.Remove(linea);
                var lineaSecundaria = aldakinDbContext.Lineas.FirstOrDefault(x => x.Idoriginal == lSelect.Idlinea);
                if (!(lineaSecundaria is null)) aldakinDbContext.Lineas.Remove(lineaSecundaria);

                List<Gastos> gasto = aldakinDbContext.Gastos.Where(x => x.Idlinea == lSelect.Idlinea).ToList();
                if (!(gasto is null)) aldakinDbContext.Gastos.RemoveRange(gasto);

                await aldakinDbContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                strReturn = lSelect.Inicio.Year + "-" + lSelect.Inicio.Month + "-" + lSelect.Inicio.Day;
                return Json(lReturn);// RedirectToAction("Index", new { strMessage = "Ha ocurrido un problema durante el proceso de borrado el parte de trabajo.;", strDate = strReturn, strAction = "loadWeek" });
            }
            string strDay = string.Empty;
            string strMonth = string.Empty;
            if (lSelect.Inicio.Month < 10)
            {
                strMonth = "0" + lSelect.Inicio.Month;
            }
            else
            {
                strMonth = lSelect.Inicio.Month.ToString();
            }
            if (lSelect.Inicio.Day < 10)
            {
                strDay = "0" + lSelect.Inicio.Day;
            }
            else
            {
                strDay = lSelect.Inicio.Day.ToString();
            }
            strReturn = lSelect.Inicio.Year + "-" + strMonth + "-" + strDay;
            lReturn.First().iValue = 1;
            lReturn.Add(new SelectData { iValue = 1,strText= "loadWeek", strValue= strReturn });
            return Json(lReturn);// RedirectToAction("Index", new { strMessage = "Parte Borrado Satisfactoriamente;", strDate = strReturn, strAction = "loadWeek" });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CloseWeek(string strDataSelected)
        {
            //datos provisionles
            ajusteUsuario();
            DateTime dtSelected;
            try
            {
                dtSelected = Convert.ToDateTime(strDataSelected);
            }
            catch(Exception ex)
            {
                return RedirectToAction("Index", new { strMessage = "Proceso abortado, error en los datos;" });
            }
            List<Estadodias> lEstadoDia = aldakinDbContext.Estadodias.Where(x => x.Idusuario == iUserId && DateTime.Compare(x.Dia, dtSelected) == 0).ToList();//
            if (lEstadoDia.Count > 0)
            {
                return RedirectToAction("Index", new { strMessage = "La semana esta cerrada, habla con tu responsable si es necesario abrirla;" });
            }
            DateTime dtIniWeek;
            DateTime dtEndWeek;
            System.DayOfWeek day = dtSelected.DayOfWeek;
            switch (day)
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
                    return RedirectToAction("Index", new { strMessage = "Error en la seleccion de dia!!!" });
            }
            if (dtIniWeek.Month != dtEndWeek.Month)
            {
                if (dtSelected.Month == dtEndWeek.Month)
                {
                    dtIniWeek = new DateTime(dtEndWeek.Year, dtEndWeek.Month, 1);
                }
                else if (dtSelected.Month == dtIniWeek.Month)
                {
                    dtEndWeek = new DateTime(dtIniWeek.Year, dtIniWeek.Month, 1).AddMonths(1).AddDays(-1);
                }
            }
            List<double> ldHorasdia = new List<double>();
            List<Estadodias> lListEstados = new List<Estadodias>();
            for (DateTime date = dtIniWeek; date < dtEndWeek; date = date.AddDays(1.0))
            {
                double dHorasDia = 0;
                listPartes = aldakinDbContext.Lineas.Where(x => x.Inicio.Date == date.Date && x.CodEnt == iUserCondEntO && x.Idusuario == iUserId).OrderBy(x => x.Inicio).ToList();
                foreach (Lineas l in listPartes)
                {
                    dHorasDia = dHorasDia + (l.Fin - l.Inicio).TotalHours;
                }
                ldHorasdia.Add(dHorasDia);
                lListEstados.Add(new Estadodias { Dia = date.Date,Idusuario=iUserId ,Estado=2,Horas= (float)(dHorasDia) });//
            }
            var transaction = await aldakinDbContext.Database.BeginTransactionAsync();
            try
            {
                aldakinDbContext.Estadodias.AddRange(lListEstados);
                await aldakinDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return RedirectToAction("Index", new { strMessage = "Ha ocurrido un problema durante el proceso de cerrar semana.;" });
            }

            string strReturn = dtSelected.Year + "-" + dtSelected.Month + "-" + dtSelected.Day;
            return RedirectToAction("Index", new { strMessage = "Semana Cerrada;", strDate = strReturn, strAction = "loadWeek" });

            //DDBB.CerrarSemana(Program.par_aplicacion.CurrentUser.idUsuario, primerDia, ultimoDia);

            //procedure de cerrar semana



            //var connection = (SqlConnection)context.Database.AsSqlServer().Connection.DbConnection;

            //var cmd = connection.CreateCommand();
            //cmd.CommandType = CommandType.StoredProcedure;
            //cmd.CommandText = "CerrarSemana";
            //cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.AddWithValue("@iidusuario", iId);
            //cmd.Parameters.AddWithValue("@iprimerdia", dtIniWeek);
            //cmd.Parameters.AddWithValue("@iultimodia", dtEndWeek);
            //cmd.ExecuteNonQuery();
            //cmd.Prepare();
            //cmd.ExecuteNonQuery();

        }
        [HttpPost]
        public JsonResult PagadorSelect(int cantidad, int cantidad2)
        {
            List<SelectData> listaSelect = new List<SelectData>();
            if(cantidad2<1) return Json(null);
            List<Tipogastos> tipoGasto = aldakinDbContext.Tipogastos.Where(x => x.Pagador == cantidad && x.CodEnt == aldakinDbContext.Ots.FirstOrDefault(o => o.Idots == cantidad2).CodEnt).ToList();
            if (!(tipoGasto == null))
            {
                foreach (Tipogastos p in tipoGasto)
                {
                    string strTemp = p.Tipo.ToUpper();
                    listaSelect.Add(new SelectData { strValue = strTemp, strText = strTemp });
                }
            }
            return Json(listaSelect);

        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLine(string strIdLinea, string ot, string strCalendario, string strHoraInicio, string strMinutoInicio, string strHoraFin, string strMinutoFin, string bHorasViaje, string bGastos, string strParte, string strPernoctacion, string strObservaciones, string strGastos)
        {
            //datos provisionles
            ajusteUsuario();
            string strReturn = string.Empty;
            bool bHorasViajeTemp = false;
            bool gGastosTemp = false;
            int iPernoctacion = 0;
            DateTime day, dtInicio, dtFin;
            var datosLinea = aldakinDbContext.Lineas.FirstOrDefault(x => x.Idlinea == Convert.ToInt32(strIdLinea));
            try
            {
                if (string.IsNullOrEmpty(strObservaciones)) strObservaciones = string.Empty;
                if (string.IsNullOrEmpty(strParte)) strParte = string.Empty;
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
                    gGastosTemp = false;
                }
                else
                {
                    gGastosTemp = true;
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
            catch (Exception ex)
            {
                return RedirectToAction("Index", new { strMessage = "Se ha producido un error en el procesamiento de los datos;" });
            }
            //Estado dia
            //"select estado from estadodias where idusuario = {0} and date(dia) = '{1}'", user.idUsuario, dia.ToString("yyyy-MM-dd")
            day = Convert.ToDateTime(strCalendario);
            dtInicio = Convert.ToDateTime(strCalendario + " " + strHoraInicio + ":" + strMinutoInicio + ":00");
            dtFin = Convert.ToDateTime(strCalendario + " " + strHoraFin + ":" + strMinutoFin + ":00");
            List<Estadodias> lEstadoDia = aldakinDbContext.Estadodias.Where(x => x.Idusuario == iUserId && DateTime.Compare(x.Dia, day) == 0).ToList();//
            if (lEstadoDia.Count > 0)
            {
                strReturn = datosLinea.Inicio.Year + "-" + datosLinea.Inicio.Month + "-" + datosLinea.Inicio.Day;
                return RedirectToAction("Index", new { strMessage = "La semana esta cerrada, habla con tu responsable para reabirla;", strDate = strReturn, strAction = "loadWeek" });
            }
            if (DateTime.Compare(dtInicio, dtFin) > 0)
            {
                strReturn = datosLinea.Inicio.Year + "-" + datosLinea.Inicio.Month + "-" + datosLinea.Inicio.Day;
                return RedirectToAction("Index", new { strMessage = "Hora de Fin de Parte anterior a la Hora de inicio de Parte;", strDate = strReturn, strAction = "loadWeek" });
            }
            //Rango usado
            List<Lineas> lLineas = aldakinDbContext.Lineas.Where(x => DateTime.Compare(x.Inicio.Date, day.Date) == 0 && x.Idusuario == iUserId && x.Validado == 0 && x.Registrado == 0 && x.Idlinea != datosLinea.Idlinea && x.Idoriginal!=datosLinea.Idlinea ).ToList();
            if (lLineas != null)
            {
                foreach (Lineas x in lLineas)
                {
                    if (DateTime.Compare(dtFin, x.Inicio) > 0 && DateTime.Compare(dtFin, x.Fin) < 0)
                    {
                        strReturn = datosLinea.Inicio.Year + "-" + datosLinea.Inicio.Month + "-" + datosLinea.Inicio.Day;
                        return RedirectToAction("Index", new { strMessage = "Rango de Horas ya utilizado;", strDate = strReturn, strAction = "loadWeek" });
                    }
                    if (DateTime.Compare(dtInicio, x.Inicio) > 0 && DateTime.Compare(dtInicio, x.Fin) < 0)
                    {
                        strReturn = datosLinea.Inicio.Year + "-" + datosLinea.Inicio.Month + "-" + datosLinea.Inicio.Day;
                        return RedirectToAction("Index", new { strMessage = "Rango de Horas ya utilizado;", strDate = strReturn, strAction = "loadWeek" });
                    }
                }
            }
            //
            double dHorasTrabajadas = (dtFin - dtInicio).TotalHours;
            if ((dHorasTrabajadas == 0) && !gGastosTemp)
            {
                strReturn = datosLinea.Inicio.Year + "-" + datosLinea.Inicio.Month + "-" + datosLinea.Inicio.Day;
                return RedirectToAction("Index", new { strMessage = "error 24h trabajadas y no puesto solo gastos;", strDate = strReturn, strAction = "loadWeek" });
            }
            //tiempo de viaje
            float TiempoViaje = 0;
            if (bHorasViajeTemp)
            {
                TiempoViaje = Convert.ToSingle((dtFin - dtInicio).TotalHours);
            }
            //gastos
            float dGastos = 0;
            float dKilometros = 0;
            int iCodEntOt = aldakinDbContext.Ots.FirstOrDefault(t => t.Idots == Convert.ToInt32(ot)).CodEnt;
            List<Gastos> lGastos = new List<Gastos>();
            if (!(String.IsNullOrEmpty(strGastos)))
            {
                string line;
                string[] substring;
                StringReader strReader = new StringReader(strGastos);
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
                                    //strReturn = datosLinea.Inicio.Year + "-" + datosLinea.Inicio.Month + "-" + datosLinea.Inicio.Day;
                                    //return RedirectToAction("Index", new { strMessage = "Los gastos son erroneos, repita el parte;", strDate = strReturn, strAction = "loadWeek" });
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

                                    if (substring[2] != "KILOMETROS") dGastos = dGastos + (float)Convert.ToDouble(substring[3].Replace('.', ','));
                                    else dKilometros = dKilometros + (float)Convert.ToDouble(substring[3].Replace('.', ','));
                                }
                            }
                            catch (Exception ex)
                            {
                                strReturn = datosLinea.Inicio.Year + "-" + datosLinea.Inicio.Month + "-" + datosLinea.Inicio.Day;
                                return RedirectToAction("Index", new { strMessage = "Los gastos son erroneos, repita el parte;", strDate = strReturn, strAction = "loadWeek" });
                            }
                        }
                        else
                        {
                            //si hay error no hace nada con la lineapara que siga con la siguiente
                            //strReturn = datosLinea.Inicio.Year + "-" + datosLinea.Inicio.Month + "-" + datosLinea.Inicio.Day;
                            //return RedirectToAction("Index", new { strMessage = "Los gastos son erroneos, repita el parte;", strDate = strReturn, strAction = "loadWeek" });
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            var otSel = aldakinDbContext.Ots.FirstOrDefault(x => x.Idots == Convert.ToInt32(ot) && x.Codigorefot != "29" && x.Cierre == null);
            if (otSel is null) return RedirectToAction("Index", new { strMessage = "En la Ot que esta usande se ha encontrado un problema, recargue la pagina;" });
            if (otSel.Nombre.Length > 10 && otSel.Nombre.Substring(0, 20) == "TRABAJOS REALIZADOS ")
            {
                try
                {
                    int otoriginal = Convert.ToInt32(strObservaciones.Substring(0, 1));
                }
                catch (Exception ex)
                {
                    strReturn = datosLinea.Inicio.Year + "-" + datosLinea.Inicio.Month + "-" + datosLinea.Inicio.Day;
                    return RedirectToAction("Index", new { strMessage = "En las OTs de trabajos para otras delegaciones, lo primero que debe aparecer en las observaciones debe ser la OT de la delegacion de origen;", strDate = strReturn, strAction = "loadWeek" });
                }
            }
            if (otSel.CodEnt == iUserCondEntO)
            {
                datosLinea.Dietas = dGastos;
                datosLinea.Km = dKilometros;
                datosLinea.Observaciones = strObservaciones.ToUpper();
                datosLinea.Horasviaje = TiempoViaje;
                datosLinea.Inicio = dtInicio;
                datosLinea.Fin = dtFin;
                datosLinea.Horas = Convert.ToSingle((dtFin - dtInicio).TotalHours) - TiempoViaje;
                datosLinea.Facturable = iPernoctacion;
                datosLinea.Npartefirmado = strParte.ToUpper();
                datosLinea.Idoriginal = 0;
                datosLinea.Validador = string.Empty;
                datosLinea.Validado = 0;
                var transaction = await aldakinDbContext.Database.BeginTransactionAsync();
                try
                {

                    aldakinDbContext.Lineas.Update(datosLinea);
                    await aldakinDbContext.SaveChangesAsync();
                    List<Gastos> gasto = aldakinDbContext.Gastos.Where(x => x.Idlinea == datosLinea.Idlinea).ToList();
                    if (!(gasto is null)) aldakinDbContext.Gastos.RemoveRange(gasto);
                    foreach (Gastos g in lGastos)
                    {
                        var gastoNew = new Gastos
                        {
                            Pagador = g.Pagador,
                            Tipo = g.Tipo,
                            Cantidad = g.Cantidad,
                            Observacion = g.Observacion.ToUpper(),
                            Idlinea = datosLinea.Idlinea
                        };
                        aldakinDbContext.Gastos.Add(gastoNew);
                    }
                    await aldakinDbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    strReturn = datosLinea.Inicio.Year + "-" + datosLinea.Inicio.Month + "-" + datosLinea.Inicio.Day;
                    return RedirectToAction("Index", new { strMessage = "Ha ocurrido un problema durante el proceso de guardar el parte de trabajo.;", strDate = strReturn, strAction = "loadWeek" });
                }
            }
            else
            {
                var user = aldakinDbContext.Usuarios.FirstOrDefault(x => x.Name == stUserrDni && x.CodEnt == otSel.CodEnt);
                if (!(user is null))
                {
                    datosLinea.Dietas = dGastos;
                    datosLinea.Km = dKilometros;
                    datosLinea.Observaciones = strObservaciones.ToUpper();
                    datosLinea.Horasviaje = TiempoViaje;
                    datosLinea.Inicio = dtInicio;
                    datosLinea.Fin = dtFin;
                    datosLinea.Horas = Convert.ToSingle((dtFin - dtInicio).TotalHours) - TiempoViaje;
                    datosLinea.Facturable = iPernoctacion;
                    datosLinea.Npartefirmado = strParte.ToUpper();
                    datosLinea.Idoriginal = 0;
                    datosLinea.Validador = string.Empty;
                    datosLinea.Validado = 0;
                    var transaction = await aldakinDbContext.Database.BeginTransactionAsync();
                    try
                    {
                        aldakinDbContext.Lineas.Update(datosLinea);
                        await aldakinDbContext.SaveChangesAsync();
                        List<Gastos> gasto = aldakinDbContext.Gastos.Where(x => x.Idlinea == datosLinea.Idlinea).ToList();
                        if (!(gasto is null)) aldakinDbContext.Gastos.RemoveRange(gasto);
                        foreach (Gastos g in lGastos)
                        {
                            var gastoNew = new Gastos
                            {
                                Pagador = g.Pagador,
                                Tipo = g.Tipo,
                                Cantidad = g.Cantidad,
                                Observacion = g.Observacion.ToUpper(),
                                Idlinea = datosLinea.Idlinea
                            };
                            aldakinDbContext.Gastos.Add(gastoNew);
                        }
                        await aldakinDbContext.SaveChangesAsync();

                        string Salida = otSel.Numero.ToString(), Primerdigito, Resto;
                        char Cero = Convert.ToChar("0"); 
                        Primerdigito = otSel.Numero.ToString().Substring(0, 1);
                        Resto = otSel.Numero.ToString().Substring(2, otSel.Numero.ToString().Length - 2);
                        Resto = Resto.TrimStart(Cero);
                        Salida = string.Format("{0}|{1}", Primerdigito, Resto);
                        string observaciones = Salida + " " + datosLinea.Observaciones.ToUpper();

                        var secundarioAntigua = aldakinDbContext.Lineas.FirstOrDefault(x => x.Idoriginal == datosLinea.Idlinea);
                        aldakinDbContext.Lineas.Remove(secundarioAntigua);
                        //from ots where cierre is null and year(apertura) = year(iinicio) and  cod_ent = icod_ent and cod_ent_d = (select cod_ent from lineas where idlinea = iidoriginal)
                        int iOt = aldakinDbContext.Ots.FirstOrDefault(x => x.Cierre == null && x.Apertura.Year == datosLinea.Inicio.Year && x.CodEnt == iUserCondEntO && x.CodEntD == aldakinDbContext.Lineas.FirstOrDefault(y => y.Idlinea == datosLinea.Idlinea).CodEnt).Idots;
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
                            Idoriginal = datosLinea.Idlinea,
                            Validador = string.Empty,
                            Validado = 0
                        };
                        aldakinDbContext.Lineas.Add(lineaSecundaria);
                        await aldakinDbContext.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        strReturn = datosLinea.Inicio.Year + "-" + datosLinea.Inicio.Month + "-" + datosLinea.Inicio.Day;
                        return RedirectToAction("Index", new { strMessage = "Ha ocurrido un problema durante el proceso de guardar el parte de trabajo.;", strDate = strReturn, strAction = "loadWeek" });
                    }
                }
                else
                {
                    strReturn = datosLinea.Inicio.Year + "-" + datosLinea.Inicio.Month + "-" + datosLinea.Inicio.Day;
                    return RedirectToAction("Index", new { strMessage = "¡¡¡No estas dado de alta en la delegacion para la que quieres registrar el parte!!!Informe a admisntracion de lo ocurrido;", strDate = strReturn, strAction = "loadWeek" });
                }
            }
            strReturn = datosLinea.Inicio.Year + "-" + datosLinea.Inicio.Month + "-" + datosLinea.Inicio.Day;
            return RedirectToAction("Index", new { strMessage = "Parte de trabajo actualizado correctamente;", strDate = strReturn, strAction = "loadWeek" });
        }
    }

    public class LineaVisual
    {
        public int Idlinea { get; set; }
        public int Idot { get; set; }
        public string NombreOt { get; set; }
        public string NombreCliente { get; set; }
        public int? Idpreslin { get; set; }
        public string NombrePreslin { get; set; }
        public float? Dietas { get; set; }
        public float? Km { get; set; }
        public string Observaciones { get; set; }
        public float? Horasviaje { get; set; }
        public float Horas { get; set; }
        public DateTime Inicio { get; set; }
        public DateTime Fin { get; set; }
        public int Idusuario { get; set; }
        public int Facturable { get; set; }
        public string strPernocta { get; set; }
        public string Npartefirmado { get; set; }
        public int? Idoriginal { get; set; }
        public sbyte Registrado { get; set; }
        public int CodEnt { get; set; }
        public sbyte? Validado { get; set; }
        public string Validador { get; set; }
        public string Gastos { get; set; }
        public int ContGastos { get; set; }
    }
}