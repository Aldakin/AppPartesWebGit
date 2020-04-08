using System;
using System.Collections.Generic;

namespace AppPartes.Data.Models
{
    public partial class Tipogastos
    {
        public Tipogastos()
        {
            Gastos = new HashSet<Gastos>();
        }

        public int Idtipogastos { get; set; }
        public string Tipo { get; set; }
        public int CodEnt { get; set; }
        public string CodArt { get; set; }
        public int Pagador { get; set; }

        public virtual ICollection<Gastos> Gastos { get; set; }
    }
}
