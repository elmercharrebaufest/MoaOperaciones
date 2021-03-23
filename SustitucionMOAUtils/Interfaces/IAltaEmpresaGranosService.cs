using SustitucionMOAModel.Dto;
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
        string GuardarArchivo(HttpPostedFileBase fileSubido, string fileKey, string mailUsuario, int proveedorId);
        Task<string> ObtenerMaterialesDataAgro();
        List<ArchivoDto> ObtenerArchivosSubidos(string mailUsuario, int proveedorId, bool esOperador);
        string EnviarSolicitudUsuario(string mailUsuario, int proveedorId, bool esGuardarYNotificar, AltaEmpresaViewModel altaEmpresa);
        string ObtenerArchivo(string mailUsuario, int archivoID, int proveedorId);
        string ObtenerArchivos(string mailUsuario, int proveedorId, string pathBase);
        string EliminarArchivo(string mailUsuario, int archivoID, int proveedorId);
        InfoProveedorDataAgroDto ObtenerInfoProveedor(string mailUsuario, int proveedorId);
        AltaEmpresaViewModel CargarSolicitudUsuario(string mailUsuario, int proveedorId);
        Task<string> ObtenerCampañasDataAgroAsync();

    }
}
