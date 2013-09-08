using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace SparklrWP.Utils
{
    /// <summary>
    /// Executes a periodic task on a background thread.
    /// </summary>
    public class PeriodicTimer : IDisposable
    {
        /// <summary>
        /// The underlying task that manages the periodic tasks.
        /// </summary>
        protected Timer scheduler;

        /// <summary>
        /// Gets the current status of the PeriodicTimer.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Gets the current time in milliseconds between to TimeoutElapsed events.
        /// </summary>
        public int Timeout { get; protected set; }

        /// <summary>
        /// Contains an internal count of the start stop counts. Start calls increase, stop calls decerase this counter.
        /// </summary>
        private int startStopCounter = -1;

        /// <summary>
        /// An infinite timeout.
        /// </summary>
        public const int InfiniteTimeout = System.Threading.Timeout.Infinite;

        /// <summary>
        /// Periodically occurs after the Timeout is elapsed. The PeriodicTimer has internal logic that stops it when the event occurs and restarts the timer when the event is handled.
        /// </summary>
        public event EventHandler TimeoutElapsed;

        /// <summary>
        /// Creates a new instance of the PeriodicTimer class.
        /// </summary>
        /// <param name="timeout">The timeout between two events in milliseconds</param>
        /// <param name="autostart">True if the timer should be started immediately</param>
        public PeriodicTimer(int timeout, bool autostart = true)
        {
            this.Timeout = timeout;
            scheduler = new Timer(schedulerElapsed);

            if (autostart)
            {
                Start();
            }
        }

        /// <summary>
        /// Changes the timeout between to event calls.
        /// </summary>
        /// <param name="timeout">The new timeout in milliseconds</param>
        public virtual void Change(int timeout)
        {
            this.Timeout = timeout;
            scheduler.Change(Timeout, InfiniteTimeout);
        }

        /// <summary>
        /// Stops the timer
        /// </summary>
        public virtual void Stop()
        {
            startStopCounter--;

            if (startStopCounter == -1)
            {
#if DEBUG
                StackFrame frame = new StackFrame(1);
                MethodBase method = frame.GetMethod();
                string name = method.Name;

                App.logger.log("A background timer has been stopped in {0}", name);
#endif

                scheduler.Change(InfiniteTimeout, InfiniteTimeout);
                IsRunning = false;
            }
        }

        /// <summary>
        /// Starts the timer
        /// </summary>
        public virtual void Start()
        {
            startStopCounter++;

            if (startStopCounter == 0)
            {
#if DEBUG
                StackFrame frame = new StackFrame(1);
                MethodBase method = frame.GetMethod();
                string name = method.Name;

                App.logger.log("A background timer has been started in {0}", name);
#endif
                scheduler.Change(Timeout, InfiniteTimeout);
                IsRunning = true;
            }
        }

        /// <summary>
        /// Raises the TieoutElapsed event
        /// </summary>
        /// <param name="state"></param>
        protected virtual void schedulerElapsed(object state)
        {
            Stop();

            if (TimeoutElapsed != null)
                TimeoutElapsed(this, null);

            Start();
        }

        /// <summary>
        /// Frees all resources taken by the BackgroundTimer
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (scheduler != null)
                scheduler.Dispose();
        }
    }
}
