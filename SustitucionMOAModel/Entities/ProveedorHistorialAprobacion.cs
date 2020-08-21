using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SustitucionMOAModel.Enums;

namespace SustitucionMOAModel.Entities
{
    public class ProveedorHistorialAprobacion
    {
        [Key]
        public int Id { get; set; }
        public int Proveedor_Id { get; set; }
        public int Usuario_Id { get; set; }
        public EstadoAprobacion EstadoAprobacion { get; set; }
        public string Observacion { get; set; }
        public DateTime Fecha { get; set; }

        [ForeignKey("Proveedor_Id")]
        public virtual Proveedor Proveedor { get; set; }

        [ForeignKey("Usuario_Id")]
        public virtual Usuario Usuario { get; set; }
    }
}
