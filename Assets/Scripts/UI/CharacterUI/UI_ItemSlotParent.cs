using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class UI_ItemSlotParent : MonoBehaviour
{
    private UI_ItemSlot[] slots;

    public void UpdateSlots(List<Inventory_Item> itemList)
    {
        if(slots == null)
            slots = GetComponentsInChildren<UI_ItemSlot>();

        for(int i = 0;i<slots.Length; i++)
        {
            if(i < itemList.Count)
            {
                slots[i].UpdateSlot(itemList[i]);
            }
            else
            {
                slots[i].UpdateSlot(null);
            }
        }

    }
}
