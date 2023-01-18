using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScript : MonoBehaviour
{
    [SerializeField] private float m_maxEnemyHP = 1000;
    [SerializeField] private float m_currentEnemyHp;
    [SerializeField] private Transform m_hpBarRemaining;
    [SerializeField] private GameObject m_enemyBullet;
    [SerializeField] private float m_bulletSpeed = 10f;

    private GameObject m_player;
    private GameObject m_mapManagerGO;
    private MapManager m_mapManager;
    private Transform m_enemyBulletContainer;

    private float m_attackSpeed = 1f;
    private float m_specialAttackSpeed = 0.2f;
    private float m_timeToAttack = 0f;
    private float m_specialAttackTimeToAttack = 0f;
    public float m_attackRange = 20f;


    private Coord m_worldCoords;

    private void Awake()
    {
        m_currentEnemyHp = m_maxEnemyHP;
        m_enemyBulletContainer = GameObject.Find("EnemyBulletContainer").GetComponent<Transform>();
        m_player = GameObject.Find("MainPlayer");
        m_mapManagerGO = GameObject.Find("Map");
        m_mapManager = m_mapManagerGO.GetComponent<MapManager>();
        m_worldCoords = new Coord((int)transform.position.x / 40, (int)transform.position.y / 20);
        m_enemyBullet.transform.localScale = new Vector3(3, 3, 1);

    }

    private void Update()
    {
        m_timeToAttack += Time.deltaTime;
        m_specialAttackTimeToAttack += Time.deltaTime;


        if(m_timeToAttack > 1/m_attackSpeed && Vector2.Distance(m_player.transform.position, transform.position) < m_attackRange)
        {
            m_timeToAttack = 0f;
            BasicAttack();
        }


        if (m_specialAttackTimeToAttack > 1/m_specialAttackSpeed)
        {
            m_specialAttackTimeToAttack = 0f;
            AttackSpecialAttack();
        }
        
    }

    private void AttackSpecialAttack()
    {
        int attack = Random.Range(0, 2);

        switch (attack)
        {
            case 0:
                AllAroundShots();
                break;
            case 1:
                StartCoroutine(AllAroundShotsWithDelay());
                break;
            default:
                StartCoroutine(AllAroundShotsWithDelay());
                break;

        }
    }
    private void BasicAttack()
    {
        GameObject newBullet = Instantiate(m_enemyBullet, transform.position, transform.rotation, m_enemyBulletContainer);
        newBullet.SetActive(true);
        newBullet.GetComponent<Rigidbody2D>().velocity = (m_player.transform.position - transform.position).normalized * m_bulletSpeed;
    }

    private void AllAroundShots()
    {
        for(int i = 0; i < 12; i++)
        {
            GameObject newBullet = Instantiate(m_enemyBullet, transform.position, transform.rotation, m_enemyBulletContainer);
            newBullet.SetActive(true);

            float radians = 30 * i * (Mathf.PI / 180);
            Vector2 direction = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
            newBullet.GetComponent<Rigidbody2D>().velocity = direction * m_bulletSpeed;
        }
    }

    private IEnumerator AllAroundShotsWithDelay()
    {
        for (int i = 0; i < 12; i++)
        {
            GameObject newBullet = Instantiate(m_enemyBullet, transform.position, transform.rotation, m_enemyBulletContainer);
            newBullet.SetActive(true);

            float radians = 30 * i * (Mathf.PI / 180);
            Vector2 direction = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
            newBullet.GetComponent<Rigidbody2D>().velocity = direction * m_bulletSpeed;
            yield return new WaitForSeconds(0.1f);
        }
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
