using SustitucionMOAModel.Models.WSMapMOA.ReporteContrato;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAWS.Interfaces
{
   public  interface IReporteContratoConsumerMOA
    {
        ReporteContratoWSMOAResponse ReporteContratoExecute(ReporteContratoWSMOARequest request);
        string SetearColorProducto(string codigoProducto);
    }
}
