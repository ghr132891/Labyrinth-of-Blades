using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InGame : MonoBehaviour
{
    private Player player;
    private Inventory_Player inventory;
    private UI_SkillSlot[] skillSlots;


    [SerializeField] private RectTransform healthRect;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;
    private float maxWidth;

    [Header("Quick Item Slots")]
    [SerializeField] private float yOffsetQuickItemParent = 150;
    [SerializeField] private Transform quickItemOptionParent;
    private UI_QuickItemOption[] quickItemOptions;
    private UI_QuickItemSlot[] quickItemSlots;


    private void Update()
    {
        Debug.Log(player.health.GetCurrentHealth());
    }
    private void Awake()
    {
        maxWidth = healthRect.sizeDelta.x;

        skillSlots = GetComponentsInChildren<UI_SkillSlot>(true);
    }
    private void Start()
    {
        quickItemSlots = GetComponentsInChildren<UI_QuickItemSlot>();

        player = FindFirstObjectByType<Player>();

        player.health.OnHealthUpdate += UpdateHealthBar;
        // 【核心修复】：不要干等事件触发，初始化时主动拉取一次最新血量！
        UpdateHealthBar();

        inventory = player.inventory;
        inventory.OnInventoryChange += UpdateQuickSlotUI;
        inventory.OnQuickItemUsed += PlayQuickSlotFreeback;
    }

    public void PlayQuickSlotFreeback(int slotNumber)=> quickItemSlots[slotNumber].SimulateButtonFreeback();

    public void UpdateQuickSlotUI()
    {
        Inventory_Item[] quickItems = inventory.quickItems;

        //for (int i = 0; i < quickItems.Length; i++)
            //quickItemSlots[i].UpdateQuickSlotUI(quickItems[i]);

;    }

    public void OpenQuickItemOPtions(UI_QuickItemSlot quickItemSlot, RectTransform targetRect)
    {
        if (quickItemOptions == null)
            quickItemOptions = quickItemOptionParent.GetComponentsInChildren<UI_QuickItemOption>(true);

        List<Inventory_Item> consumables = inventory.itemList.FindAll(item => item.itemData.itemType == ItemType.Consumable);

        for (int i = 0; i < quickItemOptions.Length; i++)
        {
            if (i < consumables.Count)
            {
                quickItemOptions[i].gameObject.SetActive(true);
                quickItemOptions[i].SetupOption(quickItemSlot, consumables[i]);

            }
            else
                quickItemOptions[i].gameObject.SetActive(false);

        }

        quickItemOptionParent.position = targetRect.position + Vector3.up * yOffsetQuickItemParent;
    }

    public void HideQuickItemOptions() => quickItemOptionParent.position = new Vector3(0, 9999);

    public UI_SkillSlot GetSkillSlot(SkillType skillType)
    {
        if (skillSlots == null)
            skillSlots = GetComponentsInChildren<UI_SkillSlot>(true);

        foreach (var slot in skillSlots)
        {
            if (slot.skillType == skillType)
            {
                slot.gameObject.SetActive(true);
                return slot;
            }
        }

        return null;
    }

    private void UpdateHealthBar()
    {
        float currentHealth = Mathf.RoundToInt(player.health.GetCurrentHealth());
        currentHealth = Mathf.Max(0, currentHealth); // 【核心修复】：限制血量不低于 0
        float maxHelath = player.stats.GetMaxHealth();
        float baseHealthLength = 200;

        float sizePercent = currentHealth / baseHealthLength;
        float targetWidth = maxWidth * sizePercent;
        float sizeDifference = Mathf.Abs(maxHelath - healthRect.sizeDelta.x);

        if (sizeDifference > .1f)
            healthRect.sizeDelta = new Vector2(maxHelath, healthRect.sizeDelta.y);

        healthText.text = currentHealth + "/" + maxHelath;
        healthSlider.value = Mathf.Max(0, player.health.GetHealthPercent());
    }

}
