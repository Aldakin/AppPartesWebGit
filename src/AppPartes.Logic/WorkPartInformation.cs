using AppPartes.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace AppPartes.Logic
{
    public class WorkPartInformation : IWorkPartInformation
    {
        private readonly AldakinDbContext aldakinDbContext;
        //apaño para usuario con claims
        private string strUserName = "";
        private string stUserrDni = "";
        private int iUserId = 0;
        private int iUserCondEntO = 0;
        public WorkPartInformation(AldakinDbContext aldakinDbContext)
        {
            this.aldakinDbContext = aldakinDbContext;
        }
        private async void GetUserDataAsync(int idAldakinUser)
        {
            var user = await aldakinDbContext.Usuarios.FirstOrDefaultAsync(x => x.Idusuario == idAldakinUser && x.CodEnt == x.CodEntO);
            if (!(user is null))
            {
                strUserName = user.Nombrecompleto.ToString();
                iUserId = Convert.ToInt16(user.Idusuario);
                iUserCondEntO = Convert.ToInt16(user.CodEntO);
                stUserrDni = user.Name;
            }
        }
        public async Task<List<SelectData>> WeekHourResume(DateTime dtSelected, int idAldakinUser)
        {
            GetUserDataAsync(idAldakinUser);
            var lReturn = new List<SelectData>();
            var iCont = 0;
            IniEndWeek(dtSelected, out var dtIniWeek, out var dtEndWeek);
            #region list hours per day in a week
            for (var date = dtIniWeek; date < dtEndWeek; date = date.AddDays(1.0))
            {
                var strRangosHora = "";
                var listPartes = await aldakinDbContext.Lineas.Where(x => x.Inicio.Date == date.Date && x.CodEnt == iUserCondEntO && x.Idusuario == iUserId).OrderBy(x => x.Inicio).ToListAsync();
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
            return lReturn;
        }
        public async Task<List<SelectData>> SelectedCompanyReadOt(int iEntidad, int idAldakinUser)
        {
            GetUserDataAsync(idAldakinUser);
            var lReturn = new List<SelectData>();
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
            return lReturn;
        }
        public async Task<List<SelectData>> SelectedCompanyReadClient(int iEntidad, int idAldakinUser)
        {
            GetUserDataAsync(idAldakinUser);
            var lReturn = new List<SelectData>();
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

            return lReturn;
        }
        public async Task<List<SelectData>> SelectedClient(int iClient, int idAldakinUser)
        {
            GetUserDataAsync(idAldakinUser);
            var lReturn = new List<SelectData>();
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
                listOts = await aldakinDbContext.Ots.Where(x => x.CodEnt == iUserCondEntO && x.CodEntD == 0 && x.Codigorefot != "29" && x.Cierre == null).OrderByDescending(x => x.Idots).ToListAsync();
                foreach (var p in listOts)
                {
                    var strTemp = p.Numero + "||" + p.Nombre;
                    lReturn.Add(new SelectData { iValue = p.Idots, strText = strTemp });
                }

            }
            return lReturn;
        }
        public async Task<List<SelectData>> SelectedOt(int iOt, int idAldakinUser)
        {
            GetUserDataAsync(idAldakinUser);
            var lReturn = new List<SelectData>();
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
            return lReturn;
        }
        public async Task<List<SelectData>> ReadLevel1(int iData, int idAldakinUser)
        {
            GetUserDataAsync(idAldakinUser);
            var lReturn = new List<SelectData>();
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
            return lReturn;
        }
        public async Task<List<SelectData>> ReadLevel2(int iData, int iData2, int idAldakinUser)
        {
            GetUserDataAsync(idAldakinUser);
            var lReturn = new List<SelectData>();
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
            return lReturn;
        }
        public async Task<List<SelectData>> ReadLevelGeneral(int iData, int idAldakinUser)
        {
            GetUserDataAsync(idAldakinUser);
            var lReturn = new List<SelectData>();
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
            return lReturn;
        }
        public async Task<List<SelectData>> SelectedPayer(int iPayer, int iOt, int idAldakinUser)
        {
            GetUserDataAsync(idAldakinUser);
            var lReturn = new List<SelectData>();
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
            return lReturn;
        }
        public static void IniEndWeek(DateTime dtSelected, out DateTime dtIniWeek, out DateTime dtEndWeek)
        {
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
                    throw new Exception();
            }
        }
    }
}
