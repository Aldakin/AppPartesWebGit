using System;
using System.Collections.Generic;

namespace AppPartes.Data.Models
{
    public partial class Gastos
    {
        public int Idlinea { get; set; }
        public int Pagador { get; set; }
        public int Tipo { get; set; }
        public float Cantidad { get; set; }
        public string Observacion { get; set; }
        public int Idgastos { get; set; }

        public virtual Tipogastos TipoNavigation { get; set; }
    }
}
