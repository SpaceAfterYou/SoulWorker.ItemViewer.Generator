using SoulWorkerResearch.SoulCore;
using SoulWorkerResearch.SoulCore.Defines;
using SoulWorkerResearch.SoulCore.Extensions;
using SoulWorkerResearch.SoulCore.Utils;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ItemKeyType = System.UInt32;

namespace SoulWorker.ItemViewer.Generator.DataTypes;

public abstract class ItemResource(BinaryReader reader)
{
    public const string TableName = "tb_item";

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
    public int SimilarGroup { get; } = reader.ReadInt32();
    public int Package { get; } = reader.ReadInt32();
    public int Unknown76 { get; } = reader.ReadInt32();
    public int Unknown77 { get; } = reader.ReadInt32();
    public int Unknown78 { get; } = reader.ReadInt32();
    public int Unknown79 { get; } = reader.ReadInt32();
    public ushort Unknown { get; } = reader.ReadUInt16();

    private static Option[] ReadOptions(BinaryReader reader)
    {
        var classes = reader.ReadByteAsEnumerable(GameConfig.PerItemStatsCount);
        var types = reader.ReadUInt32AsEnumerable(GameConfig.PerItemStatsCount);
        var values = reader.ReadInt32AsEnumerable(GameConfig.PerItemStatsCount);

        return IterableUtils.Iterate(classes, types, values).Select(item => new Option(item.Item1, item.Item2, item.Item3)).ToArray();
    }
}