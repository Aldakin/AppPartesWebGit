using System;
using System.Collections.Generic;

namespace AppPartes.Data.Models
{
    public partial class Lineasoriginales
    {
        public int Idlinea { get; set; }
        public int Idot { get; set; }
        public int? Idpreslin { get; set; }
        public float? Dietas { get; set; }
        public float? Km { get; set; }
        public string Observaciones { get; set; }
        public float? Horasviaje { get; set; }
        public float Horas { get; set; }
        public DateTime Inicio { get; set; }
        public DateTime Fin { get; set; }
        public int Idusuario { get; set; }
        public int Facturable { get; set; }
        public string Npartefirmado { get; set; }
        public int CodEnt { get; set; }
    }
}
