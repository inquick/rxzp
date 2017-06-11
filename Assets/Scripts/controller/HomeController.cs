using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using UnityEngine.UI;
using System.Threading;
using netty;
using System;

public class HomeController : MonoBehaviour
{
    // 游戏类型 (1 斗地主  2 斗牛)
    private WINDOW_ID _windowId = WINDOW_ID.WINDOW_ID_LOGIN;

    public ServerIp ipIndex = ServerIp.IP_NN_Server;

    public GameController gameController;

    public NNRoomSprite nnRoom;

    public SoundPlayer _soundPlayer;

    public SelfInfos _selfInfo;

    public Text DialogContentText;
    public Button DialogOKBtn;

    private GameType _gameType = GameType.GT_NONE;

    private string shareUrl = null;

    public string ShareUrl
    {
        set { shareUrl = value; }
        get { return shareUrl; }
    }

    public WINDOW_ID WindowId
    {
        get { return _windowId;}
        set { _windowId = value; }
    }

    public GameType GetGameType
    {
        get { return _gameType; }
    }

    public int GetLeaveRoomCardNum
    {
        get { return ddzRoom.SelfPlayer.LeaveCardCount; } 
    }

    /// <summary>
    /// 服务器ip
    /// </summary>
    private string[] serverIps = { "127.0.0.1", "139.129.98.110", "127.0.0.1", "139.129.98.110", "192.168.1.104", "http://rgk5ej.natappfree.cc" };
    /// <summary>
    /// 服务器端口
    /// </summary>
    private int[] serverPoint = { 7778, 7778, 9090, 9090, 9090, 9090 };

    public string GetServerIP
    {
        get { return serverIps[(int)ipIndex]; }
    }

    public int GetServerPoint
    {
        get { return serverPoint[(int)ipIndex]; }
    }

	// 网络包
	private List<MessageInfo> packages;
	// 当前打开的控件，UIRoot的子控件
    private Dictionary<WINDOW_ID, GameObject> windows;

    public Text TipsNoticeLabel;
    // 玩家id
    public int PlayerId
    {
        get { return ddzRoom.SelfPlayer.PlayerId; }
        set { ddzRoom.SelfPlayer.PlayerId = value; }
    }
    // 玩家
    public string PlayerName
    {
        get { return ddzRoom.SelfPlayer.Name; }
    }
    // 游戏房间数据
    private ClientRoomInfo ddzRoom = new ClientRoomInfo();

    // 抢地主
    public GameObject btngroup1;
    // 出牌
    public GameObject btngroup2;
    // 屏蔽点击事件panel
    public GameObject RaycastTargetPanel;
    
    // Loading界面
    private Loading loading;

    public SettlementOne settlementOne;
    public SettlementFinal settlementFinal;
    public NNSettlementFinal nnSettlementFinal;

	public int RoomId
	{
        set
        {
            ddzRoom.RoomId = value;
        }
		get
        {
            if (ddzRoom != null)
                return ddzRoom.RoomId;

            return 0;
        }
	}

    public ClientRoomInfo RoomInfo
    {
        get { return ddzRoom; }
    }

    // 上家出牌规则
    private int lastWeight;
    private CardsType lastCardType;

    private DizhuCardsSprite dizhuCardsSprite;


    // Use this for initialization
    void Start()
    {
        windows = new Dictionary<WINDOW_ID, GameObject>();
        
        windows.Add(WINDOW_ID.WINDOW_ID_ROOT, GameObject.Find("UIRoot"));
        windows.Add(WINDOW_ID.WINDOW_ID_LOGIN, windows[WINDOW_ID.WINDOW_ID_ROOT].transform.Find("WndLogin").gameObject);
        windows.Add(WINDOW_ID.WINDOW_ID_HOME, windows[WINDOW_ID.WINDOW_ID_ROOT].transform.Find("WndHome").gameObject);
        windows.Add(WINDOW_ID.WINDOW_ID_SHOP, windows[WINDOW_ID.WINDOW_ID_ROOT].transform.Find("WndShop").gameObject);
        windows.Add(WINDOW_ID.WINDOW_ID_HELP, windows[WINDOW_ID.WINDOW_ID_ROOT].transform.Find("WndHelp").gameObject);
        windows.Add(WINDOW_ID.WINDOW_ID_CREATE_DDZ, windows[WINDOW_ID.WINDOW_ID_ROOT].transform.Find("WndCreateDDZ").gameObject);
        windows.Add(WINDOW_ID.WINDOW_ID_CREATE_NN, windows[WINDOW_ID.WINDOW_ID_ROOT].transform.Find("WndCreateNN").gameObject);
        windows.Add(WINDOW_ID.WINDOW_ID_ENTER_DDZ, windows[WINDOW_ID.WINDOW_ID_ROOT].transform.Find("WndEnter").gameObject);
        windows.Add(WINDOW_ID.WINDOW_ID_ENTER_NN, windows[WINDOW_ID.WINDOW_ID_ENTER_DDZ]);
        windows.Add(WINDOW_ID.WINDOW_ID_GAME_DDZ, windows[WINDOW_ID.WINDOW_ID_ROOT].transform.Find("WndGameDDZ").gameObject);
        windows.Add(WINDOW_ID.WINDOW_ID_GAME_NN, windows[WINDOW_ID.WINDOW_ID_ROOT].transform.Find("WndGameNN").gameObject);
        windows.Add(WINDOW_ID.WINDOW_ID_SETTLEMENT_ONE, windows[WINDOW_ID.WINDOW_ID_ROOT].transform.Find("WndSettlementOne").gameObject);
        windows.Add(WINDOW_ID.WINDOW_ID_DDZSETTLEMENT_FINAL, windows[WINDOW_ID.WINDOW_ID_ROOT].transform.Find("WndDDZSettlementFinal").gameObject);
        windows.Add(WINDOW_ID.WINDOW_ID_NNSETTLEMENT_FINAL, windows[WINDOW_ID.WINDOW_ID_ROOT].transform.Find("WndNNSettlementFinal").gameObject);
        windows.Add(WINDOW_ID.WINDOW_ID_SETTING, windows[WINDOW_ID.WINDOW_ID_ROOT].transform.Find("WndSetting").gameObject);
        windows.Add(WINDOW_ID.WINDOW_ID_LOADING, windows[WINDOW_ID.WINDOW_ID_ROOT].transform.Find("WndLoading").gameObject);
        windows.Add(WINDOW_ID.WINDOW_ID_TIP, windows[WINDOW_ID.WINDOW_ID_ROOT].transform.Find("WndTips").gameObject);
        windows.Add(WINDOW_ID.WINDOW_ID_DIALONG_FINAL, windows[WINDOW_ID.WINDOW_ID_ROOT].transform.Find("WndDialong").gameObject);

        PlayerSprite selfPlayerSprite = windows[WINDOW_ID.WINDOW_ID_GAME_DDZ].transform.FindChild("PlayerSelf").GetComponent<PlayerSprite>();
        PlayerSprite leftPlayerSprite = windows[WINDOW_ID.WINDOW_ID_GAME_DDZ].transform.FindChild("PlayerLeft").GetComponent<PlayerSprite>();
        PlayerSprite rightPlayerSprite = windows[WINDOW_ID.WINDOW_ID_GAME_DDZ].transform.FindChild("PlayerRight").GetComponent<PlayerSprite>();
        ddzRoom.SetPlayerSprite(selfPlayerSprite, leftPlayerSprite, rightPlayerSprite);

        // 初始化自己的默认信息
        ddzRoom.SelfPlayer.IsDZ = false;
        ddzRoom.SelfPlayer.Name = ddzRoom.SelfPlayer.PlayerId.ToString();
        ddzRoom.SelfPlayer.SetInterActionMenu = windows[WINDOW_ID.WINDOW_ID_GAME_DDZ].transform.FindChild("InteractionPanel").GetComponent<InterActionMenu>();

        loading = windows[WINDOW_ID.WINDOW_ID_LOADING].GetComponent<Loading>();

        dizhuCardsSprite = windows[WINDOW_ID.WINDOW_ID_GAME_DDZ].transform.FindChild("DizhuCards").GetComponent<DizhuCardsSprite>();

        // 加载扑克牌资源
        ResourceManager.Instance.LoadGameResources();
    }

    /// <summary>
    /// 添加新收到的网络数据包
    /// </summary>
    /// <param name="newPackages"></param>
	public void AddNewPackage(List<MessageInfo> newPackages)
	{
		if (packages == null) {
			packages = new List<MessageInfo> ();
		}
		lock(packages)
		foreach(MessageInfo package in newPackages)
		{
            Debug.Log("收到新的数据包 msgId = " + package.messageId);
			packages.Add (package);
		}
	}
    /// <summary>
    /// 处理网络数据包
    /// </summary>
    /// <param name="msg"></param>
	private void HandlePackage(MessageInfo msg)
	{
        switch(msg.messageId)
        {
		case MESSAGE_ID.msg_LoginResp:
			GoHome(msg.loginResp);
            break;
        case MESSAGE_ID.msg_CreateRoomResp:
			EnterGameDDZ(msg);
			Debug.Log("创建房间成功 RoomID=" + msg.createRoomResp.roomId);
            break;
        case MESSAGE_ID.msg_EntryRoomResp:
            EnterGameDDZ(msg);
            Debug.Log("进入房间成功！");
            break;
        case MESSAGE_ID.msg_PostEntryRoom:
            OnPostEntryInfo(msg);
            break;
        case MESSAGE_ID.msg_PostDisband:
            break;
        case MESSAGE_ID.msg_PostDisbandCheck:
            CloseWindow(WINDOW_ID.WINDOW_ID_GAME_DDZ);
            OpenWindow(WINDOW_ID.WINDOW_ID_HOME);
            break;
        case MESSAGE_ID.msg_PostDiscard:
            OnPostDiscard(msg);
            break;
        case MESSAGE_ID.msg_DealResp:
            OnDealResp(msg);
            break;
        case MESSAGE_ID.msg_PostGrabHostResp:
            OnPostGrabHostResp(msg);
            break;
        case MESSAGE_ID.msg_PostDealOver:
            OnPostDealOver(msg);
            break;
        case MESSAGE_ID.msg_MsgInfo:
            ShowTips("ErrorCode : " + msg.msgInfo.type + " " + msg.msgInfo.message);
            LoadingEnd();
            break;
        case MESSAGE_ID.msg_SettlementInfo:
            // 结算
            Settlement(msg);
            break;
////////////////// 一下牛牛相关 ////////////////////////
        case MESSAGE_ID.msg_CreateNNRoomResp:
            {
                CloseWindow(WINDOW_ID.WINDOW_ID_CREATE_NN);
                LoadingEnd();
                ddzRoom.SelfPlayer.Order = 1;
                nnRoom.OnCreateRoom(ddzRoom.SelfPlayer, msg.createNNRoomResp.roomId);
                nnRoom.backHome.gameObject.SetActive(true);

                // 播放牛牛背景音乐
                _soundPlayer.PlayNNBackgroundMusic();
                // 创建牛牛游戏房间
                _gameType = GameType.GT_NN;
            }
            break;
        case MESSAGE_ID.msg_EntryNNRoomResp:
            {
                CloseWindow(WINDOW_ID.WINDOW_ID_HOME);
                OpenWindow(WINDOW_ID.WINDOW_ID_GAME_NN);
                nnRoom.OnEnterRoom(ddzRoom.SelfPlayer, msg.entryNNRoomResp);

                // 播放牛牛背景音乐
                _soundPlayer.PlayNNBackgroundMusic();
                LoadingEnd();
                // 进入牛牛游戏房间
                _gameType = GameType.GT_NN;
                nnRoom.backHome.gameObject.SetActive(false);
            }
            break;
        case MESSAGE_ID.msg_PostNNEntryRoom:
            nnRoom.OnPostEnterRoom(msg.postNNEntryRoom);
            break;
        case MESSAGE_ID.msg_StartNNGameResp:
            nnRoom.OnStartGame();
            break;
        case MESSAGE_ID.msg_PostStartNNGame:
            nnRoom.OnPostStartGame();
            break;
        case MESSAGE_ID.msg_PostNNDealResp:
            nnRoom.OnPostDealResp(msg.nnDealResp);
            break;
        case MESSAGE_ID.msg_StakeResp:
            nnRoom.OnStakeResp(msg.stakeResp);
            break;
        case MESSAGE_ID.msg_PostStakeResp:
            nnRoom.OnPostStakeResp(msg.postStakeResp);
            break;
        case MESSAGE_ID.msg_NNShowCardsResp:
            nnRoom.OnShowCardsResp(msg.nnShowCardsResp);
            break;
        case MESSAGE_ID.msg_PostNNShowCards:
            nnRoom.OnPostNNShowCards(msg.postNNShowCards);
            break;
        case MESSAGE_ID.msg_PostNNPrepareResp:
            nnRoom.OnPostNNPrepareResp(msg.postNNPrepareResp);
            break;
        case MESSAGE_ID.msg_PostStakeOver:
            nnRoom.OnPostStakeOver();
            break;
        case MESSAGE_ID.msg_PostDissolutionResp:
            nnRoom.OnPostDissolution();
            break;
        case MESSAGE_ID.msg_PostAnswerDissolutionResult:
            nnRoom.OnPostAnswerDissolution(msg.PostAnswerDissolutionResult);
            break;
        case MESSAGE_ID.msg_PostDissolutionResult:
            nnRoom.OnPostDissolutionResult();
            break;
        case MESSAGE_ID.msg_PostSendSoundResp:
            nnRoom.OnPostSendSoundResp(msg.postSendSoundResp);
            break;
		default:
			Debug.LogError ("Not Handled MsgId = " + msg.messageId);
            LoadingEnd();
            break;
        }
    }

	void Update()
	{
        // 由于UI操纵只能在主线程
		if (packages != null) {

			lock(packages)

			if (packages.Count > 0)
			{
                Debug.Log("Package Num = " + packages.Count);
				foreach (MessageInfo package in packages)
				{
					HandlePackage(package);
				}
                packages.Clear();
			}
		}

        // Android返回按钮
        if (Application.platform == RuntimePlatform.Android && (Input.GetKeyDown(KeyCode.Escape)))
        {
            ShowDialog("是否要现在离开游戏？", Quit);
        }
	}

    /// <summary>
    /// 进入主场景
    /// </summary>
	///
    private void GoHome(LoginResp msg)
    {
        if (msg.playerBaseInfo.ID > 0)
        {
            PlayerPrefs.SetInt("PlayerId", msg.playerBaseInfo.ID);
            ddzRoom.SelfPlayer.OnLoginResp(msg.playerBaseInfo);
            OpenWindow(WINDOW_ID.WINDOW_ID_HOME);
            _selfInfo.RefreshSelfInfos(ddzRoom.SelfPlayer);
            // 播放斗地主背景音乐
            _soundPlayer.PlayWelcomeMusic();
            Debug.Log("登录成功！！！");
            if (msg.shareurl != null && msg.shareurl.Length > 0 && msg.shareurl != "null")
            {
                shareUrl = msg.shareurl;
            }
            else
            {
                Debug.Log("msg.shareurl is null !!!");
                shareUrl = "http://rxcard.worldwalker.cn/wyzn/index.do";
            }
        }
        else
        {
            // 重新回到登录界面
            // 关闭主界面
            CloseWindow(WINDOW_ID.WINDOW_ID_HOME);
            // 打开登录界面
            OpenWindow(WINDOW_ID.WINDOW_ID_LOGIN);
            Debug.LogError("登录失败！！！");
        }
        LoadingEnd();
    }

    /// <summary>
    /// 进入游戏
    /// </summary>
    private void EnterGameDDZ(MessageInfo msg)
    {
        OpenWindow(WINDOW_ID.WINDOW_ID_GAME_DDZ);
        CloseWindow(WINDOW_ID.WINDOW_ID_HOME);
        if (msg.messageId == MESSAGE_ID.msg_CreateRoomResp)
        {
            ddzRoom.RoomId = msg.createRoomResp.roomId;
            ddzRoom.SelfPlayer.Order = 1;
            CloseWindow(WINDOW_ID.WINDOW_ID_CREATE_DDZ);
        }
		else if (msg.messageId == MESSAGE_ID.msg_EntryRoomResp)
        {
            ddzRoom.RoomId = msg.entryRoomResp.roomInfo.roomId;
            ddzRoom.Enter(msg);
            if (ddzRoom.GetPlayerNum() == 2)
            {
                DealRequest();
            }
            CloseWindow(WINDOW_ID.WINDOW_ID_ENTER_DDZ);
        }

        ddzRoom.InitSelfUI();

        btngroup1.SetActive(false);
        btngroup2.SetActive(false);
        RaycastTargetPanel.SetActive(true);
        // 地主牌和柰子牌显示背面
        //dizhuCardsSprite.HideCards();
        gameController.RefreshGameInfo();
        LastWeight = -1;
        LastCardType = CardsType.None;

        // 播放斗地主背景音乐
        _soundPlayer.PlayDDZBackgroundMusic();
        LoadingEnd();

        _gameType = GameType.GT_DDZ;
    }

    /// <summary>
    /// 接收到玩家进入游戏广播数据
    /// </summary>
    /// <param name="msg"></param>
    private void OnPostEntryInfo(MessageInfo msg)
    {
        ddzRoom.OnPostEntry(msg);
        //ShowTips(msg.postEntryRoom.player.name + " 加入房间");

        // 另外两个玩家加入，请求发牌
        if (ddzRoom.GetPlayerNum() == 2)
        {
            // 请求发牌
            DealRequest();
        }

        btngroup1.SetActive(false);
        btngroup2.SetActive(false);
        RaycastTargetPanel.SetActive(true);
    }

    public void DealRequest()
    {
        MessageInfo newMsg = new MessageInfo();
        newMsg.messageId = MESSAGE_ID.msg_DealReq;
        DealReq dealReq = new DealReq();
        dealReq.playerId = PlayerId;
        newMsg.dealReq = dealReq;
        PPSocket.GetInstance().SendMessage(newMsg);

        //请求发牌，重置一些记录信息
        ddzRoom.RefreshUI();
        lastCardType = CardsType.None;
        LastWeight = -1;
    }

    /// <summary>
    /// 广播出牌
    /// </summary>
    /// <param name="msg"></param>
    private void OnPostDiscard(MessageInfo msg)
    {
        if (msg.postDiscard.playerId == ddzRoom.SelfPlayer.PlayerId)
        {
            ddzRoom.SelfPlayer.OnDiscard(msg);
            // 自己出牌成功，关闭操作按钮
            RaycastTargetPanel.SetActive(true);
            btngroup1.SetActive(false);
            btngroup2.SetActive(false);
        }
        else
        {
            ddzRoom.OnDiscard(msg);
        }

        // 下个话语权者
        ddzRoom.CurrentPlayerId = msg.postDiscard.nextDiscardPlayerId;
        if (ddzRoom.CurrentPlayerId == ddzRoom.SelfPlayer.PlayerId && ddzRoom.GetPlayer(msg.postDiscard.playerId).LeaveCardCount > 0)
        {
            // 如果轮到自己出牌
            btngroup2.SetActive(true);
            // 关闭点击事件屏蔽
            RaycastTargetPanel.SetActive(false);
            ddzRoom.SelfPlayer.ShowPassBtn(!msg.postDiscard.mustDiscard);
            // 轮到自己出牌时，清空自己上次出牌显示
            ddzRoom.SelfPlayer.Player_Sprite.ClearDiscardUI();
            // 如果不得不出则可以出所有牌型
            if (msg.postDiscard.mustDiscard)
            {
                LastWeight = -1;
                LastCardType = CardsType.None;
            }
        }
        // 显示话语权者计时
        ddzRoom.ChangeClock();
    }

    /// <summary>
    /// 收到请求发牌服务器回应
    /// </summary>
    /// <param name="msg"></param>
    private void OnDealResp(MessageInfo msg)
    {
        if (msg.dealResp.playerId == PlayerId)
        {
            ddzRoom.CurrentPlayerId = msg.dealResp.grabHost;
            ddzRoom.SelfPlayer.OnDealCards(msg);
            //ShowTips( + " 是否第一个开始抢地主 ：" + selfInfo.IsFirstGrab);
        }
    }

    /// <summary>
    /// 广播抢地主返回
    /// </summary>
    /// <param name="msg"></param>
    private void OnPostGrabHostResp(MessageInfo msg)
    {
        Debug.Log("On PostGrabHost Resp！！！ type = " + msg.postGrabHostResp.type);
        if (msg.postGrabHostResp.type == -1)
        {
            // 流局，重新发牌
            // 隐藏抢地主
            btngroup1.SetActive(false);
            // 出牌按钮
            btngroup2.SetActive(false);
            RaycastTargetPanel.SetActive(true);
            DealRequest();
        }
        else
        {
            // 如果是自己抢地主，标记本局已经抢过
            if (msg.postGrabHostResp.playerId == ddzRoom.SelfPlayer.PlayerId)
            {
                ddzRoom.SelfPlayer.IsGrabHosted = true;
            }

            // 已经产生地主
            if (msg.postGrabHostResp.hostPlayerId > 0)
            {
                // 暂时先不加赖子逻辑
                //ResourceManager.Instance.CurrentLaizi = CardsLibrary.Instance[msg.postGrabHostResp.variable].GetCardWeight;
                // 先切换话语权者
                ddzRoom.CurrentPlayerId = msg.postGrabHostResp.hostPlayerId;
                List<int> dizhuCards = new List<int>();
                foreach (Poker poke in msg.postGrabHostResp.pokers)
                {
                    dizhuCards.Add(poke.ID);
                }
                if (dizhuCards.Count == 3)
                {
                    dizhuCardsSprite.ShowCards(dizhuCards, msg.postGrabHostResp.variable);
                }

                if (msg.postGrabHostResp.hostPlayerId == PlayerId)
                {
                    ddzRoom.SelfPlayer.IsDZ = true;
                    ddzRoom.SelfPlayer.OnPostGrabHost(msg);
                    // 出牌按钮
                    btngroup2.SetActive(true);
                    // 第一个出牌，必须出
                    ddzRoom.SelfPlayer.ShowPassBtn(false);
                    // 关闭点击事件屏蔽
                    RaycastTargetPanel.SetActive(false);
                }
                else
                {
                    // 设置地主
                    ddzRoom.SelfPlayer.IsDZ = false;
                    // 刷新手牌，可能存在赖子
                    ddzRoom.SelfPlayer.SortCards();
                }

                ddzRoom.SetDizhu(msg.postGrabHostResp.hostPlayerId);
                ddzRoom.ShowIdentity(true);
            }
            else
            {
                // 尚未产生地主
                ddzRoom.CurrentPlayerId = msg.postGrabHostResp.nextGrabPlayerId;
                ddzRoom.SelfPlayer.OnPostGrabHost(msg.postGrabHostResp.type);
                if (!ddzRoom.SelfPlayer.IsGrabHosted)
                {
                    // 如果自己还没有抢地主，并且是自己上家，则自己开始抢地主
                    int orderInterval = ddzRoom.SelfPlayer.Order - ddzRoom.GetPlayerOrder(msg.postGrabHostResp.playerId);
                    if (orderInterval == 1 || orderInterval == -2)
                    {
                        // 第一个抢地主地主
                        btngroup1.SetActive(true);
                        // 隐藏出牌按钮
                        btngroup2.SetActive(false);
                        // 关闭点击事件屏蔽
                        RaycastTargetPanel.SetActive(false);
                    }
                }
            }

            // 改变话语权者
            ddzRoom.ChangeClock();
        }
    }

    /// <summary>
    /// 牌发完，广播玩家，可以开始抢地主了
    /// </summary>
    /// <param name="msg"></param>
    private void OnPostDealOver(MessageInfo msg)
    {
        ddzRoom.SelfPlayer.IsGrabHosted = false;

        ddzRoom.OnDealOver();

        if (ddzRoom.CurrentPlayerId == ddzRoom.SelfPlayer.PlayerId)
        {
			// 如果自己第一个抢地主地主
            btngroup1.SetActive(true);
			// 隐藏出牌按钮
			btngroup2.SetActive (false);
			// 关闭点击事件屏蔽
			RaycastTargetPanel.SetActive (false);

            // 第一个抢地主，所有分都可以抢
            ddzRoom.SelfPlayer.OnPostGrabHost(0);

			Debug.Log ("开始抢地主");
		} else {
            Debug.Log("等待别人先抢地主");
            //ShowTips ("等待别人先抢地主");
        }
        ddzRoom.ChangeClock();
    }

    /// <summary>
    /// 结算
    /// </summary>
    /// <param name="msg"></param>
    private void Settlement(MessageInfo msg)
    {
        if (msg.settlementInfo.isOver)
        {

            switch (_gameType)
            {
                case GameType.GT_DDZ:
                    {
                        OpenWindow(WINDOW_ID.WINDOW_ID_DDZSETTLEMENT_FINAL);
                        settlementFinal.SetSettlementUI(ddzRoom, msg.settlementInfo);
                    }
                    break;
                case GameType.GT_NN:
                    {
                        OpenWindow(WINDOW_ID.WINDOW_ID_NNSETTLEMENT_FINAL);
                        nnSettlementFinal.SetSettlementUI(nnRoom, msg.settlementInfo);
                    }
                    break;
                default:
                    break;
            }

        }else
        {
            OpenWindow(WINDOW_ID.WINDOW_ID_SETTLEMENT_ONE);
            switch (_gameType)
            {
                case GameType.GT_DDZ:
                    {
                        foreach (SettlementData data in msg.settlementInfo.players)
                        {
                            ddzRoom.GetPlayer(data.ID).Score = data.finalscore;
                            if (data.ID == ddzRoom.SelfPlayer.PlayerId)
                            {
                                settlementOne.SetSettlementUI(data.gotscore);
                            }
                        }
                    }
                    break;
                case GameType.GT_NN:
                    {
                        nnRoom.Settlement(msg.settlementInfo, settlementOne);
                    }
                    break;
                default:
                    break;
            }
            
        }
    }

    ///
    /// 打开界面
    /// 
    public void OpenWindow(WINDOW_ID winId)
    {
        //CloseWindow(WindowId);
        WindowId = winId;
        Debug.logger.Log("打开界面" + winId);
        windows[winId].SetActive(true);
    }
    // 关闭界面
    public void CloseWindow(WINDOW_ID winId)
    {
        Debug.logger.Log("关闭界面" + winId);
        windows[winId].SetActive(false);
    }
    // 显示消息提示框
    public void ShowTips(string msg)
    {
        TipsNoticeLabel.text = msg;
        windows[WINDOW_ID.WINDOW_ID_TIP].SetActive(true);
    }

    public void ShowDialog(string content, UnityEngine.Events.UnityAction callback)
    {
        DialogContentText.text = content;
        DialogOKBtn.onClick.AddListener(callback);
        windows[WINDOW_ID.WINDOW_ID_DIALONG_FINAL].SetActive(true);
    }

    /// <summary>
    /// 上个玩家出牌类型
    /// </summary>
    public int LastWeight
    {
        set { lastWeight = value; }
        get { return lastWeight; }
    }

    public CardsType LastCardType
    {
        set { lastCardType = value; }
        get { return lastCardType; }
    }

    void OnDestroy()
    {
        // 关闭Socket线程和网络
        PPSocket.GetInstance().OnQuit();
    }

    public void LoadingStart()
    {
        loading.StartLoading();
    }

    public void LoadingEnd()
    {
        loading.EndLoading();
    }

//////////////////////////////////////////// 公共 ///////////////////////////////////////////////////////////
    /// <summary>
    /// 结算界面关闭之后的回调操作
    /// </summary>
    public void SettlementOneClosed()
    {
        switch (_gameType)
        {
            case GameType.GT_DDZ:
                {
                    DealRequest();
                }
                break;
            case GameType.GT_NN:
                {
                    nnRoom.Again();
                }
                break;
            default:
                break;
        }

    }
    public void Quit()
    {
        MessageInfo req = new MessageInfo();
        SignOutReq signout = new SignOutReq();
        PlayerBaseInfo playerBaseInfo = new PlayerBaseInfo();
        req.messageId = MESSAGE_ID.msg_SignOutReq;
        signout.playerid = PlayerId;
        req.signOutReq = signout;

        PPSocket.GetInstance().SendMessage(req);

        PPSocket.GetInstance().OnQuit();
        Application.Quit();
        Debug.Log("退出程序成功！！！");
    }
}
