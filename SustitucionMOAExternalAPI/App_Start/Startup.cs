using Hangfire;
using Hangfire.Common;
using Hangfire.Dashboard;
using Hangfire.States;
using Hangfire.Storage;
using Microsoft.Owin;
using Ninject;
using Owin;
using SustitucionMOAUtils.Helpers;
using SustitucionMOAUtils.Logger;
using SustitucionMOAUtils.Services;
using System;
using System.Collections.Generic;
using System.Configuration;

[assembly: OwinStartupAttribute(typeof(SustitucionMOAExternalAPI.Startup))]
namespace SustitucionMOAExternalAPI
{

    public partial class Startup
    {
        public static IKernel kernel { get; private set; }
        public void Configuration(IAppBuilder app)
        {
            kernel = new StandardKernel();

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
            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute
            {
                Attempts = 3,
                DelaysInSeconds = new int[] { 60, 60, 60 }
            });

            GlobalJobFilters.Filters.Add(new OnFinalFailureAttribute(context =>
            {
                NotificarFalloJob(context);
            }));

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

        private static void NotificarFalloJob(ApplyStateContext context)
        {
            var job = context.BackgroundJob.Job;
            var jobName = job.Type.Name;
            var method = job.Method.Name;
            var exception = ((FailedState)context.NewState).Exception;

            // 🚨 Los parámetros originales del método llamado
            var args = job.Args.ToJson();

            // Encolamos el job para loguear el fallo final, incluyendo los parámetros
            BackgroundJob.Enqueue(() => LogFallo(jobName, method, exception.Message, exception, args));
        }

        public static void LogFallo(string jobName, string method, string errorMessage, Exception exception, string parametros)
        {
            Log.ExternalAPIInfo($"Job {jobName}.{method} falló definitivamente: {errorMessage}");
            Log.ExternalAPIInfo($"Parámetros: {parametros}");
            Log.ExternalAPIError(exception);

            var to = new List<string> { ConfigurationManager.AppSettings["EmailEnvioErrores"] };
            var subject = $"Error definitivo en Job {jobName}.{method}";
            var body = $"Parámetros: {parametros}\n\nMensaje de error: {errorMessage}\n\nDetalles:\n{exception}";

            var emailService = new EmailService();
            emailService.EnviarMail(new SustitucionMOAUtils.Email.EmailSenderData
            {
                Mails = to,
                Asunto = subject,
                Cuerpo = body
            });
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


    public class OnFinalFailureAttribute : JobFilterAttribute, IApplyStateFilter
    {
        private readonly Action<ApplyStateContext> _onFailed;

        public OnFinalFailureAttribute(Action<ApplyStateContext> onFailed)
        {
            _onFailed = onFailed;
        }

        public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            // Si el job pasa al estado "Failed" y ya no tiene más reintentos pendientes
            if (context.NewState is FailedState failedState)
            {
                var retries = context.GetJobParameter<int>("RetryCount");
                var maxRetries = context.GetJobParameter<int>("RetryAttempts");

                // Si ya se hicieron todos los intentos (3 en tu caso)
                if (retries >= (maxRetries - 1))
                {
                    _onFailed?.Invoke(context);
                }
            }
        }

        public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            // No hace nada al desaplicar estados
        }
    }

}
