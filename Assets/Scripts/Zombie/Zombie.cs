using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Zombie : MonoBehaviour
{
    [Header (" Basic Stats")]
    private int m_CurrentLife;
    [SerializeField] private float m_Life = 100;

    [SerializeField] private float m_Speed = 2f;

    [Header("Attack")]
    [SerializeField] private int m_Damage = 5;
    [SerializeField] private float m_AttackDistance = 1f;
    [SerializeField] private float m_AttackTime = 1f;

    [SerializeField] private LayerMask m_PlayerLayer;
    [SerializeField] private LayerMask m_ObstacleLayer;

    [Header("Particles")]
    [SerializeField] private GameObject m_HitBloodEffect;

    [Header ("Drop")]
    [SerializeField] private GameObject m_CoinPrefab;
    [SerializeField] private GameObject m_AmmoItemPrefab;

    Rigidbody2D rb;
    PlayerController l_Player;


    [Header("Sounds")]
    [SerializeField] private AudioClip m_zombie1;
    [SerializeField] private AudioClip m_zombie2;
    [SerializeField] private AudioClip m_zombie3;
    [SerializeField] private AudioClip m_zombieHit;
    [SerializeField] private AudioClip m_zombieAtk;
    [SerializeField] private AudioClip m_zombieDie;

    [Header("Skins list")]
    public float mdsrghd = 4;
    [SerializeField] public List<Sprite> m_SkinList;
    private SpriteRenderer m_Sprite;


    private float m_AttackTimer;
    //private float l_RotateTimer = 0;
    private float l_TimeToRotate;
    private float m_minSoundTime = 4f;
    private float m_maxSoundTime = 12f;
    private float m_soundTime;
    private float m_time;
    private int m_zombieSound;

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
        m_Sprite = GetComponent<SpriteRenderer>();
        if (m_SkinList != null && m_SkinList.Count > 0)
        {
            int randomIndex = Random.Range(0, m_SkinList.Count);
            m_Sprite.sprite = m_SkinList[randomIndex];
        }
        GameManager.GetGameManager().RegisterZombie(this);
        m_Life *= GameManager.GetGameManager().GetLifeMuliplier();
        m_CurrentLife = Mathf.RoundToInt(m_Life);
        m_Speed *= GameManager.GetGameManager().GetSpeedMultiplier() * GameManager.GetGameManager().GetZombieSpeedBuff();
        rb = GetComponent<Rigidbody2D>();
        l_Player = GameManager.GetGameManager().GetPlayer();
        if (l_Player == null)
            l_Player = GameManager.GetGameManager().GetPlayer();

        l_TimeToRotate = Random.Range(1, 4);
        SetChaseState();
        m_soundTime = Random.Range(m_minSoundTime, m_maxSoundTime);
    }

    void Update()
    {
        m_time += Time.deltaTime;
        if (m_time >= m_soundTime)
        {
            m_zombieSound = Random.Range(1, 4);
            switch (m_zombieSound)
            {
                case 1: AudioSource.PlayClipAtPoint(m_zombie1, transform.position); break;

                case 2:  AudioSource.PlayClipAtPoint(m_zombie2, transform.position); break;

                case 3:  AudioSource.PlayClipAtPoint(m_zombie3, transform.position); break;
            }
            m_time = 0;
            m_soundTime = Random.Range(m_minSoundTime, m_maxSoundTime);
        }
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
            Vector2 left = Quaternion.Euler(0, 0, 90) * l_DirectionChase;
            RaycastHit2D leftHit = Physics2D.Raycast(transform.position, left, 0.7f, m_ObstacleLayer);

            if (leftHit.collider == null)
            {
                Move(left);
            }
            else
            {
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
        m_Speed = 0;
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;
        GetComponent<Collider2D>().enabled = false;
        AudioSource.PlayClipAtPoint(m_zombieDie, transform.position);
        yield return new WaitForSeconds(0.4f);
        DropLoot();
        GameManager.GetGameManager().UnregisterZombie(this);
        GameManager.GetGameManager().RegisterZombieDeath();
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
        yield return new WaitForSeconds(0.35f);
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