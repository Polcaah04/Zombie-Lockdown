using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;

public class Zombie : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [Header (" Basic Stats")]
    private int m_CurrentLife;
    [SerializeField] private float m_Life = 100;

    [SerializeField] private float m_Speed = 2f;

    [Header ("Sight")]
    [SerializeField] private float m_ViewDistance = 4f;
    [SerializeField] private int m_MaxViewDistance = 6;

    [Header("Attack")]
    [SerializeField] private int m_Damage = 5;
    [SerializeField] private float m_AttackDistance = 1f;
    [SerializeField] private float m_AttackTime = 1f;

    [SerializeField] private LayerMask m_PlayerLayer;
    [SerializeField] private LayerMask m_ObstacleLayer;
    //[SerializeField] private GameObject[] m_PatrolPoints;
    //private int m_RandomPoint;

    Rigidbody2D rb;
    PlayerController l_Player;

    
    private float m_AttackTimer;

    private float m_IdleTime = 1f;
    private float m_IdleTimer;
    private float m_PatrolTime;
    private float m_PatrolTimer;
    private float l_RotateTimer = 0;
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
        m_Life *= GameManager.GetGameManager().GetLifeMuliplier();
        m_CurrentLife = (int)m_Life;
        m_Speed *= GameManager.GetGameManager().GetSpeedMultiplier();
        rb = GetComponent<Rigidbody2D>();
        l_Player = GameManager.GetGameManager().GetPlayer();
        if(l_Player == null)
            l_Player = FindFirstObjectByType<PlayerController>();

        //m_PatrolPoints = GameObject.FindGameObjectsWithTag("Point");
        //m_RandomPoint = Random.Range(0, m_PatrolPoints.Length);
        m_IdleTimer = 0f;
        l_TimeToRotate = Random.Range(1, 4);
        SetPatrolState();
    }

    void Update()
    {
        switch (m_State)
        {
            case TState.IDLE: UpdateIdleState(); break;
            case TState.PATROL: UpdatePatrolState(); break;
            case TState.CHASE: UpdateChaseState(); break;
            case TState.ATTACK: UpdateAttackState(); break;
            case TState.DIE: UpdateDieState(); break;
        }
    }

    void SetIdleState()
    {
        Debug.Log("SET IDLE");
        m_IdleTimer = 0f;
        rb.linearVelocity = Vector2.zero;
        m_State = TState.IDLE;
    }

    void UpdateIdleState()
    {
        m_IdleTimer += Time.deltaTime;
        Rotation();

        if (SeesPlayer())
        {
            SetChaseState();
            return;
        }
        
        if (m_IdleTimer >= m_IdleTime)
        {
            SetPatrolState();
        }
    }

    void SetPatrolState()
    {
        Debug.Log("SET PATROL");
        m_PatrolTimer = 0f;
        m_PatrolTime = Random.Range(1, 3);
        m_State = TState.PATROL;
    }

    void UpdatePatrolState()
    {
        m_PatrolTimer += Time.deltaTime;
        rb.linearVelocity = transform.up * m_Speed;

        if (m_PatrolTimer >= m_PatrolTime)
        {
            SetIdleState();
            return;
        }

        if (SeesPlayer())
        {
            SetChaseState();
        }
    }


    void SetChaseState()
    {
        Debug.Log("SET CHASE");
        m_State = TState.CHASE;
    }

    void UpdateChaseState()
    {
        Vector2 l_DirectionChase = l_Player.transform.position - rb.transform.position;
        if (m_MaxViewDistance > l_DirectionChase.magnitude)
        {           
            float l_Angle = Mathf.Atan2(l_DirectionChase.y, l_DirectionChase.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, l_Angle);

            rb.linearVelocity = transform.up * m_Speed;

            if (l_DirectionChase.magnitude <= m_AttackDistance)
            {
                rb.linearVelocity = Vector2.zero;
                m_AttackTimer = 0f;
                SetAttackState();
            }
        }
        else
        {
            SetIdleState();
        }
    }
    void SetAttackState()
    {
        m_State = TState.ATTACK;
    }

    void UpdateAttackState()
    {
        Vector2 l_Direction = l_Player.transform.position - rb.transform.position;
        Debug.Log(l_Direction);   
        if (m_AttackDistance > l_Direction.magnitude)
        {
            float l_Angle = Mathf.Atan2(l_Direction.y, l_Direction.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, l_Angle);

            rb.linearVelocity = Vector2.zero;

            m_AttackTimer += Time.deltaTime;

            if (m_AttackTimer >= m_AttackTime)
            {
                l_Player.TakeDamage(m_Damage);
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
        Debug.Log("DIE");
        m_State = TState.DIE;
    }

    void UpdateDieState()
    {
        GameManager.GetGameManager().RegisterZombieDeath();
        Destroy(gameObject);
    }

    bool SeesPlayer()
    {
        Vector2 l_Direction = l_Player.transform.position - rb.transform.position;
        if (l_Direction.magnitude > m_ViewDistance)
            return false;

        RaycastHit2D l_Hit = Physics2D.Raycast(transform.position, l_Direction.normalized, m_ViewDistance, m_ObstacleLayer | m_PlayerLayer);

        return l_Hit && l_Hit.collider.CompareTag("Player");
    }

     void Rotation()
     {
        l_RotateTimer += Time.deltaTime;

        if (l_RotateTimer >= l_TimeToRotate)
        {
            transform.Rotate(0, 0, Random.Range(-60, 60));
            l_TimeToRotate = Random.Range(1, 3);
            l_RotateTimer = 0;
        }
     }

    public void TakeDamage(int damage)
    {
        m_CurrentLife -= damage;

        if (m_CurrentLife <= 0)
        {
            SetDieState();
        }
        else
        {
            //meter anim de daño si se quiere
        }
    }

    /*void Movement()
    {
        rb.linearVelocity = Vector2.MoveTowards(transform.position, m_PatrolPoints[m_RandomPoint].transform.position, m_Speed * Time.deltaTime);
        if (Vector2.Distance(transform.position, m_PatrolPoints[m_RandomPoint].transform.position) < 0.25)
        {
            m_IdleTime = Time.deltaTime;
        }
        if (m_IdleTime >= m_IdleTimer)
        {
            m_RandomPoint = Random.Range(0, m_PatrolPoints.Length);
            m_IdleTime = 0;
        }
    }*/
}