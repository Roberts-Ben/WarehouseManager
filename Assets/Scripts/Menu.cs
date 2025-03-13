using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System;
using System.IO;
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;

public class Menu : MonoBehaviour
{
    static public Menu instance;

    public GameObject canvas;
    public GameObject mainPanel;
    public GameObject optionsPanel;
    public GameObject levelPanel;
    public InfoPopup popupHandler;

    public GameObject[] levelButtonObjects;
    public List<LevelInfo> levelInfos;

    public string levelFile;
    public int selectedLevel;

    private readonly string dataPath = Application.dataPath + "/Data/LevelData.json";
    private readonly string settingsPath = Application.dataPath + "/Data/SettingsData.json";

    public List<LevelSaveData> levelSaveData;

    public GameObject audioButton;
    public List<Sprite> audioSprites;
    public bool audioEnabled;

    public GameObject menuCamera;
    public GameObject gameCamera;
    public GameObject backgroundMap;

    public void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        optionsPanel.SetActive(false);
        levelPanel.SetActive(false);
    }

    public void Play()
    {
        mainPanel.SetActive(false);
        levelPanel.SetActive(true);
        LoadAndSortLevels();
    }
    public void Options()
    {
        mainPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Back()
    {
        mainPanel.SetActive(true);
        optionsPanel.SetActive(false);
        levelPanel.SetActive(false);
        popupHandler.ClosePopup();
    }

    public void HowToPlayButton()
    {
        popupHandler.DisplayPopup(POPUPTYPE.HOWTOPLAY);
    }
    public void ResetDataButton()
    {
        popupHandler.DisplayPopup(POPUPTYPE.CONFIRMDATARESET);
    }

    public void ResetProgress()
    {
        Debug.LogWarning("Resetting prefs");
        foreach (LevelInfo levelInfo in levelInfos)
        {
            if (levelInfo.WorldID == 1 && levelInfo.LevelID == 1)
            {
                Debug.Log("Resetting level 1");
                levelInfo.LevelUnlocked = true;
                levelInfo.LevelCompleted = false;
            }
            else
            {
                levelInfo.LevelUnlocked = false;
                levelInfo.LevelCompleted = false;
            }
        }
        SavePrefs();
    }

    public void LoadAndSortLevels()
    {
        levelInfos.Clear();

        levelButtonObjects = GameObject.FindGameObjectsWithTag("LevelButton");
        foreach (GameObject levelButtonObject in levelButtonObjects)
        {
            LevelInfo levelInfo = levelButtonObject.GetComponent<LevelInfo>();
            levelInfos.Add(levelInfo);

            string buttonName = levelButtonObject.name;
            int[] IDs = Array.ConvertAll(buttonName.Split('-'), int.Parse);

            levelInfo.WorldID = IDs[0];
            levelInfo.LevelID = IDs[1];
        }

        levelInfos = levelInfos.OrderBy(w => w.WorldID).ThenBy(l => l.LevelID).ToList();
        
        LoadPrefs();

        for(int i = 0; i < levelSaveData.Count(); i++)
        {
            Button newBtn = levelInfos[i].GetComponent<Button>();
            TMP_Text levelText = newBtn.GetComponentInChildren<TMP_Text>();
            Image[] levelCompleteImage = newBtn.GetComponentsInChildren<Image>();
            
            newBtn.interactable = levelSaveData[i].levelUnlocked;
            levelCompleteImage[1].enabled = levelSaveData[i].levelCompleted;
            levelText.enabled = !levelSaveData[i].levelCompleted;
        }
    }

   public void LoadLevel(LevelInfo levelInfo)
    {
        levelFile = levelInfo.WorldID + "-" + levelInfo.LevelID;
        selectedLevel = (levelInfo.WorldID * levelInfo.LevelID) - 1;
        canvas.SetActive(false);

        SceneManager.LoadScene("Level", LoadSceneMode.Additive);
        backgroundMap.SetActive(false);
    }
    public void LevelComplete()
    {
        backgroundMap.SetActive(true);
        menuCamera.SetActive(true);
        gameCamera.SetActive(false);

        if (!levelInfos[selectedLevel].LevelCompleted)
        {
            levelInfos[selectedLevel].LevelCompleted = true;
            levelInfos[selectedLevel].LevelUnlocked = true;
            levelInfos[selectedLevel + 1].LevelUnlocked = true;
        }
        SavePrefs();
        canvas.SetActive(true);
        LoadAndSortLevels();
    }

    public void ToggleAudio()
    {
        audioEnabled = !audioEnabled;
        audioButton.GetComponent<Image>().sprite = audioSprites[audioEnabled ? 0 : 1];

    }
    public void LoadPrefs()
    {
        Debug.Log("Loading prefs");
        if (File.Exists(dataPath))
        {
            string str = File.ReadAllText(dataPath);
            levelSaveData = JsonConvert.DeserializeObject<List<LevelSaveData>>(str);

            for(int i = 0; i < levelInfos.Count; i++)
            {
                levelInfos[i].LevelUnlocked = levelSaveData[i].levelUnlocked;
                levelInfos[i].LevelCompleted = levelSaveData[i].levelCompleted;
            }
        }
        else
        {
            InitPrefs();
        }

        audioButton.GetComponent<Image>().sprite = audioSprites[audioEnabled ? 0 : 1];
    }

    public void InitPrefs()
    {
        Debug.Log("Prefs not found");
        levelSaveData.Clear();
        ResetProgress();
        foreach (LevelInfo levelInfo in levelInfos)
        {
            LevelSaveData levelData = new()
            {
                levelID = (10 * (levelInfo.WorldID - 1)) + levelInfo.LevelID,
                levelUnlocked = levelInfo.LevelUnlocked,
                levelCompleted = levelInfo.LevelCompleted
            };
            levelSaveData.Add(levelData);
        }
        string json = JsonConvert.SerializeObject(levelSaveData, Formatting.Indented);
        File.WriteAllText(dataPath, json);
    }

    public void SavePrefs()
    {
        Debug.Log("saving prefs");

        for (int i = 0; i < levelInfos.Count; i++)
        {
            levelSaveData[i].levelID = (10 * (levelInfos[i].WorldID - 1)) + levelInfos[i].LevelID;
            levelSaveData[i].levelUnlocked = levelInfos[i].LevelUnlocked;
            levelSaveData[i].levelCompleted = levelInfos[i].LevelCompleted;
        }

        string json = JsonConvert.SerializeObject(levelSaveData, Formatting.Indented);
        File.WriteAllText(dataPath, json);
    }
}

[Serializable]
public class LevelSaveData
{
    [SerializeField] public int levelID;
    [SerializeField] public bool levelUnlocked;
    [SerializeField] public bool levelCompleted;
}