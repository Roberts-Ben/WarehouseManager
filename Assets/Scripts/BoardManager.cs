using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour
{
    public static BoardManager instance;

    public Tilemap tilemap;
    public List<Tile> tiles;

    public List<Sprite> tileSprites;

    public TileBase floor;

    private int tileID = 0;
    private int tileObjID = 0; //TODO
    private bool tileoccupied = false;
    private TYPE tileType;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        tiles = new List<Tile>();
        tilemap.CompressBounds(); // Clamp the tilemap, in case it was edited recently

        PopulateTiles();
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
                    Debug.Log("Found an objective tile at: " + pos);
                    tileType = TYPE.OBJECTIVE;
                    
                    //GET OBJ ID
                }
                else if (tilemapSprite == tileSprites[3]) // Box
                {
                    Debug.Log("Found a box tile at: " + pos);
                    tileType = TYPE.FLOOR;
                    tileoccupied = true;

                    // GET BOX ID

                    TileBase current = tilemap.GetTile(pos);
                    tilemap.SwapTile(current, floor);
                }
                else if (tilemapSprite == tileSprites[4]) // Player
                {
                    Debug.Log("Found player tile at: " + pos);
                    TileBase current = tilemap.GetTile(pos);
                    tilemap.SwapTile(current, floor); //THIS SEMS BROKEN NOW
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
                else if (t.GetTileType() == TYPE.FLOOR)
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

        GameObject box = GameObject.Find("Box"); //TEMP HACK NEED TO GET ACTUAL BOX

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

                    foreach (Tile thisTile in tiles)
                    {
                        if (thisTile.GetPos() == currentTile)
                        {
                            Debug.LogWarning(t.GetID());
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