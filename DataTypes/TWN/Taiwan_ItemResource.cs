using SoulWorkerResearch.SoulCore.IO.ResTable;
using System.IO;

namespace SoulWorker.ItemViewer.Generator.DataTypes.TWN;

public sealed class Taiwan_ItemResource(BinaryReader reader) : ItemResource(reader), IEntry<Taiwan_ItemResource>
{
    static string IEntry<Taiwan_ItemResource>.TableName => TableName;
}
