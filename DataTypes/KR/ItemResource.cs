using SoulWorkerResearch.SoulCore;
using SoulWorkerResearch.SoulCore.Defines;
using SoulWorkerResearch.SoulCore.Extensions;
using SoulWorkerResearch.SoulCore.IO.ResTable;
using SoulWorkerResearch.SoulCore.Utils;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SoulWorker.ItemViewer.Generator.DataTypes.KR;

using ItemScriptKeyType = System.UInt32;
using ItemClassifyKeyType = System.UInt32;
using ItemKeyType = System.UInt32;

public sealed class ItemScriptEntry : IEntry<ItemScriptEntry>
{
    static string IEntry<ItemScriptEntry>.TableName => "tb_item_script";

    public ItemScriptKeyType Id { get; }
    public string Icon { get; }
    public string Unknown6 { get; }
    public string Unknown7 { get; }
    public string Unknown8 { get; }
    public string Unknown9 { get; }
    public string Unknown10 { get; }
    public byte Unknown11 { get; }
    public byte Unknown12 { get; }
    public byte Unknown13 { get; }
    public byte Unknown14 { get; }
    public byte Unknown15 { get; }
    public string Name { get; }
    public string Description { get; }

    public ItemScriptEntry(BinaryReader reader)
    {
        Id = reader.ReadUInt32();
        Icon = reader.ReadUTF16UnicodeString();
        Unknown6 = reader.ReadUTF16UnicodeString();
        Unknown7 = reader.ReadUTF16UnicodeString();
        Unknown8 = reader.ReadUTF16UnicodeString();
        Unknown9 = reader.ReadUTF16UnicodeString();
        Unknown10 = reader.ReadUTF16UnicodeString();
        Unknown11 = reader.ReadByte();
        Unknown12 = reader.ReadByte();
        Unknown13 = reader.ReadByte();
        Unknown14 = reader.ReadByte();
        Unknown15 = reader.ReadByte();
        Name = reader.ReadUTF16UnicodeString();
        Description = reader.ReadUTF16UnicodeString();
    }
}

public sealed class ItemClassifyEntity(BinaryReader reader) : IEntry<ItemClassifyEntity>
{
    public const string TableName = "tb_Item_Classify";
    static string IEntry<ItemClassifyEntity>.TableName => TableName;

    public sealed record ActionInfo(string Move, string Drop, string Equip, string Unequip);

    public ItemClassifyKeyType Id { get; } = reader.ReadUInt32();
    public byte Group { get; } = reader.ReadByte();
    public ushort Unknown6 { get; } = reader.ReadUInt16();
    public byte SubGroup { get; } = reader.ReadByte();
    public ushort Unknown8 { get; } = reader.ReadUInt16();
    public byte Category { get; } = reader.ReadByte();
    public ushort Unknown10 { get; } = reader.ReadUInt16();
    public byte SubCategory { get; } = reader.ReadByte();
    public ushort Unknown12 { get; } = reader.ReadUInt16();
    public byte GainType { get; } = reader.ReadByte();
    public ItemClassifyInventoryType InventoryType { get; } = reader.ReadItemClassifyInventoryType();
    public ItemClassifySlotType SlotType { get; } = reader.ReadItemClassifySlotType();
    public byte RepairType { get; } = reader.ReadByte();
    public byte UseState { get; } = reader.ReadByte();
    public byte UseType { get; } = reader.ReadByte();
    public byte ConsumeType { get; } = reader.ReadByte();
    public ushort Unknown20 { get; } = reader.ReadUInt16();
    public ushort SocketId { get; } = reader.ReadUInt16();
    public ushort Unknown22 { get; } = reader.ReadUInt16();
    public ActionInfo Action { get; } = new(
            reader.ReadUTF16UnicodeString(),
            reader.ReadUTF16UnicodeString(),
            reader.ReadUTF16UnicodeString(),
            reader.ReadUTF16UnicodeString());
    public short Unknown27 { get; } = reader.ReadInt16();
}

public sealed class ItemResource(BinaryReader reader) : IEntry<ItemResource>
{
    static string IEntry<ItemResource>.TableName => "tb_item";

    public sealed record Specification(uint Min, uint Max, uint Magic);
    public sealed record Option(byte Class, uint Type, int Value);

    public ItemKeyType Id { get; } = reader.ReadUInt32();
    public uint ClassifyId { get; } = reader.ReadUInt32();
    public byte Unknown7 { get; } = reader.ReadByte();
    public byte MaxSlots { get; } = reader.ReadByte();
    public ushort Unknown9 { get; } = reader.ReadUInt16();
    public uint SellPrice { get; } = reader.ReadUInt32();
    public uint BuyPrice { get; } = reader.ReadUInt32();
    public uint RecycleSellPrice { get; } = reader.ReadUInt32();
    public uint RecycleBuyPrice { get; } = reader.ReadUInt32();
    public ushort StackMax { get; } = reader.ReadUInt16();
    public byte BindType { get; } = reader.ReadByte();
    public uint Unknown16 { get; } = reader.ReadUInt32();
    public uint Unknown17 { get; } = reader.ReadUInt32();
    public uint Info { get; } = reader.ReadUInt32();
    public ushort MinLevel { get; } = reader.ReadUInt16();
    public Character Class { get; } = reader.ReadClass();
    public byte LimitSellType { get; } = reader.ReadByte();
    public byte SubType { get; } = reader.ReadByte();
    public byte CostumeSetType { get; } = reader.ReadByte();
    public uint CostumeSet { get; } = reader.ReadUInt32();
    public string SlotDisable { get; } = reader.ReadUTF16UnicodeString();
    public byte Endurance { get; } = reader.ReadByte();
    public byte UseValue { get; } = reader.ReadByte();
    public Specification AttackDamage { get; } = new(reader.ReadUInt32(), reader.ReadUInt32(), reader.ReadUInt32());
    public Specification Defence { get; } = new(reader.ReadUInt32(), reader.ReadUInt32(), reader.ReadUInt32());
    public IReadOnlyList<Option> Options { get; } = ReadOptions(reader);
    public uint Unknown49 { get; } = reader.ReadUInt32();
    public uint Unknown50 { get; } = reader.ReadUInt32();
    public uint Unknown51 { get; } = reader.ReadUInt32();
    public ushort ItemReinforce { get; } = reader.ReadUInt16();
    public uint ReinforceOption { get; } = reader.ReadUInt32();
    public uint Title { get; } = reader.ReadUInt32();
    public uint Evolution { get; } = reader.ReadUInt32();
    public ushort Disassemble { get; } = reader.ReadUInt16();
    public uint Furniture { get; } = reader.ReadUInt32();
    public ushort CooltimeGroup { get; } = reader.ReadUInt16();
    public uint CooltimeValue { get; } = reader.ReadUInt32();
    public byte CooltimeSave { get; } = reader.ReadByte();
    public ushort EffectType { get; } = reader.ReadUInt16();
    public uint Effect { get; } = reader.ReadUInt32();
    public ushort Unknown63 { get; } = reader.ReadUInt16();
    public byte ItemCash { get; } = reader.ReadByte();
    public byte UsePeriodType { get; } = reader.ReadByte();
    public uint UsePeriodValue { get; } = reader.ReadUInt32();
    public byte Unknown67 { get; } = reader.ReadByte();
    public uint Unknown68 { get; } = reader.ReadUInt32();
    public byte SealingCnt { get; } = reader.ReadByte();
    public byte BreakCnt { get; } = reader.ReadByte();
    public uint SimilarGroup { get; } = reader.ReadUInt32();
    public uint Package { get; } = reader.ReadUInt32();
    public int Unknown76 { get; } = reader.ReadInt32();
    public int Unknown77 { get; } = reader.ReadInt32();
    public int Unknown78 { get; } = reader.ReadInt32();
    public int Unknown79 { get; } = reader.ReadInt32();
    public int Unknown { get; } = reader.ReadUInt16();

    private static Option[] ReadOptions(BinaryReader reader)
    {
        var classes = reader.ReadByteAsEnumerable(GameConfig.PerItemStatsCount);
        var types = reader.ReadUInt32AsEnumerable(GameConfig.PerItemStatsCount);
        var values = reader.ReadInt32AsEnumerable(GameConfig.PerItemStatsCount);

        return IterableUtils.Iterate(classes, types, values).Select(item => new Option(item.Item1, item.Item2, item.Item3)).ToArray();
    }
}
