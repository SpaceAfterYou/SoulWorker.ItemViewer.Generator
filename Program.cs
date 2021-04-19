using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SoulWorkerResearch.SoulCore.IO.Datas.Bin.Table.Entities;
using SoulWorkerResearch.SoulCore.Korean.IO.Datas.Bin;
using SoulWorkerResearch.SoulCore.Types;

namespace SoulWorker.ItemViewer.Generator
{
    public static class Program
    {
        public static Task Main(string[] args)
        {
            var configuration = GetConfiguration();

            var outDir = configuration["OutDir"];
            var gameDir = configuration["GameDir"];

            var data12Path = Path.Join(gameDir, "datas", "data12.v"); 
            
            using BinReader table = new(data12Path, configuration["Data12Pass"]);

            IDictionary<uint, ItemEntity> itemsTable = table.ReadItemTable();
            IDictionary<uint, ItemClassifyEntity> itemClassifyTable = table.ReadItemClassifyTable();
            IDictionary<uint, ItemScriptEntity> itemScriptTable = table.ReadItemScriptTable();

            IEnumerable<ItemClassifyInventoryType> inventoryTypes =
                itemClassifyTable.Values.Select(entity => entity.InventoryType).Distinct().OrderBy(s => s);
            
            IEnumerable<ItemClassifySlotType> slotTypes =
                itemClassifyTable.Values.Select(entity => entity.SlotType).Distinct().OrderBy(s => s);
            
            IEnumerable<byte> gainTypes = itemClassifyTable.Values.Select(s => s.GainType).Distinct().OrderBy(s => s);

            var data = itemsTable.Values
                .Select(i =>
                {
                    ItemClassifyEntity classify = itemClassifyTable[i.ClassifyId];

                    if (itemScriptTable.TryGetValue(i.Id, out ItemScriptEntity? script))
                    {
                        return new
                        {
                            i.Id,
                            classify.SlotType,
                            classify.InventoryType,
                            classify.GainType,
                            script.Name,
                            script.Description,
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
                        Name = "",
                        Description = "",
                        Icon = "",
                        i.Class
                    };
                });

            JsonSerializerOptions options = new()
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            return Task.WhenAll(
                File.WriteAllTextAsync(Path.Join(outDir, "data.json"), JsonSerializer.Serialize(data, options)),
                File.WriteAllTextAsync(Path.Join(outDir, "inventoryTypes.json"),
                    JsonSerializer.Serialize(inventoryTypes, options)),
                File.WriteAllTextAsync(Path.Join(outDir, "slotTypes.json"),
                    JsonSerializer.Serialize(slotTypes, options)),
                File.WriteAllTextAsync(Path.Join(outDir, "gainTypes.json"),
                    JsonSerializer.Serialize(gainTypes, options)));
        }

        private static IConfiguration GetConfiguration() => new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("config/appsettings.json", false, true)
            .Build();
    }
}