using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.OrdenDeCargaFason;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IOrdenDeCargaApiService
    {
        List<OrdendesDeCargaApiDto> ObtenerOrdenes(string patenteChasis);
        ResultadoGenerico InformarViajeOrdenesDeCargaFason(IngresosEgresosFasones ingresosEgresosFasones);
        ResultadoGenerico InformarViajeOrdenesDeCargaFas(IngresosEgresosFas ingresosEgresosFas);
    }
}
