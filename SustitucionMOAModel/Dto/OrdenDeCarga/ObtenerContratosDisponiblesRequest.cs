using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto.OrdenDeCarga
{
    public class ObtenerContratosDisponiblesRequest
    {
        public string ClienteCodigo { get; set; }

        public string CorredorCodigo { get; set; }

        public string FechaDesde { get; set; }

        public string FechaHasta { get; set; }
    }
}