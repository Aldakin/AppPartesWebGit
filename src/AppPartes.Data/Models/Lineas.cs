using System;

namespace AppPartes.Data.Models
{
    public partial class Lineas
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
        public int? Idoriginal { get; set; }
        public sbyte Registrado { get; set; }
        public int CodEnt { get; set; }
        public sbyte? Validado { get; set; }
        public string Validador { get; set; }

        public virtual Ots IdotNavigation { get; set; }
        public virtual Preslin IdpreslinNavigation { get; set; }
        public virtual Usuarios IdusuarioNavigation { get; set; }
    }
}
