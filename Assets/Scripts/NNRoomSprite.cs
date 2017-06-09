using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.Scripts;
using System;
using netty;

public class NNRoomSprite : MonoBehaviour
{
    public NNPlayerSprite playerSelf;
    public NNPlayerSprite player2;
    public NNPlayerSprite player3;
    public NNPlayerSprite player4;
    public NNPlayerSprite player5;

    // 局数
    public Text currentCount;
    public Text totalCount;
    // 房间号
    public Text roomId;

    public Button backHome;
    public Button setting;
    public Button chatBtn;

    public HomeController controller;
    public NNOperationsMenu operations;

    // 解散房间
    public Dissolution dissolution;

    public SoundPlayer soundPlayer;

    public GameObject chatChooseWnd;

    private int m_roomId;

    private int games = 0;

    /// <summary>
    /// 创建房间游戏局数
    /// </summary>
    public int GameCounts
    {
        set { games = value; }
        get { return games; }
    }

    public int GetRoomId
    {
        get { return m_roomId; }
    }

    //private int _playedGameNum = 0;
    //private int _totalGameNum = 10;

    //public int PlayedGameNum
    //{
    //    set { _playedGameNum = value; }
    //    get { return _playedGameNum; }
    //}

    //public int TotalGameNum
    //{
    //    set { _totalGameNum = value; }
    //    get { return _totalGameNum; }
    //}

	// Use this for initialization
	void Start () {
        backHome.onClick.AddListener(Disband);
        setting.onClick.AddListener(Setting);
        chatBtn.onClick.AddListener(OpenChatChooseWnd);
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void Disband()
    {
        controller.ShowDialog("是否确定解散房间？", AgreeDisband);
    }

    public void AgreeDisband()
    {
        if (GameObject.Find("Dissolution") != null)
        {
            return;
        }
        MessageInfo req = new MessageInfo();
        req.messageId = MESSAGE_ID.msg_NNDissolutionReq;
        NNDissolutionReq disReq = new NNDissolutionReq();
        disReq.playerId = controller.PlayerId;
        req.nnDissolutionReq = disReq;

        PPSocket.GetInstance().SendMessage(req);

        dissolution.buttonContainer.SetActive(false);
    }

    void Setting()
    {
        controller.OpenWindow(WINDOW_ID.WINDOW_ID_SETTING);
    }

    void OpenChatChooseWnd()
    {
        chatChooseWnd.SetActive(true);
    }
    public void OnPostEnterRoom(PostNNEntryRoom msg)
    {
        ClientPlayerInfo cpi = new ClientPlayerInfo(false);
		cpi.CopyFrom (msg.player);
        BindPlayerInfoToSprite(cpi);
    }

    public void OnCreateRoom(ClientPlayerInfo self, int _roomId)
    {
        m_roomId = _roomId;
        roomId.text = m_roomId.ToString();
        playerSelf.PlayerInfo = self;
        currentCount.text = "1";
        
        operations.ShowMenuGroup(NNOperationGroup.NNOG_Play);

        InitPlayerUI();
    }
    /// <summary>
    /// 初始化玩家ui，默认都隐藏，除了自己
    /// </summary>
    private void InitPlayerUI()
    {
        player2.gameObject.SetActive(false);
        player3.gameObject.SetActive(false);
        player4.gameObject.SetActive(false);
        player5.gameObject.SetActive(false);
    }

    public void OnEnterRoom(ClientPlayerInfo self, EntryNNRoomResp msg)
    {
        m_roomId = msg.roomInfo.roomId;
        roomId.text = m_roomId.ToString();
        self.Order = msg.order;
        playerSelf.PlayerInfo = self;
        currentCount.text = msg.roomInfo.playedGames.ToString();
        totalCount.text = msg.roomInfo.totalGames.ToString();

        InitPlayerUI();

        foreach (Player p in msg.roomInfo.players)
        {
            ClientPlayerInfo cpi = new ClientPlayerInfo(false);
            cpi.CopyFrom(p);
            BindPlayerInfoToSprite(cpi);
        }
    }

    public void OnPostDealResp(PostNNDealResp msg)
    {
        playerSelf.OnPostDealResp(msg);
        // 局数
        currentCount.text = msg.playedGames.ToString();
        totalCount.text = msg.totalGames.ToString();

        player2.ShowCardBack();
        player3.ShowCardBack();
        player4.ShowCardBack();
        player5.ShowCardBack();

        if (!playerSelf.IsBanker)
        {
            operations.ShowMenuGroup(NNOperationGroup.NNOG_Yafen);
        }

        // 影藏掉OK手势
        player2.ShowOk(false);
        player3.ShowOk(false);
        player4.ShowOk(false);
        player5.ShowOk(false);
        playerSelf.ShowOk(false);
    }

    public void OnStakeResp(StakeResp msg)
    {
        playerSelf.ShowStake(msg.point);
        operations.ShowMenuGroup(NNOperationGroup.NNOG_None);
    }

    public void OnPostStakeResp(PostStakeResp msg)
    {
        if (player2.PlayerInfo != null && msg.playerid == player2.PlayerInfo.PlayerId)
        {
            player2.ShowStake(msg.point);
        }
        
        if (player3.PlayerInfo != null && msg.playerid == player3.PlayerInfo.PlayerId)
        {
            player3.ShowStake(msg.point);
        }
        
        if (player3.PlayerInfo != null && msg.playerid == player4.PlayerInfo.PlayerId)
        {
            player4.ShowStake(msg.point);
        }
        
        if (player3.PlayerInfo != null && msg.playerid == player5.PlayerInfo.PlayerId)
        {
            player5.ShowStake(msg.point);
        }
    }

    public void OnStartGame()
    {
        operations.ShowMenuGroup(NNOperationGroup.NNOG_None);
    }

    public void OnPostStartGame()
    {
        operations.ShowMenuGroup(NNOperationGroup.NNOG_Ready);
    }

    public void OnShowCardsResp(NNShowCardsResp msg)
    {
        playerSelf.OnShowCards(msg);
    }

    public void OnPostNNShowCards(PostNNShowCards msg)
    {
        if (player2.PlayerInfo != null && msg.playerId == player2.PlayerInfo.PlayerId)
        {
            player2.OnPostShowCards(msg);
        }
        
        if (player3.PlayerInfo != null && msg.playerId == player3.PlayerInfo.PlayerId)
        {
            player3.OnPostShowCards(msg);
        }
        
        if (player4.PlayerInfo != null && msg.playerId == player4.PlayerInfo.PlayerId)
        {
            player4.OnPostShowCards(msg);
        }
        
        if (player5.PlayerInfo != null && msg.playerId == player5.PlayerInfo.PlayerId)
        {
            player5.OnPostShowCards(msg);
        }

        if (msg.playerId == playerSelf.PlayerInfo.PlayerId)
        {
            playerSelf.OnPostShowCards(msg);
            // 播放音效
            soundPlayer.PlayeNNSound(msg.nntype);
        }
    }

    public void OnPostNNPrepareResp(PostNNPrepareResp msg)
    {
        if (player2.PlayerInfo != null && msg.playerId == player2.PlayerInfo.PlayerId)
        {
            player2.ShowOk(true);
        }

        if (player3.PlayerInfo != null && msg.playerId == player3.PlayerInfo.PlayerId)
        {
            player3.ShowOk(true);
        }

        if (player4.PlayerInfo != null && msg.playerId == player4.PlayerInfo.PlayerId)
        {
            player4.ShowOk(true);
        }

        if (player5.PlayerInfo != null && msg.playerId == player5.PlayerInfo.PlayerId)
        {
            player5.ShowOk(true);
        }

        if (playerSelf.PlayerInfo != null && msg.playerId == playerSelf.PlayerInfo.PlayerId)
        {
            playerSelf.ShowOk(true);
            operations.ShowMenuGroup(NNOperationGroup.NNOG_None);
        }
    }

    private void BindPlayerInfoToSprite(ClientPlayerInfo cpi)
    {
        // 比自己先进房间的玩家
        switch (playerSelf.PlayerInfo.Order - cpi.Order)
        {
            case 4:
            player2.PlayerInfo = cpi;
            return;
            case 3:
            player3.PlayerInfo = cpi;
            return;
            case 2:
            player4.PlayerInfo = cpi;
            return;
            case 1:
            player5.PlayerInfo = cpi;
            return;
            default:
            break;
        }
        // 比自己后进房间
        switch (cpi.Order - playerSelf.PlayerInfo.Order)
        {
            case 4:
                player5.PlayerInfo = cpi;
                return;
            case 3:
                player4.PlayerInfo = cpi;
                return;
            case 2:
                player3.PlayerInfo = cpi;
                return;
            case 1:
                player2.PlayerInfo = cpi;
                return;
            default:
                break;
        }
    }
    // 继续下一局
    public void Again()
    {
        // 打开准备界面
        operations.ShowMenuGroup(NNOperationGroup.NNOG_Ready);
        // 创景恢复初始状态
        player2.Again();
        player3.Again();
        player4.Again();
        player5.Again();
        playerSelf.Again();
    }

    public void Settlement(SettlementInfo sInfo, SettlementOne oneUI)
    {
        foreach (SettlementData data in sInfo.players)
        {
            UpdateNewScore(data.ID, data.finalscore);
            if (data.ID == playerSelf.PlayerInfo.PlayerId)
            {
                oneUI.SetSettlementUI(data.gotscore);
            }
        }
    }

    private void UpdateNewScore(int playerId, int score)
    {
        if (player2.PlayerInfo != null && playerId == player2.PlayerInfo.PlayerId)
        {
            player2.playerScore.text = score.ToString();
        }

        if (player3.PlayerInfo != null && playerId == player3.PlayerInfo.PlayerId)
        {
            player3.playerScore.text = score.ToString();
        }

        if (player4.PlayerInfo != null && playerId == player4.PlayerInfo.PlayerId)
        {
            player4.playerScore.text = score.ToString();
        }

        if (player5.PlayerInfo != null && playerId == player5.PlayerInfo.PlayerId)
        {
            player5.playerScore.text = score.ToString();
        }

        if (playerSelf.PlayerInfo != null && playerId == playerSelf.PlayerInfo.PlayerId)
        {
            playerSelf.playerScore.text = score.ToString();
        }
    }

    // 下注结束，可以走开牌阶段
    public void OnPostStakeOver()
    {
        operations.ShowMenuGroup(NNOperationGroup.NNOG_Open);
    }

    public ClientPlayerInfo GetPlayer(int id)
    {
        if (player2.PlayerInfo != null && id == player2.PlayerInfo.PlayerId)
        {
            return player2.PlayerInfo;
        }
        if (player3.PlayerInfo != null && id == player3.PlayerInfo.PlayerId)
        {
            return player3.PlayerInfo;
        }
        if (player4.PlayerInfo != null && id == player4.PlayerInfo.PlayerId)
        {
            return player4.PlayerInfo;
        }
        if (player5.PlayerInfo != null && id == player5.PlayerInfo.PlayerId)
        {
            return player5.PlayerInfo;
        }
        if (playerSelf.PlayerInfo != null && id == playerSelf.PlayerInfo.PlayerId)
        {
            return playerSelf.PlayerInfo;
        }

        return null;
    }

    public void OnPostDissolution()
    {
        dissolution.Show(true);
    }
    public void OnPostAnswerDissolution(PostAnswerDissolutionResult msg)
    {
        dissolution.OnPostAnswerDissolution(msg);
    }

    public void OnPostDissolutionResult()
    {
        controller.OpenWindow(WINDOW_ID.WINDOW_ID_HOME);
        dissolution.Show(false);
        this.gameObject.SetActive(false);
    }

    public void OnPostSendSoundResp(PostSendSoundResp msg)
    {
        if (player2.PlayerInfo != null && msg.playerId == player2.PlayerInfo.PlayerId)
        {
            soundPlayer.OnPostSendSoundResp(2, msg.soundId);
        }
        if (player3.PlayerInfo != null && msg.playerId == player3.PlayerInfo.PlayerId)
        {
            soundPlayer.OnPostSendSoundResp(3, msg.soundId);
        }
        if (player4.PlayerInfo != null && msg.playerId == player4.PlayerInfo.PlayerId)
        {
            soundPlayer.OnPostSendSoundResp(4, msg.soundId);
        }
        if (player5.PlayerInfo != null && msg.playerId == player5.PlayerInfo.PlayerId)
        {
            soundPlayer.OnPostSendSoundResp(5, msg.soundId);
        }
        if (playerSelf.PlayerInfo != null && msg.playerId == playerSelf.PlayerInfo.PlayerId)
        {
            soundPlayer.OnPostSendSoundResp(1, msg.soundId);
        }
    }

    public void HideChatChooseWnd()
    {
        chatChooseWnd.SetActive(false);
    }
}
