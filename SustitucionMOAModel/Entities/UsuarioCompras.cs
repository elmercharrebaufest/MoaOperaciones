using System.ComponentModel.DataAnnotations;

namespace SustitucionMOAModel.Entities
{
    public class UsuarioCompras
    {
        [Key]
        public int Id { get; set; }
        public string Mail { get; set; }
        public string Nombres { get; set; }
        public bool Habilitado { get; set; }
        public bool PorDefecto { get; set; }
    }
}
