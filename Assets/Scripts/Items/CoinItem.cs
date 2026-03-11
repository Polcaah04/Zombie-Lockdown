using UnityEngine;

public class CoinItem : Item
{
    [SerializeField] private int m_MinCoins = 5;
    [SerializeField] private int m_MaxCoins = 12;
    [SerializeField] private AudioClip m_coins;
    int m_CoinsAmmount;

    public override void Pick(PlayerController player)
    {
        AudioSource.PlayClipAtPoint(m_coins, transform.position);
        m_CoinsAmmount = Random.Range(m_MinCoins, m_MaxCoins);
        base.Pick(player);
        GameManager.GetGameManager().AddCoins(m_CoinsAmmount);
    }
}
