using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class CotizacionPosicion
    {
        [Key]
        public int Id { get; set; }
        public int Cotizacion_Id { get; set; }
        public int PeticionDeOfertaSolpPosicion_Id { get; set; }
        public decimal? Cantidad { get; set; }
        public int? UnidadDeMedida_Id { get; set; }
        public int? Moneda_Id { get; set; }
        public decimal? Precio { get; set; }
        public DateTime? FechaDeEntrega { get; set; }
        public DateTime? FechaDeVigencia { get; set; }


        [ForeignKey("Cotizacion_Id")]
        public virtual Cotizacion Cotizacion { get; set; }
        [ForeignKey("PeticionDeOfertaSolpPosicion_Id")]
        public virtual PeticionDeOfertaSolpPosicion PeticionDeOfertaSolpPosicion { get; set; }
        [ForeignKey("UnidadDeMedida_Id")]
        public virtual TablaSap UnidadDeMedida { get; set; }
        [ForeignKey("Moneda_Id")]
        public virtual TablaSap Moneda { get; set; }

        [InverseProperty("CotizacionPosicion")]
        public virtual ICollection<CotizacionSubPosicion> CotizacionSubPosiciones { get; set; } = new List<CotizacionSubPosicion>();
        public bool? NoDisponible { get; set; }
    }
}
