using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using netty;
using System;

public class NNOperationsMenu : MonoBehaviour
{
    public GameObject playgame;
    public GameObject yafen;
    public GameObject kaipai;
    public GameObject ready;

    public Button disbandBtn;
    public Button inviteBtn;
    public Button startBtn;

    public Button grabBtn1;
    public Button grabBtn2;
    public Button grabBtn3;
    public Button grabBtn4;
    public Button grabBtn5;

    public Button viewBtn;
    public Button openBtn;

    public Button readyBtn;

    public NNRoomSprite room;

    public ThirdParty thirdParty;

	// Use this for initialization
    void Start()
    {
        disbandBtn.onClick.AddListener(Disband);
        inviteBtn.onClick.AddListener(Invite);
        startBtn.onClick.AddListener(StartGame);
        grabBtn1.onClick.AddListener(Grab1);
        grabBtn2.onClick.AddListener(Grab2);
        grabBtn3.onClick.AddListener(Grab3);
        grabBtn4.onClick.AddListener(Grab4);
        grabBtn5.onClick.AddListener(Grab5);
        viewBtn.onClick.AddListener(ViewPokers);
        openBtn.onClick.AddListener(OpenPokers);
        readyBtn.onClick.AddListener(Ready);
	}

    /// <summary>
    /// 关闭窗口
    /// </summary>
    void Grab1()
    {
        Grab(1);
    }
    void Grab2()
    {
        Grab(2);
    }
    void Grab3()
    {
        Grab(3);
    }
    void Grab4()
    {
        Grab(4);
    }
    void Grab5()
    {
        Grab(5);
    }
    // 压分
    private void Grab(int score)
    {
        MessageInfo req = new MessageInfo();
        req.messageId = MESSAGE_ID.msg_StakeReq;
        StakeReq stake = new StakeReq();
        stake.playerid = room.playerSelf.PlayerInfo.PlayerId;
        stake.point = score;
        req.stakeReq = stake;

        PPSocket.GetInstance().SendMessage(req);

    }

    // 解散房间
    private void Disband()
    {
        room.Disband();
    }

    // 邀请微信好友
    private void Invite()
    {
        thirdParty.ThirdPartyShare(String.Format(Strings.SS_SHARE_ROOMINFO1, room.GetRoomId, room.GameCounts, Strings.SS_PAY_TYPS[(int)room.PayType]), String.Format(Strings.SS_SHARE_ROOMINFO2, Strings.SS_BANKER_TYPES[(int)room.CurrentBankerType], room.PayCount), 1);
    }

    // 开始游戏
    private void StartGame()
    {
        MessageInfo req = new MessageInfo();
        req.messageId = MESSAGE_ID.msg_StartNNGameReq;
        StartNNGameReq startGame = new StartNNGameReq();
        startGame.playerid = room.playerSelf.PlayerInfo.PlayerId;
        req.startNNGame = startGame;

        PPSocket.GetInstance().SendMessage(req);

        ShowMenuGroup(NNOperationGroup.NNOG_None);
    }

    // 开牌
    void OpenPokers()
    {
        MessageInfo req = new MessageInfo();
        req.messageId = MESSAGE_ID.msg_NNShowCardsReq;
        NNShowCardsReq show = new NNShowCardsReq();
        show.playerid = room.playerSelf.PlayerInfo.PlayerId;
        show.showAll = true;
        req.nnShowCardsReq = show;

        PPSocket.GetInstance().SendMessage(req);

        ShowMenuGroup(NNOperationGroup.NNOG_None);
    }
    // 看牌
    void ViewPokers()
    {
        MessageInfo req = new MessageInfo();
        req.messageId = MESSAGE_ID.msg_NNShowCardsReq;
        NNShowCardsReq show = new NNShowCardsReq();
        show.playerid = room.playerSelf.PlayerInfo.PlayerId;
        show.showAll = false;
        req.nnShowCardsReq = show;

        PPSocket.GetInstance().SendMessage(req);

        viewBtn.gameObject.SetActive(false);
    }
    // 准备
    void Ready()
    {
        MessageInfo req = new MessageInfo();
        NNPrepareReq prepare = new NNPrepareReq();
        req.messageId = MESSAGE_ID.msg_NNPrepareReq;
        prepare.playerId = room.playerSelf.PlayerInfo.PlayerId;
        req.nnPrepareReq = prepare;

        PPSocket.GetInstance().SendMessage(req);

        ShowMenuGroup(NNOperationGroup.NNOG_None);
    }

    public void ShowMenuGroup(NNOperationGroup group)
    {
        switch (group)
        {
            case NNOperationGroup.NNOG_None:
                playgame.SetActive(false);
                yafen.SetActive(false);
                kaipai.SetActive(false);
                ready.SetActive(false);
                break;
            case NNOperationGroup.NNOG_Play:
                playgame.SetActive(true);
                yafen.SetActive(false);
                kaipai.SetActive(false);
                ready.SetActive(false);
                startBtn.GetComponent<Image>().material = ResourceManager.Instance.GreyMeterial;
                startBtn.enabled = false;
                break;
            case NNOperationGroup.NNOG_Yafen:
                playgame.SetActive(false);
                yafen.SetActive(true);
                kaipai.SetActive(false);
                ready.SetActive(false);
                break;
            case NNOperationGroup.NNOG_Open:
                playgame.SetActive(false);
                yafen.SetActive(false);
                kaipai.SetActive(true);
                ready.SetActive(false);
                viewBtn.gameObject.SetActive(true);
                openBtn.gameObject.SetActive(true);
                break;
            case NNOperationGroup.NNOG_Ready:
                playgame.SetActive(false);
                yafen.SetActive(false);
                kaipai.SetActive(false);
                ready.SetActive(true);
                break;
            default:
                break;
        }
    }
}
