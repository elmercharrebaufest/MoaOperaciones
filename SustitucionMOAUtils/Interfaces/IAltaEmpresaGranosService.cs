using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.DataAgro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IAltaEmpresaGranosService
    {
        byte[] GenerarInformeComercial(ParamInformeComercial informeComercial);
        bool GuardarArchivo(HttpPostedFileBase fileSubido, string fileKey, string mailUsuario);
        string ObtenerMaterialesDataAgro();
        Dictionary<string, string> ObtenerArchivosSubidos(string mail);
        string EnviarSolicitudUsuario(string mail);
        string ObtenerArchivo(string mail, string fileKey);
    }
}
