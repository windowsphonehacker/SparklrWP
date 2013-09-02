using Microsoft.Phone.Scheduler;
using System;
namespace SparklrWP.Utils
{
    public class Task
    {
        PeriodicTask periodicTask;
        const string periodicTaskName = "Sparklr";

        public Task()
        {
#if DEBUG
            if (ScheduledActionService.Find(periodicTaskName) != null)
                ScheduledActionService.Remove(periodicTaskName);
#endif
            periodicTask = ScheduledActionService.Find(periodicTaskName) as PeriodicTask;
            if (periodicTask == null)
            {
                periodicTask = new PeriodicTask(periodicTaskName);
                periodicTask.Description = "Sparklr notification agent";
                try
                {
                    ScheduledActionService.Add(periodicTask);
                }
                catch (Exception ex)
                {
                    //TODO: Handle errors here (For example if there are too many background tasks already)
                }
            }
            // If debugging is enabled, use LaunchForTest to launch the agent in 15 seconds.
#if DEBUG
            ScheduledActionService.LaunchForTest(periodicTaskName, new TimeSpan(0, 0, 15));
#endif
        }
    }
}
