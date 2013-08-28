using Microsoft.Phone.Info;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;

namespace SparklrWP.Utils
{
#if DEBUG
    public static class MemoryDiagnosticsHelper
    {
        static Popup _popup;
        static TextBlock _currentMemoryBlock;
        static TextBlock _peakMemoryBlock;
        static DispatcherTimer _timer;
        static bool _forceGc;
        const long MaxMemory = 90 * 1024 * 1024; // 90MB, per marketplace
        static int _lastSafetyBand = -1; // to avoid needless changes of colour

        const long MaxCheckpoints = 10; // adjust as needed
        static Queue<MemoryCheckpoint> _recentCheckpoints;

        // static bool alreadyFailedPeak = false; // to avoid endless Asserts

        /// <summary>
        /// Starts the memory diagnostic timer and shows the counter
        /// </summary>
        /// <param name="timespan">The timespan between counter updates</param>
        /// <param name="forceGc">Whether or not to force a GC before collecting memory stats</param>
        [Conditional("DEBUG")]
        public static void Start(TimeSpan timespan, bool forceGc)
        {
            if (_timer != null)
                throw new InvalidOperationException("Diagnostics already running");

            _forceGc = forceGc;
            _recentCheckpoints = new Queue<MemoryCheckpoint>();

            StartTimer(timespan);
            ShowPopup();
        }

        /// <summary>
        /// Stops the timer and hides the counter
        /// </summary>
        [Conditional("DEBUG")]
        public static void Stop()
        {
            HidePopup();
            StopTimer();
            _recentCheckpoints = null;
        }

        /// <summary>
        /// Add a checkpoint to the system to help diagnose failures. Ignored in retail mode
        /// </summary>
        /// <param name="text">Text to describe the most recent thing that happened</param>
        [Conditional("DEBUG")]
        public static void Checkpoint(string text)
        {
            if (_recentCheckpoints == null)
                return;

            if (_recentCheckpoints.Count >= MaxCheckpoints - 1)
                _recentCheckpoints.Dequeue();

            _recentCheckpoints.Enqueue(new MemoryCheckpoint(text, GetCurrentMemoryUsage()));
        }

        /// <summary>
        /// Recent checkpoints stored by the app; will always be empty in retail mode
        /// </summary>
        public static IEnumerable<MemoryCheckpoint> RecentCheckpoints
        {
            get
            {
                if (_recentCheckpoints == null)
                    yield break;

                foreach (MemoryCheckpoint checkpoint in _recentCheckpoints)
                    yield return checkpoint;
            }
        }

        /// <summary>
        /// Gets the current memory usage, in bytes. Returns zero in non-debug mode
        /// </summary>
        /// <returns>Current usage</returns>
        public static long GetCurrentMemoryUsage()
        {
#if DEBUG
            // don't use DeviceExtendedProperties for release builds (requires a capability)
            return (long)DeviceExtendedProperties.GetValue("ApplicationCurrentMemoryUsage");
#else
      return 0;
#endif
        }

        /// <summary>
        /// Gets the peak memory usage, in bytes. Returns zero in non-debug mode
        /// </summary>
        /// <returns>Peak memory usage</returns>
        public static long GetPeakMemoryUsage()
        {
#if DEBUG
            // don't use DeviceExtendedProperties for release builds (requires a capability)
            return (long)DeviceExtendedProperties.GetValue("ApplicationPeakMemoryUsage");
#else
      return 0;
#endif
        }

        private static void ShowPopup()
        {
            _popup = new Popup();
            var fontSize = (double)Application.Current.Resources["PhoneFontSizeSmall"] - 2;
            var foreground = (Brush)Application.Current.Resources["PhoneForegroundBrush"];
            var sp = new StackPanel { Orientation = Orientation.Horizontal, Background = (Brush)Application.Current.Resources["PhoneSemitransparentBrush"] };
            _currentMemoryBlock = new TextBlock { Text = "---", FontSize = fontSize, Foreground = foreground };
            _peakMemoryBlock = new TextBlock { Text = "", FontSize = fontSize, Foreground = foreground, Margin = new Thickness(5, 0, 0, 0) };
            sp.Children.Add(_currentMemoryBlock);
            sp.Children.Add(new TextBlock { Text = " kb", FontSize = fontSize, Foreground = foreground });
            sp.Children.Add(_peakMemoryBlock);
            sp.RenderTransform = new CompositeTransform { Rotation = 90, TranslateX = 480, TranslateY = 425, CenterX = 0, CenterY = 0 };
            _popup.Child = sp;
            _popup.IsOpen = true;
        }

        private static void StartTimer(TimeSpan timespan)
        {
            _timer = new DispatcherTimer { Interval = timespan };
            _timer.Tick += timer_Tick;
            _timer.Start();
        }

        static void timer_Tick(object sender, EventArgs e)
        {
            if (_forceGc)
                GC.Collect();

            UpdateCurrentMemoryUsage();
            UpdatePeakMemoryUsage();
        }

        private static void UpdatePeakMemoryUsage()
        {
            //if (alreadyFailedPeak)
            //  return;

            //long peak = GetPeakMemoryUsage();
            //if (peak >= MAX_MEMORY)
            //{
            //  alreadyFailedPeak = true;
            //  Checkpoint("*MEMORY USAGE FAIL*");
            //  peakMemoryBlock.Text = "FAIL!";
            //  peakMemoryBlock.Foreground = new SolidColorBrush(Colors.Red);
            //  if (Debugger.IsAttached)
            //    Debug.Assert(false, "Peak memory condition violated");
            //}
        }

        private static void UpdateCurrentMemoryUsage()
        {
            long mem = GetCurrentMemoryUsage();
            _currentMemoryBlock.Text = string.Format("{0:N}", mem / 1024);
            int safetyBand = GetSafetyBand(mem);
            if (safetyBand != _lastSafetyBand)
            {
                _currentMemoryBlock.Foreground = GetBrushForSafetyBand(safetyBand);
                _lastSafetyBand = safetyBand;
            }
        }

        private static Brush GetBrushForSafetyBand(int safetyBand)
        {
            switch (safetyBand)
            {
                case 0:
                    return new SolidColorBrush(Colors.Green);

                case 1:
                    return new SolidColorBrush(Colors.Orange);

                default:
                    return new SolidColorBrush(Colors.Red);
            }
        }

        private static int GetSafetyBand(long mem)
        {
            double percent = mem / (double)MaxMemory;
            if (percent <= 0.75)
                return 0;

            if (percent <= 0.90)
                return 1;

            return 2;
        }

        private static void StopTimer()
        {
            _timer.Stop();
            _timer = null;
        }

        private static void HidePopup()
        {
            _popup.IsOpen = false;
            _popup = null;
        }
    }

    /// <summary>
    /// Holds checkpoint information for diagnosing memory usage
    /// </summary>
    public class MemoryCheckpoint
    {
        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="text">Text for the checkpoint</param>
        /// <param name="memoryUsage">Memory usage at the time of the checkpoint</param>
        internal MemoryCheckpoint(string text, long memoryUsage)
        {
            Text = text;
            MemoryUsage = memoryUsage;
        }

        /// <summary>
        /// The text associated with this checkpoint
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// The memory usage at the time of the checkpoint
        /// </summary>
        public long MemoryUsage { get; private set; }
    }
#endif
}
