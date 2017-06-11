using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using UnityEngine.UI;
using netty;
using System.Collections.Generic;

/// <summary>
/// 菜单面板
/// </summary>
public class CreateNNMenu : MonoBehaviour
{
    public HomeController controller;

    public NNRoomSprite room;

    private int games = 10;
    private int payType = 1;

    public Toggle checkNum10 = null;
    public Toggle checkNum20 = null;
    public Toggle checkNum30 = null;

    public Toggle checkPayLead = null;
    public Toggle checkPayAA = null;
    public Toggle checkPayWin = null;

    public Text text10 = null;
    public Text text20 = null;
    public Text text30 = null;

    public Text textpayLead = null;
    public Text textpayAA = null;
    public Text textpayWin = null;

    public Text needKey = null;
    public Text leaveKey = null;

    private Color notSelect;
    private Color selected;
    //public Games
    //{
    //AudioSettings{}
    //}

    void Start()
    {
        Debug.Log("CreateMenu Start!");
        transform.Find("bg/CreateBtn").gameObject.GetComponent<Button>().onClick.AddListener(CreateRoom);

        checkNum10.onValueChanged.AddListener(CheckNum10);
        checkNum20.onValueChanged.AddListener(CheckNum20);
        checkNum30.onValueChanged.AddListener(CheckNum30);

        checkPayLead.onValueChanged.AddListener(CheckPayLead);
        checkPayAA.onValueChanged.AddListener(CheckPayAA);
        checkPayWin.onValueChanged.AddListener(CheckPayWin);

        notSelect = text20.color;
        selected = text10.color;

        needKey.text = 1 + " 张";
        leaveKey.text = 99 + " 张";

        Debug.Log("CreateMenu Start success!");
    }

    /// <summary>
    /// 创建房间
    /// </summary>
    void CreateRoom()
    {
        //if (NeedKeyNum() > controller.GetLeaveRoomCardNum)
        //{
        //    controller.ShowTips("您的房卡数量不足，无法创建房间！");
        //    return;
        //}

        MessageInfo req = new MessageInfo();
        CreateNNRoomReq creatRoom = new CreateNNRoomReq();
        req.messageId = MESSAGE_ID.msg_CreateNNRoomReq;
        creatRoom.games = this.games;
        creatRoom.type = this.payType;
        creatRoom.playerId = controller.PlayerId;
        req.createNNRoomReq = creatRoom;

        PPSocket.GetInstance().SendMessage(req);

        controller.CloseWindow(WINDOW_ID.WINDOW_ID_HOME);
        controller.OpenWindow(WINDOW_ID.WINDOW_ID_GAME_NN);
        controller.LoadingStart();
        // 设置游戏局数
        room.GameCounts = this.games;

        Debug.Log("CreateNNRoomReq sended!");

        // 设置总局数
        controller.nnRoom.totalCount.text = this.games.ToString();
    }

    void CheckNum10(bool isSelected)
    {
        if (isSelected)
        {
            games = 10;
            text10.color = selected;
            RefreshKeyNum();
            Debug.Log("10局");
        }
        else
        {
            text10.color = notSelect;
        }
    }
    void CheckNum20(bool isSelected)
    {
        if (isSelected)
        {
            games = 20;
            text20.color = selected;
            RefreshKeyNum();
            Debug.Log("20局");
        }
        else
        {
            text10.color = notSelect;
        }
    }
    void CheckNum30(bool isSelected)
    {
        if (isSelected)
        {
            games = 30;
            text30.color = selected;
            RefreshKeyNum();
            Debug.Log("30局");
        }
        else
        {
            text30.color = notSelect;
        }
    }

    void CheckPayLead(bool isSelected)
    {
        if (isSelected)
        {
            payType = 1;
            textpayLead.color = selected;
            RefreshKeyNum();
            Debug.Log("房主支付");
        }
        else
        {
            textpayLead.color = notSelect;
        }
    }
    void CheckPayAA(bool isSelected)
    {
        if (isSelected)
        {
            payType = 2;
            textpayAA.color = selected;
            RefreshKeyNum();
            Debug.Log("AA支付");
        }
        else
        {
            textpayAA.color = notSelect;
        }
    }

    void CheckPayWin(bool isSelected)
    {
        if (isSelected)
        {
            payType = 3;
            textpayWin.color = selected;
            RefreshKeyNum();
            Debug.Log("AA支付");
        }
        else
        {
            textpayWin.color = notSelect;
        }
    }

    private void RefreshKeyNum()
    {

        needKey.text = NeedKeyNum() + " 张";
        leaveKey.text = (controller.GetLeaveRoomCardNum - NeedKeyNum()) + " 张";
    }

    private int NeedKeyNum()
    {
        int needKeyNum = 1;

        switch (payType)
        {
            case 1: // 房主支付
            case 3: // 赢家支付
                needKeyNum = games / 2;  // (5 * games / 10)
                break;
            case 2: // AA支付
                needKeyNum = games / 10;  // (1 * games / 10)
                break;
            default:
                Debug.LogError("错误的房卡支付方式 " + payType);
                break;
        }
        return needKeyNum;
    }
}
