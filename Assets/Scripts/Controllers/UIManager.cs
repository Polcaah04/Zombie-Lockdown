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