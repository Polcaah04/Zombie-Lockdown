using UnityEngine;

public class CoinItem : Item
{
    [SerializeField] private int m_MinCoins = 5;
    [SerializeField] private int m_MaxCoins = 12;
    int m_CoinsAmmount;

    public override void Pick(PlayerController player)
    {
        m_CoinsAmmount = Random.Range(m_MinCoins, m_MaxCoins);
        base.Pick(player);
        GameManager.GetGameManager().AddCoins(m_CoinsAmmount);
    }
}
