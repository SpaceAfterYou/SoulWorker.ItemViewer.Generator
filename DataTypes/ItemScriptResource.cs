using SoulWorkerResearch.SoulCore.Extensions;
using System.IO;

namespace SoulWorker.ItemViewer.Generator.DataTypes;

using ItemScriptKeyType = System.UInt32;

public abstract class ItemScriptResource(BinaryReader reader)
{
    public ItemScriptKeyType Id { get; } = reader.ReadUInt32();
    public string Icon { get; } = reader.ReadUTF16UnicodeString();
    public string Unknown6 { get; } = reader.ReadUTF16UnicodeString();
    public string Unknown7 { get; } = reader.ReadUTF16UnicodeString();
    public string Unknown8 { get; } = reader.ReadUTF16UnicodeString();
    public string Unknown9 { get; } = reader.ReadUTF16UnicodeString();
    public string Unknown10 { get; } = reader.ReadUTF16UnicodeString();
    public byte Unknown11 { get; } = reader.ReadByte();
    public byte Unknown12 { get; } = reader.ReadByte();
    public byte Unknown13 { get; } = reader.ReadByte();
    public byte Unknown14 { get; } = reader.ReadByte();
    public byte Unknown15 { get; } = reader.ReadByte();
    public string Name { get; } = reader.ReadUTF16UnicodeString();
    public string Description { get; } = reader.ReadUTF16UnicodeString();
}
