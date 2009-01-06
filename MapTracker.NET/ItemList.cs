using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapTracker.NET
{
    class ItemType
    {
        private int clientId;

        private int serverId;

        private bool hasExtraByte;

        public int ClientID
        {
            get
            {
                return clientId;
            }
            set
            {
                clientId = value;
            }
        }

        public int ServerID
        {
            get
            {
                return serverId;
            }
            set
            {
                serverId = value;
            }
        }

        public bool HasExtraByte
        {
            get
            {
                return hasExtraByte;
            }
            set
            {
                hasExtraByte = value;
            }
        }
    }

    class ItemList
    {
    }
}
