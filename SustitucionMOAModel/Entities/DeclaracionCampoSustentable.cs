using SustitucionMOAModel.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class DeclaracionCampoSustentable
    {
        [Key, Column(Order = 0)]
        public int Cosecha_Id { get; set; }

        [ForeignKey("Cosecha_Id")]
        public virtual Cosecha Cosecha { get; set; }
        
        [Key, Column(Order = 1)]
        public int Proveedor_Id { get; set; }

        [ForeignKey("Proveedor_Id")]
        public virtual Proveedor Proveedor { get; set; }

        public DateTime? FechaFirma { get; set; }

        public OpcionesDeclaracionCampoSustentable? OpcionDeclarada { get; set; }

        public double? HectareasDeclaradas { get; set; }
    }
}
