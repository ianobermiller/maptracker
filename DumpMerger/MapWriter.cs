using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MapTracker.NET
{
    public class MapWriter
    {
        public static byte NodeStart = 0xFE;
        public static byte NodeEnd = 0xFF;
        public static byte Escape = 0xFD;

        public static long MapSizePosition = 10;

        private string fileName;
        private FileStream fileStream;
        public bool CanWrite { get; private set; }

        public MapWriter(string fileName)
        {
            this.fileName = fileName;
            CanWrite = OpenFile();
        }

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
    }
}
