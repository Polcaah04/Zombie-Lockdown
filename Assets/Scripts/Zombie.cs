using UnityEngine;

public class Zombie : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private int m_Life;
    private float m_MaxLife = 100;
    private int m_Damage;
    private float m_MaxDamage = 5;

    public float m_Speed = 2f;
    public float m_ViewDistance = 2f;
    public float m_AttackDistance = 1f;
    public float m_AttackTime = 1f;

    public LayerMask m_PlayerLayer;
    public LayerMask m_ObstacleLayer;

    private Rigidbody2D rb;
    PlayerController l_Player;

    private Vector2 m_PatrolTarget;
    private float m_AttackTimer;

    private float m_IdleTime = 1f;
    private float m_IdleTimer;

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
        m_MaxLife *= GameManager.GetGameManager().GetLifeMuliplier();
        m_Life = (int)m_MaxLife;
        m_Speed *= GameManager.GetGameManager().GetSpeedMultiplier();
        rb = GetComponent<Rigidbody2D>();
        l_Player = GameManager.GetGameManager().GetPlayer();
        if(l_Player == null)
            l_Player = FindFirstObjectByType<PlayerController>();

        m_PatrolTarget = (Vector2)transform.position + (Vector2)transform.up * 3f;
        m_IdleTimer = 0f;
        m_State = TState.IDLE;
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_State)
        {
            case TState.IDLE: UpdateIdleState();
                break;
            case TState.PATROL: UpdatePatrolState();
                break;
            case TState.CHASE: UpdateChaseState();
                break;
            case TState.ATTACK: UpdateAttackState();
                break;
            case TState.DIE: UpdateDieState();
                break;
        }
    }

    void SetPatrolState()
    {
        m_PatrolTarget = (Vector2)transform.position + (Vector2)transform.up * 3f;
        m_State = TState.PATROL;
    }

    void UpdatePatrolState()
    {
        float l_DistanceToTarget = Vector2.Distance(transform.position, m_PatrolTarget);

        if (l_DistanceToTarget < 0.2f)
        {
            float rnd = Random.value;
            if (rnd < 0.4f)
            {
                SetIdleState();
                return;
            }
            else if (rnd < 0.7f)
            {
                transform.Rotate(0, 0, -30f);
            }
            else
            {
                transform.Rotate(0, 0, 30f);
            }
            m_PatrolTarget = (Vector2)transform.position + (Vector2)transform.up * 3f;
            return;
        }
        rb.linearVelocity = transform.up * m_Speed;

        if (SeesPlayer())
        {
            SetChaseState();
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
        Debug.Log("UPDATE IDLE");

        rb.linearVelocity = Vector2.zero;
        m_IdleTimer += Time.deltaTime;

        if (SeesPlayer())
        {
            Debug.Log("Seen");
            SetChaseState();
            return;
        }

        if (m_IdleTimer >= m_IdleTime)
        {
            SetPatrolState();
        }
    }

    void SetChaseState()
    {
        Debug.Log("SET CHASE");
        m_State = TState.CHASE;
    }

    void UpdateChaseState()
    {
        Debug.Log("UPDATE CHASE");
        Vector2 l_DirectionChase = l_Player.transform.position - transform.position;

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
    void SetAttackState()
    {
        m_State = TState.ATTACK;
    }

    void UpdateAttackState()
    {
        Vector2 l_Direction = l_Player.transform.position - transform.position;
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

    void SetDieState()
    {
        m_State = TState.DIE;
    }

    void UpdateDieState()
    {
        Debug.Log("DIE");
        rb.linearVelocity = Vector2.zero;
        Destroy(gameObject);
    }

    bool SeesPlayer()
    {
        Vector2 l_Direction = l_Player.transform.position - transform.position;

        if (l_Direction.magnitude > m_ViewDistance)
            return false;

        RaycastHit2D l_Hit = Physics2D.Raycast(transform.position, l_Direction.normalized, m_ViewDistance, m_ObstacleLayer | m_PlayerLayer);

        return l_Hit && l_Hit.collider.CompareTag("Player");
    }
}
