using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class UI_EquipSlotParent : MonoBehaviour
{
    private UI_EquipSlot[] equipSlots;

    public void UpdateEquipmentsSlots(List<Inventory_EquipmentSlot> equipList)
    {
        if(equipSlots == null)
            equipSlots = GetComponentsInChildren<UI_EquipSlot>();

        for (int i = 0; i < equipSlots.Length; i++)
        {
            var playerEquipSlot = equipList[i];

            if (playerEquipSlot.HasItem() == false)
                equipSlots[i].UpdateSlot(null);
            else
                equipSlots[i].UpdateSlot(playerEquipSlot.equipmentItem);

        }
    }

}
