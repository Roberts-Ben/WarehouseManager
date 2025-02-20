using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;
using System.IO;
using System.Text;
using System;

public class BoardManager : MonoBehaviour
{
    public static BoardManager instance;

    public Tilemap tilemap;

    public List<Tile> tiles;
    public List<GameObject> objectives;
    public List<Objective> objectivePositions;
    public List<TileBase> tileBases;

    public GameObject boxObjective;
    public GameObject targetObjective;
    public GameObject playerObj;

    public List<Color> objectiveColours;

    private int tileID = 0;
    private int tileObjID = 0;
    private Vector3Int tilePos = new();
    private Vector3Int tileObjOffset = new(1,1,-1);
    private bool tileoccupied = false;
    private TYPE tileType;

    public int moves;
    public int totalObjectives;

    public TMP_Text movesLabel;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        LevelLoad();
    }

    public void LevelLoad()
    {
        tiles = new List<Tile>();
        objectivePositions = new List<Objective>();

        GenerateMapFromFile();
        tilemap.CompressBounds(); // Clamp the tilemap, in case it was edited recently

        PopulateObjectives();
    }

    public void PopulateObjectives()
    {
        foreach (Tile tile in tiles)
        {
            GameObject objToSpawn = null;
            bool isBox = false;

            if (tile.tileType == TYPE.BOX)
            {
                objToSpawn = boxObjective;
                isBox = true;
                
                totalObjectives++;
            }
            else if (tile.tileType == TYPE.OBJECTIVE)
            {
                objToSpawn = targetObjective;
                isBox = false;
            }
            if (objToSpawn != null)
            {
                GameObject go = Instantiate(objToSpawn, tile.position + tileObjOffset, Quaternion.identity);
                go.GetComponent<ObjectiveInfo>().box = isBox;
                go.GetComponent<ObjectiveInfo>().objectiveID = tile.GetObjID();
                go.GetComponent<SpriteRenderer>().color = objectiveColours[tile.GetObjID()];

                Objective newObjective = new(go, isBox, tile.GetObjID(), tile.GetPos());
                objectivePositions.Add(newObjective);
                objectives.Add(go);
                go.transform.parent = GameObject.Find("Objectives").transform;
            }
        }
    }

    public void GenerateMapFromFile()
    {
        StreamReader reader = new(Application.dataPath + "/Resources/" + Menu.instance.levelFile + ".txt", Encoding.Default);

        string line = reader.ReadLine();
        string[] subs = line.Split('x');

        int width = Int32.Parse(subs[0]);
        int height = Int32.Parse(subs[1]);

        int row = 0;

        while (!reader.EndOfStream)
        {
            line = reader.ReadLine();
            if (width < line.Length)
            {
                width = line.Length;
            }

            char[] tileChar = line.ToCharArray();

            for (int x = 0; x < line.Length; x++)
            {
                tileoccupied = false;
                int tileToPlace = 0;

                if (tileChar[x] == '#')
                {
                    tilePos = new Vector3Int(x, -row, 0);
                    tileType = TYPE.WALL;
                    tileToPlace = 0;
                }
                else if (tileChar[x] == '0')
                {
                    tilePos = new Vector3Int(x, -row, 0);
                    tileType = TYPE.FLOOR;
                    tileToPlace = 1;
                }
                else if (tileChar[x] == '/')
                {
                    tilePos = new Vector3Int(x, -row, 0);
                    tileType = TYPE.FLOOR;
                    Instantiate(playerObj, (tilePos + tileObjOffset) - Vector3Int.forward, Quaternion.identity);
                    tileToPlace = 1;
                }
                else if ((int)tileChar[x] >= 65 && (int)tileChar[x] <= 90) // Upercase is box
                {
                    tilePos = new Vector3Int(x, -row, 0);
                    tileType = TYPE.BOX;
                    tileoccupied = true;
                    tileObjID = (int)tileChar[x] - 65;
                    tileToPlace = 1;
                }
                else if ((int)tileChar[x] >= 97 && (int)tileChar[x] <= 122) // lowecase is destination
                {
                    tilePos = new Vector3Int(x, -row, 0);
                    tileType = TYPE.OBJECTIVE;
                    tileObjID = (int)tileChar[x] - 97;
                    tileToPlace = 2;
                }
                else
                {
                    throw new Exception("Invalid character");
                }
                SwitchTile(tilePos, tileToPlace);
                Tile newTile = new(tileID, tilePos, tileType, tileObjID, tileoccupied);
                tiles.Add(newTile);

                tileID++;
            }
            row++;
        }
        Camera.main.transform.position = new Vector3(width/2, -height/2,-11);
        Camera.main.orthographicSize = height > width ? height : width;
    }

    public GameObject FindObjectiveObj(Vector3 pos)
    {
        foreach (Objective o in objectivePositions)
        {
            if (o.position == pos && o.GetBox())
            {
                return o.GetObj();
            }
        }
        return null;
    }

    public void SwitchTile(Vector3Int pos, int tileIndex)
    {
        tilemap.SetTile(pos, tileBases[tileIndex]);
    }

    public bool GetTile(Vector3 pos, Vector3 targetPos, Vector3 direction)
    {
        Vector3Int targetTile = tilemap.WorldToCell(targetPos);

        foreach (Tile t in tiles)
        {
            if(t.GetPos() == targetTile)
            {
                if (t.GetOccupied())
                {
                    bool canMoveBox = GetNextTile(targetPos, targetPos + direction, direction);

                    if (canMoveBox)
                    {
                        moves++;
                        UpdateMoves();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (t.GetTileType() != TYPE.WALL)
                {
                    moves++;
                    UpdateMoves();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        return false;
    }

    public bool GetNextTile(Vector3 pos, Vector3 targetPos, Vector3 direction)
    {
        Vector3Int currentTile = tilemap.WorldToCell(pos);
        Vector3Int targetTile = tilemap.WorldToCell(targetPos);

        GameObject box = FindObjectiveObj(currentTile);

        foreach (Tile t in tiles)
        {
            if (t.GetPos() == targetTile)
            {
                if (t.GetOccupied() || t.GetTileType() == TYPE.WALL)
                {
                    return false;
                }
                else 
                {
                    box.transform.position += direction;

                    foreach (Objective o in objectivePositions)
                    {
                        if (o.position == currentTile && o.box)
                        {
                            o.position = targetTile;
                        }
                    }

                    foreach (Tile thisTile in tiles)
                    {
                        if (thisTile.GetPos() == currentTile)
                        {
                            tiles[thisTile.GetID()].SetOccupied(false);
                            break;
                        }
                    }

                    tiles[t.GetID()].SetOccupied(true);
                   
                    return true;
                }
            }

        }

        return false;
    }

    public void CheckObjectives()
    {
        int objectivesMet = 0;

        foreach(GameObject go in objectives)
        {
            ObjectiveInfo info = go.GetComponent<ObjectiveInfo>();

            if(info.box) // If we are checking a box
            {
                Vector3Int currentTile = tilemap.WorldToCell(go.transform.position);
                int ID = info.objectiveID;

                foreach (Objective o in objectivePositions)
                {
                    if(!o.GetBox() && currentTile == o.position && o.objectiveID == ID) // If the box is on an objective space
                    {
                        objectivesMet++;
                    }
                }
            }
        }

        if(objectivesMet == totalObjectives)
        {
            Debug.LogWarning("Level Complete");
            Menu.instance.NewLevelComplete();
            SceneManager.UnloadSceneAsync(1);
        }
    }

    public void UpdateMoves()
    {
        movesLabel.text = "Moves: " + moves;
    }

    public void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}