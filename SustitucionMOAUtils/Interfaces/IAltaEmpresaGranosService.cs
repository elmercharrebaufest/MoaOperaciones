using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.DataAgro;
using System.Collections.Generic;
using System.Web;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IAltaEmpresaGranosService
    {
        byte[] GenerarInformeComercial(ParamInformeComercial informeComercial, string mailUsuario, int proveedorId);
        byte[] GenerarCartaDePresentacion(RptCartaDePresentacionInfo cartadePresentacion, string mailUsuario, int proveedorId);
        string GuardarArchivo(HttpPostedFileBase fileSubido, string fileKey, string mailUsuario, int proveedorId);
        string ObtenerMaterialesDataAgro();
        List<ArchivoDto> ObtenerArchivosSubidos(string mail, int proveedorId);
        string EnviarSolicitudUsuario(string mail, int proveedorId);
        string ObtenerArchivo(string mail, int archivoID, int proveedorId);
        string EliminarArchivo(string mail, int archivoID, int proveedorId);
        InfoProveedorDataAgroDto ObtenerInfoProveedor(string mailUsuario, int proveedorId);

    }
}
