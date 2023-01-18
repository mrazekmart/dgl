using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftWeaponScript : MonoBehaviour
{
    [SerializeField] private float m_baseFireRate = 1f;
    [SerializeField] private float m_bulletBaseDamage = 5;
    [SerializeField] private GameObject m_leftBullet;

    private float m_bulletCurrentDamage;

    public float m_currentFireRate;

    private FixedJoystick m_rightJoystick;
    private Transform m_leftGunHead;
    private Transform m_leftBulletContainer;

    private float m_timeElapsed = 0f;

    private bool m_canShoot = true;



    private void Awake()
    {
        m_currentFireRate = m_baseFireRate;
        m_bulletCurrentDamage = m_bulletBaseDamage;

        m_rightJoystick = GameObject.Find("RightJoystickFixed").GetComponent<FixedJoystick>();
        m_leftGunHead = GameObject.Find("LeftWeaponHead").GetComponent<Transform>();
        m_leftBulletContainer = GameObject.Find("LeftBulletContainer").GetComponent<Transform>();
    }
    public void Update()
    {
        m_timeElapsed += Time.deltaTime;
        if(!m_canShoot && m_timeElapsed > 1/m_currentFireRate)
        {
            m_canShoot = true;
        }
        if(m_canShoot && Mathf.Abs(m_rightJoystick.Horizontal) + Mathf.Abs(m_rightJoystick.Vertical) > 0.95f)
        {
            CreateBullet();
            m_canShoot = false;
            m_timeElapsed = 0f;
        }
    }

    private void CreateBullet()
    {
        if (m_leftGunHead == null)
        {
            return;
        }
        GameObject newBullet = Instantiate(m_leftBullet, m_leftGunHead.position, m_leftGunHead.rotation, m_leftBulletContainer);
        newBullet.GetComponent<LeftBulletScript>().m_currentDamage = m_bulletCurrentDamage;
        newBullet.SetActive(true);
    }
    public void OnWeaponUpgrade(InventoryItem[] items)
    {
        m_currentFireRate = m_baseFireRate;
        m_bulletCurrentDamage = m_bulletBaseDamage;
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null) continue;
            m_bulletCurrentDamage += items[i].m_item.m_weaponDamage;
            m_currentFireRate += items[i].m_item.m_weaponAttackSpeed;
        }
    }
}
