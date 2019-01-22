using Microsoft.Extensions.Logging;
using Trepub.Common.Facade;
using Trepub.Common.Interfaces;
using Quartz;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Trepub.IFS.Scheduler
{
    public class TemplateJob : IJob
    {

        public Task Execute(IJobExecutionContext context)
        {
            IScheduledJob scheduledJob = (IScheduledJob)context.JobDetail.JobDataMap.Get("scheduledJob");
            return Task.Run(() =>
            {
                var t = new Thread(new ThreadStart(() =>
                {
                    try
                    {
                        scheduledJob.RunJob();
                    }catch(Exception exc)
                    {
                        var logger = FacadeProvider.IfsFacade.GetLogger<Scheduler>();
                        logger.LogError("Scheduler: Running Job Failed." + exc.ToString());
                    }
                }));
                t.Start();
                t.Join();
            });
        }

    }
}
