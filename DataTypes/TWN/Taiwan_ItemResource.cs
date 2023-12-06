using SoulWorkerResearch.SoulCore.IO.ResTable;
using System.IO;

namespace SoulWorker.ItemViewer.Generator.DataTypes.GLB;

public sealed class Taiwan_ItemResource(BinaryReader reader) : ItemResource(reader), IEntry<Taiwan_ItemResource>
{
    static string IEntry<Taiwan_ItemResource>.TableName => TableName;
}
