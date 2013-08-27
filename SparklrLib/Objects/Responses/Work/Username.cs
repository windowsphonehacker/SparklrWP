
namespace SparklrLib.Objects.Responses.Work
{
    public class Username
    {
        public string username { get; set; }
        public int id { get; set; }

        public override string ToString()
        {
            return id + ": " + username;
        }
    }
}