[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(SustitucionMOAExternalAPI.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(SustitucionMOAExternalAPI.App_Start.NinjectWebCommon), "Stop")]

namespace SustitucionMOAExternalAPI.App_Start
{
    using System;
    using System.Data.Entity;
    using System.ServiceModel;
    using System.Web;
    using System.Web.Http;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;
    using Ninject.Web.Common.WebHost;
    using Ninject.Web.WebApi;
    using SustitucionMOAExternalAPI.Handlers;
    using SustitucionMOAExternalAPI.Managers;
    using SustitucionMOARepositorio;
    using SustitucionMOAUtils.Helpers;
    using SustitucionMOAUtils.Interfaces;
    using SustitucionMOAUtils.Interfaces.Helpers;
    using SustitucionMOAUtils.Interfaces.Validadores;
    using SustitucionMOAUtils.Interfaces.Wrappers;
    using SustitucionMOAUtils.Services;
    using SustitucionMOAUtils.Validadores;
    using SustitucionMOAUtils.Wrappers;
    using SustitucionMOAWS.Interfaces;
    using SustitucionMOAWS.ScatoWebService;
    using SustitucionMOAWS.WebApi;
    using SustitucionMOAWS.WSConsumers;

    public static class NinjectWebCommon
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application.
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
                GlobalConfiguration.Configuration.DependencyResolver = new NinjectDependencyResolver(kernel);
                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {

            kernel.Bind<ICartaPorteService>().To(typeof(CartaPorteService)).InScope(ctx => OperationContext.Current);
            kernel.Bind<ILocalidadService>().To(typeof(LocalidadService)).InScope(ctx => OperationContext.Current);
            kernel.Bind<ICuentaCorrienteService>().To(typeof(CuentaCorrienteService)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IAltaEmpresaGranosService>().To(typeof(AltaEmpresaGranosService)).InScope(ctx => OperationContext.Current);
            kernel.Bind<ICrearContratoService>().To(typeof(CrearContratoService)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IAltaEmpresaService>().To(typeof(AltaEmpresaService)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IAzureB2CService>().To(typeof(AzureB2CService)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IDataAgroService>().To(typeof(DataAgroService)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IUsuarioService>().To(typeof(UsuarioService)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IContactoMailService>().To(typeof(ContactoMailService)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IConsultaService>().To(typeof(ConsultaService)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IComprasService>().To(typeof(ComprasService)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IVendedorService>().To(typeof(VendedorService)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IAltaEmpresaNoGranosService>().To(typeof(AltaEmpresaNoGranosService)).InScope(ctx => OperationContext.Current);
            kernel.Bind<ILiquidacionService>().To(typeof(LiquidacionService)).InScope(ctx => OperationContext.Current);
            kernel.Bind<INotificacionService>().To(typeof(NotificacionService)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IOrdenDeCargaService>().To(typeof(OrdenDeCargaService)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IAplicacionCartaPorteService>().To(typeof(AplicacionCartaPorteService)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IFeriadoService>().To(typeof(FeriadoService)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IAzureService>().To(typeof(AzureService)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IReportesService>().To(typeof(ReportesService)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IReporteContratoService>().To(typeof(ReporteContratoService)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IScatoConsumer>().To(typeof(ScatoConsumer)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IScatoComandosConsumer>().To(typeof(ScatoComandosConsumer)).InScope(ctx => OperationContext.Current);
            kernel.Bind<ITicketPesadaService>().To(typeof(TicketPesadaService)).InScope(ctx => OperationContext.Current);
            kernel.Bind<ICampoSustentableService>().To(typeof(CampoSustentableService)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IHomeService>().To(typeof(HomeService)).InScope(ctx => OperationContext.Current);
            kernel.Bind<ILogPesificacionService>().To(typeof(LogPesificacionService)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IValidadorPesificacion>().To(typeof(ValidadorPesificacion)).InScope(ctx => OperationContext.Current);
            kernel.Bind<ITimeProvider>().To(typeof(CurrentTimeProvider)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IGestionImpuestosService>().To(typeof(GestionImpuestosService)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IPesificacionService>().To(typeof(PesificacionService)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IExcelExportWrapper>().To(typeof(ExcelExportWrapper)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IFileWrapper>().To(typeof(FileWrapper)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IOrdenDeCargaFasonService>().To(typeof(OrdenDeCargaFasonService)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IEcheqService>().To(typeof(EcheqService)).InScope(ctx => OperationContext.Current);

            #region InterfacesSAP
            kernel.Bind<IVendedorHabilitadoConsumerMOA>().To(typeof(VendedorHabilitadoConsumerMOA)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IOrdenCargaConsumerMOA>().To(typeof(OrdenCargaConsumerMOA)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IListarPesificacionesConsumer>().To(typeof(ListarPesificacionesConsumer)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IObtenerCecoSolpConsumerMOA>().To(typeof(ObtenerCecoSolpConsumerMOA)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IObtenerCuentasSolpConsumerMOA>().To(typeof(ObtenerCuentasSolpConsumerMOA)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IObtenerOrdenSolpConsumerMOA>().To(typeof(ObtenerOrdenSolpConsumerMOA)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IObtenerServiciosSolpConsumerMOA>().To(typeof(ObtenerServiciosSolpConsumerMOA)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IObtenerMaterialesSolpConsumerMOA>().To(typeof(ObtenerMaterialesSolpConsumerMOA)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IObtenerSolpConsumerMOA>().To(typeof(ObtenerSolpConsumerMOA)).InScope(ctx => OperationContext.Current);
            kernel.Bind<ICrearSolpConsumerMOA>().To(typeof(CrearSolpConsumerMOA)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IModificarSolpConsumerMOA>().To(typeof(ModificarSolpConsumerMOA)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IReporteContratoConsumerMOA>().To(typeof(ReporteContratoConsumerMOA)).InScope(ctx => OperationContext.Current);

            kernel.Bind<ICrearPedidoConsumerMOA>().To(typeof(CrearPedidoConsumerMOA)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IObtenerFuenteAprovisionamientoConsumerMOA>().To(typeof(ObtenerFuenteAprovisionamientoConsumerMOA)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IObtenerContratoSolpConsumerMOA>().To(typeof(ObtenerContratoSolpConsumerMOA)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IEcheqVisualizarPendientePagoConsumerMOA>().To(typeof(EcheqVisualizarPendientePagoConsumerMOA)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IEcheqModificarContratoConsumerMOA>().To(typeof(EcheqModificarContratoConsumerMOA)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IEcheqModificarFijacionConsumerMOA>().To(typeof(EcheqModificarFijacionConsumerMOA)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IEcheqModificacionDocumentoChequeConsumerMOA>().To(typeof(EcheqModificacionDocumentoChequeConsumerMOA)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IEcheqAnularAperturaChequeConsumerMOA>().To(typeof(EcheqAnularAperturaChequeConsumerMOA)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IEcheqCargaAperturaChequeConsumerMOA>().To(typeof(EcheqCargaAperturaChequeConsumerMOA)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IObtenerTipoCambioConsumerMOA>().To(typeof(ObtenerTipoCambioConsumerMOA)).InScope(ctx => OperationContext.Current);

            #endregion

            // Scato WebApi
            kernel.Bind<IScatoRepositorioClient>().To(typeof(ScatoRepositorioClient)).InSingletonScope();
            kernel.Bind<IServicioRepositorio>().To(typeof(ServicioRepositorioClient)).InScope(ctx => OperationContext.Current);

            kernel.Bind<IExternalApiService>().To(typeof(ExternalApiService)).InScope(ctx => OperationContext.Current);
            kernel.Bind<DbContext>().To<MOAOperacionesDbContext>().InTransientScope();
            kernel.Bind<IRepositorio>().To<RepositorioEF>().InTransientScope();
            kernel.Bind<ICache, Cache>().To<Cache>().InSingletonScope();
            kernel.Bind<IServicioSuscriptorAccesosConsumer>().To(typeof(ServicioSuscriptorAccesosConsumer)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IOrdenDeCargaApiService>().To(typeof(OrdenDeCargaApiService)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IAuthenticationManager>().To(typeof(AuthenticationManager)).InSingletonScope();

        }
    }
}