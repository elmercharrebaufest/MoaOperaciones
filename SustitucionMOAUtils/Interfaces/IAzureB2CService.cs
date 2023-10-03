using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.Noticia;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IAzureB2CService
    {
        Usuario LoguearUsuario(string mail, string CUIT, string granosFlag);

        NoticiasDetallesWSMOAResponse ObtenerNoticias(string proveedor);
        bool RegistrarUsuarioGranos(ref UsuarioGranos usuario);
        bool RegistrarUsuarioNoGranos(ref UsuarioNoGranos usuario);
        bool RegistrarUsuarioCorredor(ref Usuario usuario);
        bool RegistrarUsuarioGenerico(ref Usuario usuario);
        bool RegistrarUsuarioCliente(ref Usuario usuario);
        bool ExisteUsuario(Usuario usuario);
        bool ValidarCUITProveedor(ref UsuarioGranos usuario, Proveedor proveedor);
        Usuario ObtenerUsuario(string mail, string granosFlag);


    }
}
