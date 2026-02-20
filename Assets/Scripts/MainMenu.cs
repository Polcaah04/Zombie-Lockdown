using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Main Menu Buttons")]
    [SerializeField] private Button m_StartButton;
    [SerializeField] private Button m_OptionsButton;
    [SerializeField] private Button m_LeaveButton;

    //[Header("Config Play Buttons")]
    //[SerializeField] private Button m_CreatePlayButton;
    //[SerializeField] private Button m_LeaveConfigPlayButton;

    [Header("Options Menu UI")]
    [SerializeField] private Button m_ControlsButton;
    [SerializeField] private Button m_SoundButton;
    //[SerializeField] private Button m_GraphicsButton;
    [SerializeField] private Button m_SaveSettingsButton;
    [SerializeField] private Button m_LeaveSettingsButton;

    [Header("Panels")]
    [SerializeField] private GameObject m_MainMenu; //not a panel
    //[SerializeField] private GameObject m_CreatePanel;
    [SerializeField] private GameObject m_OptionsMenu; //not a panel
    [SerializeField] private GameObject m_ControlsPanel;
    [SerializeField] private GameObject m_SoundsPanel;
    //[SerializeField] private GameObject m_GraphicsPanel;

    //lists for objects in controlsPanel
    List<RectTransform> textList = new List<RectTransform>();
    List<RectTransform> inputList = new List<RectTransform>();
    List<RectTransform> sliderList = new List<RectTransform>();

    //to bind new key controls correctly
    Button m_CurrentBindingButton = null;
    bool m_WaitingForKey = false;


    void Start()
    {
        m_MainMenu.SetActive(true);
        //m_CreatePanel.SetActive(false);
        m_OptionsMenu.SetActive(false);
        m_ControlsPanel.SetActive(false);
        m_SoundsPanel.SetActive(false);
        //m_GraphicsPanel.SetActive(false);

        AddListeners();
    }

    //Main Menu Functions
    void OnStartGame()
    {
        m_MainMenu.SetActive(false);
        SceneManager.LoadScene("Map 1");
    }
    void OnOpenSettings()
    {
        m_MainMenu.SetActive(false);
        m_OptionsMenu.SetActive(true);
        m_ControlsPanel.SetActive(true);
    }
    void OnLeaveGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // Create Game Menu Functions
    /*void OnCreate()
    {
        SceneManager.LoadScene("Prologue");
    }
    void OnLeaveConfigPlay()
    {
        m_MainMenu.SetActive(true);
        //m_CreatePanel.SetActive(false);
    }*/

    // Settings Menu Functions
    void OnOpenControls()
    {

        m_ControlsPanel.SetActive(true);
        m_SoundsPanel.SetActive(false);
        //m_GraphicsPanel.SetActive(false);
    }
    void OnOpenSound()
    {
        m_ControlsPanel.SetActive(false);
        m_SoundsPanel.SetActive(true);
        //m_GraphicsPanel.SetActive(false);
    }
    void OnOpenGraphics()
    {
        m_ControlsPanel.SetActive(false);
        m_SoundsPanel.SetActive(false);
        //m_GraphicsPanel.SetActive(true);
    }
    void OnLeaveSettings()
    {
        m_MainMenu.SetActive(true);
        m_OptionsMenu.SetActive(false);
    }
    void OnSaveSettings()
    {
        Settings.Save();
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

    void OnSensitivityChange(float value)
    {
        //Settings.m_YawSpeed = value * Settings.m_BaseYawSpeed;
        //Settings.m_PitchSpeed = value * Settings.m_BasePitchSpeed;
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
            case "Crouch": Settings.m_ReloadKey = newKey; break;
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
            case "Crouch": return Settings.m_ReloadKey;
            //case "Jump": return Settings.m_JumpKey;
            case "Interact": return Settings.m_InteractKey;
        }
        return KeyCode.None;
    }

    private void AddListeners()
    {
        //MainMenu Listeners
        m_StartButton.onClick.AddListener(OnStartGame);
        m_OptionsButton.onClick.AddListener(OnOpenSettings);
        m_LeaveButton.onClick.AddListener(OnLeaveGame);

        //Create Game Menu Listeners
        //m_CreatePlayButton.onClick.AddListener(OnCreate);
        //m_LeaveConfigPlayButton.onClick.AddListener(OnLeaveConfigPlay);

        //Settings Menu Listeners
        m_ControlsButton.onClick.AddListener(OnOpenControls);
        m_SoundButton.onClick.AddListener(OnOpenSound);
        //m_GraphicsButton.onClick.AddListener(OnOpenGraphics);
        m_LeaveSettingsButton.onClick.AddListener(OnLeaveSettings);
        m_SaveSettingsButton.onClick.AddListener(OnSaveSettings);
    }

    
}
