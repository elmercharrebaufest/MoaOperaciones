using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class AdjudicacionPosicion
    {
        [Key]
        public int Id { get; set; }
        public int Adjudicacion_Id { get; set; }
        public int CotizacionPosicion_Id { get; set; }
        public decimal Cantidad { get; set; }
        public int SolpPosicion_Id { get; set; }
        public string Texto { get; set; }

        public decimal? Monto { get; set; }
        [ForeignKey("Adjudicacion_Id")]
        public virtual Adjudicacion Adjudicacion { get; set; }

        [ForeignKey("SolpPosicion_Id")]
        public virtual SolpPosicion Posicion { get; set; }
        [ForeignKey("CotizacionPosicion_Id")]
        public virtual CotizacionPosicion CotizacionPosicion { get; set; }
    }
}
