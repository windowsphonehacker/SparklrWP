
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Telerik.Windows.Controls;
using Telerik.Windows.Data;
using SparklrForWindowsPhone.Helpers;
using SparklrForWindowsPhone.ViewModels;

namespace SparklrForWindowsPhone.Pages
{
    public partial class ViewPost : PhoneApplicationPage
    {
        public ViewPost()
        {
            InitializeComponent();
            this.SetConversationParticipants();
            this.SetGroupDescriptors();
        }

        private void OnSendingMessage(object sender, ConversationViewMessageEventArgs e)
        {
            if (string.IsNullOrEmpty((e.Message as ConversationViewMessage).Text))
            {
                return;
            }
            ConversationViewMessage originalMessage = e.Message as ConversationViewMessage;
            MessagesViewModel viewModel = this.DataContext as MessagesViewModel;
            CustomMessage previousMessage = viewModel.Messages.Last();
            int group = previousMessage.Group.HasValue ? previousMessage.Group.Value : 0;
            if (previousMessage.SenderId != viewModel.You.PersonId)
            {
                group++;
            }
            CustomMessage customMessage = new CustomMessage(originalMessage.Text, originalMessage.TimeStamp, originalMessage.Type, viewModel.You.PersonId, group);
            viewModel.Messages.Add(customMessage);
        }

        private void SetConversationParticipants()
        {
            MessagesViewModel viewModel = this.DataContext as MessagesViewModel;
            viewModel.ConversationBuddy = viewModel.People[0];
            viewModel.You = viewModel.People[4];
        }

        private void SetGroupDescriptors()
        {
            this.conversationView.GroupDescriptors = new DataDescriptor[] 
            { 
                new GenericGroupDescriptor<CustomMessage, CustomMessage>(message => message)
            };
        }
    }
}
