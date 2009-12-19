using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Tibia.Objects;
using Tibia.Packets;
using Tibia.Util;
using Tibia.Packets.Incoming;
using Tibia.Constants;

namespace MapTracker
{
    // Todo:
    // Many invalid items
    // Splashes are the wrong color
    // Options for ignoring Magic walls, fields, dead bodies
    public partial class MainForm : Form
    {
        #region Variables
        ProxyBase proxy = null;
        bool useHookProxy = false;
        Client client;
        Dictionary<Location, OtMapTile> mapTiles;
        Location mapBoundsNW;
        Location mapBoundsSE;
        Location currentLocation;
        bool tracking;
        int trackedTileCount;
        int trackedItemCount;
        #endregion

        #region SplitPacket
        struct SplitPacket
        {
            public IncomingPacketType Type;
            public byte[] Packet;

            public SplitPacket(IncomingPacketType type, byte[] packet)
            {
                this.Type = type;
                this.Packet = packet;
            }
        }
        Queue<SplitPacket> packetQueue;
        #endregion

        #region Form Controls
        public MainForm(bool useHookProxy)
        {
            this.useHookProxy = useHookProxy;
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            ItemInfo.LoadItemsOtb();
            mapTiles = new Dictionary<Location, OtMapTile>();
            packetQueue = new Queue<SplitPacket>();

            Reset();

            client = ClientChooser.ShowBox();

            if (client != null)
            {
                if (!useHookProxy && client.LoggedIn)
                {
                    MessageBox.Show("Using the proxy requires that the client is not logged in.");
                    Application.Exit();
                }
                else if (!useHookProxy)
                {
                    client.Exited += new EventHandler(Client_Exited);
                    client.IO.StartProxy();
                }
                Start();
            }
            else
            {
                MessageBox.Show("MapTracker requires at least one running client.");
                Application.Exit();
            }
        }

        void Client_Exited(object sender, EventArgs e)
        {
            Invoke(new EventHandler(delegate
            {
                Stop();
                uxStart.Enabled = false;
                uxReset.Enabled = false;
            }));
        }

        private void uxStart_Click(object sender, EventArgs e)
        {
            if (tracking)
            {
                Stop();
            }
            else
            {
                Start();
            }
        }

        private void uxWrite_Click(object sender, EventArgs e)
        {
            if (mapTiles.Count > 0)
            {
                string file = OtbmMapWriter.WriteMapTilesToFile(mapTiles.Values);
                Log("All map data written to " + file);
            }
        }

        private void uxReset_Click(object sender, EventArgs e)
        {
            Reset();
        }
        #endregion

        #region Control
        private void Start()
        {
            if (useHookProxy)
            {
                proxy = new HookProxy(client);
                AddHooks();
            }
            else
            {
                proxy = client.IO.Proxy;
                AddHooks();
            }

            if (client.LoggedIn)
            {
                currentLocation = GetPlayerLocation();
            }

            uxLog.Clear();
            uxStart.Text = "Stop Map Tracking";
            tracking = true;
            uxReset.Enabled = false;
        }

        private void RemoveHooks()
        {
            if (proxy == null) return;
            proxy.ReceivedMapDescriptionIncomingPacket -= ReceivedMapPacket;
            proxy.ReceivedMoveNorthIncomingPacket -= ReceivedMapPacket;
            proxy.ReceivedMoveEastIncomingPacket -= ReceivedMapPacket;
            proxy.ReceivedMoveSouthIncomingPacket -= ReceivedMapPacket;
            proxy.ReceivedMoveWestIncomingPacket -= ReceivedMapPacket;
            proxy.ReceivedFloorChangeDownIncomingPacket -= ReceivedMapPacket;
            proxy.ReceivedFloorChangeUpIncomingPacket -= ReceivedMapPacket;
        }

        private void AddHooks()
        {
            if (proxy == null) return;
            RemoveHooks();
            proxy.ReceivedMapDescriptionIncomingPacket += ReceivedMapPacket;
            proxy.ReceivedMoveNorthIncomingPacket += ReceivedMapPacket;
            proxy.ReceivedMoveEastIncomingPacket += ReceivedMapPacket;
            proxy.ReceivedMoveSouthIncomingPacket += ReceivedMapPacket;
            proxy.ReceivedMoveWestIncomingPacket += ReceivedMapPacket;
            proxy.ReceivedFloorChangeDownIncomingPacket += ReceivedMapPacket;
            proxy.ReceivedFloorChangeUpIncomingPacket += ReceivedMapPacket;
        }

        private void Stop()
        {
            RemoveHooks();

            uxStart.Text = "Start Map Tracking";
            tracking = false;
            uxReset.Enabled = true;
        }

        private void Reset()
        {
            mapTiles.Clear();
            mapBoundsNW = Tibia.Objects.Location.Invalid;
            mapBoundsSE = Tibia.Objects.Location.Invalid;
            trackedTileCount = 0;
            trackedItemCount = 0;
            UpdateStats();
        }

        private void UpdateStats()
        {
            Invoke(new EventHandler(delegate
            {
                uxTrackedTiles.Text = trackedTileCount.ToString("0,0");
                uxTrackedItems.Text = trackedItemCount.ToString("0,0");
                uxTrackedMapSize.Text = GetMapSize().ToString();
                uxMapBoundsNW.Text = mapBoundsNW.ToString();
                uxMapBoundsSE.Text = mapBoundsSE.ToString();
            }));
        }

        private Location GetPlayerLocation()
        {
            return client.GetPlayer().Location;
        }
        #endregion

        #region Process Packets
        private bool ReceivedMapPacket(IncomingPacket packet)
        {
            lock (this)
            {
                MapPacket p = (MapPacket)packet;
                foreach (Tile tile in p.Tiles)
                {
                    if (!mapTiles.ContainsKey(tile.Location))
                    {
                        if (uxTrackCurrentFloor.Checked && tile.Location.Z 
                            != client.PlayerLocation.Z)
                            continue;

                        SetNewMapBounds(tile.Location);
                        OtMapTile mapTile = new OtMapTile();
                        mapTile.Location = tile.Location;

                        tile.Items.Add(tile.Ground);

                        foreach (Item item in tile.Items)
                        {
                            ItemInfo info = ItemInfo.GetItemInfo((ushort)item.Id);

                            if (info == null)
                            {
                                Log("ClientId not in items.otb: " + item.Id.ToString());
                                break;
                            }

                            if (!uxTrackMovable.Checked && !item.GetFlag(Tibia.Addresses.DatItem.Flag.IsImmovable))
                                continue;
                            if (!uxTrackSplashes.Checked && item.GetFlag(Tibia.Addresses.DatItem.Flag.IsSplash))
                                continue;

                            if (info.Group == ItemGroup.Ground)
                            {
                                mapTile.TileId = info.Id;
                                break;
                            }

                            OtMapItem mapItem = new OtMapItem();
                            mapItem.AttrType = AttrType.None;

                            mapItem.ItemId = info.Id;

                            if (item.HasExtraByte)
                            {
                                byte extra = item.Count;
                                if (item.GetFlag(Tibia.Addresses.DatItem.Flag.IsRune))
                                {
                                    mapItem.AttrType = AttrType.Charges;
                                    mapItem.Extra = extra;
                                }
                                else if (item.GetFlag(Tibia.Addresses.DatItem.Flag.IsStackable) ||
                                    item.GetFlag(Tibia.Addresses.DatItem.Flag.IsSplash))
                                {
                                    mapItem.AttrType = AttrType.Count;
                                    mapItem.Extra = extra;
                                }
                            }
                            mapTile.Items.Add(mapItem);
                        }
                        mapTiles.Add(tile.Location, mapTile);
                        trackedTileCount++;
                        trackedItemCount += mapTile.Items.Count;
                        UpdateStats();
                    }
                }
            }
            return true;
        }
        #endregion

        #region Helpers
        private void SetNewMapBounds(Location loc)
        {
            if (mapBoundsNW.Equals(Tibia.Objects.Location.Invalid))
            {
                mapBoundsNW = loc;
                mapBoundsSE = loc;
            }
            else
            {
                if (loc.X < mapBoundsNW.X)
                    mapBoundsNW.X = loc.X;
                if (loc.Y < mapBoundsNW.Y)
                    mapBoundsNW.Y = loc.Y;
                if (loc.Z < mapBoundsNW.Z)
                    mapBoundsNW.Z = loc.Z;

                if (loc.X > mapBoundsSE.X)
                    mapBoundsSE.X = loc.X;
                if (loc.Y > mapBoundsSE.Y)
                    mapBoundsSE.Y = loc.Y;
                if (loc.Z > mapBoundsSE.Z)
                    mapBoundsSE.Z = loc.Z;
            }
        }

        private Location GetMapSize()
        {
            Tibia.Objects.Location size = new Location();
            size.X = mapBoundsSE.X - mapBoundsNW.X + 1;
            size.Y = mapBoundsSE.Y - mapBoundsNW.Y + 1;
            size.Z = mapBoundsSE.Z - mapBoundsNW.Z + 1;
            if (size.Equals(new Location(1, 1, 1)))
                return new Location(0, 0, 0);
            return size;
        }

        private void Log(string text)
        {
            Invoke(new EventHandler(delegate
            {
                uxLog.AppendText(text + Environment.NewLine);
            }));
        }
        #endregion
    }
}
