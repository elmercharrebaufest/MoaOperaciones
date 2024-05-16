using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto.OrdenDeCarga
{
    public class GestionCuitDto
    {
        public string cuit { get; set; }
        public string razonSocial { get; set; }
        public string campo { get; set; }
        public string ordenId { get; set; }
        public bool gestiona { get; set; }

    }
}
