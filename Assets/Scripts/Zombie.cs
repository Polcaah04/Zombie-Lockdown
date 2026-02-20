using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;

public class Zombie : MonoBehaviour
{
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
        l_TimeToRotate = Random.Range(1, 4);
        SetChaseState();
    }

    void Update()
    {
        switch (m_State)
        {
            case TState.CHASE: UpdateChaseState(); break;
            case TState.ATTACK: UpdateAttackState(); break;
            case TState.DIE: UpdateDieState(); break;
        }
    }


    void SetChaseState()
    {
        Debug.Log("SET CHASE");
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
        Debug.Log("Zombie get damage");
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
    void Move(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        Vector2 newPos = rb.position + direction * m_Speed * Time.fixedDeltaTime;
        rb.MovePosition(newPos);
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