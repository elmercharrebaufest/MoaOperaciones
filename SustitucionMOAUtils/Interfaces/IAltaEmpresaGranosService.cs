using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.DataAgro;
using System.Collections.Generic;
using System.Web;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IAltaEmpresaGranosService
    {
        byte[] GenerarInformeComercial(ParamInformeComercial informeComercial, string mailUsuario);
        string GuardarArchivo(HttpPostedFileBase fileSubido, string fileKey, string mailUsuario);
        string ObtenerMaterialesDataAgro();
        List<ArchivoDto> ObtenerArchivosSubidos(string mail);
        string EnviarSolicitudUsuario(string mail);
        string ObtenerArchivo(string mail, int fileID);
        string EliminarArchivo(string mail, int archivoID);
        string ObtenerCBUSISA(string mailUsuario);
        InfoProveedorDataAgroDto ObtenerInfoProveedor(string mailUsuario);

    }
}
