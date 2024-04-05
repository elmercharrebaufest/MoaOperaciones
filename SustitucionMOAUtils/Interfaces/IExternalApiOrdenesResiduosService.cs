using SustitucionMOAModel.Dto;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IExternalApiOrdenesResiduosService
    {
        List<OrdenesDeCargaApiDto> ObtenerOrdenes(string patenteChasis = null);
    }
}
