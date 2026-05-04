using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.Compras;
using SustitucionMOAModel.Dto.OrdenesCompra;
using SustitucionMOAModel.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IEntradaServicioService
    {
        Task<List<EntradaServicioCabeceraDto>> ObtenerEntradasServicioCompleta(EntradaServicioParamsDto parametros, UsuarioDto usuario);
        string BorrarEntradaServicio(EntradaServicioParamsDto parametros, UsuarioDto usuarioActual);
        List<EntradaServicioCabeceraDto> ServicioAprobaciones_EntradasServicioCabecera(EntradaServicioParamsDto parametros, UsuarioDto usuario);
        List<EntradaServicioCabeceraDto> ObtenerESLocales(EntradaServicioParamsDto parametros, UsuarioDto usuario);
        Task<List<EntradaServicioCabeceraDto>> ObtenerESAprobadasSAP(EntradaServicioParamsDto parametros, UsuarioDto usuario);        
        List<EntradaServicioCreateRespuestaDto> CrearEntradaServicio(CreateEntradaServicioDto crearESRequestDto, string mailUsuario);
        Task<bool> NotifyRejection(EmailDetailCertificateDto emailDetailCertificateDto);
        EntradaServicioRejectRespuestaDto RechazarEntradaDeServicio(EmailDetailCertificateDto rechazo);
        Task<EntradaServicioCreateRespuestaDto> AprobarEntradaDeServicio(string nro_es_local, string Moneda);

        List<Aprobaciones> GetESTemporaria(string nroESLocal);
        string GetCurrencyType(string NroOC);
        EntradaServicioReasignacionRespuestaDto ReasignarSuplente(string nro_es_local, string suplenteOriginal, bool notificarAprobacionPendiente);
        string ActualizarInformacionIngresante(IngresanteInfoEditableDto info);

        Task NotificarReasignaciones(IEnumerable<string> ListaAp);
        void GenerarCertificacionAutomaticaPorLiberacionOC(string nroOC);
        void CertificarOrdenesDeCompraConContratoMarco();
    }
}
