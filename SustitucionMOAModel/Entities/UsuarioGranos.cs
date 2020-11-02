using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    [Table("UsuarioGranos")]
    public class UsuarioGranos : Usuario
    {
        //OJO, no usar este campo. Corresponde usar el que está en Proveedor
        public string Comercial { get; set; }
    }
}
