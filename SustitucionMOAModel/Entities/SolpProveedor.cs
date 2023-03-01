using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class SolpProveedor
    {
        [Key]
        public int Id { get; set; }
        public int SolpPosicion_Id { get; set; }
        public int? Proveedor_Id { get; set; }
        public string RazonSocial { get; set; }
        public int TipoFiltroProveedorSolp_Id { get; set; }

        [ForeignKey("SolpPosicion_Id")]
        public virtual SolpPosicion SolpPosicion { get; set; }
        [ForeignKey("Proveedor_Id")]
        public virtual Proveedor Proveedor { get; set; }
        [ForeignKey("TipoFiltroProveedorSolp_Id")]
        public virtual TablaGeneral TipoFiltroProveedorSolp { get; set; }
       
    }
}
