using System;
using System.Collections.Generic;
using AppPartes.Data.Models;

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

    public class MainDataViewLogic
    {
        public List<Ots> listOts { set; get; }
        public List<Entidad> listCompany { set; get; }
        public List<Clientes> listClient { set; get; }
        public List<Pernoctaciones> listNight { set; get; }
    }
    public class WeekDataViewLogic
    {
        public List<LineaVisual> listPartes { get; set; }
        public List<LineaVisual> listSelect { get; set; }
        public List<Pernoctaciones> listPernocta { get; set; }
        public List<double> listSemana { get; set; }
        public bool SemanaCerrada { get; set; }
        public string DateSelected { get; set; }
        public string Mensaje { get; set; }

    }
}
