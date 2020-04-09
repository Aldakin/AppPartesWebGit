using System.Collections.Generic;

namespace AppPartes.Data.Models
{
    public partial class Preslin
    {
        public Preslin()
        {
            Lineas = new HashSet<Lineas>();
        }

        public int Idpreslin { get; set; }
        public int Idpresupuesto { get; set; }
        public int CodhPes { get; set; }
        public int? CodpPes { get; set; }
        public int? Nivel { get; set; }
        public string Nombre { get; set; }
        public float? Horas { get; set; }
        public int? CodEnt { get; set; }
        public int? Anexo { get; set; }
        public int? Version { get; set; }
        public int? Numero { get; set; }
        public int Tipohora { get; set; }
        public int? RefiUobra { get; set; }

        public virtual Presupuestos IdpresupuestoNavigation { get; set; }
        public virtual ICollection<Lineas> Lineas { get; set; }
    }
}
