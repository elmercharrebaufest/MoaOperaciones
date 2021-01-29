using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class ConsultaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string Asunto { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaUltimaModificacion { get; set; }
        public string RazonSocial { get; set; }
        public string NombreVendedor { get; set; }
        public string Contrato { get; set; }
        public string CUIT { get; set; }
        public string Comprobante { get; set; }
        public DateTime? FechaPago { get; set; }
        public Decimal? Importe { get; set; }
        public Decimal? Impuesto { get; set; }
        public string Inscripcion { get; set; }
        public string Motivo { get; set; }
        public CategoriaDto Categoria { get; set; }

        public EstadoConsultaDto EstadoConsulta { get; set; }

        public IList<ComentarioDto> Comentarios { get; set; }
    }
}
