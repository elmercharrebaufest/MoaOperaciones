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
        public string CodigoCorredor { get; set; }
        public string RazonSocialCorredor { get; set; }
        public string CodigoProveedor { get; set; }
        public string RazonSocialProveedor { get; set; }
        public int Categoria_Id { get; set; }
        public int? SubCategoria_Id { get; set; }
        public string Asunto { get; set; }
        public int EstadoConsulta_Id { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaUltimaModificacion { get; set; }
        public int Usuario_Id { get; set; }
        public int UsuarioInterno_Id { get; set; }

        [ForeignKey("Usuario_Id")]
        public virtual Usuario Usuario { get; set; }

        [ForeignKey("Categoria_Id")]
        public virtual Categoria Categoria { get; set; }

        [ForeignKey("SubCategoria_Id")]
        public virtual SubCategoria SubCategoria { get; set; }

        [ForeignKey("EstadoConsulta_Id")]
        public virtual EstadoConsulta EstadoConsulta { get; set; }

        [Required]
        public virtual ConsultaDetalle Detalle { get; set; }

        public virtual ICollection<Comentario> Comentarios { get; set; }

        public Consulta() { }
    }
}
