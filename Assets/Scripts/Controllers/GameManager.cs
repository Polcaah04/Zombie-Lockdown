using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum TState
    {
        PLAYINGROUNDS = 0,
        RESTING,
        WIN,
        GAMEOVER,
        PAUSED
    }
    private TState m_State;

    //Objects
    static GameManager m_GameManager;
    PlayerController m_Player;

    //Times
    private float m_GameTime;
    private float m_RoundsTime;
    private float m_RestingTime;
    int m_RestingChangeInterval = 30;
    int m_DifficultyChangeInterval = 60;
    int m_DifficultyFixChange = 60;
    int m_RestingFixChange = 30;
    float m_RestDisplayedTime;

    //Difficulty
    float m_Difficulty = 0;
    int m_MaxDifficult = 10;
    int m_BaseMultiplier = 1;

    public int m_Coins { get; private set; } = 0;

    private float m_ZombieLifeMultiplier = 1;
    private float m_ZombieSpeedMultiplier = 1;
    private float m_ZombieSpawnRateMultiplier = 1;
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

    private void Start()
    {
        m_State = TState.PLAYINGROUNDS;
    }



    private void Update()
    {
        m_GameTime = Time.time;

        if (Input.GetKeyDown(Settings.m_PauseKey))
        {
            Pause();
        }

        if (m_State == TState.PLAYINGROUNDS)
        {
            m_RoundsTime = m_GameTime - m_RestingTime;
            if (m_RoundsTime > m_DifficultyChangeInterval && m_Difficulty < m_MaxDifficult)
            {
                m_Difficulty++;
                m_DifficultyChangeInterval += m_DifficultyFixChange;
                m_ZombieLifeMultiplier = m_BaseMultiplier + (m_Difficulty / 10);
                m_ZombieSpeedMultiplier = m_BaseMultiplier + (m_Difficulty / 10);
                m_State = TState.RESTING;             
            }
        }
        else if (m_State == TState.RESTING)
        {
            m_RestingTime = m_GameTime - m_RoundsTime;
            m_RestDisplayedTime += Time.deltaTime;
            if (m_RestingTime > m_RestingChangeInterval)
            {
                m_RestDisplayedTime = 0;
                m_RestingChangeInterval += m_RestingFixChange;
                m_State = TState.PLAYINGROUNDS;
            }
        }

        if(m_State == TState.WIN)
        {
            m_Player.enabled = false;
        }
        else if (m_State == TState.GAMEOVER)
        {
            m_Player.enabled = false;
        }
    }


    // GAME MANAGER
    public static GameManager GetGameManager()
    {
        return m_GameManager;
    }
    void Pause()
    {
        if (m_State == TState.PAUSED)
        {
            m_State = TState.PLAYINGROUNDS;
            Time.timeScale = 1f;
        }
        else if (m_State != TState.WIN && m_State != TState.GAMEOVER)
        {
            m_State = TState.PAUSED;
            Time.timeScale = 0f;
        }
    }


    // PLAYER
    public PlayerController GetPlayer()
    {
        return m_Player;
    }
    public void SetPlayer(PlayerController Player)
    {
        m_Player = Player;
    }


    // ZOMBIES
    private int m_CurrentZombies = 0;
    [SerializeField] private int m_MaxZombies = 30;

    public bool CanSpawnZombie()
    {
        return m_CurrentZombies < m_MaxZombies;
    }
    public void RegisterZombieSpawn()
    {
        m_CurrentZombies++;
    }
    public void RegisterZombieDeath()
    {
        m_CurrentZombies--;
        if (m_CurrentZombies < 0)
            m_CurrentZombies = 0;
    }


    // COINS
    public void AddCoins(int coinsCollected)
    {
        m_Coins += coinsCollected;
    }
    public int GetCoins()
    {
        return m_Coins;
    }


    // MULTIPLIERS
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

    // TIME

    public float GetGameTime()
    {
        return m_GameTime;
    }
    public float GetRoundTime()
    {
        return m_RoundsTime;
    }
    public float GetCurrentRestTime()
    {
        return m_RestDisplayedTime;
    }
    public TState GetState()
    {
        return m_State;
    }
}