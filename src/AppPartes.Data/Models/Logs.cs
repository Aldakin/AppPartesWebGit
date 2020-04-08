using System;
using System.Collections.Generic;

namespace AppPartes.Data.Models
{
    public partial class Logs
    {
        public int Idlog { get; set; }
        public DateTime Time { get; set; }
        public int Idusuario { get; set; }
        public string Domain { get; set; }
        public string Device { get; set; }
        public string DeviceUser { get; set; }
        public string Clase { get; set; }
        public string Metodo { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
        public string Custommessage { get; set; }
    }
}
