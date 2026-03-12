using UnityEngine;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI t_Hp;
    [SerializeField] private TextMeshProUGUI t_Coins;
    [SerializeField] private TextMeshProUGUI t_Ammo;
    [SerializeField] private TextMeshProUGUI t_Time;
    PlayerController l_Player;


    [Header("Icons")]
    [SerializeField] private Transform m_BuffPanel;
    [SerializeField] private GameObject m_HealingFactorIcon;
    [SerializeField] private GameObject m_MaxLifeIcon;
    [SerializeField] private GameObject m_BuffDamageIcon;
    [SerializeField] private GameObject m_BuffFireRateIcon;
    [SerializeField] private GameObject m_BuffSpeedIcon;
    [SerializeField] private GameObject m_InfiniteShootIcon;
    [SerializeField] private GameObject m_InvincibleIcon;
    public GameObject m_SecondChanceIcon;
    [SerializeField] private GameObject m_LowAmmoIcon;
    [SerializeField] private GameObject m_LowSpeedIcon;
    [SerializeField] private GameObject m_BuffZombieSpeedIcon;
    [SerializeField] private GameObject m_LowVisionIcon;
    [SerializeField] private GameObject m_BuffZombieSpawnRateIcon;


    private void Start()
    {
        GameManager l_GameController = GameManager.GetGameManager();
        l_GameController.SetUIManager(this);
        l_GameController.OnPlayerReady += OnPlayerReady;
        if (l_GameController.GetPlayer() != null)
        {
            OnPlayerReady(l_GameController.GetPlayer());
        }
    }

    private void UpdateLife(int life, int maxLife)
    {
        if (t_Hp) t_Hp.text = $"{life}";
    }

    private void UpdateCoins(int coins)
    {
        if (t_Coins) t_Coins.text = $"{coins}";
    }

    private void UpdateAmmo(int ammo, int backAmmo)
    {
        if (t_Ammo) t_Ammo.text = $"{ammo}/{backAmmo}";
    }

    private void UpdateTime(int time)
    {
        if (t_Time) t_Time.text = $"{time}";
    }


    private void OnPlayerReady(PlayerController player)
    {
        l_Player = player;
        SubscribeToPlayerEvents();
        InitializeHUD();
    }
    private void SubscribeToPlayerEvents()
    {
        l_Player.OnLifeChanged += UpdateLife;
        GameManager.GetGameManager().OnCoinsChanged += UpdateCoins;
        l_Player.OnAmmoChanged += UpdateAmmo;
        GameManager.GetGameManager().OnTimeChanged += UpdateTime;
    }
    private void InitializeHUD()
    {
        UpdateLife(l_Player.m_CurrentLife, l_Player.m_Life);
        UpdateCoins(GameManager.GetGameManager().GetCoins());
        UpdateAmmo(l_Player.m_CurrentAmmo, l_Player.m_CurrentAmmoOnBack);
        UpdateTime(Mathf.RoundToInt(GameManager.GetGameManager().GetRoundTime()));

    }

    void OnEnable()
    {
        GameManager.OnBuffObtained += ActivateBuff;
    }

    void OnDisable()
    {
        GameManager.OnBuffObtained -= ActivateBuff;
    }

    public void ActivateBuff(string buffName, bool isActivated)
    {
        GameObject iconToActivate = null;

        switch (buffName)
        {
            case "Regen": iconToActivate = m_HealingFactorIcon; break;
            case "MaxLife": iconToActivate = m_MaxLifeIcon; StartCoroutine(LifeIconCoroutine(iconToActivate)); break;
            case "Damage": iconToActivate = m_BuffDamageIcon; break;
            case "FireRate": iconToActivate = m_BuffFireRateIcon; break;
            case "Speed": iconToActivate = m_BuffSpeedIcon; break;
            case "InfiniteShoot": iconToActivate = m_InfiniteShootIcon; break;
            case "Invincible": iconToActivate = m_InvincibleIcon; break;
            case "SecondChance": iconToActivate = m_SecondChanceIcon; break;
            case "LowAmmo": iconToActivate = m_LowAmmoIcon; break;
            case "LowSpeed": iconToActivate = m_LowSpeedIcon; break;
            case "ZombieSpeed": iconToActivate = m_BuffZombieSpeedIcon; break;
            case "LowVision": iconToActivate = m_LowVisionIcon; break;
            case "ZombieSpawnRate": iconToActivate = m_BuffZombieSpawnRateIcon; break;
        }

        if (iconToActivate != null)
        {
            iconToActivate.SetActive(isActivated);
            
        }
    }

    IEnumerator LifeIconCoroutine(GameObject icon)
    {
        yield return new WaitForSeconds(2f);
        icon.SetActive(false);

    }



}

