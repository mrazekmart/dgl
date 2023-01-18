using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightWeaponScript : MonoBehaviour
{
    [SerializeField] private float m_fireRate = 2f;

    private FixedJoystick m_rightJoystick;
    private Transform m_rightGunHead;
    [SerializeField] private GameObject m_rightBullet;
    private Transform m_rightBulletContainer;

    private float m_timeElapsed = 0f;

    private bool m_canShoot = true;


    private void Awake()
    {
        m_rightJoystick = GameObject.Find("RightJoystickFixed").GetComponent<FixedJoystick>();
        m_rightGunHead = GameObject.Find("RightWeaponHead").GetComponent<Transform>();
        //m_rightBullet = GameObject.Find("RightBullet");
        m_rightBulletContainer = GameObject.Find("RightBulletContainer").GetComponent<Transform>();
    }
    public void Update()
    {
        m_timeElapsed += Time.deltaTime;
        if (!m_canShoot && m_timeElapsed > m_fireRate)
        {
            m_canShoot = true;
        }
        if (m_canShoot && Mathf.Abs(m_rightJoystick.Horizontal) + Mathf.Abs(m_rightJoystick.Vertical) > 0.95f)
        {
            CreateBullet();
            m_canShoot = false;
            m_timeElapsed = 0f;
        }
    }

    private void CreateBullet()
    {
        if (m_rightGunHead == null)
        {
            return;
        }
        GameObject newBullet = Instantiate(m_rightBullet, m_rightGunHead.position, m_rightGunHead.rotation, m_rightBulletContainer);
        newBullet.SetActive(true);
    }
}
