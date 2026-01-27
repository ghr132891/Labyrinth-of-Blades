using System;
using UnityEngine;

[Serializable]
public class Inventory_EquipmentSlot 
{
    public ItemType slotType;
    public Inventory_Item equipmentItem;

    public Inventory_Item GetEquipedItem() => equipmentItem;
    public bool HasItem() => equipmentItem != null && equipmentItem.itemData != null;

}
