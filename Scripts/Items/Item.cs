using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Item {

    public int m_itemID;
    public int m_itemType;
    public string m_itemName;
    public string m_itemDescription;
    public int m_weaponDamage;
    public int m_weaponAttackSpeed;

    public Item(int _itemId, int itemType, string _itemName, string _itemDescription, int _weaponDamage, int _weaponAttackSpeed)
    {
        m_itemID = _itemId;
        m_itemType = itemType;
        m_itemName = _itemName;
        m_itemDescription = _itemDescription;
        m_weaponDamage = _weaponDamage;
        m_weaponAttackSpeed = _weaponAttackSpeed;
    }
}
[System.Serializable]
public class ItemList
{
    public Item[] m_itemList;
}