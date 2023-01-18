using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem
{
    public int m_id;
    public Sprite m_sprite;
    public Item m_item;
    public InventoryItem(int _id, Sprite _sprite, Item _item)
    {
        m_id = _id;
        m_sprite = _sprite;
        m_item = _item;
    }
}
