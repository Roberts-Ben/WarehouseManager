using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.Text;

public class ConvertTilemapToJSON : ScriptableWizard
{
    public string outputDirectory = "/Resources/";
    public string fileName = "newLevel";
    public Vector3 levelScores = Vector3.zero;
    public Tilemap tileMap;

    private readonly Dictionary<string, char> tileTypeToChar = new()
    {
        {"WallTile", '#'},
        {"WallTile2", '#'},
        {"WallTile3", '#'},
        {"FloorTile", '0'},
        {"FloorTile2", '0'},
        {"FloorTile3", '0'},
        {"Player", '/'},
        {"Objective_Red", 'A'},
        {"Objective_Blue", 'B'},
        {"Objective_Yellow", 'C'},
        {"ObjectiveTile_Red", 'a'},
        {"ObjectiveTile_Blue", 'b'},
        {"ObjectiveTile_Yellow", 'c'},
    };

    [MenuItem("Tools/Map Generation Tool/Export Map")]
    static void CreateWindow()
    {
        ScriptableWizard.DisplayWizard<ConvertTilemapToJSON>("Map Generation Tool", "Export", "Refresh");
    }

    private void OnWizardOtherButton()
    {
        OnWizardUpdate();
    }
    private void OnWizardUpdate()
    {
        if(tileMap == null)
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

        if (!Directory.Exists("Assets/" + outputDirectory))
        {
            errorString = "Output directory is not valid.";
        }
        else
        {
            errorString = "";
        }
    }

    void OnWizardCreate()
    {
        tileMap.CompressBounds();

        Vector3Int tileMapSize = new(tileMap.size.x, tileMap.size.y, 0);
        Vector3Int tilemapOrigin = tileMap.origin;

        TileBase tileBase;

        StreamWriter writer = new (Application.dataPath + "/Resources/" + fileName + ".txt", true, Encoding.Default);

        writer.Write(tileMapSize.x + "x" + tileMapSize.y); // Set map size
        writer.WriteLine();

        // Set goals
        writer.Write(levelScores.x + " " + levelScores.y + " " + levelScores.z);
        writer.WriteLine();

        for (int y = (tilemapOrigin.y + tileMapSize.y) - 1; y >= tileMap.origin.y; y--)
        {
            for (int x = tileMap.origin.x; x < tileMapSize.x - 1; x++)
            {
                Vector3Int tilePos = new(x, y, 0);
                tileBase = tileMap.GetTile(tilePos);

                if (tileBase == null)
                {
                    Debug.LogError("Invalid tile data at: " + tilePos.x + " " + tilePos.y);
                }

                Debug.Log("Tile: " + tileBase.name + " - " + tileTypeToChar[tileBase.name]);
                writer.Write(tileTypeToChar[tileBase.name]);
            }
            writer.WriteLine(); // new line
        }
        writer.Close();
    }
}