using SoulWorkerResearch.SoulCore.IO.ResTable;
using System.IO;

namespace SoulWorker.ItemViewer.Generator.DataTypes.GLB;

public sealed class English_ItemResource(BinaryReader reader) : ItemResource(reader), IEntry<English_ItemResource>
{
    static string IEntry<English_ItemResource>.TableName => TableName;
}
