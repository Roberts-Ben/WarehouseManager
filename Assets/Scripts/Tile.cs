using UnityEngine;

public enum TYPE
{
    WALL,
    FLOOR,
    BOX,
    OBJECTIVE
};

[System.Serializable]
public class Tile
{
    public int ID;
    public Vector3 position;
    public TYPE tileType;
    public int objectiveID;
    public bool occupied;

    public Tile(int _ID, Vector3 _position, TYPE _tileType, int _objectiveID, bool _occupied)
    {
        ID = _ID;
        position = _position;
        tileType = _tileType;
        objectiveID = _objectiveID;
        occupied = _occupied;
    }

    public int GetID()
    {
        return ID;
    }
    public Vector3 GetPos()
    {
        return position;
    }
    public TYPE GetTileType()
    {
        return tileType;
    }
    public int GetObjID()
    {
        return objectiveID;
    }
    public bool GetOccupied()
    {
        return occupied;
    }

    public void SetOccupied(bool isOccupied)
    {
        occupied = isOccupied;
    }
}
