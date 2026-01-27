using System;
using System.Text;

[Serializable]
public class Inventory_Item
{
    private string itemID;

    public ItemDataSo itemData;
    public int stackSize = 1;

    public ItemModifier[] Modifiers { get; private set; }
    public ItemEffect_DataSo itemEffect;

    public float buyPrice {  get; private set; }
    public float sellPrice {  get; private set; }

    public Inventory_Item(ItemDataSo itemData)
    {
        this.itemData = itemData;
        itemEffect = itemData.itemEffect;
        buyPrice = itemData.itemPrice;
        sellPrice = itemData.itemPrice * .35f;

        Modifiers = EquipmentData()?.modifiers;
        itemID = itemData.itemName + " - " + Guid.NewGuid();
    }

    public void Addmodifiers(Entity_Stats playerStats)
    {
        foreach (var mod in Modifiers)
        {
            Stat statToModify = playerStats.GetStatByType(mod.statType);
            statToModify.AddModifier(mod.value, itemID);
        }
    }

    public void RemoveModifiers(Entity_Stats playerStats)
    {
        foreach (var mod in Modifiers)
        {
            Stat statTomodify = playerStats.GetStatByType(mod.statType);
            statTomodify.RemoveModifier(itemID);
        }
    }

    public void AddItemEffect(Player player) => itemEffect?.Subsribe(player);
    public void RemoveItemEffect() => itemEffect?.Unsubribe();
    private EquipmentDataSo EquipmentData()
    {
        if (itemData is EquipmentDataSo equipment)
            return equipment;
        else
            return null;
    }

    public bool canAddStack() => stackSize < itemData.maxStackSize;
    public void AddStack() => stackSize++;
    public void RemoveStack() => stackSize--;

    public string GetItemInfo()
    {
        StringBuilder sb = new StringBuilder();

        if (itemData.itemType == ItemType.Material)
        {
            sb.AppendLine("");
            sb.AppendLine("Used for Crafting.");
            sb.AppendLine("");
            sb.AppendLine("");
            return sb.ToString();
        }

        if (itemData.itemType == ItemType.Consumable)
        {
            sb.AppendLine("");
            sb.AppendLine(itemEffect.effectdescription);
            sb.AppendLine("");
            sb.AppendLine("");
            return sb.ToString();
        }


        sb.AppendLine("");
        sb.AppendLine("");

        foreach (var mod in Modifiers)
        {
            string modType = GetStatNameByType(mod.statType);
            string modValue = IsPercentageStat(mod.statType) ? mod.value.ToString() + "%" : mod.value.ToString();
            sb.AppendLine("+ " + modValue + " " + modType);
        }

        if (itemEffect != null)
        {
            sb.AppendLine("");
            sb.AppendLine("Unique effect:");
            sb.AppendLine(itemEffect.effectdescription);
        }

        return sb.ToString();
    }

    private string GetStatNameByType(StatType type)
    {
        switch (type)
        {
            case StatType.MaxHealth: return "Max Health";
            case StatType.HealthRegen: return "Health Regeneration";
            case StatType.Armor: return "Armor";
            case StatType.Evasion: return "Evasion";

            case StatType.Strength: return "Strength";
            case StatType.Agility: return "Agility";
            case StatType.Intelligence: return "Intelligence";
            case StatType.Vitality: return "Vitality";

            case StatType.AttackSpeed: return "Attack Speed";
            case StatType.Damage: return "Damage";
            case StatType.CritChance: return "Critical Chance";
            case StatType.CritPower: return "Critical Power";
            case StatType.ArmorReduction: return "Armor Reduction";

            case StatType.FireDamage: return "Fire Damage";
            case StatType.IceDamage: return "Ice Damage";
            case StatType.LightningDamage: return "Lightning Damage";

            case StatType.IceResistance: return "Ice Resistance";
            case StatType.FireResistance: return "Fire Resistance";
            case StatType.LightningResistance: return "Lightning Resistance";
            default: return "Unknown Stat";
        }

    }
    private bool IsPercentageStat(StatType type)
    {
        switch (type)
        {
            case StatType.CritChance:
            case StatType.CritPower:
            case StatType.ArmorReduction:
            case StatType.IceResistance:
            case StatType.FireResistance:
            case StatType.LightningResistance:
            case StatType.AttackSpeed:
            case StatType.Evasion:
                return true;
            default:
                return false;
        }
    }
}
