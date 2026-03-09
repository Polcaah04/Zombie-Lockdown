using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIWin : MonoBehaviour
{
    [SerializeField] private Button m_PlayAgain;
    [SerializeField] private Button m_ReturnToMainMenu;
    void Start()
    {
        m_PlayAgain.onClick.AddListener(PlayAgain);
        m_ReturnToMainMenu.onClick.AddListener(ReturnToMainMenu);
    }

    void PlayAgain()
    {
        GameManager.GetGameManager().ResetGame();
        SceneManager.LoadScene("Map 1");
    }

    void ReturnToMainMenu()
    {
        GameManager.GetGameManager().ResetGame();
        SceneManager.LoadScene("MainMenu");
    }
}
