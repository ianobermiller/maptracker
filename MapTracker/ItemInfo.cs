using System.Collections.Generic;

namespace MapTracker
{
    public class ItemInfo
    {
        public ushort Id;
        public ushort SpriteId = 100;
        public ItemGroup Group = ItemGroup.None;
        public bool IsAlwaysOnTop = false;
        public byte TopOrder = 5;
        public bool IsBlocking = false;
        public bool IsProjectileBlocking = false;
        public bool IsPathBlocking = false;
        public bool HasHeight = false;
        public bool IsUseable = false;
        public bool IsPickupable = false;
        public bool IsMoveable = false;
        public bool IsStackable = false;
        public bool IsVertical = false;
        public bool IsHorizontal = false;
        public bool IsHangable = false;
        public bool IsDistanceReadable = false;
        public bool IsRotatable = false;
        public bool IsReadable = false;
        public bool IsWriteable = false;
        public bool HasClientCharges = false;
        public bool CanLookThrough = false;

        private static Dictionary<ushort, ItemInfo> itemInfoDictionary = new Dictionary<ushort, ItemInfo>();

        public static ItemInfo GetItemInfo(ushort itemId)
        {
            if (itemInfoDictionary.ContainsKey(itemId))
                return itemInfoDictionary[itemId];
            else
                return null;
        }

        public static void LoadItemsOtb()
        {
            OtbReader reader = new OtbReader();
            foreach (var info in reader.GetAllItemInfo())
            {
                if (!itemInfoDictionary.ContainsKey(info.SpriteId))
                {
                    itemInfoDictionary.Add(info.SpriteId, info);
                }
            }
        }
    }
}
