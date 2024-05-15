using System.ComponentModel.DataAnnotations;


namespace SustitucionMOAModel.Entities
{
    public class TipoUsuario
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string NombreCorto { get; set; }

    }
}
