using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CraftSlot : MonoBehaviour
{
    private ItemDataSo itemToCraft;

    [SerializeField] private UI_CraftPreview craftPreview;

    [SerializeField] private Image craftItemIcon;
    [SerializeField] private TextMeshProUGUI craftItemName;

    public void SetupButton(ItemDataSo craftData)
    {
        this.itemToCraft = craftData;
        craftItemIcon.sprite = craftData.itemIcon;
        craftItemName.text = craftData.itemName;


    }
    public void UPdateCraftPreview() => craftPreview.UpdateCraftPreview(itemToCraft);


}
