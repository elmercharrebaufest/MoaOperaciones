using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Contrato.Detalle
{
    public class Calidad
    {
        public string ccpp { get; set; }
        public string kgNetosTotal { get; set; }
        public string kgAplicadosTotal { get; set; }
        public string kgDtoTotal { get; set; }
        public string dtoPorcTotal { get; set; }
        public string certificado { get; set; }
        public List<CalidadElement> registros { get; set; }
    }
}
