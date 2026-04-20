
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Entity_DropManager : MonoBehaviour
{
    [SerializeField] private GameObject itemDropPrefab;
    [SerializeField] private ItemListDataSo dropData;

    [Header("Drop restrctions")]
    [SerializeField] private float maxRarityAmount = 1200;
    [SerializeField] private int maxItemsToDrop = 3;

   

    public virtual void DropItems()
    {
        if(dropData == null)
        {
            Debug.Log("You need to assign drop data on entity" + gameObject.name);
            return;
        }

        List<ItemDataSo> itemsToDrop = RollDrops();
        int amountToDrop = Mathf.Min(itemsToDrop.Count, maxItemsToDrop);

        for(int i = 0;i < amountToDrop; i++)
        {
            CreateItemDrop(itemsToDrop[i]);
        }
    }

    protected void CreateItemDrop(ItemDataSo itemToDrop)
    {
        GameObject newItem = Instantiate(itemDropPrefab, transform.position, Quaternion.identity);
        newItem.GetComponent<Object_ItemPickUp>().SetupItem(itemToDrop);

    }
    public List<ItemDataSo> RollDrops()
    {
        List<ItemDataSo> possibleDrops = new List<ItemDataSo>();
        List<ItemDataSo> finalDrops = new List<ItemDataSo>();
        float maxRarityAmount = this.maxRarityAmount;

        foreach(var item in dropData.itemList)
        {
            float dropChance = item.GetDropChance();

            if(Random.Range(0,100) <= dropChance)
                possibleDrops.Add(item);
        }

        // 按照稀有度降序排列
        possibleDrops = possibleDrops.OrderByDescending(item => item.itemRarity).ToList();

        foreach(var item in possibleDrops)
        {
            if(maxRarityAmount > item.itemRarity)
            {
                finalDrops.Add(item);
                maxRarityAmount -= item.itemRarity;
            }
        }

        return finalDrops;
    }
}
