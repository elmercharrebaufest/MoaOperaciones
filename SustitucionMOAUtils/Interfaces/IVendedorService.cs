using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Models.WSMapMOA.Vendedor;
using SustitucionMOAModel.Models.WSMapMOA.Vendedor.Detalle;
using SustitucionMOAModel.Models.WSMapMOA.Vendedor.Habilitado;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IVendedorService
    {
        VendedorDetalleWSMOAResponse GetDatosFiscales(string vendedor, string proveedor);
        VendedoresWSMOAResponse GetVendedores(string proveedor, string fechaInicio, string fechaFin);
        VendedorHabilitadoWSMOAResponse GetVendedorStatus(string cuit, string user);
        List<ProveedorDto> GetVendedoresPendientes(string mailUsuario);
    }

}
