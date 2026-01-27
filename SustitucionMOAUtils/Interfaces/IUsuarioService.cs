using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.UsuarioDtos;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOAModel.Models.WSMapMOA.Vendedor;
using System.Collections.Generic;
using System.Linq;
using Models = SustitucionMOAModel.Models;

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
        string GuardarRoles(List<int> idRoles, int idUsuario, string usuarioSap);
        List<ProveedorDto> GetVendedoresUsuario(string usuarioMail);
        List<RolDropdownDto> GetRolesUsuario(int idUsuario);
        List<Rol> GetRolesUsuario(string email);
        UsuarioReasignacionDto GetPeriodoReasignacion(int idUsuario);
        List<Rol> GetRolesApiKey(string apikey);
        void SeccionVisitada(string mailUsuario, string seccion);
        ProveedorDto GetProveedorPorCodigo(string codigo, string mailUsuario);
        ProveedorDto VerificarYObtenerProveedor(string mailUsuario, string codigoCorredor, string codigoProveedor);
        string ObtenerNuevoApiKey(string usuario);
        List<ProveedorDto> ListarProveedores(string filtro);
        ResultadoGenerico GrabarProveedor(ProveedorDto proveedorDto, EstadoAprobacion estadoAprobacion = EstadoAprobacion.AltaIncompleta, bool mantenerEstadoAprobacionExistente = false, string mailUsuarioAdmin = "");
        UsuarioDto GetUsuarioPorId(int id);
        List<ProveedorDto> GetProvedoresEmail(int tipoProveedorId, string email, string cuitUsuario);
        List<TipoUsuarioDto> GetTipoUsuario();
        List<string> ValidarMailUsuario(UsuarioModificacionDto usuarioModificacionDto);
        string ModificarUsuario(UsuarioModificacionDto usuarioModificacionDto);
        List<ProveedorAuditoriaDto> GetProveedorAuditoriaPorUsuario(int usuarioId);
        IEnumerable<IGrouping<int, UsuarioDto>> ListarUsuarioCreadorSolp(UsuarioDto usuario);
        IEnumerable<string> ListarFiscalesSolp();
        ProveedorDto TraerProveedorEnSAP(string codigoProveedor, string codigoCorredor);
        string EliminarCuitNoHabilitado(int proveedorId, string mailUsuarioSesion);
        void AsignarNuevaCUIT(AsignarNuevaCuitDto datosAsignar, string mailUsuarioSesion);
        List<DestinatarioDto> ObtenerDestinatariosConsulta(int proveedorId);
        List<ProveedorDto> GetProveedoresUsuario(int usuarioId);
        void DesasociarVendedor(int usuarioId, int proveedorId, string mailUsuarioSesion);
        List<string> GetMailUsuarios(string mail);
        string ObtenerConfiguracion(string mailUsuario, TipoConfiguracionUsuario tipo);
        void GuardarConfiguracionUsuario(string mailUsuario, string valor, TipoConfiguracionUsuario tipo);

        ProveedorComprasDto ObtenerYCrearProveedorCompras(string codigoProveedor);
        ObtenerProveedorWSMOAResponse ObtenerProveedorSap(string codigoProveedor);
        VendedoresWSMOAResponse ObtenerVendedorSap(string codigoProveedor, List<Models.FechaWS> fechas);

        List<UsuarioComprasDto> ListarUsuarioCompras();
        void GuardarSuplente(int idUsuario, string suplente, string fechaDesde, string fechaHasta, bool esExterno);
        List<ProveedorARelacionar> GetProveedoresARelacionar(string cuit, string mailUsuario);
        List<string> GetMailsUsuariosAprobadores();
    }
}
