using System;
using System.Collections.Generic;

namespace AppPartes.Data.Models
{
    public partial class Logcambios
    {
        public int Idlogpermisos { get; set; }
        public string Name { get; set; }
        public DateTime Momento { get; set; }
        public string Message { get; set; }
        public string Autor { get; set; }
    }
}
