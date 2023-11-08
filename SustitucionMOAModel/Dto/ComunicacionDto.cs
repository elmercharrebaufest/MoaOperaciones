using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SustitucionMOAModel.Dto
{
    public class ComunicacionDto
    {
        public int Id { get; set; }
        public int ComunicacionTipo { get; set; }
        public string ProveedorId { get; set; }
        public string FechaCreacion { get; set; }
        public bool Leida { get; set; }
        public string Detalle { get; set; }
        public string CM05 { get; set; }
        public DateTime FechaRecomunicacion { get; set; }
        public string Comprobante { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string DescripcionWeb { get; set; }
        public int ConsultaId { get; set; }
        public int ConsultaCategoriaId { get; set; }
        public int UsuarioId { get; set; }

        public string DescripcionCategoria { get; set; }

        //public virtual Categoria categoria { get; set; }
    }
}
