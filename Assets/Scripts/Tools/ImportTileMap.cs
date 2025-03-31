using UnityEngine;
using System.IO;
using UnityEditor;
using UnityEngine.Tilemaps;

public class ImportTileMap : ScriptableWizard
{
    public string fileName;
    public Tilemap tileMap;

    [MenuItem("Tools/Map Generation Tool/Import Map")]
    static void CreateWindow()
    {
        ScriptableWizard.DisplayWizard<ImportTileMap>("Map Generation Tool", "Import");
    }

    private void OnWizardUpdate()
    {
        if (tileMap == null)
        {
            try
            {
                tileMap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
            }
            catch
            {
                Debug.LogWarning("Tilemap could not be found in the current scene.");
            }
        }

        if (!File.Exists(Application.dataPath + "/Resources/" + fileName + ".txt"))
        {
            errorString = "File not valid: " + Application.dataPath + "/Resources/" + fileName + ".txt";
            
        }
        else
        {
            errorString = "";
        }
    }

    void OnWizardCreate()
    {
        GameObject.Find("Grid").GetComponent<BoardManager>().GenerateMapFromFile(fileName, true);
    }
}