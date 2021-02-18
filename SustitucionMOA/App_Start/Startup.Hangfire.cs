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
            app.UseHangfireDashboard();
        }
    }
}