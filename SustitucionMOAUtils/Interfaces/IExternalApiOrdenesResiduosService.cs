using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.OrdenResiduos;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IExternalApiOrdenesResiduosService
    {
        List<OrdenesDeCargaApiDto> ObtenerOrdenes(string patenteChasis = null);
        void ActualizarOrden(ActualizarOrdenResiduosExternalDto datos);
    }
}
