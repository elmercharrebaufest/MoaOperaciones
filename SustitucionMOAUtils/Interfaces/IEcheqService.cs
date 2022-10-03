using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.WSMapMOA.Echeq;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IEcheqService
    {
        List<EcheqVisualizacionPendientePago> ObtenerPendientePago(string proveedor, string fechaInicio, string fechaFin);
    }
}
