using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MapTracker.NET
{
    public class OtbReader : OtFileManager
    {
        private string fileName = "items.otb";
        private Dictionary<ushort, ushort> clientToServerDict;
        FileStream stream;
        byte[] buffer = new byte[128];

        public OtbReader()
        {
            clientToServerDict = new Dictionary<ushort, ushort>();
        }
        
        public Dictionary<ushort, ushort> GetClientToServerDictionary()
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
                            if (type >=0 && type <= 13)
                                HandleItem();
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

            return clientToServerDict;
        }

        private void HandleItem()
        {
            ushort serverId = 0;
            ushort clientId = 0;
            // skip 4 flag bytes
            ReadAndUnescape(4);

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
                clientToServerDict.Add(clientId, serverId);
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
