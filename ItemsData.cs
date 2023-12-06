using SoulWorkerResearch.SoulCore.Defines;

namespace SoulWorker.ItemViewer.Generator
{
    public readonly record struct ItemsData(uint Id, ItemClassifySlotType SlotType, ItemClassifyInventoryType InventoryType, byte GainType, string Icon, Character Class);
}

// https://youtu.be/pOjMRP34sBs
