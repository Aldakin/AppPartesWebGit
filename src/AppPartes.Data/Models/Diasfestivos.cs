using System;
using System.Collections.Generic;

namespace AppPartes.Data.Models
{
    public partial class Diasfestivos
    {
        public int Idfestivos { get; set; }
        public DateTime Dia { get; set; }
        public bool Jornadareducida { get; set; }
        public int Calendario { get; set; }
    }
}
