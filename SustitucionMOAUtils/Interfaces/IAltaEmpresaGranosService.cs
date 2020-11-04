using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.DataAgro;
using SustitucionMOAModel.Models.ViewModel.AltaEmpresa;
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
        List<ArchivoDto> ObtenerArchivosSubidos(string mail, int proveedorId, bool esOperador);
        string EnviarSolicitudUsuario(string mail, int proveedorId, AltaEmpresaViewModel altaEmpresa);
        string ObtenerArchivo(string mail, int archivoID, int proveedorId);
        string EliminarArchivo(string mail, int archivoID, int proveedorId);
        InfoProveedorDataAgroDto ObtenerInfoProveedor(string mailUsuario, int proveedorId);
        AltaEmpresaViewModel CargarSolicitudUsuario(string mail, int proveedorId);
        string ObtenerCampañasDataAgro();
    }
}
