
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
using SparklrForWindowsPhone.ViewModels;

namespace SparklrForWindowsPhone.Helpers
{
    public class MessageItemTemplateSelector : DataTemplateSelector
    {
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

        public DataTemplate EmptyDataItemTemplate
        {
            get;
            set;
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            RadDataBoundListBoxItem listBoxItem = container as RadDataBoundListBoxItem;
            if (listBoxItem.AssociatedDataItem.Previous is Telerik.Windows.Data.IDataSourceGroup)
            {
                return this.EmptyDataItemTemplate;
            }
            CustomMessage message = item as CustomMessage;
            if (message.Type == ConversationViewMessageType.Incoming)
            {
                return this.IncomingTemplate;
            }
            else
            {
                return this.OutgoingTemplate;
            }
        }
    }
}
