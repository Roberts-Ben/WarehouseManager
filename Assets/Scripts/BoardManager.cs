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

    public TileBase playertileBase;
    public List<TileBase> floorTileBases;
    public List<TileBase> wallTileBases;
    public List<TileBase> objectiveTileBases;
    public List<TileBase> objectiveBoxTileBases;

    public GameObject boxObjective;
    public GameObject targetObjective;
    public GameObject playerObj;

    public List<Sprite> objectiveSprites;
    public List<Color> objectiveColours;

    private int tileID = 0;
    private int tileObjID = 0;
    private Vector3Int tilePos = new();
    private Vector3Int ObjOffset = new(1,1,-2);
    private Vector3Int ObjTileOffset = new(1, 1, -1);
    private bool tileoccupied = false;
    private TYPE tileType;

    public int moves;
    public int totalObjectives;

    public TMP_Text movesLabel;

    void Awake()
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

        GenerateMapFromFile(Menu.instance.levelFile, false);
        tilemap.CompressBounds(); // Clamp the tilemap, in case it was edited recently

        PopulateObjectives();
    }

    public void PopulateObjectives()
    {
        foreach (Tile tile in tiles)
        {
            GameObject objToSpawn = null;
            bool isBox = false;

            if (tile.TileType == TYPE.BOX)
            {
                objToSpawn = boxObjective;
                isBox = true;
                
                totalObjectives++;
            }
            else if (tile.TileType == TYPE.OBJECTIVE)
            {
                objToSpawn = targetObjective;
                isBox = false;
            }
            if (objToSpawn != null)
            {
                Vector3 finalPos = tile.Position;
                if(isBox)
                {
                    finalPos += ObjOffset;
                }
                else
                {
                    finalPos += ObjTileOffset;
                }
                GameObject go = Instantiate(objToSpawn, finalPos, Quaternion.identity);
                ObjectiveInfo info = go.GetComponent<ObjectiveInfo>();
                SpriteRenderer spriteRenderer = go.GetComponent<SpriteRenderer>();
                info.box = isBox;
                info.objectiveID = tile.ObjectiveID;

                if(isBox)
                {
                    spriteRenderer.sprite = objectiveSprites[tile.ObjectiveID];
                }
                else
                {
                    spriteRenderer.color = objectiveColours[tile.ObjectiveID];
                }

                Objective newObjective = new()
                {
                    ObjectiveObj = go,
                    IsBox = isBox,
                    ObjectiveID = tile.ObjectiveID,
                    Position = tile.Position
                };
                objectivePositions.Add(newObjective);
                objectives.Add(go);
                go.transform.parent = GameObject.Find("Objectives").transform;
            }
        }
    }

    public void GenerateMapFromFile(string levelPath, bool loadedViaTool)
    {
        StreamReader reader = new(Application.dataPath + "/Resources/" + levelPath + ".txt", Encoding.Default);

        string line = reader.ReadLine(); // Get the grid size here
        string[] subs = line.Split('x');

        int width = Int32.Parse(subs[0]);
        int height = Int32.Parse(subs[1]);

        int row = 0;

        line = reader.ReadLine(); // Get the required moves for each rating

        if(!loadedViaTool)
        {
            Menu.instance.levelInfos[GetSelectedLevel(levelPath)].LevelRatings = Array.ConvertAll(line.Split(' '), int.Parse);
        }

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
                    tileToPlace = UnityEngine.Random.Range(0,wallTileBases.Count);
                }
                else if (tileChar[x] == '0')
                {
                    tilePos = new Vector3Int(x, -row, 0);
                    tileType = TYPE.FLOOR;
                    tileToPlace = UnityEngine.Random.Range(0, floorTileBases.Count);
                }
                else if (tileChar[x] == '/')
                {
                    tilePos = new Vector3Int(x, -row, 0);
                    tileType = TYPE.PLAYER;
                    tileToPlace = UnityEngine.Random.Range(0, floorTileBases.Count);

                    if (!loadedViaTool)
                    {
                        GameObject player = Instantiate(playerObj, (tilePos + ObjOffset) - Vector3Int.forward, Quaternion.identity);
                        player.transform.parent = transform;
                    }
                }
                else if ((int)tileChar[x] >= 65 && (int)tileChar[x] <= 90) // Upercase is box
                {
                    tilePos = new Vector3Int(x, -row, 0);
                    tileType = TYPE.BOX;
                    tileoccupied = true;
                    tileObjID = (int)tileChar[x] - 65;
                    tileToPlace = tileObjID; // Force objective sprite                   
                }
                else if ((int)tileChar[x] >= 97 && (int)tileChar[x] <= 122) // lowecase is destination
                {
                    tilePos = new Vector3Int(x, -row, 0);
                    tileType = TYPE.OBJECTIVE;
                    tileObjID = (int)tileChar[x] - 97;
                    tileToPlace = tileObjID;
                }
                else
                {
                    throw new Exception("Invalid character");
                }
                SwitchTile(tilePos, tileType, tileToPlace, loadedViaTool);

                if (!loadedViaTool)
                {
                    Tile newTile = new()
                    {
                        ID = tileID,
                        Position = tilePos,
                        TileType = tileType,
                        ObjectiveID = tileObjID,
                        Occupied = tileoccupied
                    };
                    tiles.Add(newTile);

                    tileID++;
                }
            }
            row++;
        }
        if(!loadedViaTool)
        {
            GameObject menuCam = Menu.instance.menuCamera;
            GameObject gameCam = Menu.instance.gameCamera;

            gameCam.transform.position = new Vector3(width / 2 + 1, -height / 2 + 2, -11);
            gameCam.GetComponent<Camera>().orthographicSize = width / 2 + 1;
            menuCam.SetActive(false);
            gameCam.SetActive(true);
        }
    }

    public int GetSelectedLevel(string levelName)
    {
        List<int> levelID = new();
        foreach (var s in levelName.Split('-'))
        {
            int num;
            if (int.TryParse(s, out num))
                levelID.Add(num);
        }

        return (levelID[0] * levelID[1]) - 1;
    }

    public void ClearTileMap()
    {
        tilemap.ClearAllTiles();
        tiles.Clear();
    }

    public GameObject FindObjectiveObj(Vector3 pos)
    {
        foreach (Objective o in objectivePositions)
        {
            if (o.Position == pos && o.IsBox)
            {
                return o.ObjectiveObj;
            }
        }
        return null;
    }

    public void SwitchTile(Vector3Int pos, TYPE tile,  int tileIndex, bool loadedViaTool)
    {
        switch (tile)
        {
            case TYPE.WALL:
                tilemap.SetTile(pos, wallTileBases[tileIndex]);
                break;
            case TYPE.FLOOR:
                tilemap.SetTile(pos, floorTileBases[tileIndex]);
                break;
            case TYPE.BOX:
                if(!loadedViaTool)
                {
                    tilemap.SetTile(pos, floorTileBases[tileIndex]);
                }
                else
                {
                    tilemap.SetTile(pos, objectiveBoxTileBases[tileIndex]);
                }
                break;
            case TYPE.OBJECTIVE:
                tilemap.SetTile(pos, objectiveTileBases[tileIndex]);
                break;
            case TYPE.PLAYER:
                if(!loadedViaTool)
                {
                    tilemap.SetTile(pos, floorTileBases[tileIndex]);
                }
                else
                {
                    tilemap.SetTile(pos, playertileBase);
                }
                break;
            default:
                break;
        }
    }

    public bool GetTile(Vector3 targetPos, Vector3 direction)
    {
        Vector3Int targetTile = tilemap.WorldToCell(targetPos);

        foreach (Tile t in tiles)
        {
            if(t.Position == targetTile)
            {
                if (t.Occupied)
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
                else if (t.TileType != TYPE.WALL)
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
            if (t.Position == targetTile)
            {
                if (t.Occupied || t.TileType == TYPE.WALL)
                {
                    return false;
                }
                else 
                {
                    box.transform.position += direction;

                    foreach (Objective o in objectivePositions)
                    {
                        if (o.Position == currentTile && o.IsBox)
                        {
                            o.Position = targetTile;
                        }
                    }

                    foreach (Tile thisTile in tiles)
                    {
                        if (thisTile.Position == currentTile)
                        {
                            tiles[thisTile.ID].Occupied = false;
                            break;
                        }
                    }

                    tiles[t.ID].Occupied = true;
                   
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
                    if(!o.IsBox && currentTile == o.Position && o.ObjectiveID == ID) // If the box is on an objective space
                    {
                        objectivesMet++;
                    }
                }
            }
        }

        if(objectivesMet == totalObjectives)
        {
            Debug.LogWarning("Level Complete");
            Menu.instance.LevelComplete(moves);
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