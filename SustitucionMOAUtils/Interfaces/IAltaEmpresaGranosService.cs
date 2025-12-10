using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.DataAgro;
using SustitucionMOAModel.Models.ViewModel.AltaEmpresa;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IAltaEmpresaGranosService
    {
        byte[] GenerarInformeComercial(ParamInformeComercial informeComercial, string mailUsuario, int proveedorId);
        byte[] GenerarCartaDePresentacion(RptCartaDePresentacionInfo cartadePresentacion, string mailUsuario, int proveedorId);
        Localidad GetLocalidad(int localidadId);
        string GuardarArchivo(HttpPostedFileBase fileSubido, string fileKey, string mailUsuario, int proveedorId);
        //Task<string> ObtenerMaterialesDataAgro();
        List<ArchivoDto> ObtenerArchivosSubidos(string mailUsuario, int proveedorId, bool esOperador);
        string EnviarSolicitudUsuario(string mailUsuario, int proveedorId, bool esGuardarYNotificar, AltaEmpresaViewModel altaEmpresa);
        string ObtenerArchivo(string mailUsuario, int archivoID, int proveedorId);
        string ObtenerArchivos(string mailUsuario, int proveedorId, string pathBase);
        string EliminarArchivo(string mailUsuario, int archivoID, int proveedorId);
        InfoProveedorDataAgroDto ObtenerInfoProveedor(string mailUsuario, int proveedorId);
        AltaEmpresaViewModel CargarSolicitudUsuario(string mailUsuario, int proveedorId);
        //Task<string> ObtenerCampañasDataAgroAsync();

        string SolicitudAltaInterna(string mailUsuario, int proveedorId, AltaEmpresaViewModel altaEmpresa);
        string GrabarProveedorAltaInternaGranos(string cuit, string mailUsuario, string mailVendedor);
    }
}
