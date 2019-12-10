using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject optionsPanel;
    public GameObject levelPanel;

    public GameObject[] levelButtonObjects;
    public List<Button> levelButtons;

    public void Awake()
    {
        levelButtonObjects = GameObject.FindGameObjectsWithTag("LevelButton");

        foreach (GameObject go in levelButtonObjects)
        {
            Button newBtn = go.GetComponent<Button>();
            levelButtons.Add(newBtn);
        }

        optionsPanel.SetActive(false);
        levelPanel.SetActive(false);
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
}
