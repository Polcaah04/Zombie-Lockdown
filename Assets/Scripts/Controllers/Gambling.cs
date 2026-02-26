using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class Gambling : MonoBehaviour
{

    [SerializeField] private int m_Cost = 300;

    
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
        return false;
    }

    void RandomizeItem()
    {
        float l_RandomValue = Random.value;
        if (l_RandomValue < 0.1)
        {
            GameManager.GetGameManager().GetPlayer().AddMaxLife();
        }
        else if (l_RandomValue < 0.2)
        {
            GameManager.GetGameManager().GetPlayer().SwitchMiniGun();
        }
        else if (l_RandomValue < 0.3)
        {

        }
        else if (l_RandomValue < 0.4)
        {
            
        }
        else if (l_RandomValue < 0.5)
        {

        }
        else if (l_RandomValue < 0.6)
        {
            
        }
        else if (l_RandomValue < 0.7)
        {

        }
        else if (l_RandomValue < 0.8)
        {

        }
        else if (l_RandomValue < 0.9)
        {

        }
        else if (l_RandomValue < 1)
        {

        }
        else if (l_RandomValue < 1.1)
        {

        }
    }

    IEnumerator BuffCoroutine(int activeTime)
    {
        yield return null;
    }
}
