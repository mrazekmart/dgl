using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryHandlerSingleton : MonoBehaviour
{
    [SerializeField] private LeftWeaponScript m_leftWeaponScript;
    private GameObject m_weaponUpgradePanel;

    public TextAsset m_jsonItemList;
    public ItemList m_itemList = new ItemList();

    private Image m_buttonImage_1 = null;
    private Image m_buttonImage_2 = null;
    private Image m_buttonImage_3 = null;
    private Image m_buttonImage_4 = null;
    private Image m_buttonImage_5 = null;
    private Image m_buttonImage_6 = null;
    private Image m_buttonImage_7 = null;
    private Image m_buttonImage_8 = null;

    private Image m_leftWeaponImage_1 = null;
    private Image m_leftWeaponImage_2 = null;
    private Image m_leftWeaponImage_3 = null;
    private Image m_leftWeaponImage_4 = null;
    private Image m_leftWeaponImage_5 = null;

    private Image[] m_imageList;
    private Image[] m_leftWeaponImageList;

    private InventoryItem[] m_inventorySlotsItemID;

    private InventoryItem[] m_leftWeaponSlotsItems;

    private int m_emptyInventorySlots = 8;

    private bool m_isItemGrabbed = false;
    private int m_itemGrabbedSlotIndex = 0;

    public static InventoryHandlerSingleton Instance
    {
        get; private set;
    }
    public int EmptyInventorySlots { get => m_emptyInventorySlots; set => m_emptyInventorySlots = value; }
    public InventoryItem[] LeftWeaponSlotsItems { get => m_leftWeaponSlotsItems; set => m_leftWeaponSlotsItems = value; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        InitializeUI();
        m_itemList = JsonUtility.FromJson<ItemList>(m_jsonItemList.text);

    }

    public void UpdateInventory(int pickedItem)
    {

        for (int i = 0; i < m_inventorySlotsItemID.Length; i++)
        {
            if (m_inventorySlotsItemID[i] == null)
            {
                string gameObjectName = InterfaceItems.ITEMS_NAME_PREFIX + pickedItem + InterfaceItems.ITEMS_NAME_CLONE_PREFIX;
                Sprite itemSprite = GameObject.Find(gameObjectName).GetComponent<SpriteRenderer>().sprite;
                m_imageList[i].sprite = itemSprite;

                Item newItem = m_itemList.m_itemList[pickedItem];
                m_inventorySlotsItemID[i] = new InventoryItem(pickedItem, itemSprite, newItem);
                EmptyInventorySlots--;
                break;
            }
        }
    }

    public void ItemPickedFromInventory(int inventoryIndex)
    {
        m_isItemGrabbed = true;
        m_itemGrabbedSlotIndex = inventoryIndex;
    }
    public void ItemPlacedToSlot(int slotIndex)
    {
        if (m_isItemGrabbed)
        {
            //place item into weapon slots
            Sprite spr = m_inventorySlotsItemID[m_itemGrabbedSlotIndex].m_sprite;
            m_leftWeaponImageList[slotIndex].sprite = spr;
            LeftWeaponSlotsItems[slotIndex] = new InventoryItem(m_inventorySlotsItemID[m_itemGrabbedSlotIndex].m_id, spr, m_inventorySlotsItemID[m_itemGrabbedSlotIndex].m_item);
            m_isItemGrabbed = false;
            
            //upgrade weapon bullet
            m_leftWeaponScript.OnWeaponUpgrade(LeftWeaponSlotsItems);

            //remove from inventory
            m_inventorySlotsItemID[m_itemGrabbedSlotIndex] = null;
            m_imageList[m_itemGrabbedSlotIndex].sprite = null;
        }
    }

    public void InitializeUI()
    {
        m_buttonImage_1 = GameObject.Find("InventoryIcon_1").GetComponent<Image>();
        m_buttonImage_2 = GameObject.Find("InventoryIcon_2").GetComponent<Image>();
        m_buttonImage_3 = GameObject.Find("InventoryIcon_3").GetComponent<Image>();
        m_buttonImage_4 = GameObject.Find("InventoryIcon_4").GetComponent<Image>();
        m_buttonImage_5 = GameObject.Find("InventoryIcon_5").GetComponent<Image>();
        m_buttonImage_6 = GameObject.Find("InventoryIcon_6").GetComponent<Image>();
        m_buttonImage_7 = GameObject.Find("InventoryIcon_7").GetComponent<Image>();
        m_buttonImage_8 = GameObject.Find("InventoryIcon_8").GetComponent<Image>();

        m_imageList = new Image[8] { m_buttonImage_1, m_buttonImage_2, m_buttonImage_3,
            m_buttonImage_4, m_buttonImage_5, m_buttonImage_6, m_buttonImage_7, m_buttonImage_8 };
        m_inventorySlotsItemID = new InventoryItem[8] { null, null, null, null, null, null, null, null };

        m_leftWeaponImage_1 = GameObject.Find("LeftWeaponImage_1").GetComponent<Image>();
        m_leftWeaponImage_2 = GameObject.Find("LeftWeaponImage_2").GetComponent<Image>();
        m_leftWeaponImage_3 = GameObject.Find("LeftWeaponImage_3").GetComponent<Image>();
        m_leftWeaponImage_4 = GameObject.Find("LeftWeaponImage_4").GetComponent<Image>();
        m_leftWeaponImage_5 = GameObject.Find("LeftWeaponImage_5").GetComponent<Image>();

        m_leftWeaponImageList = new Image[5] { m_leftWeaponImage_1, m_leftWeaponImage_2, m_leftWeaponImage_3,
            m_leftWeaponImage_4, m_leftWeaponImage_5};
        LeftWeaponSlotsItems = new InventoryItem[5] { null, null, null, null, null };

        m_weaponUpgradePanel = GameObject.Find("WeaponUpgradePanel");
        m_weaponUpgradePanel.SetActive(false);
    }
}
