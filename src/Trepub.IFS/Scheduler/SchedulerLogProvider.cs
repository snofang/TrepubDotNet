using Microsoft.Extensions.Logging;
using Trepub.Common.Facade;
using Quartz.Logging;
using System;

namespace Trepub.IFS.Scheduler
{
    public class SchedulerLogProvider : ILogProvider
    {
        ILogger<Scheduler> logger;
        public SchedulerLogProvider()
        {
            this.logger = FacadeProvider.IfsFacade.GetLogger<Scheduler>();
        }

        public Logger GetLogger(string name)
        {
            return (level, func, exception, parameters) =>
            {
                if (level >= Quartz.Logging.LogLevel.Info && func != null)
                {
                    logger.LogInformation(func(), parameters);
                }
                return true;
            };
        }

        public IDisposable OpenNestedContext(string message)
        {
            throw new NotImplementedException();
        }

        public IDisposable OpenMappedContext(string key, string value)
        {
            throw new NotImplementedException();
        }

    }
}
