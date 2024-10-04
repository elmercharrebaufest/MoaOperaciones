using SustitucionMOAModel.Consultas;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.Compras;
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
        RespuestaGuardarSOLP GuardarSolp(SolpDto solp, HttpFileCollectionBase adjuntos);
        string ObtenerRutaArchivo(int archivoId);
        List<TablaSapDto> ObtenerTablaSap(string tabla);
        List<TablaSapDto> ListarTablaSap(List<string> tabla);
        List<TablaGeneralDto> ObtenerTablaGeneral(string tabla);
        List<CentroDireccionDto> ObtenerCentrosDireccion();
        ListaPaginada<SolpDto> ListarSolp(UsuarioDto usuarioActual, Paginacion paginacion, string nroSolp, string nombrePedido, DateTime? desde, DateTime? hasta, bool sap, bool mantenimiento, bool web, bool repoAutomatica, bool contratoMarco, List<int> usuarios = null, List<int> estados = null, List<int> centros = null, List<int> grupoDeCompras = null, List<int> claseDocumento = null, List<string> tipoImputacion = null, List<int> valorTipoImputacion = null);
        string BorrarSolp(int idSolp);
        SolpDto TraerSolpId(int idSolp);
        SolpESDto TraerSolpPorNumero(string nroSolp);
        List<TablaEstadoDto> ObtenerTablaEstado(string tabla);
        byte[] GenerarSolpPdf(int idSolp);
        Pdf GenerarPeticionDeOfertaUsuarioPdf(int idPeticionDeOfertaUsuario);
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
        List<ServicioSolpDto> AutocompleteCodigoServicioSolp(string valor);

        List<ProveedorDto> AutocompleteProveedor(string valor);
        void ActualizarEstadoSolpBulk();
        List<UsuarioComprasDto> ListarUsuarioCompras();
        void ObtenerSolpesDesdeSAPJob(ObtenerSolpRequest obtenerSolpRequest);
        List<MaterialSolpDto> AutocompleteMaterialSolp(string valor, int centroId);
        List<MaterialSolpDto> AutocompleteCodigoMaterialSolp(string valor, int centroId);
        List<ProvinciaDto> ListarProvincia();
        void EnviarEmailSolp(EmailComposeDto emailCompose);
        SolpDescargaZipPorLink PuedeDescargarPliegoDesdeLink(int solpId, Guid? token);
        List<FuenteAprovisionamientoDto> ListarFuenteAprovisionamiento(string fechaEntregaPosicion, string numeroMaterial, string centro);
        List<ContratoSolp> ObtenerContratoMarco(string numeroContrato, string centro);
        ListaPaginada<SolpDto> ListarSolpComprador(int usuario_Id, Paginacion paginacion, string nroSolp, string nombrePedido, DateTime? desde, DateTime? hasta, bool sap, bool mantenimiento, bool web, bool repoAutomatica, EstadoListarTratamientoSolp listarPendiente, bool contratoMarco, List<int> usuarios = null, List<int> estados = null, List<int> centros = null, List<int> grupoDeCompras = null, List<int> claseDocumento = null, List<string> tipoImputacion = null, List<int> valorTipoImputacion = null);
        List<AsociarContratoDto> DevolverContratosAsociados(List<SolpPosicionDto> posiciones);
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
        RespuestaGuardarSOLP GrabarRevisionTecnica(List<PeticionDeOfertaUsarioDto> peticionDeOfertaUsuarioDto, int usuarioId, bool finalizar, PeticionDeOfertaRevisionTecnicaDto revision);
        PeticionDeOfertaDto TraerCotizacion(int peticionId);
        RespuestaGuardarSOLP GrabarCotizacion(GuardarCotizacion cotizacionDto, HttpFileCollectionBase adjuntos, bool esFinalizado, int usuarioActualId, bool enviarMail);
        GuardarCotizacion ObtenerPrecioTotalPosicionProveedor(GuardarCotizacion cotizacionDto);
        RespuestaCrearOrdenDeCompra GrabarAdjudicacion(AdjudicacionDto adjudicacionDto, int usuarioActualId, string mensaje);
        List<AdjudicacionDto> ListarAdjudicaciones(int solpId);
        AdjudicacionDto ObtenerAdjudicacion(int adjudicacionId);
        AdjudicacionDto ObtenerAdjudicacion(string nroOC);
        OrdenDeCompraSAPDto ObtenerOrdenDeCompra(string nroOC);
        List<RespuestaCrearOrdenDeCompra> CrearOrdenDeCompraConRegistroInfo(List<RegistroInfoDto> registros, int usuarioActualId);
        RespuestaGuardarSOLP CerrarCotizacion(int peticionId, int usuarioActualId, string observaciones);
        DatosUltimaSolpDto ObtenerUltimaSolp(int usuarioId);
        RegistroInfoDto ObtenerUltimoRegistroMaterial(string material, string centro, string grupoDeCompras);
        void ActualizarFechaLiberacionOC(string nroOc, DateTime fechaLiberacion);
        LegajoExternoDto ObtenerLegajoParaExternos(int adjudicacionId, string token, string mailUsuario);
        List<OrdenDeCompraSAPDto> ObtenerReporteOrdenDeCompra(string nroOC, string fechaDesde, string fechaHasta, string codigoProveedor);
        Resultado GrabarPeticionDeOfertaVisualizacionPrecio(PeticionDeOfertaVisualizacionPrecioDto peticionDeOfertaVisualizacionPrecioDto, HttpFileCollectionBase adjuntos);
        ChatsDto ObtenerChat(int solpId, int usuarioActualId);
        Resultado GrabarMensajeChatInterno(ChatInternoComprasDto mensaje);
        string ExportarChatInternoAtexto(int solpId, string rutaArchivo, int? peticionDeOfertaUsuarioId);
        ProveedorComprasDto DevolverMonedaProveedor(string codigoProveedor);
        List<RegionSap> ListarRegionesSap();
        bool ValidarSolpTratada(string nroSolp);
        List<LiberadorSapDto> ListarLiberadorSap();
        List<TablaSapDto> ListarUnidadesDeMedida(string material);
        ResultadoGenerico EditarOrdenDeCompra(AdjudicacionDto adjudicacion);
        InfoVisitasDeObraDto ListarVisitasDeObra(List<VisitaObraDto> visitas);
        List<TablaGeneralDto> ObtenerImputaciones(string tabla);
        void ObtenerDatosReporteSolp();

        List<PeticionDeOfertaDto> ListarPeticionesDeOferta(int solpId);
        List<int> ListarClaseDocumento(int usuarioId);
        Resultado ActualizarProveedorVisibleEnSolicitante(int peticionDeOfertaUsuarioId, bool esVisible);
        List<UsuarioDto> ListarUsuarioSolicitante();
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
    }
}