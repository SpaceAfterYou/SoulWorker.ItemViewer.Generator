using SoulWorkerResearch.SoulCore.IO.ResTable;
using System.IO;

namespace SoulWorker.ItemViewer.Generator.DataTypes.KR;

public sealed class Korean_ItemResource(BinaryReader reader) : ItemResource(reader), IEntry<Korean_ItemResource>
{
    static string IEntry<Korean_ItemResource>.TableName => TableName;
}
