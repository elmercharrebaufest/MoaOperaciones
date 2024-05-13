using SustitucionMOAModel.Entities;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Text;
using System.Web;

namespace SustitucionMOAModel.Dto.ArchivoBoleto
{
    public class CrearReqArchivoBoletoDto
    {
        [Required]
        public  HttpPostedFileBase Archivo { get; set; }

        public string EmailUsuario { get; set; } = null;
        public int? ProveedorId { get; set; } = null;

        public Usuario UsuarioCreador { get; set; } = null;
        public Proveedor ProveedorUsado { get; set; } = null;
    }
}
