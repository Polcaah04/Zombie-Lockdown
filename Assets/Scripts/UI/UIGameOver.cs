using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIGameOver : MonoBehaviour
{
    [SerializeField] private Button m_Retry;
    [SerializeField] private Button m_ReturnToMainMenu;
    void Start()
    {
        m_Retry.onClick.AddListener(Retry);
        m_ReturnToMainMenu.onClick.AddListener(ReturnMainMenu);
    }

    void Retry()
    {
        GameManager.GetGameManager().ResetGame();
        SceneManager.LoadScene("Map 1");
    }

    void ReturnMainMenu()
    {
        GameManager.GetGameManager().ResetGame();
        SceneManager.LoadScene("MainMenu");
    }


}
