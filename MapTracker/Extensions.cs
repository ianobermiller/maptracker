using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapTracker
{
    public static class Extensions
    {
        public static bool HasFlag(this ItemFlags e, ItemFlags x)
        {
            return (e & x) == x;
        }
    }
}
