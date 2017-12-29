using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public static class ConfigHandler {

    private const string CONFIG_PATH = "Resources/Config";
    private const string CONFIG_FILENAME = "LevelConfig.json";

    private const string RESOURCES_PATH = "Config";
    private const string RESOURCES_FILENAME = "LevelConfig";

    public static LevelConfigData LoadConfig()
    {
        string path = string.Format("{0}/{1}", RESOURCES_PATH, RESOURCES_FILENAME);
        LevelConfigData levelConfig = null;
        try
        {
            TextAsset text = Resources.Load<TextAsset>(path);
            string json = text.text;
            levelConfig = JsonUtility.FromJson<LevelConfigData>(json);
        }
        catch
        {
            Debug.Log("unable to load config");
        }
        return levelConfig;
    }

    public static void SaveConfig(LevelConfigData config)
    {
        string json = JsonUtility.ToJson(config, true);
        string savePath = string.Format("{0}/{1}/{2}", Application.dataPath, CONFIG_PATH, CONFIG_FILENAME);
        
        FileStream fs = new FileStream(savePath, FileMode.OpenOrCreate);
        StreamWriter writer = new StreamWriter(fs);

        writer.Write(json);
        writer.Close();
    }
}

[Serializable]
public class LevelConfigData
{
    /// <summary>
    /// Size in units of the grid (default unit is 100 pixels)
    /// </summary>
    public float GridSize;
}