using SustitucionMOAModel.Models.DataAgro;
using System.Collections.Generic;
using System.Web;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IAltaEmpresaGranosService
    {
        byte[] GenerarInformeComercial(ParamInformeComercial informeComercial, string mailUsuario);
        string GuardarArchivo(HttpPostedFileBase fileSubido, string fileKey, string mailUsuario);
        string BorrarArchivo(string mailUsuario, string fileKey, int fileID);
        string ObtenerMaterialesDataAgro();
        Dictionary<string, string> ObtenerArchivosSubidos(string mail);
        string EnviarSolicitudUsuario(string mail);
        string ObtenerArchivo(string mail, string fileKey);
        string ObtenerCBUSISA(string mailUsuario);
    }
}
