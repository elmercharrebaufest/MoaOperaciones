using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    [Table("UsuarioGranos")]
    public class UsuarioGranos : Usuario
    {
        public string Comercial { get; set; }
    }
}
