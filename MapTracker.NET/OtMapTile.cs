using System;
using System.Collections.Generic;
using Tibia;
using Tibia.Objects;

namespace MapTracker.NET
{
    public class OtMapTile
    {
        public Location Location;
        public ushort TileId;
        public List<OtMapItem> Items = new List<OtMapItem>();
    }
}
