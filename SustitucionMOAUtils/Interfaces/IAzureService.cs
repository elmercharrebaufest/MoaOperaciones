using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IAzureService
    {
        Task<string> AnalizarImagenAsync(HttpPostedFileBase file);
        Task<IList<string>> ObtenerResultadoOCRAsync(string operacionId);
        Task SubirArchivoABlobStorageAsync(HttpPostedFileBase archivo, string coe);
    }
}
