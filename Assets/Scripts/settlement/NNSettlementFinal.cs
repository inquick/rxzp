using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using netty;

public class NNSettlementFinal : MonoBehaviour
{
    public Image winSprite;
    public SettlementPlayerInfo player1;
    public SettlementPlayerInfo player2;
    public SettlementPlayerInfo player3;
    public SettlementPlayerInfo player4;
    public SettlementPlayerInfo player5;
    public Button closeBtn;
    public Button shareBtn;
    public HomeController controller;
    public ThirdParty thirdParty;

    private Dictionary<int, SettlementPlayerInfo> players = new Dictionary<int,SettlementPlayerInfo>();
	// Use this for initialization
	void Start () {
        closeBtn.onClick.AddListener(CloseWnd);
        shareBtn.onClick.AddListener(ShareScreenshot);
	}

    /// <summary>
    /// 关闭窗口
    /// </summary>
    void CloseWnd()
    {
        transform.gameObject.SetActive(false);
        controller.CloseWindow(WINDOW_ID.WINDOW_ID_GAME_NN);
        controller.OpenWindow(WINDOW_ID.WINDOW_ID_HOME);
    }

    void ShareScreenshot()
    {
        string imagePath = "";
        Debug.LogError("开始截屏！");
        Application.CaptureScreenshot("screenshot.png");
        Debug.LogError("截屏成功！");
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            imagePath = Application.persistentDataPath;
        }
        else if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            imagePath = Application.dataPath;
        }
        else if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            imagePath = Application.dataPath;
            imagePath = imagePath.Replace("/Assets", null);
        }
        // 调用设备微信分享
        thirdParty.ShareScreenshot(imagePath + "/screenshot.png");
    }
	
	// Update is called once per frame
	void Update () {
	}

    public void SetSettlementUI(NNRoomSprite room, SettlementInfo info)
    {
        if (players.Count == 0)
        {
            players.Add(0, player1);
            players.Add(1, player2);
            players.Add(2, player3);
            players.Add(3, player4);
            players.Add(4, player5);

        }
        bool selfWin = false;

        foreach (SettlementData data in info.players)
        {
            if (data.ID == room.playerSelf.PlayerInfo.PlayerId)
            {
                selfWin = data.isWin;
            }
        }
        int index = 0;
        foreach (SettlementData data in info.players)
        {
            players[index].SetSettlementUI(data.finalscore, room.GetPlayer(data.ID), selfWin);
            index++;
        }

        // 输赢背景框
        for(;index < 5; ++index)
        {
            players[index].info.SetActive(false);
            string path = "";
            if (selfWin)
            {
                path = "settlement/win/text_bg";
            }
            else
            {
                path = "settlement/lose/text_bg";
            }
            players[index].textBg.sprite = Resources.Load<Sprite>(path);
        }
    }
}
