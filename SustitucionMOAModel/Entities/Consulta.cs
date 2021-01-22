using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class Consulta
    {
        [Key]
        public int Id { get; set; }
        public int Proveedor_Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public int Categoria_Id { get; set; }
        public string Asunto { get; set; }
        public int EstadoConsulta_Id { get; set; }
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


        [ForeignKey("Proveedor_Id")]
        public virtual Proveedor Proveedor { get; set; }

        [ForeignKey("Categoria_Id")]
        public virtual Categoria Categoria { get; set; }

        [ForeignKey("EstadoConsulta_Id")]
        public virtual EstadoConsulta EstadoConsulta { get; set; }

        public virtual ICollection<Comentario> Cometarios { get; set; }
    }
}
