using UnityEngine;
using System.IO;
using UnityEditor;
using UnityEngine.Tilemaps;

public class ClearTileMap : ScriptableWizard
{
    public string fileName;
    public Tilemap tileMap;

    [MenuItem("Tools/Map Generation Tool/Clear Map")]
    static void ClearMap()
    {
        GameObject.Find("Grid").GetComponent<BoardManager>().ClearTileMap();
    }
}