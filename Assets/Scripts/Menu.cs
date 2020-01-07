using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    static public Menu instance;

    public GameObject mainPanel;
    public GameObject optionsPanel;
    public GameObject levelPanel;

    public GameObject[] levelButtonObjects;
    public List<Button> levelButtons;

    public int levelsUnlocked;
    public int levelsCompleted;

    public void Awake()
    {
        instance = this;

        levelsUnlocked = PlayerPrefs.GetInt("levelsUnlocked", 1);
        levelsCompleted = PlayerPrefs.GetInt("levelsCompleted", 0);

        levelButtonObjects = GameObject.FindGameObjectsWithTag("LevelButton");

        foreach (GameObject go in levelButtonObjects)
        {
            Button newBtn = go.GetComponent<Button>();
            levelButtons.Add(newBtn);
            newBtn.interactable = false;
        }

        for (int i = 0; i <= levelButtons.Count; i++)
        {
            if(i < levelsUnlocked)
            {
                levelButtons[i].interactable = true;
            }

            if(i < levelsCompleted)
            {
                levelButtons[i].GetComponent<Image>().color = Color.green;
            }
        }

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

    public void SavePrefs()
    {
        PlayerPrefs.SetInt("levelsUnlocked", levelsUnlocked);
        PlayerPrefs.SetInt("levelsCompleted", levelsCompleted);

        PlayerPrefs.Save();
    }
}
