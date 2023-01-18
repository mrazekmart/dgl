using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftBulletScript : MonoBehaviour
{

    [SerializeField] private float m_bulletSpeed = 600f;
    [SerializeField] private float m_bulletLifeTime = 10f;

    private Transform m_leftGunHead;

    public float m_sizeX = 0.2f;
    public float m_sizeY = 0.2f;

    public float m_currentDamage = 0;


    private bool m_bulletsPiercing = false;


    void Awake()
    {
        m_leftGunHead = GameObject.Find("LeftWeaponHead").GetComponent<Transform>();

        GetComponent<Rigidbody2D>().AddForce(m_leftGunHead.right * m_bulletSpeed, ForceMode2D.Force);
        transform.localScale = new Vector2(m_sizeX, m_sizeY);
        GetComponent<SpriteRenderer>().color = Color.red;
    }

    private void FixedUpdate()
    {
        m_bulletLifeTime -= Time.deltaTime;
        if (m_bulletLifeTime < 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Weapon")
        {
            return;
        }

        if(other.tag == "Enemy")
        {
            other.GetComponent<StupidEnemyScript>().TakeDamage(m_currentDamage);
            if (!m_bulletsPiercing)
            {
                Destroy(gameObject);
            }
        }
        if (other.tag == "BossEnemy")
        {
            other.GetComponent<BossScript>().TakeDamage(m_currentDamage);
            if (!m_bulletsPiercing)
            {
                Destroy(gameObject);
            }
        }
    }

    private bool pulseDown = true;
    private void Pulsating()
    {
        if(m_sizeY < 0.3f)  pulseDown = false; 
        if(m_sizeY > 1f)    pulseDown = true;

        if (pulseDown) m_sizeY -= 0.01f;
        else m_sizeY += 0.01f;

        transform.localScale = new Vector2(m_sizeX, m_sizeY);
    }
}
