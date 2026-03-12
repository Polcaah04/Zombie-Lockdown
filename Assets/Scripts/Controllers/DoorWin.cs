using UnityEngine;
using TMPro;

public class DoorWin : MonoBehaviour
{
    [SerializeField] private int m_Price = 500;

    private bool m_PlayerInside = false;
    [SerializeField] private TextMeshProUGUI interactText;

    private void Update()
    {
        if (m_PlayerInside && Input.GetKeyDown(Settings.m_InteractKey))
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
            if (interactText != null)
            {
                interactText.gameObject.SetActive(true);
                interactText.text = $"[E] Interactuar - Precio: {m_Price}";
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            m_PlayerInside = false;
            if (interactText != null)
                interactText.gameObject.SetActive(false);
        }
    }
}