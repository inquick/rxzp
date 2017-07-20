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
    public GameObject idCon;

    public Image headIcon;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void SetSettlementUI(int score, ClientPlayerInfo player, bool selfWin)
    {
        headIcon.gameObject.SetActive(true);
        playerName.gameObject.SetActive(true);
        idCon.SetActive(true);
        winScore.transform.parent.gameObject.SetActive(true);

        headIcon.sprite = player.HeadIcon;

        string path = "";
        if (score > 0)
        {
            path = "settlement/win/";
            winIcon.SetActive(true);

            playerName.text = player.Name;
            id.text = player.PlayerId.ToString();
        }
        else
        {
            path = "settlement/lose/";
            winIcon.SetActive(false);

            playerName.text = string.Format("<color=#ffffffff>{0}</color>", player.Name);
            id.text = string.Format("<color=#ffffffff>{0}</color>", player.PlayerId.ToString());
        }
        finalText.sprite = Resources.Load<Sprite>(path + "final");
        winScore.SetScore(score);
    }

    public void HideInfo()
    {
        headIcon.gameObject.SetActive(false);
        playerName.gameObject.SetActive(false);
        idCon.SetActive(false);
        winScore.transform.parent.gameObject.SetActive(false);
        winIcon.SetActive(false);
    }
}
