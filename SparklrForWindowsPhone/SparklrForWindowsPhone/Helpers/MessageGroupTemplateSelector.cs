
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Telerik.Windows.Controls;
using Telerik.Windows.Data;
using SparklrForWindowsPhone.ViewModels;

namespace SparklrForWindowsPhone.Helpers
{
    public class MessageGroupTemplateSelector : DataTemplateSelector
    {
        private CustomMessage previousMessage;
        private int uniqueGroupIdentifier = 0;

        public DataTemplate IncomingTemplate
        {
            get;
            set;
        }

        public DataTemplate OutgoingTemplate
        {
            get;
            set;
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DataGroup currentGroup = item as DataGroup;
            CustomMessage firstMessageInGroup = currentGroup.Key as CustomMessage;
            switch (firstMessageInGroup.Type)
            {
                case ConversationViewMessageType.Incoming:
                    return this.IncomingTemplate;
                case ConversationViewMessageType.Outgoing:
                    return this.OutgoingTemplate;
            }

            return null;

        }
    }
}
