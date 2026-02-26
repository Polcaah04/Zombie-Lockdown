using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI t_Hp;
    [SerializeField] private TextMeshProUGUI t_Coins;
    [SerializeField] private TextMeshProUGUI t_Ammo;
    [SerializeField] private TextMeshProUGUI t_Time;
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
        UpdateTime((int)GameManager.GetGameManager().GetRoundTime());

    }
}

