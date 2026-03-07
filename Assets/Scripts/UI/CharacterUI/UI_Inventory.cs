using NUnit.Framework;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Inventory : MonoBehaviour
{
    private Inventory_Player inventory;

    [SerializeField] private UI_ItemSlotParent inventorySlotsParent;
    [SerializeField] private UI_EquipSlotParent equipSlotParent;
    [SerializeField] private TextMeshProUGUI goldText;

   

    private void Awake()
    {
        inventory = FindFirstObjectByType<Inventory_Player>();

        inventory.OnInventoryChange += UpdateUI;

        UpdateUI();
       
    }

    private void OnEnable()
    {
        if (inventory == null)
            return;

        UpdateUI();
    }
    private void UpdateUI()
    {
        inventorySlotsParent.UpdateSlots(inventory.itemList);
        equipSlotParent.UpdateEquipmentsSlots(inventory.equipmentList);
        goldText.text = inventory.gold.ToString() + "g.";
    }

    

    

}
