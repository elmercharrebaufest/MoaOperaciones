using Hangfire;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Ninject;
using Ninject.Web.Common;
using Ninject.Web.Common.WebHost;
using SustitucionMOA.Jobs;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Services;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.WSConsumers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
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
            //kernel.Load(new WebNinjectModule());

            kernel.Bind<ICartaPorteService>().To(typeof(CartaPorteService)).InScope(ctx => OperationContext.Current);

            kernel.Bind<ICuentaCorrienteService>().To(typeof(CuentaCorrienteService)).InScope(ctx => OperationContext.Current);

            kernel.Bind<IAltaEmpresaGranosService>().To(typeof(AltaEmpresaGranosService)).InScope(ctx => OperationContext.Current);
            kernel.Bind<ICrearContratoService>().To(typeof(CrearContratoService)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IAltaEmpresaService>().To(typeof(AltaEmpresaService)).InScope(ctx => OperationContext.Current);

            kernel.Bind<IAzureB2CService>().To(typeof(AzureB2CService)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IDataAgroService>().To(typeof(DataAgroService)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IUsuarioService>().To(typeof(UsuarioService)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IContactoMailService>().To(typeof(ContactoMailService)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IConsultaService>().To(typeof(ConsultaService)).InScope(ctx => OperationContext.Current);

            kernel.Bind<IVendedorService>().To(typeof(VendedorService)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IAltaEmpresaNoGranosService>().To(typeof(AltaEmpresaNoGranosService)).InScope(ctx => OperationContext.Current);
            kernel.Bind<ILiquidacionService>().To(typeof(LiquidacionService)).InScope(ctx => OperationContext.Current);

            kernel.Bind<INotificacionService>().To(typeof(NotificacionService)).InScope(ctx => OperationContext.Current);

            kernel.Bind<IAzureService>().To(typeof(AzureService)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IReportesService>().To(typeof(ReportesService)).InScope(ctx => OperationContext.Current);

            kernel.Bind<IScatoConsumer>().To(typeof(ScatoConsumer)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IScatoComandosConsumer>().To(typeof(ScatoComandosConsumer)).InScope(ctx => OperationContext.Current);

            kernel.Bind<ITicketPesadaService>().To(typeof(TicketPesadaService)).InScope(ctx => OperationContext.Current);


            kernel.Bind<IReporteLiquidacionesInformadasJob>().To(typeof(ReporteLiquidacionesInformadasJob)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IReporteCamposSustentablesTSAJob>().To(typeof(ReporteCamposSustentablesTSAJob)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IReporteConflictosCamposSustentablesJob>().To(typeof(ReporteConflictosCamposSustentablesJob)).InScope(ctx => OperationContext.Current);

            kernel.Bind<ICampoSustentableService>().To(typeof(CampoSustentableService)).InScope(ctx => OperationContext.Current);


            #region InterfacesSAP
            kernel.Bind<IVendedorHabilitadoConsumerMOA>().To(typeof(VendedorHabilitadoConsumerMOA)).InScope(ctx => OperationContext.Current);
            #endregion


            kernel.Bind<DbContext>().To<MOAOperacionesDbContext>().InTransientScope();
            kernel.Bind<IRepositorio>().To<RepositorioEF>().InTransientScope();
            kernel.Bind<ICache, Cache>().To<Cache>().InSingletonScope();
            //Activador Ninject Hangfire
            GlobalConfiguration.Configuration.UseNinjectActivator(kernel);
        }
    }
}