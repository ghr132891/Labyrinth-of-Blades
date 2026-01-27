using TMPro;
using UnityEngine;

public class UI_ItemToolTip : UI_ToolTip
{
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemType;
    [SerializeField] private TextMeshProUGUI itemInfo;

    [SerializeField] private TextMeshProUGUI itemPrice;
    [SerializeField] private Transform merchantInfo;
    [SerializeField] private Transform inventoryInfo;

    public void ShowToolTip(bool show, RectTransform targetRect, Inventory_Item itemToShow, bool buyPrice = false, bool showMerchantInfo = false)
    {
        base.ShowToolTip(show, targetRect);

        merchantInfo.gameObject.SetActive(showMerchantInfo);
        inventoryInfo.gameObject.SetActive(!showMerchantInfo);

        float price = buyPrice ? itemToShow.buyPrice : itemToShow.sellPrice;
        float totalPrice = price * itemToShow.stackSize;

        string fullStackPrice = ($"Price:{price}x{itemToShow.stackSize} - {totalPrice}g.");
        string singleStackPrice = ($"Price:{price}g.");

        itemPrice.text = itemToShow.stackSize > 1 ? fullStackPrice : singleStackPrice;
        itemType.text = itemToShow.itemData.itemType.ToString();
        itemInfo.text = itemToShow.GetItemInfo();

        string color = GetColorByRarity(itemToShow.itemData.itemRarity);
        itemName.text = GetColoredText(color, itemToShow.itemData.itemName);
    }

    private string GetColorByRarity(int rarity)
    {
        if (rarity <= 100) return "white";
        if (rarity <= 300) return "green";
        if (rarity <= 600) return "blue";
        if (rarity <= 800) return "purple";

        return "orange";
    }
}
