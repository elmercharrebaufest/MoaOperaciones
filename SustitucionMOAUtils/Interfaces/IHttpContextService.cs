using iTextSharp.text;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IHttpContextService
    {
        string ObtenerPathLogoMail();
        string GetDirectory(string path);
        Image ObtenerLogoImagen();
    }
}