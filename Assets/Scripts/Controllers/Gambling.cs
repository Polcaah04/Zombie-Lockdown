using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class Gambling : MonoBehaviour
{

    [SerializeField] private int m_Cost = 300;
    private float m_DistanceToInteract = 2.5f;
    

    [Header("Modifiers")]

    [SerializeField] private float m_LifeMultiplier = 1.2f;
    [SerializeField] private float m_FireRateValue = 0.1f;
    [SerializeField] private int m_MiniGunDamage = 20;
    [SerializeField] private int m_DamageAdded = 5;
    [SerializeField] private int m_DivideAmmoPerThis = 3;
    [SerializeField] private float m_DivideSpeedPerThis = 1.25f;
    
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

        if (l_RandomValue < 0.1)
        {
            GameManager.GetGameManager().GetPlayer().AddMaxLife(m_LifeMultiplier);
        }
        else if (l_RandomValue < 0.2)
        {
            GameManager.GetGameManager().GetPlayer().SwitchMiniGun(m_FireRateValue, m_MiniGunDamage);
        }
        else if (l_RandomValue < 0.3)
        {
            GameManager.GetGameManager().GetPlayer().BuffDamage(m_DamageAdded);
        }
        else if (l_RandomValue < 0.4)
        {
            GameManager.GetGameManager().GetPlayer().MakeInvincible();
        }
        else if (l_RandomValue < 0.5)
        {
            GameManager.GetGameManager().GetPlayer().SecondChance();
        }
        else if (l_RandomValue < 0.6)
        {
            GameManager.GetGameManager().GetPlayer().NerfMaxAmmo(m_DivideAmmoPerThis);
        }
        else if (l_RandomValue < 0.7)
        {
            GameManager.GetGameManager().GetPlayer().NerfSpeed(m_DivideSpeedPerThis);
        }
        else if (l_RandomValue < 0.8)
        {
            //Zombies mas rapidos 10% (implementar en zombie)
        }
        else if (l_RandomValue < 0.9)
        {
            //FOV reducido a un 70% (implementar en cam)
        }
        else if (l_RandomValue < 1)
        {
            //Crear mecanica Dash, cada vez que toque la mejora se debe reducir el cooldown del dash en 1s (empieza en 10s cooldown)
        }
        else if (l_RandomValue < 1.1)
        {
            //Spawnrate de zombis aumentado (a 30 segundos?)
        }
    }

    IEnumerator BuffCoroutine(int activeTime)
    {
        yield return null;
    }
}
