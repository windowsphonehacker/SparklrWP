
namespace SparklrLib.Objects.Responses
{
    public class InitialPayload
    {
        public object @private { get; set; }
        public string[] networks { get; set; }
        public int avatarid { get; set; }
        public object[] blacklist { get; set; }
        public Objects.Responses.Beacon.Timeline[] timelineStream { get; set; }

        //Not returned as Array by the API.
        public object displayNames { get; set; }
        public object userHandles { get; set; }
    }
}
