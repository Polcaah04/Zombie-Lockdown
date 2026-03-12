using UnityEngine;
using System.Collections;

public class Zombie : MonoBehaviour
{
    [Header (" Basic Stats")]
    private int m_CurrentLife;
    [SerializeField] private float m_Life = 100;

    [SerializeField] private float m_Speed = 2f;

    /*[Header ("Sight")]
    [SerializeField] private float m_ViewDistance = 4f;
    [SerializeField] private int m_MaxViewDistance = 6;*/

    [Header("Attack")]
    [SerializeField] private int m_Damage = 5;
    [SerializeField] private float m_AttackDistance = 1f;
    [SerializeField] private float m_AttackTime = 1f;

    [SerializeField] private LayerMask m_PlayerLayer;
    [SerializeField] private LayerMask m_ObstacleLayer;
    //[SerializeField] private GameObject[] m_PatrolPoints;
    //private int m_RandomPoint;

    [Header("Particles")]
    [SerializeField] private GameObject m_HitBloodEffect;

    [Header ("Drop")]
    [SerializeField] private GameObject m_CoinPrefab;
    [SerializeField] private GameObject m_AmmoItemPrefab;

    Rigidbody2D rb;
    PlayerController l_Player;


    [Header("Sounds")]
    [SerializeField] private AudioClip m_zombieHit;
    [SerializeField] private AudioClip m_zombieAtk;
    [SerializeField] private AudioClip m_zombieDie;


    private float m_AttackTimer;
    //private float l_RotateTimer = 0;
    private float l_TimeToRotate;

    enum TState
    {
        IDLE = 0,
        PATROL,
        CHASE,
        ATTACK,
        DIE
    }
    private TState m_State;


    void Start()
    {
        GameManager.GetGameManager().RegisterZombie(this);
        m_Life *= GameManager.GetGameManager().GetLifeMuliplier();
        m_CurrentLife = (int)m_Life;
        m_Speed *= GameManager.GetGameManager().GetSpeedMultiplier() * GameManager.GetGameManager().GetZombieSpeedBuff();
        rb = GetComponent<Rigidbody2D>();
        l_Player = GameManager.GetGameManager().GetPlayer();
        if (l_Player == null)
            l_Player = GameManager.GetGameManager().GetPlayer();

        //m_PatrolPoints = GameObject.FindGameObjectsWithTag("Point");
        //m_RandomPoint = Random.Range(0, m_PatrolPoints.Length);
        l_TimeToRotate = Random.Range(1, 4);
        SetChaseState();
    }

    void Update()
    {
        switch (m_State)
        {
            case TState.CHASE: UpdateChaseState(); break;
            case TState.ATTACK: UpdateAttackState(); break;
        }
    }


    void SetChaseState()
    {
        m_State = TState.CHASE;
    }
    
    void UpdateChaseState()
    {
        if (l_Player == null) return;

        Vector2 l_DirectionChase = (l_Player.transform.position - rb.transform.position).normalized;

        RaycastHit2D sight = Physics2D.Raycast(transform.position, l_DirectionChase, l_DirectionChase.magnitude, m_ObstacleLayer); //Ve directamente al player?
        if (sight.collider == null)
            Move(l_DirectionChase);
        else
        {
            // probar izquierda
            Vector2 left = Quaternion.Euler(0, 0, 90) * l_DirectionChase;
            RaycastHit2D leftHit = Physics2D.Raycast(transform.position, left, 0.7f, m_ObstacleLayer);

            if (leftHit.collider == null)
            {
                Move(left);
            }
            else
            {
                // probar derecha
                Vector2 right = Quaternion.Euler(0, 0, -90) * l_DirectionChase;
                Move(right);
            }
        }

        if (l_DirectionChase.magnitude <= m_AttackDistance)
        {
            rb.linearVelocity = Vector2.zero;
            m_AttackTimer = 0f;
            SetAttackState();
        }
    }

    void SetAttackState()
    {
        m_State = TState.ATTACK;
    }

    void UpdateAttackState()
    {
        Vector2 l_Direction = l_Player.transform.position - rb.transform.position;
        if (m_AttackDistance > l_Direction.magnitude)
        {
            float l_Angle = Mathf.Atan2(l_Direction.y, l_Direction.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, l_Angle);

            rb.linearVelocity = Vector2.zero;

            m_AttackTimer += Time.deltaTime;

            if (m_AttackTimer >= m_AttackTime)
            {
                AudioSource.PlayClipAtPoint(m_zombieAtk, transform.position);
                l_Player.PlayerTakeDamage(m_Damage);
                m_AttackTimer = 0f;
            }
        }
        if (m_AttackDistance < l_Direction.magnitude)
        {
            m_AttackTimer = 0f;
            SetChaseState();
        }
    }

    void SetDieState()
    {
        m_State = TState.DIE;
        StartCoroutine(DieCoroutine());

    }

    IEnumerator DieCoroutine()
    {
        AudioSource.PlayClipAtPoint(m_zombieDie, transform.position);
        DropLoot();
        GameManager.GetGameManager().UnregisterZombie(this);
        GameManager.GetGameManager().RegisterZombieDeath();
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }


    public void ZombieTakeDamage(int damage, Vector2 hitPoint)
    {
        m_CurrentLife -= damage;

        if (m_HitBloodEffect != null)
        {
            GameObject l_Particles = Instantiate(m_HitBloodEffect, hitPoint, Quaternion.identity);
            StartCoroutine(DestroyParticles(l_Particles));
        }

        if (m_CurrentLife <= 0)
        {
            SetDieState();
        }
        else
        {
            AudioSource.PlayClipAtPoint(m_zombieHit, transform.position);
            //meter anim de daño si se quiere
        }
    }

    IEnumerator DestroyParticles(GameObject particles)
    {
        yield return new WaitForSeconds(0.4f);
        Destroy(particles);
    }
    void Move(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        Vector2 newPos = rb.position + direction * m_Speed * Time.fixedDeltaTime;
        rb.MovePosition(newPos);
    }

    void DropLoot()
    {
        Instantiate(m_CoinPrefab, transform.position, Quaternion.identity);
        float rnd = Random.value;
        if (rnd < 0.4f)
        {
            Instantiate(m_AmmoItemPrefab, transform.position, Quaternion.identity);
        }
    }

    public void BuffSpeed(float value)
    {
        m_Speed *= value;
    }
}