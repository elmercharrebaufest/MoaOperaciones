using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class AdjudicacionDto
    {
        [Key]
        public int Id { get; set; }
        public int Cotizacion_Id { get; set; }
        public int Solp_Id { get; set; }
        public string NumeroOrdenDeCompra { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int UsuarioCreador_Id { get; set; }
        public string UsuarioCreador { get; set; }
        public int Moneda_Id { get; set; }
        public string MonedaDescripcion { get; set; }
        public decimal MontoTotal { get; set; }
        public List<AdjudicacionPosicionDto> AdjudicacionPosiciones { get; set; } = new List<AdjudicacionPosicionDto>();
    }
}
