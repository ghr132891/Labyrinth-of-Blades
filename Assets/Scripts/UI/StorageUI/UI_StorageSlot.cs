using UnityEngine;
using UnityEngine.EventSystems;

public class UI_StorageSlot : UI_ItemSlot
{
    private Inventory_Storage storage;
    public enum StorageSlotType { StorageSlot, PlayerInventorySlot }
    public StorageSlotType slotType;
    public void SetStorage(Inventory_Storage storage) => this.storage = storage;

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (itemInSlot == null)
            return;

        bool transferFullStack = Input.GetKey(KeyCode.LeftControl);

        if (slotType == StorageSlotType.StorageSlot)
            storage.FromStorageToPlayer(itemInSlot, transferFullStack);

        if (slotType == StorageSlotType.PlayerInventorySlot)
            storage.FromPlayerToStorage(itemInSlot, transferFullStack);

        ui.itemToolTip.ShowToolTip(false, null);
    }


}
