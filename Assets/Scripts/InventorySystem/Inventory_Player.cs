using System.Collections.Generic;
using UnityEngine;

public class Inventory_Player : Inventory_Base
{
    public float gold = 10000;

    private Player player;
    public List<Inventory_EquipmentSlot> equipmentList;
    public Inventory_Storage storage { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<Player>();
        storage = FindFirstObjectByType<Inventory_Storage>();
    }

    public void TryEquipItem(Inventory_Item item)
    {
        var inventoryITem = FindItem(item);
        var matchingSlots = equipmentList.FindAll(slot => slot.slotType == item.itemData.itemType);

        foreach (var slot in matchingSlots)
        {
            if (slot.HasItem() == false)
            {
                EquipItem(inventoryITem, slot);
                return;
            }
        }

        var slotToReplace = matchingSlots[0];
        var itemToUnequip = slotToReplace.equipmentItem;

        UnequipItem(itemToUnequip, slotToReplace != null);
        EquipItem(inventoryITem, slotToReplace);

    }
    private void EquipItem(Inventory_Item itemToEquip, Inventory_EquipmentSlot slot)
    {
        float savedHealthPercent = player.health.GetHealthPercent();

        slot.equipmentItem = itemToEquip;
        slot.equipmentItem.Addmodifiers(player.stats);
        slot.equipmentItem.AddItemEffect(player);


        player.health.SetHealthToPercent(savedHealthPercent);
        RemoveOneItem(itemToEquip);
    }

    public void UnequipItem(Inventory_Item itemToUnequip, bool replacingItem = false)
    {
        if (canAddItem(itemToUnequip) == false && replacingItem == false)
        {
            Debug.Log("No, Space.");
            return;
        }

        float savedHealthPercent = player.health.GetHealthPercent();

        var slotToUnequip = equipmentList.Find(slot => slot.equipmentItem == itemToUnequip);

        if (slotToUnequip != null)
            slotToUnequip.equipmentItem = null;

        itemToUnequip.RemoveModifiers(player.stats);
        itemToUnequip.RemoveItemEffect();



        player.health.SetHealthToPercent(savedHealthPercent);
        AddItem(itemToUnequip);
    }
}
