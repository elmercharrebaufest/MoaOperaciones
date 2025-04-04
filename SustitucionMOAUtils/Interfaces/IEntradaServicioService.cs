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
        Task<List<EntradaServicioCabeceraDto>> ServicioSAP_EntradasServicioCabecera(EntradaServicioParamsDto parametros, UsuarioDto usuario);
        List<EntradaServicioCabeceraDto> ServicioAprobaciones_EntradasServicioCabecera(EntradaServicioParamsDto parametros, UsuarioDto usuario);
        Task<EntradaServicioCreateRespuestaDto> CrearEntradaServicioAsync(EntradaServicioCreateParamsDto parametros, string userMail, List<ReporteDto> reporte, List<string> idAdjuntos, string solpedNumber = null, string proveedor = null);

        EntradaServicioCreateRespuestaDto CrearEntradaServicioTemporal(EntradaServicioCreateParamsDto parametros, string userMail, List<ReporteDto> reporte, List<string> idAdjuntos, string solpedNumber = null, string proveedor = null);

        EntradaServicioCreateRespuestaDto ValidarIngresante(EntradaServicioCreateParamsDto parametros, string userMail, string nroSolped);
        Task<bool> NotifyRejection(EmailDetailCertificateDto emailDetailCertificateDto);
        EntradaServicioRejectRespuestaDto RechazarEntradaDeServicio(EmailDetailCertificateDto rechazo);
        Task<EntradaServicioCreateRespuestaDto> AprobarEntradaDeServicio(string nro_es_local, string Moneda);

        List<Aprobaciones> GetESTemporaria(string nroESLocal);
        string GetCurrencyType(string NroOC);
        EntradaServicioReasignacionRespuestaDto ReasignarSuplente(string nro_es_local, string mail);
        string ActualizarInformacionIngresante(IngresanteInfoEditableDto info);

        Task NotificarReasignaciones(IEnumerable<string> ListaAp);
        void GenerarCertificacionAutomaticaPorLiberacionOC(string nroOC);
    }
}
