using UnityEngine;

[System.Serializable]
public class Objective
{
    public GameObject objectiveObj;
    public bool box;
    public int objectiveID;
    public Vector3 position;

    public Objective(GameObject _objectiveObj, bool _box, int _objectiveID, Vector3 _position)
    {
        objectiveObj = _objectiveObj;
        box = _box;
        objectiveID = _objectiveID;
        position = _position;
    }

    public GameObject GetObj()
    {
        return objectiveObj;
    }

    public bool GetBox()
    {
        return box;
    }
}
