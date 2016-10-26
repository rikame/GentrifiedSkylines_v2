using ICities;
using System;

namespace GentrifiedSkylines.Detours
{
    public class ChirpMessage : IChirperMessage
    {
        public ChirpMessage(String name, String body)
        {
            senderName = name;
            text = body;
        }

        public uint senderID
        {
            get { return 0; }
        }

        public string senderName
        {
            get; set;
        }

        public string text
        {
            get; set;
        }
    }
}