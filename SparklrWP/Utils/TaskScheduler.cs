
using Microsoft.Phone.Scheduler;
namespace SparklrWP.Utils
{
    /// <summary>
    /// Allows easy scheduling of the background agent
    /// </summary>
    public static class TaskScheduler
    {
        private static readonly string taskName = Resources.AppResources.BackgroundAgentName;
        private static readonly string taskDescription = Resources.AppResources.BackgroundAgentDescription;

        /// <summary>
        /// Schedules the background agent. Will launch it in 15 seconds in a debug setting.
        /// </summary>
        public static void ScheduleAgent()
        {
            UnscheduleAgent();

            PeriodicTask task = new PeriodicTask(taskName);
            task.Description = taskDescription;

            ScheduledActionService.Add(task);

#if DEBUG
            ScheduledActionService.LaunchForTest(taskName, new System.TimeSpan(0, 0, 15));
#endif
        }

        /// <summary>
        /// Unschedules the background task
        /// </summary>
        public static void UnscheduleAgent()
        {
            if (ScheduledActionService.Find(taskName) != null)
                ScheduledActionService.Remove(taskName);
        }

        public static string LastExitReason
        {
            get
            {
                PeriodicTask p = ScheduledActionService.Find(taskName) as PeriodicTask;

                if (p != null)
                {
                    return p.LastExitReason.ToString();
                }
                else
                {
                    return "Unavailable";
                }
            }
        }

        /// <summary>
        /// Gets if the task is currently scheduled
        /// </summary>
        public static bool IsScheduled
        {
            get
            {
                return ScheduledActionService.Find(taskName) != null;
            }
        }
    }
}
