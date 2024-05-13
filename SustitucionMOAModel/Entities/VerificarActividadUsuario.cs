using System.ComponentModel.DataAnnotations;

namespace SustitucionMOAModel.Entities
{
    public class VerificarActividadUsuario
    {
        [Key]
        public string Tabla { get; set; }
        public string Columna { get; set; }
        public bool SeEncontraronRegistros { get; set; }
    }
}
