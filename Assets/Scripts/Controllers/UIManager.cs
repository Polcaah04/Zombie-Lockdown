<<<<<<< Updated upstream
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_CoinsText;
    [SerializeField] private TextMeshProUGUI m_TimeText;
    [SerializeField] private TextMeshProUGUI m_AmmoText;
    [SerializeField] private GameObject m_WinUI;
    [SerializeField] private GameObject m_GameOverUI;
    [SerializeField] private GameObject m_PauseUI;

    private PlayerController m_Player;
    void Start()
    {
        m_Player = GameManager.GetGameManager().GetPlayer();
    }

    void Update()
    {
        GameManager gm = GameManager.GetGameManager();

        if (m_Player == null)
            m_Player = gm.GetPlayer();

        if (m_Player == null)
            return;

        // Coins
        m_CoinsText.text = gm.GetCoins().ToString();

        // Time
        m_TimeText.text = Mathf.FloorToInt(gm.GetRoundTime()).ToString();

        // Ammo
        m_AmmoText.text = m_Player.GetAmmo() + " / " + m_Player.GetAmmoOnBack();
        //Life
        //m_LifeText.text = m_Player.GetCurrentLife() + "/" + m_Player.GetMaxLife();

        // States
        m_WinUI.SetActive(gm.GetState() == GameManager.TState.WIN);
        m_GameOverUI.SetActive(gm.GetState() == GameManager.TState.GAMEOVER);
        if (gm.GetState() == GameManager.TState.PAUSED)
        {
            m_PauseUI.SetActive(true);
        }
        else
        {
            m_PauseUI.SetActive(false);
        }
    }
}
=======
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI t_Hp;
    [SerializeField] private TextMeshProUGUI t_Coins;
    [SerializeField] private TextMeshProUGUI t_Ammo;
    PlayerController l_Player;

    private void Start()
    {
        GameManager l_GameController = GameManager.GetGameManager();
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
    }
    private void InitializeHUD()
    {
        UpdateLife(l_Player.m_CurrentLife, l_Player.m_Life);
        UpdateCoins(GameManager.GetGameManager().GetCoins());
        UpdateAmmo(l_Player.m_CurrentAmmo, l_Player.m_CurrentAmmoOnBack);
    }
}
>>>>>>> Stashed changes
