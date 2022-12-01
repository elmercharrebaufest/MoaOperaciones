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
        List<OrdendesDeCargaApiDto> ObtenerOrdenes();
        void InformarViajeOrdenesDeCargaFason(IngresosEgresosFasones ingresosEgresosFasones);
        void InformarViajeOrdenesDeCargaFas(IngresosEgresosFas ingresosEgresosFas);
    }
}
