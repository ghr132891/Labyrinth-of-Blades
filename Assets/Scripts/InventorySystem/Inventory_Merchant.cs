
using System.Collections.Generic;
using UnityEngine;

public class Inventory_Merchant : Inventory_Base
{
    private Inventory_Player inventory;

    [SerializeField] private ItemListDataSo shopData;
    [SerializeField] private int minItemAmount = 4;


    protected override void Awake()
    {
        base.Awake();
        FillShopList();
    }

    public void TryBuyItem(Inventory_Item itemToBuy,bool buyFullStack)
    {
        int amountToBuy = buyFullStack ? itemToBuy.stackSize : 1;
        
        for(int i = 0; i < amountToBuy; i++)
        {
            if(inventory.gold < itemToBuy.buyPrice)
            {
                Debug.Log("No enough money.");
                return;
            }

            if(itemToBuy.itemData.itemType == ItemType.Material)
            {
                inventory.storage.AddMaterialToStash(itemToBuy);
            }
            else
            {
                if (inventory.canAddItem(itemToBuy))
                {
                    var itemToAdd = new Inventory_Item(itemToBuy.itemData);
                    inventory.AddItem(itemToAdd);
                }
            }

            inventory.gold = inventory.gold - itemToBuy.buyPrice;
            RemoveOneItem(itemToBuy);
        }

        TriggerUpdateUI();
    }

    public void TrySellItem(Inventory_Item itemToSell,bool sellFullStack)
    {
        int amountToSell = sellFullStack? itemToSell.stackSize : 1;

        for (int i = 0;i < amountToSell; i++)
        {
            float sellPrice = itemToSell.sellPrice;

            inventory.gold = inventory.gold + sellPrice;
            inventory.RemoveOneItem(itemToSell);
        }

        TriggerUpdateUI();
    }

    public void FillShopList()
    {
        itemList.Clear();
        TriggerUpdateUI();

        List<Inventory_Item> possibleItems = new List<Inventory_Item>();

        foreach(var itemData in shopData.itemList)
        {
            int randomStack = Random.Range(itemData.minStackSizeAtShop,itemData.maxStackSizeAtShop + 1);
            int finalStack = Mathf.Clamp(randomStack,1,itemData.maxStackSizeAtShop);

            Inventory_Item itemToAdd = new Inventory_Item(itemData);
            itemToAdd.stackSize = finalStack;

            possibleItems.Add(itemToAdd);
        }

        int randomItemAmount = Random.Range(minItemAmount,maxInventorySize + 1);
        int finalAmount = Mathf.Clamp(randomItemAmount, 1, possibleItems.Count);

        for (int i = 0;i < finalAmount; i++)
        {
            var randomIndex = Random.Range(0,possibleItems.Count);
            var item = possibleItems[randomIndex];

            if (canAddItem(item))
            {
                possibleItems.Remove(item);
                AddItem(item);

            }
        }

        TriggerUpdateUI();
    }


    public void SetInventory(Inventory_Player inventory) => this.inventory = inventory;


}
