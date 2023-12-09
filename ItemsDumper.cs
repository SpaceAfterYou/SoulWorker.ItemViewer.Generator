using System;
using System.Collections.Generic;
using System.Linq;

using ItemClassifyTable = System.Collections.Generic.Dictionary<uint, SoulWorker.ItemViewer.Generator.DataTypes.ItemClassifyResource>;
using KoreanItemScriptTable = System.Collections.Generic.Dictionary<uint, SoulWorker.ItemViewer.Generator.DataTypes.KOR.Korean_ItemScriptResource>;
using ItemsTable = System.Collections.Generic.Dictionary<uint, SoulWorker.ItemViewer.Generator.DataTypes.ItemResource>;
using SoulWorker.ItemViewer.Generator.DataTypes.KOR;

namespace SoulWorker.ItemViewer.Generator
{
    public class ItemsDumper(ItemsTable itemsTable, ItemClassifyTable itemClassifyTable, KoreanItemScriptTable itemScriptTable)
    {
        public IEnumerable<ItemsData[]> Dump() => itemsTable.Values
            .Select(i =>
            {
                var classify = itemClassifyTable.GetValueOrDefault(i.ClassifyId);
                ArgumentNullException.ThrowIfNull(classify);

                if (itemScriptTable.TryGetValue(i.Id, out Korean_ItemScriptResource? script))
                {
                    return new
                    {
                        i.Id,
                        classify.SlotType,
                        classify.InventoryType,
                        classify.GainType,
                        Icon = $"GUI/{script.Icon}.png",
                        i.Class
                    };
                }

                return new
                {
                    i.Id,
                    classify.SlotType,
                    classify.InventoryType,
                    classify.GainType,
                    Icon = string.Empty,
                    i.Class
                };
            })
            .Select(e => new ItemsData(e.Id, e.SlotType, e.InventoryType, e.GainType, e.Icon, e.Class))
            .Chunk(100);
    }
}

// https://youtu.be/pOjMRP34sBs
