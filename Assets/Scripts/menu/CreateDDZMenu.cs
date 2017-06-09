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
public class CreateDDZMenu : MonoBehaviour
{
    public HomeController controller;

    // Use this for initialization
    private int baseKeyNum = 1;
    private int currentKeyNum = 100;

    private int games = 5;
    private int payType = 1;

    private GameObject checkNum5 = null;
    private GameObject checkNum10 = null;
    private GameObject checkNum15 = null;

    private GameObject checkPay1 = null;
    private GameObject checkPay3 = null;

    private Text text5 = null;
    private Text text10 = null;
    private Text text15 = null;

    private Text textpay1 = null;
    private Text textpay3 = null;

    private Color notSelect;
    private Color selected;

    private Text needKey = null;
    private Text leaveKey = null;
    //public Games
    //{
    //AudioSettings{}
    //}

    void Start()
    {
        Debug.Log("CreateMenu Start!");
        transform.Find("bg/CreateBtn").gameObject.GetComponent<Button>().onClick.AddListener(CreateRoom);

        transform.Find("bg/Close").gameObject.GetComponent<Button>().onClick.AddListener(Close);

        checkNum5 = transform.Find("bg/games/5").gameObject;
        checkNum10 = transform.Find("bg/games/10").gameObject;
        checkNum15 = transform.Find("bg/games/15").gameObject;

        checkPay1 = transform.Find("bg/pay/1").gameObject;
        checkPay3 = transform.Find("bg/pay/3").gameObject;

        checkNum5.GetComponent<Toggle>().onValueChanged.AddListener(CheckNum5);
        checkNum10.GetComponent<Toggle>().onValueChanged.AddListener(CheckNum10);
        checkNum15.GetComponent<Toggle>().onValueChanged.AddListener(CheckNum15);

        checkPay1.GetComponent<Toggle>().onValueChanged.AddListener(CheckPay1);
        checkPay3.GetComponent<Toggle>().onValueChanged.AddListener(CheckPay3);

        text5 = checkNum5.transform.Find("Label").GetComponent<Text>();
        text10 = checkNum10.transform.Find("Label").GetComponent<Text>();
        text15 = checkNum15.transform.Find("Label").GetComponent<Text>();

        notSelect = text10.color;
        selected = text5.color;

        textpay1 = checkPay1.transform.Find("Label").GetComponent<Text>();
        textpay3 = checkPay3.transform.Find("Label").GetComponent<Text>();

        needKey = transform.Find("bg/Needkey/Num").gameObject.GetComponent<Text>();
        leaveKey = transform.Find("bg/Leavekey/Num").gameObject.GetComponent<Text>();

        needKey.text = 3 + " 张";
        leaveKey.text = 97 + " 张";

        Debug.Log("CreateMenu Start success!");
    }

    /// <summary>
    /// 创建房间
    /// </summary>
    void CreateRoom()
    {
        MessageInfo req = new MessageInfo();
        CreateRoomReq creatRoom = new CreateRoomReq();
        req.messageId = MESSAGE_ID.msg_CreateRoomReq;
        creatRoom.games = this.games;
        creatRoom.type = this.payType;
        creatRoom.playerId = controller.PlayerId;
        req.createRoomReq = creatRoom;

        PPSocket.GetInstance().SendMessage(req);

        controller.CloseWindow(WINDOW_ID.WINDOW_ID_HOME);
        controller.OpenWindow(WINDOW_ID.WINDOW_ID_GAME_DDZ);
        controller.LoadingStart();
        Close();

        Debug.Log("CreateRoomReq sended!");
    }

    void Close()
    {
        transform.gameObject.SetActive(false);
    }

    void CheckNum5(bool isSelected)
    {
        if (isSelected)
        {
            games = 5;
            text5.color = selected;
            RefreshKeyNum();
            Debug.Log("5局");
        }
        else
        {
            text5.color = notSelect;
        }
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
    void CheckNum15(bool isSelected)
    {
        if (isSelected)
        {
            games = 15;
            text15.color = selected;
            RefreshKeyNum();
            Debug.Log("15局");
        }
        else
        {
            text15.color = notSelect;
        }
    }

    void CheckPay1(bool isSelected)
    {
        if (isSelected)
        {
            payType = 1;
            textpay1.color = selected;
            RefreshKeyNum();
            Debug.Log("房主支付");
        }
        else
        {
            textpay1.color = notSelect;
        }
    }
    void CheckPay3(bool isSelected)
    {
        if (isSelected)
        {
            payType = 2;
            textpay3.color = selected;
            RefreshKeyNum();
            Debug.Log("AA支付");
        }
        else
        {
            textpay3.color = notSelect;
        }
    }

    void RefreshKeyNum()
    {
        int needKeyNum = baseKeyNum * games / 5;
        if (payType == 1)
        {
            needKeyNum *= 3;
        }
        needKey.text = needKeyNum + " 张";
        leaveKey.text = (currentKeyNum - needKeyNum) + " 张";
    }
}
