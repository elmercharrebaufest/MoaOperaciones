using Hangfire;
using Hangfire.Dashboard;
using Microsoft.Owin;
using Owin;
using System;
using System.Web.Http;
using Ninject;

[assembly: OwinStartupAttribute(typeof(SustitucionMOAExternalAPI.Startup))]
namespace SustitucionMOAExternalAPI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureHangFire(app);
        }

        private void ConfigureHangFire(IAppBuilder app)
        { 
            Hangfire.GlobalConfiguration.Configuration
                           .UseSqlServerStorage("HfContexto", new Hangfire.SqlServer.SqlServerStorageOptions
                           {
                               SchemaName = "HangfireExternalAPI"
                           })
                           .UseNLogLogProvider();
            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });

            var dashboardOptions = new DashboardOptions
            {
                //Authorization = new[]
                //{
                //    new AuthorizationFilter { /*Users = "admin, superuser",*/ Roles = "Admin, Support" }//,new ClaimsBasedAuthorizationFilter("name", "value")
                //}
            };


            app.UseHangfireDashboard("/Hangfire", dashboardOptions);

            var backgroundJobServerOptions = new BackgroundJobServerOptions
            {
                WorkerCount = 1 // Solo un worker, una tarea a la vez
            };
            app.UseHangfireServer(backgroundJobServerOptions);
        }
    }

    public class NinjectJobActivator : JobActivator
    {
        private readonly IKernel _kernel;

        public NinjectJobActivator(IKernel kernel)
        {
            _kernel = kernel;
        }

        public override object ActivateJob(Type jobType)
        {
            return _kernel.Get(jobType);
        }
    }
}
