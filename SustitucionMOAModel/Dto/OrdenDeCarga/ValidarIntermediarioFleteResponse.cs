using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto.OrdenDeCarga
{
    public class ValidarIntermediarioFleteResponse
    {
        public bool EsCuitValido { get; set; }

        public bool ExisteIntermediario { get; set; }

        public string RazonSocial { get; set; }
    }
}
