using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SparklrForWindowsPhone.Helpers
{
    //ExceptionHandler. Created by Suraj (TheInterframe) on 2014-06-13 at 4:19pm.
    public class ExceptionHandler
    {
        /// <summary>
        /// Custom pre-defined messages used for exception handling. These should be used in most exception handling stuations. 
        /// </summary>
        /// <param name="MessageType">Message Type (IE: If the app can recover the MessageType = Warning)</param>
        /// <param name="ExceptionObject">The exception object from the try, catch.</param>
        public void Message(MessageTypes MessageType, Exception ExceptionObject)
        {
            if (MessageType == MessageTypes.Error)
            {
                MessageBox.Show("Looks like the app ran into a problem that wasn't handled!\n\nIf you see our highly trained team of monkeys show them this:\n\n" + ExceptionObject.Message + "\nStack Trace:\n" + ExceptionObject.StackTrace, "Uh Oh!" , MessageBoxButton.OK);
                return;
            }
            else
            {
                if(MessageType == MessageTypes.Info)
                {
                    MessageBox.Show("Looks like the app ran into something\n\nIt's not too important but you can show our highly trained team of monkeys this:\n\n" + ExceptionObject.Message + "\nStack Trace:\n" + ExceptionObject.StackTrace, "Just letting you know", MessageBoxButton.OK);
                    return;
                }
                else
                {
                    MessageBox.Show("Whoah there! Looks like somethings not right here\n\nIt's not too important but you can show our highly trained team of monkeys this:\n\n" + ExceptionObject.Message + "\nStack Trace:\n" + ExceptionObject.StackTrace, "Woah There!", MessageBoxButton.OK);
                    return;
                }
            }
        }
        /// <summary>
        /// List of message types...
        /// </summary>
        public enum MessageTypes
        {
            Error = 1,
            Warning = 2,
            Info = 3,
        }
    }
}
