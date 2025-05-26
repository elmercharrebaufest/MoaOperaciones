using SustitucionMOAModel.Dto.OrdenesCompra;
using SustitucionMOAModel.Entities;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Services.Email.Dto
{
    public class MailAprobacionESRequest
    {
        public string DestinatarioMail { get; set; }

        public int UsuarioId { get; set; }

        public List<MailAprobacionESPosicion> Posiciones { get; set; }
    }


    public class MailAprobacionESPosicion
    {
        public string ProveedorRazonSocial { get; set; }

        public Aprobaciones Aprobacion { get; set; }

        public List<ReporteDto> Reportes { get; set; }

        public IEnumerable<AdjuntosEntradasDeServicio> Adjuntos { get; set; }
    }
}
