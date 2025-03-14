using UnityEngine;
using UnityEngine.UI;

public class LevelInfo : MonoBehaviour
{
    public int WorldID { get; set; }
    public int LevelID { get; set; }
    public bool LevelUnlocked { get; set; }
    public bool LevelCompleted { get; set; }
    public int[] LevelRatings { get; set; }
    public int BestRating { get; set; }
    public Image[] RatingImages { get; set; }
}
