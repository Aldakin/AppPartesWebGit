using AppPartes.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppPartes.Logic
{
    public class LoadIndexController : ILoadIndexController
    {
        private readonly AldakinDbContext aldakinDbContext;
        private readonly IWriteDataBase _iWriteDataBase;
        private readonly IWorkPartInformation _iWorkPartInformation;

        //apaño para usuario con claims
        private string _strUserName = "";
        private string _stUserDni = "";
        private int _iUserId;
        private int _iUserCondEntO;
        private int _iUserLevel;
        private bool _bError;
        public LoadIndexController(AldakinDbContext aldakinDbContext, IWriteDataBase iWriteDataBase, IWorkPartInformation iworkPartInformation)
        {
            this.aldakinDbContext = aldakinDbContext;
            _iWriteDataBase = iWriteDataBase;
            _iWorkPartInformation = iworkPartInformation;
        }
        private async void WriteUserDataAsync(int idAldakinUser)
        {
            var user = await _iWriteDataBase.GetUserDataAsync(idAldakinUser);
            if (!(user is null))
            {
                _strUserName = user.strUserName;
                _iUserId = user.iUserId;
                _iUserCondEntO = user.iUserCondEntO;
                _stUserDni = user.stUserrDni;
                _iUserLevel = user.iLevel;
                _bError = false;
            }
            else
            {
                _bError = true;
            }
        }
        private UserData WriteUserData()
        {
            var oReturn = new UserData
            {
                strUserName = _strUserName,
                iUserId = _iUserId,
                iUserCondEntO = _iUserCondEntO,
                stUserrDni = _stUserDni,
                iLevel = _iUserLevel
            };
            return oReturn;
        }
        private async Task<List<Entidad>> GetAldakinCompaniesAndRunningAsync(string strAction = "AC")
        {
            var lReturn = new List<Entidad>();
            //if (_bError) return null;
            var aux = await aldakinDbContext.Entidad.FirstOrDefaultAsync(x => x.CodEnt == _iUserCondEntO);
            var lTemp = await aldakinDbContext.Entidad.Where(x => x.CodEnt != _iUserCondEntO).OrderByDescending(x => x.Nombre).ToListAsync();
            lTemp.Insert(0, aux);
            foreach (Entidad e in lTemp)
            {
                string strTemp = strAction + e.CodEnt;
                var temp = await aldakinDbContext.Servicios.FirstOrDefaultAsync(x => x.CodEnt == e.CodEnt && x.Condicion == strTemp);
                if (temp.Ejecutar == 1)
                {
                    lReturn.Add(new Entidad
                    {
                        CodEnt = e.CodEnt,
                        Nombre = e.Nombre + "(Running)"
                    });
                }
                else
                {
                    lReturn.Add(new Entidad
                    {
                        CodEnt = e.CodEnt,
                        Nombre = e.Nombre + "(NotRunning)"
                    });
                }
            }
            return lReturn;
        }
        public async Task<HomeDataViewLogic> LoadHomeControllerAsync(int idAldakinUser)
        {
            var oReturn = new HomeDataViewLogic();
            try
            {
                WriteUserDataAsync(idAldakinUser);
                oReturn.user = WriteUserData();
                oReturn.listCompanyUpdate = await GetAldakinCompaniesAndRunningAsync("AC");
                oReturn.listCompanyCsv = await GetAldakinCompaniesAndRunningAsync("CS");
                oReturn.strError = string.Empty;
            }
            catch (Exception ex)
            {
                oReturn.strError = "Error durnate la carga de la pagina";
            }
            return oReturn;
        }
        public async Task<MainDataViewLogic> LoadMainControllerAsync(int idAldakinUser)
        {
            var oReturn = new MainDataViewLogic();
            try
            {
                WriteUserDataAsync(idAldakinUser);
                oReturn.listOts = await GetOtsAsync();
                oReturn.listCompany = await GetAldakinCompaniesAsync();
                oReturn.listClient = await GetAldakinClientsAsync();
                oReturn.listNight = await GetAldakinNightAsync();
                oReturn.bMessage = await PendingMessageAsync();
                oReturn.strError = string.Empty;
            }
            catch (Exception ex)
            {
                oReturn.strError = "Error durnate la carga de la pagina";
            }
            return oReturn;
        }
        public async Task<SearchPendingViewLogic> SearchPendingControllerAsync(int idAldakinUser)
        {
            var oReturn = new SearchPendingViewLogic();
            try
            {
                WriteUserDataAsync(idAldakinUser);
                oReturn.listCompany = await GetAldakinCompaniesAsync();
                oReturn.strError = string.Empty;
            }
            catch (Exception ex)
            {
                oReturn.strError = "Error durnate la carga de la pagina";
            }
            return oReturn;
        }
        public async Task<WeekDataViewLogic> LoadWeekControllerAsync(int idAldakinUser, string strDate = "", string strAction = "", string strId = "")
        {
            var oReturn = new WeekDataViewLogic();
            try
            {
                WriteUserDataAsync(idAldakinUser);
                DateTime day, dtIniWeek = DateTime.Now, dtEndWeek = DateTime.Now;
                oReturn.bMessage = await PendingMessageAsync();
                if (string.IsNullOrEmpty(strDate) && string.IsNullOrEmpty(strAction))
                {
                    //oReturn.Mensaje = "";
                }
                else
                {
                    if ((string.IsNullOrEmpty(strId)))
                    {
                        strId = "0";
                    }
                    var lSelect = await aldakinDbContext.Lineas.FirstOrDefaultAsync(x => x.Idlinea == Convert.ToInt32(strId));
                    List<Estadodias> lEstadoDia;
                    switch (strAction)
                    {
                        case "loadWeek":
                            try
                            {
                                var dtSelected = Convert.ToDateTime(strDate);
                                WorkPartInformation.IniEndWeek(dtSelected, out dtIniWeek, out dtEndWeek);
                                oReturn.listSemana = await ResumeHourPerDayAsync(dtIniWeek, dtEndWeek);
                                oReturn.listPartes = await GetWeekWorkerPartsAsync(dtIniWeek, dtEndWeek);
                                var weekStatus = await aldakinDbContext.Estadodias.FirstOrDefaultAsync(x => x.Dia == dtSelected.Date && x.Idusuario == _iUserId);
                                if (weekStatus is null)
                                {
                                    oReturn.SemanaCerrada = false;
                                }
                                else
                                {
                                    oReturn.SemanaCerrada = true;
                                }
                                oReturn.DateSelected = dtSelected.ToString("yyyy-MM-dd"); // dtSelected.Date;
                            }
                            catch (Exception ex)
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
                            lEstadoDia = await aldakinDbContext.Estadodias.Where(x => x.Idusuario == _iUserId && DateTime.Compare(x.Dia, day.Date) == 0).ToListAsync();//
                            if (lEstadoDia.Count > 0)
                            {
                                oReturn.Mensaje = "La semana esta cerrada, habla con tu responsable para reabirla;";
                            }
                            oReturn.listSelect = await GetDayWorkerPartAsync(lSelect);
                            oReturn.listPernocta = await GetAldakinNightAsync(lSelect);
                            oReturn.DateSelected = lSelect.Inicio.Date.ToString("yyyy-MM-dd"); // dtSelected.Date;
                            break;
                        default:
                            //return View();
                            break;
                    }
                }
                oReturn.strError = string.Empty;
            }
            catch (Exception ex)
            {
                oReturn.strError = "Error durnate la carga de la pagina";
            }
            return oReturn;
        }
        public async Task<LoginDataViewLogic> LoadLoginControllerAsync()
        {
            var oReturn = new LoginDataViewLogic();
            try
            {
                oReturn.listCompany = await GetAllAldakinCompaniesAsync();
                oReturn.strError = string.Empty;
            }
            catch (Exception ex)
            {
                oReturn.strError = "Error durnate la carga de la pagina";
            }
            return oReturn;
        }
        public async Task<MessageViewLogic> LoadMessageControllerAsync(int idAldakinUser, int idMessage)
        {
            var oReturn = new MessageViewLogic();
            try
            {
                WriteUserDataAsync(idAldakinUser);
                if (idMessage > 0)
                {
                    oReturn.oMessage = await GetMessageAsync(idMessage);
                }
                else
                {
                    oReturn.oMessage = null;
                }
                oReturn.listMessages = await GetAllMessagesAsync();
                oReturn.strError = string.Empty;
            }
            catch (Exception ex)
            {
                oReturn.strError = "Error durnate la carga de la pagina";
            }
            return oReturn;
        }
        public async Task<SearchViewLogic> LoadSearchControllerAsync(int idAldakinUser, string strDate, string strEntity)
        {
            var oReturn = new SearchViewLogic();
            try
            {
                WriteUserDataAsync(idAldakinUser);
                oReturn.listCompany = await GetAldakinCompaniesAsync();
                if ((!(string.IsNullOrEmpty(strDate))) && (!(string.IsNullOrEmpty(strEntity))))
                {
                    oReturn.listResume = await _iWorkPartInformation.StatusEntityAsync(idAldakinUser, strDate, strEntity);
                }
                oReturn.strError = string.Empty;
            }
            catch (Exception ex)
            {
                oReturn.strError = "Error durnate la carga de la pagina";
            }
            return oReturn;
        }

        private async Task<bool> PendingMessageAsync()
        {
            bool bReturn = true;
            var message = await aldakinDbContext.Mensajes.FirstOrDefaultAsync(x => x.A == _iUserId && x.Estado == true);
            if (message is null) bReturn = false;
            return bReturn;
        }
        private async Task<List<LineMessage>> GetAllMessagesAsync()
        {
            var lReturn = new List<LineMessage>();
            try
            {
                var lTemp = await aldakinDbContext.Mensajes.Where(x => x.A == _iUserId && x.Estado == true).OrderByDescending(x => x.Fecha).ToListAsync();
                foreach (Mensajes m in lTemp)
                {
                    var de = await aldakinDbContext.Usuarios.FirstOrDefaultAsync(x => x.Idusuario == m.De);
                    if ((m.Idlinea < 1) || (m.Idlinea is null))
                    {
                        m.Idlinea = 0;
                    }
                    if (string.IsNullOrEmpty(m.Asunto)) m.Asunto = string.Empty;
                    if (string.IsNullOrEmpty(m.Mensaje)) m.Mensaje = string.Empty;
                    lReturn.Add
                        (new LineMessage
                        {
                            Idmensajes = m.Idmensajes,
                            De = m.De,
                            strDe = de.Nombrecompleto,
                            A = m.A,
                            strA = _strUserName,
                            Fecha = m.Fecha,
                            Asunto = m.Asunto,
                            Mensaje = m.Mensaje,
                            Idlinea = m.Idlinea ?? 0,
                            Inicial = m.Inicial,
                            Estado = m.Estado
                        });
                }
            }
            catch (Exception ex)
            {
                lReturn = null;
            }
            return lReturn;
        }
        private async Task<LineMessage> GetMessageAsync(int idMessage)
        {
            try
            {
                var oTemp = await aldakinDbContext.Mensajes.FirstOrDefaultAsync(x => x.Idmensajes == idMessage && x.A == _iUserId);
                var strDe = await aldakinDbContext.Usuarios.FirstOrDefaultAsync(x => x.Idusuario == oTemp.De);

                var oReturn = new LineMessage
                {
                    Idmensajes = oTemp.Idmensajes,
                    De = oTemp.De,
                    strDe = strDe.Nombrecompleto,
                    A = oTemp.A,
                    strA = _strUserName,
                    Fecha = oTemp.Fecha,
                    Asunto = oTemp.Asunto,
                    Mensaje = oTemp.Mensaje,
                    Idlinea = oTemp.Idlinea ?? 0,
                    Inicial = oTemp.Inicial,
                    Estado = oTemp.Estado
                };
                if (!await _iWriteDataBase.ReadUserMessageAsync(idMessage)) oReturn = null;

                return oReturn;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private async Task<List<Ots>> GetOtsAsync()
        {
            var lReturn = new List<Ots>();
            var totalOts = aldakinDbContext.Ots.Where(x => x.CodEnt == _iUserCondEntO && x.CodEntD == 0 && x.Codigorefot != "29" && x.Cierre == null);
            var totalType1Ots = totalOts.Where(x => x.Tipoot == 1);
            var totalType2Ots = totalOts.Join(aldakinDbContext.Presupuestos.Where(x => x.CodEnt == _iUserCondEntO), o => o.Idots, i => i.Idot, (o, p) => o);//original
            lReturn = await totalType1Ots.Concat(totalType2Ots).Distinct().OrderBy(x => x.Numero).ToListAsync();
            return lReturn;
        }
        private async Task<List<Entidad>> GetAldakinCompaniesAsync()
        {
            var lReturn = new List<Entidad>();
            var aux = await aldakinDbContext.Entidad.FirstOrDefaultAsync(x => x.CodEnt == _iUserCondEntO);
            lReturn = await aldakinDbContext.Entidad.Where(x => x.CodEnt != _iUserCondEntO).OrderByDescending(x => x.Nombre).ToListAsync();
            lReturn.Insert(0, aux);
            return lReturn;
        }
        private async Task<List<Entidad>> GetAllAldakinCompaniesAsync()
        {
            var lReturn = new List<Entidad>();
            lReturn = await aldakinDbContext.Entidad.Where(x => x.CodEnt != 0).ToListAsync();
            return lReturn;
        }
        private async Task<List<Clientes>> GetAldakinClientsAsync()
        {
            var lReturn = new List<Clientes>();
            lReturn = await (from c in aldakinDbContext.Clientes
                             from o in aldakinDbContext.Ots
                             where (
                             (c.Idclientes == o.Cliente)
                             && (o.Cierre == null)
                             && (o.Codigorefot != "29")
                             && (c.CodEnt == _iUserCondEntO)
                             )
                             select c).Distinct().OrderBy(c => c.Nombre).ToListAsync();
            return lReturn;
        }
        private async Task<List<Pernoctaciones>> GetAldakinNightAsync()
        {
            var lReturn = new List<Pernoctaciones>();
            lReturn = await aldakinDbContext.Pernoctaciones.Where(x => x.CodEnt == _iUserCondEntO).ToListAsync();
            return lReturn;
        }
        private async Task<List<Pernoctaciones>> GetAldakinNightAsync(Lineas lSelect)
        {
            var lReturn = new List<Pernoctaciones>();
            lReturn = await aldakinDbContext.Pernoctaciones.Where(x => x.CodEnt == lSelect.CodEnt).ToListAsync();
            return lReturn;
        }
        private async Task<List<double>> ResumeHourPerDayAsync(DateTime dtIniWeek, DateTime dtEndWeek)
        {
            var lReturn = new List<double>();
            for (var date = dtIniWeek; date <= dtEndWeek; date = date.AddDays(1.0))
            {
                double dHorasDia = 0;
                var Temp = await aldakinDbContext.Lineas.Where(x => x.Inicio.Date == date.Date && x.CodEnt == _iUserCondEntO && x.Idusuario == _iUserId).OrderBy(x => x.Inicio).ToListAsync();
                foreach (var l in Temp)
                {
                    dHorasDia = dHorasDia + (l.Fin - l.Inicio).TotalHours;
                }
                lReturn.Add(dHorasDia);
            }
            return lReturn;
        }
        private async Task<List<List<LineaVisual>>> GetWeekWorkerPartsAsync(DateTime dtIniWeek, DateTime dtEndWeek)
        {
            var lReturn = new List<List<LineaVisual>>();
            var NombreOt = string.Empty;
            var lTemp = await aldakinDbContext.Lineas.Where(x => x.Inicio > dtIniWeek && x.Fin < dtEndWeek && x.Idusuario == _iUserId && x.CodEnt == _iUserCondEntO).OrderBy(x => x.Inicio).ToListAsync();
            lReturn = await CreateVisualWorkerPart(lTemp); // ver si es linea original y obteniendo nombres de ots y empresas            
            return lReturn;
        }
        private async Task<List<List<LineaVisual>>> CreateVisualWorkerPart(List<Lineas> lTemp)
        {
            var lReturn = new List<List<LineaVisual>>();
            var lSunday = new List<LineaVisual>();
            var lMonday = new List<LineaVisual>();
            var lTuesday = new List<LineaVisual>();
            var lWednesday = new List<LineaVisual>();
            var lThursday = new List<LineaVisual>();
            var lFriday = new List<LineaVisual>();
            var lSaturday = new List<LineaVisual>();

            string strObservaciones;
            var NombreOt = string.Empty;
            var NombreCliente = string.Empty;
            var strPernocta = string.Empty;
            var strPreslin = string.Empty;
            foreach (var l in lTemp)
            {
                var oTemp = new LineaVisual();
                oTemp = null;
                var nombre = await aldakinDbContext.Ots.FirstOrDefaultAsync(x => x.Idots == l.Idot);
                NombreOt ="["+ nombre.Numero + "] " + nombre.Nombre;
                var nombreCliente = await aldakinDbContext.Clientes.FirstOrDefaultAsync(x => x.Idclientes == aldakinDbContext.Ots.FirstOrDefault(o => o.Idots == l.Idot).Cliente);
                NombreCliente = nombreCliente.Nombre;
                if (l.Facturable == 0)
                {
                    strPernocta = "NO";
                }
                else
                {
                    var pernocta = await aldakinDbContext.Pernoctaciones.FirstOrDefaultAsync(x => x.Tipo == l.Facturable && x.CodEnt == _iUserCondEntO);
                    strPernocta = pernocta.Descripcion;
                }
                if (l.Idoriginal == 0)
                {
                    if (l.Observaciones.Length > 50)
                    {
                        strObservaciones = l.Observaciones.Substring(0, 45) + "...";
                    }
                    else
                    {
                        strObservaciones = l.Observaciones;
                    }
                    oTemp = (new LineaVisual
                    {
                        Idlinea = l.Idlinea,
                        Idot = l.Idot,
                        NombreOt = NombreOt,
                        NombreCliente = NombreCliente,
                        NombrePreslin = strPreslin,
                        Dietas = l.Dietas,
                        Km = l.Km,
                        Observaciones = strObservaciones,
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
                    var lineaOriginal = await aldakinDbContext.Lineas.FirstOrDefaultAsync(x => x.Idlinea == l.Idoriginal);
                    oTemp = (new LineaVisual
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
                var i = Convert.ToInt32(l.Inicio.DayOfWeek) ;
                switch (i)
                {
                    case 0:
                        lSunday.Add(oTemp);
                        break;
                    case 1:
                        lMonday.Add(oTemp);
                        break;
                    case 2:
                        lTuesday.Add(oTemp);
                        break;
                    case 3:
                        lWednesday.Add(oTemp);
                        break;
                    case 4:
                        lThursday.Add(oTemp);
                        break;
                    case 5:
                        lFriday.Add(oTemp);
                        break;
                    case 6:
                        lSaturday.Add(oTemp);
                        break;
                    default:
                        break;
                }
            }
            lReturn.Add(lSunday);
            lReturn.Add(lMonday);
            lReturn.Add(lTuesday);
            lReturn.Add(lWednesday);
            lReturn.Add(lThursday);
            lReturn.Add(lFriday);
            lReturn.Add(lSaturday);
            return lReturn;
        }
        private async Task<List<LineaVisual>> GetDayWorkerPartAsync(Lineas lSelect)
        {
            var lReturn = new List<LineaVisual>();
            var NombreOt = string.Empty;
            var NombreCliente = string.Empty;
            var nombreOt = await aldakinDbContext.Ots.FirstOrDefaultAsync(x => x.Idots == lSelect.Idot);
            NombreOt = nombreOt.Nombre;
            var nombreCliente = await aldakinDbContext.Clientes.FirstOrDefaultAsync(x => x.Idclientes == aldakinDbContext.Ots.FirstOrDefault(o => o.Idots == lSelect.Idot).Cliente);
            NombreCliente = nombreCliente.Nombre;
            var lGastos = await aldakinDbContext.Gastos.Where(x => x.Idlinea == lSelect.Idlinea).ToListAsync();
            var iCont = 0;
            var strGastos = "";
            foreach (var g in lGastos)
            {
                var strPagador = "";
                var tipo = await aldakinDbContext.Tipogastos.FirstOrDefaultAsync(x => x.Idtipogastos == g.Tipo);
                var strTipo = tipo.Tipo;
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
            lReturn.Add(new LineaVisual
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
            return lReturn;
        }
    }
}
