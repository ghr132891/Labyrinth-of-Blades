using System.Runtime.InteropServices;
using UnityEngine;

public class UI_Storage : MonoBehaviour
{
    private Inventory_Storage storage;
    private Inventory_Player inventory;

    [SerializeField] private UI_ItemSlotParent inventoryParent;
    [SerializeField] private UI_ItemSlotParent storageParent;
    [SerializeField] private UI_ItemSlotParent materialStashParent;

    public void SetupStorageUI(Inventory_Storage storage)
    {
        this.storage = storage;
        inventory = storage.playerInventory;

        storage.OnInventoryChange += UpdateUI;
        UpdateUI();

        UI_StorageSlot[] storageSlots = GetComponentsInChildren<UI_StorageSlot>();

        foreach(var slot in storageSlots)
        {
            slot.SetStorage(storage);
        }
    }

    private void OnEnable()
    {
        UpdateUI();
    }
    private void UpdateUI()
    {
        if (storage == null)
            return;

        inventoryParent.UpdateSlots(inventory.itemList);
        storageParent.UpdateSlots(storage.itemList);
        materialStashParent.UpdateSlots(storage.materialStash);
    }
}
