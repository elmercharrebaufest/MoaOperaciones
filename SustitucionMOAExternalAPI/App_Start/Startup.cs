using Hangfire;
using Hangfire.Dashboard;
using Microsoft.Owin;
using Owin;
using System;
using System.Web.Http;

[assembly: OwinStartupAttribute(typeof(SustitucionMOAExternalAPI.Startup))]
namespace SustitucionMOAExternalAPI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureHangFire(app);
        }

        private void ConfigureHangFire(IAppBuilder app)
        {
            Hangfire.GlobalConfiguration.Configuration
                           .UseSqlServerStorage("HfContexto").UseNLogLogProvider();

            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });

            var options = new DashboardOptions
            {
                //Authorization = new[]
                //{
                //    new AuthorizationFilter { /*Users = "admin, superuser",*/ Roles = "Admin, Support" }//,new ClaimsBasedAuthorizationFilter("name", "value")
                //}
            };
            app.UseHangfireDashboard("/Hangfire", options);
            app.UseHangfireServer();
            BackgroundJob.Enqueue(() => Console.WriteLine("¡Hola desde Hangfire!"));

        }
    }
}
