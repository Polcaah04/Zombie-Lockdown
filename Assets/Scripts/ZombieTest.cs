using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ZombieTest : MonoBehaviour
{
    enum TState
    {
        IDLE = 0,
        PATROL,
        ALERT,
        CHASE,
        ATTACK,
        DIE
    }

    private TState m_State;
    private TState m_PreviousState;
    private float m_AlertTimer = 2.0f;

    public NavMeshAgent m_NavMeshAgent;
    public Transform m_Target;
    private PlayerController l_Player;

    [Header("Distances")]
    public float m_MaxChaseDistance = 6f;
    public float m_AttackDistance = 1f;

    [Header("Patrol")]
    public List<Transform> m_PatrolPositions;
    private int m_CurrentPatrolPositionId = 0;

    [Header("Sight")]
    public float m_SightAngle = 60.0f;
    public LayerMask m_SightLayerMask;
    public float m_EyesHeight = 1.5f;
    public float m_PlayerEyesHeight = 1.6f;
    public LayerMask m_ObstacleMask;

    [Header("Hearing")]
    public float m_MaxHearDistance = 3.0f;

    [Header("Life")]
    public int m_Life = 100;

    [Header("Shoot")]

    public float m_AttackRate = 1.0f;
    public int m_Damage = 5;
    private float m_NextAttack = 0f;
    public Transform m_AttackPoint;

    private void Awake()
    {
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        SetIdleState();
    }

    private void Update()
    {
        switch (m_State)
        {
            case TState.IDLE: UpdateIdleState(); break;
            case TState.PATROL: UpdatePatrolState(); break;
            case TState.ALERT: UpdateAlertState(); break;
            case TState.CHASE: UpdateChaseState(); break;
            case TState.ATTACK: UpdateAttackState(); break;
            case TState.DIE: UpdateDieState(); break;
        }
    }

    void SetIdleState()
    {
        m_State = TState.IDLE;
        m_NavMeshAgent.isStopped = true;
    }

    void UpdateIdleState()
    {
        SetPatrolState();
    }

    void SetPatrolState()
    {
        m_State = TState.PATROL;
        m_NavMeshAgent.isStopped = false;
        MoveToNextPatrolPosition();
    }

    void UpdatePatrolState()
    {
        if (!m_NavMeshAgent.pathPending && m_NavMeshAgent.remainingDistance < 0.5f)
        {
            MoveToNextPatrolPosition();
        }
        if (HearsPlayer())
        {
            SetAlertState();
        }

        if (SeesPlayer())
        {
            SetChaseState();
        }
    }
    void SetAlertState()
    {
        m_State = TState.ALERT;
        m_AlertTimer = 2.0f;
        m_NavMeshAgent.isStopped = true;
    }

    void UpdateAlertState()
    {
        transform.Rotate(Vector3.up, 120f * Time.deltaTime);
        m_AlertTimer -= Time.deltaTime;
        if (SeesPlayer())
        {
            SetChaseState();

        }
        else if (m_AlertTimer <= 0f)
        {
            SetPatrolState();
        }
    }

    void SetChaseState()
    {
        m_State = TState.CHASE;
        m_NavMeshAgent.isStopped = false;
    }

    void UpdateChaseState()
    {
        if (m_Target == null) return;

        float distance = Vector3.Distance(m_Target.position, transform.position);
        if (distance <= m_AttackDistance)
        {
            SetAttackState();
        }
        else if (distance > m_MaxChaseDistance)
        {
            SetPatrolState();
        }
        else
        {
            m_NavMeshAgent.destination = m_Target.position;
        }
    }
    void SetAttackState()
    {
        m_State = TState.ATTACK;
        m_NavMeshAgent.isStopped = true;
    }

    void UpdateAttackState()
    {
        if (m_Target == null) return;

        float distance = Vector3.Distance(m_Target.position, transform.position);

        if (distance > m_AttackDistance)
        {
            SetChaseState();
            return;
        }
        if (Time.time >= m_NextAttack)
        {
            Shoot();
            m_NextAttack = Time.time + 1f / m_AttackRate;
        }
    }

    void Shoot()
    {
        Vector3 direction = (m_Target.position - m_AttackPoint.position).normalized;

        if (Physics.Raycast(m_AttackPoint.position, direction, out RaycastHit hit, m_AttackDistance))
        {
            l_Player = GameManager.GetGameManager().GetPlayer();
            if (l_Player != null)
            {
                l_Player.TakeDamage(m_Damage);
            }

            Debug.DrawRay(m_AttackPoint.position, direction * m_AttackDistance, Color.red, 0.2f);
        }
    }




    void SetDieState()
    {
        m_State = TState.DIE;

    }

    void UpdateDieState()
    {
        m_NavMeshAgent.isStopped = true;
        StartCoroutine(Destroy());
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }
    void MoveToNextPatrolPosition()
    {
        if (m_PatrolPositions.Count == 0) return;

        m_NavMeshAgent.destination = m_PatrolPositions[m_CurrentPatrolPositionId].position;
        m_CurrentPatrolPositionId = (m_CurrentPatrolPositionId + 1) % m_PatrolPositions.Count;
    }

    bool SeesPlayer()
    {
        if (m_Target == null)
            return false;
        Vector3 origin = transform.position + transform.up * m_EyesHeight;
        Vector3 targetEyes = m_Target.position + Vector3.up * m_PlayerEyesHeight;

        Vector3 toPlayer = targetEyes - origin;
        float distance = toPlayer.magnitude;
        if (distance <= 0.001f)
            return true;

        Vector3 dir = toPlayer / distance;
        float halfAngle = m_SightAngle * 0.5f;
        float angleToPlayer = Vector3.Angle(transform.forward, dir);
        if (angleToPlayer > halfAngle)
            return false;
        if (Physics.Raycast(origin, dir, out RaycastHit hit, distance, m_ObstacleMask))
        {
            return false;
        }
        return true;
    }

    bool HearsPlayer()
    {
        float distance = Vector3.Distance(m_Target.position, transform.position);
        return distance < m_MaxHearDistance;
    }

    public void Hit(int damage)
    {
        if (m_State == TState.DIE)
            return;

        m_Life -= damage;

        if (m_Life <= 0)
        {
            SetDieState();
        }
    }
}
