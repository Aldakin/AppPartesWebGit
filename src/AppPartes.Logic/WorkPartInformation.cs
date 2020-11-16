using AppPartes.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Runtime.Serialization.Formatters;
using System.Security.Cryptography.X509Certificates;
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
                lReturn.Add(new SelectData { iValue = 0, strText = "Ha ocurrido un error,Pruebe a seleccionar de nuevo la ot ", strValue = "Ha ocurrido un error,Pruebe a seleccionar de nuevo la ot" });
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
                    lReturn = await PendingMonthWorkPartAsync(u.Idusuario, dtCalendario);

                }
            }
            else
            {
                lReturn = await PendingMonthWorkPartAsync(iUser, dtCalendario);
            }
            return lReturn;
        }
        public async Task<List<SelectData>> GetWorkerValidationAsnc(int idAldakinUser, int iEntity)
        {
            List<SelectData> lReturn = new List<SelectData>();
            var lValidationUser = new List<Usuarios>();
            var uTemp = new Usuarios();
            //WriteUserDataAsync(idAldakinUser);
            //if (_iUserLevel < 5)
            //{
            //    var userValidation = await aldakinDbContext.Responsables.FirstOrDefaultAsync(x => x.Name == _stUserDni);
            //    List<string> split = userValidation.PersonasName.Split(new Char[] { '|' }).Distinct().ToList();
            //    foreach (string s in split)
            //    {
            //        uTemp = await aldakinDbContext.Usuarios.FirstOrDefaultAsync(x => x.Name == s && x.CodEnt == x.CodEntO && x.Baja == 0);
            //        lValidationUser.Add(uTemp);
            //        //lValidationUser.Insert(0, uTemp);
            //    }
            //}
            //else
            //{
            //    lValidationUser = await aldakinDbContext.Usuarios.Where(x => x.CodEnt == iEntity && x.CodEnt == x.CodEntO && x.Baja == 0).OrderBy(x=>x.Nombrecompleto).ToListAsync();
            //}
            lValidationUser = await ListValidationUsersAsync(idAldakinUser, iEntity);
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
            //WriteUserDataAsync(idAldakinUser);
            //if (_iUserLevel < 5)
            //{
            //    var userValidation = await aldakinDbContext.Responsables.FirstOrDefaultAsync(x => x.Name == _stUserDni);
            //    List<string> split = userValidation.Ots.Split(new Char[] { '|' }).Distinct().ToList();
            //    foreach (string s in split)
            //    {
            //        lValidationOT.Add(await aldakinDbContext.Ots.FirstOrDefaultAsync(x => x.Idots == Convert.ToInt32(s)));

            //    }
            //}
            //else
            //{
            //    lValidationOT = await aldakinDbContext.Ots.Where(x => x.CodEnt == iEntity && x.Cierre == null).ToListAsync();
            //}
            lValidationOT = await ListValidationOtsAsync(idAldakinUser, iEntity);
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
        public async Task<List<Usuarios>> ListValidationUsersAsync(int idAldakinUser, int iEntity = 0)
        {
            var lValidationUser = new List<Usuarios>();
            var uTemp = new Usuarios();
            WriteUserDataAsync(idAldakinUser);
            var user = await aldakinDbContext.Usuarios.FirstOrDefaultAsync(x => x.Idusuario == idAldakinUser);
            //lValidationUser = null;
            if (_iUserLevel < 5)
            {
                var userValidation = await aldakinDbContext.Responsables.FirstOrDefaultAsync(x => x.Name == user.Name);
                if (!(userValidation is null))
                {
                    List<string> split = userValidation.PersonasName.Split(new Char[] { '|' }).Distinct().ToList();
                    foreach (string s in split)
                    {
                        uTemp = await aldakinDbContext.Usuarios.FirstOrDefaultAsync(x => x.Name == s && x.CodEnt == x.CodEntO && x.Baja == 0);
                        lValidationUser.Add(uTemp);
                        //lValidationUser.Insert(0, uTemp);
                    }
                }
            }
            else
            {
                if (iEntity > 0)
                {
                    lValidationUser = await aldakinDbContext.Usuarios.Where(x => x.CodEnt == iEntity && x.CodEnt == x.CodEntO && x.Baja == 0).OrderBy(x => x.Nombrecompleto).ToListAsync();
                }
                else
                {
                    //si no hay entidad seleccinada la lista vuelve vacia (no null)
                }
            }
            return lValidationUser;
        }
        public async Task<List<Ots>> ListValidationOtsAsync(int idAldakinUser, int iEntity = 0)
        {
            List<Ots> lValidationOT = new List<Ots>();
            WriteUserDataAsync(idAldakinUser);
            var user = await aldakinDbContext.Usuarios.FirstOrDefaultAsync(x => x.Idusuario == idAldakinUser);
            //lValidationOT = null;
            if (_iUserLevel < 5)
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
                if (iEntity > 0)
                {
                    lValidationOT = await aldakinDbContext.Ots.Where(x => x.CodEnt == iEntity && x.Cierre == null).ToListAsync();
                }
                else
                {
                    //si no hay entidad seleccinada la lista vuelve vacia (no null)
                }
            }
            return lValidationOT;
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
            //lValidationUser = null;
            if (_iUserLevel < 5)
            {
                var userValidation = await aldakinDbContext.Responsables.FirstOrDefaultAsync(x => x.Name == _stUserDni);
                List<string> split = userValidation.PersonasName.Split(new Char[] { '|' }).Distinct().ToList();
                foreach (string s in split)
                {
                    lValidationUser.Add(await aldakinDbContext.Usuarios.FirstOrDefaultAsync(x => x.Name == s && x.CodEnt == x.CodEntO && x.Baja == 0));
                }
            }
            else
            {
                lValidationUser = await aldakinDbContext.Usuarios.Where(x => x.CodEnt == iEntity && x.CodEnt == x.CodEntO && x.Baja == 0).OrderBy(x => x.Nombrecompleto).ToListAsync();
            }
            var totalPartMonth = await aldakinDbContext.Lineas.Where(x => x.Inicio.Day >= dtIni.Day && x.Inicio.Month >= dtIni.Month && x.Inicio.Year >= dtIni.Year && x.Fin.Day <= dtEnd.Day && x.Fin.Month <= dtEnd.Month && x.Fin.Year <= dtEnd.Year && x.CodEnt == iEntity).ToListAsync();

            foreach (Usuarios u in lValidationUser)
            {
                ViewMounthResume oTemp = new ViewMounthResume();
                oTemp.iMoroso = 0;
                //oTemp.lHour = new List<double>();
                oTemp.lDay = new List<string>();
                oTemp.User = "[" + u.Nombrecompleto + "]";
                oTemp.dayStatus = new List<SearchDay>();
                var partMonth = (from p in totalPartMonth
                                 where (p.Idusuario == u.Idusuario)
                                 select p).ToList();

                //////////var xx = partMonth.GroupBy(x => x.Inicio.Date);
                ////////////SearchDay oSearch = new SearchDay();
                //////////for (var date = dtIni; date <= dtEnd; date = date.AddDays(1.0))
                //////////{
                //////////    SearchDay oSearch = new SearchDay();
                //////////    oSearch.hour = 0;
                //////////    oSearch.colour = await DayStatusColour(0, 0, 0, date, 0, u.CodEnt);
                //////////    oSearch.day = date.Day;

                //////////    var strDay = string.Empty;
                //////////    switch (date.DayOfWeek)
                //////////    {
                //////////        case System.DayOfWeek.Sunday:
                //////////            strDay = "D:" + date.Day;
                //////////            break;
                //////////        case System.DayOfWeek.Monday:
                //////////            strDay = "L:" + date.Day;
                //////////            break;
                //////////        case System.DayOfWeek.Tuesday:
                //////////            strDay = "M:" + date.Day;
                //////////            break;
                //////////        case System.DayOfWeek.Wednesday:
                //////////            strDay = "X:" + date.Day;
                //////////            break;
                //////////        case System.DayOfWeek.Thursday:
                //////////            strDay = "J:" + date.Day;
                //////////            break;
                //////////        case System.DayOfWeek.Friday:
                //////////            strDay = "V:" + date.Day;
                //////////            break;
                //////////        case System.DayOfWeek.Saturday:
                //////////            strDay = "S:" + date.Day;
                //////////            break;
                //////////    }
                //////////    oTemp.lDay.Add(strDay);
                //////////    oTemp.dayStatus.Add(oSearch);
                //////////}

                //////////foreach (Lineas l in xx.ToList())
                //////////{
                //////////    SearchDay oSearch1 = new SearchDay();
                //////////    var lTemp = partMonth.Where(x => x.Inicio.Date == l.Inicio.Date).ToList();
                //////////    double dHour = 0.0;
                //////////    int iValidated = 0, iGenerated = 0;
                //////////    //var partDay = await aldakinDbContext.Lineas.Where(x => x.Inicio.Day == date.Day && x.Inicio.Month == date.Month && x.Inicio.Year == date.Year && x.Idusuario == u.Idusuario).ToListAsync();

                //////////    dHour = lTemp.Sum(x => x.Horas);
                //////////    iValidated = Convert.ToInt32(lTemp.Sum(x => x.Validado));
                //////////    iGenerated = Convert.ToInt32(lTemp.Sum(x => x.Registrado));
                //////////    //foreach (Lineas l in partDay)
                //////////    //{
                //////////    //    dHour = dHour + l.Horas;
                //////////    //    if (l.Validado == 1) iValidated++;
                //////////    //    if (l.Registrado == 1) iGenerated++;
                //////////    //}
                //////////    var status = await aldakinDbContext.Estadodias.FirstOrDefaultAsync(x => x.Dia.Day == l.Inicio.Day && x.Dia.Month == l.Inicio.Month && x.Dia.Year == l.Inicio.Year && x.Idusuario == u.Idusuario);
                //////////    //bool status = false;
                //////////    if (status is null)
                //////////    {
                //////////        iStatus = 0;//no cerrada blanco
                //////////    }
                //////////    else
                //////////    {
                //////////        if (status.Estado == 2)
                //////////        {
                //////////            iStatus = 2;//cerrada amarillo
                //////////        }
                //////////        else
                //////////        {
                //////////            iStatus = 4;//generada azul
                //////////        }
                //////////    }
                //////////    oSearch1.day = l.Inicio.Day;
                //////////    oSearch1.hour = dHour;
                //////////    oSearch1.colour = await DayStatusColour(lTemp.Count, iGenerated, iValidated, l.Inicio, iStatus, u.CodEnt);
                //////////    oTemp.dayStatus.RemoveAll(x => x.day == l.Inicio.Day);
                //////////    oTemp.dayStatus.Add(oSearch1);
                //////////}



                //var partMonth = await aldakinDbContext.Lineas.Where(x => x.Inicio.Day >= dtIni.Day && x.Inicio.Month >= dtIni.Month && x.Inicio.Year >= dtIni.Year && x.Fin.Day <= dtEnd.Day && x.Fin.Month <= dtEnd.Month && x.Fin.Year <= dtEnd.Year && x.Idusuario == u.Idusuario && x.CodEnt == u.CodEnt).ToListAsync();
                //dtEnd = partMonth.Max(x => x.Fin);
                //DateTime s = dtEnd;
                //TimeSpan ts = new TimeSpan(00, 00, 0);
                //dtEnd = s.Date + ts;
                for (var date = dtIni; date <= dtEnd; date = date.AddDays(1.0))
                {
                    SearchDay oSeach = new SearchDay();
                    double dHour = 0.0;
                    int iValidated = 0, iGenerated = 0;
                    //var partDay = await aldakinDbContext.Lineas.Where(x => x.Inicio.Day == date.Day && x.Inicio.Month == date.Month && x.Inicio.Year == date.Year && x.Idusuario == u.Idusuario).ToListAsync();
                    var partDay = (from p in partMonth
                                   where (p.Inicio.Day == date.Day)
                                   && (p.Inicio.Month == date.Month)
                                   && (p.Inicio.Year == date.Year)
                                   select p).ToList();

                    dHour = partDay.Sum(x => x.Horas);
                    iValidated = Convert.ToInt32(partDay.Sum(x => x.Validado));
                    iGenerated = Convert.ToInt32(partDay.Sum(x => x.Registrado));
                    //foreach (Lineas l in partDay)
                    //{
                    //    dHour = dHour + l.Horas;
                    //    if (l.Validado == 1) iValidated++;
                    //    if (l.Registrado == 1) iGenerated++;
                    //}
                    var status = await aldakinDbContext.Estadodias.FirstOrDefaultAsync(x => x.Dia.Day == date.Day && x.Dia.Month == date.Month && x.Dia.Year == date.Year && x.Idusuario == u.Idusuario);
                    //bool status = false;
                    if (status is null)
                    {
                        iStatus = 0;//no cerrada blanco
                        DateTime dtIniWeek, dtEndWeek;
                        IniEndWeek(DateTime.Now.Date, out dtIniWeek, out dtEndWeek);
                        if (date < dtIniWeek.Date.AddDays(-1))
                        {
                            oTemp.iMoroso = u.Idusuario;
                        }
                    }
                    else
                    {
                        if (status.Estado == 2)
                        {
                            iStatus = 2;//cerrada amarillo
                        }
                        else
                        {
                            iStatus = 4;//generada azul
                        }
                    }
                    oSeach.hour = dHour;
                    oSeach.colour = await DayStatusColour(partDay.Count, iGenerated, iValidated, date, iStatus, u.CodEnt);
                    var strDay = string.Empty;
                    switch (date.DayOfWeek)
                    {
                        case System.DayOfWeek.Sunday:
                            strDay = "D:" + date.Day;
                            break;
                        case System.DayOfWeek.Monday:
                            strDay = "L:" + date.Day;
                            break;
                        case System.DayOfWeek.Tuesday:
                            strDay = "M:" + date.Day;
                            break;
                        case System.DayOfWeek.Wednesday:
                            strDay = "X:" + date.Day;
                            break;
                        case System.DayOfWeek.Thursday:
                            strDay = "J:" + date.Day;
                            break;
                        case System.DayOfWeek.Friday:
                            strDay = "V:" + date.Day;
                            break;
                        case System.DayOfWeek.Saturday:
                            strDay = "S:" + date.Day;
                            break;
                    }
                    oTemp.lDay.Add(strDay);
                    oTemp.dayStatus.Add(oSeach);
                }
                lReturn.Add(oTemp);
            }
            return lReturn;
        }
        
        public async Task<string> StringWeekResumeAsync(int idAldakinUser, string strDate, string strOt, string strWorker, string strEntity)
        {
            string strReturn = string.Empty;
            var lTemp = new List<Lineas>();
            DateTime dtIniWeek, dtEndWeek;
            DateTime dtSelected = Convert.ToDateTime(strDate);
            IniEndWeek(dtSelected, out dtIniWeek, out dtEndWeek);
            if (dtIniWeek.Month < dtSelected.Month)
            {
                dtIniWeek = new DateTime(dtSelected.Year, dtSelected.Month, 01);
            }
            if (dtEndWeek.Month > dtSelected.Month)
            {
                dtEndWeek = new DateTime(dtSelected.Year, dtSelected.Month + 1, 01);
                dtEndWeek = dtEndWeek.AddDays(-1);
            }

            try
            {
                int iOt = Convert.ToInt32(strOt);
                int iWorker = Convert.ToInt32(strWorker);
                int iEntity = Convert.ToInt32(strEntity);
                if ((iWorker > 0) && (iOt > 0))
                {
                    //seleccionado trabajador y ot
                    lTemp = await aldakinDbContext.Lineas.Where(x => x.Inicio.Date >= dtIniWeek.Date && x.Fin.Date <= dtEndWeek.Date && x.Idusuario == iWorker && x.CodEnt == iEntity && x.Idot == iOt && x.Validado == 0 && x.Registrado == 0).OrderBy(x => x.Inicio).ToListAsync();
                }
                else
                {
                    if ((iWorker > 0) && (iOt == 0))
                    {
                        //seleccionado trabajador y no ot
                        lTemp = await aldakinDbContext.Lineas.Where(x => x.Inicio.Date >= dtIniWeek.Date && x.Fin.Date <= dtEndWeek.Date && x.Idusuario == iWorker && x.CodEnt == iEntity && x.Validado == 0 && x.Registrado == 0).OrderBy(x => x.Inicio).ToListAsync();//&& x.CodEnt == _iUserCondEntO
                    }
                    else
                    {
                        if ((iWorker == 0) && (iOt > 0))
                        {
                            // no seleccionado trabajador y seleccionado ot
                            lTemp = await aldakinDbContext.Lineas.Where(x => x.Inicio.Date >= dtIniWeek.Date && x.Fin.Date <= dtEndWeek.Date && x.CodEnt == iEntity && x.Idot == iOt && x.Validado == 0 && x.Registrado == 0).OrderBy(x => x.Inicio).ToListAsync();
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
                    var oTemp = await aldakinDbContext.Estadodias.FirstOrDefaultAsync(x => x.Idusuario == l.Idusuario && x.Dia == l.Inicio.Date);
                    if (!(oTemp is null))
                    {
                        strReturn = l.Idlinea + "|" + strReturn;
                    }
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
            if (dtIniWeek.Month < dtSelected.Month)
            {
                dtIniWeek = new DateTime(dtSelected.Year, dtSelected.Month, 01);
            }
            if (dtEndWeek.Month > dtSelected.Month)
            {
                dtEndWeek = new DateTime(dtSelected.Year, dtSelected.Month + 1, 01);
                dtEndWeek = dtEndWeek.AddDays(-1);
            }
            try
            {
                int iOt = Convert.ToInt32(strOt);
                int iWorker = Convert.ToInt32(strWorker);
                int iEntity = Convert.ToInt32(strEntity);
                if ((iWorker > 0) && (iOt > 0))
                {
                    //seleccionado trabajador y ot
                    lTemp = await aldakinDbContext.Lineas.Where(x => x.Inicio.Date >= dtIniWeek.Date && x.Fin.Date <= dtEndWeek.Date && x.Idusuario == iWorker && x.CodEnt == iEntity && x.Idot == iOt).OrderBy(x => x.Inicio).ToListAsync();
                }
                else
                {
                    if ((iWorker > 0) && (iOt == 0))
                    {
                        //seleccionado trabajador y no ot
                        lTemp = await aldakinDbContext.Lineas.Where(x => x.Inicio.Date >= dtIniWeek.Date && x.Fin.Date <= dtEndWeek.Date && x.Idusuario == iWorker && x.CodEnt == iEntity).OrderBy(x => x.Inicio).ToListAsync();//&& x.CodEnt == _iUserCondEntO
                    }
                    else
                    {
                        if ((iWorker == 0) && (iOt > 0))
                        {
                            // no seleccionado trabajador y seleccionado ot
                            lTemp = await aldakinDbContext.Lineas.Where(x => x.Inicio.Date >= dtIniWeek.Date && x.Fin.Date <= dtEndWeek.Date && x.CodEnt == iEntity && x.Idot == iOt).OrderBy(x => x.Inicio).ToListAsync();
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
        public async Task<string> PrepareWorkLineAsync(WorkerLineData dataToInsertLine, int idAldakinUser, int idAdminUser, string strAction = "")
        {
            //dataToInsertLine datos del parte que hay que procesar antes de guardar es de tipo WorkerLineData(casi todo es string)
            //idAldakinUser id usuario que esta creando el parte
            //para opbtener la id del propietario del parte lo obtenemos de dataToInsertLine

            //el orden de las operaciones es importante

            WriteUserDataAsync(idAldakinUser);
            string strReturn = string.Empty;
            string strAdminName = string.Empty;
            var oLinea = new Lineas();
            var oLineOriginal = new Lineas();
            var oLineSecundaria = new Lineas();
            var iPernoctacion = 0;
            var oOt = new Ots();
            DateTime dtInicio, dtFin;
            var lGastos = new List<Gastos>();
            float fGastos = 0, fKilometros = 0;
            int iUserCodEnt = 0, iOtOriginal = 0, iNumOt = 0, iIdLinea = 0;
            try
            {
                try
                {
                    oLinea.Idusuario = dataToInsertLine.iIdUsuario;
                    oLinea.Idot = Convert.ToInt32(dataToInsertLine.strOt);
                    oOt = await aldakinDbContext.Ots.FirstOrDefaultAsync(x => x.Idots == oLinea.Idot);
                    var oUser = await aldakinDbContext.Usuarios.FirstOrDefaultAsync(x => x.Idusuario == oLinea.Idusuario && x.CodEnt == x.CodEntO);
                    oLinea.CodEnt = oOt.CodEnt;
                    iNumOt = oOt.Numero;
                    iUserCodEnt = oUser.CodEnt;
                    //si hay linea se usa para la edicion de partes
                    if (!(string.Equals(dataToInsertLine.strIdlineaAntigua, "0")))
                    {
                        oLineOriginal = await aldakinDbContext.Lineas.FirstOrDefaultAsync(x => x.Idlinea == Convert.ToInt32(dataToInsertLine.strIdlineaAntigua));
                        //esto es caca
                        //oLineSecundaria = await aldakinDbContext.Lineas.FirstOrDefaultAsync(x => x.Idlinea == oLineOriginal.Idoriginal);
                        //if (!(oLineSecundaria is null))
                        //{
                        //    oOt = await aldakinDbContext.Ots.FirstOrDefaultAsync(x => x.Idots == oLineSecundaria.Idot);
                        //    oLinea.CodEnt = oOt.CodEnt;
                        //    iNumOt = oOt.Numero;
                        //}
                    }
                    if ((string.IsNullOrEmpty(dataToInsertLine.strPreslin)) || (string.Equals(dataToInsertLine.strPreslin, "0")))
                    {
                        oLinea.Idpreslin = null;
                    }
                    else
                    {
                        if (dataToInsertLine.strPreslin.Equals("-1"))
                        {
                            oLinea.Idpreslin = null;
                        }
                        else
                        {
                            oLinea.Idpreslin = Convert.ToInt32(dataToInsertLine.strPreslin);
                        }
                    }
                    if (string.IsNullOrEmpty(dataToInsertLine.strObservaciones))
                    {
                        oLinea.Observaciones = string.Empty;
                    }
                    else
                    {
                        oLinea.Observaciones = dataToInsertLine.strObservaciones.ToUpper();
                    }
                    if (string.IsNullOrEmpty(dataToInsertLine.strParte))
                    {
                        oLinea.Npartefirmado = string.Empty;
                    }
                    else
                    {
                        oLinea.Npartefirmado = dataToInsertLine.strParte.ToUpper();
                    }
                    //Analisis de las horas
                    //metemos el parte rellenado y el usuario propietario del parte y no devuelve las fechas modificadas y ajustadas(si es solo gatos rangos usados o si semana cerrada)
                    strReturn = AnalizeWorkLineDayAsync(dataToInsertLine, oLinea.Idusuario, idAdminUser, oLineOriginal.Idlinea, out dtInicio, out dtFin);
                    if (!(string.IsNullOrEmpty(strReturn)))
                    {
                        //si devuelve string es que algo ha ido mal
                        return strReturn;
                    }
                    else
                    {
                        oLinea.Inicio = dtInicio;
                        oLinea.Fin = dtFin;
                    }
                    if (!(string.IsNullOrEmpty(dataToInsertLine.bHorasViaje)))
                    {
                        oLinea.Horasviaje = Convert.ToSingle((dtFin - dtInicio).TotalHours);
                    }
                    else
                    {
                        oLinea.Horasviaje = 0;
                    }
                    if (string.IsNullOrEmpty(dataToInsertLine.strPernoctacion))
                    {
                        oLinea.Facturable = 0;
                    }
                    else
                    {
                        oLinea.Facturable = Convert.ToInt32(dataToInsertLine.strPernoctacion);
                    }
                    //Gastos
                    //metemos ot del parte la lista de gastos del parte y nos dara una lista de gastos y el valor de los gastos y kilometros
                    strReturn = CreateWorkExpenses(oLinea.Idot, dataToInsertLine.strGastos, out lGastos, out fGastos, out fKilometros);
                    if (!(string.IsNullOrEmpty(strReturn)))
                    {
                        //si devuelve string es que algo ha ido mal
                        return strReturn;
                    }
                    oLinea.Km = fKilometros;
                    oLinea.Dietas = fGastos;
                    //analiza si hay ot original trabajos para otras delegaciones
                    strReturn = OriginalOT(oLinea.Idot, oLinea.Observaciones, iUserCodEnt,oUser.Name, out iOtOriginal);
                    if (!(string.IsNullOrEmpty(strReturn)))
                    {
                        //si devuelve string es que algo ha ido mal
                        return strReturn;
                    }
                    if(iOtOriginal==0)
                    {
                        return "No estas dado de alta en la delegacion origen de la Ot";
                    }
                    oLinea.Idoriginal = 0;
                    //horas del parte
                    oLinea.Horas = Convert.ToSingle((dtFin - dtInicio).TotalHours) - oLinea.Horasviaje ?? 0;
                    if (string.Equals(dataToInsertLine.strAction, "SaveAndValidate"))
                    {
                        oLinea.Validado = 1;
                        var admin = await aldakinDbContext.Usuarios.FirstOrDefaultAsync(x => x.Idusuario == idAdminUser);
                        strAdminName = admin.Nombrecompleto + "[" + DateTime.Now + "]";
                        oLinea.Validador = strAdminName;
                    }
                    oLinea.Validador = strAdminName;
                    if ((!(string.IsNullOrEmpty(strAction))) && (string.Equals(strAction, "edit")))
                    {

                        if (!(oLineOriginal is null))
                        {
                            //existe unn trabajos para en la linea a editar
                            var oReturn = await _iWriteDataBase.DeleteWorkerLineAsync(oLineOriginal.Idlinea, oLineOriginal.Idusuario, idAdminUser);
                            if (oReturn.Count != 2)//if (oReturn.First().iValue==1)
                            {
                                //si devuelve string es que algo ha ido mal
                                strReturn = "Ha ocurrido un error al borrar las lineas del parte original";
                                return strReturn;
                            }
                        }
                        else
                        {
                            strReturn = "Error No Hay Parte a editar";
                            return strReturn;
                        }
                    }

                    int idOriginal = await _iWriteDataBase.InsertWorkerLineAsync(oLinea);
                    if (idOriginal == 0)
                    {
                        //si devuelve string es que algo ha ido mal
                        strReturn = "Ha ocurrido un error al insertar la linea Original";
                        return strReturn;
                    }
                    else
                    {
                        oLinea.Idlinea = idOriginal;
                        if (oLinea.CodEnt != iUserCodEnt)
                        {
                            try
                            {
                                string Salida = iNumOt.ToString(), Primerdigito, Resto;

                                var Cero = Convert.ToChar("0");
                                Primerdigito = iNumOt.ToString().Substring(0, 1);
                                if (iNumOt > 9)
                                {
                                    Resto = iNumOt.ToString().Substring(2, iNumOt.ToString().Length - 2);
                                    Resto = Resto.TrimStart(Cero);
                                }
                                else
                                {
                                    Resto = "0";
                                }
                                Salida = string.Format("{0}|{1}", Primerdigito, Resto);
                                var observaciones = Salida + " " + oLinea.Observaciones;

                                //from ots where cierre is null and year(apertura) = year(iinicio) and  cod_ent = icod_ent and cod_ent_d = (select cod_ent from lineas where idlinea = iidoriginal)
                                var ot = await aldakinDbContext.Ots.FirstOrDefaultAsync(x => x.Cierre == null && x.Apertura.Year == oLinea.Inicio.Year && x.CodEnt == iUserCodEnt && x.CodEntD == aldakinDbContext.Lineas.FirstOrDefault(y => y.Idlinea == oLinea.Idlinea).CodEnt);
                                var iOt = ot.Idots;
                                var oLineaSecundaria = new Lineas();
                                oLineaSecundaria = oLinea;
                                oLineaSecundaria.Idot = iOt;
                                oLineaSecundaria.Idpreslin = null;
                                oLineaSecundaria.Observaciones = observaciones.ToUpper();
                                oLineaSecundaria.CodEnt = iUserCodEnt;
                                oLineaSecundaria.Idoriginal = oLinea.Idlinea;
                                oLineaSecundaria.Validado = oLinea.Validado;
                                oLineaSecundaria.Validador = strAdminName;
                                var iCopy = await _iWriteDataBase.InsertWorkerLineAsync(oLineaSecundaria);
                                if (iCopy == 0)
                                {
                                    //si devuelve string es que algo ha ido mal
                                    strReturn = "Ha ocurrido un error al insertar la linea Secundaria";
                                    return strReturn;
                                }
                            }
                            catch(Exception ex)
                            {
                                var oReturn = await _iWriteDataBase.DeleteWorkerLineAsync(idOriginal, oLineOriginal.Idusuario, idAdminUser);
                                if (oReturn.Count != 2)//if (oReturn.First().iValue==1)
                                {
                                    //si devuelve string es que algo ha ido mal
                                    strReturn = "Ha ocurrido un error al borrar las lineas del parte original";
                                    return strReturn;
                                }
                                strReturn = "Ha ocurrido un error al gestionar el trabajos para Hable con Administracion";
                            }
                        }
                        if ((lGastos.Count > 0) &&(string.IsNullOrEmpty(strReturn)))
                        {
                            strReturn = await _iWriteDataBase.InsertUserWorkExpensesAsync(lGastos, oLinea.Idlinea);
                            if (!(string.IsNullOrEmpty(strReturn)))
                            {
                                //si devuelve string es que algo ha ido mal
                                strReturn = "Ha ocurrido un error al insertar los gastos";
                                return strReturn;
                            }
                        }
                    }

                    //si hay mensaje de aviso de modificacion por parte del administrador
                    if ((!(string.IsNullOrEmpty(dataToInsertLine.strMensaje))) && (!(oLineOriginal is null)))
                    {
                        var strPreslin = string.Empty;
                        var oPres = await aldakinDbContext.Preslin.FirstOrDefaultAsync(x => x.Idpreslin == oLineOriginal.Idpreslin);
                        if (!(oPres is null))
                        {
                            strPreslin = oPres.Nombre;
                        }
                        var oOtOriginal = await aldakinDbContext.Ots.FirstOrDefaultAsync(x => x.Idots == oLineOriginal.Idot);
                        var strMenssage = "ORIGINAL|" + oOtOriginal.Numero + "-" + oOtOriginal.Nombre + "|" + strPreslin + "|" + oLineOriginal.Inicio + "|" + oLineOriginal.Fin + "\r\n";
                        oPres = await aldakinDbContext.Preslin.FirstOrDefaultAsync(x => x.Idpreslin == oLinea.Idpreslin);
                        strPreslin = string.Empty;
                        if (!(oPres is null))
                        {
                            strPreslin = oPres.Nombre;
                        }
                        strMenssage = strMenssage + "MODIFICADO|" + oOt.Numero + "-" + oOt.Nombre + "|" + strPreslin + "|" + oLinea.Inicio + "|" + oLinea.Fin + "\r\n";
                        strMenssage = strMenssage + "\r\n" + dataToInsertLine.strMensaje.ToUpper();
                        var message = new LineMessage
                        {
                            Asunto = "AVISO: MODIFICACION DE PARTE",
                            Mensaje = strMenssage,
                            Idlinea = oLinea.Idlinea,
                            De = idAdminUser,
                            A = idAldakinUser,
                            Inicial = 0
                        };

                        await _iWriteDataBase.AnswerMessageAsync(message);
                    }
                }
                catch (Exception ex)
                {
                    strReturn = "Se ha producido un error en el procesamiento de los datos;";
                    return (strReturn);
                }
            }
            catch (Exception ex)
            {
                strReturn = "Se ha producido un error en el procesamiento de los datos;";
                return (strReturn);
            }
            strReturn = "Parte trabajado satisfactoriamente";
            return (strReturn);
        }
        public async Task<string> NewValidationUsersAsync(int idAldakinUser, string strUsers, string strWorker)
        {
            var strReturn = string.Empty;
            var strListUsers = string.Empty;
            try
            {
                var autor = await aldakinDbContext.Usuarios.FirstOrDefaultAsync(x => x.Idusuario == idAldakinUser);
                var name = await aldakinDbContext.Usuarios.FirstOrDefaultAsync(x => x.Nombrecompleto == strWorker && x.CodEnt == x.CodEntO);
                List<string> split = strUsers.Split(new Char[] { '|' }).Distinct().ToList();
                foreach (string s in split)
                {
                    try
                    {

                        var users = await aldakinDbContext.Usuarios.FirstOrDefaultAsync(x => x.Idusuario == Convert.ToInt32(s));
                        if (!(users is null))
                        {
                            if (string.IsNullOrEmpty(strListUsers))
                            {
                                strListUsers = users.Name;
                            }
                            else
                            {
                                strListUsers = strListUsers + "|" + users.Name;
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
                if (!(strListUsers is null))
                {

                    strReturn = await _iWriteDataBase.WritePermissionAsync(name.Name, strListUsers, autor.Name, "trabajador");
                }
                else
                {
                    strReturn = "Se ha dado un error en los datos de los permisos reintente y avise a administración";
                }
            }
            catch (Exception ex)
            {
                strReturn = "Se ha dado un error en los datos de los permisos reintente y avise a administración";
            }
            return strReturn;
        }
        public async Task<string> NewValidationOtAsync(int idAldakinUser, string strOts, string strWorker)
        {
            var strReturn = string.Empty;
            var strListUsers = string.Empty;
            try
            {
                var autor = await aldakinDbContext.Usuarios.FirstOrDefaultAsync(x => x.Idusuario == idAldakinUser);
                var name = await aldakinDbContext.Usuarios.FirstOrDefaultAsync(x => x.Nombrecompleto == strWorker && x.CodEnt == x.CodEntO);
                List<string> split = strOts.Split(new Char[] { '|' }).Distinct().ToList();
                foreach (string s in split)
                {
                    try
                    {
                        var ot = await aldakinDbContext.Ots.FirstOrDefaultAsync(x => x.Idots == Convert.ToInt32(s));
                        if (!(ot is null))
                        {
                            if (string.IsNullOrEmpty(strListUsers))
                            {
                                strListUsers = Convert.ToString(ot.Idots);
                            }
                            else
                            {
                                strListUsers = strListUsers + "|" + Convert.ToString(ot.Idots);
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
                if (!(strListUsers is null))
                {
                    strReturn = await _iWriteDataBase.WritePermissionAsync(name.Name, strListUsers, autor.Name, "ot");
                }
                else
                {
                    strReturn = "Se ha dado un error en los datos de los permisos reintente y avise a administración";
                }
            }
            catch (Exception ex)
            {
                strReturn = "Se ha dado un error en los datos de los permisos reintente y avise a administración";
            }
            return strReturn;
        }
        //struct idsComprobacion
        //{
        //    public int idOt;
        //    public int idPreslin;
        //};
        public async Task<List<ExcelWorkerReport>> WorkerReportAsync(int iIdUser, DateTime dtSelected)
        {
            
            List<ExcelWorkerReport> lReturn = new List<ExcelWorkerReport>();
            var lLineas = await aldakinDbContext.Lineas.Where(x => x.Idusuario == iIdUser && x.Inicio.Month == dtSelected.Month && x.Inicio.Year == dtSelected.Year).OrderBy(x=>x.Inicio).ToListAsync();
            var oUser = await aldakinDbContext.Usuarios.FirstOrDefaultAsync(x => x.Idusuario == iIdUser);
            foreach(Lineas l in lLineas)
            {
                lReturn.Add(new ExcelWorkerReport
                {
                    strEmpleado = oUser.Nombrecompleto+"["+ oUser.CodEmpl + "]",
                    strFecha = l.Inicio.Date.ToString(),
                    strIni=l.Inicio.Hour+":"+l.Inicio.Minute,
                    strFin = l.Fin.Hour + ":" + l.Fin.Minute,
                }); 
            }
            return lReturn;

        }
        public async Task<List<ListExcel1>> ReviewHourMonthAsync(int iCodEnt, DateTime dtSelected)
        {

            List<ListExcel1> lReturn = new List<ListExcel1>();
            var lLineas = await aldakinDbContext.Lineas.Where(x => x.CodEnt == iCodEnt && x.Inicio.Month == dtSelected.Month && x.Inicio.Year == dtSelected.Year).ToListAsync();
            var lUsuarios = lLineas.Select(o => o.Idusuario).Distinct().ToList();

            //var min = lLineas.Min(x => x.Inicio.Date);
            //var max = lLineas.Max(x => x.Inicio.Date);
            var min = new DateTime(dtSelected.Year, dtSelected.Month, 1);
            var max = new DateTime(dtSelected.Year, dtSelected.Month + 1, 1).AddDays(-1);
            var estadoDias= await aldakinDbContext.Estadodias.Where(x => x.Dia.Date >= min.Date && x.Dia.Date <= max.Date).ToListAsync();
            foreach (int u in lUsuarios)
            {
                ListExcel1 o = new ListExcel1();
                var usuario = await aldakinDbContext.Usuarios.FirstOrDefaultAsync(x => x.Idusuario == u);
                var lUserLines = lLineas.Where(x => x.Idusuario == u).ToList();
                //var lDateUserLines = lUserLines.Select(x => x.Inicio.Date).Distinct().OrderBy(x => x.Date).ToList();
                //var dtMin = lUserLines.Min(x => x.Inicio);
                //var dtMax = lUserLines.Max(x => x.Inicio);
                o.nombre = usuario.Nombrecompleto;
                List<ExcelFormat> dato = new List<ExcelFormat>();
                List<DateTime> dia = new List<DateTime>();
                List<double> horas = new List<double>();
                for (DateTime dt = min; dt <= max; dt = dt.AddDays(1.0))
                {
                //    foreach (DateTime dt in lDateUserLines)
                //{
                    if ((dt.Day==30)&&(u== 2079))
                    {
                        var t = 0;
                    }    
                    float horasSuma;
                    string color="#FFFFFF";
                    var lPartsDay = lUserLines.Where(x => x.Inicio.Date == dt.Date).ToList();
                    if((lPartsDay.Count==0)||(lPartsDay is null))
                    {
                        horasSuma = 0;
                    }
                    else
                    {
                        //horasSuma = lPartsDay.Sum(x => x.Horas);
                        horasSuma = 0;
                        foreach (Lineas l in lPartsDay)
                        {
                            horasSuma = horasSuma + Convert.ToSingle((l.Fin - l.Inicio).TotalHours);
                        }
                        var x = lPartsDay.Count;
                        var iValidated = Convert.ToInt32(lPartsDay.Sum(x => x.Validado));
                        var iGenerated = Convert.ToInt32(lPartsDay.Sum(x => x.Registrado));
                        var status = estadoDias.FirstOrDefault(x => x.Dia.Day == dt.Day && x.Dia.Month == dt.Month && x.Dia.Year == dt.Year && x.Idusuario == u); // await aldakinDbContext.Estadodias.FirstOrDefaultAsync(x => x.Dia.Day == dt.Day && x.Dia.Month == dt.Month && x.Dia.Year == dt.Year && x.Idusuario == u);
                        //bool status = false;
                        var iStatus = 0;//no cerrada blanco
                        if (status is null)
                        {
                            iStatus = 0;//no cerrada blanco
                        }
                        else
                        {
                            if (status.Estado == 2)
                            {
                                iStatus = 2;//cerrada amarillo
                            }
                            else
                            {
                                iStatus = 4;//generada azul
                            }
                        }
                        color = await DayStatusColour(lPartsDay.Count, iGenerated, iValidated, dt, iStatus, usuario.CodEnt);
                    }
                    dato.Add(new ExcelFormat
                    {
                        dia = dt.Date,
                        horas = horasSuma,
                        color= color
                    }); 
                }
                o.Datos=dato;
                //o.Datos.dia= dia;
                //o.horas = horas;
                lReturn.Add(o);
            }
            return lReturn;
        }

        public async Task<string> GetUserNameAsync(int iUser)
        {
            string strReturn;
            var user = await aldakinDbContext.Usuarios.FirstOrDefaultAsync(x => x.Idusuario == iUser);
            if (user is null)
            {
                strReturn = "NoUserData";
            }
            else
            {
                strReturn = user.Nombrecompleto;
            }
            return strReturn;
        }

        public async Task<List<ExcelTipoGasto>> ReviewExpenseTypeExpenseAsync(int iCodEnt, DateTime dtSelected)
        {
            var kilTrabajador = 0.25F;
            var kilAldakin = 0;
            var kilIineraciaTrabajador = 0.15F;

            List<ExcelTipoGasto> lReturn = new List<ExcelTipoGasto>();
            try
            {
                //return lReturn;
                int iTipoHora = 0, iMomento = 0; ;
                float[] fMomentos = new float[4];
                DateTime PrimerDia = new DateTime(dtSelected.Year, dtSelected.Month, 1, 0, 0, 0);
                DateTime UltimoDia = PrimerDia.AddMonths(1).AddDays(-1);
                //todos los partes del mes y de la delegacion
                List<Lineas> lLineasAll = await aldakinDbContext.Lineas.Where(x => x.Inicio.Month == dtSelected.Month && x.Inicio.Year == dtSelected.Year && x.Registrado == 1 && x.CodEnt == iCodEnt && (x.Km>0 || x.Dietas>0)).ToListAsync();
                //usuarios con parte
                var tipoGastos = await aldakinDbContext.Tipogastos.Where(x => x.CodEnt == iCodEnt).ToListAsync();
                var user = await aldakinDbContext.Usuarios.Where(x => x.CodEnt == x.CodEntO && x.CodEnt == iCodEnt).ToListAsync();
                var lUsuarios = lLineasAll.Select(o => o.Idusuario).Distinct().ToList();
                foreach (int u in lUsuarios)
                {
                    var oUser = user.FirstOrDefault(x => x.Idusuario == u);
                    if (!(oUser is null))
                    {
                        var nombre = oUser.Nombrecompleto;
                        var codEmpl = oUser.CodEmpl;
                        //listar los partes de trabajos ese mes
                        List<Lineas> lTemp = lLineasAll.Where(x => x.Idusuario == u).ToList();
                        //distintas ots
                        var lOts = lTemp.Select(o => o.Idot).Distinct().ToList();
                        foreach (int o in lOts)
                        {
                            var oPresu = await aldakinDbContext.Presupuestos.FirstOrDefaultAsync(x => x.Idot == o);
                            string strNumeroPresu = "";
                            if (!(oPresu is null)) strNumeroPresu = oPresu.Nombre.ToString();
                            string strNombreCapitulo = "";
                            int iAnexo = 0;
                            int iVersion = 0;
                            var oOt = await aldakinDbContext.Ots.FirstOrDefaultAsync(x => x.Idots == o);
                            var nombreOt = oOt.Nombre;
                            var numeroOt = oOt.Numero;
                            int iRefOT;
                            try
                            {
                                iRefOT = Convert.ToInt32(oOt.Codigorefot);
                            }
                            catch (Exception ex)
                            {
                                iRefOT = 0;
                            }



                            //var oTipoHora = await aldakinDbContext.Tipohora.FirstOrDefaultAsync(x => x.RefOt == iRefOT);
                            List<Lineas> lLineasOt = lTemp.Where(x => x.Idot == o).ToList();
                            var lpTemp = lLineasOt.Select(o => o.Idpreslin).Distinct().ToList();
                            if (lpTemp is null)
                            {
                                foreach (Lineas l in lLineasOt)
                                {
                                    var lGastos = await aldakinDbContext.Gastos.Where(x => x.Idlinea == l.Idlinea).ToListAsync();
                                    foreach (Tipogastos tg in tipoGastos)
                                    {
                                        float fCantidad = 0;
                                        float fCoste = 0;
                                        var gasto = lGastos.Where(x => x.Tipo == tg.Idtipogastos).ToList();
                                        fCantidad = gasto.Sum(x => x.Cantidad);
                                        if (fCantidad > 0)
                                        {
                                            switch (tg.CodArt.ToLower())
                                            {
                                                case "kil": //KILOMETROS TRABAJADOR
                                                    fCoste = fCantidad * kilTrabajador;
                                                    break;
                                                case "kilempresa": //KILOMETROS ALDAKIN
                                                    fCoste = fCantidad * kilAldakin;
                                                    break;
                                                case "kilpropio": //KIL ITINERANCIA TRABAJADOR
                                                    fCoste = fCantidad * kilIineraciaTrabajador;
                                                    break;
                                                default:
                                                    fCoste = fCantidad * 1;
                                                    break;
                                            }
                                            lReturn.Add(new ExcelTipoGasto
                                            {
                                                strEmpleado = nombre,
                                                iCodEmpleado = codEmpl,
                                                iOT = numeroOt,
                                                strOT = nombreOt,
                                                strPresu = strNumeroPresu,
                                                strCapitulo = "",
                                                strNomCapitulo = strNombreCapitulo,
                                                iAnexo = iAnexo,
                                                iVersion = iVersion,
                                                fCantidad = fCantidad,
                                                fCoste = fCoste,
                                                strArticulo = tg.Tipo
                                            });
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //los partes tienen preslin
                                foreach (int? p in lpTemp)
                                {
                                    if (p is null)
                                    {
                                        foreach (Lineas l in lLineasOt)
                                        {
                                            var lGastos = await aldakinDbContext.Gastos.Where(x => x.Idlinea == l.Idlinea).ToListAsync();
                                            foreach (Tipogastos tg in tipoGastos)
                                            {
                                                float fCantidad = 0;
                                                float fCoste = 0;
                                                var gasto = lGastos.Where(x => x.Tipo == tg.Idtipogastos).ToList();
                                                fCantidad = gasto.Sum(x => x.Cantidad);
                                                if(fCantidad>0)
                                                {
                                                    switch (tg.CodArt.ToLower())
                                                    {
                                                        case "kil": //KILOMETROS TRABAJADOR
                                                            fCoste = fCantidad * kilTrabajador;
                                                            break;
                                                        case "kilempresa": //KILOMETROS ALDAKIN
                                                            fCoste = fCantidad * kilAldakin;
                                                            break;
                                                        case "kilpropio": //KIL ITINERANCIA TRABAJADOR
                                                            fCoste = fCantidad * kilIineraciaTrabajador;
                                                            break;
                                                        default:
                                                            fCoste = fCantidad * 1;
                                                            break;
                                                    }
                                                    lReturn.Add(new ExcelTipoGasto
                                                    {
                                                        strEmpleado = nombre,
                                                        iCodEmpleado = codEmpl,
                                                        iOT = numeroOt,
                                                        strOT = nombreOt,
                                                        strPresu = strNumeroPresu,
                                                        strCapitulo = "",
                                                        strNomCapitulo = strNombreCapitulo,
                                                        iAnexo = iAnexo,
                                                        iVersion = iVersion,
                                                        fCantidad = fCantidad,
                                                        fCoste = fCoste,
                                                        strArticulo = tg.Tipo
                                                    });
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var oPreslin = await aldakinDbContext.Preslin.FirstOrDefaultAsync(x => x.Idpreslin == p);
                                        iAnexo = oPreslin.Anexo ?? 0;
                                        iVersion = oPreslin.Version ?? 0;
                                        strNombreCapitulo = oPreslin.Nombre.ToString();
                                        List<Lineas> lLineasPres = lTemp.Where(x => x.Idpreslin == p).ToList();
                                        string strChapter = await ReadChapter(oPreslin.Idpreslin);
                                        foreach (Lineas l in lLineasOt)
                                        {
                                            var lGastos = await aldakinDbContext.Gastos.Where(x => x.Idlinea == l.Idlinea).ToListAsync();
                                            foreach (Tipogastos tg in tipoGastos)
                                            {
                                                float fCantidad = 0;
                                                float fCoste = 0;
                                                var gasto = lGastos.Where(x => x.Tipo == tg.Idtipogastos).ToList();
                                                fCantidad = gasto.Sum(x => x.Cantidad);
                                                if (fCantidad > 0)
                                                {
                                                    switch (tg.CodArt.ToLower())
                                                    {
                                                        case "kil": //KILOMETROS TRABAJADOR
                                                            fCoste = fCantidad * kilTrabajador;
                                                            break;
                                                        case "kilempresa": //KILOMETROS ALDAKIN
                                                            fCoste = fCantidad * kilAldakin;
                                                            break;
                                                        case "kilpropio": //KIL ITINERANCIA TRABAJADOR
                                                            fCoste = fCantidad * kilIineraciaTrabajador;
                                                            break;
                                                        default:
                                                            fCoste = fCantidad * 1;
                                                            break;
                                                    }
                                                    lReturn.Add(new ExcelTipoGasto
                                                    {
                                                        strEmpleado = nombre,
                                                        iCodEmpleado = codEmpl,
                                                        iOT = numeroOt,
                                                        strOT = nombreOt,
                                                        strPresu = strNumeroPresu,
                                                        strCapitulo = "",
                                                        strNomCapitulo = strNombreCapitulo,
                                                        iAnexo = iAnexo,
                                                        iVersion = iVersion,
                                                        fCantidad = fCantidad,
                                                        fCoste = fCoste,
                                                        strArticulo = tg.Tipo
                                                    });
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var e = ex;
            }
            return lReturn;
        }
        public async Task<List<ExcelTipoHora>> ReviewHourTypeHourAsync(int iCodEnt, DateTime dtSelected)
        {
            List<ExcelTipoHora> lReturn = new List<ExcelTipoHora>();
            try
            {
                //return lReturn;
                int iTipoHora = 0, iMomento = 0; ;
                float[] fMomentos = new float[4];
                DateTime PrimerDia = new DateTime(dtSelected.Year, dtSelected.Month, 1, 0, 0, 0);
                DateTime UltimoDia = PrimerDia.AddMonths(1).AddDays(-1);
                //todos los partes del mes y de la delegacion
                List<Lineas> lLineasAll = await aldakinDbContext.Lineas.Where(x => x.Inicio.Month == dtSelected.Month && x.Inicio.Year == dtSelected.Year && x.Registrado == 1 && x.CodEnt == iCodEnt).ToListAsync();
                //usuarios con parte
                var user = await aldakinDbContext.Usuarios.Where(x => x.CodEnt == x.CodEntO && x.CodEnt == iCodEnt).ToListAsync();
                var lUsuarios = lLineasAll.Select(o => o.Idusuario).Distinct().ToList(); 
                foreach (int u in lUsuarios)
                {
                    var oUser = user.FirstOrDefault(x=>x.Idusuario==u);
                    if (!(oUser is null))
                    {
                        var nombre = oUser.Nombrecompleto;
                        var codEmpl = oUser.CodEmpl;
                        //listar los partes de trabajos ese mes
                        List<Lineas> lTemp = lLineasAll.Where(x => x.Idusuario == u).ToList();
                        //distintas ots
                        var lOts = lTemp.Select(o => o.Idot).Distinct().ToList();
                        foreach (int o in lOts)
                        {
                            var oPresu = await aldakinDbContext.Presupuestos.FirstOrDefaultAsync(x => x.Idot == o);
                            string strNumeroPresu = "";
                            if (!(oPresu is null)) strNumeroPresu = oPresu.Nombre.ToString();
                            string strNombreCapitulo = "";
                            int iAnexo = 0;
                            int iVersion = 0;
                            var oOt = await aldakinDbContext.Ots.FirstOrDefaultAsync(x => x.Idots == o);
                            var nombreOt = oOt.Nombre;
                            var numeroOt = oOt.Numero;
                            int iRefOT;
                            try
                            {
                                iRefOT = Convert.ToInt32(oOt.Codigorefot);
                            }
                            catch (Exception ex)
                            {
                                iRefOT = 0;
                            }
                            var oTipoHora = await aldakinDbContext.Tipohora.FirstOrDefaultAsync(x => x.RefOt == iRefOT);
                            List<Lineas> lLineasOt = lTemp.Where(x => x.Idot == o).ToList();
                            var lpTemp = lLineasOt.Select(o => o.Idpreslin).Distinct().ToList();
                            if (lpTemp is null)
                            {
                                //los partes que NO tienen preslin
                                fMomentos = await WorkMomentDay(lLineasOt, oOt.CodEnt);
                                for (int i = 0; i <= 3; i++)
                                {
                                    //0 = normal, 1 = noche, 2 = sabado, 3 = festivo
                                    float fHoras;
                                    int iTipo;
                                    switch (i)
                                    {
                                        case 0:
                                            fHoras = fMomentos[0];
                                            iTipo = oTipoHora.Normal;
                                            break;
                                        case 1:
                                            fHoras = fMomentos[1];
                                            iTipo = oTipoHora.Noche;
                                            break;
                                        case 2:
                                            fHoras = fMomentos[2];
                                            iTipo = oTipoHora.Sabado;
                                            break;
                                        case 3:
                                            fHoras = fMomentos[3];
                                            iTipo = oTipoHora.Festivo;
                                            break;
                                        default:
                                            fHoras = 0;
                                            iTipo = 0;
                                            break;
                                    }
                                    lReturn.Add(new ExcelTipoHora
                                    {
                                        strEmpleado = nombre,
                                        iCodEmpleado = codEmpl,
                                        iOT = numeroOt,
                                        strOT = nombreOt,
                                        strPresu = strNumeroPresu,
                                        strCapitulo = "",
                                        strNomCapitulo = strNombreCapitulo,
                                        iAnexo = iAnexo,
                                        iVersion = iVersion,
                                        fHoras = fHoras,
                                        iTipoHora = iTipo
                                    });
                                }
                            }
                            else
                            {
                                //los partes tienen preslin
                                foreach (int? p in lpTemp)
                                {
                                    if (p is null)
                                    {
                                        fMomentos = await WorkMomentDay(lLineasOt, oOt.CodEnt);
                                        for (int i = 0; i <= 3; i++)
                                        {
                                            //0 = normal, 1 = noche, 2 = sabado, 3 = festivo
                                            float fHoras;
                                            int iTipo;
                                            switch (i)
                                            {
                                                case 0:
                                                    fHoras = fMomentos[0];
                                                    iTipo = oTipoHora.Normal;
                                                    break;
                                                case 1:
                                                    fHoras = fMomentos[1];
                                                    iTipo = oTipoHora.Noche;
                                                    break;
                                                case 2:
                                                    fHoras = fMomentos[2];
                                                    iTipo = oTipoHora.Sabado;
                                                    break;
                                                case 3:
                                                    fHoras = fMomentos[3];
                                                    iTipo = oTipoHora.Festivo;
                                                    break;
                                                default:
                                                    fHoras = 0;
                                                    iTipo = 0;
                                                    break;
                                            }
                                            lReturn.Add(new ExcelTipoHora
                                            {
                                                strEmpleado = nombre,
                                                iCodEmpleado = codEmpl,
                                                iOT = numeroOt,
                                                strOT = nombreOt,
                                                strPresu = strNumeroPresu,
                                                strCapitulo = "",
                                                strNomCapitulo = strNombreCapitulo,
                                                iAnexo = iAnexo,
                                                iVersion = iVersion,
                                                fHoras = fHoras,
                                                iTipoHora = iTipo

                                            });
                                        }
                                    }
                                    else
                                    {
                                        var oPreslin = await aldakinDbContext.Preslin.FirstOrDefaultAsync(x => x.Idpreslin == p);
                                        iAnexo = oPreslin.Anexo ?? 0;
                                        iVersion = oPreslin.Version ?? 0;
                                        strNombreCapitulo = oPreslin.Nombre.ToString();
                                        List<Lineas> lLineasPres = lTemp.Where(x => x.Idpreslin == p).ToList();
                                        string strChapter = await ReadChapter(oPreslin.Idpreslin);
                                        fMomentos = await WorkMomentDay(lLineasPres, oOt.CodEnt);
                                        for (int i = 0; i <= 3; i++)
                                        {
                                            //0 = normal, 1 = noche, 2 = sabado, 3 = festivo
                                            float fHoras;
                                            int iTipo;
                                            switch (i)
                                            {
                                                case 0:
                                                    fHoras = fMomentos[0];
                                                    iTipo = oTipoHora.Normal;
                                                    break;
                                                case 1:
                                                    fHoras = fMomentos[1];
                                                    iTipo = oTipoHora.Noche;
                                                    break;
                                                case 2:
                                                    fHoras = fMomentos[2];
                                                    iTipo = oTipoHora.Sabado;
                                                    break;
                                                case 3:
                                                    fHoras = fMomentos[3];
                                                    iTipo = oTipoHora.Festivo;
                                                    break;
                                                default:
                                                    fHoras = 0;
                                                    iTipo = 0;
                                                    break;
                                            }
                                            lReturn.Add(new ExcelTipoHora
                                            {
                                                strEmpleado = nombre,
                                                iCodEmpleado = codEmpl,
                                                iOT = numeroOt,
                                                strOT = nombreOt,
                                                strPresu = strNumeroPresu,
                                                strCapitulo = strChapter,
                                                strNomCapitulo = strNombreCapitulo,
                                                iAnexo = iAnexo,
                                                iVersion = iVersion,
                                                fHoras = fHoras,
                                                iTipoHora = iTipo
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var e = ex;
            }
            return lReturn;
        }
        private async Task<string> ReadChapter(int iIdPreslin)
        {
            string strReturn = string.Empty;
            string[] strTemp = new string[8];
            strTemp[0] = "0";
            strTemp[1] = "0";
            strTemp[2] = "0";
            strTemp[3] = "0";
            strTemp[4] = "0";
            strTemp[5] = "0";
            strTemp[6] = "0";
            strTemp[7] = "0";
            var pres = await aldakinDbContext.Preslin.FirstOrDefaultAsync(x=>x.Idpreslin==iIdPreslin);
            if (!(pres is null))
            {
                var codp = pres.CodpPes ?? 0;
                var t = pres.Numero ?? 0;
                var x = pres.Nivel ?? 0;
                strTemp[x] = t.ToString();
                for (int i=x;i>0;i--)
                {
                    var level = await aldakinDbContext.Preslin.FirstOrDefaultAsync(x=>x.Idpresupuesto==pres.Idpresupuesto && x.CodhPes== codp && x.Anexo==pres.Anexo && x.Version == pres.Version);
                    codp = level.CodpPes ?? 0;
                    t = level.Numero ?? 0;
                    x = level.Nivel ?? 0;
                    strTemp[x] = t.ToString();
                }
            }
            strReturn = strTemp[1] + strTemp[2] + strTemp[3] + strTemp[4] + strTemp[5] + strTemp[6] + strTemp[7];
            return strReturn;
        }
        private async Task<float[]> WorkMomentDay(List<Lineas> lLines, int iCodEnt)//, DateTime dtIni, DateTime dtEnd
        {
            //devuelve un array con las horas trabajadas entre la hora inicio y fin segun su momneto del dia
            //    el indice del array marcha el momento del dia
            //0 = normal, 1 = noche, 2 = sabado, 3 = festivo
            float[] fReturn = new float[4];
            fReturn[0] = 0;
            fReturn[1] = 0;
            fReturn[2] = 0;
            fReturn[3] = 0;
            foreach (Lineas l in lLines)
            {
                float fHora0 = 0;
                float fHora1 = 0;
                float fHora2 = 0;
                float fHora3 = 0;
                //de cada linea calculo las hoas de cada franja
                //y añado al array de salida fReturn[0]=fReturn[0]+horas;

                var festivo = await aldakinDbContext.Diasfestivos.FirstOrDefaultAsync(x => x.Calendario == iCodEnt && x.Dia == l.Inicio.Date);
                //si es festivo??
                if ((!(festivo is null)) && (festivo.Jornadareducida == false))
                {
                    //es fectivo
                    fHora0 = 0;
                    fHora1 = 0;
                    fHora2 = 0;
                    fHora3 = Convert.ToSingle((l.Fin - l.Inicio).TotalHours);
                }
                else
                {
                    //es domingo??
                    if (l.Inicio.DayOfWeek == DayOfWeek.Sunday)
                    {
                        fHora0 = 0;
                        fHora1 = 0;
                        fHora2 = 0;
                        fHora3 = Convert.ToSingle((l.Fin - l.Inicio).TotalHours);
                    }
                    else
                    {
                        //es sabado
                        if (l.Inicio.DayOfWeek == DayOfWeek.Saturday)
                        {
                            //sabado
                            //horas de 00:00 a 06:00 festivo
                            //horas de 06:00 a 14:00 sabado
                            //horas de 14:00 a 24:00 festivo


                            //Inicio > 00:00 &&Inicio < 06:00 && fin <= 6:00 (OK)
                            if (l.Inicio.TimeOfDay >= new TimeSpan(0, 0, 0) && l.Inicio.TimeOfDay < new TimeSpan(6, 0, 0) && l.Fin.TimeOfDay <= new TimeSpan(6, 0, 0))
                            {
                                fHora0 = 0;
                                fHora1 = 0;
                                fHora2 = 0;
                                fHora3 = Convert.ToSingle((l.Fin - l.Inicio).TotalHours);
                            }

                            //Inicio > 00:00 &&Inicio < 06:00  && fin <= 14:00 (OK)
                            if (l.Inicio.TimeOfDay >= new TimeSpan(0, 0, 0) && l.Inicio.TimeOfDay < new TimeSpan(6, 0, 0) && l.Fin.TimeOfDay <= new TimeSpan(14, 0, 0))
                            {
                                fHora0 = 0;
                                fHora1 = 0;
                                fHora2 = Convert.ToSingle((new TimeSpan(14, 0, 0) - l.Fin.TimeOfDay).TotalHours);
                                fHora3 = Convert.ToSingle((new TimeSpan(6, 0, 0) - l.Inicio.TimeOfDay).TotalHours);
                            }

                            //Inicio > 00:00 &&Inicio < 06:00   && fin => 14:00 (OK)
                            if (l.Inicio.TimeOfDay >= new TimeSpan(0, 0, 0) && l.Inicio.TimeOfDay < new TimeSpan(6, 0, 0) && l.Fin.TimeOfDay >= new TimeSpan(14, 0, 0))
                            {
                                fHora0 = 0;
                                fHora1 = 0;
                                fHora2 = Convert.ToSingle((new TimeSpan(14, 0, 0) - new TimeSpan(6, 0, 0)).TotalHours);
                                fHora3 = Convert.ToSingle((((new TimeSpan(6, 0, 0) - l.Inicio.TimeOfDay).TotalHours)) + (((l.Fin.TimeOfDay - new TimeSpan(14, 0, 0)).TotalHours)));
                            }

                            //Inicio > 6:00 && fin <= 14:00 (OK)
                            if (l.Inicio.TimeOfDay >= new TimeSpan(6, 0, 0) && l.Fin.TimeOfDay <= new TimeSpan(14, 0, 0))
                            {
                                fHora0 = 0;
                                fHora1 = 0;
                                fHora2 = Convert.ToSingle((l.Fin.TimeOfDay - l.Inicio.TimeOfDay).TotalHours);
                                fHora3 = 0;
                            }

                            //Inicio > 6:00 && fin >= 14:00 (OK)
                            if (l.Inicio.TimeOfDay >= new TimeSpan(6, 0, 0) && l.Fin.TimeOfDay >= new TimeSpan(14, 0, 0))
                            {
                                fHora0 = 0;
                                fHora1 = 0;
                                fHora2 = Convert.ToSingle((new TimeSpan(14, 0, 0) - l.Inicio.TimeOfDay).TotalHours);
                                fHora3 = Convert.ToSingle((l.Fin.TimeOfDay - new TimeSpan(14, 0, 0)).TotalHours);
                            }

                            //Inicio > 14:00 && fin >= 14:00 (OK)
                            if (l.Inicio.TimeOfDay >= new TimeSpan(14, 0, 0) && l.Fin.TimeOfDay > new TimeSpan(14, 0, 0))
                            {
                                fHora0 = 0;
                                fHora1 = 0;
                                fHora2 = 0;
                                fHora3 = Convert.ToSingle((l.Fin - l.Inicio).TotalHours);
                            }

                        }
                        else
                        {
                            //es dia normal?
                            //horas de 00:00 a 06:00 nocturno
                            //horas de 06:00 a 22:00 normal
                            //horas de 22:00 a 24:00 nocturno

                            //Inicio > 00:00 &&Inicio < 06:00 && fin <= 6:00 (OK)
                            if (l.Inicio.TimeOfDay >= new TimeSpan(0, 0, 0) && l.Inicio.TimeOfDay < new TimeSpan(6, 0, 0) && l.Fin.TimeOfDay <= new TimeSpan(6, 0, 0))
                            {
                                fHora0 = 0;
                                fHora1 = Convert.ToSingle((l.Fin - l.Inicio).TotalHours);
                                fHora2 = 0;
                                fHora3 = 0;
                            }

                            //Inicio > 00:00 &&Inicio < 06:00  && fin <= 22:00 (OK)
                            if (l.Inicio.TimeOfDay >= new TimeSpan(0, 0, 0) && l.Inicio.TimeOfDay < new TimeSpan(6, 0, 0) && l.Fin.TimeOfDay <= new TimeSpan(22, 0, 0))
                            {
                                fHora0 = Convert.ToSingle((new TimeSpan(22, 0, 0) - l.Fin.TimeOfDay).TotalHours);
                                fHora1 = Convert.ToSingle((new TimeSpan(6, 0, 0) - l.Inicio.TimeOfDay).TotalHours);
                                fHora2 = 0;
                                fHora3 = 0;
                            }

                            //Inicio > 00:00 &&Inicio < 06:00   && fin => 22:00 (OK)
                            if (l.Inicio.TimeOfDay >= new TimeSpan(0, 0, 0) && l.Inicio.TimeOfDay < new TimeSpan(6, 0, 0) && l.Fin.TimeOfDay >= new TimeSpan(22, 0, 0))
                            {
                                fHora0 = Convert.ToSingle((new TimeSpan(22, 0, 0) - new TimeSpan(6, 0, 0)).TotalHours);
                                fHora1 = Convert.ToSingle((((new TimeSpan(6, 0, 0) - l.Inicio.TimeOfDay).TotalHours)) + (((l.Fin.TimeOfDay - new TimeSpan(22, 0, 0)).TotalHours)));
                                fHora2 = 0;
                                fHora3 = 0;
                            }

                            //Inicio > 6:00 && fin <= 22:00 (OK)
                            if (l.Inicio.TimeOfDay >= new TimeSpan(6, 0, 0) && l.Fin.TimeOfDay <= new TimeSpan(22, 0, 0))
                            {
                                fHora0 = Convert.ToSingle((l.Fin.TimeOfDay - l.Inicio.TimeOfDay).TotalHours);
                                fHora1 = 0;
                                fHora2 = 0;
                                fHora3 = 0;
                            }

                            //Inicio > 6:00 && fin >= 22:00 (OK)
                            if (l.Inicio.TimeOfDay >= new TimeSpan(6, 0, 0) && l.Fin.TimeOfDay >= new TimeSpan(22, 0, 0))
                            {
                                fHora0 = Convert.ToSingle((new TimeSpan(22, 0, 0) - l.Inicio.TimeOfDay).TotalHours);
                                fHora1 = Convert.ToSingle((l.Fin.TimeOfDay - new TimeSpan(22, 0, 0)).TotalHours);
                                fHora2 = 0;
                                fHora3 = 0;
                            }

                            //Inicio > 14:00 && fin >= 14:00 (OK)
                            if (l.Inicio.TimeOfDay >= new TimeSpan(22, 0, 0) && l.Fin.TimeOfDay > new TimeSpan(22, 0, 0))
                            {
                                fHora0 = 0;
                                fHora1 = Convert.ToSingle((l.Fin - l.Inicio).TotalHours);
                                fHora2 = 0;
                                fHora3 = 0;
                            }
                        }
                    }
                }

                fReturn[0] = fReturn[0] + fHora0;
                fReturn[1] = fReturn[1] + fHora1;
                fReturn[2] = fReturn[2] + fHora2;
                fReturn[3] = fReturn[3] + fHora3;
            }


            return fReturn;
        }


        //private int WorkMomentDay(DateTime dtIni,DateTime dtEnd)
        //{
        //    int iReturn = 0;

        //    #region Domingos
        //    if (dtIni.DayOfWeek == DayOfWeek.Sunday)
        //    {
        //        iReturn = 3;
        //    }
        //    #endregion


        //    #region Sabados
        //    else if (dtIni.DayOfWeek == DayOfWeek.Saturday)
        //    {
        //        #region Inicio < 6:00 && fin <= 14:00 (OK)
        //        if (dtIni.TimeOfDay < new TimeSpan(6, 0, 0) && dtEnd.TimeOfDay <= new TimeSpan(14, 0, 0) && dtEnd.TimeOfDay != new TimeSpan(0, 0, 0))
        //        {
        //            #region Acaba despues de las 6:00
        //            if (dtEnd.TimeOfDay > new TimeSpan(6, 0, 0))
        //            {
        //                //Parte diurna
        //                tmp[tmp.Length - 1].idLinea = lineas[i].idLinea;
        //                tmp[tmp.Length - 1].idOriginal = lineas[i].idOriginal;
        //                tmp[tmp.Length - 1].idOt = lineas[i].idOt;
        //                tmp[tmp.Length - 1].idpreslin = lineas[i].idpreslin;
        //                tmp[tmp.Length - 1].Inicio = new DateTime(lineas[i].Inicio.Year, lineas[i].Inicio.Month, lineas[i].Inicio.Day, 6, 0, 0);
        //                tmp[tmp.Length - 1].Fin = lineas[i].Fin;


        //                diurnas = (lineas[i].Fin.TimeOfDay - new TimeSpan(6, 0, 0)).TotalHours;


        //                if (lineas[i].Horas == 0)
        //                {
        //                    tmp[tmp.Length - 1].HorasViaje = (decimal)diurnas;
        //                }
        //                else if (lineas[i].HorasViaje == 0)
        //                {
        //                    tmp[tmp.Length - 1].Horas = (decimal)diurnas;
        //                }
        //                if (myArrayFestivos != null)
        //                {
        //                    if (dtIni.DayOfWeek == DayOfWeek.Saturday) iReturn = 2;//"Sabado";
        //                    else if (dtIni.DayOfWeek == DayOfWeek.Sunday || festivoActual.Festivo) iReturn = 3;
        //                    else tmp[tmp.Length - 1].momento = 0;

        //                }
        //                else if (myArrayFestivos == null)
        //                {
        //                    if (lineas[i].Inicio.DayOfWeek == DayOfWeek.Saturday) tmp[tmp.Length - 1].momento = 2;//"Sabado";
        //                    else if (lineas[i].Inicio.DayOfWeek == DayOfWeek.Sunday) tmp[tmp.Length - 1].momento = 3;
        //                    else tmp[tmp.Length - 1].momento = 0;
        //                }

        //                tmp[tmp.Length - 1].Pernocta = lineas[i].Pernocta;
        //                tmp[tmp.Length - 1].NParteFirmado = lineas[i].NParteFirmado;
        //                tmp[tmp.Length - 1].Observaciones = lineas[i].Observaciones.ToUpper();

        //                //Parte nocrutna
        //                Array.Resize(ref tmp, tmp.Length + 1);
        //                tmp[tmp.Length - 1] = new Linea();
        //                tmp[tmp.Length - 1].idLinea = -2;
        //                tmp[tmp.Length - 1].idOriginal = -2;
        //                tmp[tmp.Length - 1].idOt = lineas[i].idOt;
        //                tmp[tmp.Length - 1].idpreslin = lineas[i].idpreslin;
        //                tmp[tmp.Length - 1].Inicio = lineas[i].Inicio;
        //                tmp[tmp.Length - 1].Fin = new DateTime(lineas[i].Inicio.Year, lineas[i].Inicio.Month, lineas[i].Inicio.Day, 6, 0, 0);


        //                nocturnas = (new TimeSpan(6, 0, 0) - lineas[i].Inicio.TimeOfDay).TotalHours;


        //                if (lineas[i].Horas == 0)
        //                {
        //                    tmp[tmp.Length - 1].HorasViaje = (decimal)nocturnas;
        //                }
        //                else if (lineas[i].HorasViaje == 0)
        //                {
        //                    tmp[tmp.Length - 1].Horas = (decimal)nocturnas;
        //                }
        //                tmp[tmp.Length - 1].momento = 1; //Nocturna

        //                tmp[tmp.Length - 1].Pernocta = lineas[i].Pernocta;
        //                tmp[tmp.Length - 1].NParteFirmado = lineas[i].NParteFirmado;
        //                tmp[tmp.Length - 1].Observaciones = lineas[i].Observaciones.ToUpper();
        //            }
        //            #endregion
        //            #region Acaba antes de las 6:00
        //            else if (lineas[i].Fin.TimeOfDay <= new TimeSpan(6, 0, 0))
        //            {
        //                tmp[tmp.Length - 1].idLinea = lineas[i].idLinea;
        //                tmp[tmp.Length - 1].idOriginal = lineas[i].idOriginal;
        //                tmp[tmp.Length - 1].idOt = lineas[i].idOt;
        //                tmp[tmp.Length - 1].idpreslin = lineas[i].idpreslin;
        //                tmp[tmp.Length - 1].Inicio = lineas[i].Inicio;
        //                tmp[tmp.Length - 1].Fin = lineas[i].Fin;
        //                tmp[tmp.Length - 1].HorasViaje = lineas[i].HorasViaje;
        //                tmp[tmp.Length - 1].Horas = lineas[i].Horas;

        //                tmp[tmp.Length - 1].momento = 1; //Nocturna
        //                tmp[tmp.Length - 1].Pernocta = lineas[i].Pernocta;
        //                tmp[tmp.Length - 1].NParteFirmado = lineas[i].NParteFirmado;
        //                tmp[tmp.Length - 1].Observaciones = lineas[i].Observaciones.ToUpper();

        //            }
        //            #endregion
        //        }
        //        #endregion

        //        #region Inicio >= 6:00 && Fin <= 14:00 (OK)
        //        else if (lineas[i].Inicio.TimeOfDay >= new TimeSpan(6, 0, 0) && lineas[i].Fin.TimeOfDay <= new TimeSpan(14, 0, 0) && lineas[i].Fin.TimeOfDay != new TimeSpan(0, 0, 0))
        //        {
        //            tmp[tmp.Length - 1].Inicio = lineas[i].Inicio;
        //            tmp[tmp.Length - 1].Fin = lineas[i].Fin;
        //            tmp[tmp.Length - 1].idLinea = lineas[i].idLinea;
        //            tmp[tmp.Length - 1].idOriginal = lineas[i].idOriginal;
        //            tmp[tmp.Length - 1].idOt = lineas[i].idOt;
        //            tmp[tmp.Length - 1].idpreslin = lineas[i].idpreslin;
        //            tmp[tmp.Length - 1].Horas = lineas[i].Horas;
        //            tmp[tmp.Length - 1].HorasViaje = lineas[i].HorasViaje;


        //            if (myArrayFestivos != null)
        //            {
        //                if (lineas[i].Inicio.DayOfWeek == DayOfWeek.Saturday) tmp[tmp.Length - 1].momento = 2;//"Sabado";
        //                else if (lineas[i].Inicio.DayOfWeek == DayOfWeek.Sunday || festivoActual.Festivo) tmp[tmp.Length - 1].momento = 3;
        //                else tmp[tmp.Length - 1].momento = 0;

        //            }
        //            else if (myArrayFestivos == null)
        //            {
        //                if (lineas[i].Inicio.DayOfWeek == DayOfWeek.Saturday) tmp[tmp.Length - 1].momento = 2;//"Sabado";
        //                else if (lineas[i].Inicio.DayOfWeek == DayOfWeek.Sunday) tmp[tmp.Length - 1].momento = 3;
        //                else tmp[tmp.Length - 1].momento = 0;
        //            }
        //            tmp[tmp.Length - 1].Pernocta = lineas[i].Pernocta;
        //            tmp[tmp.Length - 1].NParteFirmado = lineas[i].NParteFirmado;
        //            tmp[tmp.Length - 1].Observaciones = lineas[i].Observaciones.ToUpper();

        //        }

        //        //Sustituir hasta aqui
        //        #endregion

        //        #region Inicio >= 6:00 && Inicio < 14:00 && fin > 14:00 && fin <= 22:00
        //        else if (lineas[i].Fin.TimeOfDay > new TimeSpan(14, 0, 0) && lineas[i].Fin.TimeOfDay <= new TimeSpan(22, 0, 0) && lineas[i].Inicio.TimeOfDay >= new TimeSpan(6, 0, 0) && lineas[i].Inicio.TimeOfDay < new TimeSpan(14, 0, 0))
        //        {
        //            #region Empieza antes de las 14:00
        //            if (lineas[i].Inicio.TimeOfDay < new TimeSpan(14, 0, 0))
        //            {
        //                //Parte diurna
        //                tmp[tmp.Length - 1].idLinea = lineas[i].idLinea;
        //                tmp[tmp.Length - 1].idOriginal = lineas[i].idOriginal;
        //                tmp[tmp.Length - 1].idOt = lineas[i].idOt;
        //                tmp[tmp.Length - 1].idpreslin = lineas[i].idpreslin;
        //                tmp[tmp.Length - 1].Inicio = lineas[i].Inicio;
        //                tmp[tmp.Length - 1].Fin = new DateTime(lineas[i].Inicio.Year, lineas[i].Inicio.Month, lineas[i].Inicio.Day, 14, 0, 0);


        //                diurnas = (new TimeSpan(14, 0, 0) - lineas[i].Inicio.TimeOfDay).TotalHours;


        //                if (lineas[i].Horas == 0)
        //                {
        //                    tmp[tmp.Length - 1].HorasViaje = (decimal)diurnas;
        //                }
        //                else if (lineas[i].HorasViaje == 0)
        //                {
        //                    tmp[tmp.Length - 1].Horas = (decimal)diurnas;
        //                }


        //                if (myArrayFestivos != null)
        //                {
        //                    if (lineas[i].Inicio.DayOfWeek == DayOfWeek.Saturday) tmp[tmp.Length - 1].momento = 2;//"Sabado";
        //                    else if (lineas[i].Inicio.DayOfWeek == DayOfWeek.Sunday || festivoActual.Festivo) tmp[tmp.Length - 1].momento = 3;
        //                    else tmp[tmp.Length - 1].momento = 0;

        //                }
        //                else if (myArrayFestivos == null)
        //                {
        //                    if (lineas[i].Inicio.DayOfWeek == DayOfWeek.Saturday) tmp[tmp.Length - 1].momento = 2;//"Sabado";
        //                    else if (lineas[i].Inicio.DayOfWeek == DayOfWeek.Sunday) tmp[tmp.Length - 1].momento = 3;
        //                    else tmp[tmp.Length - 1].momento = 0;
        //                }

        //                tmp[tmp.Length - 1].Pernocta = lineas[i].Pernocta;
        //                tmp[tmp.Length - 1].NParteFirmado = lineas[i].NParteFirmado;
        //                tmp[tmp.Length - 1].Observaciones = lineas[i].Observaciones.ToUpper();

        //                //Parte festiva
        //                Array.Resize(ref tmp, tmp.Length + 1);
        //                tmp[tmp.Length - 1] = new Linea();
        //                tmp[tmp.Length - 1].idLinea = -2;
        //                tmp[tmp.Length - 1].idOriginal = -2;
        //                tmp[tmp.Length - 1].idOt = lineas[i].idOt;
        //                tmp[tmp.Length - 1].idpreslin = lineas[i].idpreslin;
        //                tmp[tmp.Length - 1].Inicio = new DateTime(lineas[i].Inicio.Year, lineas[i].Inicio.Month, lineas[i].Inicio.Day, 14, 0, 0);
        //                tmp[tmp.Length - 1].Fin = lineas[i].Fin;


        //                nocturnas = (lineas[i].Fin.TimeOfDay - new TimeSpan(14, 0, 0)).TotalHours;


        //                if (lineas[i].Horas == 0)
        //                {
        //                    tmp[tmp.Length - 1].HorasViaje = (decimal)nocturnas;
        //                }
        //                else if (lineas[i].HorasViaje == 0)
        //                {
        //                    tmp[tmp.Length - 1].Horas = (decimal)nocturnas;
        //                }
        //                tmp[tmp.Length - 1].momento = 3; //festiva

        //                tmp[tmp.Length - 1].Pernocta = lineas[i].Pernocta;
        //                tmp[tmp.Length - 1].NParteFirmado = lineas[i].NParteFirmado;
        //                tmp[tmp.Length - 1].Observaciones = lineas[i].Observaciones.ToUpper();
        //            }
        //            #endregion
        //        }
        //        #endregion

        //        #region Inicio < 6:00 && fin > 14:00 && fin < 22:00
        //        else if (lineas[i].Fin.TimeOfDay > new TimeSpan(14, 0, 0) && lineas[i].Fin.TimeOfDay <= new TimeSpan(22, 0, 0) && lineas[i].Inicio.TimeOfDay < new TimeSpan(6, 0, 0))
        //        {

        //            //Parte nocturna (Mañana)                   
        //            tmp[tmp.Length - 1].idLinea = lineas[i].idLinea;
        //            tmp[tmp.Length - 1].idOriginal = lineas[i].idOriginal;
        //            tmp[tmp.Length - 1].idOt = lineas[i].idOt;
        //            tmp[tmp.Length - 1].idpreslin = lineas[i].idpreslin;
        //            tmp[tmp.Length - 1].Inicio = lineas[i].Inicio;
        //            tmp[tmp.Length - 1].Fin = new DateTime(lineas[i].Inicio.Year, lineas[i].Inicio.Month, lineas[i].Inicio.Day, 6, 0, 0);


        //            nocturnas = (new TimeSpan(6, 0, 0) - lineas[i].Inicio.TimeOfDay).TotalHours;


        //            if (lineas[i].Horas == 0)
        //            {
        //                tmp[tmp.Length - 1].HorasViaje = (decimal)nocturnas;
        //            }
        //            else if (lineas[i].HorasViaje == 0)
        //            {
        //                tmp[tmp.Length - 1].Horas = (decimal)nocturnas;
        //            }
        //            tmp[tmp.Length - 1].momento = 1; //Nocturna

        //            tmp[tmp.Length - 1].Pernocta = lineas[i].Pernocta;
        //            tmp[tmp.Length - 1].NParteFirmado = lineas[i].NParteFirmado;
        //            tmp[tmp.Length - 1].Observaciones = lineas[i].Observaciones.ToUpper();

        //            //Parte diurna
        //            Array.Resize(ref tmp, tmp.Length + 1);
        //            tmp[tmp.Length - 1] = new Linea();
        //            tmp[tmp.Length - 1].idLinea = -2;
        //            tmp[tmp.Length - 1].idOriginal = -2;
        //            tmp[tmp.Length - 1].idOt = lineas[i].idOt;
        //            tmp[tmp.Length - 1].idpreslin = lineas[i].idpreslin;
        //            tmp[tmp.Length - 1].Inicio = new DateTime(lineas[i].Inicio.Year, lineas[i].Inicio.Month, lineas[i].Inicio.Day, 6, 0, 0);
        //            tmp[tmp.Length - 1].Fin = new DateTime(lineas[i].Inicio.Year, lineas[i].Inicio.Month, lineas[i].Inicio.Day, 14, 0, 0);


        //            diurnas = 8.0;


        //            if (lineas[i].Horas == 0)
        //            {
        //                tmp[tmp.Length - 1].HorasViaje = (decimal)diurnas;
        //            }
        //            else if (lineas[i].HorasViaje == 0)
        //            {
        //                tmp[tmp.Length - 1].Horas = (decimal)diurnas;
        //            }


        //            if (myArrayFestivos != null)
        //            {
        //                if (lineas[i].Inicio.DayOfWeek == DayOfWeek.Saturday) tmp[tmp.Length - 1].momento = 2;//"Sabado";
        //                else if (lineas[i].Inicio.DayOfWeek == DayOfWeek.Sunday || festivoActual.Festivo) tmp[tmp.Length - 1].momento = 3;
        //                else tmp[tmp.Length - 1].momento = 0;

        //            }
        //            else if (myArrayFestivos == null)
        //            {
        //                if (lineas[i].Inicio.DayOfWeek == DayOfWeek.Saturday) tmp[tmp.Length - 1].momento = 2;//"Sabado";
        //                else if (lineas[i].Inicio.DayOfWeek == DayOfWeek.Sunday) tmp[tmp.Length - 1].momento = 3;
        //                else tmp[tmp.Length - 1].momento = 0;
        //            }

        //            tmp[tmp.Length - 1].Pernocta = lineas[i].Pernocta;
        //            tmp[tmp.Length - 1].NParteFirmado = lineas[i].NParteFirmado;
        //            tmp[tmp.Length - 1].Observaciones = lineas[i].Observaciones.ToUpper();

        //            //Parte nocturna (Tarde)
        //            Array.Resize(ref tmp, tmp.Length + 1);
        //            tmp[tmp.Length - 1] = new Linea();
        //            tmp[tmp.Length - 1].idLinea = -2;
        //            tmp[tmp.Length - 1].idOriginal = -2;
        //            tmp[tmp.Length - 1].idOt = lineas[i].idOt;
        //            tmp[tmp.Length - 1].idpreslin = lineas[i].idpreslin;
        //            tmp[tmp.Length - 1].Inicio = new DateTime(lineas[i].Inicio.Year, lineas[i].Inicio.Month, lineas[i].Inicio.Day, 14, 0, 0);
        //            tmp[tmp.Length - 1].Fin = lineas[i].Fin;

        //            nocturnas = (lineas[i].Fin.TimeOfDay - new TimeSpan(14, 0, 0)).TotalHours;


        //            if (lineas[i].Horas == 0)
        //            {
        //                tmp[tmp.Length - 1].HorasViaje = (decimal)nocturnas;
        //            }
        //            else if (lineas[i].HorasViaje == 0)
        //            {
        //                tmp[tmp.Length - 1].Horas = (decimal)nocturnas;
        //            }
        //            tmp[tmp.Length - 1].momento = 3; //Nocturna

        //            tmp[tmp.Length - 1].Pernocta = lineas[i].Pernocta;
        //            tmp[tmp.Length - 1].NParteFirmado = lineas[i].NParteFirmado;
        //            tmp[tmp.Length - 1].Observaciones = lineas[i].Observaciones.ToUpper();

        //        }
        //        #endregion

        //        #region Inicio >= 14:00 && fin < 22:00
        //        else if (lineas[i].Inicio.TimeOfDay >= new TimeSpan(14, 0, 0) && lineas[i].Inicio.TimeOfDay <= new TimeSpan(22, 0, 0) && lineas[i].Fin.TimeOfDay <= new TimeSpan(22, 0, 0))
        //        {
        //            #region Empieza despues de 14:00
        //            if (lineas[i].Inicio.TimeOfDay >= new TimeSpan(14, 0, 0))
        //            {
        //                tmp[tmp.Length - 1].idLinea = lineas[i].idLinea;
        //                tmp[tmp.Length - 1].idOriginal = lineas[i].idOriginal;
        //                tmp[tmp.Length - 1].idOt = lineas[i].idOt;
        //                tmp[tmp.Length - 1].idpreslin = lineas[i].idpreslin;
        //                tmp[tmp.Length - 1].Inicio = lineas[i].Inicio;
        //                tmp[tmp.Length - 1].Fin = lineas[i].Fin;


        //                nocturnas = (lineas[i].Fin.TimeOfDay - lineas[i].Inicio.TimeOfDay).TotalHours;


        //                if (lineas[i].Horas == 0)
        //                {
        //                    tmp[tmp.Length - 1].HorasViaje = (decimal)nocturnas;
        //                }
        //                else if (lineas[i].HorasViaje == 0)
        //                {
        //                    tmp[tmp.Length - 1].Horas = (decimal)nocturnas;
        //                }
        //                tmp[tmp.Length - 1].momento = 3; //Festiva
        //                tmp[tmp.Length - 1].Pernocta = lineas[i].Pernocta;
        //                tmp[tmp.Length - 1].NParteFirmado = lineas[i].NParteFirmado;
        //                tmp[tmp.Length - 1].Observaciones = lineas[i].Observaciones.ToUpper();

        //            }
        //            #endregion
        //        }
        //        #endregion

        //        #region Inicio >= 0:00 && fin > 22:00 
        //        else if (lineas[i].Inicio.TimeOfDay >= new TimeSpan(0, 0, 0) && (lineas[i].Fin.TimeOfDay > new TimeSpan(22, 0, 0) || lineas[i].Fin.TimeOfDay == new TimeSpan(0, 0, 0)))
        //        {
        //            #region Empieza antes de las 6:00
        //            if (lineas[i].Inicio.TimeOfDay < new TimeSpan(6, 0, 0))
        //            {
        //                //Parte nocturna (Mañana)
        //                tmp[tmp.Length - 1].idLinea = lineas[i].idLinea;
        //                tmp[tmp.Length - 1].idOriginal = lineas[i].idOriginal;
        //                tmp[tmp.Length - 1].idOt = lineas[i].idOt;
        //                tmp[tmp.Length - 1].idpreslin = lineas[i].idpreslin;
        //                tmp[tmp.Length - 1].Inicio = lineas[i].Inicio;
        //                tmp[tmp.Length - 1].Fin = new DateTime(lineas[i].Inicio.Year, lineas[i].Inicio.Month, lineas[i].Inicio.Day, 6, 0, 0);

        //                nocturnas = (new TimeSpan(6, 0, 0) - lineas[i].Inicio.TimeOfDay).TotalHours;


        //                if (lineas[i].Horas == 0)
        //                {
        //                    tmp[tmp.Length - 1].HorasViaje = (decimal)nocturnas;
        //                }
        //                else if (lineas[i].HorasViaje == 0)
        //                {
        //                    tmp[tmp.Length - 1].Horas = (decimal)nocturnas;
        //                }
        //                tmp[tmp.Length - 1].momento = 1;//"Nocturna";    

        //                tmp[tmp.Length - 1].Pernocta = lineas[i].Pernocta;
        //                tmp[tmp.Length - 1].NParteFirmado = lineas[i].NParteFirmado;
        //                tmp[tmp.Length - 1].Observaciones = lineas[i].Observaciones.ToUpper();

        //                //Parte sabado
        //                tmp[tmp.Length - 1].idLinea = lineas[i].idLinea;
        //                tmp[tmp.Length - 1].idOriginal = lineas[i].idOriginal;
        //                tmp[tmp.Length - 1].idOt = lineas[i].idOt;
        //                tmp[tmp.Length - 1].idpreslin = lineas[i].idpreslin;
        //                tmp[tmp.Length - 1].Inicio = new DateTime(lineas[i].Inicio.Year, lineas[i].Inicio.Month, lineas[i].Inicio.Day, 6, 0, 0);
        //                tmp[tmp.Length - 1].Fin = new DateTime(lineas[i].Inicio.Year, lineas[i].Inicio.Month, lineas[i].Inicio.Day, 14, 0, 0);

        //                diurnas = 8.0;


        //                if (lineas[i].Horas == 0)
        //                {
        //                    tmp[tmp.Length - 1].HorasViaje = (decimal)diurnas;
        //                }
        //                else if (lineas[i].HorasViaje == 0)
        //                {
        //                    tmp[tmp.Length - 1].Horas = (decimal)diurnas;
        //                }
        //                tmp[tmp.Length - 1].momento = 2;//"Sabado";    

        //                tmp[tmp.Length - 1].Pernocta = lineas[i].Pernocta;
        //                tmp[tmp.Length - 1].NParteFirmado = lineas[i].NParteFirmado;
        //                tmp[tmp.Length - 1].Observaciones = lineas[i].Observaciones.ToUpper();

        //                //Parte Festiva
        //                Array.Resize(ref tmp, tmp.Length + 1);
        //                tmp[tmp.Length - 1] = new Linea();
        //                tmp[tmp.Length - 1].idLinea = -2;
        //                tmp[tmp.Length - 1].idOriginal = -2;
        //                tmp[tmp.Length - 1].idOt = lineas[i].idOt;
        //                tmp[tmp.Length - 1].idpreslin = lineas[i].idpreslin;
        //                tmp[tmp.Length - 1].Inicio = new DateTime(lineas[i].Inicio.Year, lineas[i].Inicio.Month, lineas[i].Inicio.Day, 14, 0, 0);
        //                tmp[tmp.Length - 1].Fin = new DateTime(lineas[i].Inicio.Year, lineas[i].Inicio.Month, lineas[i].Inicio.Day, 22, 0, 0);



        //                nocturnas = 8.0;


        //                if (lineas[i].Horas == 0)
        //                {
        //                    tmp[tmp.Length - 1].HorasViaje = (decimal)nocturnas;
        //                }
        //                else if (lineas[i].HorasViaje == 0)
        //                {
        //                    tmp[tmp.Length - 1].Horas = (decimal)nocturnas;
        //                }
        //                tmp[tmp.Length - 1].momento = 3; //Festiva

        //                tmp[tmp.Length - 1].Pernocta = lineas[i].Pernocta;
        //                tmp[tmp.Length - 1].NParteFirmado = lineas[i].NParteFirmado;
        //                tmp[tmp.Length - 1].Observaciones = lineas[i].Observaciones.ToUpper();

        //                //Parte nocrutna (Tarde)
        //                Array.Resize(ref tmp, tmp.Length + 1);
        //                tmp[tmp.Length - 1] = new Linea();
        //                tmp[tmp.Length - 1].idLinea = -2;
        //                tmp[tmp.Length - 1].idOriginal = -2;
        //                tmp[tmp.Length - 1].idOt = lineas[i].idOt;
        //                tmp[tmp.Length - 1].idpreslin = lineas[i].idpreslin;
        //                tmp[tmp.Length - 1].Inicio = new DateTime(lineas[i].Inicio.Year, lineas[i].Inicio.Month, lineas[i].Inicio.Day, 22, 0, 0);
        //                tmp[tmp.Length - 1].Fin = lineas[i].Fin;


        //                nocturnas = (tmp[tmp.Length - 1].Fin - tmp[tmp.Length - 1].Inicio).TotalHours;


        //                if (lineas[i].Horas == 0)
        //                {
        //                    tmp[tmp.Length - 1].HorasViaje = (decimal)nocturnas;
        //                }
        //                else if (lineas[i].HorasViaje == 0)
        //                {
        //                    tmp[tmp.Length - 1].Horas = (decimal)nocturnas;
        //                }
        //                tmp[tmp.Length - 1].momento = 1; //Nocturna

        //                tmp[tmp.Length - 1].Pernocta = lineas[i].Pernocta;
        //                tmp[tmp.Length - 1].NParteFirmado = lineas[i].NParteFirmado;
        //                tmp[tmp.Length - 1].Observaciones = lineas[i].Observaciones.ToUpper();
        //            }
        //            #endregion

        //            #region Empieza antes de las 14:00
        //            else if (lineas[i].Inicio.TimeOfDay < new TimeSpan(14, 0, 0))
        //            {
        //                //Parte sabado
        //                tmp[tmp.Length - 1].idLinea = lineas[i].idLinea;
        //                tmp[tmp.Length - 1].idOriginal = lineas[i].idOriginal;
        //                tmp[tmp.Length - 1].idOt = lineas[i].idOt;
        //                tmp[tmp.Length - 1].idpreslin = lineas[i].idpreslin;
        //                tmp[tmp.Length - 1].Inicio = lineas[i].Inicio;
        //                tmp[tmp.Length - 1].Fin = new DateTime(lineas[i].Inicio.Year, lineas[i].Inicio.Month, lineas[i].Inicio.Day, 14, 0, 0);

        //                diurnas = (new TimeSpan(14, 0, 0) - lineas[i].Inicio.TimeOfDay).TotalHours;


        //                if (lineas[i].Horas == 0)
        //                {
        //                    tmp[tmp.Length - 1].HorasViaje = (decimal)diurnas;
        //                }
        //                else if (lineas[i].HorasViaje == 0)
        //                {
        //                    tmp[tmp.Length - 1].Horas = (decimal)diurnas;
        //                }
        //                tmp[tmp.Length - 1].momento = 2;//"Sabado";    

        //                tmp[tmp.Length - 1].Pernocta = lineas[i].Pernocta;
        //                tmp[tmp.Length - 1].NParteFirmado = lineas[i].NParteFirmado;
        //                tmp[tmp.Length - 1].Observaciones = lineas[i].Observaciones.ToUpper();

        //                //Parte Festiva
        //                Array.Resize(ref tmp, tmp.Length + 1);
        //                tmp[tmp.Length - 1] = new Linea();
        //                tmp[tmp.Length - 1].idLinea = -2;
        //                tmp[tmp.Length - 1].idOriginal = -2;
        //                tmp[tmp.Length - 1].idOt = lineas[i].idOt;
        //                tmp[tmp.Length - 1].idpreslin = lineas[i].idpreslin;
        //                tmp[tmp.Length - 1].Inicio = new DateTime(lineas[i].Inicio.Year, lineas[i].Inicio.Month, lineas[i].Inicio.Day, 14, 0, 0);
        //                tmp[tmp.Length - 1].Fin = new DateTime(lineas[i].Inicio.Year, lineas[i].Inicio.Month, lineas[i].Inicio.Day, 22, 0, 0);



        //                nocturnas = 8.0;


        //                if (lineas[i].Horas == 0)
        //                {
        //                    tmp[tmp.Length - 1].HorasViaje = (decimal)nocturnas;
        //                }
        //                else if (lineas[i].HorasViaje == 0)
        //                {
        //                    tmp[tmp.Length - 1].Horas = (decimal)nocturnas;
        //                }
        //                tmp[tmp.Length - 1].momento = 3; //FEstiva

        //                tmp[tmp.Length - 1].Pernocta = lineas[i].Pernocta;
        //                tmp[tmp.Length - 1].NParteFirmado = lineas[i].NParteFirmado;
        //                tmp[tmp.Length - 1].Observaciones = lineas[i].Observaciones.ToUpper();

        //                //Parte nocrutna
        //                Array.Resize(ref tmp, tmp.Length + 1);
        //                tmp[tmp.Length - 1] = new Linea();
        //                tmp[tmp.Length - 1].idLinea = -2;
        //                tmp[tmp.Length - 1].idOriginal = -2;
        //                tmp[tmp.Length - 1].idOt = lineas[i].idOt;
        //                tmp[tmp.Length - 1].idpreslin = lineas[i].idpreslin;
        //                tmp[tmp.Length - 1].Inicio = new DateTime(lineas[i].Inicio.Year, lineas[i].Inicio.Month, lineas[i].Inicio.Day, 22, 0, 0);
        //                tmp[tmp.Length - 1].Fin = lineas[i].Fin;


        //                if (lineas[i].Fin.TimeOfDay == new TimeSpan(0, 0, 0)) nocturnas = 2.0;
        //                else nocturnas = (lineas[i].Fin.TimeOfDay - new TimeSpan(22, 0, 0)).TotalHours;


        //                if (lineas[i].Horas == 0)
        //                {
        //                    tmp[tmp.Length - 1].HorasViaje = (decimal)nocturnas;
        //                }
        //                else if (lineas[i].HorasViaje == 0)
        //                {
        //                    tmp[tmp.Length - 1].Horas = (decimal)nocturnas;
        //                }
        //                tmp[tmp.Length - 1].momento = 1; //Nocturna

        //                tmp[tmp.Length - 1].Pernocta = lineas[i].Pernocta;
        //                tmp[tmp.Length - 1].NParteFirmado = lineas[i].NParteFirmado;
        //                tmp[tmp.Length - 1].Observaciones = lineas[i].Observaciones.ToUpper();
        //            }
        //            #endregion

        //            #region Empieza antes de las 22:00
        //            else if (lineas[i].Inicio.TimeOfDay < new TimeSpan(22, 0, 0))
        //            {
        //                //Parte diurna
        //                tmp[tmp.Length - 1].idLinea = lineas[i].idLinea;
        //                tmp[tmp.Length - 1].idOriginal = lineas[i].idOriginal;
        //                tmp[tmp.Length - 1].idOt = lineas[i].idOt;
        //                tmp[tmp.Length - 1].idpreslin = lineas[i].idpreslin;
        //                tmp[tmp.Length - 1].Inicio = lineas[i].Inicio;
        //                tmp[tmp.Length - 1].Fin = new DateTime(lineas[i].Inicio.Year, lineas[i].Inicio.Month, lineas[i].Inicio.Day, 22, 0, 0);

        //                diurnas = (new TimeSpan(22, 0, 0) - lineas[i].Inicio.TimeOfDay).TotalHours;


        //                if (lineas[i].Horas == 0)
        //                {
        //                    tmp[tmp.Length - 1].HorasViaje = (decimal)diurnas;
        //                }
        //                else if (lineas[i].HorasViaje == 0)
        //                {
        //                    tmp[tmp.Length - 1].Horas = (decimal)diurnas;
        //                }
        //                tmp[tmp.Length - 1].momento = 3;//"Festivo";    

        //                tmp[tmp.Length - 1].Pernocta = lineas[i].Pernocta;
        //                tmp[tmp.Length - 1].NParteFirmado = lineas[i].NParteFirmado;
        //                tmp[tmp.Length - 1].Observaciones = lineas[i].Observaciones.ToUpper();

        //                //Parte nocrutna
        //                Array.Resize(ref tmp, tmp.Length + 1);
        //                tmp[tmp.Length - 1] = new Linea();
        //                tmp[tmp.Length - 1].idLinea = -2;
        //                tmp[tmp.Length - 1].idOriginal = -2;
        //                tmp[tmp.Length - 1].idOt = lineas[i].idOt;
        //                tmp[tmp.Length - 1].idpreslin = lineas[i].idpreslin;
        //                tmp[tmp.Length - 1].Inicio = new DateTime(lineas[i].Inicio.Year, lineas[i].Inicio.Month, lineas[i].Inicio.Day, 22, 0, 0);
        //                tmp[tmp.Length - 1].Fin = lineas[i].Fin;


        //                if (lineas[i].Fin.TimeOfDay == new TimeSpan(0, 0, 0)) nocturnas = 2.0;
        //                else nocturnas = (lineas[i].Fin.TimeOfDay - new TimeSpan(22, 0, 0)).TotalHours;


        //                if (lineas[i].Horas == 0)
        //                {
        //                    tmp[tmp.Length - 1].HorasViaje = (decimal)nocturnas;
        //                }
        //                else if (lineas[i].HorasViaje == 0)
        //                {
        //                    tmp[tmp.Length - 1].Horas = (decimal)nocturnas;
        //                }
        //                tmp[tmp.Length - 1].momento = 1; //Nocturna

        //                tmp[tmp.Length - 1].Pernocta = lineas[i].Pernocta;
        //                tmp[tmp.Length - 1].NParteFirmado = lineas[i].NParteFirmado;
        //                tmp[tmp.Length - 1].Observaciones = lineas[i].Observaciones.ToUpper();
        //            }
        //            #endregion

        //            #region Empieza despues de las 22:00
        //            else if (lineas[i].Inicio.TimeOfDay >= new TimeSpan(22, 0, 0))
        //            {
        //                tmp[tmp.Length - 1].idLinea = lineas[i].idLinea;
        //                tmp[tmp.Length - 1].idOriginal = lineas[i].idOriginal;
        //                tmp[tmp.Length - 1].idOt = lineas[i].idOt;
        //                tmp[tmp.Length - 1].idpreslin = lineas[i].idpreslin;
        //                tmp[tmp.Length - 1].Inicio = lineas[i].Inicio;
        //                tmp[tmp.Length - 1].Fin = lineas[i].Fin;
        //                tmp[tmp.Length - 1].HorasViaje = lineas[i].HorasViaje;
        //                tmp[tmp.Length - 1].Horas = lineas[i].Horas;


        //                tmp[tmp.Length - 1].momento = 1; //Nocturna
        //                tmp[tmp.Length - 1].Pernocta = lineas[i].Pernocta;
        //                tmp[tmp.Length - 1].NParteFirmado = lineas[i].NParteFirmado;
        //                tmp[tmp.Length - 1].Observaciones = lineas[i].Observaciones.ToUpper();

        //            }
        //            #endregion
        //        }
        //        #endregion

        //    }
        //    #endregion

        //    #region Lunes a Viernes
        //    else
        //    {
        //        #region Inicio < 6:00 && fin <= 22:00 (OK)
        //        if (lineas[i].Inicio.TimeOfDay < new TimeSpan(6, 0, 0) && lineas[i].Fin.TimeOfDay <= new TimeSpan(22, 0, 0))
        //        {
        //            #region Acaba despues de las 6:00
        //            if (lineas[i].Fin.TimeOfDay > new TimeSpan(6, 0, 0))
        //            {
        //                //Parte diurna
        //                tmp[tmp.Length - 1].idLinea = lineas[i].idLinea;
        //                tmp[tmp.Length - 1].idOriginal = lineas[i].idOriginal;
        //                tmp[tmp.Length - 1].idOt = lineas[i].idOt;
        //                tmp[tmp.Length - 1].idpreslin = lineas[i].idpreslin;
        //                tmp[tmp.Length - 1].Inicio = new DateTime(lineas[i].Inicio.Year, lineas[i].Inicio.Month, lineas[i].Inicio.Day, 6, 0, 0);
        //                tmp[tmp.Length - 1].Fin = lineas[i].Fin;


        //                diurnas = (lineas[i].Fin.TimeOfDay - new TimeSpan(6, 0, 0)).TotalHours;


        //                if (lineas[i].Horas == 0)
        //                {
        //                    tmp[tmp.Length - 1].HorasViaje = (decimal)diurnas;
        //                }
        //                else if (lineas[i].HorasViaje == 0)
        //                {
        //                    tmp[tmp.Length - 1].Horas = (decimal)diurnas;
        //                }
        //                if (myArrayFestivos != null)
        //                {
        //                    if (lineas[i].Inicio.DayOfWeek == DayOfWeek.Saturday) tmp[tmp.Length - 1].momento = 2;//"Sabado";
        //                    else if (lineas[i].Inicio.DayOfWeek == DayOfWeek.Sunday || festivoActual.Festivo) tmp[tmp.Length - 1].momento = 3;
        //                    else tmp[tmp.Length - 1].momento = 0;

        //                }
        //                else if (myArrayFestivos == null)
        //                {
        //                    if (lineas[i].Inicio.DayOfWeek == DayOfWeek.Saturday) tmp[tmp.Length - 1].momento = 2;//"Sabado";
        //                    else if (lineas[i].Inicio.DayOfWeek == DayOfWeek.Sunday) tmp[tmp.Length - 1].momento = 3;
        //                    else tmp[tmp.Length - 1].momento = 0;
        //                }

        //                tmp[tmp.Length - 1].Pernocta = lineas[i].Pernocta;
        //                tmp[tmp.Length - 1].NParteFirmado = lineas[i].NParteFirmado;
        //                tmp[tmp.Length - 1].Observaciones = lineas[i].Observaciones.ToUpper();

        //                //Parte nocrutna
        //                Array.Resize(ref tmp, tmp.Length + 1);
        //                tmp[tmp.Length - 1] = new Linea();
        //                tmp[tmp.Length - 1].idLinea = -2;
        //                tmp[tmp.Length - 1].idOriginal = -2;
        //                tmp[tmp.Length - 1].idOt = lineas[i].idOt;
        //                tmp[tmp.Length - 1].idpreslin = lineas[i].idpreslin;
        //                tmp[tmp.Length - 1].Inicio = lineas[i].Inicio;
        //                tmp[tmp.Length - 1].Fin = new DateTime(lineas[i].Inicio.Year, lineas[i].Inicio.Month, lineas[i].Inicio.Day, 6, 0, 0);


        //                nocturnas = (new TimeSpan(6, 0, 0) - lineas[i].Inicio.TimeOfDay).TotalHours;


        //                if (lineas[i].Horas == 0)
        //                {
        //                    tmp[tmp.Length - 1].HorasViaje = (decimal)nocturnas;
        //                }
        //                else if (lineas[i].HorasViaje == 0)
        //                {
        //                    tmp[tmp.Length - 1].Horas = (decimal)nocturnas;
        //                }
        //                tmp[tmp.Length - 1].momento = 1; //Nocturna

        //                tmp[tmp.Length - 1].Pernocta = lineas[i].Pernocta;
        //                tmp[tmp.Length - 1].NParteFirmado = lineas[i].NParteFirmado;
        //                tmp[tmp.Length - 1].Observaciones = lineas[i].Observaciones.ToUpper();
        //            }
        //            #endregion
        //            #region Acaba antes de las 6:00
        //            else if (lineas[i].Fin.TimeOfDay <= new TimeSpan(6, 0, 0))
        //            {
        //                tmp[tmp.Length - 1].idLinea = lineas[i].idLinea;
        //                tmp[tmp.Length - 1].idOriginal = lineas[i].idOriginal;
        //                tmp[tmp.Length - 1].idOt = lineas[i].idOt;
        //                tmp[tmp.Length - 1].idpreslin = lineas[i].idpreslin;
        //                tmp[tmp.Length - 1].Inicio = lineas[i].Inicio;
        //                tmp[tmp.Length - 1].Fin = lineas[i].Fin;
        //                tmp[tmp.Length - 1].HorasViaje = lineas[i].HorasViaje;
        //                tmp[tmp.Length - 1].Horas = lineas[i].Horas;

        //                tmp[tmp.Length - 1].momento = 1; //Nocturna
        //                tmp[tmp.Length - 1].Pernocta = lineas[i].Pernocta;
        //                tmp[tmp.Length - 1].NParteFirmado = lineas[i].NParteFirmado;
        //                tmp[tmp.Length - 1].Observaciones = lineas[i].Observaciones.ToUpper();

        //            }
        //            #endregion
        //        }
        //        #endregion

        //        #region Inicio >= 6:00 && fin > 22:00 (OK)
        //        else if ((lineas[i].Fin.TimeOfDay > new TimeSpan(22, 0, 0) || lineas[i].Fin.TimeOfDay == new TimeSpan(0, 0, 0)) && lineas[i].Inicio.TimeOfDay >= new TimeSpan(6, 0, 0))
        //        {
        //            #region Empieza antes de las 22:00
        //            if (lineas[i].Inicio.TimeOfDay < new TimeSpan(22, 0, 0))
        //            {
        //                //Parte diurna
        //                tmp[tmp.Length - 1].idLinea = lineas[i].idLinea;
        //                tmp[tmp.Length - 1].idOriginal = lineas[i].idOriginal;
        //                tmp[tmp.Length - 1].idOt = lineas[i].idOt;
        //                tmp[tmp.Length - 1].idpreslin = lineas[i].idpreslin;
        //                tmp[tmp.Length - 1].Inicio = lineas[i].Inicio;
        //                tmp[tmp.Length - 1].Fin = new DateTime(lineas[i].Inicio.Year, lineas[i].Inicio.Month, lineas[i].Inicio.Day, 22, 0, 0);


        //                diurnas = (new TimeSpan(22, 0, 0) - lineas[i].Inicio.TimeOfDay).TotalHours;


        //                if (lineas[i].Horas == 0)
        //                {
        //                    tmp[tmp.Length - 1].HorasViaje = (decimal)diurnas;
        //                }
        //                else if (lineas[i].HorasViaje == 0)
        //                {
        //                    tmp[tmp.Length - 1].Horas = (decimal)diurnas;
        //                }


        //                if (myArrayFestivos != null)
        //                {
        //                    if (lineas[i].Inicio.DayOfWeek == DayOfWeek.Saturday) tmp[tmp.Length - 1].momento = 2;//"Sabado";
        //                    else if (lineas[i].Inicio.DayOfWeek == DayOfWeek.Sunday || festivoActual.Festivo) tmp[tmp.Length - 1].momento = 3;
        //                    else tmp[tmp.Length - 1].momento = 0;

        //                }
        //                else if (myArrayFestivos == null)
        //                {
        //                    if (lineas[i].Inicio.DayOfWeek == DayOfWeek.Saturday) tmp[tmp.Length - 1].momento = 2;//"Sabado";
        //                    else if (lineas[i].Inicio.DayOfWeek == DayOfWeek.Sunday) tmp[tmp.Length - 1].momento = 3;
        //                    else tmp[tmp.Length - 1].momento = 0;
        //                }

        //                tmp[tmp.Length - 1].Pernocta = lineas[i].Pernocta;
        //                tmp[tmp.Length - 1].NParteFirmado = lineas[i].NParteFirmado;
        //                tmp[tmp.Length - 1].Observaciones = lineas[i].Observaciones.ToUpper();

        //                //Parte nocturna
        //                Array.Resize(ref tmp, tmp.Length + 1);
        //                tmp[tmp.Length - 1] = new Linea();
        //                tmp[tmp.Length - 1].idLinea = -2;
        //                tmp[tmp.Length - 1].idOriginal = -2;
        //                tmp[tmp.Length - 1].idOt = lineas[i].idOt;
        //                tmp[tmp.Length - 1].idpreslin = lineas[i].idpreslin;
        //                tmp[tmp.Length - 1].Inicio = new DateTime(lineas[i].Inicio.Year, lineas[i].Inicio.Month, lineas[i].Inicio.Day, 22, 0, 0);
        //                tmp[tmp.Length - 1].Fin = lineas[i].Fin;

        //                nocturnas = (tmp[tmp.Length - 1].Fin - tmp[tmp.Length - 1].Inicio).TotalHours;



        //                if (lineas[i].Horas == 0)
        //                {
        //                    tmp[tmp.Length - 1].HorasViaje = (decimal)nocturnas;
        //                }
        //                else if (lineas[i].HorasViaje == 0)
        //                {
        //                    tmp[tmp.Length - 1].Horas = (decimal)nocturnas;
        //                }
        //                tmp[tmp.Length - 1].momento = 1; //Nocturna

        //                tmp[tmp.Length - 1].Pernocta = lineas[i].Pernocta;
        //                tmp[tmp.Length - 1].NParteFirmado = lineas[i].NParteFirmado;
        //                tmp[tmp.Length - 1].Observaciones = lineas[i].Observaciones.ToUpper();
        //            }
        //            #endregion
        //            #region Empieza despues de 22:00
        //            else if (lineas[i].Inicio.TimeOfDay >= new TimeSpan(22, 0, 0))
        //            {
        //                tmp[tmp.Length - 1].idLinea = lineas[i].idLinea;
        //                tmp[tmp.Length - 1].idOriginal = lineas[i].idOriginal;
        //                tmp[tmp.Length - 1].idOt = lineas[i].idOt;
        //                tmp[tmp.Length - 1].idpreslin = lineas[i].idpreslin;
        //                tmp[tmp.Length - 1].Inicio = lineas[i].Inicio;
        //                tmp[tmp.Length - 1].Fin = lineas[i].Fin;

        //                nocturnas = (tmp[tmp.Length - 1].Fin - tmp[tmp.Length - 1].Inicio).TotalHours;


        //                if (lineas[i].Horas == 0)
        //                {
        //                    tmp[tmp.Length - 1].HorasViaje = (decimal)nocturnas;
        //                }
        //                else if (lineas[i].HorasViaje == 0)
        //                {
        //                    tmp[tmp.Length - 1].Horas = (decimal)nocturnas;
        //                }
        //                tmp[tmp.Length - 1].momento = 1; //Nocturna
        //                tmp[tmp.Length - 1].Pernocta = lineas[i].Pernocta;
        //                tmp[tmp.Length - 1].NParteFirmado = lineas[i].NParteFirmado;
        //                tmp[tmp.Length - 1].Observaciones = lineas[i].Observaciones.ToUpper();

        //            }
        //            #endregion
        //        }
        //        #endregion

        //        #region Inicio < 6:00 && fin > 22:00
        //        else if ((lineas[i].Fin.TimeOfDay > new TimeSpan(22, 0, 0) || lineas[i].Fin.TimeOfDay == new TimeSpan(0, 0, 0)) && lineas[i].Inicio.TimeOfDay < new TimeSpan(6, 0, 0))
        //        {


        //            //Parte nocturna (Mañana)                   
        //            tmp[tmp.Length - 1].idLinea = lineas[i].idLinea;
        //            tmp[tmp.Length - 1].idOriginal = lineas[i].idOriginal;
        //            tmp[tmp.Length - 1].idOt = lineas[i].idOt;
        //            tmp[tmp.Length - 1].idpreslin = lineas[i].idpreslin;
        //            tmp[tmp.Length - 1].Inicio = lineas[i].Inicio;
        //            tmp[tmp.Length - 1].Fin = new DateTime(lineas[i].Inicio.Year, lineas[i].Inicio.Month, lineas[i].Inicio.Day, 6, 0, 0);


        //            nocturnas = (new TimeSpan(6, 0, 0) - lineas[i].Inicio.TimeOfDay).TotalHours;


        //            if (lineas[i].Horas == 0)
        //            {
        //                tmp[tmp.Length - 1].HorasViaje = (decimal)nocturnas;
        //            }
        //            else if (lineas[i].HorasViaje == 0)
        //            {
        //                tmp[tmp.Length - 1].Horas = (decimal)nocturnas;
        //            }
        //            tmp[tmp.Length - 1].momento = 1; //Nocturna

        //            tmp[tmp.Length - 1].Pernocta = lineas[i].Pernocta;
        //            tmp[tmp.Length - 1].NParteFirmado = lineas[i].NParteFirmado;
        //            tmp[tmp.Length - 1].Observaciones = lineas[i].Observaciones.ToUpper();

        //            //Parte diurna
        //            Array.Resize(ref tmp, tmp.Length + 1);
        //            tmp[tmp.Length - 1] = new Linea();
        //            tmp[tmp.Length - 1].idLinea = -2;
        //            tmp[tmp.Length - 1].idOriginal = -2;
        //            tmp[tmp.Length - 1].idOt = lineas[i].idOt;
        //            tmp[tmp.Length - 1].idpreslin = lineas[i].idpreslin;
        //            tmp[tmp.Length - 1].Inicio = new DateTime(lineas[i].Inicio.Year, lineas[i].Inicio.Month, lineas[i].Inicio.Day, 6, 0, 0);
        //            tmp[tmp.Length - 1].Fin = new DateTime(lineas[i].Inicio.Year, lineas[i].Inicio.Month, lineas[i].Inicio.Day, 22, 0, 0);


        //            diurnas = 16.0;


        //            if (lineas[i].Horas == 0)
        //            {
        //                tmp[tmp.Length - 1].HorasViaje = (decimal)diurnas;
        //            }
        //            else if (lineas[i].HorasViaje == 0)
        //            {
        //                tmp[tmp.Length - 1].Horas = (decimal)diurnas;
        //            }


        //            if (myArrayFestivos != null)
        //            {
        //                if (lineas[i].Inicio.DayOfWeek == DayOfWeek.Saturday) tmp[tmp.Length - 1].momento = 2;//"Sabado";
        //                else if (lineas[i].Inicio.DayOfWeek == DayOfWeek.Sunday || festivoActual.Festivo) tmp[tmp.Length - 1].momento = 3;
        //                else tmp[tmp.Length - 1].momento = 0;

        //            }
        //            else if (myArrayFestivos == null)
        //            {
        //                if (lineas[i].Inicio.DayOfWeek == DayOfWeek.Saturday) tmp[tmp.Length - 1].momento = 2;//"Sabado";
        //                else if (lineas[i].Inicio.DayOfWeek == DayOfWeek.Sunday) tmp[tmp.Length - 1].momento = 3;
        //                else tmp[tmp.Length - 1].momento = 0;
        //            }

        //            tmp[tmp.Length - 1].Pernocta = lineas[i].Pernocta;
        //            tmp[tmp.Length - 1].NParteFirmado = lineas[i].NParteFirmado;
        //            tmp[tmp.Length - 1].Observaciones = lineas[i].Observaciones.ToUpper();

        //            //Parte nocturna (Tarde)
        //            Array.Resize(ref tmp, tmp.Length + 1);
        //            tmp[tmp.Length - 1] = new Linea();
        //            tmp[tmp.Length - 1].idLinea = -2;
        //            tmp[tmp.Length - 1].idOriginal = -2;
        //            tmp[tmp.Length - 1].idOt = lineas[i].idOt;
        //            tmp[tmp.Length - 1].idpreslin = lineas[i].idpreslin;
        //            tmp[tmp.Length - 1].Inicio = new DateTime(lineas[i].Inicio.Year, lineas[i].Inicio.Month, lineas[i].Inicio.Day, 22, 0, 0);
        //            tmp[tmp.Length - 1].Fin = lineas[i].Fin;

        //            nocturnas = (tmp[tmp.Length - 1].Fin - tmp[tmp.Length - 1].Inicio).TotalHours;


        //            if (lineas[i].Horas == 0)
        //            {
        //                tmp[tmp.Length - 1].HorasViaje = (decimal)nocturnas;
        //            }
        //            else if (lineas[i].HorasViaje == 0)
        //            {
        //                tmp[tmp.Length - 1].Horas = (decimal)nocturnas;
        //            }
        //            tmp[tmp.Length - 1].momento = 1; //Nocturna

        //            tmp[tmp.Length - 1].Pernocta = lineas[i].Pernocta;
        //            tmp[tmp.Length - 1].NParteFirmado = lineas[i].NParteFirmado;
        //            tmp[tmp.Length - 1].Observaciones = lineas[i].Observaciones.ToUpper();

        //        }
        //        #endregion

        //        #region Inicio >= 6:00 && Fin <= 22:00 (OK)
        //        else if (lineas[i].Inicio.TimeOfDay >= new TimeSpan(6, 0, 0) && lineas[i].Inicio.TimeOfDay <= new TimeSpan(22, 0, 0) && lineas[i].Fin.TimeOfDay <= new TimeSpan(22, 0, 0))
        //        {
        //            tmp[tmp.Length - 1].Inicio = lineas[i].Inicio;
        //            tmp[tmp.Length - 1].Fin = lineas[i].Fin;
        //            tmp[tmp.Length - 1].idLinea = lineas[i].idLinea;
        //            tmp[tmp.Length - 1].idOriginal = lineas[i].idOriginal;
        //            tmp[tmp.Length - 1].idOt = lineas[i].idOt;
        //            tmp[tmp.Length - 1].idpreslin = lineas[i].idpreslin;
        //            tmp[tmp.Length - 1].Horas = lineas[i].Horas;
        //            tmp[tmp.Length - 1].HorasViaje = lineas[i].HorasViaje;


        //            if (myArrayFestivos != null)
        //            {
        //                if (lineas[i].Inicio.DayOfWeek == DayOfWeek.Saturday) tmp[tmp.Length - 1].momento = 2;//"Sabado";
        //                else if (lineas[i].Inicio.DayOfWeek == DayOfWeek.Sunday || festivoActual.Festivo) tmp[tmp.Length - 1].momento = 3;
        //                else tmp[tmp.Length - 1].momento = 0;

        //            }
        //            else if (myArrayFestivos == null)
        //            {
        //                if (lineas[i].Inicio.DayOfWeek == DayOfWeek.Saturday) tmp[tmp.Length - 1].momento = 2;//"Sabado";
        //                else if (lineas[i].Inicio.DayOfWeek == DayOfWeek.Sunday) tmp[tmp.Length - 1].momento = 3;
        //                else tmp[tmp.Length - 1].momento = 0;
        //            }
        //            tmp[tmp.Length - 1].Pernocta = lineas[i].Pernocta;
        //            tmp[tmp.Length - 1].NParteFirmado = lineas[i].NParteFirmado;
        //            tmp[tmp.Length - 1].Observaciones = lineas[i].Observaciones.ToUpper();

        //        }
        //        #endregion
        //    }
        //    #endregion

        //    return iReturn;
        //}

        //public FileResult PruebaArchivos()
        //{
        //    // Create a string array with the lines of text
        //    string[] lines = { "First line", "Second line", "Third line" };

        //    // Set a variable to the Documents path.
        //    string docPath =
        //      Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        //    // Write the string array to a new file named "WriteLines.txt".
        //    using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "WriteLines.txt")))
        //    {
        //        foreach (string line in lines)
        //            outputFile.WriteLine(line);
        //    }
        //    return File(docPath, "WriteLines.txt");
        //}

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
        public static string ConvertDateTimeToString(DateTime dtInput)
        {
            string strReturn = string.Empty;
            if (dtInput.Day < 10)
            {
                strReturn = "0" + dtInput.Day;
            }
            else
            {
                strReturn = Convert.ToString(dtInput.Day);
            }
            if (dtInput.Month < 10)
            {
                strReturn = strReturn + "/0" + dtInput.Month;
            }
            else
            {
                strReturn = strReturn + "/" + dtInput.Month;
            }
            strReturn = strReturn + "/" + dtInput.Year;
            if (dtInput.Hour < 10)
            {
                strReturn = strReturn + " 0" + dtInput.Hour;
            }
            else
            {
                strReturn = strReturn + " " + dtInput.Hour;
            }
            if (dtInput.Minute < 10)
            {
                strReturn = strReturn + ":0" + dtInput.Minute;
            }
            else
            {
                strReturn = strReturn + ":" + dtInput.Minute;
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
                    var estadoSemana = await aldakinDbContext.Estadodias.FirstOrDefaultAsync(x => x.Idusuario == iIdUser && x.Dia == date);
                    if (!(estadoSemana is null))
                    {
                        //semana cerrada
                    }
                    var lineTemp = await aldakinDbContext.Lineas.Where(x => x.Inicio >= dtIniWeek && x.Fin <= dtEndWeek && x.Idusuario == iIdUser).ToListAsync();

                    double dHour = lineTemp.Sum(x => x.Horas);

                    //aqui


                    if (date.Day == 1)
                    {
                        dtIniWeek = date;
                    }
                    double dHourWork = 0;
                    double dHourTravel = 0;
                    bool bClose = false;
                    lineTemp = await aldakinDbContext.Lineas.Where(x => x.Inicio >= dtIniWeek && x.Fin <= dtEndWeek && x.Idusuario == iIdUser).ToListAsync();
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
        private string AnalizeWorkLineDayAsync(WorkerLineData line, int iPartUser, int iUserAdmin, int idLineaOriginal, out DateTime dtIni, out DateTime dtEnd)
        {
            //analiZar si la semana esta cerrada si es solo gastos, si las horas estan usadas, si las hora fin es anterior a hora incio
            string strReturn = string.Empty;
            var lLineas = new List<Lineas>();
            try
            {
                if (!(string.IsNullOrEmpty(line.bGastos)))
                {
                    line.strHoraInicio = "00";
                    line.strMinutoInicio = "00";
                    line.strHoraFin = "00";
                    line.strMinutoFin = "00";
                }
                dtIni = Convert.ToDateTime(line.strCalendario + " " + line.strHoraInicio + ":" + line.strMinutoInicio + ":00");
                dtEnd = Convert.ToDateTime(line.strCalendario + " " + line.strHoraFin + ":" + line.strMinutoFin + ":00");
                DateTime day = dtIni;
                var lEstadoDia =  aldakinDbContext.Estadodias.Where(x => x.Idusuario == iPartUser && DateTime.Compare(x.Dia, day.Date) == 0).ToList();//
                if (!((iUserAdmin > 0) && (idLineaOriginal > 0)))
                {
                    if (lEstadoDia.Count > 0)
                    {
                        strReturn = "La semana esta cerrada, habla con tu responsable para reabirla;";
                        dtIni = new DateTime();
                        dtEnd = new DateTime();
                        return (strReturn);
                    }
                }
                if ((string.IsNullOrEmpty(line.bGastos)))
                {
                    if (DateTime.Compare(dtIni, dtEnd) > 0)
                    {
                        DateTime dtTemp = Convert.ToDateTime(line.strCalendario + "  00 :00:00");
                        if (DateTime.Compare(dtTemp, dtEnd) == 0)
                        {
                            dtEnd = dtEnd.AddDays(1);
                        }
                        else
                        {
                            strReturn = "Hora de Fin de Parte anterior a la Hora de inicio de Parte;";
                            dtIni = new DateTime();
                            dtEnd = new DateTime();
                            return (strReturn);
                        }
                    }
                    if ((iUserAdmin > 0) && (idLineaOriginal > 0))
                    {
                        lLineas =  aldakinDbContext.Lineas.Where(x => DateTime.Compare(x.Inicio.Date, day.Date) == 0 && x.Idlinea != idLineaOriginal && x.Idusuario == iPartUser && x.CodEnt== Convert.ToInt32(line.strEntidad)&& x.Idoriginal == 0 && x.Validado == 0 && x.Registrado == 0).ToList();
                    }
                    else
                    {
                        lLineas = aldakinDbContext.Lineas.Where(x => DateTime.Compare(x.Inicio.Date, day.Date) == 0 && x.Idusuario == iPartUser && x.CodEnt == Convert.ToInt32(line.strEntidad) && x.Validado == 0 && x.Registrado == 0).ToList();
                    }
                    if (WriteDataBase.RangeIsUsed(lLineas, dtEnd, dtIni, ref strReturn))
                    {
                        strReturn = "Rango de horas del parte ya utilizado";
                        dtIni = new DateTime();
                        dtEnd = new DateTime();
                        return (strReturn);
                    }
                }
            }
            catch (Exception ex)
            {
                strReturn = "Se ha producido un error en el procesamiento el estado de la semana;";
                dtIni = new DateTime();
                dtEnd = new DateTime();
            }
            return strReturn;
        }
        private string CreateWorkExpenses(int iOt, string strGastos, out List<Gastos> lGastosOut, out float fGastosOut, out float fKilometrosOut)
        {
            //crear los gastos segun lo que se ha introducido en el parte web
            var strReturn = string.Empty;
            var lGastos = new List<Gastos>();
            int iCodEntOt;
            float fGastos = 0, fKilometros = 0;
            try
            {
                var icodEntOt = aldakinDbContext.Ots.FirstOrDefault(t => t.Idots == iOt);
                iCodEntOt = icodEntOt.CodEnt;

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
                                        //bien
                                        var temp = substring[3].Replace('.', ',');
                                        //var gasto = (float)(Convert.ToDouble(temp));
                                        float gasto = Convert.ToSingle(temp, CultureInfo.CreateSpecificCulture("es-ES"));
                                        lGastos.Add(new Gastos
                                        {
                                            Pagador = Convert.ToInt32(substring[1]),
                                            Tipo = pagador.Idtipogastos,
                                            Cantidad = gasto,//float.Parse(substring[3].Replace('.', ',')) ,//(float)Convert.ToDouble(substring[3].Replace(',', '.')),
                                            Observacion = substring[4]
                                        });

                                        if (substring[2] != "KILOMETROS")
                                        {
                                            fGastos = fGastos + gasto;
                                        }
                                        else
                                        {
                                            fKilometros = fKilometros + gasto;
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    //si hay error no hace nada con la lineapara que siga con la siguiente
                                    //return RedirectToAction("Index", new { strMessage = "Los gastos son erroneos, repita el parte;" });
                                    strReturn = "Se ha producido un error en el procesamiento de gastos;";
                                    lGastosOut = null;
                                    fGastosOut = 0;
                                    fKilometrosOut = 0;
                                    return (strReturn);
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
            }
            catch (Exception ex)
            {
                strReturn = "Se ha producido un error en el procesamiento de gastos;";
                lGastosOut = null;
                fGastosOut = 0;
                fKilometrosOut = 0;
                return (strReturn);
            }
            lGastosOut = lGastos;
            fGastosOut = fGastos;
            fKilometrosOut = fKilometros;

            return strReturn;
        }
        private string OriginalOT(int iOt, string strObservaciones,int iUserCodEnt,string strUserName,  out int iOtOriginalOut)
        {
            string strReturn = string.Empty;
            iOtOriginalOut = iOt;
            //trabajos realizados
            var otSel = new Ots();
            try
            {
                otSel = aldakinDbContext.Ots.FirstOrDefault(x => x.Idots == iOt && x.Codigorefot != "29" && x.Cierre == null);
                if (otSel is null)
                {
                    strReturn = "En la Ot que esta usando se ha encontrado un problema, recargue la pagina;";
                    iOtOriginalOut = 0;
                    return (strReturn);
                }
                if (otSel.Nombre.Length > 20 && otSel.Nombre.Substring(0, 20) == "TRABAJOS REALIZADOS ")
                {
                    try
                    {
                        iOtOriginalOut = Convert.ToInt32(strObservaciones.Substring(0, 1));
                    }
                    catch (Exception)
                    {
                        strReturn = "En las OTs de trabajos para otras delegaciones, lo primero que debe aparecer en las observaciones debe ser la OT de la delegacion de origen;";
                        iOtOriginalOut = 0;
                        return (strReturn);
                    }
                }
                if(otSel.CodEnt!=iUserCodEnt)
                {
                    var userOt =  aldakinDbContext.Usuarios.FirstOrDefault(x => x.CodEnt == otSel.CodEnt && x.Name == strUserName);
                    if (userOt is null)
                    {
                        iOtOriginalOut = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                strReturn = "Se ha producido un error en el procesamiento Trabajos realezados(PARA);";
                iOtOriginalOut = 0;
                return (strReturn);
            }
            return strReturn;
        }
        private async Task<string> DayStatusColour(int iNumPart, int iGenerated, int iValidated, DateTime dtDay, int iStatus, int iCodEnt)
        {
            //inumpart: cantidad de partes
            //igenerated: cantidad de partes generados
            //ivalidated: cantidad e partes validados
            //iStatus = 0;//no cerrada blanco
            //iStatus = 2;//cerrada amarillo
            //iStatus = 4;//generada azul

            string strReturn = "#FFFFFF";
            string strAllGenerated = "#0000FF";//
            string strAllValidated = "#006400";//
            string strAllClose = "#FFFF00";//
            string strEmpty = "#FFFFFF";
            string strWeekend = "#D3D3D3";//
            string strHalfValidated = "#FF8C00";
            string strHalfJouney = "#F0FFF0";
            if (iNumPart == 0)
            {
                if (((Convert.ToInt32(dtDay.DayOfWeek) == 0)) || ((Convert.ToInt32(dtDay.DayOfWeek) == 6)))
                {
                    strReturn = strWeekend;
                }
                else
                {
                    var holiDay = await aldakinDbContext.Diasfestivos.FirstOrDefaultAsync(x => x.Dia == dtDay.Date && x.Calendario == iCodEnt);
                    if (!(holiDay is null))
                    {
                        if (holiDay.Jornadareducida == true)
                        {
                            strReturn = strHalfJouney;
                        }
                        else
                        {
                            strReturn = strWeekend;
                        }
                    }
                    else
                    {
                        strReturn = strEmpty;
                    }
                }
            }
            else
            {
                if (iNumPart == iGenerated)
                {
                    //todo el dia generado  	#0000FF
                    strReturn = strAllGenerated;
                }
                else
                {
                    if ((iNumPart == iValidated))
                    {
                        //no todo el dia generado todo el dia validado #006400
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
                                if (iStatus == 2)
                                {

                                    strReturn = strAllClose;
                                }
                                else
                                {
                                    var holiDay = await aldakinDbContext.Diasfestivos.FirstOrDefaultAsync(x => x.Dia == dtDay.Date && x.Calendario == iCodEnt);
                                    if (!(holiDay is null))
                                    {
                                        if (holiDay.Jornadareducida == true)
                                        {
                                            strReturn = strHalfJouney;
                                        }
                                        else
                                        {
                                            strReturn = strWeekend;
                                        }
                                    }
                                    else
                                    {
                                        strReturn = strEmpty;
                                    }
                                }
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
    }
}
