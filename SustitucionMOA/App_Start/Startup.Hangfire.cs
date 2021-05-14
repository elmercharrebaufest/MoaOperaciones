using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Hangfire;
using Owin;

namespace SustitucionMOA
{
    public partial class Startup
    {
        public void ConfigureHangfire(IAppBuilder app)
        {
            GlobalConfiguration.Configuration.UseSqlServerStorage("HfContexto");
            GlobalConfiguration.Configuration.UseNLogLogProvider();
            app.UseHangfireServer();
            app.UseHangfireDashboard();
            Register();
        }

        private void Register()
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById("Argentina Standard Time");

            RecurringJob.AddOrUpdate<Jobs.IReporteLiquidacionesInformadasJob>(
                "ReporteLiquidacionesInformadasJob",
                j => j.Execute(),
                "30 6 * * *", tz);



            RecurringJob.AddOrUpdate<Jobs.IVerificarTransporteOrdenesDeCargaJob>(
                "VerificarTransporteOrdenesDeCargaJob",
                j => j.Execute(),
                "30 6 * * *", tz);

            RecurringJob.AddOrUpdate<Jobs.IReporteCamposSustentablesTSAJob>(
                "ReporteCamposSustentablesTSAJob",
                j => j.Execute(),
                "30 6 * * *", tz);


            RecurringJob.AddOrUpdate<Jobs.IReporteConflictosCamposSustentablesJob>(
                "ReporteConflictosCamposSustentablesJob",
                j => j.Execute(),
                "30 6 * * *", tz);
        }
    }
}