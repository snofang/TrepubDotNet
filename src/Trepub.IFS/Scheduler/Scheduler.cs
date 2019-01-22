using System.Collections.Generic;
using Trepub.Common.Interfaces;
using Quartz;
using Quartz.Logging;
using System.Collections.Specialized;
using Quartz.Impl;
using System;

namespace Trepub.IFS.Scheduler
{
    public class Scheduler : Common.Interfaces.IScheduler
    {
        protected Quartz.IScheduler scheduler;
        protected Dictionary<IScheduledJob, TriggerKey> tirggers;
        protected readonly string defaultScheduleGroupName = "AppSchedules";

        public Scheduler()
        {

            this.tirggers = new Dictionary<IScheduledJob, TriggerKey>();

            //configuring logger
            LogProvider.SetCurrentLogProvider(new SchedulerLogProvider());

            // Grab the Scheduler instance from the Factory
            NameValueCollection props = new NameValueCollection
                {
                    { "quartz.serializer.type", "binary" }
                };
            StdSchedulerFactory factory = new StdSchedulerFactory(props);
            this.scheduler = factory.GetScheduler().Result;

            // and start it off
            scheduler.Start().Wait();
        }

        public void Schedule(IScheduledJob scheduledJob, int everySeconds)
        {
            // define the job and tie it to our HelloJob class
            IDictionary<string, object> data = new Dictionary<string, object>();
            data.Add("scheduledJob", scheduledJob);

            IJobDetail job = JobBuilder.Create<TemplateJob>()
                .WithIdentity("schedule:" + scheduledJob.GetType().Name, defaultScheduleGroupName)
                .UsingJobData(new JobDataMap(data))
                .Build();

            // Trigger the job to run now, and then repeat every 10 seconds
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger:"+ scheduledJob.GetType().Name, defaultScheduleGroupName)
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(everySeconds)
                    .RepeatForever())
                .Build();

            // Tell quartz to schedule the job using our trigger
            this.scheduler.ScheduleJob(job, trigger).Wait();
            this.tirggers.Add(scheduledJob, trigger.Key);
        }

        public void ScheduleDaily(IScheduledJob scheduledJob, int hour, int minutes)
        {
            // define the job and tie it to our HelloJob class
            IDictionary<string, object> data = new Dictionary<string, object>();
            data.Add("scheduledJob", scheduledJob);

            IJobDetail job = JobBuilder.Create<TemplateJob>()
                .WithIdentity("schedule:" + scheduledJob.GetType().Name, defaultScheduleGroupName)
                .UsingJobData(new JobDataMap(data))
                .Build();

            // Trigger the job to run now, and then repeat every 10 seconds
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger:" + scheduledJob.GetType().Name, defaultScheduleGroupName)
                .StartNow()
                .WithDailyTimeIntervalSchedule(x => x
                    .OnEveryDay()
                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(hour, minutes))
                    .EndingDailyAfterCount(1))
                .Build();

            // Tell quartz to schedule the job using our trigger
            this.scheduler.ScheduleJob(job, trigger).Wait();
            this.tirggers.Add(scheduledJob, trigger.Key);
        }


        public void UnSchedule(IScheduledJob scheduledJob)
        {
            if (this.tirggers.ContainsKey(scheduledJob))
            {
                this.scheduler.UnscheduleJob(this.tirggers[scheduledJob]);
            }
        }

        public void Shutdown()
        {
            scheduler.Shutdown().Wait();
        }


    }
}
