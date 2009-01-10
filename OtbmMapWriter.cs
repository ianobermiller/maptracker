using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Tibia.Objects;

namespace MapTracker.NET
{
    public class OtbmMapWriter : OtFileManager
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
        public void WriteHeader()
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
            WriteUInt32(3);
            // Minor version items
            WriteUInt32(12);
        }

        public void WriteMapStart()
        {
            WriteNodeStart(NodeType.MapData);
            // write descriptions
            // spawn file
            // house file
        }

        public void WriteNodeStart(NodeType type)
        {
            WriteByte(NodeStart, false);
            WriteByte((byte)type);
        }

        public void WriteNodeEnd()
        {
            WriteByte(NodeEnd, false);
        }

        public void WriteDescription(string text)
        {
            WriteAttrType(AttrType.Description);
            WriteString(text);
        }

        public void WriteBytes(byte[] data, bool unescape)
        {
            foreach(byte b in data)
            {
                if (unescape && (b == NodeStart || b == NodeEnd || b == Escape))
                {
                    fileStream.WriteByte(Escape);
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
        public static void WriteMapTilesToFile(IEnumerable<OtMapTile> mapTiles)
        {
            string fn = Directory.GetCurrentDirectory() + "\\mapdump_" + DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss.ffff") + ".otbm";
            OtbmMapWriter mapWriter = new OtbmMapWriter(fn);
            mapWriter.WriteHeader();
            mapWriter.WriteMapStart();
            foreach (OtMapTile tile in mapTiles)
            {
                mapWriter.WriteNodeStart(NodeType.TileArea);
                mapWriter.WriteTileAreaCoords(tile.Location);
                mapWriter.WriteNodeStart(NodeType.Tile);
                mapWriter.WriteTileCoords(tile.Location);

                mapWriter.WriteAttrType(AttrType.Item);
                mapWriter.WriteUInt16(tile.TileId);

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
