using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class PesificacionSapDto
    {
        public string FechaCarga { get; set; }
        public string Contrato { get; set; }
        public string Fijacion { get; set; }
        public decimal Kilos { get; set; }
        public decimal Precio { get; set; }
        public string FechaPesificacion { get; set; }
        public decimal TipoCambio { get; set; }
    }
}
