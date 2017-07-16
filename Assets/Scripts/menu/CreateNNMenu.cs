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
    private PayTypes payType = PayTypes.PT_PAY_AA;
    private BankerType m_bankerType = BankerType.BT_BAWANG;

    public Toggle checkNum10 = null;
    public Toggle checkNum20 = null;
    public Toggle checkNum30 = null;

    public Toggle checkPayLead = null;
    public Toggle checkPayAA = null;
    public Toggle checkPayWin = null;

    public Toggle checkbawang = null;
    public Toggle checklun = null;
    public Toggle checkzhuan = null;

    public Text text10 = null;
    public Text text20 = null;
    public Text text30 = null;

    public Text textpayLead = null;
    public Text textpayAA = null;
    public Text textpayWin = null;

    public Text textbawang = null;
    public Text textlun = null;
    public Text textzhuan = null;

    public Text needKey = null;
    public Text leaveKey = null;

    public Button createBtn;

    private Color notSelect;
    private Color selected;
    //public Games
    //{
    //AudioSettings{}
    //}

    void Start()
    {
        Debug.Log("CreateMenu Start!");

        createBtn.onClick.AddListener(CreateRoom);

        checkNum10.onValueChanged.AddListener(CheckNum10);
        checkNum20.onValueChanged.AddListener(CheckNum20);
        checkNum30.onValueChanged.AddListener(CheckNum30);

        checkPayLead.onValueChanged.AddListener(CheckPayLead);
        checkPayAA.onValueChanged.AddListener(CheckPayAA);
        checkPayWin.onValueChanged.AddListener(CheckPayWin);

        checkbawang.onValueChanged.AddListener(CheckBawang);
        checklun.onValueChanged.AddListener(CheckLun);
        checkzhuan.onValueChanged.AddListener(CheckZhuan);

        notSelect = text20.color;
        selected = text10.color;

        RefreshButtonStatus();

        RefreshKeyNum();

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
        CreateNNRoomReq createRoom = new CreateNNRoomReq();
        req.messageId = MESSAGE_ID.msg_CreateNNRoomReq;
        createRoom.games = this.games;
        createRoom.type = (int)this.payType;
        createRoom.bankerType = this.m_bankerType;
        createRoom.playerId = controller.PlayerId;
        req.createNNRoomReq = createRoom;

        PPSocket.GetInstance().SendMessage(req);

        controller.CloseWindow(WINDOW_ID.WINDOW_ID_HOME);
        controller.OpenWindow(WINDOW_ID.WINDOW_ID_GAME_NN);
        controller.LoadingStart();
        // 设置游戏局数
        room.GameCounts = this.games;
        room.PayType = this.payType;
        room.PayCount = NeedKeyNum();
        room.CurrentBankerType = this.m_bankerType;

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
            RefreshButtonStatus();
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
            RefreshButtonStatus();
            RefreshKeyNum();
            Debug.Log("20局");
        }
        else
        {
            text20.color = notSelect;
        }
    }
    void CheckNum30(bool isSelected)
    {
        if (isSelected)
        {
            games = 30;
            text30.color = selected;
            RefreshButtonStatus();
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
            payType = PayTypes.PT_PAY_ROOM_OWNER;
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
            payType = PayTypes.PT_PAY_AA;
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
            payType = PayTypes.PT_PAY_WIN;
            textpayWin.color = selected;
            RefreshKeyNum();
            Debug.Log("赢家支付");
        }
        else
        {
            textpayWin.color = notSelect;
        }
    }

    void CheckBawang(bool isSelected)
    {
        if (isSelected)
        {
            m_bankerType = BankerType.BT_BAWANG;
            textbawang.color = selected;
            Debug.Log("霸王庄");
        }
        else
        {
            textbawang.color = notSelect;
        }
    }
    void CheckLun(bool isSelected)
    {
        if (isSelected)
        {
            m_bankerType = BankerType.BT_LUNZHUANG;
            textlun.color = selected;
            Debug.Log("轮庄");
        }
        else
        {
            textlun.color = notSelect;
        }
    }

    void CheckZhuan(bool isSelected)
    {
        if (isSelected)
        {
            m_bankerType = BankerType.BT_ZHUANZHUANG;
            textzhuan.color = selected;
            Debug.Log("转庄");
        }
        else
        {
            textzhuan.color = notSelect;
        }
    }

    private void RefreshKeyNum()
    {

        needKey.text = string.Format(Strings.SS_CARD_NUMS, NeedKeyNum());
        leaveKey.text = string.Format(Strings.SS_CARD_NUMS, controller.GetLeaveRoomCardNum - NeedKeyNum());
    }

    private int NeedKeyNum()
    {
        int needKeyNum = 1;

        switch (payType)
        {
            case PayTypes.PT_PAY_ROOM_OWNER: // 房主支付
            case PayTypes.PT_PAY_WIN: // 赢家支付
                needKeyNum = 2 * games / 5;  // (5 * games / 10)
                break;
            case PayTypes.PT_PAY_AA: // AA支付
                needKeyNum = games / 10;  // (1 * games / 10)
                break;
            default:
                Debug.LogError("错误的房卡支付方式 " + payType);
                break;
        }
        return needKeyNum;
    }

    private void RefreshButtonStatus()
    {
        if (controller.GetLeaveRoomCardNum < 1)
        {
            createBtn.enabled = false;
            createBtn.GetComponent<Image>().material = ResourceManager.Instance.GreyMeterial;
            checkPayLead.interactable = false;
            checkPayWin.interactable = false;
            checkNum20.interactable = false;
            checkNum30.interactable = false;
            SetTextGrey(textpayLead, true);
            SetTextGrey(textpayWin, true);
            SetTextGrey(text20, true);
            SetTextGrey(text30, true);
        }
        else
        {
            createBtn.enabled = true;
            createBtn.GetComponent<Image>().material = null;

            if (controller.GetLeaveRoomCardNum < 2)
            {
                checkNum20.interactable = false;
                checkNum30.interactable = false;
                SetTextGrey(text20, true);
                SetTextGrey(text30, true);
            }
            else if (controller.GetLeaveRoomCardNum < 3)
            {
                checkNum20.interactable = true;
                SetTextGrey(text20, false);

                checkNum30.interactable = false;
                SetTextGrey(text30, true);
            }
            else
            {
                checkNum20.interactable = true;
                checkNum30.interactable = true;
                SetTextGrey(text20, false);
                SetTextGrey(text30, false);
            }

            switch (games)
            {
                case 10:
                    if (controller.GetLeaveRoomCardNum < 4)
                    {
                        checkPayLead.interactable = false;
                        checkPayWin.interactable = false;
                        SetTextGrey(textpayLead, true);
                        SetTextGrey(textpayWin, true);
                        checkPayAA.isOn = true;
                    }
                    else
                    {
                        checkPayLead.interactable = true;
                        checkPayWin.interactable = true;
                        SetTextGrey(textpayLead, false);
                        SetTextGrey(textpayWin, false);
                    }
                    break;
                case 20:
                    if (controller.GetLeaveRoomCardNum < 8)
                    {
                        checkPayLead.interactable = false;
                        checkPayWin.interactable = false;
                        SetTextGrey(textpayLead, true);
                        SetTextGrey(textpayWin, true);
                        checkPayAA.isOn = true;
                    }
                    else
                    {
                        checkPayLead.interactable = true;
                        checkPayWin.interactable = true;
                        SetTextGrey(textpayLead, false);
                        SetTextGrey(textpayWin, false);
                    }
                    break;
                case 30:
                    if (controller.GetLeaveRoomCardNum < 12)
                    {
                        checkPayLead.interactable = false;
                        checkPayWin.interactable = false;
                        SetTextGrey(textpayLead, true);
                        SetTextGrey(textpayWin, true);
                        checkPayAA.isOn = true;
                    }
                    else
                    {
                        checkPayLead.interactable = true;
                        checkPayWin.interactable = true;
                        SetTextGrey(textpayLead, false);
                        SetTextGrey(textpayWin, false);
                    }
                    break;
            }
        }
    }

    private void SetTextGrey(Text txt, bool flag)
    {
        if (flag)
        {
            txt.color = Color.gray;
        }
        else
        {
            txt.color = notSelect;
        }
    }
}
