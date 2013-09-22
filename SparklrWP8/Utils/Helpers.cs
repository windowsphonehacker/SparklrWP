extern alias ImageToolsDLL;
using Coding4Fun.Toolkit.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace SparklrWP.Utils
{
    static class Helpers
    {
        /// <summary>
        /// Displays a non intrusive toast notification. Thread safe.
        /// </summary>
        /// <param name="text">The text to display</param>
        public static void Notify(string text)
        {
            SmartDispatcher.BeginInvoke(() =>
            {
                ToastPrompt p = new ToastPrompt();
                p.Message = text;
                p.Show();
            });
        }

        /// <summary>
        /// Displays a non intrusive toast notification. Thread safe.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void NotifyFormatted(string format, params object[] args)
        {
            Notify(string.Format(format, args));
        }

        /// <summary>
        /// Displays a non intrusive toast notification. Thread safe.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void NotifyFormatted(string caption, string format, params object[] args)
        {
            Notify(caption, string.Format(format, args));
        }

        /// <summary>
        /// Displays a non intrusive toast notification
        /// </summary>
        /// <param name="text">The text to display</param>
        /// <param name="caption">The title of the message</param>
        public static void Notify(string caption, string text)
        {
            ToastPrompt p = new ToastPrompt();
            p.Message = text;
            p.Title = caption;
            p.Show();
        }

        /// <summary>
        /// Finds a scroll viewer inside the specified parents visual tree.
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static ScrollViewer FindScrollViewer(this DependencyObject parent)
        {
            var childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < childCount; i++)
            {
                var elt = VisualTreeHelper.GetChild(parent, i);
                if (elt is ScrollViewer) return (ScrollViewer)elt;
                var result = FindScrollViewer(elt);
                if (result != null) return result;
            }
            return null;
        }

        /// <summary>
        /// Updates the binding on a textbox
        /// </summary>
        /// <param name="textBox"></param>
        public static void UpdateBinding(this TextBox textBox)
        {
            BindingExpression bindingExpression =
                    textBox.GetBindingExpression(TextBox.TextProperty);
            if (bindingExpression != null)
            {
                bindingExpression.UpdateSource();
            }
        }
    }
}
