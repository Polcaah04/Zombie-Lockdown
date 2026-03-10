using System.Collections;
using TMPro.Examples;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Gambling : MonoBehaviour
{

    [SerializeField] private int m_Cost = 300;
    private float m_DistanceToInteract = 2.5f;  

    [Header("Player Basic Modifiers")]

    [SerializeField] private float m_LifeMultiplier = 1.2f;
    [SerializeField] private int m_DamageAdded = 5;
    [SerializeField] private float m_BuffSpeed = 1.15f;
    [SerializeField] private float m_NerfSpeed = 1.25f;
    [SerializeField] private float m_RegenDelayRatio = 0.5f;
    [SerializeField] private float m_RegenRateRatio = 0.6f;
    [SerializeField] private float m_SecondChanceLife = 0.65f;

    [Header("Gun Modifiers")]

    [SerializeField] private float m_FireRateReduction = 0.2f;
    [SerializeField] private float m_InfiniteFireRateValue = 0.1f;
    [SerializeField] private int m_MiniGunDamage = 20;
    [SerializeField] private int m_NerfAmmo = 2;

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
        float l_RandomBuffOrDebuff = Random.value;
        float l_RandomValue = Random.value;
        Debug.Log("Buff/Debuff value: " + l_RandomBuffOrDebuff);
        Debug.Log("Value: " + l_RandomValue);
        if (l_RandomBuffOrDebuff < 0.6)
        {
            Debug.Log("Chose buff.");
            if (l_RandomValue < 0.26)
            {
                GameManager.GetGameManager().GetPlayer().BuffHealthRegen(m_RegenDelayRatio, m_RegenRateRatio);
                GameManager.GetGameManager().m_BuffList.AddRange((System.Collections.Generic.IEnumerable<IEnumerator>)TimeCoroutine(30f, 1));
            }
            else if (l_RandomValue < 0.46)
            {
                GameManager.GetGameManager().GetPlayer().AddMaxLife(m_LifeMultiplier);
            }
            else if (l_RandomValue < 0.62)
            {
                GameManager.GetGameManager().GetPlayer().BuffDamage(m_DamageAdded);
                GameManager.GetGameManager().m_BuffList.AddRange((System.Collections.Generic.IEnumerable<IEnumerator>)TimeCoroutine(60f, 3));

            }
            else if (l_RandomValue < 68)
            {
                GameManager.GetGameManager().GetPlayer().BuffFireRate(m_FireRateReduction);
                GameManager.GetGameManager().m_BuffList.AddRange((System.Collections.Generic.IEnumerable<IEnumerator>)TimeCoroutine(60f, 4));
            }
            else if (l_RandomValue < 72)
            {
                GameManager.GetGameManager().GetPlayer().BuffSpeed(m_BuffSpeed);
                GameManager.GetGameManager().m_BuffList.AddRange((System.Collections.Generic.IEnumerable<IEnumerator>)TimeCoroutine(60f, 5));
            }
            else if (l_RandomValue < 0.76)
            {
                GameManager.GetGameManager().GetPlayer().InfiniteAmmo(m_InfiniteFireRateValue, m_MiniGunDamage);
                GameManager.GetGameManager().m_BuffList.AddRange((System.Collections.Generic.IEnumerable<IEnumerator>)TimeCoroutine(15f, 6));
            }
            else if (l_RandomValue < 0.88)
            {
                GameManager.GetGameManager().GetPlayer().MakeInvincible(true);
                GameManager.GetGameManager().m_BuffList.AddRange((System.Collections.Generic.IEnumerable<IEnumerator>)TimeCoroutine(4f, 7));
            }
            else if (l_RandomValue < 1)
            {
                GameManager.GetGameManager().GetPlayer().SecondChance(true, m_SecondChanceLife);
                GameManager.GetGameManager().m_BuffList.AddRange((System.Collections.Generic.IEnumerable<IEnumerator>)TimeCoroutine(30f, 8));
            }
        }
        else if (l_RandomBuffOrDebuff < 1)
        {
            Debug.Log("Chose debuff.");
            if (l_RandomValue < 0.2)
            {
                GameManager.GetGameManager().GetPlayer().NerfMaxAmmo(m_NerfAmmo);
                GameManager.GetGameManager().m_BuffList.AddRange((System.Collections.Generic.IEnumerable<IEnumerator>)TimeCoroutine(60f, 9));
            }
            else if (l_RandomValue < 0.4)
            {
                GameManager.GetGameManager().GetPlayer().NerfSpeed(m_NerfSpeed);
                GameManager.GetGameManager().m_BuffList.AddRange((System.Collections.Generic.IEnumerable<IEnumerator>)TimeCoroutine(60f, 10));
            }
            else if (l_RandomValue < 0.6)
            {
                //Zombies mas rapidos 10% (implementar en zombie)
                GameManager.GetGameManager().m_BuffList.AddRange((System.Collections.Generic.IEnumerable<IEnumerator>)TimeCoroutine(60f, 11));
            }
            else if (l_RandomValue < 0.8)
            {
                GameManager.GetGameManager().GetCamera().ReduceFOV();
                GameManager.GetGameManager().m_BuffList.AddRange((System.Collections.Generic.IEnumerable<IEnumerator>)TimeCoroutine(60f, 12));
            }
            else if (l_RandomValue < 1)
            {
                //Spawnrate de zombis aumentado (a 30 segundos?)
                GameManager.GetGameManager().m_BuffList.AddRange((System.Collections.Generic.IEnumerable<IEnumerator>)TimeCoroutine(60f, 13));
            }
        }
        
        

        
        
        
    }

    IEnumerator TimeCoroutine(float activeTime, int coroutineValue)
    {      
        while (activeTime > 0)
        {
            while (GameManager.GetGameManager().GetState() == GameManager.TState.PLAYINGROUNDS)
            {
                Debug.Log(activeTime);
                activeTime -= Time.deltaTime;
                yield return null;
            }   
            yield return null;
        }

        switch (coroutineValue)
        {
            case 1:

                break;
            case 2:

                break;
            case 3:

                break;
            case 4:

                break;
             case 5:

                break;
            case 6:

                break;
            case 7:

                break;
            case 8:

                break;
            case 9:

                break;
            case 10:

                break;
            case 11:

                break;
            case 12:

                break;
            case 13:

                break;
        }



    }
}
