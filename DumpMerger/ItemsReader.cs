using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MapTracker.NET
{
    public class TibiaItem
    {
        public int serverId;
        public bool hasExtraByte;
        public AttrType type;
    }

    public class ItemsReader
    {
        public const byte NodeStart = 0xFE;
        public const byte NodeEnd = 0xFF;
        public const byte Escape = 0xFD;

        private string fileName = "items.otb";
        public static Dictionary<ushort, TibiaItem> clientToServerDict;
        FileStream stream;
        byte[] buffer = new byte[128];

        public ItemsReader()
        {
            clientToServerDict = new Dictionary<ushort, TibiaItem>();
        }

        public void GetClientToServerDictionary()
        {
            stream = File.OpenRead(fileName);

            bool unparseNext = false;
            int cur;
            while ((cur = stream.ReadByte()) != -1)
            {
                switch (cur)
                {
                    case NodeStart:
                        if (unparseNext)
                        {
                            unparseNext = false;
                        }
                        else
                        {
                            int type = stream.ReadByte();
                            if (type >=0 && type <= 18)
                                HandleItem(type);
                            break;
                        }
                        break;
                    case NodeEnd:
                        if (unparseNext)
                        {
                            unparseNext = false;
                        }
                        else
                        {

                        }
                        break;
                    case Escape:
                        unparseNext = true;
                        break;
                }
            }

            return;
        }

        private void HandleItem(int itemGroup)
        {
            ushort serverId = 0;
            ushort clientId = 0;
            // 4 flag bytes
            ushort flags = BitConverter.ToUInt16(ReadAndUnescape(4), 0);

            byte attr = ReadAndUnescape(1)[0];
            ushort len = BitConverter.ToUInt16(ReadAndUnescape(2), 0);

            if (attr == 0x10)
            {
                serverId = BitConverter.ToUInt16(ReadAndUnescape(2), 0);
            }
            attr = ReadAndUnescape(1)[0];
            if (attr == 0x11)
            {
                len = BitConverter.ToUInt16(ReadAndUnescape(2), 0);
                clientId = BitConverter.ToUInt16(ReadAndUnescape(2), 0);
            }

            if (clientId > 0 && !clientToServerDict.ContainsKey(clientId))
            {
                TibiaItem ti = new TibiaItem();
                ti.serverId = serverId;
                ti.type = AttrType.None;

                if ((flags & 128) == 128)
                {
                    ti.type = AttrType.Count;
                }

                if (itemGroup == 6) // rune
                {
                    ti.type = AttrType.Charges;
                }

                if (itemGroup == 11) // stacks
                {
                    ti.type = AttrType.Count;
                }

                //if (itemGroup == 12) // fluid
                //{
                //    ti.attribute = 4;
                //    ti.hasExtraByte = true;
                //}

                clientToServerDict.Add(clientId, ti);
            }
        }

        private byte[] ReadAndUnescape(int count)
        {
            byte[] buffer = new byte[count];
            for (int i = 0; i < count; i++)
            {
                // read the server and client ids
                byte tmp = (byte)stream.ReadByte();

                if (tmp == Escape)
                {
                    buffer[i] = (byte)stream.ReadByte();
                }
                else
                {
                    buffer[i] = tmp;
                }
            }
            return buffer;
        }
    }
}
