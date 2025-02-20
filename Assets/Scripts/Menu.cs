using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Text.RegularExpressions;
using System;
using UnityEngine.TextCore.Text;

public class Menu : MonoBehaviour
{
    static public Menu instance;

    public GameObject mainPanel;
    public GameObject optionsPanel;
    public GameObject levelPanel;

    public GameObject[] levelButtonObjects;
    public List<LevelID> levelIDs;
    public List<Button> levelButtons;

    public int levelsUnlocked;
    public int levelsCompleted;

    public void Awake()
    {
        instance = this;

        CheckPrefs();

        levelsUnlocked = PlayerPrefs.GetInt("levelsUnlocked", 1);
        levelsCompleted = PlayerPrefs.GetInt("levelsCompleted", 0);

        LoadAndSortLevels();

        optionsPanel.SetActive(false);
        levelPanel.SetActive(false);

        SavePrefs();
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

        foreach (LevelID level in levelIDs)
        {
            Button newBtn = level.gameObject.GetComponent<Button>();
            levelButtons.Add(newBtn);
            newBtn.interactable = false;
        }

        for (int i = 0; i <= levelButtons.Count; i++)
        {
            if (i < levelsUnlocked)
            {
                levelButtons[i].interactable = true;
            }

            if (i < levelsCompleted)
            {
                levelButtons[i].GetComponent<Image>().color = Color.green;
            }
        }
    }

   public void LoadLevel(int level)
    {
        SceneManager.LoadScene(level);
    }

    public void NewLevelComplete()
    {
        levelsCompleted++;
        levelsUnlocked++;
        SavePrefs();
    }

    public void CheckPrefs()
    {
        if (!PlayerPrefs.HasKey("levelsUnlocked"))
        {
            PlayerPrefs.SetInt("levelsUnlocked", 1);
        }
        if (!PlayerPrefs.HasKey("levelsCompleted"))
        {
            PlayerPrefs.SetInt("levelsCompleted", 0);
        }
    }
    public void SavePrefs()
    {
        PlayerPrefs.SetInt("levelsUnlocked", levelsUnlocked);
        PlayerPrefs.SetInt("levelsCompleted", levelsCompleted);

        PlayerPrefs.Save();
    }
}
