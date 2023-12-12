using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.WSMapMOA.Login;
using SustitucionMOAModel.Models.WSMapMOA.Usuario;
using SustitucionMOAModel.Models.WSMapMOA.Usuario.Perfil;
using System.Collections.Generic;
using System.Linq;

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
        string GuardarRoles(List<int> idRol, int idUsuario, string usuarioSap);
        List<ProveedorDto> GetVendedoresUsuario(string usuarioMail);
        List<RolDropdownDto> GetRolesUsuario(int idUsuario);
        List<Rol> GetRolesUsuario(string email);
        List<Rol> GetRolesApiKey(string apikey);
        void SeccionVisitada(string mailUsuario, string seccion);
        ProveedorDto GetProveedorPorCodigo(string codigo, string mailUsuario);
        ProveedorDto VerificarYObtenerProveedor(string mailUsuario, string codigoCorredor, string codigoProveedor);
        string ObtenerNuevoApiKey(string usuario);
        List<ProveedorDto> ListarProveedores(string filtro);
        ResultadoGenerico GrabarProveedor(ProveedorDto proveedorDto, EstadoAprobacion estadoAprobacion = EstadoAprobacion.DocumentacionPendiente);
        UsuarioDto GetUsuarioPorId(int id);
        List<ProveedorDto> GetProvedoresEmail(int tipoProveedorId, string email, string cuitUsuario);
        List<TipoUsuarioDto> GetTipoUsuario();
        List<string> ValidarMailUsuario(UsuarioModificacionDto usuarioModificacionDto);
        string ModificarUsuario(UsuarioModificacionDto usuarioModificacionDto);
        List<ProveedorAuditoriaDto> GetProveedorAuditoriaPorUsuario(int usuarioId);
        IEnumerable<IGrouping<int, UsuarioDto>> ListarUsuarioCreadorSolp();
        ProveedorDto TraerProveedorEnSAP(string codigoProveedor, string codigoCorredor);
        string EliminarCuitNoHabilitado(int proveedorId, string mailUsuarioSesion);
        ProveedorDto GetProveedorAprobadoPorCuit(string cuit, string mailUsuarioSesion);
        void AsignarNuevaCUIT(AsignarNuevaCuitDto datosAsignar, string mailUsuarioSesion);
    }
}
