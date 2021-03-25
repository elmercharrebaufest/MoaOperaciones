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

        List<UsuarioDto> GetUsuarios();
        UsuarioDto GetUsuario(string email);

        string HabilitarUsuario(string usuarioMail);

        string DeshabilitarUsuario(string usuarioMail);

        Rol ObtenerRolPorCodigo(string codigo);

        List<RolDropdownDto> GetRoles();

        byte[] getDocumento(string nombre);
        string GuardarRoles(List<int> idRol, int idUsuario);
        List<ProveedorDto> GetVendedoresUsuario(string usuarioMail);

        List<RolDropdownDto> GetRolesUsuario(int idUsuario);
        List<Rol> GetRolesUsuario(string email);
        void SeccionVisitada(string mailUsuario, string seccion);
    }
}
