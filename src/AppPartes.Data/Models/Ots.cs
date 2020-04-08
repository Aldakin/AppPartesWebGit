using System;
using System.Collections.Generic;

namespace AppPartes.Data.Models
{
    public partial class Ots
    {
        public Ots()
        {
            Lineas = new HashSet<Lineas>();
            Presupuestos = new HashSet<Presupuestos>();
        }

        public int Idots { get; set; }
        public int Numero { get; set; }
        public string Nombre { get; set; }
        public DateTime Apertura { get; set; }
        public DateTime? Cierre { get; set; }
        public int? Creador { get; set; }
        public int Cliente { get; set; }
        public int Responsable { get; set; }
        public int? Responsable1 { get; set; }
        public int? Responsable2 { get; set; }
        public int Tipoot { get; set; }
        public string Codigorefot { get; set; }
        public int CodEnt { get; set; }
        public int? CodEntD { get; set; }
        public bool FacViajes { get; set; }
        public bool FacDietas { get; set; }
        public bool FacKm { get; set; }
        public float? MaxViajes { get; set; }
        public float? MaxDietas { get; set; }
        public float? MaxKm { get; set; }
        public bool? CkCapitulo { get; set; }

        public virtual Clientes ClienteNavigation { get; set; }
        public virtual Entidad CodEntNavigation { get; set; }
        public virtual ICollection<Lineas> Lineas { get; set; }
        public virtual ICollection<Presupuestos> Presupuestos { get; set; }
    }
}
