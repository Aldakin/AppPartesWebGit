using System;

namespace AppPartes.Data.Models
{
    public partial class Correo
    {
        public int Idusuario { get; set; }
        public int Semana { get; set; }
        public int Mes { get; set; }
        public int Año { get; set; }
        public bool Validado { get; set; }
        public bool Enviado { get; set; }
        public DateTime? Fecha { get; set; }
    }
}
