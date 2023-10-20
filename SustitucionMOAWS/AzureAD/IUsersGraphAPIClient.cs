using SustitucionMOAWS.AzureAD.Model;

namespace SustitucionMOAWS.AzureAD
{
    public interface IUsersGraphAPIClient
    {
        void BorrarUsuario(string idUsuario);

        ObtenerUsuarioResponse ObtenerUsuarioPorDisplayName(string displayName);
    }
}