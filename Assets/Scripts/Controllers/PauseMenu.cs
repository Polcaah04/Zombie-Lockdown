using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static GameManager;

public class PauseMenu : MonoBehaviour
{
    private bool m_Paused = false;
    private bool m_IsOptions = false;
    private bool m_IsConfirmingExit = false;


    [Header("Panels")]
    [SerializeField] private GameObject m_PausePanel;
    [SerializeField] private GameObject m_OptionsPanel;
    [SerializeField] private GameObject m_ControlsPanel;
    [SerializeField] private GameObject m_SoundsPanel;
    [SerializeField] private GameObject m_ConfirmExitPanel;

    [Header("Buttons")]
    [SerializeField] private Button m_Resume;
    [SerializeField] private Button m_Leave;
    [SerializeField] private Button m_Options;
    [SerializeField] private Button m_LeaveSettingsButton;
    [SerializeField] private Button m_ControlsButton;
    [SerializeField] private Button m_SoundButton;
    [SerializeField] private Button m_SaveSettingsButton;
    [SerializeField] private Button m_Quit;
    [SerializeField] private Button m_NegateQuit;

    //to bind new key controls correctly
    Button m_CurrentBindingButton = null;
    bool m_WaitingForKey = false;

    //lists for objects in controlsPanel
    List<RectTransform> textList = new List<RectTransform>();
    List<RectTransform> inputList = new List<RectTransform>();



    void Start()
    {
        m_Resume.onClick.AddListener(Resume);
        m_Quit.onClick.AddListener(ConfirmExit);
        m_NegateQuit.onClick.AddListener(NegateExit);
        m_Leave.onClick.AddListener(OnLeave);
        m_Options.onClick.AddListener(OnOptions);
        m_LeaveSettingsButton.onClick.AddListener(ReturnToPauseMenu);
        m_ControlsButton.onClick.AddListener(ControlsPanel);
        m_SoundButton.onClick.AddListener(SoundsPanel);
        if (GameManager.GetGameManager().GetPlayer() != null)
        {
            m_PausePanel.SetActive(false);
            m_OptionsPanel.SetActive(false);
            m_ConfirmExitPanel.SetActive(false);
        }

        foreach (Transform child in m_ControlsPanel.transform)
        {
            //gets child texts on ControlsPanel and adds them to textList
            TMPro.TextMeshProUGUI l_Text = child.GetComponent<TMPro.TextMeshProUGUI>();
            if (l_Text != null)
            {
                textList.Add(child.GetComponent<RectTransform>());
            }
            //gets child buttons on ControlsPanel and adds them to inputList
            Button l_Input = child.GetComponent<Button>();
            if (l_Input != null)
            {
                l_Input.onClick.AddListener(OnButtonKeyBind);
                inputList.Add(child.GetComponent<RectTransform>());
            }
        }

        for (int i = 0; i < inputList.Count; i++)
        {
            //gets the text from each TextMesh in controlspanel, and then asigns the respective key to its respective button using function GetKeyFromSettings
            string controlName = textList[i].GetComponent<TMPro.TextMeshProUGUI>().text;
            KeyCode key = GetKeyFromSettings(controlName);
            TMPro.TextMeshProUGUI btnText = inputList[i].GetComponent<Button>().GetComponentInChildren<TMPro.TextMeshProUGUI>();
            btnText.text = key.ToString();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(Settings.m_PauseKey))
        {       
            if (!m_Paused && !m_IsOptions && !m_IsConfirmingExit)
            {
                ShowPanels(m_PausePanel, true);
                Cursor.lockState = CursorLockMode.Confined;
                Pause();
            }
            else if (m_Paused && !m_IsOptions && !m_IsConfirmingExit)
            {
                ShowPanels(m_PausePanel, false);
                Cursor.lockState = CursorLockMode.Locked;
                Pause();
            }
            else if (!m_Paused && m_IsOptions && !m_IsConfirmingExit)
            {
                ShowPanels(m_OptionsPanel, false);
                ShowPanels(m_PausePanel, true);
                Cursor.lockState = CursorLockMode.Confined;
            }
            else if (m_Paused && !m_IsOptions && m_IsConfirmingExit)
            {
                ShowPanels(m_ConfirmExitPanel, false);
                ShowPanels(m_PausePanel, true);
                Cursor.lockState = CursorLockMode.Confined;
            }
        }
    }

    void Pause()
    {
        if (GameManager.GetGameManager().GetState() == GameManager.TState.PAUSED)
        {
            GameManager.GetGameManager().SetState(GameManager.GetGameManager().GetPreviousState());
            Time.timeScale = 1f;
        }
        else if (GameManager.GetGameManager().GetState() != TState.WIN && GameManager.GetGameManager().GetState() != TState.GAMEOVER)
        {
            GameManager.GetGameManager().SetPreviousState(GameManager.GetGameManager().GetState());
            Debug.Log(GameManager.GetGameManager().GetPreviousState());
            GameManager.GetGameManager().SetState(GameManager.TState.PAUSED);
            Time.timeScale = 0f;
        }
    }

    void ShowPanels(GameObject panel, bool updated)
    {
        if (panel.name == "Pause_Panel")
        {
            m_Paused = updated;
        }
        else if (panel.name == "Options_Panel")
        {
            m_IsOptions = updated;
        }
        else if (panel.name == "ConfirmExit_Panel")
            m_IsConfirmingExit = updated;
        panel.SetActive(updated);
    }

    void Resume()
    {
        ShowPanels(m_PausePanel, false);
        ShowPanels(m_OptionsPanel,false);
        ShowPanels(m_ConfirmExitPanel, false);
        Pause();
        Cursor.lockState = CursorLockMode.Confined;
    }

    void OnLeave()
    {
        ShowPanels(m_ConfirmExitPanel, true);
        ShowPanels(m_PausePanel, true);
        Cursor.lockState = CursorLockMode.Confined;
    }

    void ConfirmExit()
    {
        GameManager.GetGameManager().ResetGame();
        SceneManager.LoadScene("MainMenu");
    }

    void NegateExit()
    {
        ShowPanels(m_ConfirmExitPanel, false);
        ShowPanels(m_PausePanel, true);
        Cursor.lockState = CursorLockMode.Confined;
    }

    void OnOptions()
    {
        ShowPanels(m_PausePanel, false);
        ShowPanels(m_OptionsPanel, true);
        Cursor.lockState = CursorLockMode.Confined;
    }

    void ReturnToPauseMenu()
    {
        ShowPanels(m_OptionsPanel, false);
        ShowPanels(m_PausePanel, true);
        Cursor.lockState = CursorLockMode.Confined;
    }

    void ControlsPanel()
    {
        m_ControlsPanel.SetActive(true);
        m_SoundsPanel.SetActive(false);
    }

    void SoundsPanel()
    {
        m_ControlsPanel.SetActive(false);
        m_SoundsPanel.SetActive(true);
    }

    void OnButtonKeyBind()
    {
        //gets the selected button component, saves the actual key, and waits for WaitForKeyPress
        m_CurrentBindingButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Button>();

        if (m_CurrentBindingButton == null)
            return;
        m_WaitingForKey = true;
        string m_LastKeyBind = m_CurrentBindingButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text;
        KeyCode lastKeyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), m_LastKeyBind);
        m_CurrentBindingButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "...";

        StartCoroutine(WaitForKeyPress(lastKeyCode));
    }

    IEnumerator WaitForKeyPress(KeyCode lastKey)
    {
        //waits to press a key, gets the pressed key, and if its Escape or mouse buttons, returns to the last keybind, else, gets the new key
        while (m_WaitingForKey)
        {
            if (Input.anyKeyDown)
            {
                foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(key))
                    {
                        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButton(0) || Input.GetMouseButton(1))
                        {
                            AssignKeyToSetting(lastKey);
                            m_WaitingForKey = false;
                        }
                        else
                        {
                            AssignKeyToSetting(key);
                            m_WaitingForKey = false;
                        }
                        break;
                    }
                }
            }
            yield return null;
        }
    }

    void AssignKeyToSetting(KeyCode newKey)
    {

        if (m_CurrentBindingButton == null)
            return;
        //gets the text from the TextMesh gameobject respective to the button selected, compares it, and sends the new key to its respective setting
        string controlName = m_CurrentBindingButton.transform.parent.GetComponentInChildren<TMPro.TextMeshProUGUI>().text;
        switch (controlName)
        {
            case "Forward": Settings.m_ForwardKey = newKey; break;
            case "Backward": Settings.m_BackwardKey = newKey; break;
            case "Left": Settings.m_LeftKey = newKey; break;
            case "Right": Settings.m_RightKey = newKey; break;
            case "Run": Settings.m_RunKey = newKey; break;
            case "Reload": Settings.m_ReloadKey = newKey; break;
            //case "Jump": Settings.m_JumpKey = newKey; break;
            case "Interact": Settings.m_InteractKey = newKey; break;
        }
        //sets the text of the button to the new keybind
        m_CurrentBindingButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = newKey.ToString();
    }
    KeyCode GetKeyFromSettings(string controlName)
    {
        //gets the keybind from settings and sends it to the call of the function
        switch (controlName)
        {
            case "Forward": return Settings.m_ForwardKey;
            case "Backward": return Settings.m_BackwardKey;
            case "Left": return Settings.m_LeftKey;
            case "Right": return Settings.m_RightKey;
            case "Run": return Settings.m_RunKey;
            case "Reload": return Settings.m_ReloadKey;
            //case "Jump": return Settings.m_JumpKey;
            case "Interact": return Settings.m_InteractKey;
        }
        return KeyCode.None;
    }
}
