using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header ("Basic Stats")]
    [SerializeField] private int m_Life = 100;
    private int m_CurrentLife;
    [SerializeField] private float m_Speed = 2f;

    [Header ("Weapon")]
    [SerializeField] private int m_MaxAmmo = 30;
    [SerializeField] private int m_CurrentAmmo;
    [SerializeField] private int m_MaxAmmoOnBack = 120;
    [SerializeField] private int m_CurrentAmmoOnBack;
    [SerializeField] private float m_FireRate;
    private bool m_IsShooting = false;
    private bool m_IsReloading = false;
    

    [Header("Objects")]
    [SerializeField] private GameObject m_Weapon;
    PlayerController l_Player;
    Vector3 m_StartingPosition;
    Quaternion m_StartingRotation;
    Rigidbody2D m_RigidBody;
    Vector2 m_Movement;

    [Header("Inputs")]
    [SerializeField] private KeyCode m_ForwardKey = KeyCode.W;
    [SerializeField] private KeyCode m_BackWardKey = KeyCode.S;
    [SerializeField] private KeyCode m_LeftKey = KeyCode.A;
    [SerializeField] private KeyCode m_RightKey = KeyCode.D;
    [SerializeField] private KeyCode m_ReloadKey = KeyCode.R;


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
        Cursor.lockState = CursorLockMode.Confined;
        Vector3 l_MousePosition = Input.mousePosition;
        l_MousePosition.z = -Camera.main.transform.position.z;
        Vector3 l_WorldPos = Camera.main.ScreenToWorldPoint(l_MousePosition);
        Vector2 l_LookDirection = l_WorldPos - transform.position;
        float angle = Mathf.Atan2(l_LookDirection.y, l_LookDirection.x) * Mathf.Rad2Deg;
        m_Movement = Vector2.zero;

        if (Input.GetKey(m_ForwardKey))
            m_Movement.y += m_Speed;
        if (Input.GetKey(m_BackWardKey))
            m_Movement.y -= m_Speed;
        if (Input.GetKey(m_RightKey))
            m_Movement.x += m_Speed;
        if (Input.GetKey(m_LeftKey))
            m_Movement.x -= m_Speed;

        m_Movement.Normalize();
        m_Movement *= m_Speed;
        
        transform.rotation = Quaternion.Euler(0, 0, angle);
        if (CanShoot() && Input.GetMouseButtonDown(0))
        {
            Shoot();
        }


        if (CanReload() && Input.GetKeyDown(m_ReloadKey))
        {
            Reload();
        }
    }
    private void FixedUpdate()
    {
        m_RigidBody.MovePosition(m_RigidBody.position + m_Movement * Time.fixedDeltaTime);
    }


    bool CanShoot()
    {
        return m_IsReloading == false && m_CurrentAmmo > 0;
    }

    void Shoot()
    {
        Debug.Log("Pium pium");
    }

    bool CanReload()
    {
        return m_CurrentAmmo < m_MaxAmmo && m_IsReloading == false && m_IsShooting == false && m_CurrentAmmoOnBack > 0;
    }

    void Reload()
    {
        Debug.Log("Reloading");
        m_IsReloading = true;
        StartCoroutine(ReloadCoroutine());
        
        m_CurrentAmmo = m_MaxAmmo;
        m_IsReloading = false;
    }

    IEnumerator ReloadCoroutine()
    {

        yield return new WaitForSeconds(2.5f);
    }   
    
    public void TakeDamage (int damage)
    {
        m_CurrentLife -= damage;
        Debug.Log(m_CurrentLife);
        if (m_CurrentLife <= 0)
        {
            Die();
        }
        else
        {
            //meter animación de daño si se quiere
        }
    }

    void Die()
    {
        //meter animación de muerte
        Destroy(gameObject);
    }

    

}

