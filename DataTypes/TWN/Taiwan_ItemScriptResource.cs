using SoulWorkerResearch.SoulCore.IO.ResTable;
using System.IO;

namespace SoulWorker.ItemViewer.Generator.DataTypes;

public sealed class Taiwan_ItemScriptResource(BinaryReader reader) : ItemScriptResource(reader), IEntry<Taiwan_ItemScriptResource>
{
    static string IEntry<Taiwan_ItemScriptResource>.TableName => "tb_item_script_ENG";
}
