using UnityEngine;
using UnityEngine.UI;

public class LevelInfo : MonoBehaviour
{
    public int WorldID { get; set; }
    public int LevelID { get; set; }

    public bool LevelUnlocked { get; set; }
    public bool LevelCompleted { get; set; }

    public Image CompletedImage { get; set; }
}
