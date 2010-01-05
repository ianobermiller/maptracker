using System.Collections.Generic;
using System.Drawing;
using Tibia.Objects;

namespace MapTracker
{
    public class OtMapTile
    {
        public Location Location;
        public ushort TileId;
        public List<OtMapItem> Items = new List<OtMapItem>();
        public Color MapColor;
    }
}
