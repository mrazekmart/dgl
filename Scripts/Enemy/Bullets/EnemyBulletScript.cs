using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletScript : MonoBehaviour
{
    [SerializeField] private float m_damage = 10f;
    [SerializeField] private float m_bulletLifeTime = 8f;

    private GameObject m_player;

    public float Damage { get => m_damage; set => m_damage = value; }

    void Awake()
    {
        m_player = GameObject.Find("MainPlayer");
    }

    private void FixedUpdate()
    {
        m_bulletLifeTime -= Time.deltaTime;
        if (m_bulletLifeTime < 0)
        {
            Destroy(gameObject);
        }
    }
/*    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy" || other.tag == "BossEnemy" || other.tag == "EnemyBullet")
        {
            return;
        }
        if(other.tag == "Player")
        {
            m_player.GetComponent<PlayerScript>().TakeDamage(m_damage);
        }
        Destroy(gameObject);
    }*/
}
