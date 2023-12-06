using SoulWorkerResearch.SoulCore.Defines;
using SoulWorkerResearch.SoulCore.Extensions;
using SoulWorkerResearch.SoulCore.IO.ResTable;
using System.IO;

namespace SoulWorker.ItemViewer.Generator.DataTypes;

using ItemClassifyKeyType = System.UInt32;

public sealed class ItemClassifyResource(BinaryReader reader) : IEntry<ItemClassifyResource>
{
    public const string TableName = "tb_Item_Classify";
    static string IEntry<ItemClassifyResource>.TableName => TableName;

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
