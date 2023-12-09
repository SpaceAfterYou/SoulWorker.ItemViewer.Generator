using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SoulWorker.ItemViewer.Generator.DataTypes;
using SoulWorker.ItemViewer.Generator.DataTypes.ENG;
using SoulWorker.ItemViewer.Generator.DataTypes.KOR;
using SoulWorker.ItemViewer.Generator.DataTypes.TWN;
using SoulWorkerResearch.SoulCore.IO;
using SoulWorkerResearch.SoulCore.IO.ResTable;

namespace SoulWorker.ItemViewer.Generator
{
    public static class Program
    {
        public static async Task Main()
        {
            var configuration = GetConfiguration();

            var kr = new
            {
                GameDir = configuration["Kor:GameDir"],
                DataPass = configuration["Kor:DataPass"],
                DataPath = configuration["Kor:DataPath"],
            };

            var en = new
            {
                GameDir = configuration["Eng:GameDir"],
                DataPass = configuration["Eng:DataPass"],
                DataPath = configuration["Eng:DataPath"],
            };

            var frontDir = configuration["FrontDir"];


            using var krZip = EncryptedZipFile.Create(Path.Join(kr.GameDir, "datas", kr.DataPath), kr.DataPass);
            using var enZip = EncryptedZipFile.Create(Path.Join(en.GameDir, "datas", en.DataPath), en.DataPass);

            var krReader = new ArchiveReader(krZip);
            var enReader = new ArchiveReader(enZip);

            var itemClassifyTable = await krReader.ReadEntriesAsync<ItemClassifyResource>(default).ToDictionaryAsync(e => e.Id, e => e);
            var itemsTable = await krReader.ReadEntriesAsync<Korean_ItemResource>(default).ToDictionaryAsync(e => e.Id, e => e);
            
            var enItemScriptTable = await enReader.ReadEntriesAsync<English_ItemScriptResource>(default).ToDictionaryAsync(e => e.Id, e => e);
            var krItemScriptTable = await krReader.ReadEntriesAsync<Korean_ItemScriptResource>(default).ToDictionaryAsync(e => e.Id, e => e);
            var twnItemScriptTable = await enReader.ReadEntriesAsync<Taiwan_ItemScriptResource>(default).ToDictionaryAsync(e => e.Id, e => e);

            var inventoryTypes = itemClassifyTable.Values.Select(entity => entity.InventoryType).Distinct().OrderBy(s => s);
            var slotTypes = itemClassifyTable.Values.Select(entity => entity.SlotType).Distinct().OrderBy(s => s);
            var gainTypes = itemClassifyTable.Values.Select(s => s.GainType).Distinct().OrderBy(s => s);

            var krLocaleDumper = new LocaleDumper(krItemScriptTable.ToDictionary(e => e.Key, e => (ItemScriptResource)e.Value), itemsTable.ToDictionary(e => e.Key, e => (ItemResource)e.Value));
            var enLocaleDumper = new LocaleDumper(enItemScriptTable.ToDictionary(e => e.Key, e => (ItemScriptResource)e.Value), itemsTable.ToDictionary(e => e.Key, e => (ItemResource)e.Value));
            var twnLocaleDumper = new LocaleDumper(twnItemScriptTable.ToDictionary(e => e.Key, e => (ItemScriptResource)e.Value), itemsTable.ToDictionary(e => e.Key, e => (ItemResource)e.Value));
            
            var itemsDumper = new ItemsDumper(itemsTable.ToDictionary(e => e.Key, e => (ItemResource)e.Value), itemClassifyTable, krItemScriptTable);
            
            JsonSerializerOptions options = new()
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            var localeDir = Path.Join(frontDir, "locale");
            var items = itemsDumper.Dump().ToArray();

            await Task.WhenAll(
                File.WriteAllTextAsync(Path.Join(localeDir, "kor", "items.json"), JsonSerializer.Serialize(krLocaleDumper.Dump(), options)),
                File.WriteAllTextAsync(Path.Join(localeDir, "eng", "items.json"), JsonSerializer.Serialize(enLocaleDumper.Dump(), options)),
                File.WriteAllTextAsync(Path.Join(localeDir, "twn", "items.json"), JsonSerializer.Serialize(twnLocaleDumper.Dump(), options)),

                Task.WhenAll(items.Select((e, i) => File.WriteAllTextAsync(Path.Join(frontDir, "public", "items", $"part{i}.json"), JsonSerializer.Serialize(e, options)))),
                
                File.WriteAllTextAsync(Path.Join(frontDir, "stores", "items", "__generated__", "items-count.ts"), $"export const parts = {items.Length}"),
                
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
// https://youtu.be/UjZqcDYbvAE