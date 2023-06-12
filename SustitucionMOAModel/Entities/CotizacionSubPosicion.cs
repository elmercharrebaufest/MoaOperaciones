using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class CotizacionSubPosicion
    {
        [Key]
        public int Id { get; set; }
        public int CotizacionPosicion_Id { get; set; }
        public int SolpSubPosicion_Id { get; set; }
        public int? Cantidad { get; set; }
        public int? UnidadDeMedida_Id { get; set; }
        public int? Moneda_Id { get; set; }     
        public decimal? Precio { get; set; }
       

        [ForeignKey("CotizacionPosicion_Id")]
        public virtual CotizacionPosicion CotizacionPosicion { get; set; }
        [ForeignKey("SolpSubPosicion_Id")]
        public virtual SolpSubposicion SolpSubPosicion { get; set; }
        [ForeignKey("UnidadDeMedida_Id")]
        public virtual TablaSap UnidadDeMedida { get; set; }
        [ForeignKey("Moneda_Id")]
        public virtual TablaSap Moneda { get; set; }


    }
}
