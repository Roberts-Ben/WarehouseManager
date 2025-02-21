using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System;
using System.IO;
using UnityEngine.UI;
using Newtonsoft.Json;

public class Menu : MonoBehaviour
{
    static public Menu instance;

    public GameObject mainPanel;
    public GameObject optionsPanel;
    public GameObject levelPanel;

    public GameObject[] levelButtonObjects;
    public List<LevelID> levelIDs;
    public List<Button> levelButtons;

    public int maxLevelReached;

    public string levelFile;
    public int selectedLevel;

    public GameObject canvas;

    public string levelJSON;
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
        levelButtonObjects = GameObject.FindGameObjectsWithTag("LevelButton");
        foreach (GameObject levelButtonObject in levelButtonObjects)
        {
            levelIDs.Add(levelButtonObject.GetComponent<LevelID>());
        }

        levelIDs = levelIDs.OrderBy(w => w.worldID).ThenBy(l => l.levelID).ToList();

        LoadPrefs();

        for(int i = 0; i < levelSaveData.Count(); i++)
        {
            Button newBtn = levelIDs[i].gameObject.GetComponent<Button>();
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
                levelIDs[i].GetComponent<Image>().color = Color.green;
            }
        }
    }

   public void LoadLevel(LevelID levelID)
    {
        levelFile = levelID.worldID + "-" + levelID.levelID;
        selectedLevel = levelID.worldID * levelID.levelID;
        canvas.SetActive(false);

        SceneManager.LoadScene("Level", LoadSceneMode.Additive);
        backgroundMap.SetActive(false);
    }
    public void NewLevelComplete()
    {
        LoadAndSortLevels();
        canvas.SetActive(true);
        if (!levelIDs[selectedLevel].GetLevelCompleted())
        {
            Debug.Log("Newly completed level");
            levelIDs[selectedLevel - 1].SetLevelCompleted(true);
            levelIDs[selectedLevel].SetLevelUnlocked(true);
        }
        SavePrefs();
    }

    public void ResetPrefs()
    {
        Debug.LogWarning("Resetting prefs");
        foreach(LevelSaveData levelData in levelSaveData)
        {
            if(levelData.levelID == 1)
            {
                levelData.levelUnlocked = true;
                levelData.levelCompleted = false;
            }
            else
            {
                levelData.levelUnlocked = false;
                levelData.levelCompleted = false;
            }    
        }
        SavePrefs();
    }

    public void ToggleAudio()
    {
        audioEnabled = !audioEnabled;
        audioButton.GetComponent<Image>().sprite = audioSprites[audioEnabled ? 1 : 0];

    }
    public void LoadPrefs()
    {
        Debug.Log("Loading prefs");
        string str = File.ReadAllText(Application.dataPath + "/Data/LevelData.json");
        levelSaveData = JsonConvert.DeserializeObject<List<LevelSaveData>>(str);

        audioButton.GetComponent<Image>().sprite = audioSprites[audioEnabled ? 1 : 0];
    }
    public void SavePrefs()
    {
        Debug.Log("saving prefs");
        levelSaveData = new();

        foreach (LevelID levelID in levelIDs)
        {
            LevelSaveData levelData = new()
            {
                levelID = levelID.worldID * levelID.levelID,
                levelUnlocked = levelID.levelUnlocked,
                levelCompleted = levelID.levelCompleted
            };

            levelSaveData.Add(levelData);
        }
        string json = JsonConvert.SerializeObject(levelSaveData, Formatting.Indented);
        File.WriteAllText(Application.dataPath + "/Data/LevelData.json", json);
    }
}

[Serializable]
public class LevelSaveData
{
    [SerializeField] public int levelID;
    [SerializeField] public bool levelUnlocked;
    [SerializeField] public bool levelCompleted;
}