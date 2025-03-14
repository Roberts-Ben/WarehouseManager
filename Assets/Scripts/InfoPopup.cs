using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public enum POPUPTYPE
{
    HOWTOPLAY,
    LEVELCOMPLETE,
    CONFIRMDATARESET,
    CONFIRMQUIT
}

public class InfoPopup : MonoBehaviour
{
    public List<string> titles;
    public List<string> descriptions;
    public List<Vector2> panelSizes;
    public List<string> icons;
    public List<GameObject> actionButtons;

    public TMP_Text title;
    public TMP_Text description;
    public RectTransform panel;
    public Image icon;

    public void DisplayPopup(POPUPTYPE popupType)
    {
        switch (popupType)
        {
            case POPUPTYPE.HOWTOPLAY:
                title.text = titles[0];
                description.text = descriptions[0];
                panel.sizeDelta = panelSizes[0];

                foreach(GameObject go in actionButtons)
                {
                    go.SetActive(false);
                }
                break;
            case POPUPTYPE.CONFIRMDATARESET:
                title.text = titles[2];
                description.text = descriptions[2];
                panel.sizeDelta = panelSizes[2];
                actionButtons[0].SetActive(true); // Yes/No
                actionButtons[1].SetActive(true);
                break;
            default:
                break;
        }

        this.gameObject.SetActive(true);
    }

    public void ClosePopup()
    {
        this.gameObject.SetActive(false);
    }

    public void ConfirmDataReset()
    {
        Menu.instance.ResetProgress();
        ClosePopup();
    }
}
