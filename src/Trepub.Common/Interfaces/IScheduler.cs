using System;
using System.Collections.Generic;
using System.Text;

namespace Trepub.Common.Interfaces
{
    public interface IScheduler
    {
        void Schedule(IScheduledJob scheduledJob, int everySeconds);
        void UnSchedule(IScheduledJob scheduledJob);
        void ScheduleDaily(IScheduledJob scheduledJob, int hour, int minutes);

    }
}
