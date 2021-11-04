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

            RecurringJob.AddOrUpdate<Jobs.IActualizarBaseDeDatosSolpSapJob>(
                "ActualizarBaseDeDatosSolpSapJob",
                j => j.Execute(),
                "0 23 * * *", tz);

            //Actualmente corre a las 12 1 vez al dia. Si se quiere que corra cada 1 hora usar: "0 * * * *"
            RecurringJob.AddOrUpdate<Jobs.IVencimientoOrdenesDeCargaSapJob>(
                "VencimientoOrdenesDeCargaSapJob",
                j => j.Execute(),
                "0 0 * * *", tz);

            RecurringJob.AddOrUpdate<Jobs.IActualizarEstadoSolpSapJob>(
                "ActualizarEstadoSolpSapJob",
                j => j.Execute(),
                "0 * * * *", tz);

            RecurringJob.AddOrUpdate<Jobs.IObtenerSolpsDesdeSAPJob>(
                "ObtenerSolpsDesdeSAPJob",
                j => j.Execute(),
                "*/15 * * * *", tz);
        }
    }
}