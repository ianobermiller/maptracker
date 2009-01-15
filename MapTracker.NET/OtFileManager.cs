using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapTracker.NET
{
    public class OtFileManager
    {
        #region Constants
        public const byte NodeStart = 0xFE;
        public const byte NodeEnd = 0xFF;
        public const byte Escape = 0xFD;
        #endregion
    }
}
