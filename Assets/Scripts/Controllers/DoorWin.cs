using UnityEngine;

public class DoorWin : MonoBehaviour
{
    [SerializeField] private int m_Price = 500;

    private bool m_PlayerInside = false;

    private void Update()
    {
        if (m_PlayerInside && Input.GetKeyDown(KeyCode.E))
        {
            TryBuyDoor();
        }
    }

    private void TryBuyDoor()
    {
        if (GameManager.GetGameManager().GetCoins() >= m_Price)
        {
            GameManager.GetGameManager().WinGame();
        }
        else
        {
            Debug.Log("No tienes suficiente dinero");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            m_PlayerInside = true;
            Debug.Log("Pulsa E para abrir la puerta");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            m_PlayerInside = false;
        }
    }
}