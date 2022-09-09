using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOAWS.WSConsumers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IComprasService
    {
        RespuestaGuardarSOLP GuardarSolp(SolpDto solp, HttpFileCollectionBase adjuntos);
        string ObtenerRutaArchivo(int archivoId);

        List<TablaSapDto> ObtenerTablaSap(string tabla);
        List<TablaGeneralDto> ObtenerTablaGeneral(string tabla);
        List<CentroDireccionDto> ObtenerCentrosDireccion();
        List<SolpDto> ListarSolp(UsuarioDto usuarioActual);
        string BorrarSolp(int idSolp);
        SolpDto TraerSolpId(int idSolp);
        List<TablaEstadoDto> ObtenerTablaEstado(string tabla);
        byte[] GenerarSolpPdf(int idSolp);
        string GenerarZipPliego(int idSolp, string pathBase);
        List<TablaSapDto> ObtenerServiciosSap();
        List<TablaSapDto> AutocompleteTablaSap(string tabla, string valor);
        List<TablaSapDto> ObtenerCecoSap();
        List<TablaSapDto> ObtenerCuentasSap();
        List<TablaSapDto> ObtenerOrdenesSap(string idOrder = "");
        List<TablaSapDto> ObtenerDatosPorCodigosSap(List<TablaSapDto> codigos);
        void ActualizarMaterialesSolp();
        void ActualizarFechaLiberacion(string nrosolp, DateTime fechaLiberacion);
        void ActualizarServiciosSolp();
        List<ServicioSolpDto> ObtenerDatosPorCodigosSapServicioSolp(List<string> codigos);
        List<ServicioSolpDto> AutocompleteServicioSolp(string valor);
        void ActualizarEstadoSolpBulk();
        void ActualizarEstadoSolp(string nroSolp, int idEstado);
        List<UsuarioComprasRelacionConUsuariosDto> ListarUsuarioCompras(UsuarioDto usuarioActual);
        void ObtenerSolpesDesdeSAPJob(ObtenerSolpRequest obtenerSolpRequest);
        List<MaterialSolpDto> AutocompleteMaterialSolp(string valor, int centroId);
        List<ProvinciaDTO> ListarProvincia();
        void EnviarEmailSolp(EmailComposeDto emailCompose);
        SolpDescargaZipPorLink PuedeDescargarPliegoDesdeLink(int solpId, Guid? token);
        List<FuenteAprovisionamientoDto> ListarFuenteAprovisionamiento(string fechaEntregaPosicion, string numeroMaterial, string centro);
        List<ContratoSolp> ObtenerContratoMarco(string numeroContrato, string centro);
    }
}
