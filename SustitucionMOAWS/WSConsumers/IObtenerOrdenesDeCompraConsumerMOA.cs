using SustitucionMOAModel.Dto;
using System.Collections.Generic;

namespace SustitucionMOAWS.WSConsumers
{
    public interface IObtenerOrdenesDeCompraConsumerMOA
    {
        List<OrdenCompraDto> Request(OrderParamsDto parametros, bool usuarioSolp = false);
    }
}