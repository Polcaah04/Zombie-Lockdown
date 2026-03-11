using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Gambling : MonoBehaviour
{

    [SerializeField] private int m_Cost = 300;
    private float m_DistanceToInteract = 2.5f;
    [SerializeField] private AudioClip m_audio;


    [Header("Player Basic Modifiers")]

    [SerializeField] private float m_LifeMultiplier = 1.2f;
    [SerializeField] private int m_DamageAdded = 5;
    [SerializeField] private float m_BuffSpeed = 1.15f;
    [SerializeField] private float m_NerfSpeed = 1.25f;
    [SerializeField] private float m_RegenDelayRatio = 0.5f;
    [SerializeField] private float m_RegenRateRatio = 0.6f;

    [Header("Gun Modifiers")]

    [SerializeField] private float m_FireRateReduction = 0.1f;
    [SerializeField] private float m_InfiniteFireRateReduction = 0.2f;
    [SerializeField] private int m_MiniGunDamageBuff = 15;
    [SerializeField] private int m_NerfAmmo = 2;

    [Header("Zombie Modifiers")]
    [SerializeField] private float m_ZombieSpeedMultiplier = 1.1f;
    [SerializeField] private float m_BuffZombieSpawnRate = 0.6f;
    
    void Update()
    {       
        if (OnRange() &&Input.GetKeyDown(Settings.m_InteractKey))
        {
            Pay();
        }
    }

    void Pay()
    {
        if (GameManager.GetGameManager().GetCoins() > m_Cost)
        {
            GameManager.GetGameManager().AddCoins(-m_Cost);
            AudioSource.PlayClipAtPoint(m_audio, transform.position);
            RandomizeItem();
            
        }
        else if (GameManager.GetGameManager().GetCoins() < m_Cost)
        {
            Debug.Log("Not enough money");
        }
    }

    bool OnRange()
    {
        Vector2 l_Distance = new Vector2(transform.position.x - GameManager.GetGameManager().GetPlayer().transform.position.x, transform.position.y - GameManager.GetGameManager().GetPlayer().transform.position.y);
        if (l_Distance.magnitude < m_DistanceToInteract)
        {
            return true;
        }
        return false;
    }

    void RandomizeItem()
    {
        float l_RandomBuffOrDebuff = Random.value;
        float l_RandomValue = Random.value;
        if (l_RandomBuffOrDebuff < 1)
        {
            Debug.Log("Chose buff.");
            if (l_RandomValue < 1)
            {
                Debug.Log("Healing factor");
                GameManager.GetGameManager().GetPlayer().BuffHealthRegen(m_RegenDelayRatio, m_RegenRateRatio);
                GameManager.GetGameManager().m_BuffList.Add(TimeCoroutine(30f, 1));
            }
            else if (l_RandomValue < 2)
            {
                GameManager.GetGameManager().GetPlayer().AddMaxLife(m_LifeMultiplier);
            }
            else if (l_RandomValue < 2)
            {
                GameManager.GetGameManager().GetPlayer().BuffDamage(m_DamageAdded);
                GameManager.GetGameManager().m_BuffList.Add(TimeCoroutine(60f, 3));

            }
            else if (l_RandomValue < 2)
            {
                GameManager.GetGameManager().GetPlayer().BuffFireRate(m_FireRateReduction);
                GameManager.GetGameManager().m_BuffList.Add(TimeCoroutine(60f, 4));
            }
            else if (l_RandomValue < 2)
            {
                GameManager.GetGameManager().GetPlayer().BuffSpeed(m_BuffSpeed);
                GameManager.GetGameManager().m_BuffList.Add(TimeCoroutine(60f, 5));
            }
            else if (l_RandomValue < 2)
            {
                GameManager.GetGameManager().GetPlayer().InfiniteAmmo(m_InfiniteFireRateReduction, m_MiniGunDamageBuff);
                GameManager.GetGameManager().m_BuffList.Add(TimeCoroutine(15f, 6));
            }
            else if (l_RandomValue < 2)
            {
                GameManager.GetGameManager().GetPlayer().MakeInvincible(true);
                GameManager.GetGameManager().m_BuffList.Add(TimeCoroutine(20f, 7));
            }
            else if (l_RandomValue < 2)
            {
                GameManager.GetGameManager().GetPlayer().SecondChance(true);
                GameManager.GetGameManager().m_BuffList.Add(TimeCoroutine(30f, 8));
            }
        }
        else if (l_RandomBuffOrDebuff < 2)
        {
            Debug.Log("Chose debuff.");
            if (l_RandomValue < 0.2)
            {
                GameManager.GetGameManager().GetPlayer().NerfMaxAmmo(m_NerfAmmo);
                GameManager.GetGameManager().m_BuffList.Add(TimeCoroutine(60f, 9));
            }
            else if (l_RandomValue < 0.4)
            {
                GameManager.GetGameManager().GetPlayer().NerfSpeed(m_NerfSpeed);
                GameManager.GetGameManager().m_BuffList.Add(TimeCoroutine(60f, 10));
            }
            else if (l_RandomValue < 0.6)
            {
                GameManager.GetGameManager().BuffAllZombieSpeed(m_ZombieSpeedMultiplier);
                GameManager.GetGameManager().m_BuffList.Add(TimeCoroutine(60f, 11));
            }
            else if (l_RandomValue < 0.8)
            {
                GameManager.GetGameManager().GetCamera().ReduceFOV();
                GameManager.GetGameManager().m_BuffList.Add(TimeCoroutine(60f, 12));
            }
            else if (l_RandomValue < 1)
            {
                foreach (ZombieSpawner spawner in GameManager.GetGameManager().GetSpawners())
                {
                    spawner.IncreaseSpawnRate(m_BuffZombieSpawnRate);
                }
                GameManager.GetGameManager().m_BuffList.Add(TimeCoroutine(60f, 13));
            }
        }          
    }

    IEnumerator TimeCoroutine(float activeTime, int coroutineValue)
    {      
        while (activeTime > 0)
        {
            if(GameManager.GetGameManager().GetState() == GameManager.TState.PLAYINGROUNDS)
            {
                Debug.Log(activeTime);
                activeTime -= Time.deltaTime;
            }   
            yield return null;
        }
        Debug.Log("Finished coroutine");
        switch (coroutineValue)
        {
            case 1:
                Debug.Log("Normal health regen");
                GameManager.GetGameManager().GetPlayer().BuffHealthRegen(1/m_RegenDelayRatio, 1/m_RegenRateRatio);
                break;
            case 2:
                break;
            case 3:
                GameManager.GetGameManager().GetPlayer().BuffDamage(-m_DamageAdded);
                break;
            case 4:
                GameManager.GetGameManager().GetPlayer().BuffFireRate(-m_FireRateReduction);
                break;
             case 5:
                GameManager.GetGameManager().GetPlayer().BuffSpeed(1 / m_BuffSpeed);    
                break;
            case 6:
                GameManager.GetGameManager().GetPlayer().InfiniteAmmo(-m_InfiniteFireRateReduction, -m_MiniGunDamageBuff);
                break;
            case 7:
                GameManager.GetGameManager().GetPlayer().MakeInvincible(false);
                break;
            case 8:
                GameManager.GetGameManager().GetPlayer().SecondChance(false);
                break;
            case 9:
                GameManager.GetGameManager().GetPlayer().NerfMaxAmmo(1 / m_NerfAmmo);
                break;
            case 10:
                GameManager.GetGameManager().GetPlayer().NerfSpeed(1 / m_NerfSpeed);
                break;
            case 11:
                GameManager.GetGameManager().BuffAllZombieSpeed(1 / m_ZombieSpeedMultiplier);
                break;
            case 12:
                GameManager.GetGameManager().GetCamera().IncreaseFOV();
                break;
            case 13:
                foreach (ZombieSpawner spawner in GameManager.GetGameManager().GetSpawners())
                {
                    spawner.IncreaseSpawnRate(1/m_BuffZombieSpawnRate);
                }
                break;
        }
    }
}
