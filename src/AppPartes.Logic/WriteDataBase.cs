using AppPartes.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AppPartes.Logic
{
    public class WriteDataBase : IWriteDataBase
    {
        private readonly AldakinDbContext aldakinDbContext;
        //apaño para usuario con claims
        private string _strUserName = "";
        private string _stUserDni = "";
        private int _iUserId = 0;
        private int _iUserCondEntO = 0;
        private int _iUserLevel = 0;
        public WriteDataBase(AldakinDbContext aldakinDbContext)
        {
            this.aldakinDbContext = aldakinDbContext;
        }
        private async void WriteUserDataAsync(int idAldakinUser)
        {
            var user = await GetUserDataAsync(idAldakinUser);
            _strUserName = user.strUserName;
            _iUserId = user.iUserId;
            _iUserCondEntO = user.iUserCondEntO;
            _stUserDni = user.stUserrDni;
            _iUserLevel = user.iLevel;
        }
        public async Task<UserData> GetUserDataAsync(int idAldakinUser)
        {
            UserData oReturn = new UserData();
            try
            {
                var user = aldakinDbContext.Usuarios.FirstOrDefault(x => x.Idusuario == idAldakinUser && x.CodEnt == x.CodEntO);
                if (!(user is null))
                {
                    var writeUser = new UserData
                    {
                        strUserName = user.Nombrecompleto.ToString(),
                        iUserId = Convert.ToInt16(user.Idusuario),
                        iUserCondEntO = Convert.ToInt16(user.CodEntO),
                        stUserrDni = user.Name,
                        iLevel = user.Autorizacion
                    };
                    oReturn = writeUser;
                }
            }
            catch (Exception ex)
            {
                oReturn = null;
            }
            return oReturn;
        }
        public async Task<string> InsertWorkerLineAsync(WorkerLineData dataToInsertLine, int idAldakinUser)
        {
            WriteUserDataAsync(idAldakinUser);
            string strReturn = string.Empty;
            //datos provisionles
            var datosLinea = new Lineas();
            var bHorasViajeTemp = false;
            var iPernoctacion = 0;
            DateTime day, dtInicio, dtFin;
            //Estado dia
            //"select estado from estadodias where idusuario = {0} and date(dia) = '{1}'", user.idUsuario, dia.ToString("yyyy-MM-dd")
            try
            {
                if (string.IsNullOrEmpty(dataToInsertLine.strObservaciones))
                {
                    dataToInsertLine.strObservaciones = string.Empty;
                }
                if (string.IsNullOrEmpty(dataToInsertLine.strParte))
                {
                    dataToInsertLine.strParte = string.Empty;
                }
                if (string.IsNullOrEmpty(dataToInsertLine.bHorasViaje))
                {
                    bHorasViajeTemp = false;
                }
                else
                {
                    bHorasViajeTemp = true;
                }
                if (string.IsNullOrEmpty(dataToInsertLine.bGastos))
                {
                }
                else
                {
                    dataToInsertLine.strHoraInicio = "00";
                    dataToInsertLine.strMinutoInicio = "00";
                    dataToInsertLine.strHoraFin = "00";
                    dataToInsertLine.strMinutoFin = "00";
                }
                if (string.IsNullOrEmpty(dataToInsertLine.strPernoctacion))
                {
                    iPernoctacion = 0;
                }
                else
                {
                    iPernoctacion = Convert.ToInt32(dataToInsertLine.strPernoctacion);
                }

                day = Convert.ToDateTime(dataToInsertLine.strCalendario);
                dtInicio = Convert.ToDateTime(dataToInsertLine.strCalendario + " " + dataToInsertLine.strHoraInicio + ":" + dataToInsertLine.strMinutoInicio + ":00");
                dtFin = Convert.ToDateTime(dataToInsertLine.strCalendario + " " + dataToInsertLine.strHoraFin + ":" + dataToInsertLine.strMinutoFin + ":00");
            }
            catch (Exception ex)
            {
                strReturn = "Se ha producido un error en el procesamiento de los datos;";
                return (strReturn);
            }
            var lEstadoDia = await aldakinDbContext.Estadodias.Where(x => x.Idusuario == _iUserId && DateTime.Compare(x.Dia, day) == 0).ToListAsync();//
            if (lEstadoDia.Count > 0)
            {
                strReturn = "La semana esta cerrada, habla con tu responsable para reabirla;";
                return (strReturn);
            }
            if (DateTime.Compare(dtInicio, dtFin) > 0)
            {
                strReturn = "Hora de Fin de Parte anterior a la Hora de inicio de Parte;";
                return (strReturn);
            }
            //Review if range of time is used
            var lLineas = await aldakinDbContext.Lineas.Where(x => DateTime.Compare(x.Inicio.Date, day.Date) == 0 && x.Idusuario == _iUserId && x.Validado == 0 && x.Registrado == 0).ToListAsync();
            if (RangeIsUsed(lLineas, dtFin, dtInicio, ref strReturn)) return strReturn;

            //gastos
            float dGastos = 0;
            float dKilometros = 0;
            var icodEntOt = await aldakinDbContext.Ots.FirstOrDefaultAsync(t => t.Idots == Convert.ToInt32(dataToInsertLine.strOt));
            var iCodEntOt = icodEntOt.CodEnt;
            var lGastos = new List<Gastos>();

            if (!(string.IsNullOrEmpty(dataToInsertLine.strGastos)))
            {
                string line;
                string[] substring;
                var strReader = new StringReader(dataToInsertLine.strGastos);
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
                                var pagador = await aldakinDbContext.Tipogastos.FirstOrDefaultAsync(x => x.CodEnt == iCodEntOt && string.Equals(x.Tipo, substring[2]) && x.Pagador == Convert.ToInt32(substring[1]));
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

            var otSel = await aldakinDbContext.Ots.FirstOrDefaultAsync(x => x.Idots == Convert.ToInt32(dataToInsertLine.strOt) && x.Codigorefot != "29" && x.Cierre == null);
            if (otSel is null)
            {
                strReturn = "En la Ot que esta usando se ha encontrado un problema, recargue la pagina;";
                return (strReturn);
            }

            if (otSel.Nombre.Length > 20 && otSel.Nombre.Substring(0, 20) == "TRABAJOS REALIZADOS ")
            {
                try
                {
                    var otoriginal = Convert.ToInt32(dataToInsertLine.strObservaciones.Substring(0, 1));
                }
                catch (Exception)
                {
                    strReturn = "En las OTs de trabajos para otras delegaciones, lo primero que debe aparecer en las observaciones debe ser la OT de la delegacion de origen;";
                    return (strReturn);
                }
            }
            if (string.IsNullOrEmpty(dataToInsertLine.strPreslin))
            {
                datosLinea.Idpreslin = null;
            }
            else
            {
                if (dataToInsertLine.strPreslin.Equals("-1"))
                {
                    datosLinea.Idpreslin = null;
                }
                else
                {
                    datosLinea.Idpreslin = Convert.ToInt32(dataToInsertLine.strPreslin);
                }
            }
            datosLinea.Idot = Convert.ToInt32(dataToInsertLine.strOt);
            datosLinea.Dietas = dGastos;
            datosLinea.Km = dKilometros;
            datosLinea.Observaciones = dataToInsertLine.strObservaciones.ToUpper();
            datosLinea.Horasviaje = TiempoViaje;
            datosLinea.Inicio = dtInicio;
            datosLinea.Fin = dtFin;
            datosLinea.Horas = Convert.ToSingle((dtFin - dtInicio).TotalHours) - TiempoViaje;
            datosLinea.Idusuario = _iUserId;
            datosLinea.Facturable = iPernoctacion;
            datosLinea.Npartefirmado = dataToInsertLine.strParte.ToUpper();
            datosLinea.CodEnt = _iUserCondEntO;
            datosLinea.Idoriginal = 0;
            datosLinea.Validador = string.Empty;
            datosLinea.Validado = 0;
            //inserccion segun codent...
            if (otSel.CodEnt == _iUserCondEntO)
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
                    strReturn = "Ha ocurrido un problema durante el proceso de guardar el parte de trabajo.;";
                    return (strReturn);
                }
            }
            else
            {
                var user = await aldakinDbContext.Usuarios.FirstOrDefaultAsync(x => x.Name == _stUserDni && x.CodEnt == Convert.ToInt16(dataToInsertLine.strEntidad));
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
                        var ot = await aldakinDbContext.Ots.FirstOrDefaultAsync(x => x.Cierre == null && x.Apertura.Year == datosLinea.Inicio.Year && x.CodEnt == _iUserCondEntO && x.CodEntD == aldakinDbContext.Lineas.FirstOrDefault(y => y.Idlinea == linea.Idlinea).CodEnt);
                        var iOt = ot.Idots;
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
                            CodEnt = _iUserCondEntO,//codigo entidad del usuario
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
                        strReturn = "Ha ocurrido un problema durante el proceso de guardar el parte de trabajo.;";
                        return (strReturn);
                    }
                }
                else
                {
                    strReturn = "¡¡¡No estas dado de alta en la delegacion para la que quieres registrar el parte!!!Informe a admisntracion de lo ocurrido;";
                    return (strReturn);
                }
            }
            strReturn = "Parte rellenado satisfactoriamente";
            return (strReturn);
        }
        public async Task<List<SelectData>> DeleteWorkerLineAsync(int iLine, int idAldakinUser)
        {
            WriteUserDataAsync(idAldakinUser);
            string strReturn;
            var oReturn = new List<SelectData>
            {
                new SelectData { iValue = 0 }
            };
            //var strReturn = string.Empty;
            //var iIdLinea = 0;
            //try
            //{
            //    iIdLinea = Convert.ToInt32(iLine);
            //}
            //catch (Exception)
            //{
            //    return oReturn;
            //}
            //if (iIdLinea == 0)
            //{
            //    return oReturn;
            //}
            //var lSelect = await aldakinDbContext.Lineas.FirstOrDefaultAsync(x => x.Idlinea == Convert.ToInt32(iIdLinea));
            //DateTime day;
            //if (lSelect is null)
            //{
            //    return oReturn;
            //}
            //day = lSelect.Inicio;
            //var lEstadoDia = await aldakinDbContext.Estadodias.Where(x => x.Idusuario == _iUserId && DateTime.Compare(x.Dia, day.Date) == 0).ToListAsync();//
            //if (lEstadoDia.Count > 0)
            //{
            //    return oReturn;
            //}
            var lSelect = await ReviewLineData(iLine, _iUserId);
            if (lSelect is null) return oReturn;
            var transaction = await aldakinDbContext.Database.BeginTransactionAsync();
            try
            {
                var linea = new Lineas();
                linea = lSelect;
                aldakinDbContext.Lineas.Remove(linea);
                var lineaSecundaria = await aldakinDbContext.Lineas.FirstOrDefaultAsync(x => x.Idoriginal == lSelect.Idlinea);
                if (!(lineaSecundaria is null))
                {
                    aldakinDbContext.Lineas.Remove(lineaSecundaria);
                }
                var gasto = await aldakinDbContext.Gastos.Where(x => x.Idlinea == lSelect.Idlinea).ToListAsync();
                if (!(gasto is null))
                {
                    aldakinDbContext.Gastos.RemoveRange(gasto);
                }
                await aldakinDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                strReturn = lSelect.Inicio.Year + "-" + lSelect.Inicio.Month + "-" + lSelect.Inicio.Day;
                return oReturn;
            }
            var strDay = string.Empty;
            var strMonth = string.Empty;
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
            oReturn.First().iValue = 1;
            oReturn.Add(new SelectData { iValue = 1, strText = "loadWeek", strValue = strReturn });
            return oReturn;
        }
        public async Task<SelectData> CloseWorkerWeekAsync(string strDataSelected, int idAldakinUser)
        {
            WriteUserDataAsync(idAldakinUser);
            var oReturn = new SelectData
            {
                iValue = 0,
                strText = string.Empty,
                strValue = string.Empty
            };
            DateTime dtSelected;
            try
            {
                dtSelected = Convert.ToDateTime(strDataSelected);
            }
            catch (Exception)
            {
                oReturn.iValue = 0;
                oReturn.strText = "Proceso abortado, error en los datos;";
                return oReturn;
            }
            var lEstadoDia = await aldakinDbContext.Estadodias.Where(x => x.Idusuario == _iUserId && DateTime.Compare(x.Dia, dtSelected) == 0).ToListAsync();//
            if (lEstadoDia.Count > 0)
            {
                oReturn.iValue = 0;
                oReturn.strText = "La semana esta cerrada, habla con tu responsable si es necesario abrirla;";
                return oReturn;
            }
            DateTime dtIniWeek;
            DateTime dtEndWeek;
            var day = dtSelected.DayOfWeek;
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
                    oReturn.iValue = 0;
                    oReturn.strText = "Error en la seleccion de dia!!!";
                    return oReturn;
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
            var ldHorasdia = new List<double>();
            var lListEstados = new List<Estadodias>();
            var listPartes = new List<Lineas>();
            for (var date = dtIniWeek; date < dtEndWeek; date = date.AddDays(1.0))
            {
                double dHorasDia = 0;
                listPartes = await aldakinDbContext.Lineas.Where(x => x.Inicio.Date == date.Date && x.CodEnt == _iUserCondEntO && x.Idusuario == _iUserId).OrderBy(x => x.Inicio).ToListAsync();
                foreach (var l in listPartes)
                {
                    dHorasDia = dHorasDia + (l.Fin - l.Inicio).TotalHours;
                }
                ldHorasdia.Add(dHorasDia);
                lListEstados.Add(new Estadodias { Dia = date.Date, Idusuario = _iUserId, Estado = 2, Horas = (float)(dHorasDia) });//
            }
            var transaction = await aldakinDbContext.Database.BeginTransactionAsync();
            try
            {
                aldakinDbContext.Estadodias.AddRange(lListEstados);
                await aldakinDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                oReturn.iValue = 0;
                oReturn.strText = "Ha ocurrido un problema durante el proceso de cerrar semana.;";
                return oReturn;
            }

            var strReturn = dtSelected.Year + "-" + dtSelected.Month + "-" + dtSelected.Day;
            oReturn.iValue = 1;
            oReturn.strText = "Semana Cerrada satisfactoriamente;";
            oReturn.strValue = dtSelected.ToString();
            return oReturn;
        }
        public async Task<SelectData> EditWorkerLineAsync(WorkerLineData dataToEditLine, int idAldakinUser)
        {
            WriteUserDataAsync(idAldakinUser);
            var oReturn = new SelectData
            {
                iValue = 0,
                strText = string.Empty,
                strValue = string.Empty
            };
            var strReturn = string.Empty;
            var bHorasViajeTemp = false;
            var gGastosTemp = false;
            var iPernoctacion = 0;
            DateTime day, dtInicio, dtFin;
            var datosLinea = await aldakinDbContext.Lineas.FirstOrDefaultAsync(x => x.Idlinea == Convert.ToInt32(dataToEditLine.strIdLinea));
            try
            {
                if (string.IsNullOrEmpty(dataToEditLine.strObservaciones))
                {
                    dataToEditLine.strObservaciones = string.Empty;
                }

                if (string.IsNullOrEmpty(dataToEditLine.strParte))
                {
                    dataToEditLine.strParte = string.Empty;
                }

                if (string.IsNullOrEmpty(dataToEditLine.bHorasViaje))
                {
                    bHorasViajeTemp = false;
                }
                else
                {
                    bHorasViajeTemp = true;
                }
                if (string.IsNullOrEmpty(dataToEditLine.bGastos))
                {
                    gGastosTemp = false;
                }
                else
                {
                    gGastosTemp = true;
                    dataToEditLine.strHoraInicio = "00";
                    dataToEditLine.strMinutoInicio = "00";
                    dataToEditLine.strHoraFin = "00";
                    dataToEditLine.strMinutoFin = "00";
                }
                if (string.IsNullOrEmpty(dataToEditLine.strPernoctacion))
                {
                    iPernoctacion = 0;
                }
                else
                {
                    iPernoctacion = Convert.ToInt32(dataToEditLine.strPernoctacion);
                }
                day = Convert.ToDateTime(dataToEditLine.strCalendario);
                dtInicio = Convert.ToDateTime(dataToEditLine.strCalendario + " " + dataToEditLine.strHoraInicio + ":" + dataToEditLine.strMinutoInicio + ":00");
                dtFin = Convert.ToDateTime(dataToEditLine.strCalendario + " " + dataToEditLine.strHoraFin + ":" + dataToEditLine.strMinutoFin + ":00");
            }
            catch (Exception)
            {
                oReturn.strText = "Se ha producido un error en el procesamiento de los datos;";
                oReturn.iValue = 0;
                return oReturn;
            }
            //Estado dia
            //"select estado from estadodias where idusuario = {0} and date(dia) = '{1}'", user.idUsuario, dia.ToString("yyyy-MM-dd")
            day = Convert.ToDateTime(dataToEditLine.strCalendario);
            dtInicio = Convert.ToDateTime(dataToEditLine.strCalendario + " " + dataToEditLine.strHoraInicio + ":" + dataToEditLine.strMinutoInicio + ":00");
            dtFin = Convert.ToDateTime(dataToEditLine.strCalendario + " " + dataToEditLine.strHoraFin + ":" + dataToEditLine.strMinutoFin + ":00");
            var lEstadoDia = await aldakinDbContext.Estadodias.Where(x => x.Idusuario == _iUserId && DateTime.Compare(x.Dia, day) == 0).ToListAsync();//
            if (lEstadoDia.Count > 0)
            {
                strReturn = datosLinea.Inicio.Year + "-" + datosLinea.Inicio.Month + "-" + datosLinea.Inicio.Day;
                oReturn.strText = "La semana esta cerrada, habla con tu responsable para reabirla;";
                oReturn.iValue = 1;
                oReturn.strValue = strReturn;
                return oReturn;
            }
            if (DateTime.Compare(dtInicio, dtFin) > 0)
            {
                strReturn = datosLinea.Inicio.Year + "-" + datosLinea.Inicio.Month + "-" + datosLinea.Inicio.Day;
                oReturn.strText = "Hora de Fin de Parte anterior a la Hora de inicio de Parte;";
                oReturn.iValue = 1;
                oReturn.strValue = strReturn;
                return oReturn;
            }
            //Rango usado
            var lLineas = await aldakinDbContext.Lineas.Where(x => DateTime.Compare(x.Inicio.Date, day.Date) == 0 && x.Idusuario == _iUserId && x.Validado == 0 && x.Registrado == 0 && x.Idlinea != datosLinea.Idlinea && x.Idoriginal != datosLinea.Idlinea).ToListAsync();
            if (RangeIsUsed(lLineas, dtFin, dtInicio, ref strReturn))
            {
                strReturn = datosLinea.Inicio.Year + "-" + datosLinea.Inicio.Month + "-" + datosLinea.Inicio.Day;
                oReturn.strText = "Rango de Horas ya utilizado;";
                oReturn.iValue = 1;
                oReturn.strValue = strReturn;
                return oReturn;
            }
            //
            var dHorasTrabajadas = (dtFin - dtInicio).TotalHours;
            if ((dHorasTrabajadas == 0) && !gGastosTemp)
            {
                strReturn = datosLinea.Inicio.Year + "-" + datosLinea.Inicio.Month + "-" + datosLinea.Inicio.Day;
                oReturn.strText = "error 24h trabajadas y no puesto solo gastos;";
                oReturn.iValue = 1;
                oReturn.strValue = strReturn;
                return oReturn;
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
            var codEntOt = await aldakinDbContext.Ots.FirstOrDefaultAsync(t => t.Idots == Convert.ToInt32(dataToEditLine.strOt));
            var iCodEntOt = codEntOt.CodEnt;
            var lGastos = new List<Gastos>();
            if (!(string.IsNullOrEmpty(dataToEditLine.strGastos)))
            {
                string line;
                string[] substring;
                var strReader = new StringReader(dataToEditLine.strGastos);
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
                                var pagador = await aldakinDbContext.Tipogastos.FirstOrDefaultAsync(x => x.CodEnt == iCodEntOt && string.Equals(x.Tipo, substring[2]) && x.Pagador == Convert.ToInt32(substring[1]));
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
                                strReturn = datosLinea.Inicio.Year + "-" + datosLinea.Inicio.Month + "-" + datosLinea.Inicio.Day;
                                oReturn.strText = "Los gastos son erroneos, repita el parte;";
                                oReturn.iValue = 1;
                                oReturn.strValue = strReturn;
                                return oReturn;
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
            var otSel = await aldakinDbContext.Ots.FirstOrDefaultAsync(x => x.Idots == Convert.ToInt32(dataToEditLine.strOt) && x.Codigorefot != "29" && x.Cierre == null);
            if (otSel is null)
            {
                oReturn.strText = "En la Ot que esta usande se ha encontrado un problema, recargue la pagina;";
                oReturn.iValue = 0;
                oReturn.strValue = string.Empty;
                return oReturn;
            }

            if (otSel.Nombre.Length > 10 && otSel.Nombre.Substring(0, 20) == "TRABAJOS REALIZADOS ")
            {
                try
                {
                    var otoriginal = Convert.ToInt32(dataToEditLine.strObservaciones.Substring(0, 1));
                }
                catch (Exception)
                {
                    strReturn = datosLinea.Inicio.Year + "-" + datosLinea.Inicio.Month + "-" + datosLinea.Inicio.Day;
                    oReturn.strText = "En las OTs de trabajos para otras delegaciones, lo primero que debe aparecer en las observaciones debe ser la OT de la delegacion de origen;";
                    oReturn.iValue = 1;
                    oReturn.strValue = strReturn;
                    return oReturn;
                }
            }
            if (otSel.CodEnt == _iUserCondEntO)
            {
                datosLinea.Dietas = dGastos;
                datosLinea.Km = dKilometros;
                datosLinea.Observaciones = dataToEditLine.strObservaciones.ToUpper();
                datosLinea.Horasviaje = TiempoViaje;
                datosLinea.Inicio = dtInicio;
                datosLinea.Fin = dtFin;
                datosLinea.Horas = Convert.ToSingle((dtFin - dtInicio).TotalHours) - TiempoViaje;
                datosLinea.Facturable = iPernoctacion;
                datosLinea.Npartefirmado = dataToEditLine.strParte.ToUpper();
                datosLinea.Idoriginal = 0;
                datosLinea.Validador = string.Empty;
                datosLinea.Validado = 0;
                var transaction = await aldakinDbContext.Database.BeginTransactionAsync();
                try
                {
                    aldakinDbContext.Lineas.Update(datosLinea);
                    await aldakinDbContext.SaveChangesAsync();
                    var gasto = await aldakinDbContext.Gastos.Where(x => x.Idlinea == datosLinea.Idlinea).ToListAsync();
                    if (!(gasto is null))
                    {
                        aldakinDbContext.Gastos.RemoveRange(gasto);
                    }

                    foreach (var g in lGastos)
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
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    strReturn = datosLinea.Inicio.Year + "-" + datosLinea.Inicio.Month + "-" + datosLinea.Inicio.Day;
                    oReturn.strText = "Ha ocurrido un problema durante el proceso de guardar el parte de trabajo.;";
                    oReturn.iValue = 1;
                    oReturn.strValue = strReturn;
                    return oReturn;
                }
            }
            else
            {
                var user = await aldakinDbContext.Usuarios.FirstOrDefaultAsync(x => x.Name == _stUserDni && x.CodEnt == otSel.CodEnt);
                if (!(user is null))
                {
                    datosLinea.Dietas = dGastos;
                    datosLinea.Km = dKilometros;
                    datosLinea.Observaciones = dataToEditLine.strObservaciones.ToUpper();
                    datosLinea.Horasviaje = TiempoViaje;
                    datosLinea.Inicio = dtInicio;
                    datosLinea.Fin = dtFin;
                    datosLinea.Horas = Convert.ToSingle((dtFin - dtInicio).TotalHours) - TiempoViaje;
                    datosLinea.Facturable = iPernoctacion;
                    datosLinea.Npartefirmado = dataToEditLine.strParte.ToUpper();
                    datosLinea.Idoriginal = 0;
                    datosLinea.Validador = string.Empty;
                    datosLinea.Validado = 0;
                    var transaction = await aldakinDbContext.Database.BeginTransactionAsync();
                    try
                    {
                        aldakinDbContext.Lineas.Update(datosLinea);
                        await aldakinDbContext.SaveChangesAsync();
                        var gasto = await aldakinDbContext.Gastos.Where(x => x.Idlinea == datosLinea.Idlinea).ToListAsync();
                        if (!(gasto is null))
                        {
                            aldakinDbContext.Gastos.RemoveRange(gasto);
                        }

                        foreach (var g in lGastos)
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
                        var Cero = Convert.ToChar("0");
                        Primerdigito = otSel.Numero.ToString().Substring(0, 1);
                        Resto = otSel.Numero.ToString().Substring(2, otSel.Numero.ToString().Length - 2);
                        Resto = Resto.TrimStart(Cero);
                        Salida = string.Format("{0}|{1}", Primerdigito, Resto);
                        var observaciones = Salida + " " + datosLinea.Observaciones.ToUpper();

                        var secundarioAntigua = await aldakinDbContext.Lineas.FirstOrDefaultAsync(x => x.Idoriginal == datosLinea.Idlinea);
                        aldakinDbContext.Lineas.Remove(secundarioAntigua);
                        //from ots where cierre is null and year(apertura) = year(iinicio) and  cod_ent = icod_ent and cod_ent_d = (select cod_ent from lineas where idlinea = iidoriginal)
                        var ot = await aldakinDbContext.Ots.FirstOrDefaultAsync(x => x.Cierre == null && x.Apertura.Year == datosLinea.Inicio.Year && x.CodEnt == _iUserCondEntO && x.CodEntD == aldakinDbContext.Lineas.FirstOrDefault(y => y.Idlinea == datosLinea.Idlinea).CodEnt);
                        var iOt = ot.Idots;
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
                            CodEnt = _iUserCondEntO,//codigo entidad del usuario
                            Idoriginal = datosLinea.Idlinea,
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
                        strReturn = datosLinea.Inicio.Year + "-" + datosLinea.Inicio.Month + "-" + datosLinea.Inicio.Day;
                        oReturn.strText = "Ha ocurrido un problema durante el proceso de guardar el parte de trabajo.;";
                        oReturn.iValue = 1;
                        oReturn.strValue = strReturn;
                        return oReturn;
                    }
                }
                else
                {
                    strReturn = datosLinea.Inicio.Year + "-" + datosLinea.Inicio.Month + "-" + datosLinea.Inicio.Day;
                    oReturn.strText = "¡¡¡No estas dado de alta en la delegacion para la que quieres registrar el parte!!!Informe a admisntracion de lo ocurrido;";
                    oReturn.iValue = 1;
                    oReturn.strValue = strReturn;
                    return oReturn;
                }
            }
            strReturn = datosLinea.Inicio.Year + "-" + datosLinea.Inicio.Month + "-" + datosLinea.Inicio.Day;
            oReturn.strText = "Parte editado Satisfactoriamente;";
            oReturn.iValue = 1;
            oReturn.strValue = strReturn;
            return oReturn;
        }
        public async Task<bool> ReadUserMessageAsync(int iIdMessage)
        {
            bool bReturn = false;
            var message = await aldakinDbContext.Mensajes.FirstOrDefaultAsync(x => x.Idmensajes == iIdMessage);
            var transaction = await aldakinDbContext.Database.BeginTransactionAsync();
            try
            {
                message.Estado = false;
                message.Vistodestino = DateTime.Now;
                aldakinDbContext.Mensajes.Update(message);
                await aldakinDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                bReturn = true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                bReturn = true;
            }
            return bReturn;
        }
        public async Task<string> AnswerMessageAsync(LineMessage line)
        {
            string strReturn;
            try
            {
                var message = new Mensajes
                {
                    De = line.De,
                    A = line.A,
                    Fecha = DateTime.Now,
                    Asunto = line.Asunto,
                    Mensaje = line.Mensaje,
                    Vistoremite = DateTime.Now,
                    Idlinea = line.Idlinea,
                    Inicial = line.Inicial,
                    Estado = true
                };
                var transaction = await aldakinDbContext.Database.BeginTransactionAsync();
                try
                {
                    aldakinDbContext.Mensajes.Add(message);
                    await aldakinDbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    strReturn = "Ocurrio un Error al enviar el mensaje";
                }
                strReturn = "Mensaje enviado correctamente";
            }
            catch (Exception ex)
            {
                strReturn = "Ocurrio un Error con los datos del mensaje";
            }
            return strReturn;
        }
        public async Task<string> UpdateEntityDataOrCsvAsync(int iIdEntity, int idAldakinUser, string strAction = "AC")
        {
            string strReturn = string.Empty;
            WriteUserDataAsync(idAldakinUser);
            string strTemp = strAction + iIdEntity;
            try
            {
                if (_iUserLevel >= 3)
                {
                    var temp = await aldakinDbContext.Servicios.FirstOrDefaultAsync(x => x.CodEnt == iIdEntity && x.Condicion == strTemp);
                    if (!(temp is null))
                    {
                        temp.Ejecutar = 1;
                        var transaction = await aldakinDbContext.Database.BeginTransactionAsync();
                        try
                        {
                            aldakinDbContext.Servicios.Update(temp);
                            await aldakinDbContext.SaveChangesAsync();
                            await transaction.CommitAsync();
                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync();
                            strReturn = "Ha ocurrido un error al mandar actualizar los datos de la delegación";
                        }
                        strReturn = "La actualización durara aproximadamente 15 minutos.";
                    }
                    else
                    {
                        strReturn = "Ha ocurrido un error al mandar actualizar los datos de la delegación";
                    }
                }
                else
                {
                    strReturn = "No tiene permiso para ejecutar esta accion.";
                }
            }
            catch (Exception ex)
            {
                strReturn = "Ha ocurrido un error al mandar actualizar los datos de la delegación";
            }
            return strReturn;
        }
        private bool RangeIsUsed(List<Lineas> lLineas, DateTime dtFin, DateTime dtInicio, ref string strReturn)
        {
            bool bReturn;
            strReturn = string.Empty;
            bReturn = false;//true==error
            if (lLineas != null)
            {
                foreach (var x in lLineas)
                {
                    if (DateTime.Compare(dtFin, x.Inicio) > 0 && DateTime.Compare(dtFin, x.Fin) < 0)
                    {
                        bReturn = true;
                        strReturn = "Rango de Horas ya utilizado;";
                    }
                    if (DateTime.Compare(dtInicio, x.Inicio) > 0 && DateTime.Compare(dtInicio, x.Fin) < 0)
                    {
                        bReturn = true;
                        strReturn = "Rango de Horas ya utilizado;";
                    }
                }
            }
            return bReturn;
        }
        private async Task<Lineas> ReviewLineData(int iLine, int idAldakinUser)
        {
            var oReturn = new Lineas();
            var strReturn = string.Empty;
            var iIdLinea = 0;
            try
            {
                iIdLinea = Convert.ToInt32(iLine);
            }
            catch (Exception)
            {
                oReturn = null;
                return oReturn;
            }
            if (iIdLinea == 0)
            {
                oReturn = null;
                return oReturn;
            }
            oReturn = await aldakinDbContext.Lineas.FirstOrDefaultAsync(x => x.Idlinea == Convert.ToInt32(iIdLinea));
            DateTime day;
            if (oReturn is null)
            {
                oReturn = null;
                return oReturn;
            }
            day = oReturn.Inicio;
            var lEstadoDia = await aldakinDbContext.Estadodias.Where(x => x.Idusuario == _iUserId && DateTime.Compare(x.Dia, day.Date) == 0).ToListAsync();//
            if (lEstadoDia.Count > 0)
            {
                oReturn = null;
                return oReturn;
            }
            return oReturn;
        }
    }

}
