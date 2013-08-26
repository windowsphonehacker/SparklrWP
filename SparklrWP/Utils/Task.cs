using Microsoft.Phone.Scheduler;
using System;
namespace SparklrWP.Utils
{
    public class Task
    {
        PeriodicTask periodicTask;
        string periodicTaskName = "Sparklr";

        public Task()
        {
            periodicTask = ScheduledActionService.Find(periodicTaskName) as PeriodicTask;
#if DEBUG
            if (periodicTask != null)
            {
                ScheduledActionService.Remove(periodicTaskName);
                periodicTask = null;
            }
#endif
            if(periodicTask == null){
                periodicTask = new PeriodicTask(periodicTaskName);
                periodicTask.Description = "Sparklr notification agent";
                try
                {
                    ScheduledActionService.Add(periodicTask);
                }
                catch (Exception e)
                {

                }
            }
                // If debugging is enabled, use LaunchForTest to launch the agent in one minute.
        }
    }
}
