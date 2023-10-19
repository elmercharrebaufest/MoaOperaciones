using SustitucionMOAModel.Dto.AplicacionCartaPorte;
using System.Collections.Generic;

namespace SustitucionMOAWS.Interfaces
{
    public interface IAplicacionCartaPorteConsumer
    {
        List<CartaPorteParaAplicacionCartaPorte> ObtenerCartasPorteProveedor(string contrato, string proveedor);
        List<ContratoParaAplicacionCartaPorte> ObtenerContratosProveedor(string cartaPorte, string proveedor);
    }
}
