using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Ninject;
using Ninject.Web.Common;
using Ninject.Web.Common.WebHost;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Services;
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
            kernel.Bind<IAltaEmpresaGranosService>().To(typeof(AltaEmpresaGranosService)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IAltaEmpresaService>().To(typeof(AltaEmpresaService)).InScope(ctx => OperationContext.Current);

            kernel.Bind<IAzureB2CService>().To(typeof(AzureB2CService)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IDataAgroService>().To(typeof(DataAgroService)).InScope(ctx => OperationContext.Current);
            kernel.Bind<IUsuarioService>().To(typeof(UsuarioService)).InScope(ctx => OperationContext.Current);

            kernel.Bind<IVendedorService>().To(typeof(VendedorService)).InScope(ctx => OperationContext.Current);


            kernel.Bind<DbContext>().To<MOAOperacionesDbContext>().InTransientScope();
            kernel.Bind<IRepositorio>().To<RepositorioEF>().InTransientScope();
        }
    }
}