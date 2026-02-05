using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float m_Speed = 2f;
    [SerializeField] private int m_Life = 100;
    private int m_CurrentLife;
    private int m_MaxAmmo = 30;
    private int m_CurrentAmmo;
    private int m_MaxAmmoOnBack = 120;
    private int m_CurrentAmmoOnBack;
    private PlayerController l_Player;
    private Vector3 m_StartingPosition;
    private Quaternion m_StartingRotation;
    private Rigidbody2D m_RigidBody;
    private Vector2 m_Movement;

    void Start()
    {
        m_CurrentAmmo = m_MaxAmmo;
        m_CurrentLife = m_Life;
        m_RigidBody = GetComponent<Rigidbody2D>();
        //m_Shield = m_MaxShield;

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

        if (Input.GetKey(KeyCode.W))
            m_Movement.y += m_Speed;
        if (Input.GetKey(KeyCode.S))
            m_Movement.y -= m_Speed;
        if (Input.GetKey(KeyCode.D))
            m_Movement.x += m_Speed;
        if (Input.GetKey(KeyCode.A))
            m_Movement.x -= m_Speed;

        m_Movement.Normalize();
        m_Movement *= m_Speed;
        
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    private void FixedUpdate()
    {
        m_RigidBody.MovePosition(m_RigidBody.position + m_Movement * Time.fixedDeltaTime);
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

