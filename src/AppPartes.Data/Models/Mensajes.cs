using System;
using System.Collections.Generic;

namespace AppPartes.Data.Models
{
    public partial class Mensajes
    {
        public int Idmensajes { get; set; }
        public int De { get; set; }
        public int A { get; set; }
        public DateTime Fecha { get; set; }
        public string Asunto { get; set; }
        public string Mensaje { get; set; }
        public DateTime? Vistodestino { get; set; }
        public DateTime? Vistoremite { get; set; }
        public int? Idlinea { get; set; }
        public int Inicial { get; set; }
        public bool Estado { get; set; }
    }
}
