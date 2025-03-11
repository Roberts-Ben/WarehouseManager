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
    public int ID { get; set; }
    public Vector3 Position { get; set; }
    public TYPE TileType { get; set; }
    public int ObjectiveID { get; set; }
    public bool Occupied { get; set; }
}
