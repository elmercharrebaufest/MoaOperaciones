using System.ComponentModel.DataAnnotations;

namespace SustitucionMOAModel.Dto
{
    public class AsignarNuevaCuitDto
    {
        [Required]
        public int IdUsuario { get; set; }
        [Required]
        public string MailUsuario { get; set; }
        [Required]
        public string CuitAAsignar { get; set; }
        [Required]
        public string RazonSocialAAsignar { get; set; }
        [Required]
        public int TipoProveedorIdAAsignar { get; set; }
        [Required]
        public string CodigoProveedorAAsignar { get; set; }
    }
}
