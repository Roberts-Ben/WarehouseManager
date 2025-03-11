using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System;
using System.IO;
using UnityEngine.UI;
using Newtonsoft.Json;
using UnityEngine.Analytics;

public class Menu : MonoBehaviour
{
    static public Menu instance;

    public GameObject mainPanel;
    public GameObject optionsPanel;
    public GameObject levelPanel;

    public GameObject[] levelButtonObjects;
    public List<LevelInfo> levelInfos;
    public List<Button> levelButtons;

    public string levelFile;
    public int selectedLevel;

    private string dataPath = Application.dataPath + "/Data/LevelData.json";

    public GameObject canvas;

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

        LoadAndSortLevels();

        optionsPanel.SetActive(false);
        levelPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            ResetPrefs();
            LoadAndSortLevels();
        }
    }
    public void Play()
    {
        mainPanel.SetActive(false);
        levelPanel.SetActive(true);
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
    }

    public void LoadAndSortLevels()
    {
        levelButtons.Clear();
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
            Button newBtn = levelInfos[i].gameObject.GetComponent<Button>();
            Image[] levelCompleteImage = newBtn.GetComponentsInChildren<Image>();
            levelButtons.Add(newBtn);

            if (levelSaveData[i].levelUnlocked)
            {
                newBtn.interactable = true;
            }
            else
            {
                newBtn.interactable = false;
            }
            if (levelSaveData[i].levelCompleted)
            {
                Debug.Log("Level: " + levelSaveData[i].levelID + " completed");
                levelInfos[i].CompletedImage = levelCompleteImage[1];
                levelInfos[i].CompletedImage.color = Color.green;
            }
        }
    }

   public void LoadLevel(LevelInfo levelInfo)
    {
        levelFile = levelInfo.WorldID + "-" + levelInfo.LevelID;
        selectedLevel = levelInfo.WorldID * levelInfo.LevelID;
        canvas.SetActive(false);

        SceneManager.LoadScene("Level", LoadSceneMode.Additive);
        backgroundMap.SetActive(false);
    }
    public void NewLevelComplete()
    {
        backgroundMap.SetActive(true);
        menuCamera.SetActive(true);
        gameCamera.SetActive(false);

        if (!levelInfos[selectedLevel - 1].LevelCompleted)
        {
            levelInfos[selectedLevel - 1].LevelCompleted = true;
            levelInfos[selectedLevel - 1].LevelUnlocked = true;
            levelInfos[selectedLevel].LevelUnlocked = true;
        }
        SavePrefs();
        canvas.SetActive(true);
        LoadAndSortLevels();
    }

    public void ToggleAudio()
    {
        audioEnabled = !audioEnabled;
        audioButton.GetComponent<Image>().sprite = audioSprites[audioEnabled ? 1 : 0];

    }
    public void LoadPrefs()
    {
        Debug.Log("Loading prefs");
        if (File.Exists(dataPath))
        {
            string str = File.ReadAllText(dataPath);
            levelSaveData = JsonConvert.DeserializeObject<List<LevelSaveData>>(str);
        }
        else
        {
            InitPrefs();
        }

        audioButton.GetComponent<Image>().sprite = audioSprites[audioEnabled ? 1 : 0];
    }

    public void InitPrefs()
    {
        Debug.Log("Prefs not found");
        levelSaveData.Clear();
        ResetPrefs();
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
    public void ResetPrefs()
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