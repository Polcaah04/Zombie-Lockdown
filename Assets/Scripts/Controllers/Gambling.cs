using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class Gambling : MonoBehaviour
{

    [SerializeField] private int m_Cost = 300;
    private float m_DistanceToInteract = 2.5f;  

    [Header("Player Basic Modifiers")]

    [SerializeField] private float m_LifeMultiplier = 1.2f;
    [SerializeField] private int m_DamageAdded = 5;
    [SerializeField] private float m_DivideSpeedPerThis = 1.25f;
    [SerializeField] private float m_RegenDelayRatio = 0.5f;
    [SerializeField] private float m_RegenRateRatio = 0.6f;
    [SerializeField] private float m_SecondChanceLife = 0.65f;

    [Header ("Gun Modifiers")]

    [SerializeField] private float m_FireRateValue = 0.1f;
    [SerializeField] private int m_MiniGunDamage = 20;
    [SerializeField] private int m_DivideAmmoPerThis = 3;

    [Header("Zombie Modifiers")]
    [SerializeField] private float m_ZombieSpeedMultiplier = 1.1f;
    void Start()
    {
        
    }

    
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
        float l_RandomValue = Random.value;

        if (l_RandomValue < 0.18)
        {
            GameManager.GetGameManager().GetPlayer().AddMaxLife(m_LifeMultiplier);
            StartCoroutine(TimeCoroutine(60f));
        }
        else if (l_RandomValue < 0.34)
        {
            GameManager.GetGameManager().GetPlayer().InfiniteAmmo(m_FireRateValue, m_MiniGunDamage);
            StartCoroutine(TimeCoroutine(60f));
        }
        else if (l_RandomValue < 0.48)
        {
            GameManager.GetGameManager().GetPlayer().BuffDamage(m_DamageAdded);
            StartCoroutine(TimeCoroutine(60f));
        }
        else if (l_RandomValue < 0.62)
        {
            GameManager.GetGameManager().GetPlayer().MakeInvincible(true);
            StartCoroutine(TimeCoroutine(4f));
        }
        else if (l_RandomValue < 0.74)
        {
            GameManager.GetGameManager().GetPlayer().SecondChance(true, m_SecondChanceLife);
            StartCoroutine(TimeCoroutine(30f));
        }
        else if (l_RandomValue < 0.84)
        {
            GameManager.GetGameManager().GetPlayer().BuffHealthRegen(m_RegenDelayRatio, m_RegenRateRatio);
            StartCoroutine(TimeCoroutine(60f));
        }
        else if (l_RandomValue < 0.92)
        {
            GameManager.GetGameManager().GetPlayer().NerfMaxAmmo(m_DivideAmmoPerThis);
        }
        else if (l_RandomValue < 0.1)
        {
            GameManager.GetGameManager().GetPlayer().NerfSpeed(m_DivideSpeedPerThis);
        }
        else if (l_RandomValue < 1.1)
        {
            //Zombies mas rapidos 10% (implementar en zombie)
            StartCoroutine(TimeCoroutine(60f));
        }
        else if (l_RandomValue < 1.3)
        {
            //FOV reducido a un 70% (implementar en cam)
            StartCoroutine(TimeCoroutine(60f));
        }
        else if (l_RandomValue < 1.1)
        {
            //Spawnrate de zombis aumentado (a 30 segundos?)
            StartCoroutine(TimeCoroutine(60f));
        }
    }

    IEnumerator TimeCoroutine(float activeTime)
    {      
        while (activeTime > 0)
        {
            while (GameManager.GetGameManager().GetState() == GameManager.TState.PLAYINGROUNDS)
            {
                yield return null;
            }
            activeTime -= Time.deltaTime;
            yield return null;
        }
    }
}
