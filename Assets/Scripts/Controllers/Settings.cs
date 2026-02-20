using System.IO;
using UnityEngine;

public class Settings
{
    //player controlls
    public static KeyCode m_ForwardKey = KeyCode.W;
    public static KeyCode m_BackwardKey = KeyCode.S;
    public static KeyCode m_LeftKey = KeyCode.A;
    public static KeyCode m_RightKey = KeyCode.D;
    public static KeyCode m_RunKey = KeyCode.LeftShift;
    public static KeyCode m_ReloadKey = KeyCode.R;
    public static KeyCode m_InteractKey = KeyCode.F;
    public static KeyCode m_PauseKey = KeyCode.Escape;
    /*public static KeyCode m_JumpKey = KeyCode.Space;
    public static float m_YawSpeed = 20.0f;
    public static float m_BaseYawSpeed = m_YawSpeed;
    public static float m_PitchSpeed = 20;
    public static float m_BasePitchSpeed = m_PitchSpeed;
    public static float m_SensValue;
    public static bool m_InvertedYaw = false;
    public static bool m_InvertedPitch = true;*/

    //path to save config
    private static string configPath = Application.persistentDataPath + "/config.ini";

    //on start/call, Loads config.ini 
    static Settings()
    {
        Load();
    }

    public static void Load()
    {
        //evades duplicating config.ini
        if (!File.Exists(configPath))
        {
            Save();
            return;
        }
        //reads all config.ini lines
        string[] lines = File.ReadAllLines(configPath);
        foreach (string line in lines)
        {
            if (line.StartsWith("Forward="))
                m_ForwardKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), line.Substring(8));
            if (line.StartsWith("Backward="))
                m_BackwardKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), line.Substring(9));
            if (line.StartsWith("Left="))
                m_LeftKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), line.Substring(5));
            if (line.StartsWith("Right="))
                m_RightKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), line.Substring(6));
            //if (line.StartsWith("Jump="))
                //m_JumpKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), line.Substring(5));
            if (line.StartsWith("Reload="))
                m_ReloadKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), line.Substring(7));
            if (line.StartsWith("Run="))
                m_RunKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), line.Substring(4));
            if (line.StartsWith("Interact="))
                m_InteractKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), line.Substring(9));
            /*if (line.StartsWith("Inventory="))
                m_InventoryKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), line.Substring(10));
            if (line.StartsWith("Pause="))
                m_PauseKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), line.Substring(6));
            if (line.StartsWith("XSens="))
                m_YawSpeed = float.Parse(line.Substring(6));
            if (line.StartsWith("YSens="))
                m_PitchSpeed = float.Parse(line.Substring(6));
            if (line.StartsWith("InvertX="))
                m_InvertedYaw = bool.Parse(line.Substring(8));
            if (line.StartsWith("InvertY="))
                m_InvertedPitch = bool.Parse(line.Substring(8));
            if (line.StartsWith("SensValue="))
                m_SensValue = float.Parse(line.Substring(10));*/
        }
    }

    public static void Save()
    {
        //saves new config given
        using (StreamWriter writer = new StreamWriter(configPath))
        {
            writer.WriteLine("[PLAYER CONTROLS]");
            writer.WriteLine("Forward=" + m_ForwardKey);
            writer.WriteLine("Backward=" + m_BackwardKey);
            writer.WriteLine("Left=" + m_LeftKey);
            writer.WriteLine("Right=" + m_RightKey);
            //writer.WriteLine("Jump=" + m_JumpKey);
            writer.WriteLine("Reload=" + m_ReloadKey);
            writer.WriteLine("Run=" + m_RunKey);
            writer.WriteLine("Interact=" + m_InteractKey);
            //writer.WriteLine("Inventory=" + m_InventoryKey);
            writer.WriteLine("Pause=" + m_PauseKey);
            /*writer.WriteLine("XSens=" + m_YawSpeed);
            writer.WriteLine("YSens=" + m_PitchSpeed);
            writer.WriteLine("InvertX=" + m_InvertedYaw);
            writer.WriteLine("InvertY=" + m_InvertedPitch);
            writer.WriteLine("The sens value varies from 1- 10");
            writer.WriteLine("SensValue=" + m_SensValue);*/
            writer.WriteLine("");
            writer.WriteLine("[GRAPHIC SETTINGS]");
            writer.WriteLine("");
            writer.WriteLine("[SOUND SETTINGS]");
            writer.WriteLine("");
        }
    }
}
