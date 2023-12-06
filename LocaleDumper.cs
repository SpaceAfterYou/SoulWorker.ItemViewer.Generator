using System.Collections.Generic;
using System.Linq;
using SoulWorker.ItemViewer.Generator.DataTypes;
using ItemScriptTable2 = System.Collections.Generic.Dictionary<uint, SoulWorker.ItemViewer.Generator.DataTypes.ItemScriptResource>;
using ItemsTable = System.Collections.Generic.Dictionary<uint, SoulWorker.ItemViewer.Generator.DataTypes.ItemResource>;

namespace SoulWorker.ItemViewer.Generator
{
    public class LocaleDumper(ItemScriptTable2 itemScriptTable, ItemsTable itemsTable)
    {
        public Dictionary<uint, LocaleData> Dump() => itemsTable.Values
            .Select(i =>
            {
                if (itemScriptTable.TryGetValue(i.Id, out ItemScriptResource? script))
                {
                    return new
                    {
                        i.Id,
                        script.Name,
                        script.Description
                    };
                }

                return new
                {
                    i.Id,
                    Name = string.Empty,
                    Description = string.Empty,
                };
            })
            .ToDictionary(e => e.Id, e => new LocaleData(e.Name, e.Description));
    }
}

// https://youtu.be/pOjMRP34sBs
