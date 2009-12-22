using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tibia.Packets;
using System.IO;
using Tibia.Objects;
using System.Threading;

namespace MapTracker
{
    public partial class CamLoader : Form
    {
        Action<string> logMethod;
        Client client;
        ProxyBase proxy;
        long start;
        int totalPacketCount;
        int parsedCount;

        public CamLoader(Client client, ProxyBase proxy, Action<string> logMethod)
        {
            this.client = client;
            this.proxy = proxy;
            this.logMethod = logMethod;
            InitializeComponent();
        }

        private void CamLoader_Shown(object sender, EventArgs e)
        {
            this.Text = "Select a CAM recording...";

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Tibia CAM files (*.cam)|*.cam|All files (*.*)|*.*";
            dialog.Title = "Select a CAM recording";

            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                this.Close();
                return;
            }

            this.Text = "Tracking CAM file...";
            Action<string> parseAction = ParseAllPackets;
            parseAction.BeginInvoke(dialog.FileName, null, null);
        }

        private void ParseAllPackets(string fileName)
        {
            start = DateTime.Now.Ticks;
            try
            {
                //CAM FILE
                int version = 0;
                BinaryReader reader = new BinaryReader(new FileStream(fileName, FileMode.Open, FileAccess.Read));

                //- 32 unknown bytes (hash?)
                reader.ReadBytes(32);

                //- 4 bytes of Tibia version (for example 8.4 will look like 08 04 00 00)
                version += reader.ReadByte() * 100;
                version += reader.ReadByte() * 10;
                version += reader.ReadByte() * 1;
                reader.ReadByte();

                //- 4 bytes of metadata length (can be zero'ed)
                int metadataLength = reader.ReadInt32();

                //- x bytes of metadata (unknown, can be skipped)
                reader.ReadBytes(metadataLength);

                //- 4 bytes of compressed data length
                int compressedDataLength = reader.ReadInt32();

                //- 5 bytes of LZMA header
                //- 8 bytes of decompressed data length
                //- x bytes of compressed data
                byte[] compressedData = reader.ReadBytes(compressedDataLength + 13);
                byte[] uncompressedData = SevenZip.Compression.LZMA.SevenZipHelper.Decompress(compressedData);

                reader = new BinaryReader(new MemoryStream(uncompressedData));

                //Compressed data structure:
                //- 2 bytes (0x06 0x02) (constant?)
                reader.ReadBytes(2);
                //- 4 bytes - (numOfPackets + 57)
                int packetCount = reader.ReadInt32() - 57;

                this.totalPacketCount = packetCount;
                this.parsedCount = 0;

                //- x bytes of packets - one after one
                for (int i = 0; i < packetCount; i++)
                {                    
                    //Packet structure:
                    //- 2 bytes - (logicalPacketLen + 2)
                    reader.ReadInt16();
                    //- 4 bytes - timeStamp
                    int timestamp = reader.ReadInt32();
                    //- 2 bytes - logicalPacketLen
                    int packetlength = reader.ReadInt16();
                    //- x bytes - logical packet without headers (0a xx xx xx)
                    byte[] packet = reader.ReadBytes(packetlength);
                    //- 4 bytes - footer (can be anything)
                    reader.ReadUInt32();

                    ThreadPool.QueueUserWorkItem(new WaitCallback(delegate
                    {
                        proxy.ParseServerPacket(client, packet);
                        this.Invoke(new EventHandler(delegate { this.DoneParsing(); }));
                    }));

                    this.Invoke(new EventHandler(delegate { this.Owner.Update(); }));
                }
            }
            catch (Exception ex)
            {
                logMethod("Exception while loading CAM file:\n" + ex);
                this.Invoke(new EventHandler(delegate { this.Text = "Done, with errors"; }));
            }
        }

        void DoneParsing()
        {
            this.parsedCount++;
            this.uxProgess.Value = (int)(((double)this.parsedCount / this.totalPacketCount) * 100);

            if (this.parsedCount == this.totalPacketCount)
            {
                TimeSpan elapsed = new TimeSpan(DateTime.Now.Ticks - start);
                logMethod(String.Format("Tracked map from CAM in {0} min {1} sec.",
                    (int)elapsed.TotalMinutes, elapsed.Seconds));
                this.Invoke(new EventHandler(delegate { this.Text = "Done"; }));
                this.Invoke(new EventHandler(delegate { this.Close(); }));
            }
        }
    }
}
