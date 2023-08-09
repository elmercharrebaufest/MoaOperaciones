using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class ProveedorAuditoria
    {
        [Key]
        public int Id { get; set; }
        public int Usuario_Id { get; set; }
        public int Proveedor_Id { get; set; }
        public string CodigoProveedor { get; set; }
        public string Mail { get; set; }
        public string Cuit { get; set; }
        public int TipoProveedor_Id { get; set; }
        public string RazonSocial { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public string UsuarioActualizacion { get; set; }
    }
}
