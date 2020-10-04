using AppPartes.Data.Models;
using System;
using System.Collections.Generic;

namespace AppPartes.Logic
{
    public class GeneralTypeClasses
    {

    }
    public class SelectData
    {
        public int iValue { set; get; }
        public string strText { set; get; }
        public string strValue { set; get; }
    }
    public class WorkerLineData
    {
        public string strIdLinea { set; get; }
        public string strEntidad { set; get; }
        public string strOt { set; get; }
        public int iIdUsuario { set; get; }
        public string strPresupuesto { set; get; }
        public string strNivel1 { set; get; }
        public string strNivel2 { set; get; }
        public string strNivel3 { set; get; }
        public string strNivel4 { set; get; }
        public string strNivel5 { set; get; }
        public string strNivel6 { set; get; }
        public string strNivel7 { set; get; }
        public string strCalendario { set; get; }
        public string strHoraInicio { set; get; }
        public string strMinutoInicio { set; get; }
        public string strHoraFin { set; get; }
        public string strMinutoFin { set; get; }
        public string bHorasViaje { set; get; }
        public string bGastos { set; get; }
        public string strParte { set; get; }
        public string strPernoctacion { set; get; }
        public string strObservaciones { set; get; }
        public string strPreslin { set; get; }
        public string strGastos { set; get; }
        public string strMensaje { set; get; }
        public string strIdlineaAntigua { set; get; }
        public string strAction { set; get; }

    }
    public class LineaVisual
    {
        public int iEntidad { get; set; }
        public string strEntidad { set; get; }
        public int Idot { get; set; }
        public string NombreOt { get; set; }
        public int iPresu { set; get; }
        public string NombrePresu { get; set; }

        public List<Presupuestos> lpresupuesto { set; get; }

        public List<Preslin> lNivel1 { get; set; }
        public List<Preslin> lNivel2 { get; set; }
        public List<Preslin> lNivel3 { get; set; }
        public List<Preslin> lNivel4 { get; set; }
        public List<Preslin> lNivel5 { get; set; }
        public List<Preslin> lNivel6 { get; set; }
        public List<Preslin> lNivel7 { get; set; }
        public int iNivel1 { get; set; }
        public int iNivel2 { get; set; }
        public int iNivel3 { get; set; }
        public int iNivel4 { get; set; }
        public int iNivel5 { get; set; }
        public int iNivel6 { get; set; }
        public int iNivel7 { get; set; }
        public string strNivel1 { get; set; }
        public string strNivel2 { get; set; }
        public string strNivel3 { get; set; }
        public string strNivel4 { get; set; }
        public string strNivel5 { get; set; }
        public string strNivel6 { get; set; }
        public string strNivel7 { get; set; }


        public int Idlinea { get; set; }
        public string NombreUsuario { get; set; }
        public string NombreCliente { get; set; }
        public int? Idpreslin { get; set; }
        public string NombrePreslin { get; set; }
        public float? Dietas { get; set; }
        public float? Km { get; set; }
        public string Observaciones { get; set; }
        public string ObservacionesCompleta { get; set; }
        public float? Horasviaje { get; set; }
        public float Horas { get; set; }
        public string strInicio { get; set; }
        public string strFin { get; set; }
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
        public int iStatus { get; set; }
    }
    public class LineMessage
    {
        public int Idmensajes { get; set; }
        public int De { get; set; }
        public string strDe { get; set; }
        public int A { get; set; }
        public string strA { get; set; }
        public DateTime Fecha { get; set; }
        public string Asunto { get; set; }
        public string Mensaje { get; set; }
        public int Idlinea { get; set; }
        public int Inicial { get; set; }
        public bool Estado { get; set; }
    }
    public class UserData
    {
        public string strUserName { set; get; }
        public string stUserrDni { set; get; }
        public int iUserId { set; get; }
        public int iUserCondEntO { set; get; }
        public int iLevel { set; get; }
        public bool bAdmin { set; get; }
    }
    public class HomeDataViewLogic
    {
        public UserData user { set; get; }
        public List<Entidad> listCompanyUpdate { set; get; }
        public List<Entidad> listCompanyCsv { set; get; }
        public string strError { set; get; }
        public string strVersion { set; get; }

    }
    public class MainDataViewLogic
    {
        public List<Ots> listOts { set; get; }
        public List<Entidad> listCompany { set; get; }
        public List<Clientes> listClient { set; get; }
        public List<Pernoctaciones> listNight { set; get; }
        public bool bMessage { set; get; }
        public string strError { set; get; }
    }
    public class WeekDataViewLogic
    {
        public List<List<LineaVisual>> listPartes { get; set; }
        public List<LineaVisual> listSelect { get; set; }
        public List<Pernoctaciones> listPernocta { get; set; }
        public List<Entidad> listCompany { get; set; }
        public List<Clientes> listClient { get; set; }
        public List<Ots> listOts { set; get; }
        public List<double> listSemana { get; set; }
        public bool SemanaCerrada { get; set; }
        public string DateSelected { get; set; }
        public string Mensaje { get; set; }
        public bool bMessage { set; get; }
        public string Gastos { set; get; }
        public string strError { set; get; }
    }
    public class LoginDataViewLogic
    {
        public List<Entidad> listCompany { set; get; }
        public bool bMessage { set; get; }
        public string strError { set; get; }
    }
    public class MessageViewLogic
    {
        public List<LineMessage> listMessages { get; set; }
        public LineMessage oMessage { get; set; }
        public bool bMessage { set; get; }
        public string strError { set; get; }
    }
    public class SearchViewLogic
    {
        public List<Entidad> listCompany { set; get; }
        public string strError { set; get; }

        public List<ViewMounthResume> listResume { get; set; }
        public List<List<LineaVisual>> listWeekResume { get; set; }
        public string strGlobalValidation { get; set; }
        public string strDate { get; set; }
        public string strDate1 { get; set; }
        public string strEntity { get; set; }
        public string strWorker { get; set; }
        public string strAction{set;get;}
        public bool bLevelError { set; get; }
    }
    public class SearchEditViewLogic
    {

        public string strError { set; get; }
        public List<LineaVisual> listSelect { set; get; }
        public List<Pernoctaciones> listPernocta { set; get; }
        public List<Entidad> listCompany { get; set; }
        public List<Clientes> listClient { get; set; }
        public List<Ots> listOts { set; get; }
        public string DateSelected { set; get; }
        public string Gastos { set; get; }
        public bool bLevelError { set; get; }
    }
    public class SearchPendingViewLogic
    {
        public List<Entidad> listCompany { set; get; }
        public List<string> lSummary { set; get; }
        public string strError { set; get; }
        public bool bLevelError { set; get; }
    }
    public class UdObraPresuViewLogic
    {
        public List<Udobrapresu> lUdObra { set; get; }
        public List<Entidad> lEntidad { set; get; }
        public string strMensaje { set; get; }
        public bool bLevelError { set; get; }
    }
    public class HoliDaysViewLogic
    {
        public string strMensaje { set; get; }
        public List<Entidad> lEntidad { set; get; }
        public List<Diasfestivos> lDiasFestivos { set; get; }
        public string dtSelectedIni { set; get; }
        public string dtSelectedFin { set; get; }
        public string strEntidadSelec { set; get; }
        public bool bLevelError { set; get; }
    }
    public class ReportsViewLogic
    {
        public string strMensaje { set; get; }
        public List<Entidad> lEntidad { set; get; }
    }
    public class ViewMounthResume
    {
        public string User { get; set; }
        public List<string> lDay { get; set; }
        public List<SearchDay> dayStatus { set; get; }
        public int iMoroso{set;get;}
        //public List<double> lHour { get; set; }
        //public List<string> lStatusColour { get; set; }
    }
    public class SearchDay
    {
        public int day { set; get; } 
        public double hour { set; get; }
        public string colour { set; get; }
    }
    public class PermisosViewLogic
    {
        public List<Usuarios> lUser { set; get; }
        public List<Entidad> lEnt { set; get; }
        public List<Usuarios> lUserAllUser { set; get; }
        public List<Usuarios> lUserSelected { set; get; }
        public List<Ots> lOtsSelected { set; get; }
        public List<Ots> lAllOts { set; get; }
        public string strUserSelected { set; get; }
        public string strError { set; get; }
        public bool bLevelError { set; get; }
    }
    public class Excel
    {
        public string str1 { set; get; }
        public string str2 { set; get; }
        public string str3 { set; get; }
        public string str4 { set; get; }
        public string str5 { set; get; }
        public string str6 { set; get; }
        public string str7 { set; get; }
        public string str8 { set; get; }
        public string str9 { set; get; }
        public string str10 { set; get; }
        public string str11{ set; get; }
        public string str12 { set; get; }
        public string str13 { set; get; }
        public string str14 { set; get; }
        public string str15 { set; get; }
        public string str16 { set; get; }
    }
}
