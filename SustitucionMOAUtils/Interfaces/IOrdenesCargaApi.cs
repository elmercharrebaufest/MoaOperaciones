using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.OrdenDeCargaFason;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IOrdenDeCargaApiService
    {
        List<OrdenesDeCargaApiDto> ObtenerOrdenes(string patenteChasis);
        ResultadoGenerico InformarViajeOrdenesDeCargaFason(IngresosEgresosFasones ingresosEgresosFasones);
        ResultadoGenerico InformarViajeOrdenesDeCargaFas(IngresosEgresosFas ingresosEgresosFas);
    }
}
