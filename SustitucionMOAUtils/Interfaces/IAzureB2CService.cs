using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.Noticia;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IAzureB2CService
    {
        Usuario LoguearUsuario(string mail, string CUIT, string granosFlag);

        Usuario ObtenerUsuario(string mail, string granosFlag);

        NoticiasDetallesWSMOAResponse getNoticias(string proveedor);

    }
}
