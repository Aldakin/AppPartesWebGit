using System;
using System.Collections.Generic;

namespace AppPartes.Data.Models
{
    public partial class Entidad
    {
        public Entidad()
        {
            Ots = new HashSet<Ots>();
            Usuarios = new HashSet<Usuarios>();
        }

        public int CodEnt { get; set; }
        public string Nombre { get; set; }

        public virtual ICollection<Ots> Ots { get; set; }
        public virtual ICollection<Usuarios> Usuarios { get; set; }
    }
}
