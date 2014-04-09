using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SparklrSharp;
using SparklrSharp.Sparklr;
using System.Threading;

namespace SampleApplication.cs
{
    class Program
    {
        private static bool done = false;
        private static bool suppressOutput = false;
        /*
         * This is an example program that shows the usage of SparklrSharp.
         * SparklrSharp abstracts (almost) every API call. You don't have to
         * handle results yourself. It aims to provide a intuitive and easy
         * to use way to communicate with the sparklr server. Almost every
         * function is async so you don't freeze your UI thread.
         * 
         * In console applications, you have to "dispatch" to a second, async
         * function.
         * 
         * You should always add a "using SparklSharp;", as there are some
         * extension methods to make your life easier.
         * 
         * The "SparklrSharp.Sparklr" namespace contains all objects you need
         * to work with. You will only need to use those two namespaces in most
         * cases.
         * 
         * If your plattform doesn't support async/await you can add the
         * Microsoft.Bcl.Async nuget-package.
         */
        static void Main(string[] args)
        {
            mainLoop();
            while (!done)
            {
                if (!suppressOutput)
                {
                    Console.Write(".");
                    Thread.Sleep(1000);
                }
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        private static async void mainLoop()
        {
            //Create a new connection
            Connection conn = new Connection();
            bool authenticated = false;

            //Check if sparklr is online
            if (!await conn.GetAwakeAsync())
            {
                Console.WriteLine("Sparklr is offline :(");
            }
            else
            {
                Console.WriteLine("Sparklr is online");

                //Now we need to authenticate
                while (!authenticated)
                {
                    suppressOutput = true;
                    Console.WriteLine("Username:");
                    string username = Console.ReadLine();

                    Console.WriteLine("Password:");
                    string password = Console.ReadLine();

                    suppressOutput = false;
                    authenticated = await conn.SigninAsync(username, password);
                }

                Console.WriteLine("Signed in");

                while (conn.CurrentUser == null)
                    Thread.Sleep(100);

                var u = await conn.CurrentUser.GetMentionsAsync(conn);

                //Get the number of notifications
                Notification[] notifications = await conn.GetNotificationsAsync();
                Console.WriteLine("You have {0} notifications:", notifications.Length);

                foreach (Notification n in notifications)
                    Console.WriteLine("\t{0}", n.NotificationText);

                /*
                 * Now we can get a list of all messages for example.
                 * The Inbox is associated with the connection to allow
                 * applications where multiple connections exist in parallel.
                 * First you need to refresh the inbox manually.
                 */

                Console.WriteLine("Refreshing inbox");
                await conn.RefreshInboxAsync();

                //After the refresh we can dump the messages.
                Console.WriteLine("You have {0} conversations.", conn.Inbox.Count);

                int index = 0;
                foreach (Message m in conn.Inbox)
                {
                    Console.WriteLine("{3}: Converstation with {0}(@{1}):\t{2}", m.Author.Name, m.Author.Handle, m.Content, index);
                    index++;
                }

                //We now let the user select a conversation he wants to read on the console.
                Console.WriteLine("Enter the conversation you want to read.");

                suppressOutput = true;
                int selected = Convert.ToInt32(Console.ReadLine());
                suppressOutput = false;

                if (selected < index && selected >= 0)
                {
                    Console.WriteLine("Retreiving conversation with {0}.", conn.Inbox[selected].Author.Name);

                    //We retreive the appropriate connection.
                    Conversation conversation = conn.GetConversationWith(conn.Inbox[selected].Author);

                    Console.WriteLine("Loading messages...");

                    //Now we can load more messages until everything is loaded. Messages will be loaded in chunks of 30 messages per request.
                    //You most likely don't even need all messages, so you can load them on demand

                    //There is also an extension method that returns an IEnumerable<Message>, it is however synchronous.
                    //You can retreive the IEnumerable by calling conn.ConversationWith(USER) and using it driectly
                    //Example: foreach(Message m in conn.ConversationWith(USER))

                    while (conversation.NeedsRefresh)
                    {
                        Console.WriteLine("Loading more messages, we currently loaded {0} messages in total", conversation.Messages.Count);
                        await conversation.LoadMore();
                    }

                    suppressOutput = true;
                    //Now we can iterate over all retreived messages and print them to the console.
                    foreach (Message m in conversation.Messages)
                    {
                        Console.WriteLine("{0}:\t{1}", m.Author.Name.PadRight(16, ' '), m.Content);
                    }
                    suppressOutput = false;

                    //Finally we send a message on our own
                    Console.WriteLine("Enter a message to send (or press enter to skip)");

                    suppressOutput = true;
                    string content = Console.ReadLine();
                    suppressOutput = false;

                    if (content != String.Empty)
                    {
                        await conversation.SendMessage(content);
                    }
                }

                //Finally we sign out.
                await conn.SignoffAsync();
                Console.WriteLine("Signed out");
                done = true;
            }
        }
    }
}
