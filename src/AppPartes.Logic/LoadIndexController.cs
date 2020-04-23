﻿using AppPartes.Data.Models;
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
        //apaño para usuario con claims
        private string _strUserName = "";
        private string _stUserDni = "";
        private int _iUserId ;
        private int _iUserCondEntO ;
        public LoadIndexController(AldakinDbContext aldakinDbContext, IWriteDataBase iWriteDataBase)
        {
            this.aldakinDbContext = aldakinDbContext;
            _iWriteDataBase = iWriteDataBase;
        }
        private async void WriteUserDataAsync(int idAldakinUser)
        {
            var user = await _iWriteDataBase.GetUserDataAsync(idAldakinUser);
            _strUserName = user.strUserName;
            _iUserId = user.iUserId;
            _iUserCondEntO = user.iUserCondEntO;
            _stUserDni = user.stUserrDni;
        }
        public async Task<MainDataViewLogic> LoadMainController(int idAldakinUser)
        {
            var oReturn = new MainDataViewLogic();
            WriteUserDataAsync(idAldakinUser);
            oReturn.listOts = await GetOts();
            oReturn.listCompany = await GetAldakinCompanies();
            oReturn.listClient = await GetAldakinClients();
            oReturn.listNight = await GetAldakinNight();
            return oReturn;
        }
        public async Task<WeekDataViewLogic> LoadWeekController(int idAldakinUser, string strDate = "", string strAction = "", string strId = "")
        {
            var oReturn = new WeekDataViewLogic();
            WriteUserDataAsync(idAldakinUser);
            DateTime day, dtIniWeek = DateTime.Now, dtEndWeek = DateTime.Now;
            if (string.IsNullOrEmpty(strDate) && string.IsNullOrEmpty(strAction))
            {
                //oReturn.Mensaje = "";
            }
            else
            {
                //var listVisual = new List<LineaVisual>();
                //var NombreOt = string.Empty;
                //var NombreCliente = string.Empty;
                //var strPernocta = string.Empty;
                //var strPreslin = string.Empty;
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
                            oReturn.listSemana = await ResumeHourPerDay(dtIniWeek, dtEndWeek);

                            oReturn.listPartes = await GetWeekWorkerParts(dtIniWeek, dtEndWeek);
                            var weekStatus =await  aldakinDbContext.Estadodias.FirstOrDefaultAsync(x => x.Dia == dtSelected.Date && x.Idusuario == _iUserId);
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
                        lEstadoDia = await aldakinDbContext.Estadodias.Where(x => x.Idusuario == _iUserId && DateTime.Compare(x.Dia, day.Date) == 0).ToListAsync();//
                        if (lEstadoDia.Count > 0)
                        {
                            oReturn.Mensaje = "La semana esta cerrada, habla con tu responsable para reabirla;";
                        }
                        oReturn.listSelect = await GetDayWorkerPart(lSelect);
                        oReturn.listPernocta = await GetAldakinNight(lSelect);
                        oReturn.DateSelected = lSelect.Inicio.Date.ToString("yyyy-MM-dd"); // dtSelected.Date;
                        break;
                    default:
                        //return View();
                        break;
                }
            }
            return oReturn;
        }
        public async Task<LoginDataViewLogic> LoadLoginController()
        {
            var lReturn = new LoginDataViewLogic();
            lReturn.listCompany = await GetAllAldakinCompanies();
            return lReturn;
        }
        private async Task<List<Ots>> GetOts()
        {
            var lReturn = new List<Ots>();
            var totalOts = aldakinDbContext.Ots.Where(x => x.CodEnt == _iUserCondEntO && x.CodEntD == 0 && x.Codigorefot != "29" && x.Cierre == null);
            var totalType1Ots = totalOts.Where(x => x.Tipoot == 1);
            var totalType2Ots = totalOts.Join(aldakinDbContext.Presupuestos.Where(x => x.CodEnt == _iUserCondEntO), o => o.Idots, i => i.Idot, (o, p) => o);//original
            lReturn = await totalType1Ots.Concat(totalType2Ots).Distinct().OrderBy(x => x.Numero).ToListAsync();
            return lReturn;
        }
        private async Task<List<Entidad>> GetAldakinCompanies()
        {
            var lReturn = new List<Entidad>();
            var aux = await aldakinDbContext.Entidad.FirstOrDefaultAsync(x => x.CodEnt == _iUserCondEntO);
            lReturn = await aldakinDbContext.Entidad.Where(x => x.CodEnt != _iUserCondEntO).OrderByDescending(x => x.Nombre).ToListAsync();
            lReturn.Insert(0, aux);
            return lReturn;
        }
        private async Task<List<Entidad>> GetAllAldakinCompanies()
        {
            var lReturn = new List<Entidad>();
            lReturn = await aldakinDbContext.Entidad.Where(x => x.CodEnt !=0).ToListAsync();
            return lReturn;
        }
        private async Task<List<Clientes>> GetAldakinClients()
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
        private async Task<List<Pernoctaciones>> GetAldakinNight()
        {
            var lReturn = new List<Pernoctaciones>();
            lReturn = await aldakinDbContext.Pernoctaciones.Where(x => x.CodEnt == _iUserCondEntO).ToListAsync();
            return lReturn;
        }
        private async Task<List<Pernoctaciones>> GetAldakinNight(Lineas lSelect)
        {
            var lReturn = new List<Pernoctaciones>();
            lReturn = await aldakinDbContext.Pernoctaciones.Where(x => x.CodEnt == lSelect.CodEnt).ToListAsync();
            return lReturn;
        }
        private async Task<List<double>> ResumeHourPerDay(DateTime dtIniWeek, DateTime dtEndWeek)
        {
            var lReturn = new List<double>();
            for (var date = dtIniWeek; date < dtEndWeek; date = date.AddDays(1.0))
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
        private async Task<List<LineaVisual>> GetWeekWorkerParts(DateTime dtIniWeek, DateTime dtEndWeek)
        {
            var lReturn = new List<LineaVisual>();
            var NombreOt = string.Empty;
            var NombreCliente = string.Empty;
            var strPernocta = string.Empty;
            var strPreslin = string.Empty;
            var lTemp = await aldakinDbContext.Lineas.Where(x => x.Inicio > dtIniWeek && x.Fin < dtEndWeek && x.Idusuario == _iUserId && x.CodEnt == _iUserCondEntO).OrderBy(x => x.Inicio).ToListAsync();

            //aqui tengo que recorrer lisPartes e ir volvandolo a otra clase nueva para nomstrar losnombres de las ots y empresas...
            //ademas tengo que ver si es la linea original o la secundaria para que si es la secundaria la cambie por la principal
            foreach (var l in lTemp)
            {
                var nombre = await aldakinDbContext.Ots.FirstOrDefaultAsync(x => x.Idots == l.Idot);
                NombreOt = nombre.Nombre;
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
                    lReturn.Add(new LineaVisual
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
                    var lineaOriginal = await aldakinDbContext.Lineas.FirstOrDefaultAsync(x => x.Idlinea == l.Idoriginal);
                    lReturn.Add(new LineaVisual
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
            return lReturn;
        }
        private async Task<List<LineaVisual>> GetDayWorkerPart(Lineas lSelect)
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
