using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SoulWorker.ItemViewer.Generator.DataTypes;
using SoulWorker.ItemViewer.Generator.DataTypes.KR;
using SoulWorkerResearch.SoulCore.Defines;
using SoulWorkerResearch.SoulCore.IO;
using SoulWorkerResearch.SoulCore.IO.ResTable;

using ItemClassifyTable = System.Collections.Generic.Dictionary<uint, SoulWorker.ItemViewer.Generator.DataTypes.ItemClassifyResource>;
using ItemScriptTable2 = System.Collections.Generic.Dictionary<uint, SoulWorker.ItemViewer.Generator.DataTypes.ItemScriptResource>;
using KoreanItemScriptTable = System.Collections.Generic.Dictionary<uint, SoulWorker.ItemViewer.Generator.DataTypes.Korean_ItemScriptResource>;
using ItemsTable = System.Collections.Generic.Dictionary<uint, SoulWorker.ItemViewer.Generator.DataTypes.ItemResource>;

namespace SoulWorker.ItemViewer.Generator
{
    public readonly record struct LocaleData(string Name, string Description);

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

    public readonly record struct ItemsData(uint Id, ItemClassifySlotType SlotType, ItemClassifyInventoryType InventoryType, byte GainType, string Icon, Character Class);

    public class ItemsDumper(ItemsTable itemsTable, ItemClassifyTable itemClassifyTable, KoreanItemScriptTable itemScriptTable)
    {
        public IEnumerable<ItemsData> Dump() => itemsTable.Values
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
            .Select(e => new ItemsData(e.Id, e.SlotType, e.InventoryType, e.GainType, e.Icon, e.Class));
    }

    public static class Program
    {
        public static async Task Main()
        {
            var configuration = GetConfiguration();

            var kr = new
            {
                GameDir = configuration["Kr:GameDir"],
                DataPass = configuration["Kr:DataPass"],
                DataPath = configuration["Kr:DataPath"],
            };

            var en = new
            {
                GameDir = configuration["En:GameDir"],
                DataPass = configuration["En:DataPass"],
                DataPath = configuration["En:DataPath"],
            };

            var frontDir = configuration["FrontDir"];


            using var krZip = EncryptedZipFile.Create(Path.Join(kr.GameDir, "datas", kr.DataPath), kr.DataPass);
            using var enZip = EncryptedZipFile.Create(Path.Join(en.GameDir, "datas", en.DataPath), en.DataPass);

            var krReader = new ArchiveReader(krZip);
            var enReader = new ArchiveReader(enZip);

            var itemClassifyTable = await krReader.ReadEntriesAsync<ItemClassifyResource>(default).ToDictionaryAsync(e => e.Id, e => e);
            var itemsTable = await krReader.ReadEntriesAsync<Korean_ItemResource>(default).ToDictionaryAsync(e => e.Id, e => e);
            
            var enItemScriptTable = await enReader.ReadEntriesAsync<Taiwan_ItemScriptResource>(default).ToDictionaryAsync(e => e.Id, e => e);
            var krItemScriptTable = await krReader.ReadEntriesAsync<Korean_ItemScriptResource>(default).ToDictionaryAsync(e => e.Id, e => e);

            var inventoryTypes = itemClassifyTable.Values.Select(entity => entity.InventoryType).Distinct().OrderBy(s => s);
            var slotTypes = itemClassifyTable.Values.Select(entity => entity.SlotType).Distinct().OrderBy(s => s);
            var gainTypes = itemClassifyTable.Values.Select(s => s.GainType).Distinct().OrderBy(s => s);

            var krLocaleDumper = new LocaleDumper(krItemScriptTable.ToDictionary(e => e.Key, e => (ItemScriptResource)e.Value), itemsTable.ToDictionary(e => e.Key, e => (ItemResource)e.Value));
            var enLocaleDumper = new LocaleDumper(enItemScriptTable.ToDictionary(e => e.Key, e => (ItemScriptResource)e.Value), itemsTable.ToDictionary(e => e.Key, e => (ItemResource)e.Value));
            
            var itemsDumper = new ItemsDumper(itemsTable.ToDictionary(e => e.Key, e => (ItemResource)e.Value), itemClassifyTable, krItemScriptTable);
            
            JsonSerializerOptions options = new()
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            var localeDir = Path.Join(frontDir, "src", "locales", "messages");

            await Task.WhenAll(
                File.WriteAllTextAsync(Path.Join(localeDir, "kr.json"), JsonSerializer.Serialize(krLocaleDumper.Dump(), options)),
                File.WriteAllTextAsync(Path.Join(localeDir, "en.json"), JsonSerializer.Serialize(enLocaleDumper.Dump(), options)),

                File.WriteAllTextAsync(Path.Join(frontDir, "public", "data.json"), JsonSerializer.Serialize(itemsDumper.Dump(), options)),
                File.WriteAllTextAsync(Path.Join(frontDir, "public", "inventoryTypes.json"), JsonSerializer.Serialize(inventoryTypes, options)),
                File.WriteAllTextAsync(Path.Join(frontDir, "public", "slotTypes.json"), JsonSerializer.Serialize(slotTypes, options)),
                File.WriteAllTextAsync(Path.Join(frontDir, "public", "gainTypes.json"), JsonSerializer.Serialize(gainTypes, options)));
        }

        private static IConfiguration GetConfiguration() => new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("config/appsettings.json", false, true)
            .Build();
    }
}

// https://youtu.be/pOjMRP34sBs
