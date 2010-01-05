using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tibia.Objects;

namespace MapTracker
{
    public partial class MiniMap : Form
    {
        Bitmap[] levels;
        int currentLevel = 0;
        int maxLevel = 0;
        public MiniMap()
        {
            InitializeComponent();
        }

        public void ShowMap(IEnumerable<OtMapTile> tiles, Location mapSize, Location mapBoundsNW, Location mapBoundsSE)
        {
            if (mapSize.Z == 0)
            {
                maxLevel = -1;
                return;
            }

            this.ClientSize = new Size(mapSize.X, mapSize.Y);
            levels = new Bitmap[mapSize.Z];
            currentLevel = 0;
            maxLevel = mapBoundsSE.Z - mapBoundsNW.Z;

            if (7 - mapBoundsNW.Z > 0 && 7 - mapBoundsNW.Z < maxLevel)
                currentLevel = 7 - mapBoundsNW.Z;

            foreach (var tile in tiles)
            {
                if (levels[tile.Location.Z - mapBoundsNW.Z] == null)
                {
                    levels[tile.Location.Z - mapBoundsNW.Z] = new Bitmap(mapSize.X, mapSize.Y);
                    Graphics g = Graphics.FromImage(levels[tile.Location.Z - mapBoundsNW.Z]);
                    g.Clear(Color.Black);
                }

                levels[tile.Location.Z - mapBoundsNW.Z].SetPixel(
                    tile.Location.X - mapBoundsNW.X, 
                    tile.Location.Y - mapBoundsNW.Y, 
                    tile.MapColor
                ); 
            }
            
            uxMap.Image = levels[currentLevel];
        }

        private void MiniMap_KeyDown(object sender, KeyEventArgs e)
        {
            if (maxLevel == -1)
                return;

            if (e.KeyCode == Keys.PageDown)
            {
                currentLevel++;
            }
            else if (e.KeyCode == Keys.PageUp)
            {
                currentLevel--;
            }
            if (currentLevel > maxLevel)
                currentLevel = maxLevel;
            else if (currentLevel < 0)
                currentLevel = 0;
            uxMap.Image = levels[currentLevel];
        }
    }
}
