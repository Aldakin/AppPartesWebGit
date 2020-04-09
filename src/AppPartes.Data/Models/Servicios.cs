namespace AppPartes.Data.Models
{
    public partial class Servicios
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public sbyte Ejecutar { get; set; }
        public string Condicion { get; set; }
        public int CodEnt { get; set; }
    }
}
