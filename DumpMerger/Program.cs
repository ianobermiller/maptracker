using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MapTracker.NET;
using Tibia.Objects;

namespace DumpMerger
{
    class Program
    {
        public static HashSet<Location> trackedTiles;
        public static Dictionary<ushort, TibiaItem> dic;

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                System.Console.Out.WriteLine("Drop the dump files here! Exiting...");
                System.Console.In.ReadLine();

                return;
            }

            System.Console.In.ReadLine();

            System.Console.Out.Write("Validating dump files... ");

            List<string> toParse = new List<string>();

            FileStream fio;
            foreach (string file in args)
            {
                fio = new FileStream(file, FileMode.Open);

                byte[] arr = new byte[4];
                fio.Read(arr, 0, 4);

                if ("DUMP".Equals(Encoding.ASCII.GetString(arr)))
                {
                    toParse.Add(file);
                }               
            }

            if (toParse.Count == 0)
            {
                System.Console.Out.WriteLine("no valid dump files where found, exiting.");
                return;
            }
            else
            {
                System.Console.Out.WriteLine("ok.");
            }

            System.Console.Out.Write("Openning items.otb... ");

            ItemsReader OTB_ItemsReader = new ItemsReader();
            OTB_ItemsReader.GetClientToServerDictionary();
            dic = ItemsReader.clientToServerDict;

            trackedTiles = new HashSet<Location>();

            string fn = Directory.GetCurrentDirectory() + "\\Merged at " + DateTime.Now.ToString("dd-MM-yyyy HH-mm-ss.ffff") + ".otbm";
            MapWriter map = new MapWriter(fn);
            map.WriteHeader();
            map.WriteMapStart();

            List<string>.Enumerator enumList = toParse.GetEnumerator();

            while (enumList.MoveNext())
            {
                string fileCurrent = enumList.Current;
                fio = new FileStream(fileCurrent, FileMode.Open);

                Location pos = new Location();
                bool ignoreTile;
                int clientId, serverId, extra, n;

                while (ReadTile(fio, pos))
                {
                    if (trackedTiles.Contains(pos))
                    {
                        ignoreTile = true;
                    }
                    else
                    {
                        ignoreTile = false;
                        trackedTiles.Add(pos);
                        n = 0;
                    }

                    if (!ignoreTile)
                    {
                        map.WriteNodeStart(NodeType.TileArea);
                        map.WriteTileAreaCoords(pos);

                        pos.X = 0; pos.Y = 0; pos.Z = 0;
                        map.WriteNodeStart(NodeType.Tile);
                        map.WriteTileCoords(pos);
                    }

                    while (ReadItem(clientId, serverId, extra))
                    {
                        if (!ignoreTile)
                        {
                            if (++n == 1)
                            {
                                map.WriteAttrType(AttrType.Item);
                                map.WriteUInt16(serverId);
                            }
                            else
                            {
                                map.WriteNodeStart(NodeType.Item);
                                map.WriteUInt16(serverId);

                                TibiaItem ti = dic[clientId];
                                if (ti.type != AttrType.None)
                                {
                                    map.WriteAttrType(ti.type);
                                    map.WriteByte(count);
                                }
                                map.WriteNodeEnd();
                            }
                        }
                    }

                    if (!ignoreTile)
                    {
                        map.WriteNodeEnd();
                        map.WriteNodeEnd();
                    }
                }

                fio.Close();
            }

            map.WriteNodeEnd();
            map.WriteNodeEnd();
            map.Close();
        }

        private static bool ReadTile(FileStream fio, Location pos)
        {
            if (fio.Position == fio.Length)
            {
                return false;
            }

            byte[] p = new byte[5];
            fio.Read(p, 0, 5);

            (*pos).X = p[0] | (p[1] << 8);
            (*pos).Y = p[2] | (p[3] << 8);
            (*pos).Z = p[4];
        }

        private static bool ReadItem(FileStream fio, int clientItem, int serverItem, int extra)
        {
            byte[] b = new byte[2];
            fio.Read(b, 0, 2);

            int item = b[0] | (b[1] << 8);

            if (item == 0xFFFF)
            {
                return false;
            }

            *clientItem = item + 100;

            TibiaItem ti = dic[*clientItem];
            *serverItem = ti.serverId;

            if (ti.type != AttrType.None)
            {
                fio.Read(b, 0, 1);
                *extra = b[0];
            }

            return true;
        }
    }
}
