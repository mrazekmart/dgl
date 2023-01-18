using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] private float m_maxPlayerHp = 100;
    private GameObject m_playerHP;
    private RectTransform m_rectTransform;
    private GameObject m_weaponUpgradeUI;

    private float m_rawImageWidth;
    private float m_rawImageX;

    public float m_currentPlayerHp;

    private List<GameObject> m_pickableGameObjectsInRange;

    private bool m_isWeaponUpgradeMachine = false;
    private bool m_isWeaponUpgradeMachineUIActive = false;

    private bool m_isBodyUpgradeMachine = false;

    private void Awake()
    {
        m_playerHP = GameObject.Find("PlayerHPBar");
        m_weaponUpgradeUI = GameObject.Find("WeaponUpgradePanel");

        m_rectTransform = m_playerHP.GetComponent<RectTransform>();
        m_pickableGameObjectsInRange = new List<GameObject>();

        m_currentPlayerHp = m_maxPlayerHp;
        m_rawImageWidth = m_rectTransform.sizeDelta.x;
        m_rawImageX = m_rectTransform.anchoredPosition.x;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // if there is a item in range and E is presed, pick the Item
            if (m_pickableGameObjectsInRange.Count > 0) PickUpSingleItem();

            // handling popiping of Upgrade Machine UI
            if (m_isWeaponUpgradeMachine && !m_isWeaponUpgradeMachineUIActive)
            {
                m_weaponUpgradeUI.SetActive(true);
                m_isWeaponUpgradeMachineUIActive = true;
            }else if (m_isWeaponUpgradeMachineUIActive)
            {
                m_weaponUpgradeUI.SetActive(false);
                m_isWeaponUpgradeMachineUIActive = false;
            }
        }
        // hiding Upgrade Machine UI if machine upgrade station is left
        if(m_isWeaponUpgradeMachineUIActive && !m_isWeaponUpgradeMachine)
        {
            m_weaponUpgradeUI.SetActive(false);
            m_isWeaponUpgradeMachineUIActive = false;
        }
    }

    private void PickUpSingleItem()
    {
        if (InventoryHandlerSingleton.Instance.EmptyInventorySlots == 0) return;

        GameObject pickedGameObject = m_pickableGameObjectsInRange[0];
        m_pickableGameObjectsInRange.RemoveAt(0);

        string itemId = pickedGameObject.name[..^7];
        itemId = itemId[InterfaceItems.ITEMS_NAME_PREFIX_LENGTH..];
        if (int.TryParse(itemId, out int result))
        {
            InventoryHandlerSingleton.Instance.UpdateInventory(result);
            Destroy(pickedGameObject);
        }
    }

    public void TakeDamage(float damage)
    {
        m_currentPlayerHp -= damage;
        UpdateHpBar();
    }

    public void UpdateHpBar()
    {
        Vector2 anchXY = m_rectTransform.anchoredPosition;
        Vector2 deltaXY = m_rectTransform.sizeDelta;
        float newWidth = m_rawImageWidth * (m_currentPlayerHp / m_maxPlayerHp);
        deltaXY.x = newWidth;
        anchXY.x = m_rawImageX - (m_rawImageWidth - newWidth) / 2;
        m_rectTransform.anchoredPosition = anchXY;
        m_rectTransform.sizeDelta = deltaXY;

    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "EnemyBullet")
        {
            TakeDamage(collision.gameObject.GetComponent<EnemyBulletScript>().Damage);
            Destroy(collision.gameObject);
        }
        if (collision.transform.tag == "Pickable")
        {
            Debug.Log(collision.name);
            m_pickableGameObjectsInRange.Add(collision.gameObject);
        }
        if (collision.transform.tag == "WeaponUpgradeMachine")
        {
            m_isWeaponUpgradeMachine = true;
        }
        if (collision.transform.tag == "BodyUpgradeMachine")
        {
            m_isBodyUpgradeMachine = true;
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Pickable")
        {
            m_pickableGameObjectsInRange.Remove(collision.gameObject);
        }
        if (collision.transform.tag == "WeaponUpgradeMachine")
        {
            m_isWeaponUpgradeMachine = false;
        }
        if (collision.transform.tag == "BodyUpgradeMachine")
        {
            m_isBodyUpgradeMachine = false;
        }
    }
}
