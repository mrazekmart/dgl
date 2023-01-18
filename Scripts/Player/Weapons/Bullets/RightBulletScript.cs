using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightBulletScript : MonoBehaviour
{

    [SerializeField] private float m_bulletSpeed = 600f;
    [SerializeField] private float m_bulletLifeTime = 10f;

    private Transform m_rightGunHead;

    private float m_sizeX = 0.2f;
    private float m_sizeY = 0.2f;


    void Awake()
    {
        m_rightGunHead = GameObject.Find("RightWeaponHead").GetComponent<Transform>();

        GetComponent<Rigidbody2D>().AddForce(m_rightGunHead.right * m_bulletSpeed, ForceMode2D.Force);
        transform.localScale = new Vector2(m_sizeX, m_sizeY);
        GetComponent<SpriteRenderer>().color = Color.blue;
    }

    private void FixedUpdate()
    {
        m_bulletLifeTime -= Time.deltaTime;
        if (m_bulletLifeTime < 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Weapon")
        {
            return;
        }
    }
}
