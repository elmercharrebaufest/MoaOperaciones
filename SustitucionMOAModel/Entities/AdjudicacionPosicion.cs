using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class AdjudicacionPosicion
    {
        [Key]
        public int Id { get; set; }
        public int Adjudicacion_Id { get; set; }
        public int CotizacionPosicion_Id { get; set; }
        public int Cantidad { get; set; }
        public int SolpPosicion_Id { get; set; }

        [ForeignKey("Adjudicacion_Id")]
        public virtual Adjudicacion Adjudicacion { get; set; }

        [ForeignKey("SolpPosicion_Id")]
        public virtual SolpPosicion Posicion { get; set; }
        [ForeignKey("CotizacionPosicion_Id")]
        public virtual CotizacionPosicion CotizacionPosicion { get; set; }
    }
}
