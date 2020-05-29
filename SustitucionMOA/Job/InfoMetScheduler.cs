using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Quartz;
using Quartz.Impl;

namespace SustitucionMOA.Job
{
    public class InfoMetScheduler
    {
        public static async void Start()
        {
            NameValueCollection props = new NameValueCollection{
                { "quartz.serializer.type", "binary" }
            };
            StdSchedulerFactory factory = new StdSchedulerFactory(props);

            IScheduler sched = await factory.GetScheduler();
            await sched.Start();

            IJobDetail job = JobBuilder.Create<InfoMetJob>()
                .WithIdentity("InfoMetJob", "grupoInfoMet")
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
              .WithIdentity("infoMetTrigger", "grupoInfoMet")
              .StartNow()
              .WithSimpleSchedule(x => x
                  .WithIntervalInMinutes(5)
                  .RepeatForever())
              .Build();

            await sched.ScheduleJob(job, trigger);
        }
    }
}