
using System;
using System.Threading.Tasks;
namespace SparklrWP.Utils
{
    public static class NotificationHelpers
    {
        public async static Task<string> Format(int type, string body, int user, SparklrLib.SparklrClient client)
        {
            var result = await client.GetUsernamesAsync(new int[] { user });

            string username = user.ToString();
            if (result.IsSuccessful && result.Object.Length >= 1)
                username = result.Object[0].username;

            switch (type)
            {
                case 1:
                    if (body == "☝")
                    {
                        return String.Format("{0} likes your post.", username);
                    }
                    else
                    {
                        return String.Format("{0} commented {1}.", username, body);
                    }
                case 2:
                    return String.Format("{0} mentioned you.", username);
                case 3:
                    return String.Format("{0} messaged you.", username);
                default:
#if DEBUG
                    throw new NotSupportedException("This type is not implemented");
#else
                    return "";
#endif
            }
        }

        public static Uri GenerateActionUri(SparklrLib.Objects.Responses.Beacon.Notification n)
        {
            switch (n.type)
            {
                case 1:
                case 2:
                    return new Uri("/Pages/DetailsPage.xaml?id=" + n.action, UriKind.Relative);
                case 3:
                    return new Uri("/Pages/ChatPage.xaml?id=" + n.from, UriKind.Relative);
                default:
#if DEBUG
                    throw new NotSupportedException("The type is not supported");
#else
                    return null;
#endif
            }
        }
    }
}
