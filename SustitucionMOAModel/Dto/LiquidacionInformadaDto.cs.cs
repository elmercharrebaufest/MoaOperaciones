using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class LiquidacionInformadaDto
    {
        public Guid Id { get; set; }
        public int Proveedor_Id { get; set; }
        public string COE { get; set; }
        //  [Column(TypeName = "date")]
        public string FechaComprobante { get; set; }
        //  [Column(TypeName = "date")]
        public string FechaInformada { get; set; }
    }
}
