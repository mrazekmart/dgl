using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUpgradePanelScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void LeftWeaponSlot1()
    {
        InventoryHandlerSingleton.Instance.ItemPlacedToSlot(0);
    }
    public void LeftWeaponSlot2()
    {
        InventoryHandlerSingleton.Instance.ItemPlacedToSlot(1);
    }
    public void LeftWeaponSlot3()
    {
        InventoryHandlerSingleton.Instance.ItemPlacedToSlot(2);
    }
    public void LeftWeaponSlot4()
    {
        InventoryHandlerSingleton.Instance.ItemPlacedToSlot(3);
    }
    public void LeftWeaponSlot5()
    {
        InventoryHandlerSingleton.Instance.ItemPlacedToSlot(4);
    }
    public void InventorySlot1()
    {
        InventoryHandlerSingleton.Instance.ItemPickedFromInventory(0);
    }
    public void InventorySlot2()
    {
        InventoryHandlerSingleton.Instance.ItemPickedFromInventory(1);
    }
    public void InventorySlot3()
    {
        InventoryHandlerSingleton.Instance.ItemPickedFromInventory(2);
    }
    public void InventorySlot4()
    {
        InventoryHandlerSingleton.Instance.ItemPickedFromInventory(3);
    }
    public void InventorySlot5()
    {
        InventoryHandlerSingleton.Instance.ItemPickedFromInventory(4);
    }
    public void InventorySlot6()
    {
        InventoryHandlerSingleton.Instance.ItemPickedFromInventory(5);
    }
    public void InventorySlot7()
    {
        InventoryHandlerSingleton.Instance.ItemPickedFromInventory(6);
    }
    public void InventorySlot8()
    {
        InventoryHandlerSingleton.Instance.ItemPickedFromInventory(7);
    }
}
