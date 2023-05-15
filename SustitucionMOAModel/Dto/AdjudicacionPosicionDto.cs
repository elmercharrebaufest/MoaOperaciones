using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class AdjudicacionPosicionDto
    {
        [Key]
        public int Id { get; set; }
        public int Adjudicacion_Id { get; set; }
        public int CotizacionPosicion_Id { get; set; }
        public int? Cantidad { get; set; }
        public int SolpPosicion_Id { get; set; }
              
    }
}
