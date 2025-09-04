using SustitucionMOAModel.Consultas;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.Compras;
using SustitucionMOAModel.Dto.Compras.POMultiple;
using SustitucionMOAModel.Dto.Compras.PrecargaSolp;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.WSMapMOA;
using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOAWS.WSConsumers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IComprasService
    {
        Pliego GuardarPliego(SolpDto solp,
                           HttpFileCollectionBase adjuntos,
                           bool condEsp,
                           string rutaArchivos = null,
                           Pliego pliegoEntity = null,
                           bool esPliegoMultiple = false);

        RespuestaGuardarSOLP GuardarSolp(SolpDto solp, HttpFileCollectionBase adjuntos);
        string ObtenerRutaArchivo(int archivoId);
        List<TablaGeneralDto> ObtenerTablaGeneral(string tabla);
        List<TablaGeneralDto> ObtenerTiposPosicionSolp();
        List<TablaSapDto> ObtenerMonedas();
        List<TablaSapDto> ObtenerGrupoCompras();
        List<TablaSapDto> ObtenerGrupoArticulos();
        List<TablaSapDto> ObtenerCentros();
        List<TablaSapDto> ObtenerAlmacenes();
        List<TablaSapDto> ObtenerUnidades();
        List<CentroDireccionDto> ObtenerCentrosDireccion();
        string BorrarSolp(int idSolp);
        SolpDto TraerSolpPliego(int idPliego, out int solpCount);
        SolpDto TraerSolpId(int idSolp);
        SolpESDto TraerSolpPorNumero(string nroSolp);
        List<TablaEstadoDto> ObtenerTablaEstado(string tabla);
        byte[] GenerarSolpPdf(int id, bool esPliego = false);
        Pdf GenerarPeticionDeOfertaUsuarioPdf(int idPeticionDeOfertaUsuario);
        string GenerarZipPliego(int idSolp, string pathBase, out string mimeType);
        string AgregarArchivosAlZipPliego(IEnumerable<Archivo> archivos, string middleFileName, string pathBase, bool pdfPliegoDisponible, string pdfFilePath = null, string pdfFilename = null);
        List<TablaSapDto> ObtenerDatosPorCodigosSap(List<TablaSapDto> codigos);
        void ActualizarFechaLiberacion(string nrosolp, DateTime fechaLiberacion);
        void ActualizarServiciosSolp();
        void ActualizarServicioSolpDadoCodigo(string codigo);
        List<ServicioSolpDto> ObtenerDatosPorCodigosSapServicioSolp(List<string> codigos);
        List<ServicioSolpDto> AutocompleteServicioSolp(string valor);
        List<ServicioSolpDto> AutocompleteCodigoServicioSolp(string valor);

        List<ProveedorDto> AutocompleteProveedor(string valor);
        void ActualizarEstadoSolpBulk();
        void ObtenerSolpesDesdeSAPJob(ObtenerSolpRequest obtenerSolpRequest);
        List<MaterialSolpDto> AutocompleteMaterialSolp(string valor, int centroId);
        List<ProvinciaDto> ListarProvincia();
        void EnviarEmailSolp(EmailComposeDto emailCompose);
        SolpDescargaZipPorLink PuedeDescargarPliegoDesdeLink(int solpId, Guid? token);
        ListaPaginada<SolpDto> ListarSolpComprador(int usuario_Id, Paginacion paginacion, string nroSolp, string nombrePedido, DateTime? desde, DateTime? hasta, bool sap, bool mantenimiento, bool web, bool repoAutomatica, EstadoListarTratamientoSolp listarPendiente, bool contratoMarco, List<int> usuarios = null, List<int> estados = null, List<int> centros = null, List<int> grupoDeCompras = null, List<int> claseDocumento = null, List<string> tipoImputacion = null, List<int> valorTipoImputacion = null, TipoPliego tipoPliego = TipoPliego.All);
        SolpCompraDto ObtenerSolpCompras(int id);
        RespuestaGuardarSOLP GrabarPeticionDeOferta(GuardarPeticionDeOfertaDto peticionDeOferta, HttpFileCollectionBase adjuntos, bool enviarMail, List<RegistroInfoDto> registroInfo);
        ObtenerLegajoResponse ObtenerLegajo(int peticionDeOfertaId, int? idPeticionDeOfertaUsuario, bool esProveedor, string mailUsuario, bool esSolicitante);
        Resultado GuardarAdjuntosPeticionDeOferta(int idPeticion, HttpFileCollectionBase files, UsuarioDto usuarioDto);
        string DescargarLegajo(int idPeticion, string path, int? idPeticionDeOfertaUsuario, bool esProveedor, int? adjudicacionId, string mailUsuario, bool esSolicitante);
        RespuestaGuardarSOLP GrabarCircular(CircularDto circularDto, HttpFileCollectionBase adjuntos, bool esAutomatico = false, List<PeticionDeOfertaUsuario> usuarios = null);
        PeticionDeOfertaDto ObtenerPeticionDeOfertaParaCircular(int peticionId);
        RespuestaGuardarSOLP GrabarProveedoresEnPeticionDeOferta(List<int> usuariosId, int peticionId);
        ListaPaginada<PeticionDeOfertaDto> ListarPOProveedor(Paginacion paginacion, string nroSolp, string nroPo, string nombrePedido, string username, DateTime? desde, DateTime? hasta, int? estadoLicitacion, int? estadoCotizacion);
        PeticionDeOfertaDto ListarOfertasComprador(int PeticionOferta_Id, UsuarioDto usuario);
        string DescargarAdjuntosCotizacion(int idCotizacion, string pathBase, bool desdeRevisionTecnica);
        RespuestaGuardarSOLP GrabarRevisionTecnica(List<PeticionDeOfertaUsuarioDto> peticionDeOfertaUsuarioDto, int usuarioId, bool finalizar, PeticionDeOfertaRevisionTecnicaDto revision);
        PeticionDeOfertaDto TraerCotizacion(int peticionId);
        RespuestaGuardarSOLP GrabarCotizacion(GuardarCotizacion cotizacionDto, HttpFileCollectionBase adjuntos, bool esFinalizado, bool enviarMail);
        GuardarCotizacion ObtenerPrecioTotalPosicionProveedor(GuardarCotizacion cotizacionDto);
        RespuestaCrearOrdenDeCompra GrabarAdjudicacion(AdjudicacionDto adjudicacionDto, int usuarioActualId, string mensaje);
        OrdenDeCompraSAPDto ObtenerOrdenDeCompra(string nroOC);
        List<RespuestaCrearOrdenDeCompra> CrearOrdenDeCompraConRegistroInfo(List<RegistroInfoDto> registros, int usuarioActualId);
        RespuestaGuardarSOLP CerrarCotizacion(int peticionId, int usuarioActualId, string observaciones);
        void ActualizarFechaLiberacionOC(string nroOc, DateTime fechaLiberacion);

        void AdjustUnitPriceAndQuantity(RegistroInfoDto registro, decimal cantidad, decimal precio, UnidadesDeMedida unidadActual, UnidadesDeMedida unidadObjetivo);

        LegajoExternoDto ObtenerLegajoParaExternos(int adjudicacionId, string token, string mailUsuario);
        Resultado GrabarPeticionDeOfertaVisualizacionPrecio(PeticionDeOfertaVisualizacionPrecioDto peticionDeOfertaVisualizacionPrecioDto, HttpFileCollectionBase adjuntos);
        ChatsDto ObtenerChat(int solpId, int usuarioActualId);
        Resultado GrabarMensajeChatInterno(ChatInternoComprasDto mensaje);
        string ExportarChatInternoAtexto(int solpId, string rutaArchivo, int? peticionDeOfertaUsuarioId);
        ProveedorComprasDto DevolverMonedaProveedor(string codigoProveedor);
        List<RegionSap> ListarRegionesSap();
        bool ValidarSolpTratada(string nroSolp);
        List<LiberadorSapDto> ListarLiberadorSap();
        List<TablaGeneralDto> ObtenerTiposImputaciones();

        bool TieneCondicionEspecial(Solp solp);
        bool TieneCondicionEspecial(SolpDto solp);

        string ObtenerRutaArchivos(int id, string path);

        void ObtenerDatosReporteSolp();

        List<PeticionDeOfertaDto> ListarPeticionesDeOferta(int solpId);
        List<int> ListarClaseDocumento(int usuarioId);
        Resultado ActualizarProveedorVisibleEnSolicitante(int peticionDeOfertaUsuarioId, bool esVisible);
        List<CotizacionHistorialDto> ObtenerHistorial(int id);
        List<POPosicionDto> ListarPosicionesPOMultiple(DateTime? desde,
                                                       DateTime? hasta,
                                                       bool sap,
                                                       bool mantenimiento,
                                                       bool web,
                                                       bool repoAutomatica,
                                                       bool? tratada,
                                                       bool contratoMarco,
                                                       List<int> centros = null,
                                                       List<int> grupoDeCompras = null,
                                                       List<int> claseDocumento = null,
                                                       List<string> tipoImputacion = null,
                                                       List<int> valorTipoImputacion = null,
                                                       int? numeroPo = null);
        List<SolpCrearPoMultipleDto> ListarPosicionesPOMultipleServicio(DateTime? desde,
                                                                        DateTime? hasta,
                                                                        bool sap,
                                                                        bool mantenimiento,
                                                                        bool web,
                                                                        bool repoAutomatica,
                                                                        bool? tratada,
                                                                        bool contratoMarco,
                                                                        List<int> centros = null,
                                                                        List<int> grupoDeCompras = null,
                                                                        List<int> claseDocumento = null,
                                                                        List<string> tipoImputacion = null,
                                                                        List<int> valorTipoImputacion = null,
                                                                        int? numeroPo = null,
                                                                        string nombrePliego = null,
                                                                        TipoPliego tipoPliego = TipoPliego.All);

        IEnumerable<PosicionCrearPoMultipleDto> ListarPosicionesPOMultipleSolpId(int idSolp);
        IEnumerable<SubPosicionCrearPoMultipleDto> ListarSubPosicionesPOMultipleSolpId(int idPosicion);

        MemoryStream DescargarPosicionesPOMultiple(DateTime? desde,
                                                   DateTime? hasta,
                                                   bool sap,
                                                   bool mantenimiento,
                                                   bool web,
                                                   bool repoAutomatica,
                                                   bool? tratada,
                                                   bool contratoMarco,
                                                   List<int> centros = null,
                                                   List<int> grupoDeCompras = null,
                                                   List<int> claseDocumento = null,
                                                   List<string> tipoImputacion = null,
                                                   List<int> valorTipoImputacion = null,
                                                   int? numeroPo = null);

        MemoryStream DescargarPosicionesPOMultipleServicio(DateTime? desde,
                                                   DateTime? hasta,
                                                   bool sap,
                                                   bool mantenimiento,
                                                   bool web,
                                                   bool repoAutomatica,
                                                   bool? tratada,
                                                   bool contratoMarco,
                                                   List<int> centros = null,
                                                   List<int> grupoDeCompras = null,
                                                   List<int> claseDocumento = null,
                                                   List<string> tipoImputacion = null,
                                                   List<int> valorTipoImputacion = null,
                                                   int? numeroPo = null,
                                                   string nombrePliego = null);

        SolpCompraDto ObtenerPosicionesMultipleCompras(List<int> listaId);

        HistorialDeFechaDto ListarHistorialDeFechas(int peticionDeOfertaId);
        Resultado GrabarMensajeChatExterno(ChatExternoComprasDto mensaje);
        ChatsDto ObtenerChatProveedor(int peticionDeOfertaUsuarioId, int usuarioActualId);
        void MarcarChatProveedorComoLeido(ChatProveedoresDto proveedor);
        void ExecuteObtenerSolpesDesdeSAPJob(ObtenerSolpRequest obtenerSolpRequest);
        List<SolpDto> ListarSolpCondicionEspecial(FiltroDto filtroDto);
        Resultado AgruparPeticionesDeOferta(int usuarioId, string ids);
        RespuestaCrearOrdenDeCompra ValidarPrecioCotizado(AdjudicacionDto adjudicacionDto);
        AdjuntosSolpDto ObtenerAdjuntosSolpAgrupar(string nroSolp);
        Resultado DesagruparPO(string nroSolp, string po);
        Resultado GuardarEnvioCircularProveedor(int id, EnviarCircularEnum envioCircularA, DateTime? fechaLimite);
        ValidarFechaVigenciaRegistroInfoResDto ValidarFechaVigenciaRegistroInfo(ValidarFechaVigenciaRegistroInfoReqDto request);
        void ActualizarFechaVigenciaRegistroInfo(ActualizarFechaVigenciaRegistroInfoDto datos);
        byte[] GenerarExcelHistorialMovimientos(int peticionDeOfertaId);
        byte[] GenerarHistorialCotizaciones(int cotizacionId);
        byte[] GenerarArchivoRevisionTecnica(int peticionDeOfertaId);

        string DescargarAdjuntosProveedores(int idPeticion, string path, int? idPeticionDeOfertaUsuario);

        List<KeyValuePair<EstadoListarTratamientoSolp, string>> ListarPendienteListComboOptions();
        ProcesarPrecargaSolpResponse ProcesarArchivoPrecargaSolp(HttpPostedFileBase archivo, int tipoSolpId);
        void GuardarCertificacionesParciales(List<AdjudicacionDto> adjudicaciones);
        AdjudicacionDto ObtenerAdjudicacion(string nroOC);
        List<PeticionDeOfertaDesvincularDto> ObtenerPeticionesDeOfertaParaDesvincularMaterial(int solpPosicionId);
        List<PeticionDeOfertaDesvincularDto> ObtenerPeticionesDeOfertaParaDesvincularServicio(int solpId);
        void DesvincularSolpDePOMultipleMaterial(int solpPosicionId, List<int> idsPOsADesvincular);
        void DesvincularSolpDePOMultipleServicio(int solpId, List<int> idsPOsADesvincular);
    }
}