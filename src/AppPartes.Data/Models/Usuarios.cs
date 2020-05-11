using System;
using System.Collections.Generic;

namespace AppPartes.Data.Models
{
    public partial class Usuarios
    {
        public object idusuario;

        public Usuarios()
        {
            Lineas = new HashSet<Lineas>();
        }

        public int Idusuario { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public int Autorizacion { get; set; }
        public string Nombrecompleto { get; set; }
        public int Idcategoria { get; set; }
        public string Email { get; set; }
        public DateTime? Incorporacion { get; set; }
        public sbyte? Baja { get; set; }
        public int CodEnt { get; set; }
        public int? CodEntO { get; set; }
        public int CodEmpl { get; set; }
        public int Calendario { get; set; }

        public virtual Entidad CodEntNavigation { get; set; }
        public virtual ICollection<Lineas> Lineas { get; set; }
    }
}
