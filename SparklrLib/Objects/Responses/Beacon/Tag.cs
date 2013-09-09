﻿
namespace SparklrLib.Objects.Responses.Beacon
{
    public class Datum
    {
        public int from { get; set; }
        public int id { get; set; }
        public int to { get; set; }
        public int type { get; set; }
        public object flags { get; set; }
        public string meta { get; set; }
        public string imageUrl
        {
            get
            {
                return meta.Split(',')[0];
            }
        }
        public int time { get; set; }
        public int @public { get; set; }
        public string message { get; set; }
        public int? via { get; set; }
        public int? origid { get; set; }
        public int? commentcount { get; set; }
        public int? modified { get; set; }
        public string network { get; set; }
    }

    public class Tag : BeaconBase
    {
        public Datum[] data { get; set; }
    }
}