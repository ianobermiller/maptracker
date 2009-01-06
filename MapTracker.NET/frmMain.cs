using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Tibia.Objects;
using Tibia.Packets;
using System.IO;
using Tibia.Util;

namespace MapTracker.NET
{
    public partial class frmMain : Form
    {
        #region Variables
        Client client;
        List<Client> lc;
        HashSet<Location> trackedTiles;
        
        FileStream fio;
        #endregion

        #region Form Controls
        public frmMain()
        {
            InitializeComponent();
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            if (!ReloadClients())
            {
                MessageBox.Show("First, start a Tibia client!");
                Application.Exit();
            }
        }

        private void btnTracking_Click(object sender, EventArgs e)
        {
            if (btnTracking.Text == "Stop Map Tracking")
            {
                StopTracking();
            }
            else if (btnTracking.Text == "Start Map Tracking")
            {
                StartTracking();
            }
        }

        private void cbClientList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (btnTracking.Text == "Stop Map Tracking")
            {
                StopTracking();

                client = lc[cbClientList.SelectedIndex];
            }
        }
        #endregion

        #region Misc
        private bool ReloadClients()
        {
            cbClientList.Items.Clear();
            cbClientList.Text = "";
            lc = Client.GetClients();
            if (lc.Count > 0)
            {
                foreach (Client c in lc)
                {
                    cbClientList.Items.Add(c.ToString());
                }

                cbClientList.SelectedIndex = 0;
                client = lc[0];

                return true;
            }
            else
            {
                return false;
            }
        }

        private void StartTracking()
        {
            btnTracking.Text = "Stop Map Tracking";

            try
            {
                string fn = Directory.GetCurrentDirectory() + "\\Dump at " + DateTime.Now.ToString("dd-MM-yyyy HH-mm-ss.ffff") + ".dumpfile";
                fio = new FileStream(fn, FileMode.CreateNew);
                fio.Write(new byte[4] { (byte)'D', (byte)'U', (byte)'M', (byte)'P' }, 0, 4); // DUMP
            }
            catch (Exception ex)
            {
                MessageBox.Show("Couldn't create dump file! Exiting.\n\n" + ex.StackTrace);
                Application.Exit();
            }
            finally
            {
                trackedTiles = new HashSet<Location>();

                client.StopRawSocket();
                client.StartRawSocket();
                client.RawSocket.IncomingSplitPacket += new RawSocket.SplitPacket(RawSocket_IncomingSplitPacket);
            }
        }

        private void StopTracking()
        {
            btnTracking.Text = "Start Map Tracking";

            client.StopRawSocket();

            fio.Close();
            fio = null;

            trackedTiles.Clear();
            trackedTiles = null;
        }
        #endregion

        #region Packet Tracking, Parsing and Dumping
        private void RawSocket_IncomingSplitPacket(byte type, byte[] packet)
        {
            if (type < 0x64 || type > 0x68)
                return;

            NetworkMessage msg = new NetworkMessage(packet);
            Location pos;

            type = msg.GetByte();

            if (type == 0x64)
            {
                pos = msg.GetLocation();
                parseMapDescription(msg, pos.X - 8, pos.Y - 6, pos.Z, 18, 14);
                return;
            }

            pos = client.GetPlayer().Location;

            if (type == 0x65)
            {
                pos.Y--;
                parseMapDescription(msg, pos.X - 8, pos.Y - 6, pos.Z, 18, 1);
            }

            if (type == 0x66)
            {
                pos.X++;
                parseMapDescription(msg, pos.X + 9, pos.Y - 6, pos.Z, 1, 14);
            }

            if (type == 0x67)
            {
                pos.Y++;
                parseMapDescription(msg, pos.X - 8, pos.Y + 7, pos.Z, 18, 1);
            }

            if (type == 0x68)
            {
                pos.X--;
                parseMapDescription(msg, (int)(pos.X - 8), (int)(pos.Y - 6), (int)(pos.Z), 1, 14);
            }
        }

        protected short m_skipTiles;

        protected bool parseMapDescription(NetworkMessage msg, int x, int y, int z, int width, int height)
        {
            int startz, endz, zstep;
            //calculate map limits
            if (z > 7)
            {
                startz = z - 2;
                endz = System.Math.Min(16 - 1, z + 2);
                zstep = 1;
            }
            else
            {
                startz = 7;
                endz = 0;
                zstep = -1;
            }

            for (int nz = startz; nz != endz + zstep; nz += zstep)
            {
                //pare each floor
                if (!parseFloorDescription(msg, x, y, nz, width, height, z - nz))
                    return false;
            }

            return true;
        }

        protected bool parseFloorDescription(NetworkMessage msg, int x, int y, int z, int width, int height, int offset)
        {
            ushort skipTiles;

            for (int nx = 0; nx < width; nx++)
            {
                for (int ny = 0; ny < height; ny++)
                {
                    if (m_skipTiles == 0)
                    {
                        ushort tileOpt = msg.PeekUInt16();
                        if (tileOpt >= 0xFF00)
                        {
                            skipTiles = msg.GetUInt16();                            
                            m_skipTiles = (short)(skipTiles & 0xFF);
                        }
                        else
                        {
                            Location pos = new Location(x + nx + offset, y + ny + offset, z);

                            if (!parseTileDescription(msg, pos))
                            {
                                return false;
                            }
                            skipTiles = msg.GetUInt16();
                            m_skipTiles = (short)(skipTiles & 0xFF);
                        }
                    }
                    else
                    {
                        m_skipTiles--;
                    }
                }
            }
            return true;
        }

        bool ignoreTile;

        protected bool parseTileDescription(NetworkMessage msg, Location pos)
        {
            if (trackedTiles.Contains(pos))
            {
                ignoreTile = true;
            }
            else
            {
                ignoreTile = false;
                trackedTiles.Add(pos);

                fio.WriteByte((byte)(pos.X));
                fio.WriteByte((byte)(pos.X >> 8));
                fio.WriteByte((byte)(pos.Y));
                fio.WriteByte((byte)(pos.Y >> 8));
                fio.WriteByte((byte)(pos.Z));
            }

            int n = 0;
            while (true)
            {
                n++;

                ushort inspectTileId = msg.PeekUInt16();

                if (inspectTileId >= 0xFF00)
                {
                    if (!ignoreTile)
                    {
                        fio.WriteByte(0xFF);
                        fio.WriteByte(0xFF);
                    }

                    return true;
                }
                else
                {
                    if (n > 10)
                    {
                        return false;
                    }

                    try
                    {
                        internalGetThing(msg, pos);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        protected bool internalGetThing(NetworkMessage msg, Location pos)
        {
            ushort thingId = msg.GetUInt16();

            if (thingId == 0x0061 || thingId == 0x0062)
            {

                if (thingId == 0x0062)
                {
                    msg.Position += 4;
                }
                else if (thingId == 0x0061)
                {
                    msg.Position += 8;
                    int len = msg.GetUInt16();
                    msg.Position += len;
                }

                msg.Position += 2;
                int outfit = msg.GetUInt16();
                if (outfit == 0)
                    msg.Position += 2;
                else
                    msg.Position += 5;
                msg.Position += 6;

                return true;
            }
            else if (thingId == 0x0063)
            {
                msg.Position += 5;

                return true;
            }
            else
            {
                if (!ignoreTile)
                {
                    Item item = new Item(client, thingId);
                    thingId -= 100;

                    fio.WriteByte((byte)thingId);
                    fio.WriteByte((byte)(thingId >> 8));

                    if (item.HasExtraByte)
                    {
                        fio.WriteByte(msg.GetByte());
                    }
                }

                return true;
            }
        }
        #endregion
    }
}
