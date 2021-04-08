using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.Login;
using SustitucionMOAModel.Models.WSMapMOA.Usuario;
using SustitucionMOAModel.Models.WSMapMOA.Usuario.Perfil;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IUsuarioService
    {
        GetPerfilesResponseMOA getPerfiles();

        List<UsuarioDto> GetUsuarios();
        UsuarioDto GetUsuario(string email);

        string cambiarContrasenia(string username, string contraseniaActual, string contraseniaNueva);
        LoginWSMOAResponse registrar(string numeroProveedor, string claveActivacion, string username, string contrasenia);

        string alta(UsuarioAlta usuario);

        string recuperarContrasenia(string usename);

        string desbloquear(string usename);

        string HabilitarUsuario(string usuarioMail);

        string DeshabilitarUsuario(string usuarioMail);

        Rol ObtenerRolPorCodigo(string codigo);

        Dictionary<string, List<RolDropdownDto>> GetRoles();

        byte[] getDocumento(string nombre);
        string GuardarRoles(List<int> idRol, int idUsuario);
        List<ProveedorDto> GetVendedoresUsuario(string usuarioMail);

        List<RolDropdownDto> GetRolesUsuario(int idUsuario);
        List<Rol> GetRolesUsuario(string email);
        void SeccionVisitada(string mailUsuario, string seccion);
    }
}
