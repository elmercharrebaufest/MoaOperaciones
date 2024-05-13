using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Hangfire;
using Owin;
using Hangfire.Dashboard;

namespace SustitucionMOA
{
    public partial class Startup
    {
        public void ConfigureHangfire(IAppBuilder app)
        {
            GlobalConfiguration.Configuration.UseSqlServerStorage("HfContexto");
            GlobalConfiguration.Configuration.UseNLogLogProvider();
            //app.UseHangfireServer();
            ////app.UseHangfireDashboard("/hangfire");
            //app.UseHangfireDashboard("/hangfire", new DashboardOptions
            //{
            //    Authorization = new[] { new HangFireAuthorizationFilter() }
            //});
            //Register();
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
                "0 * * * *", tz);

            RecurringJob.AddOrUpdate<Jobs.IEnviarASAPOrdenDeCargaJob>(
                "EnviarASAPOrdenDeCargaJob",
                j => j.Execute(),
                "*/15 * * * *", tz);

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
                "30 8 * * *", tz);

            RecurringJob.AddOrUpdate<Jobs.IVencimientoOrdenesDeCargaFasonJob>(
               "VencimientoOrdenesDeCargaFasonJob",
               j => j.Execute(),
               "30 8 * * *", tz);

            RecurringJob.RemoveIfExists("ActualizarEstadoSolpSapJob");

            RecurringJob.RemoveIfExists("ObtenerSolpsDesdeSAPJob");

            RecurringJob.AddOrUpdate<Jobs.IActualizarLocalidadesPartidosJob>("ActualizarLocalidades", j => j.Execute(),
                 "0 0 * * *", tz);

            RecurringJob.AddOrUpdate<Jobs.IActualizarSISAJob>("ActualizarSISAJob", j => j.Execute(),
                 "0 12 * * *", tz);

            RecurringJob.AddOrUpdate<Jobs.IReporteLoginsJob>(
                "ReporteLoginsJob",
                j => j.Execute(),
                "0 6 1 * *", tz);
            RecurringJob.AddOrUpdate<Jobs.IVerificarSituacionCrediticiaJob>(
                "VerificarSituacionCrediticiaJob",
                j => j.Execute(),
                "*/15 * * * *", tz);
            RecurringJob.AddOrUpdate<Jobs.IVerificarTransporteOrdenesDeCargaFasonJob>(
                "VerificarTransporteOrdenesDeCargaFasonJob",
                j => j.Execute(),
                "0 * * * *", tz);
            RecurringJob.AddOrUpdate<Jobs.IAltaClienteSAPJob>(
               "AltaClienteSAPJob",
               j => j.Execute(),
               "0 13,23 * * *", tz);
            RecurringJob.AddOrUpdate<Jobs.IVerificarOrdenesFacturaCompensadaJob>(
                "VerificarOrdenesFacturaCompensadaJob",
                j => j.Execute(),
                "0 * * * *", tz);
            RecurringJob.AddOrUpdate<Jobs.ICcSsObtenerArchivosUcropJob>(
                "CcSsObtenerArchivosUcropJob",
                j => j.Execute(),
                "*/5 * * * *", tz);
            RecurringJob.AddOrUpdate<Jobs.IEnviarCamposUcropitJob>(
                "EnviarCamposUcropitJob",
                j => j.Execute(),
                "0 0 * * TUE,THU", tz);
            RecurringJob.AddOrUpdate<Jobs.IVencimientoOrdenesResiduosJob>(
                "VencimientoOrdenesResiduosJob",
                j => j.Execute(),
                "30 8 * * *", tz);
        }
    }

    public class HangFireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            bool boolAuthorizeCurrentUserToAccessHangFireDashboard = false;

            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {


                // Obtén el ClaimsPrincipal actual del contexto HTTP
                System.Security.Claims.ClaimsPrincipal userClaimsPrincipal = HttpContext.Current.User as System.Security.Claims.ClaimsPrincipal;

                if (userClaimsPrincipal != null)
                {
                    // Accede a la identidad del usuario actual
                    System.Security.Claims.ClaimsIdentity userIdentity = userClaimsPrincipal.Identity as System.Security.Claims.ClaimsIdentity;

                    if (userIdentity != null)
                    {
                        // Busca la reclamación "permisos" con el valor "APIKEY"
                        IEnumerable<System.Security.Claims.Claim> permisosClaim = userIdentity.FindAll("permisos");

                        if (permisosClaim != null && permisosClaim.Any(a => a.Value == "HANGFIREDASHBOARD"))
                        {
                            boolAuthorizeCurrentUserToAccessHangFireDashboard = true;
                        }
                    }
                }
            }
            return boolAuthorizeCurrentUserToAccessHangFireDashboard;

        }
    }
}