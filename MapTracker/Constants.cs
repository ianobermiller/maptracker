using System;
using System.Collections.Generic;

namespace MapTracker
{
    public class Constants
    {
        public const byte NodeStart = 0xFE;
        public const byte NodeEnd = 0xFF;
        public const byte Escape = 0xFD;

        public static Dictionary<uint, Version> TibiaVersionToMapVersion = new Dictionary<uint,Version>
        {
            { 810, new Version(2, 8) },
            { 853, new Version(3, 16) }
        };
    }
}
