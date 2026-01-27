using UnityEngine;

public class UI_CraftListButton : MonoBehaviour
{
    [SerializeField] private ItemListDataSo craftData;
    private UI_CraftSlot[] craftSlots;

    public void SetCraftSlots(UI_CraftSlot[] craftSlots ) => this.craftSlots = craftSlots;

    public void UpdateCraftSlots()
    {
        if(craftData == null)
        {
            Debug.Log("You need to assign craft list data.");
            return;
        }

        foreach(var slot in craftSlots)
        {
            slot.gameObject.SetActive(false);
        }

        for (int i = 0; i <craftData.itemList.Length;i++)
        {
            ItemDataSo itemData = craftData.itemList[i];

            craftSlots[i].gameObject.SetActive(true);
            craftSlots[i].SetupButton(itemData);
        }
    }
}
