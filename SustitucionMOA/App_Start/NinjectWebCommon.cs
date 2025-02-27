using Hangfire;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Ninject;
using Ninject.Web.Common;
using Ninject.Web.Common.WebHost;
using SustitucionMOA.Jobs;
using SustitucionMOARepositorio;
using SustitucionMOARepositorio.Repositorios;
using SustitucionMOARepositorio.Repositorios.Interfaces;
using SustitucionMOAUtils.DesignPattern.Classes;
using SustitucionMOAUtils.DesignPattern.Interfaces;
using SustitucionMOAUtils.Export.CampoSustentable;
using SustitucionMOAUtils.Helpers;
using SustitucionMOAUtils.Interfaces.Helpers;
using SustitucionMOAUtils.Interfaces.Validadores;
using SustitucionMOAUtils.Interfaces.Wrappers;
using SustitucionMOAUtils.Validadores;
using SustitucionMOAUtils.Wrappers;
using SustitucionMOAWS.AzureAD;
using SustitucionMOAWS.GoogleDrive;
using SustitucionMOAWS.GoogleDrive.Interfaces;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.ScatoWebService;
using SustitucionMOAWS.WebApi;
using SustitucionMOAWS.WebApi.OpenStreetMap;
using SustitucionMOAWS.WebApi.OSRM;
using SustitucionMOAWS.WSConsumers;
using System;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Web;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(SustitucionMOA.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(SustitucionMOA.App_Start.NinjectWebCommon), "Stop")]
namespace SustitucionMOA.App_Start
{
    public static class NinjectWebCommon
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
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
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

            RegisterServices(kernel);
            return kernel;
        }


        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<IReporteLiquidacionesInformadasJob>().To(typeof(ReporteLiquidacionesInformadasJob)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IVerificarTransporteOrdenesDeCargaJob>().To(typeof(VerificarTransporteOrdenesDeCargaJob)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IVerificarTransporteOrdenesDeCargaFasonJob>().To(typeof(VerificarTransporteOrdenesDeCargaFasonJob)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IVerificarOrdenesFacturaCompensadaJob>().To(typeof(VerificarOrdenesFacturaCompensadaJob)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IEnviarASAPOrdenDeCargaJob>().To(typeof(EnviarASAPOrdenDeCargaJob)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IReporteCamposSustentablesTSAJob>().To(typeof(ReporteCamposSustentablesTSAJob)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IReporteConflictosCamposSustentablesJob>().To(typeof(ReporteConflictosCamposSustentablesJob)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IActualizarBaseDeDatosSolpSapJob>().To(typeof(ActualizarBaseDeDatosSolpSapJob)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IVencimientoOrdenesDeCargaSapJob>().To(typeof(VencimientoOrdenesDeCargaSapJob)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IVencimientoOrdenesDeCargaFasonJob>().To(typeof(VencimientoOrdenesDeCargaFasonJob)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IVencimientoOrdenesResiduosJob>().To(typeof(VencimientoOrdenesResiduosJob)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IActualizarEstadoSolpSapJob>().To(typeof(ActualizarEstadoSolpSapJob)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IObtenerSolpsDesdeSAPJob>().To(typeof(ObtenerSolpsDesdeSAPJob)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IActualizarLocalidadesPartidosJob>().To(typeof(ActualizarLocalidadesPartidosJob)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IActualizarSISAJob>().To(typeof(ActualizarSISAJob)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IValidadorPesificacion>().To(typeof(ValidadorPesificacion)).InScope(ctx => OperationContext.Current);
            kernel.Bind<ITimeProvider>().To(typeof(CurrentTimeProvider)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IExcelExportWrapper>().To(typeof(ExcelExportWrapper)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IFileWrapper>().To(typeof(FileWrapper)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IReporteLoginsJob>().To(typeof(ReporteLoginsJob)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IVerificarSituacionCrediticiaJob>().To(typeof(VerificarSituacionCrediticiaJob)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IObtenerRegistroInfoConsumerMOA>().To(typeof(ObtenerRegistroInfoConsumerMOA)).InScope(ctx => OperationContext.Current);

            kernel.Bind<IAltaClienteSAPJob>().To(typeof(AltaClienteSAPJob)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IEnviarMailReporteSOLPJob>().To(typeof(EnviarMailReporteSOLPJob)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IReporteOrdenDeCompraConsumerMOA>().To(typeof(ReporteOrdenDeCompraConsumerMOA)).InScope(ctx => OperationContext.Current);
            kernel.Bind<ICcSsObtenerArchivosUcropJob>().To(typeof(CcSsObtenerArchivosUcropJob)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IEnviarCamposUcropitJob>().To(typeof(EnviarCamposUcropitJob)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IDerivacionAutomaticaJob>().To(typeof(DerivacionesAutomaticaJob)).InScope(ctx => OperationContext.Current);
            kernel.Bind<INotificarAprobacionesPendientesJob>().To(typeof(NotificarAprobacionesPendientesJob)).InScope(ctx => OperationContext.Current);

            kernel.Bind<INotificacionErroresJob>().To(typeof(NotificacionErroresJob)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IEliminarFacturasAntiguasJob>().To(typeof(EliminarFacturasAntiguasJob)).InScope(ctx => OperationContext.Current);


            #region Registro

            var assembly = Assembly.Load("SustitucionMOAUtils");

            var types = assembly.GetTypes()
                .Where(type => type.IsClass)
                .ToList();

            foreach (var type in types)
            {
                if (type.Name.EndsWith("Service"))
                {
                    var interfaces = type.GetInterfaces();
                    if (interfaces.Length > 0)
                    {
                        foreach (var interfaceType in interfaces)
                        {
                            kernel.Bind(interfaceType).To(type).InScope(ctx => OperationContext.Current);
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException($"La clase {type.Name} no implementa ninguna interfaz.");
                    }
                }
            }
            #endregion

            kernel.Bind<ICampoSustentablePdfGenerator>().To(typeof(CampoSustentablePdfGenerator)).InScope(ctx => OperationContext.Current);

            #region InterfacesSAP
            kernel.Bind<IVendedorHabilitadoConsumerMOA>().To(typeof(VendedorHabilitadoConsumerMOA)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IOrdenCargaConsumerMOA>().To(typeof(OrdenCargaConsumerMOA)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IAplicacionCartaPorteConsumer>().To(typeof(AplicacionCartaPorteConsumer)).InScope(ctx => OperationContext.Current);
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
            kernel.Bind<IObtenerOrdenDeCompraConsumerMOA>().To(typeof(ObtenerOrdenDeCompraConsumerMOA)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IObtenerOrdenesDeCompraParaSOLPConsumerMOA>().To(typeof(ObtenerOrdenesDeCompraParaSOLPConsumerMOA)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IObtenerProveedorConsumerMOA>().To(typeof(ObtenerProveedorConsumerMOA)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IModificarOrdenDeCompraConsumerMOA>().To(typeof(ModificarOrdenDeCompraConsumerMOA)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IVendedoresConsumerMOA>().To(typeof(VendedoresConsumerMOA)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IAgregarRegistroInfoConsumerMOA>().To(typeof(AgregarRegistroInfoConsumerMOA)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IObtenerUnidadesDeMedidaAlternativasConsumerMOA>().To(typeof(ObtenerUnidadesDeMedidaAlternativasConsumerMOA)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IObtenerPDFOrdenCompraConsumerMOA>().To(typeof(ObtenerPDFOrdenCompraConsumerMOA)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IListarSolpPendientesConsumerMOA>().To(typeof(ListarSolpPendientesConsumerMOA)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IObtenerAdjuntosSOLPEDConsumerMOA>().To(typeof(ObtenerAdjuntosSOLPEDConsumerMOA)).InScope(ctx => OperationContext.Current);


            kernel.Bind<IScatoConsumer>().To(typeof(ScatoConsumer)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IScatoComandosConsumer>().To(typeof(ScatoComandosConsumer)).InScope(ctx => OperationContext.Current);


            #endregion
            // Scato WebApi
            kernel.Bind<IScatoRepositorioClient>().To(typeof(ScatoRepositorioClient)).InSingletonScope();
            kernel.Bind<IServicioRepositorio>().To(typeof(ServicioRepositorioClient)).InScope(ctx => OperationContext.Current);

            // CNRT WebApi
            kernel.Bind<ICNRTClient>().To(typeof(CNRTClient)).InSingletonScope();

            // OSRM WebApi
            kernel.Bind<IOsrmApiClient>().To(typeof(OsrmApiClient)).InTransientScope();

            // OpenStreetMap WebApi
            kernel.Bind<IOpenStreetMapClient>().To(typeof(OpenStreetMapClient)).InTransientScope();

            // Azure
            kernel.Bind<IAzureADConsumer>().To(typeof(AzureADConsumer)).InTransientScope();
            kernel.Bind<IUsersGraphAPIClient>().To(typeof(UsersGraphAPIClient)).InScope(ctx => HttpContext.Current);

            kernel.Bind<DbContext>().To<MOAOperacionesDbContext>().InScope(ctx => HttpContext.Current);
            kernel.Bind<IRepositorio>().To<RepositorioEF>().InScope(ctx => HttpContext.Current);
            kernel.Bind<IRepositorioUsuario>().To<RepositorioUsuario>().InScope(ctx => HttpContext.Current);
            kernel.Bind<IRepositorioCampoSustentable>().To<RepositorioCampoSustentable>().InScope(ctx => HttpContext.Current);
            kernel.Bind<IRepositorioCompras>().To<RepositorioCompras>().InScope(ctx => HttpContext.Current);
            kernel.Bind<IRepositorioOrdenDeCarga>().To<RepositorioOrdenDeCarga>().InScope(ctx => HttpContext.Current);
            kernel.Bind<IRepositorioOrdenDeCargaFason>().To<RepositorioOrdenDeCargaFason>().InScope(ctx => HttpContext.Current);
            kernel.Bind<IRepositorioOrdenResiduos>().To<RepositorioOrdenResiduos>().InScope(ctx => HttpContext.Current);
            kernel.Bind<IRepositorioUbicacionGeografica>().To<RepositorioUbicacionGeografica>().InScope(ctx => HttpContext.Current);
            kernel.Bind<IRepositorioConsultas>().To<RepositorioConsultas>().InScope(ctx => HttpContext.Current);
            kernel.Bind<IRepositorioEntradaServicio>().To<RepositorioEntradaServicio>().InScope(ctx => HttpContext.Current);
            kernel.Bind<ICache, Cache>().To<Cache>().InSingletonScope();

            //Consulta Strategies
            kernel.Bind<IConsultaContext>().To<ConsultaContext>().InTransientScope();
            kernel.Bind<IConsultaStrategy>().To<ConsultaOrdenesStrategy>().InTransientScope();
            kernel.Bind<IConsultaStrategy>().To<ConsultaFinalStrategy>().InTransientScope();
            kernel.Bind<IConsultaStrategy>().To<ConsultaParcialStrategy>().InTransientScope();
            kernel.Bind<IConsultaStrategy>().To<ConsultaGeneralStrategy>().InTransientScope();
            kernel.Bind<IConsultaStrategy>().To<ConsultaActualizacionStrategy>().InTransientScope();


            // GoogleDrive
            kernel.Bind<IGoogleDriveHelper>().To<GoogleDriveHelper>().InScope(ctx => HttpContext.Current);
            kernel.Bind<ICampoSustentableGoogleDrive>().To(typeof(CampoSustentableGoogleDrive)).InSingletonScope();
            //Activador Ninject Hangfire
            GlobalConfiguration.Configuration.UseNinjectActivator(kernel);
        }

        /// <summary>
        /// Gets the configured kernel.
        /// </summary>
        /// <returns>The kernel.</returns>
        public static IKernel GetKernel()
        {
            return bootstrapper.Kernel;
        }

    }
}
