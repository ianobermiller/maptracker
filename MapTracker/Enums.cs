using System;

namespace MapTracker
{
    public enum NodeType : byte
    {
        RootV1 = 0, // 1 does not work
        MapData = 2,
        ItemDef = 3,
        TileArea = 4,
        Tile = 5,
        Item = 6,
        TileSquare = 7,
        TileRef = 8,
        Spawns = 9,
        SpawnArea = 10,
        Monster = 11,
        Towns = 12,
        Town = 13,
        HouseTile = 14,
        Waypoints = 15,
        Waypoint = 16
    }

    public enum AttrType : byte
    {
        None = 0,
        Description = 1,
        ExtFile = 2,
        TileFlags = 3,
        ActionID = 4,
        UniqueID = 5,
        Text = 6,
        Desc = 7,
        TeleDest = 8,
        Item = 9,
        DepotID = 10,
        ExpSpawnFile = 11,
        RuneCharges = 12,
        ExtHouseFile = 13,
        HouseDoorID = 14,
        Count = 15,
        Duration = 16,
        DecayingState = 17,
        WrittenDate = 18,
        WrittenBy = 19,
        SleeperGUID = 20,
        SleepStart = 21,
        Charges = 22
    }

    public enum ItemAttribute : byte
    {
        ServerId = 0x10,
        ClientId,
        Name,				/*deprecated*/
        Description,			/*deprecated*/
        Speed,
        Slot,				/*deprecated*/
        MaxItems,			/*deprecated*/
        Weight,			/*deprecated*/
        Weapon,			/*deprecated*/
        Ammunition,				/*deprecated*/
        Armor,			/*deprecated*/
        MagicLevel,			/*deprecated*/
        MagicFieldType,		/*deprecated*/
        Writeable,		/*deprecated*/
        RotateTo,			/*deprecated*/
        Decay,			/*deprecated*/
        SpriteHash,
        MiniMapColor,
        Attr07,
        Attr08,
        Light,

        //1-byte aligned
        Decay2,			/*deprecated*/
        Weapon2,			/*deprecated*/
        Ammunition2,				/*deprecated*/
        Armor2,			/*deprecated*/
        Writeable2,		/*deprecated*/
        Light2,
        TopOrder,
        Writeable3		/*deprecated*/
    }

    public enum ItemGroup
    {
        None = 0,
        Ground,
        Container,
        Weapon,
        Ammunition,
        Armor,
        Charges,
        Teleport,
        MagicField,
        Writeable,
        Key,
        Splash,
        Fluid,
        Door,
        Deprecated,
        Depot,
        Mailbox,
        TrashHolder,
        Bed
    }

    [FlagsAttribute]
    public enum ItemFlags : uint
    {
        BlocksSolid = 1,
        BlocksProjectile = 2,
        BlocksPathFinding = 4,
        HasHeight = 8,
        Useable = 16,
        Pickupable = 32,
        Moveable = 64,
        Stackable = 128,
        FloorChangeDown = 256,
        FloorChangeNorth = 512,
        FloorChangeEast = 1024,
        FloorChangeSouth = 2048,
        FloorChangeWest = 4096,
        AlwaysOnTop = 8192,
        Readable = 16384,
        Rotatable = 32768,
        Hangable = 65536,
        Vertical = 131072,
        Horizontal = 262144,
        CannotDecay = 524288,
        AllowDistanceRead = 1048576,
        Unused = 2097152,
        ClientCharges = 4194304,
        LookThrough = 8388608
    }
}
