using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Extensions.Configuration;
using SoulWorker.ItemViewer.Generator.DataTypes.KR;
using SoulWorkerResearch.SoulCore.IO;
using SoulWorkerResearch.SoulCore.IO.ResTable;

namespace SoulWorker.ItemViewer.Generator
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var configuration = GetConfiguration();

            var outDir = configuration["OutDir"];
            var gameDir = configuration["GameDir"];
            var password = configuration["Data12Pass"];

            var data12Path = Path.Join(gameDir, "datas", "data12.v");

            var zip = EncryptedZipFile.Create(data12Path, password);
            var reader = new ArchiveReader(zip);

            var itemClassifyTable = await reader.ReadEntriesAsync<ItemClassifyEntity>(default).ToDictionaryAsync(e => e.Id, e => e);
            var itemScriptTable = await reader.ReadEntriesAsync<ItemScriptEntry>(default).ToDictionaryAsync(e => e.Id, e => e);
            var itemsTable = await reader.ReadEntriesAsync<ItemResource>(default).ToDictionaryAsync(e => e.Id, e => e);

            var inventoryTypes = itemClassifyTable.Values.Select(entity => entity.InventoryType).Distinct().OrderBy(s => s);
            var slotTypes = itemClassifyTable.Values.Select(entity => entity.SlotType).Distinct().OrderBy(s => s);
            var gainTypes = itemClassifyTable.Values.Select(s => s.GainType).Distinct().OrderBy(s => s);

            var data = itemsTable.Values
                .Select(i =>
                {
                    ItemClassifyEntity classify = itemClassifyTable[i.ClassifyId];

                    if (itemScriptTable.TryGetValue(i.Id, out ItemScriptEntry? script))
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

            await Task.WhenAll(
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

// https://youtu.be/pOjMRP34sBs
