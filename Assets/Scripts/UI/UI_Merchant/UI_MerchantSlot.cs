using UnityEngine;
using UnityEngine.EventSystems;

public class UI_MerchantSlot : UI_ItemSlot
{
    private Inventory_Merchant merchant;
    public enum MerchantSlotType { MerchantSlot, PlayerSlot }
    public MerchantSlotType slotType;

    public override void OnPointerDown(PointerEventData eventData)
    {
        if(itemInSlot == null)
            return;

        bool rightButton = eventData.button == PointerEventData.InputButton.Right;
        bool leftButton = eventData.button == PointerEventData.InputButton.Left;

        if(slotType == MerchantSlotType.PlayerSlot)
        {
            if (rightButton)
            {
                bool sellFullStack = Input.GetKey(KeyCode.LeftControl);
                merchant.TrySellItem(itemInSlot,sellFullStack);
            }
            else if (leftButton)
            {
                base.OnPointerDown(eventData);
            }
        }
        else if(slotType == MerchantSlotType.MerchantSlot)
        {
            if (leftButton)
                return;

            bool buyFullStack = Input.GetKey(KeyCode.LeftControl);
            merchant.TryBuyItem(itemInSlot,buyFullStack); 
        }

        ui.itemToolTip.ShowToolTip(false, null);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if(itemInSlot == null)
            return;

        if(slotType == MerchantSlotType.MerchantSlot)
            ui.itemToolTip.ShowToolTip(true,rect,itemInSlot,true,true);
        else
            ui.itemToolTip.ShowToolTip(true,rect,itemInSlot,false,true);


    }
    public void SetupMerchantUI(Inventory_Merchant merchant ) => this.merchant = merchant;


}
