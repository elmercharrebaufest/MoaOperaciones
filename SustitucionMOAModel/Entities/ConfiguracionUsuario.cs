using SustitucionMOAModel.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class ConfiguracionUsuario
    {
        [Key]
        public int Id { get; set; }
        public TipoConfiguracionUsuario Tipo { get; set; }
        public string Valor { get; set; }
        public int Usuario_Id { get; set; }
        [ForeignKey("Usuario_Id")]
        public virtual Usuario Usuario { get; set; }
    }
}
