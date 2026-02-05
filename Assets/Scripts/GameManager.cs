using System.Collections;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    enum TState
    {
        PLAYING = 0,
        GAMEOVER
    }

    private TState m_State;

    static GameManager m_GameManager;
    PlayerController m_Player;
    public float m_GameTime;
    int m_DifficultyChangeInterval = 60;
    int m_DifficultyFixChange = 60;
    float m_Difficulty = 0;
    int m_MaxDifficult = 10;
    int m_BaseMultiplier = 1;

    public int m_Coins { get; private set; } = 0;

    public float m_ZombieLifeMultiplier { get; private set; } = 1;
    public float m_ZombieSpeedMultiplier { get; private set; } = 1;
    public float m_ZombieSpawnRateMultiplier { get; private set; } = 1;
    void Awake()
    {
        if (m_GameManager != null)
        {
            GameObject.Destroy(gameObject);
            return;
        }
        m_GameManager = this;
        DontDestroyOnLoad(gameObject);
    }



    private void Update()
    {
        m_GameTime = Time.time;
        if (m_GameTime > m_DifficultyChangeInterval && m_Difficulty < m_MaxDifficult)
        {
            Debug.Log(m_GameTime);
            m_Difficulty++;
            m_DifficultyChangeInterval += m_DifficultyFixChange;
            m_ZombieLifeMultiplier = m_BaseMultiplier + (m_Difficulty / 10);
            m_ZombieSpeedMultiplier = m_BaseMultiplier + (m_Difficulty / 10);
        }
    }

    public static GameManager GetGameManager()
    {
        return m_GameManager;
    }


    public PlayerController GetPlayer()
    {
        return m_Player;
    }
    public void SetPlayer(PlayerController Player)
    {
        m_Player = Player;
    }

    void AddCoins(int coinsCollected)
    {
        m_Coins += coinsCollected;
    }

    public float GetLifeMuliplier()
    {
        return m_ZombieLifeMultiplier;
    }

    public float GetSpeedMultiplier()
    {
        return m_ZombieSpeedMultiplier;
    }

    public float GetSpawnRateMultiplier()
    {
        return m_ZombieSpawnRateMultiplier;
    }
}
