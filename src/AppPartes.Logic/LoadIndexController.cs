using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AppPartes.Data.Models;

namespace AppPartes.Logic
{
    public class LoadIndexController: ILoadIndexController
    {
        private readonly AldakinDbContext aldakinDbContext;
        //apaño para usuario con claims
        private string strUserName = "";
        private string stUserrDni = "";
        private int iUserId = 0;
        private int iUserCondEntO = 0;
        public LoadIndexController(AldakinDbContext aldakinDbContext)
        {
            this.aldakinDbContext = aldakinDbContext;
            AjusteUsuario();
        }
        private void AjusteUsuario()
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
        public MainDataViewLogic LoadMainController()
        {
            MainDataViewLogic oReturn = new MainDataViewLogic();
            List<Ots> listOts;
            List<Entidad> listadept;
            List<Clientes> listaClient;
            //List<Presupuestos> lPresupuestos;
            List<Pernoctaciones> lPernoctas;
            //pruebas con claims
            //var identity = ((ClaimsIdentity)Thread.CurrentPrincipal.Identity);
            //var claim = identity.Claims.Where(x => x.Type == ClaimTypes.Name).FirstOrDefault();
            //var name =User.Claims.FirstOrDefault(x => x.Type.ToString().Equals("id", StringComparison.InvariantCultureIgnoreCase));

            AjusteUsuario();
            //obtener ots
            var totalOts = aldakinDbContext.Ots.Where(x => x.CodEnt == iUserCondEntO && x.CodEntD == 0 && x.Codigorefot != "29" && x.Cierre == null);
            var totalType1Ots = totalOts.Where(x => x.Tipoot == 1);
            var totalType2Ots = totalOts.Join(aldakinDbContext.Presupuestos.Where(x => x.CodEnt == iUserCondEntO), o => o.Idots, i => i.Idot, (o, p) => o);//original
            listOts = totalType1Ots.Concat(totalType2Ots).Distinct().OrderBy(x => x.Numero).ToList();
            oReturn.listOts = listOts;
            //lReturn.Add(new DataLogic { listaOts = listOts });
            //consulta original
            //select * from Ots where  cod_ent = {0} and cod_ent_d = 0 and codigorefot != 29 and cierre IS NULL and (tipoot = 1 or idots in (select idot from presupuestos where cod_ent = {0})), cod_ent)

            //obtener empresas
            var aux = aldakinDbContext.Entidad.FirstOrDefault(x => x.CodEnt == iUserCondEntO);
            listadept = aldakinDbContext.Entidad.Where(x => x.CodEnt != iUserCondEntO).OrderByDescending(x => x.Nombre).ToList();
            listadept.Insert(0, aux);
            oReturn.listCompany = listadept;

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

            oReturn.listClient = listaClient;

            //obtener pernoctaciones
            lPernoctas = aldakinDbContext.Pernoctaciones.Where(x => x.CodEnt == iUserCondEntO).ToList();
            oReturn.listNight = lPernoctas;

            return oReturn;
        }

        public WeekDataViewLogic LoadWeekController(string strDate = "", string strAction = "", string strId = "")
        {
            WeekDataViewLogic oReturn = new WeekDataViewLogic();
            AjusteUsuario();
            DateTime day,dtIniWeek=DateTime.Now, dtEndWeek= DateTime.Now;
            if (string.IsNullOrEmpty(strDate) && string.IsNullOrEmpty(strAction))
            {
                //oReturn.Mensaje = "";
            }
            else
            {
                var listVisual = new List<LineaVisual>();
                var NombreOt = string.Empty;
                var NombreCliente = string.Empty;
                var strPernocta = string.Empty;
                var strPreslin = string.Empty;
                if ((string.IsNullOrEmpty(strId)))
                {
                    strId = "0";
                }

                var lSelect = aldakinDbContext.Lineas.FirstOrDefault(x => x.Idlinea == Convert.ToInt32(strId));
                List<Estadodias> lEstadoDia;
                //ViewBag.Semana = null;
                //ViewBag.Lineas = null;
                //ViewBag.DateSelected = null;
                switch (strAction)
                {
                    case "loadWeek":
                        try
                        {
                            var dtSelected = Convert.ToDateTime(strDate);
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
                                    //oReturn = null;
                                    break;
                            }
                            var ldHorasdia = new List<double>();
                            for (var date = dtIniWeek; date < dtEndWeek; date = date.AddDays(1.0))
                            {
                                double dHorasDia = 0;
                                var Temp = aldakinDbContext.Lineas.Where(x => x.Inicio.Date == date.Date && x.CodEnt == iUserCondEntO && x.Idusuario == iUserId).OrderBy(x => x.Inicio).ToList();
                                foreach (var l in Temp)
                                {
                                    dHorasDia = dHorasDia + (l.Fin - l.Inicio).TotalHours;
                                }
                                ldHorasdia.Add(dHorasDia);
                            }
                            oReturn.listSemana = ldHorasdia;
                            
                            var lTemp = aldakinDbContext.Lineas.Where(x => x.Inicio > dtIniWeek && x.Fin < dtEndWeek && x.Idusuario == iUserId && x.CodEnt == iUserCondEntO).OrderBy(x => x.Inicio).ToList();
                            
                            //aqui tengo que recorrer lisPartes e ir volvandolo a otra clase nueva para nomstrar losnombres de las ots y empresas...
                            //ademas tengo que ver si es la linea original o la secundaria para que si es la secundaria la cambie por la principal
                            foreach (var l in lTemp)
                            {
                                NombreOt = aldakinDbContext.Ots.FirstOrDefault(x => x.Idots == l.Idot).Nombre;
                                NombreCliente = aldakinDbContext.Clientes.FirstOrDefault(x => x.Idclientes == aldakinDbContext.Ots.FirstOrDefault(o => o.Idots == l.Idot).Cliente).Nombre;
                                if (l.Facturable == 0)
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
                                        strPernocta = strPernocta,
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
                            oReturn.listPartes = listVisual;
                            if (!(aldakinDbContext.Estadodias.FirstOrDefault(x => x.Dia == dtSelected.Date && x.Idusuario == iUserId) is null))
                            {
                                oReturn.SemanaCerrada = true;
                            }
                            else
                            {
                                oReturn.SemanaCerrada = false;
                            }
                            oReturn.DateSelected = dtSelected.ToString("yyyy-MM-dd"); // dtSelected.Date;
                        }
                        catch (Exception)
                        {
                            oReturn.Mensaje = "ocurrio un error!!!";
                        }
                        break;

                    case "editLine":
                        if (lSelect is null)
                        {
                            oReturn.Mensaje = "Error en la seleccion de parte";
                        }

                        day = lSelect.Inicio;
                        lEstadoDia = aldakinDbContext.Estadodias.Where(x => x.Idusuario == iUserId && DateTime.Compare(x.Dia, day.Date) == 0).ToList();//
                        if (lEstadoDia.Count > 0)
                        {
                            oReturn.Mensaje = "La semana esta cerrada, habla con tu responsable para reabirla;";
                        }
                        NombreOt = aldakinDbContext.Ots.FirstOrDefault(x => x.Idots == lSelect.Idot).Nombre;
                        NombreCliente = aldakinDbContext.Clientes.FirstOrDefault(x => x.Idclientes == aldakinDbContext.Ots.FirstOrDefault(o => o.Idots == lSelect.Idot).Cliente).Nombre;
                        var lGastos = aldakinDbContext.Gastos.Where(x => x.Idlinea == lSelect.Idlinea).ToList();
                        var iCont = 0;
                        var strGastos = "";
                        foreach (var g in lGastos)
                        {
                            var strPagador = "";
                            var strTipo = aldakinDbContext.Tipogastos.FirstOrDefault(x => x.Idtipogastos == g.Tipo).Tipo;
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
                        oReturn.listSelect = listVisual;
                        //obtener pernoctaciones
                        var lPernoctas = aldakinDbContext.Pernoctaciones.Where(x => x.CodEnt == lSelect.CodEnt).ToList();
                        oReturn.listPernocta = lPernoctas;

                        oReturn.DateSelected = lSelect.Inicio.Date.ToString("yyyy-MM-dd"); // dtSelected.Date;
                        break;
                    default:
                        //return View();
                        break;
                }
            }
            return oReturn;
        }
    }
}
