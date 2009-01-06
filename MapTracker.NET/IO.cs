using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapTracker.NET
{
    class NodeStruct
    {
        public NodeStruct()
        {
            start = 0;
            propsSize = 0;
            next = 0;
            child = 0;
            type = 0;
        }

        private UInt64 start;
        private UInt64 propsSize;
        private UInt64 type;
        NodeStruct next;
        NodeStruct child;
    }
}
