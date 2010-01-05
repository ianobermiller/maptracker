using System;
using System.Collections.Generic;

namespace MapTracker
{
    public class Constants
    {
        public const byte NodeStart = 0xFE;
        public const byte NodeEnd = 0xFF;
        public const byte Escape = 0xFD;

        public static Version GetMapVersion(ushort tibiaVersion)
        {
            if (tibiaVersion >= 854)
            {
                return new Version(0x03, 0x10);
            }
            else if (tibiaVersion >= 850)
            {
                return new Version(0x03, 0x0F);
            }
            else if (tibiaVersion >= 840)
            {
                return new Version(0x03, 0x0C);
            }
            else if (tibiaVersion >= 820)
            {
                return new Version(0x03, 0x0A);
            }
            else if (tibiaVersion >= 810)
            {
                return new Version(0x02, 0x08);
            }
            else // if (tibiaVersion >= 800)
            {
                return new Version(0x02, 0x07);
            }
        }
    }
}
