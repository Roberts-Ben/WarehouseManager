using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour
{
    public static BoardManager instance;

    public Tilemap tilemap;

    public List<Tile> tiles;
    public List<GameObject> objectives;
    public List<Objective> objectivePositions;

    public List<Sprite> tileSprites;
    public TileBase floor;

    private int tileID = 0;
    private int tileObjID = 0;
    private bool tileoccupied = false;
    private TYPE tileType;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        tiles = new List<Tile>();
        objectivePositions = new List<Objective>();

        tilemap.CompressBounds(); // Clamp the tilemap, in case it was edited recently

        PopulateObjectives();
        PopulateTiles();
    }

    public void PopulateObjectives()
    {
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("Objective");
        foreach (GameObject go in objectsWithTag)
        {
            ObjectiveInfo objectiveInfo = go.GetComponent<ObjectiveInfo>();

            Vector3Int objectiveTile = tilemap.WorldToCell(go.transform.position);

            Objective newObjective = new Objective(go, objectiveInfo.box, objectiveInfo.objectiveID, objectiveTile);
            objectivePositions.Add(newObjective);
            objectives.Add(go);
        }
    }

    public void PopulateTiles()
    {
        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                tileoccupied = false;
                Sprite tilemapSprite = tilemap.GetSprite(pos);

                if (tilemapSprite == tileSprites[0]) // Wall
                {
                    tileType = TYPE.WALL;
                }
                else if (tilemapSprite == tileSprites[1]) // Floor
                {
                    tileType = TYPE.FLOOR;
                }
                else if (tilemapSprite == tileSprites[2]) // Objective
                {
                    tileType = TYPE.OBJECTIVE;

                    tileObjID = FindObjective(pos);

                    //SwitchTile(tilemap.GetTile(pos));
                }
                else if (tilemapSprite == tileSprites[3]) // Box
                {
                    tileType = TYPE.BOX;
                    tileoccupied = true;
                    Debug.LogWarning("tile at: " + pos + " is occupied");

                    tileObjID = FindObjective(pos);

                    //SwitchTile(tilemap.GetTile(pos));
                }
                else if (tilemapSprite == tileSprites[4]) // Player
                {
                    //SwitchTile(tilemap.GetTile(pos));
                }
                else
                {
                    Debug.LogError("Invalid Sprite");
                }
            }
            Tile newTile = new Tile(tileID, pos, tileType, tileObjID, tileoccupied);
            tiles.Add(newTile);

            tileID++;
        }
    }

    public int FindObjective(Vector3 pos)
    {
        foreach (Objective o in objectivePositions)
        {
            if (o.position == pos)
            {
                return o.objectiveID;
            }
        }
        return 0;
    }

    public GameObject FindObjectiveObj(Vector3 pos)
    {
        foreach (Objective o in objectivePositions)
        {
            if (o.position == pos)
            {
                return o.GetObj();
            }
        }
        return null;
    }

    public void SwitchTile(TileBase t)
    {
        tilemap.SwapTile(t, floor);
    }

    public bool GetTile(Vector3 pos, Vector3 targetPos, Vector3 direction)
    {
        //Vector3Int currentTile = tilemap.WorldToCell(pos);
        Vector3Int targetTile = tilemap.WorldToCell(targetPos);

        foreach (Tile t in tiles)
        {
            if(t.GetPos() == targetTile)
            {
                Debug.Log("Found Tile: " + targetTile);
                if (t.GetOccupied())
                {
                    Debug.Log("There is a box here");  

                    bool canMoveBox = GetNextTile(targetPos, targetPos + direction, direction);

                    if (canMoveBox)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (t.GetTileType() != TYPE.WALL)
                {
                    Debug.Log("Floor Tile. Moving to: " + tilemap.GetCellCenterWorld(targetTile));
                    return true;
                }
                else
                {
                    Debug.Log("Invalid movement");
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
                Debug.Log("Found Next Tile: " + targetTile);
                if (t.GetOccupied() || t.GetTileType() == TYPE.WALL)
                {
                    return false;
                }
                else 
                {
                    box.transform.position += direction;
                    Debug.Log("Box moving");

                    foreach (Objective o in objectivePositions)
                    {
                        if (o.position == currentTile)
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
}