using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Tibia.Objects;
using Tibia.Packets.Incoming;
using System.Xml;

namespace MapTracker
{
    public class OtbmMapWriter
    {
        #region Vars/Properties
        private string fileName;
        private FileStream fileStream;
        public bool CanWrite { get; private set; }
        #endregion

        #region Constructor
        public OtbmMapWriter(string fileName)
        {
            this.fileName = fileName;
            CanWrite = OpenFile();
        }
        #endregion

        #region Open/Close
        private bool OpenFile()
        {
            try
            {
                fileStream = File.OpenWrite(fileName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Close()
        {
            fileStream.Close();
        }
        #endregion

        #region Write
        public void WriteHeader(Version version)
        {
            // Version, unescaped
            WriteUInt32(0, false);

            // Root node
            WriteNodeStart(NodeType.RootV1);

            // Header information
            // Version
            WriteUInt32(2);
            // Width
            WriteUInt16(0xFCFC);
            // Height
            WriteUInt16(0xFCFC);

            // Major version items
            WriteUInt32((uint)version.Major);
            // Minor version items
            WriteUInt32((uint)version.Minor);
        }

        public void WriteMapStart(string houseFileName, string spawnFileName)
        {
            WriteNodeStart(NodeType.MapData);

            WriteAttrType(AttrType.Description);
            WriteString("Created with MapTracker: http://code.google.com/p/maptracker");

            WriteAttrType(AttrType.ExtHouseFile);
            WriteString(houseFileName);

            WriteAttrType(AttrType.ExpSpawnFile);
            WriteString(spawnFileName);
        }

        public void WriteNodeStart(NodeType type)
        {
            WriteByte(Constants.NodeStart, false);
            WriteByte((byte)type);
        }

        public void WriteNodeEnd()
        {
            WriteByte(Constants.NodeEnd, false);
        }

        public void WriteBytes(byte[] data, bool unescape)
        {
            foreach(byte b in data)
            {
                if (unescape && (b == Constants.NodeStart || b == Constants.NodeEnd || b == Constants.Escape))
                {
                    fileStream.WriteByte(Constants.Escape);
                }

                fileStream.WriteByte(b);
            }
        }

        public void WriteBytes(byte[] data)
        {
            WriteBytes(data, true);
        }

        public void WriteByte(byte value)
        {
            WriteByte(value, true);
        }

        public void WriteByte(byte value, bool unescape)
        {
            WriteBytes(new byte[] { value }, unescape);
        }

        public void WriteAttrType(AttrType at)
        {
            WriteByte((byte)at);
        }

        public void WriteUInt16(UInt16 value)
        {
            WriteBytes(BitConverter.GetBytes(value));
        }

        public void WriteUInt32(UInt32 value)
        {
            WriteUInt32(value, true);
        }

        public void WriteUInt32(UInt32 value, bool unescape)
        {
            WriteBytes(BitConverter.GetBytes(value), unescape);
        }

        public void WriteString(string text)
        {
            WriteUInt16((UInt16)text.Length);
            WriteBytes(Encoding.ASCII.GetBytes(text));
        }

        public void WriteTileAreaCoords(Tibia.Objects.Location loc)
        {
            WriteUInt16((UInt16)(loc.X & 0xFF00));
            WriteUInt16((UInt16)(loc.Y & 0xFF00));
            WriteByte((byte)loc.Z);
        }

        public void WriteTileCoords(Tibia.Objects.Location loc)
        {
            WriteByte((byte)loc.X);
            WriteByte((byte)loc.Y);
        }
        #endregion

        #region Static Methods
        public static string WriteMapTilesToFile(IEnumerable<OtMapTile> mapTiles, IEnumerable<PacketCreature> creatures, Version version)
        {
            string baseFileName = "mapdump_" + DateTime.Now.ToString("yyyyMMddHHmmss");
            string otbmFileName = baseFileName + ".otbm";
            string houseFileName = baseFileName + "-house.xml";
            string spawnFileName = baseFileName + "-spawn.xml";
            WriteOtbm(otbmFileName, houseFileName, spawnFileName, version, mapTiles);
            WriteHouses(baseFileName + "-house.xml");
            WriteCreatures(baseFileName + "-spawn.xml", creatures);
            return Path.Combine(Directory.GetCurrentDirectory(), otbmFileName);
        }

        private static void WriteCreatures(string spawnFileName, IEnumerable<PacketCreature> creatures)
        {
            using (XmlWriter writer = XmlWriter.Create(new FileStream(spawnFileName, FileMode.Create)))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spawns");
                foreach (var creature in creatures)
                {
                    if (creature.Id < 0x40000000)
                        continue; // Skip players

                    writer.WriteStartElement("spawn");

                        writer.WriteAttributeString("centerx", (creature.Location.X - 1).ToString());
                        writer.WriteAttributeString("centery", creature.Location.Y.ToString());
                        writer.WriteAttributeString("centerz", creature.Location.Z.ToString());
                        writer.WriteAttributeString("radius", "1");

                        writer.WriteStartElement("monster");

                            writer.WriteAttributeString("name", creature.Name);
                            writer.WriteAttributeString("x", "1");
                            writer.WriteAttributeString("y", "0");
                            writer.WriteAttributeString("z", creature.Location.Z.ToString());
                            writer.WriteAttributeString("spawntime", "60");
                           
                        writer.WriteEndElement();

                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.Close();
            }
        }

        private static void WriteHouses(string housesFileName)
        {
            using (XmlWriter writer = XmlWriter.Create(new FileStream(housesFileName, FileMode.Create)))
            {
                writer.WriteStartDocument();
                writer.WriteElementString("houses", "");
                writer.Close();
            }
        }

        private static void WriteOtbm(string otbmFileName, string houseFileName, string spawnFileName, Version version, IEnumerable<OtMapTile> mapTiles)
        {
            OtbmMapWriter mapWriter = new OtbmMapWriter(otbmFileName);
            mapWriter.WriteHeader(version);
            mapWriter.WriteMapStart(houseFileName, spawnFileName);
            foreach (OtMapTile tile in mapTiles)
            {
                mapWriter.WriteNodeStart(NodeType.TileArea);
                mapWriter.WriteTileAreaCoords(tile.Location);
                mapWriter.WriteNodeStart(NodeType.Tile);
                mapWriter.WriteTileCoords(tile.Location);

                if (tile.TileId > 0)
                {
                    mapWriter.WriteAttrType(AttrType.Item);
                    mapWriter.WriteUInt16(tile.TileId);
                }

                foreach (OtMapItem item in tile.Items)
                {
                    mapWriter.WriteNodeStart(NodeType.Item);
                    mapWriter.WriteUInt16(item.ItemId);
                    if (item.AttrType != AttrType.None)
                    {
                        mapWriter.WriteAttrType(item.AttrType);
                        mapWriter.WriteByte(item.Extra);
                    }
                    mapWriter.WriteNodeEnd();
                }

                mapWriter.WriteNodeEnd();
                mapWriter.WriteNodeEnd();
            }
            mapWriter.WriteNodeEnd(); // Map Data node
            mapWriter.WriteNodeEnd(); // Root node
            mapWriter.Close();
        }
        #endregion
    }
}
