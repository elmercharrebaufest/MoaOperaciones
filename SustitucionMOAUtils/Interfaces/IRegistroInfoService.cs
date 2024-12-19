using SustitucionMOAModel.Dto;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IRegistroInfoService
    {
        List<RegistroInfoDto> ObtenerRegistroInfoConsumer(string material, string centro, string organizacionDeCompras, string proveedor);

        RegistroInfoDto ObtenerUltimoRegistroPorMaterialYProveedor(string material, string centro, string grupoDeCompras, string proveedor = "");
    }
}
