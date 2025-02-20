using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelID : MonoBehaviour
{
    public int worldID;
    public int levelID;

    public bool levelUnlocked;
    public bool levelCompleted;

    public int GetWorldID()
    {
        return levelID;
    }
    public int GetLevelID()
    {
        return levelID;
    }
    public bool GetLevelUnlocked()
    {
        return levelUnlocked;
    }
    public bool GetLevelCompleted()
    {
        return levelCompleted;
    }

    public void SetLevelUnlocked(bool unlocked)
    {
        levelUnlocked = unlocked;
    }
    public void SetLevelCompleted(bool complete)
    {
        levelCompleted = complete;
    }
}
