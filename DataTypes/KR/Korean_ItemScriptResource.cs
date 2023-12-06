using SoulWorkerResearch.SoulCore.IO.ResTable;
using System.IO;

namespace SoulWorker.ItemViewer.Generator.DataTypes;

public sealed class Korean_ItemScriptResource(BinaryReader reader) : ItemScriptResource(reader), IEntry<Korean_ItemScriptResource>
{
    static string IEntry<Korean_ItemScriptResource>.TableName => "tb_item_script";
}
