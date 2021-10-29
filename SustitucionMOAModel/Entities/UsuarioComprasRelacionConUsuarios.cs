using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class UsuarioComprasRelacionConUsuarios
    {
        [Key]
        public int Id { get; set; }
        public int Usuario_Id { get; set; }
        [ForeignKey("Usuario_Id")]
        public virtual Usuario Usuario { get; set; }
        public int UsuarioCompras_Id { get; set; }
        [ForeignKey("UsuarioCompras_Id")]
        public virtual UsuarioCompras UsuarioCompras { get; set; }
    }
}
