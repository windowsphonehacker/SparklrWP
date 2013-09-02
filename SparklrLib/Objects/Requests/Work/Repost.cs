namespace SparklrLib.Objects.Requests.Work
{
    public class Repost
    {
        public Repost(int id, string reply)
        {
            this.id = id;
            this.reply = reply;
        }

        public Repost(int id)
        {
            this.id = id;
        }

        /// <summary>
        /// The post id
        /// </summary>
        public int id { get; private set; }

        /// <summary>
        /// The comment content
        /// </summary>
        public string reply { get; private set; }
    }
}
