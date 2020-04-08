using System;
using System.Collections.Generic;

namespace AppPartes.Data.Models
{
    public partial class Logdesvalidacion
    {
        public int Idlogdesvalidacion { get; set; }
        public int Idlinea { get; set; }
        public DateTime Fecha { get; set; }
        public string Autor { get; set; }
    }
}
