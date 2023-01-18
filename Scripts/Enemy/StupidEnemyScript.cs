using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StupidEnemyScript : MonoBehaviour
{
    [SerializeField] private float m_maxEnemyHP = 100;
    [SerializeField] private float m_currentEnemyHp;
    [SerializeField] private Transform m_hpBarRemaining;
    [SerializeField] private GameObject m_enemyBullet;
    [SerializeField] private float m_bulletSpeed = 10f;

    private GameObject m_player;
    private GameObject m_mapManagerGO;
    private MapManager m_mapManager;
    private Transform m_enemyBulletContainer;

    private float m_attackSpeed = 1f;
    private float m_timeToAttack = 0f;
    public float m_attackRange = 10f;

    private void Awake()
    {
        m_currentEnemyHp = m_maxEnemyHP;
        m_enemyBulletContainer = GameObject.Find("EnemyBulletContainer").GetComponent<Transform>();
        m_player = GameObject.Find("MainPlayer");
        m_mapManagerGO = GameObject.Find("Map");
        m_mapManager = m_mapManagerGO.GetComponent<MapManager>();
    }

    private void Update()
    {
        m_timeToAttack += Time.deltaTime;

        if(m_timeToAttack > m_attackSpeed && Vector2.Distance(m_player.transform.position, transform.position) < m_attackRange)
        {
            m_timeToAttack = 0f;
            Attack();
        }
    }

    private void Attack()
    {
        GameObject newBullet = Instantiate(m_enemyBullet, transform.position, transform.rotation, m_enemyBulletContainer);
        newBullet.SetActive(true);
        newBullet.GetComponent<Rigidbody2D>().velocity = (m_player.transform.position - transform.position).normalized * m_bulletSpeed;
    }

    public void TakeDamage(float damage)
    {
        m_currentEnemyHp -= damage;
        m_hpBarRemaining.localScale = new Vector3(m_currentEnemyHp/m_maxEnemyHP, m_hpBarRemaining.localScale.y, m_hpBarRemaining.localScale.z);
        if (m_currentEnemyHp <= 0)
        {
            m_mapManager.OnEnemyKilled();
            Destroy(gameObject);
            return;
        }
    }
}
