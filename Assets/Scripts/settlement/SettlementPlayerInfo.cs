using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SettlementPlayerInfo : MonoBehaviour
{
    public Image finalText;
    public WinScore winScore;
    public GameObject winIcon;

    public Text playerName;
    public Text id;
    public Text idTitle;

    public Image headIcon;
    public Image textBg;

    public GameObject info;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void SetSettlementUI(int score, ClientPlayerInfo player, bool selfWin)
    {
        info.SetActive(true);

        headIcon.sprite = player.HeadIcon;

        string path = "";
        if (score > 0)
        {
            path = "settlement/win/";
            winIcon.SetActive(true);

            playerName.text = player.Name;
            id.text = player.PlayerId.ToString();
            idTitle.text = "ID :";
        }
        else
        {
            path = "settlement/lose/";
            winIcon.SetActive(false);

            playerName.text = string.Format("<color=#ffffffff>{0}</color>", player.Name);
            id.text = string.Format("<color=#ffffffff>{0}</color>", player.PlayerId.ToString());
            idTitle.text = "<color=#ffffffff>ID :</color>";
        }
        finalText.sprite = Resources.Load<Sprite>(path + "final");

        path = "";
        if (selfWin)
        {
            path = "settlement/win/text_bg";
        }else
        {
            path = "settlement/lose/text_bg";
        }
        textBg.sprite = Resources.Load<Sprite>(path);
        winScore.SetScore(score);
    }
}
