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

    // Use this for initialization
    private int baseKeyNum = 1;
    private int currentKeyNum = 100;

    private int games = 10;
    private int payType = 1;

    public Toggle checkNum10 = null;
    public Toggle checkNum20 = null;
    public Toggle checkNum30 = null;

    public Toggle checkPaySelf = null;
    public Toggle checkPayAA = null;

    public Text text10 = null;
    public Text text20 = null;
    public Text text30 = null;

    public Text textpaySelf = null;
    public Text textpayAA = null;

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

        checkPaySelf.onValueChanged.AddListener(CheckPaySelf);
        checkPayAA.onValueChanged.AddListener(CheckPayAA);

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

    void CheckPaySelf(bool isSelected)
    {
        if (isSelected)
        {
            payType = 1;
            textpaySelf.color = selected;
            RefreshKeyNum();
            Debug.Log("房主支付");
        }
        else
        {
            textpaySelf.color = notSelect;
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

    void RefreshKeyNum()
    {
        int needKeyNum = baseKeyNum * games / 10;
        if (payType == 1)
        {
            needKeyNum *= 3;
        }
        needKey.text = needKeyNum + " 张";
        leaveKey.text = (currentKeyNum - needKeyNum) + " 张";
    }
}
