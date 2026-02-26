using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header ("Basic Stats")]
    [SerializeField] public int m_Life = 100;
    public int m_CurrentLife;
    [SerializeField] private float m_Speed = 2f;

    [Header("Shoot")]
    public Transform m_Crosshair;
    public GameObject m_HitEffect;
    public float m_ShootMaxDistance = 50.0f;
    public LayerMask m_ShootLayerMask;
    //public GameObject m_ShootParticles;
    public GameObject m_LineTracer;
    public float m_NextFire = 0;
    public float m_ReloadTime = 2f;

    [Header ("Weapon")]
    [SerializeField] private int m_MaxAmmo = 30;
    [SerializeField] public int m_CurrentAmmo;
    [SerializeField] private int m_MaxAmmoOnBack = 120;
    [SerializeField] public int m_CurrentAmmoOnBack;
    [SerializeField] public float m_FireRate = 0.2f;
    private bool m_IsShooting = false;
    private bool m_IsReloading = false;
    

    [Header("Objects")]
    [SerializeField] private GameObject m_Weapon;
    PlayerController l_Player;
    Vector3 m_StartingPosition;
    Quaternion m_StartingRotation;
    Rigidbody2D m_RigidBody;
    Vector2 m_Movement;

    public event Action<int, int> OnLifeChanged;
    public event Action<int, int> OnAmmoChanged;

    [Header("Sprint / Stamina")]
    [SerializeField] private float m_SprintMultiplier = 1.5f;
    private bool m_IsSprinting = false;

    void Start()
    {
        m_CurrentAmmo = m_MaxAmmo;
        m_CurrentAmmoOnBack = m_MaxAmmoOnBack / 2;
        m_CurrentLife = m_Life;
        m_RigidBody = GetComponent<Rigidbody2D>();

        PlayerController l_Player = GameManager.GetGameManager().GetPlayer();
        if (l_Player != null)
        {
            l_Player.transform.position = transform.position;
            l_Player.transform.rotation = transform.rotation;
            GameObject.Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        GameManager.GetGameManager().SetPlayer(this);

        m_StartingPosition = transform.position;
        m_StartingRotation = transform.rotation;
 
    }

    void Update()
    {
<<<<<<< Updated upstream
        if (GameManager.GetGameManager().GetState() == GameManager.TState.PAUSED)
            return;

        if (m_ShootTimer > 0f)
        {
            m_ShootTimer -= Time.deltaTime;
        }
        else
        {
            m_CanShoot = true;
        }
=======
>>>>>>> Stashed changes

        Cursor.lockState = CursorLockMode.Confined;
        Vector3 l_MousePosition = Input.mousePosition;
        l_MousePosition.z = -Camera.main.transform.position.z;
        Vector3 l_WorldPos = Camera.main.ScreenToWorldPoint(l_MousePosition);
        Vector2 l_LookDirection = l_WorldPos - transform.position;
        float angle = Mathf.Atan2(l_LookDirection.y, l_LookDirection.x) * Mathf.Rad2Deg;
        m_Movement = Vector2.zero;

        if (Input.GetKey(Settings.m_ForwardKey))
            m_Movement.y += m_Speed;
        if (Input.GetKey(Settings.m_BackwardKey))
            m_Movement.y -= m_Speed;
        if (Input.GetKey(Settings.m_RightKey))
            m_Movement.x += m_Speed;
        if (Input.GetKey(Settings.m_LeftKey))
            m_Movement.x -= m_Speed;

        Vector2 inputDir = m_Movement;
        inputDir.Normalize();

        // SPRINT
        m_IsSprinting = Input.GetKey(Settings.m_RunKey);
        float speed = m_Speed;
        if (m_IsSprinting)
            speed *= m_SprintMultiplier;

        m_Movement = inputDir * speed;

        transform.rotation = Quaternion.Euler(0, 0, angle);
        if (CanShoot() && Input.GetMouseButtonDown(0) && Time.time >= m_NextFire)
        {
            Shoot();
        }


        if (CanReload() && Input.GetKeyDown(Settings.m_ReloadKey))
        {
            Reload();
        }
    }
    private void FixedUpdate()
    {
        m_RigidBody.MovePosition(m_RigidBody.position + m_Movement * Time.fixedDeltaTime);
    }

    //SHOOT
    bool CanShoot()
    {
        return m_IsReloading == false && m_CurrentAmmo > 0;
    }

    void Shoot()
    {
        Debug.Log("Pium pium");

        Vector2 origin = m_Weapon.transform.position;
        Vector2 direction = transform.right;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, m_ShootMaxDistance, m_ShootLayerMask);
        Vector2 endPos = origin + direction * m_ShootMaxDistance;

        if (hit.collider != null)
        {
            Debug.Log("Golpeó a Zombie");

            if (hit.collider.CompareTag("Zombie"))
            {
                hit.collider.GetComponent<Zombie>().TakeDamage(10);
            }
            endPos = hit.point;
        }

        if (m_LineTracer != null)
        {
            GameObject tracer = Instantiate(m_LineTracer);
            tracer.GetComponent<BulletTracer>().Init(origin, endPos);
        }
<<<<<<< Updated upstream

        m_CurrentAmmo--;

        m_CanShoot = false;
        m_ShootTimer = m_CooldownBetweenShots;
=======
        SetAmmo(m_CurrentAmmo - 1, m_CurrentAmmoOnBack);
        m_NextFire = Time.time + m_FireRate;
>>>>>>> Stashed changes
    }

    //RELOAD
    bool CanReload()
    {
        return m_CurrentAmmo < m_MaxAmmo && m_IsReloading == false && m_IsShooting == false && m_CurrentAmmoOnBack > 0;
    }

    void Reload()
    {
        Debug.Log("Reloading");
        m_IsReloading = true;
        StartCoroutine(ReloadCoroutine());
    }



    void SetAmmo(int onLoad, int back)
    {
        m_CurrentAmmo = Mathf.Clamp(onLoad, 0, m_MaxAmmo);
        m_CurrentAmmoOnBack = Mathf.Max(back, 0);
        OnAmmoChanged?.Invoke(m_CurrentAmmo, m_CurrentAmmoOnBack);
    }

    IEnumerator ReloadCoroutine()
    {

        yield return new WaitForSeconds(2.5f);
        int neededAmmo = m_MaxAmmo - m_CurrentAmmo;
        int ammoToLoad = Mathf.Min(neededAmmo, m_CurrentAmmoOnBack);
        SetAmmo(m_CurrentAmmo + ammoToLoad, m_CurrentAmmoOnBack - ammoToLoad);
        m_IsReloading = false;
    }   
    
    //DAMAGE
    public void TakeDamage (int damage)
    {
        m_CurrentLife -= damage;
        Debug.Log(m_CurrentLife);
        OnLifeChanged?.Invoke(m_CurrentLife, m_Life);
        if (m_CurrentLife <= 0)
        {
            Die();
        }
        else
        {
            //meter animación de daño si se quiere
        }
    }

    //DIE
    void Die()
    {
        GameManager.GetGameManager().SetState(GameManager.TState.GAMEOVER);
        //meter animación de muerte

        Destroy(gameObject);
    }

    void CreateShootHitParticles(Vector2 position)
    {
        Instantiate(m_HitEffect, position, Quaternion.identity);
    }

<<<<<<< Updated upstream
    //GETTERS
    public int GetAmmo()
    {
        return m_CurrentAmmo;
    }
    public int GetMaxAmmo()
    {
        return m_MaxAmmo;
    }
    public int GetAmmoOnBack()
    {
        return m_CurrentAmmoOnBack;
    }
    public int GetCurrentLife()
    {
        return m_CurrentLife;
    }
    public int GetMaxLife()
    {
        return m_Life;
=======
    public void AddMaxLife()
    {
        m_Life = (int)(m_Life * 1.2f);

    }

    public void SwitchMiniGun()
    {
        m_FireRate /= 2;
>>>>>>> Stashed changes
    }
}