using System.Collections.Generic;

namespace AppPartes.Data.Models
{
    public partial class Presupuestos
    {
        public Presupuestos()
        {
            Preslin = new HashSet<Preslin>();
        }

        public int Idpresupuestos { get; set; }
        public int Idot { get; set; }
        public string Nombre { get; set; }
        public int Numero { get; set; }
        public int CodEnt { get; set; }

        public virtual Ots IdotNavigation { get; set; }
        public virtual ICollection<Preslin> Preslin { get; set; }
    }
}
