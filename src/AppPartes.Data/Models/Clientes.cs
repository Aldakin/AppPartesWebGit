using System;
using System.Collections.Generic;

namespace AppPartes.Data.Models
{
    public partial class Clientes
    {
        public Clientes()
        {
            Ots = new HashSet<Ots>();
        }

        public int Idclientes { get; set; }
        public string Nombre { get; set; }
        public int Codigo { get; set; }
        public int CodEnt { get; set; }

        public virtual ICollection<Ots> Ots { get; set; }
    }
}
