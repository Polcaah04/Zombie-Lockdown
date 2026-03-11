using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header ("Basic Stats")]
    public int m_Life = 100;
    public int m_CurrentLife;
    [SerializeField] private float m_Speed = 2f;
    private bool m_IsInvincible = false;
    private bool m_HasSecondChance = false;
    private float m_SecondLifeHp;

    [Header("Life Regen")]
    [SerializeField] private float m_RegenDelay = 5f;
    [SerializeField] private int m_RegenAmount = 5; 
    [SerializeField] private float m_RegenRate = 3f;
    private Coroutine m_RegenCoroutine;

    [Header("Shoot")]
    [SerializeField] private Transform m_Crosshair;
    [SerializeField] private GameObject m_HitEffect;
    [SerializeField] private float m_ShootMaxDistance = 50.0f;
    [SerializeField] private LayerMask m_ShootLayerMask;
    [SerializeField] private GameObject m_LineTracer;
    private float m_NextFire = 0;
    [SerializeField] private int m_Damage = 10;
    [Header ("Weapon")]
    [SerializeField] private int m_MaxAmmo = 30;
    [SerializeField] public int m_CurrentAmmo;
    [SerializeField] private int m_MaxAmmoOnBack = 120;
    [SerializeField] public int m_CurrentAmmoOnBack;
    [SerializeField] public float m_FireRate = 0.2f;
    private bool m_IsShooting = false;
    private bool m_IsReloading = false;

    [Header("Particles")]
    [SerializeField] private GameObject m_HitBloodEffect;

    [Header("Animations")]
    public Animator m_WeaponAnimator;
    private Animator m_Animator;


    [Header("Objects")]
    [SerializeField] private GameObject m_Weapon;

    Vector3 m_StartingPosition;
    Quaternion m_StartingRotation;
    Rigidbody2D m_RigidBody;
    Vector2 m_Movement;

    public event Action<int, int> OnLifeChanged;
    public event Action<int, int> OnAmmoChanged;

    [Header("Sprint / Stamina")]
    [SerializeField] private float m_SprintMultiplier = 1.5f;
    private bool m_IsSprinting = false;

    [Header("UI Damage")]
    [SerializeField] private Image m_DamageOverlay;
    [SerializeField] private float m_DamageFlashTime = 0.2f;

    [Header("Sounds")]
    [SerializeField] private AudioClip m_shoot;
    [SerializeField] private AudioClip m_reload;
    [SerializeField] private AudioClip m_dmg;

    void Start()
    {
        m_CurrentAmmo = m_MaxAmmo;
        m_CurrentAmmoOnBack = m_MaxAmmoOnBack / 2;
        m_CurrentLife = m_Life;
        m_RigidBody = GetComponent<Rigidbody2D>();
        m_Animator = GetComponent<Animator>();

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
        

        if (GameManager.GetGameManager().GetState() == GameManager.TState.PAUSED)
            return;

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
        m_Animator.SetFloat("speed", m_Movement.magnitude);

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
        AudioSource.PlayClipAtPoint(m_shoot, transform.position);
        SetShootAnimation();
        Vector2 origin = m_Weapon.transform.position;
        Vector2 direction = transform.right;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, m_ShootMaxDistance, m_ShootLayerMask);
        Vector2 endPos = origin + direction * m_ShootMaxDistance;

        if (hit.collider != null)
        {

            if (hit.collider.CompareTag("Zombie"))
            {
                Zombie z = hit.collider.GetComponent<Zombie>();
                z.ZombieTakeDamage(m_Damage, hit.point);
            }

            endPos = hit.point;

            if (m_HitEffect != null)
            {
                CreateShootHitParticles(hit.point);
            }
        }

        if (m_LineTracer != null)
        {
            GameObject tracer = Instantiate(m_LineTracer);
            tracer.GetComponent<BulletTracer>().Init(origin, endPos);
        }    
        SetAmmo(m_CurrentAmmo - 1, m_CurrentAmmoOnBack);
        m_NextFire = Time.time + m_FireRate;
    }

    //RELOAD
    bool CanReload()
    {
        return m_CurrentAmmo < m_MaxAmmo && m_IsReloading == false && m_IsShooting == false && m_CurrentAmmoOnBack > 0;
    }

    void Reload()
    {
        AudioSource.PlayClipAtPoint(m_reload, transform.position);
        m_IsReloading = true;
        SetReloadAnimation();
        StartCoroutine(ReloadCoroutine());
    }

    public void AddAmmo(int ammo)
    {
        m_CurrentAmmoOnBack += ammo;
        if(m_CurrentAmmoOnBack > m_MaxAmmoOnBack)
            m_CurrentAmmoOnBack = m_MaxAmmoOnBack;
        OnAmmoChanged?.Invoke(m_CurrentAmmo, m_CurrentAmmoOnBack);
    }
    void SetAmmo(int onLoad, int back)
    {
        m_CurrentAmmo = Mathf.Clamp(onLoad, 0, m_MaxAmmo);
        m_CurrentAmmoOnBack = Mathf.Max(back, 0);
        OnAmmoChanged?.Invoke(m_CurrentAmmo, m_CurrentAmmoOnBack);
    }

    IEnumerator ReloadCoroutine()
    {
        yield return new WaitForSeconds(1.3f);
        int neededAmmo = m_MaxAmmo - m_CurrentAmmo;
        int ammoToLoad = Mathf.Min(neededAmmo, m_CurrentAmmoOnBack);
        SetAmmo(m_CurrentAmmo + ammoToLoad, m_CurrentAmmoOnBack - ammoToLoad);
        m_IsReloading = false;
    }   
    
    //DAMAGE
    public void PlayerTakeDamage (int damage)
    {
        if (m_IsInvincible == false)
        {
            AudioSource.PlayClipAtPoint(m_dmg, transform.position);
            m_CurrentLife -= damage;
            OnLifeChanged?.Invoke(m_CurrentLife, m_Life);

            if (m_HitBloodEffect != null)
            {
                Vector3 spawnPos = transform.position + transform.up * 0.1f;
                Instantiate(m_HitBloodEffect, spawnPos, Quaternion.identity);
            }
                
            ShowDamageOverlay();
            StartLifeRegen();
        }
        
        if (m_CurrentLife <= 0)
        {
            if (m_HasSecondChance == false)
            {
                Die();
            }
            else if (m_HasSecondChance == true)
            {
                m_CurrentLife = (int)(m_Life * m_SecondLifeHp);
            }
            
        }
        else
        {
            //meter animación de daño si se quiere
        }
    }

    void StartLifeRegen()
    {
        if (m_RegenCoroutine != null)
            StopCoroutine(m_RegenCoroutine);

        m_RegenCoroutine = StartCoroutine(LifeRegenCoroutine());
    }

    IEnumerator LifeRegenCoroutine()
    {
        yield return new WaitForSeconds(m_RegenDelay);

        while (m_CurrentLife < m_Life)
        {
            m_CurrentLife += m_RegenAmount;
            m_CurrentLife = Mathf.Clamp(m_CurrentLife, 0, m_Life);

            OnLifeChanged?.Invoke(m_CurrentLife, m_Life);

            yield return new WaitForSeconds(m_RegenRate);
        }
    }

    //DIE
    void Die()
    {

        //meter animacion de muerte
        Destroy(this.gameObject);
        GameManager.GetGameManager().GameOver();
        
    }

    //ANIMATIONS
    void SetReloadAnimation()
    {
        m_WeaponAnimator.SetTrigger("reload");
    }
    void SetShootAnimation()
    {
        m_WeaponAnimator.SetTrigger("shoot");
    }

    void CreateShootHitParticles(Vector2 position)
    {
        Instantiate(m_HitEffect, position, Quaternion.identity);
    }

    //UI DAMAGE
    private void ShowDamageOverlay()
    {
        if (m_DamageOverlay == null) return; 
        StartCoroutine(DamageFlashCoroutine());
    }

    private IEnumerator DamageFlashCoroutine()
    {
        Color c = m_DamageOverlay.color;
        c.a = 0.2f; 
        m_DamageOverlay.color = c;

        yield return new WaitForSeconds(m_DamageFlashTime);

        c.a = 0f; 
        m_DamageOverlay.color = c;
    }

    public void BuffHealthRegen(float regenDelayRatio, float regenRateRatio)
    {
        m_RegenDelay *= regenDelayRatio;
        m_RegenRate *= regenRateRatio;
    }

    public void AddMaxLife(float multiplier)
    {
        m_Life = (int)(m_Life * multiplier);
        StartLifeRegen();
    }

    public void BuffSpeed(float multiplier)
    {
        m_Speed *= multiplier;
    }

    public void BuffFireRate(float value)
    {
        m_FireRate -= value;
    }

    public void InfiniteAmmo(float fireRateValue, int damageValue)
    {
        m_FireRate = fireRateValue;
        if (m_FireRate <= 0f)
        {
            m_FireRate = 0.1f;
        }
        m_Damage = damageValue;
    }

    public void BuffDamage(int value)
    {
        m_Damage += value;
    }

    public void MakeInvincible(bool value)
    {
        m_IsInvincible = value;
    }

    public void SecondChance(bool value, float lifeSet)
    {
        m_HasSecondChance = value;
        m_SecondLifeHp = lifeSet;        
    }

    public void NerfMaxAmmo(int value)
    {
        m_MaxAmmo /= value;
        m_MaxAmmoOnBack /= value;
    }

    public void NerfSpeed(float value)
    {
        m_Speed /= value;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
        {
            Item item = other.GetComponent<Item>();
            if (item != null)
            {
                item.Pick(this);
            }
        }
    }
}