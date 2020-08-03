using AppPartes.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
namespace AppPartes.Logic
{
    public class WorkPartInformation : IWorkPartInformation
    {
        private readonly AldakinDbContext aldakinDbContext;
        private readonly IWriteDataBase _iWriteDataBase;
        //apaño para usuario con claims
        private string _strUserName = "";
        private string _stUserDni = "";
        private int _iUserId = 0;
        private int _iUserCondEntO = 0;
        private int _iUserLevel = 0;
        public WorkPartInformation(AldakinDbContext aldakinDbContext, IWriteDataBase iWriteDataBase)
        {
            this.aldakinDbContext = aldakinDbContext;
            _iWriteDataBase = iWriteDataBase;
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
            }
            else
            {

            }
        }
        public async Task<List<SelectData>> WeekHourResume(DateTime dtSelected, int idAldakinUser)
        {
            var lReturn = new List<SelectData>();
            try
            {
                WriteUserDataAsync(idAldakinUser);
                var iCont = 0;
                IniEndWeek(dtSelected, out var dtIniWeek, out var dtEndWeek);
                #region list hours per day in a week
                for (var date = dtIniWeek; date <= dtEndWeek; date = date.AddDays(1.0))
                {
                    var strRangosHora = "";
                    var listPartes = await aldakinDbContext.Lineas.Where(x => x.Inicio.Date == date.Date && x.CodEnt == _iUserCondEntO && x.Idusuario == _iUserId).OrderBy(x => x.Inicio).ToListAsync();
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
                    lReturn.Add(new SelectData { iValue = iCont, strText = strRangosHora });
                    iCont++;
                }
                #endregion
            }
            catch (Exception ex)
            {
                lReturn.Clear();
                lReturn.Add(new SelectData { iValue = 0, strText = "Ha ocurrido un error,Recargue pagina", strValue = "Ha ocurrido un error,Recargue pagina" });
            }
            return lReturn;
        }
        public async Task<List<SelectData>> SelectedCompanyReadOt(int iEntidad, int idAldakinUser)
        {
            var lReturn = new List<SelectData>();
            try
            {
                WriteUserDataAsync(idAldakinUser);
                List<Ots> listOts = null;
                //listOts = aldakinDbContext.Ots.Where(x => x.CodEnt == cantidad && x.CodEntD == 0 && x.Codigorefot != "29" && x.Cierre == null).OrderByDescending(x => x.Idots).ToList();
                if (iEntidad < 1)
                {
                    throw new Exception();
                }
                var totalOts = aldakinDbContext.Ots.Where(x => x.CodEnt == iEntidad && x.CodEntD == 0 && x.Codigorefot != "29" && x.Cierre == null);
                var totalType1Ots = totalOts.Where(x => x.Tipoot == 1);
                var totalType2Ots = totalOts.Join(aldakinDbContext.Presupuestos.Where(x => x.CodEnt == iEntidad), o => o.Idots, i => i.Idot, (o, p) => o);//original
                listOts = await totalType1Ots.Concat(totalType2Ots).Distinct().OrderBy(x => x.Numero).ToListAsync();
                foreach (var p in listOts)
                {
                    var strTemp = p.Numero + "||" + p.Nombre;
                    lReturn.Add(new SelectData { iValue = p.Idots, strText = strTemp });
                }
            }
            catch (Exception ex)
            {
                lReturn.Clear();
                lReturn.Add(new SelectData { iValue = 0, strText = "Ha ocurrido un error,Recargue pagina", strValue = "Ha ocurrido un error,Recargue pagina" });
            }
            return lReturn;
        }
        public async Task<List<SelectData>> SelectedCompanyReadClient(int iEntidad, int idAldakinUser)
        {
            var lReturn = new List<SelectData>();
            try
            {
                WriteUserDataAsync(idAldakinUser);
                List<Clientes> listaClient = null;
                if (iEntidad < 1)
                {
                    throw new Exception();
                }
                listaClient = await (from c in aldakinDbContext.Clientes
                                     from o in aldakinDbContext.Ots
                                     where (
                                     (c.Idclientes == o.Cliente)
                                     && (o.Cierre == null)
                                     && (o.Codigorefot != "29")
                                     && (c.CodEnt == iEntidad)
                                     )
                                     select c).Distinct().OrderBy(c => c.Nombre).ToListAsync();
                //select distinct Clientes.* from Clientes, Ots where Clientes.idclientes = Ots.cliente and Ots.cierre IS NULL and Ots.codigorefot != 29 and Clientes.cod_ent = { 0}", cod_ent)
                foreach (var p in listaClient)
                {
                    var strTemp = p.Nombre;
                    lReturn.Add(new SelectData { iValue = p.Idclientes, strText = strTemp });
                }
            }
            catch (Exception ex)
            {
                lReturn.Clear();
                lReturn.Add(new SelectData { iValue = 0, strText = "Ha ocurrido un error,Recargue pagina", strValue = "Ha ocurrido un error,Recargue pagina" });
            }
            return lReturn;
        }
        public async Task<List<SelectData>> SelectedClient(int iClient, int idAldakinUser)
        {
            var lReturn = new List<SelectData>();
            try
            {
                WriteUserDataAsync(idAldakinUser);
                List<Ots> listOts = null;
                if (iClient != 0)
                {
                    listOts = await aldakinDbContext.Ots.Where(x => x.Cierre == null && x.Cliente == iClient && x.Codigorefot != "29" && x.CodEntD != -1).OrderByDescending(x => x.Idots).ToListAsync();
                    foreach (var p in listOts)
                    {
                        var strTemp = p.Numero + "||" + p.Nombre;
                        lReturn.Add(new SelectData { iValue = p.Idots, strText = strTemp });
                    }
                }
                else
                {
                    listOts = null;
                    listOts = await aldakinDbContext.Ots.Where(x => x.CodEnt == _iUserCondEntO && x.CodEntD == 0 && x.Codigorefot != "29" && x.Cierre == null).OrderByDescending(x => x.Idots).ToListAsync();
                    foreach (var p in listOts)
                    {
                        var strTemp = p.Numero + "||" + p.Nombre;
                        lReturn.Add(new SelectData { iValue = p.Idots, strText = strTemp });
                    }

                }
            }
            catch (Exception ex)
            {
                lReturn.Clear();
                lReturn.Add(new SelectData { iValue = 0, strText = "Ha ocurrido un error,Recargue pagina", strValue = "Ha ocurrido un error,Recargue pagina" });
            }
            return lReturn;
        }
        public async Task<List<SelectData>> SelectedOt(int iOt, int idAldakinUser)
        {
            var lReturn = new List<SelectData>();
            try
            {
                WriteUserDataAsync(idAldakinUser);
                List<Presupuestos> lPresupuestos = null;
                lPresupuestos = null;
                if (iOt > 0)
                {
                    var lTempOts = await aldakinDbContext.Ots.Where(x => x.Idots == iOt).OrderByDescending(x => x.Idots).ToListAsync();
                    if (lTempOts[0].Tipoot == 1)
                    {
                        lPresupuestos = null;
                    }
                    else if (lTempOts[0].Tipoot == 2)
                    {
                        lPresupuestos = await aldakinDbContext.Presupuestos.Where(x => x.Idot == iOt).ToListAsync();
                    }
                    if (lPresupuestos == null)
                    {
                        lReturn = null;
                    }
                    else
                    {
                        foreach (var p in lPresupuestos)
                        {
                            var strTemp = p.Numero + "||" + p.Nombre;
                            lReturn.Add(new SelectData { iValue = p.Idpresupuestos, strText = strTemp });

                        }
                    }
                }
                else
                {
                    lReturn = null;
                }
            }
            catch (Exception ex)
            {
                lReturn.Clear();
                lReturn.Add(new SelectData { iValue = 0, strText = "Ha ocurrido un error,Recargue pagina", strValue = "Ha ocurrido un error,Recargue pagina" });
            }
            return lReturn;
        }
        public async Task<List<SelectData>> ReadLevel1(int iData, int idAldakinUser)
        {
            var lReturn = new List<SelectData>();
            try
            {
                WriteUserDataAsync(idAldakinUser);
                List<Preslin> lPreslin = null;
                if (iData < 1)
                {
                    throw new Exception();
                }
                lPreslin = await aldakinDbContext.Preslin.Where(x => x.Idpresupuesto == iData && x.Horas != 0 && x.Nivel == 1).ToListAsync();
                if (lPreslin == null)
                {
                    lReturn = null;
                }
                else
                {
                    foreach (var p in lPreslin)
                    {
                        var strTemp = p.Nombre;
                        lReturn.Add(new SelectData { iValue = p.Idpreslin, strText = strTemp });
                    }
                }
            }
            catch (Exception ex)
            {
                lReturn.Clear();
                lReturn.Add(new SelectData { iValue = 0, strText = "Ha ocurrido un error,Recargue pagina", strValue = "Ha ocurrido un error,Recargue pagina" });
            }
            return lReturn;
        }
        public async Task<List<SelectData>> ReadLevel2(int iData, int iData2, int idAldakinUser)
        {
            var lReturn = new List<SelectData>();
            try
            {
                WriteUserDataAsync(idAldakinUser);
                List<Preslin> lPreslin = null;
                if (iData < 1 || iData2 < 1)
                {
                    throw new Exception();
                }
                lPreslin = await aldakinDbContext.Preslin.Where(x => x.Idpresupuesto == iData2 && x.Horas != 0 && x.Nivel == 1).ToListAsync();
                //List<Preslin> lNivelTemp = lPreslin.Where(x => x.Idpreslin == cantidad).ToList();
                var lNivelTemp = lPreslin.FirstOrDefault(x => x.Idpreslin == iData);
                if (!(lNivelTemp is null))
                {
                    lPreslin = await aldakinDbContext.Preslin.Where(x => x.Horas != 0 && x.Idpresupuesto == lNivelTemp.Idpresupuesto && x.CodpPes == lNivelTemp.CodhPes && x.Version == lNivelTemp.Version && x.Anexo == lNivelTemp.Anexo).ToListAsync();
                }
                else
                {
                    lPreslin = null;
                }
                //cmd = new MySqlCommand(String.Format("SELECT idpreslin, idpresupuesto, codh_pes, codp_pes, nivel,nombre, horas, version, anexo FROM preslin where horas != 0 and idpresupuesto ={0} and codp_pes = {1} and version = {2} and anexo = {3} ORDER BY nombre", pres.idPresupuesto, pres.codh_pes, pres.Version, pres.Anexo), conexionBD);//ORDER BY codh_pes
                if (lPreslin == null)
                {
                    lReturn = null;
                }
                else
                {
                    foreach (var p in lPreslin)
                    {
                        var strTemp = p.Nombre;
                        lReturn.Add(new SelectData { iValue = p.Idpreslin, strText = strTemp });
                    }
                }
            }
            catch (Exception ex)
            {
                lReturn.Clear();
                lReturn.Add(new SelectData { iValue = 0, strText = "Ha ocurrido un error,Recargue pagina", strValue = "Ha ocurrido un error,Recargue pagina" });
            }
            return lReturn;
        }
        public async Task<List<SelectData>> ReadLevelGeneral(int iData, int idAldakinUser)
        {
            var lReturn = new List<SelectData>();
            try
            {
                WriteUserDataAsync(idAldakinUser);
                List<Preslin> lPreslin = null;
                if (iData < 1)
                {
                    throw new Exception();
                }
                var lNivelTemp = await aldakinDbContext.Preslin.FirstOrDefaultAsync(x => x.Horas != 0 && x.Idpreslin == iData);
                if (lNivelTemp is null)
                {
                    throw new Exception();
                }
                else
                {
                    lPreslin = await aldakinDbContext.Preslin.Where(x => x.Horas != 0 && x.Idpresupuesto == lNivelTemp.Idpresupuesto && x.CodpPes == lNivelTemp.CodhPes && x.Version == lNivelTemp.Version && x.Anexo == lNivelTemp.Anexo).ToListAsync();
                    if (lPreslin == null)
                    {
                        lReturn = null;
                    }
                    else
                    {
                        foreach (var p in lPreslin)
                        {
                            var strTemp = p.Nombre;
                            lReturn.Add(new SelectData { iValue = p.Idpreslin, strText = strTemp });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lReturn.Clear();
                lReturn.Add(new SelectData { iValue = 0, strText = "Ha ocurrido un error,Recargue pagina", strValue = "Ha ocurrido un error,Recargue pagina" });
            }
            return lReturn;
        }
        public async Task<List<SelectData>> SelectedPayerAsync(int iPayer, int iOt, int idAldakinUser)
        {
            var lReturn = new List<SelectData>();
            try
            {
                WriteUserDataAsync(idAldakinUser);
                if (iOt < 1)
                {
                    throw new Exception();
                }

                var tipoGasto = await aldakinDbContext.Tipogastos.Where(x => x.Pagador == iPayer && x.CodEnt == aldakinDbContext.Ots.FirstOrDefault(o => o.Idots == iOt).CodEnt).ToListAsync();
                if (!(tipoGasto == null))
                {
                    foreach (var p in tipoGasto)
                    {
                        var strTemp = p.Tipo.ToUpper();
                        lReturn.Add(new SelectData { strValue = strTemp, strText = strTemp });
                    }
                }
            }
            catch (Exception ex)
            {
                lReturn.Clear();
                lReturn.Add(new SelectData { iValue = 0, strText = "Ha ocurrido un error,Recargue pagina", strValue = "Ha ocurrido un error,Recargue pagina" });
            }
            return lReturn;
        }
        public async Task<List<string>> PendingWorkPartApiAsync(string strCalendario, string strUser, string strEntity)
        {
            var lReturn = new List<string>();
            DateTime dtCalendario = Convert.ToDateTime(strCalendario);
            int iUser = Convert.ToInt32(strUser);
            int iEntity = Convert.ToInt32(strEntity);
            if (iUser == 0)
            {
                var user = await _iWriteDataBase.GetAllUsersAsync(iEntity);
                foreach (Usuarios u in user)
                {
                    var temp = await PendingMonthWorkPartAsync(u.Idusuario, dtCalendario);
                    foreach (string s in temp)
                    {
                        lReturn.Add(s);
                    }
                }
            }
            else
            {
                var temp1 = await PendingMonthWorkPartAsync(iUser, dtCalendario);
                foreach (string s in temp1)
                {
                    lReturn.Add(s);
                }
            }
            return lReturn;
        }
        private async Task<List<string>> PendingMonthWorkPartAsync(int iIdUser, DateTime dtSelected)
        {
            var lReturn = new List<string>();
            DateTime dtIniMonth = Convert.ToDateTime("01/" + dtSelected.Month + "/" + dtSelected.Year);
            DateTime dtIniWeek, dtEndWeek;
            var userTemp = await aldakinDbContext.Usuarios.FirstOrDefaultAsync(x => x.Idusuario == iIdUser);
            if (!(userTemp is null))
            {
                for (var date = dtIniMonth; date <= dtSelected; date = date.AddDays(1.0))
                {
                    IniEndWeek(date, out dtIniWeek, out dtEndWeek);
                    if (date.Day == 1)
                    {
                        dtIniWeek = date;
                    }
                    double dHourWork = 0;
                    double dHourTravel = 0;
                    bool bClose = false;
                    var lineTemp = await aldakinDbContext.Lineas.Where(x => x.Inicio >= dtIniWeek && x.Fin <= dtEndWeek && x.Idusuario == iIdUser).ToListAsync();
                    foreach (Lineas l in lineTemp)
                    {
                        dHourWork = dHourWork + l.Horas;
                        dHourTravel = dHourTravel + l.Horasviaje ?? 0;
                    }
                    var close = await aldakinDbContext.Estadodias.FirstOrDefaultAsync(x => x.Dia == dtIniWeek && x.Idusuario == iIdUser);
                    if (!(close is null))
                    {
                        bClose = true;
                    }
                    else
                    {
                        bClose = false;
                    }
                    lReturn.Add(userTemp.Nombrecompleto + "(" + dtIniWeek.Day + "/" + dtIniWeek.Month + "/" + dtIniWeek.Year + ") Cerrada: " + bClose + " Horas: " + dHourWork + " Horas viaje: " + dHourTravel);
                    date = dtEndWeek.AddDays(1);
                }
            }
            else
            {
                lReturn.Add("Usuario: " + iIdUser + " no encontrado");
            }
            return lReturn;
        }
        public async Task<List<SelectData>> GetWorkerValidationAsnc(int idAldakinUser, int iEntity)
        {
            List<SelectData> lReturn = new List<SelectData>();
            List<Usuarios> lValidationUser = new List<Usuarios>();
            WriteUserDataAsync(idAldakinUser);
            var user = await aldakinDbContext.Usuarios.FirstOrDefaultAsync(x => x.Idusuario == _iUserId && x.CodEnt == x.CodEntO);
            lValidationUser = null;
            if (user.Autorizacion < 5)
            {
                var userValidation = await aldakinDbContext.Responsables.FirstOrDefaultAsync(x => x.Name == user.Name);
                List<string> split = userValidation.PersonasName.Split(new Char[] { '|' }).Distinct().ToList();
                foreach (string s in split)
                {
                    lValidationUser.Add(await aldakinDbContext.Usuarios.FirstOrDefaultAsync(x => x.Name == s && x.CodEnt == x.CodEntO && x.Baja == 0));
                }
            }
            else
            {
                lValidationUser = await aldakinDbContext.Usuarios.Where(x => x.CodEnt == iEntity && x.CodEnt == x.CodEntO && x.Baja == 0).ToListAsync();
            }
            foreach (Usuarios u in lValidationUser)
            {
                lReturn.Add(new SelectData
                {
                    iValue = u.Idusuario,
                    strText = u.Nombrecompleto
                });
            }
            return lReturn;
        }
        public async Task<List<SelectData>> GetOtValidationAsync(int idAldakinUser, int iEntity)
        {
            List<SelectData> lReturn = new List<SelectData>();
            List<Ots> lValidationOT = new List<Ots>();
            WriteUserDataAsync(idAldakinUser);
            var user = await aldakinDbContext.Usuarios.FirstOrDefaultAsync(x => x.Idusuario == _iUserId && x.CodEnt == x.CodEntO);
            lValidationOT = null;
            if (user.Autorizacion < 5)
            {
                var userValidation = await aldakinDbContext.Responsables.FirstOrDefaultAsync(x => x.Name == user.Name);
                List<string> split = userValidation.Ots.Split(new Char[] { '|' }).Distinct().ToList();
                foreach (string s in split)
                {
                    lValidationOT.Add(await aldakinDbContext.Ots.FirstOrDefaultAsync(x => x.Idots == Convert.ToInt32(s)));

                }
            }
            else
            {
                lValidationOT = await aldakinDbContext.Ots.Where(x => x.CodEnt == iEntity && x.Cierre == null).ToListAsync();
            }
            foreach (Ots o in lValidationOT)
            {
                lReturn.Add(new SelectData
                {
                    iValue = o.Idots,
                    strText = o.Numero + " " + o.Nombre
                });
            }
            return lReturn;
        }
        public async Task<List<ViewMounthResume>> StatusEntityResumeAsync(int idAldakinUser, string strCalendario, string strEntity)
        {
            var lReturn = new List<ViewMounthResume>();
            int iStatus = 0;
            DateTime dtIni, dtEnd;
            List<Usuarios> lValidationUser = new List<Usuarios>();
            DateTime dtSelected = Convert.ToDateTime(strCalendario);
            dtIni = new DateTime(dtSelected.Year, dtSelected.Month, 1);
            dtEnd = new DateTime(dtSelected.Year, dtSelected.Month + 1, 1).AddDays(-1);
            int iEntity = Convert.ToInt32(strEntity);
            WriteUserDataAsync(idAldakinUser);
            var user = await aldakinDbContext.Usuarios.FirstOrDefaultAsync(x => x.Idusuario == _iUserId && x.CodEnt == x.CodEntO);
            lValidationUser = null;
            if (user.Autorizacion < 5)
            {
                var userValidation = await aldakinDbContext.Responsables.FirstOrDefaultAsync(x => x.Name == user.Name);
                List<string> split = userValidation.PersonasName.Split(new Char[] { '|' }).Distinct().ToList();
                foreach (string s in split)
                {
                    lValidationUser.Add(await aldakinDbContext.Usuarios.FirstOrDefaultAsync(x => x.Name == s && x.CodEnt == x.CodEntO && x.Baja == 0));
                }
            }
            else
            {
                lValidationUser = await aldakinDbContext.Usuarios.Where(x => x.CodEnt == iEntity && x.CodEnt == x.CodEntO && x.Baja == 0).ToListAsync();
            }
            foreach (Usuarios u in lValidationUser)
            {
                ViewMounthResume oTemp = new ViewMounthResume();
                //oTemp.lHour = new List<double>();
                oTemp.lDay = new List<int>();
                oTemp.User = "[" + u.Nombrecompleto + "]";
                oTemp.dayStatus = new List<SearchDay>();
                var partMonth = await aldakinDbContext.Lineas.Where(x => x.Inicio.Day >= dtIni.Day && x.Inicio.Month >= dtIni.Month && x.Inicio.Year >= dtIni.Year && x.Fin.Day <= dtEnd.Day && x.Fin.Month <= dtEnd.Month && x.Fin.Year <= dtEnd.Year && x.Idusuario == u.Idusuario && x.CodEnt==u.CodEnt).ToListAsync();

                for (var date = dtIni; date <= dtEnd; date = date.AddDays(1.0))
                {
                    SearchDay oSeach = new SearchDay();
                    double dHour = 0.0;
                    int iValidated = 0, iGenerated = 0;
                    //var partDay = await aldakinDbContext.Lineas.Where(x => x.Inicio.Day == date.Day && x.Inicio.Month == date.Month && x.Inicio.Year == date.Year && x.Idusuario == u.Idusuario).ToListAsync();
                    var partDay = (from p in partMonth
                                   where (p.Inicio.Day == date.Day)
                                   && (p.Inicio.Month == date.Month)
                                   && (p.Inicio.Year == date.Year)select p).ToList();
                    //result = (from p in lUser
                    //          where (p.Dni.Contains(txtDniBuscar.Text)
                    //          && (p.Nombre.Contains(txtNombreBuscar.Text.ToUpper()) || (p.Apellidos.Contains(txtNombreBuscar.Text.ToUpper())))
                    //          && p.Email.Contains(txtEmailBuscar.Text)
                    //           )
                    //          select p).ToList();

                    foreach (Lineas l in partDay)
                    {
                        dHour = dHour + l.Horas;
                        if (l.Validado == 1) iValidated++;
                        if (l.Registrado == 1) iGenerated++;
                    }
                    var status = await aldakinDbContext.Estadodias.FirstOrDefaultAsync(x => x.Dia.Day == date.Day && x.Dia.Month == date.Month && x.Dia.Year == date.Year && x.Idusuario == u.Idusuario);
                    //bool status = false;
                    if(status is null)
                    {
                        iStatus = 0;//no cerrada
                    }
                    else
                    {
                        if(status.Estado==2)
                        {
                            iStatus = 2;//cerrada
                        }
                        else
                        {
                            iStatus = 4;//quien sabe
                           // hay que ver como poner bien el mostrar los estados de los dias, cerrados, validaods, etc
                        }
                    }
                    oSeach.hour = dHour;
                    oSeach.colour = DayStatusColour(partDay.Count, iGenerated, iValidated, date);
                    oTemp.lDay.Add(date.Day);
                    oTemp.dayStatus.Add(oSeach);
                }
                lReturn.Add(oTemp);
            }
            return lReturn;
        }
        public async Task<string>StringWeekResumeAsync(int idAldakinUser, string strDate, string strOt, string strWorker, string strEntity)
        {
            string strReturn =  string.Empty;
            var lTemp = new List<Lineas>();
            DateTime dtIniWeek, dtEndWeek;
            DateTime dtSelected = Convert.ToDateTime(strDate);
            IniEndWeek(dtSelected, out dtIniWeek, out dtEndWeek);
            try
            {
                int iOt = Convert.ToInt32(strOt);
                int iWorker = Convert.ToInt32(strWorker);
                int iEntity = Convert.ToInt32(strEntity);
                if ((iWorker > 0) && (iOt > 0))
                {
                    //seleccionado trabajador y ot
                    lTemp = await aldakinDbContext.Lineas.Where(x => x.Inicio > dtIniWeek && x.Fin < dtEndWeek && x.Idusuario == iWorker && x.CodEnt == iEntity && x.Idot == iOt && x.Validado==0 &&x.Registrado==0).OrderBy(x => x.Inicio).ToListAsync();
                }
                else
                {
                    if ((iWorker > 0) && (iOt == 0))
                    {
                        //seleccionado trabajador y no ot
                        lTemp = await aldakinDbContext.Lineas.Where(x => x.Inicio > dtIniWeek && x.Fin < dtEndWeek && x.Idusuario == iWorker && x.CodEnt == iEntity && x.Validado == 0 && x.Registrado == 0).OrderBy(x => x.Inicio).ToListAsync();//&& x.CodEnt == _iUserCondEntO
                    }
                    else
                    {
                        if ((iWorker == 0) && (iOt > 0))
                        {
                            // no seleccionado trabajador y seleccionado ot
                            lTemp = await aldakinDbContext.Lineas.Where(x => x.Inicio > dtIniWeek && x.Fin < dtEndWeek && x.CodEnt == iEntity && x.Idot == iOt && x.Validado == 0 && x.Registrado == 0).OrderBy(x => x.Inicio).ToListAsync();
                        }
                        else
                        {
                            //caso que no deberia suceder
                            lTemp = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lTemp = null;
            }
            if (!(lTemp is null))
            {
                foreach (Lineas l in lTemp)
                {
                    strReturn = l.Idlinea + "|" + strReturn;
                }
            }
            return strReturn;
        }
        public async Task<List<List<LineaVisual>>> StatusWeekResumeAsync(int idAldakinUser, string strDate, string strOt, string strWorker, string strEntity)
        {
            var lReturn = new List<List<LineaVisual>>();
            var lTemp = new List<Lineas>();
            DateTime dtIniWeek, dtEndWeek;
            lReturn = null;
            DateTime dtSelected = Convert.ToDateTime(strDate);
            IniEndWeek(dtSelected, out dtIniWeek, out dtEndWeek);
            try
            {
                int iOt = Convert.ToInt32(strOt);
                int iWorker = Convert.ToInt32(strWorker);
                int iEntity = Convert.ToInt32(strEntity);
                if ((iWorker > 0) && (iOt > 0))
                {
                    //seleccionado trabajador y ot
                    lTemp = await aldakinDbContext.Lineas.Where(x => x.Inicio > dtIniWeek && x.Fin < dtEndWeek && x.Idusuario == iWorker && x.CodEnt == iEntity && x.Idot == iOt).OrderBy(x => x.Inicio).ToListAsync();
                }
                else
                {
                    if ((iWorker > 0) && (iOt == 0))
                    {
                        //seleccionado trabajador y no ot
                        lTemp = await aldakinDbContext.Lineas.Where(x => x.Inicio > dtIniWeek && x.Fin < dtEndWeek && x.Idusuario == iWorker && x.CodEnt == iEntity).OrderBy(x => x.Inicio).ToListAsync();//&& x.CodEnt == _iUserCondEntO
                    }
                    else
                    {
                        if ((iWorker == 0) && (iOt > 0))
                        {
                            // no seleccionado trabajador y seleccionado ot
                            lTemp = await aldakinDbContext.Lineas.Where(x => x.Inicio > dtIniWeek && x.Fin < dtEndWeek && x.CodEnt == iEntity && x.Idot == iOt).OrderBy(x => x.Inicio).ToListAsync();
                        }
                        else
                        {
                            //caso que no deberia suceder
                            lTemp = null;
                        }
                    }
                }

                if (!(lTemp is null))
                {
                    lReturn = await _iWriteDataBase.CreateVisualWorkerPartAsync(lTemp); // ver si es linea original y obteniendo nombres de ots y empresas  
                }
            }
            catch (Exception ex)
            {
                lReturn = null;
            }
            return lReturn;
        }
        private string DayStatusColour(int iNumPart, int iGenerated, int iValidated, DateTime dtDay)
        {
            string strReturn = "#FFFFFF";
            string strAllGenerated = "#D2691E";
            string strAllValidated = "#B0C4DE";
            string strEmpty = "#FFFFFF";
            string strWeekend = "#D3D3D3";
            string strHalfValidated = "#FF8C00";
            if (iNumPart == 0)
            {
                if (((Convert.ToInt32(dtDay.DayOfWeek) == 0)) || ((Convert.ToInt32(dtDay.DayOfWeek) == 6)))
                {
                    strReturn = strWeekend;
                }
                else
                {
                    strReturn = strEmpty;
                }
            }
            else
            {
                if (iNumPart == iGenerated)
                {
                    //todo el dia volcado #D2691E
                    strReturn = strAllGenerated;
                }
                else
                {
                    if ((iNumPart == iValidated))
                    {
                        //no todo el dia generado todo el dia validado #B0C4DE
                        strReturn = strAllValidated;
                    }
                    else
                    {
                        if ((iValidated == 0))
                        {
                            //ni generado no validado
                            //blanco #FFFFFF o gris #D3D3D3
                            if (((Convert.ToInt32(dtDay.DayOfWeek) == 0)) || ((Convert.ToInt32(dtDay.DayOfWeek) == 6)))
                            {
                                strReturn = strWeekend;
                            }
                            else
                            {
                                strReturn = strEmpty;
                            }
                        }
                        else
                        {
                            if ((iValidated > 0))
                            {
                                //no generado algunos partes validados
                                //naranja   #FF8C00
                                strReturn = strHalfValidated;
                            }
                            else
                            {
                                if (((Convert.ToInt32(dtDay.DayOfWeek) == 0)) || ((Convert.ToInt32(dtDay.DayOfWeek) == 6)))
                                {
                                    strReturn = strWeekend;
                                }
                                else
                                {
                                    strReturn = strEmpty;
                                }
                            }
                        }
                    }
                }
            }
            return strReturn;

        }
        private async Task<List<Usuarios>> UserPendingWorkPartAsync(DateTime dtSelected, int iEntity)
        {
            var lReturn = new List<Usuarios>();
            var userTemp = await aldakinDbContext.Usuarios.Where(x => x.CodEnt == iEntity && x.Baja == 0).ToListAsync();
            if (!(userTemp is null))
            {
                foreach (Usuarios u in userTemp)
                {
                    DateTime dtIniMonth = Convert.ToDateTime("01/" + dtSelected.Month + "/" + dtSelected.Year);
                    DateTime dtIniWeek, dtEndWeek;
                    for (var date = dtIniMonth; date <= dtSelected; date = date.AddDays(1.0))
                    {
                        IniEndWeek(date, out dtIniWeek, out dtEndWeek);
                        if (date.Day == 1)
                        {
                            dtIniWeek = date;
                        }
                        var close = await aldakinDbContext.Estadodias.FirstOrDefaultAsync(x => x.Dia == dtIniWeek && x.Idusuario == u.Idusuario);
                        if (!(close is null))
                        {
                            lReturn.Add(u);
                        }
                        else
                        {
                            //no actions
                        }
                        date = dtEndWeek.AddDays(1);
                        break;
                    }
                }
            }
            return lReturn;
        }
        public async Task<bool> SendAdvicePendingWorkPart(string strCalendario, string strUser, string strEntity)
        {
            bool bReturn = false;
            string strSubject = "Partes Pendientes";
            string strTo = "";
            string strSender = "";
            string strText = "Buenos dias, /r/n Este email se envia desde la aplicacion de partes de la empresa /r/n Revise y pongase al día en sus partes de trabajo. /r/n Un saludo;";
            DateTime dtSelected = Convert.ToDateTime(strCalendario);
            int iEntity = Convert.ToInt32(strEntity);
            var users = await UserPendingWorkPartAsync(dtSelected, iEntity);
            foreach (Usuarios u in users)
            {
                strTo = u.Email;
                strSender = "aplicacion@aldakin.com";
                SendEmail(strSubject, strTo, strSender, strText);
            }
            return bReturn;
        }
        private bool SendEmail(string strSubject, string strTo, string strSender, string strText)
        {
            //meter estos daatos en secrets
            string Email = "aplicacion@aldakin.com";
            string Pass = "ALD2015apli";
            string Host = "smtp.aldakin.com";
            //estos datos a secrets
            bool bReturn = false;
            string sTempMensaje = "";
            MailMessage msg = new MailMessage();
            msg.To.Add(strTo);
            msg.Subject = strSubject;
            msg.SubjectEncoding = System.Text.Encoding.UTF8;
            msg.Bcc.Add(strSender);

            sTempMensaje = strText.Replace("\r\n", " <br>");
            msg.Body = sTempMensaje;
            msg.BodyEncoding = System.Text.Encoding.UTF8;
            msg.IsBodyHtml = true;
            msg.From = new MailAddress(Email);

            SmtpClient cliente = new SmtpClient();
            cliente.Credentials = new System.Net.NetworkCredential(Email, Pass);
            cliente.Port = 587;
            //cliente.EnableSsl = true;
            cliente.Host = Host; //mail.domiio.com
            try
            {
                cliente.Send(msg);
                bReturn = true;
            }
            catch (Exception ex)
            {
                bReturn = false;
            }
            return bReturn;
        }
        public static void IniEndWeek(DateTime dtSelected, out DateTime dtIniWeek, out DateTime dtEndWeek)
        {
            var dayWeek = dtSelected.DayOfWeek;
            switch (dayWeek)
            {
                case System.DayOfWeek.Sunday:
                    dtIniWeek = dtSelected.AddDays(-6);
                    dtEndWeek = dtSelected.AddDays(0);// (+1);
                    break;
                case System.DayOfWeek.Saturday:
                    dtIniWeek = dtSelected.AddDays(-5);
                    dtEndWeek = dtSelected.AddDays(+1);// (+2);
                    break;
                case System.DayOfWeek.Friday:
                    dtIniWeek = dtSelected.AddDays(-4);
                    dtEndWeek = dtSelected.AddDays(+2); //(+3);
                    break;
                case System.DayOfWeek.Thursday:
                    dtIniWeek = dtSelected.AddDays(-3);
                    dtEndWeek = dtSelected.AddDays(+3); //(+4);
                    break;
                case System.DayOfWeek.Wednesday:
                    dtIniWeek = dtSelected.AddDays(-2);
                    dtEndWeek = dtSelected.AddDays(+4); //(+5);
                    break;
                case System.DayOfWeek.Tuesday:
                    dtIniWeek = dtSelected.AddDays(-1);
                    dtEndWeek = dtSelected.AddDays(+5); //(+6);
                    break;
                case System.DayOfWeek.Monday:
                    dtIniWeek = dtSelected.AddDays(0);
                    dtEndWeek = dtSelected.AddDays(+6); //(+7);
                    break;
                default:
                    throw new Exception();
            }
            DateTime s = dtEndWeek;
            TimeSpan ts = new TimeSpan(23, 59, 0);
            dtEndWeek = s.Date + ts;
        }
    }
}
