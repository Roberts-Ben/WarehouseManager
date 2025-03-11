using UnityEngine;

[System.Serializable]
public class Objective
{
    public GameObject ObjectiveObj { get; set; }
    public bool IsBox { get; set; }
    public int ObjectiveID { get; set; }
    public Vector3 Position { get; set; }
}
