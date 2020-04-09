using System;

namespace AppPartes.Data.Models
{
    public partial class Estadodias
    {
        public int Id { get; set; }
        public DateTime Dia { get; set; }
        public int Idusuario { get; set; }
        public int Estado { get; set; }
        public float Horas { get; set; }
    }
}
