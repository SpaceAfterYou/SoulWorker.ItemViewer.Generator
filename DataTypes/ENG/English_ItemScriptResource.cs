using SoulWorkerResearch.SoulCore.IO.ResTable;
using System.IO;

namespace SoulWorker.ItemViewer.Generator.DataTypes.ENG;

public sealed class English_ItemScriptResource(BinaryReader reader) : ItemScriptResource(reader), IEntry<English_ItemScriptResource>
{
    static string IEntry<English_ItemScriptResource>.TableName => "tb_item_script_ENG";
}
