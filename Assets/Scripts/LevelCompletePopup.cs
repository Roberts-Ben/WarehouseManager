using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelCompletePopup : MonoBehaviour
{
    public TMP_Text moves;
    public GameObject panel;

    private int finalRating;
    public List<Image> levelRatingImages;
    public List<Sprite> ratingSprites;

    public void DisplayPopup(int _moves, int _level)
    {
        moves.text = _moves.ToString();
        panel.SetActive(true);

        finalRating = Menu.instance.levelInfos[_level].BestRating;

        for(int i = 0; i < levelRatingImages.Count; i++)
        {
            if(i < finalRating)
            {
                levelRatingImages[i].sprite = ratingSprites[1]; // Animate these instead
            }
            else
            {
                levelRatingImages[i].sprite = ratingSprites[0];
            }
        }
    }

    public void ClosePopup()
    {
        panel.SetActive(false);
        Menu.instance.ReturnToMenu();
        SceneManager.UnloadSceneAsync(1);
    }
}
