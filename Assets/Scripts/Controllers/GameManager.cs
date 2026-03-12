using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private TState m_LastState;

    //Objects
    static GameManager m_GameManager;
    PlayerController m_Player;
    CameraController m_Camera;
    UIManager m_UIManager;

    //Times
    private float m_RoundsDisplayedTime;
    private float m_RestDisplayedTime;
    [SerializeField] private float roundDuration = 60f;
    [SerializeField] private float restDuration = 30f;
    private float m_StateTimer = 0f;

    //BuffCoroutine
    public List<IEnumerator> m_BuffList = new List<IEnumerator>();
    public static event Action<string, bool> OnBuffObtained;

    //Difficulty
    private float m_Difficulty = 0;
    private int m_MaxDifficult = 10;
    private int m_BaseMultiplier = 1;

    // ZOMBIES
    private int m_CurrentZombies = 0;
    [SerializeField] private int m_MaxZombies = 30;
    private int m_ZombiesPerRound;
    private float m_ZombieSpeedBuffMultiplier = 1f;
    private List<Zombie> m_Zombies = new List<Zombie>(); //Only for buffs
    private List<ZombieSpawner> m_Spawners = new List<ZombieSpawner>(); //Only for buffs

    [HideInInspector] public int m_Coins { get; private set; } = 0;

    private float m_ZombieLifeMultiplier = 1;
    private float m_ZombieSpeedMultiplier = 1;
    private float m_ZombieSpawnRateMultiplier = 1;

    public event Action<PlayerController> OnPlayerReady;
    public event Action<int> OnCoinsChanged;
    public event Action<int> OnTimeChanged;

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
        m_ZombiesPerRound = 6;
    }

    private void Update()
    {
        m_StateTimer += Time.deltaTime;
        if (m_State == TState.PLAYINGROUNDS)
        {
            UpdateRound();
        }
        else if (m_State == TState.RESTING)
        {
            UpdateRest();
        }
    }
    public void WinGame()
    {
        m_State = TState.WIN;
        SceneManager.LoadScene("Win");
    }

    public void GameOver()
    {
        m_State = TState.GAMEOVER;
        SceneManager.LoadScene("GameOver");
    }

    void UpdateRound()
    {
        foreach (IEnumerator l_Coroutine in m_BuffList)
        {
            StartCoroutine(l_Coroutine);
        }
        m_BuffList.Clear();

        OnTimeChanged?.Invoke((int)m_StateTimer);

        if (m_StateTimer >= roundDuration)
        {
            ChangeState(TState.RESTING);
        }
    }

    void UpdateRest()
    {    
        if (m_CurrentZombies > 0)
        {
            m_StateTimer = 0f;
            return;
        }          

        OnTimeChanged?.Invoke((int)m_StateTimer);

        if (m_StateTimer >= restDuration)
        {
            StartNewRound();
        }
    }

    void ChangeState(TState newState)
    {
        m_LastState = m_State;
        m_State = newState;
        m_StateTimer = 0f;
    }

    void StartNewRound()
    {
        IncreaseDifficulty();
        ChangeState(TState.PLAYINGROUNDS);
    }

    void IncreaseDifficulty()
    {
        if (m_Difficulty >= m_MaxDifficult)
            return;

        m_Difficulty++;

        m_ZombieLifeMultiplier = m_BaseMultiplier + (m_Difficulty / 10f);
        m_ZombieSpeedMultiplier = m_BaseMultiplier + (m_Difficulty / 10f);

        if (m_ZombiesPerRound < m_MaxZombies)
        {
            m_ZombiesPerRound += (int)(m_BaseMultiplier + (m_Difficulty / 2f));

            if (m_ZombiesPerRound > m_MaxZombies)
                m_ZombiesPerRound = m_MaxZombies;
        }
    }

    // GAME MANAGER

    public static GameManager GetGameManager()
    {
        return m_GameManager;
    }

    public void ResetGame()
    {
        if (m_Player != null)
        {
            Destroy(m_Player.gameObject);
            m_Player = null;
        }
        Time.timeScale = 1f;

        m_State = TState.PLAYINGROUNDS;

        m_Coins = 0;
        m_CurrentZombies = 0;
        m_Difficulty = 0;

        m_RestDisplayedTime = 0;
        m_ZombiesPerRound = 6;
        m_ZombieLifeMultiplier = 1;
        m_ZombieSpeedMultiplier = 1;
        m_ZombieSpawnRateMultiplier = 1;
    }

    // PLAYER
    public PlayerController GetPlayer()
    {
        return m_Player;
    }
    public void SetPlayer(PlayerController Player)
    {
        m_Player = Player;
        OnPlayerReady?.Invoke(m_Player);
    }

    // CAMERA
    public CameraController GetCamera()
    {
        return m_Camera;
    }
    public void SetCamera(CameraController Camera)
    {
        m_Camera = Camera;
    }

    // SPAWNERS

    public void RegisterSpawner(ZombieSpawner spawner)
    {
        if (!m_Spawners.Contains(spawner))
            m_Spawners.Add(spawner);
    }

    public List<ZombieSpawner> GetSpawners()
    {
        return m_Spawners;
    }

    // ZOMBIES

    public void RegisterZombie(Zombie zombie)
    {
        m_Zombies.Add(zombie);
    }

    public void UnregisterZombie(Zombie zombie)
    {
        m_Zombies.Remove(zombie);
    }

    public List<Zombie> GetZombies()
    {
        return m_Zombies;
    }

    public bool CanSpawnZombie()
    {
        return m_CurrentZombies < m_ZombiesPerRound;
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
    public void BuffZombieSpeed(float multiplier)
    {
        m_ZombieSpeedBuffMultiplier = multiplier;
    }

    public float GetZombieSpeedBuff()
    {
        return m_ZombieSpeedBuffMultiplier;
    }

    public void BuffAllZombieSpeed(float multiplier)
    {
        m_ZombieSpeedBuffMultiplier = multiplier;
        foreach (Zombie zombie in m_Zombies)
        {
            zombie.BuffSpeed(multiplier);
        }
    }

    // COINS

    public void AddCoins(int coinsCollected)
    {
        m_Coins += coinsCollected;
        OnCoinsChanged?.Invoke(m_Coins);
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

    public float GetRoundTime()
    {
        return m_RoundsDisplayedTime;
    }
    public float GetCurrentRestTime()
    {
        return m_RestDisplayedTime;
    }

    //STATES 

    public TState GetState()
    {
        return m_State;
    }

    public void SetState(TState state)
    {
        m_State = state;
    }

    public TState GetPreviousState()
    {
        return m_LastState;
    }

    public void SetPreviousState(TState state)
    {
        m_LastState = state; 
    }

    //EVENTS
    public static void NotifyBuff(string buffName, bool isActivated)
    {
        OnBuffObtained?.Invoke(buffName, isActivated);
    }
}