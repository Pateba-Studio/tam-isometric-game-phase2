using System;
using System.Collections.Generic;

namespace Structures
{
    [Serializable]
    public class MediaSourceData
    {
        public int id;
        public string master_value;
        public string media;
        public string audio;
    }

    [Serializable]
    public class MediaSource
    {
        public bool success;
        public List<MediaSourceData> data;
        public bool watch_intro;
    }
}