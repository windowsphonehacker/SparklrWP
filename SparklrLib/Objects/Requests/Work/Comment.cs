namespace SparklrLib.Objects.Requests.Work
{
    public class Comment
    {
        /// <summary>
        /// The original author
        /// </summary>
        public int to { get; set; }

        /// <summary>
        /// The post id
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// The comment content
        /// </summary>
        public string comment { get; set; }
    }
}
