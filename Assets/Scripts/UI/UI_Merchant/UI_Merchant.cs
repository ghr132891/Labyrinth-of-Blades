using UnityEngine;

public class UI_Merchant : MonoBehaviour
{
    private Inventory_Player inventory;
    private Inventory_Merchant merchant;

    [SerializeField] private UI_ItemSlotParent inventorySlots;
    [SerializeField] private UI_ItemSlotParent merchantSlots;
    [SerializeField] private UI_EquipSlotParent equipSlots;

    public void SetupMerchantUI(Inventory_Merchant merchant, Inventory_Player inventory)
    {
        this.inventory = inventory;
        this.merchant = merchant;

        this.inventory.OnInventoryChange += UpdateSlotUI;
        this.merchant.OnInventoryChange += UpdateSlotUI;
        UpdateSlotUI();

        UI_MerchantSlot[] merchantSlots = GetComponentsInChildren<UI_MerchantSlot>();

        foreach (var slot in merchantSlots)
            slot.SetupMerchantUI(merchant);
    }

    private void UpdateSlotUI()
    {
        if(inventory == null)
            return;

        merchantSlots.UpdateSlots(merchant.itemList);
        inventorySlots.UpdateSlots(inventory.itemList);
        equipSlots.UpdateEquipmentsSlots(inventory.equipmentList);
    }

}
