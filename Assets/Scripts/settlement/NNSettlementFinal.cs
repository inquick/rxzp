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
    public Button backHome;
    public Button shareBtn;
    public HomeController controller;
    public ThirdParty thirdParty;

    public Text time;
    public Text roomId;

    private Dictionary<int, SettlementPlayerInfo> players = new Dictionary<int,SettlementPlayerInfo>();
	// Use this for initialization
	void Start () {
        closeBtn.onClick.AddListener(CloseWnd);
        backHome.onClick.AddListener(CloseWnd);
        shareBtn.onClick.AddListener(ShareScreenshot);
	}

    /// <summary>
    /// 关闭窗口
    /// </summary>
    void CloseWnd()
    {
        transform.gameObject.SetActive(false);
        controller.CloseWindow(WINDOW_ID.WINDOW_ID_GAME_NN);
        controller.BackHome();
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
        time.text = System.DateTime.Now.ToString();
        roomId.text = room.GetRoomId.ToString();

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
                room.playerSelf.PlayerInfo.LeaveCardCount = data.leaveCardNum;
            }
        }
        int index = 0;
        foreach (SettlementData data in info.players)
        {
            players[index].SetSettlementUI(data.finalscore, room.GetPlayer(data.ID), selfWin);
            index++;
        }
        while (index < 5)
        {
            players[index].HideInfo();
            index++;
        }
    }
}
