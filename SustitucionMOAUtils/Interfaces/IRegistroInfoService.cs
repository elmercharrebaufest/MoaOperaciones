using SustitucionMOAModel.Dto;
using SustitucionMOAWS.WSConsumers;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IRegistroInfoService
    {
        CrearSolpConsumerMOAResponse CrearOActualizarRegistrosInfoEnSap(List<RegistroInfoDto> registros);

        List<RegistroInfoDto> ObtenerRegistroInfoConsumer(string material, string centro, string organizacionDeCompras, string proveedor);

        RegistroInfoDto ObtenerUltimoRegistroPorMaterialYProveedor(string material, string centro, string grupoDeCompras, string proveedor = "");
    }
}
