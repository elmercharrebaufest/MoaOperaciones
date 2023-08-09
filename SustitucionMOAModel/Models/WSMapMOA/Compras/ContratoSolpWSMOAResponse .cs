using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAModel.Dto;

namespace SustitucionMOAModel.Models.WSMapMOA.Compras
{
    public class ContratoSolpWSMOAResponse
    {
        public string error { get; set; }
        public List<ContratoSolp> ContratosSolp { get; set; }

    }
}
