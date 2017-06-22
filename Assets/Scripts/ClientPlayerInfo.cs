using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using netty;

public class ClientPlayerInfo
{
    // 玩家id
    private int playerId = 0;

    private string name = "";

    private string headIconUrl = "";

    private List<int> pokers = null;

    private List<int> discardIds = null;

    private int score = 0;

    private bool isDZ = false;
    // 剩余房卡数量
    private int leaveCardCount = 0;

    private PlayerSprite playerSprite = null;

    private InterActionMenu interActionMenu = null;

    private bool isSelf = false;

    private int order;

    // 是否抢过地主
    private bool isGrabHosted = false;

    private Sprite headIcon = null;

    private bool m_isonline = true;

    private int m_wxopenid = 0;

    public int WxOpenId
    {
        set { m_wxopenid = value; }
        get { return m_wxopenid; }
    }

    public bool GetIsOnline
    {
        get { return m_isonline; }
    }

    public Sprite HeadIcon
    {
        set { headIcon = value; }
        get { return headIcon; }
    }

    public bool IsGrabHosted
    {
        set { isGrabHosted = value; }
        get { return isGrabHosted; }
    }

    public int Order
    {
        set
        {
            order = value;
            if (playerSprite != null)
            {
                // 数据已经和UI精灵绑定则刷新
                playerSprite.RefreshCardsUI();
            }
        }
        get { return order; }
    }

    public PlayerSprite Player_Sprite
    {
        set
        {
            playerSprite = value;
            if(playerSprite)
            {
                playerSprite.SetPlayerInfo = this;
            }
        }
        get { return playerSprite; }
    }

    public InterActionMenu SetInterActionMenu
    {
        set
        {
            interActionMenu = value;
            if (interActionMenu)
            {
                interActionMenu.SetPlayerInfo = this;
            }
        }
    }

    public ClientPlayerInfo(bool _isSelf)
    {
        isSelf = _isSelf;
        pokers = new List<int>();
        discardIds = new List<int>();
    }

    public int PlayerId
    {
        set { playerId = value; }
        get { return playerId; }
    }

    public string Name
    {
        set { name = value; }
        get { return name; }
    }

    public string HeadIconUrl
    {
        set { headIconUrl = value; }
        get { return headIconUrl; }
    }

    public int Score
    {
        set { score = value; }
        get { return score; }
    }

    public bool IsDZ
    {
        set { isDZ = value; }
        get { return isDZ; }
    }

    /// <summary>
    ///  清空手牌
    /// </summary>
    public void ClearPokers()
    {
        if (pokers != null)
        {
            pokers.Clear();
        }
        leaveCardCount = 0;
        isDZ = false;
    }

    public void CopyFrom(Player player)
    {
        this.PlayerId = player.ID;
        this.Name = player.name;
        this.HeadIconUrl = player.imgUrl;
        this.Score = player.score;
        this.IsDZ = player.isDz;
        this.order = player.order;
        this.m_isonline = player.isOnline;

        leaveCardCount = 0;
        if (headIconUrl == null || headIconUrl.Length == 0 || headIconUrl == "null")
        {
            headIconUrl = "https://timgsa.baidu.com/timg?image&quality=80&size=b9999_10000&sec=1492235773979&di=42b5ddb3d50d6ea32fafee903833c44c&imgtype=0&src=http%3A%2F%2Fwenwen.soso.com%2Fp%2F20110825%2F20110825115928-858187777.jpg";
        }

        if (playerSprite != null)
        {
            // 数据已经和UI精灵绑定则刷新
            playerSprite.InitUserUI();
        }
    }

    public List<int> GetLeavePokeList()
    {
        return pokers;
    }

    public List<int> GetDiscardList()
    {
        return discardIds;
    }

    public int LeaveCardCount
    {
        get { return leaveCardCount; }
        set { leaveCardCount = value; }
    }

    /// <summary>
    /// 出牌成功后同步本地数据
    /// </summary>
    public void OnDiscard(MessageInfo msg)
    {
        discardIds.Clear();
        foreach(int cardId in msg.postDiscard.cardIds)
        {
            discardIds.Add(cardId);
            if (isSelf)
            {
                pokers.Remove(cardId);
            }
        }
        // 牌组排序
        CardRules.SortCards(discardIds);
        CardRules.SortCards(pokers);

        leaveCardCount = msg.postDiscard.remainderPokersNum;

        if (playerSprite != null)
        {
            // 出牌后重新刷新手牌
            playerSprite.RefreshCardsUI();
            // 重新刷新出牌
            playerSprite.RefreshDiscardUI();
            // 刷新积分等信息
            playerSprite.RefreshScoreUI();
        }

        HomeController controller = GameObject.Find("UIRoot").GetComponent<HomeController>();
        int[] selectedCardsArray = discardIds.ToArray();
        //检测是否符合出牌规则
        CardsType type = CardsType.None;
        if (CardRules.PopEnable(selectedCardsArray, out type))
        {
            controller.LastCardType = type;
            controller.LastWeight = CardRules.GetWeight(selectedCardsArray, type);
        }

        discardIds.Clear();
    }

    /// <summary>
    /// 服务器发牌回复
    /// </summary>
    /// <param name="msg"></param>
    public string OnDealCards(MessageInfo msg)
    {
        ClearPokers();
        string debugInfo = "收到牌信息： ";
        foreach (Poker poker in msg.dealResp.pokers)
        {
            pokers.Add(poker.ID);
            debugInfo += "[" + poker.ID + "]";
        }
        leaveCardCount = pokers.Count;
        Debug.Log(debugInfo);

        CardRules.SortCards(pokers);

		if (playerSprite != null) {
			// 数据已经和UI精灵绑定则刷新
            playerSprite.RefreshCardsUI();
		} else {
			Debug.LogError ("PlayerSprite is null!");
		}

		return debugInfo;
    }

    public void OnPostGrabHost(MessageInfo msg)
    {
        string debugInfo = "地主牌信息： ";
        foreach (Poker p in msg.postGrabHostResp.pokers)
        {
            pokers.Add(p.ID);
            ++leaveCardCount;
            debugInfo += "[" + p.ID + "]";
        }

        CardRules.SortCards(pokers);

        if (playerSprite != null)
        {
            // 数据已经和UI精灵绑定则刷新
            playerSprite.RefreshCardsUI();
        }
        Debug.Log(debugInfo);
    }

    public void OnPostGrabHost(int type)
    {
        if (playerSprite != null)
        {
            // 数据已经和UI精灵绑定则刷新
            interActionMenu.OnPostGrabHost(type);
        }
    }

    /// <summary>
    /// 获取选择的牌列表
    /// </summary>
    /// <returns></returns>
    public List<int> GetSelectedCardIds()
    {
        return discardIds;
    }

    /// <summary>
    /// 添加出牌
    /// </summary>
    /// <param name="cardId"></param>
    public void AddDiscard(int cardId)
    {
        discardIds.Add(cardId);
    }
    /// <summary>
    /// 撤销出牌
    /// </summary>
    /// <param name="cardId"></param>
    public void RemoveDiscard(int cardId)
    {
        discardIds.Remove(cardId);
    }

    public void SortCards()
    {
        CardRules.SortCards(pokers);

        if (playerSprite != null)
        {
            // 数据已经和UI精灵绑定则刷新
            playerSprite.RefreshCardsUI();
        }
    }

    public void ShowPassBtn(bool show)
    {
        interActionMenu.passBtn.gameObject.SetActive(show);
    }

    public void OnLoginResp(PlayerBaseInfo info)
    {
        playerId = info.ID;
        name = info.name;
        headIconUrl = info.imgUrl;
        leaveCardCount = info.cardNum;

        PlayerPrefs.SetInt("PLAYERID", playerId);
        PlayerPrefs.SetString("TOKEN", info.token);

        if (headIconUrl == null || headIconUrl.Length == 0 || headIconUrl == "null")
        {
            headIconUrl = "https://timgsa.baidu.com/timg?image&quality=80&size=b9999_10000&sec=1492235773979&di=42b5ddb3d50d6ea32fafee903833c44c&imgtype=0&src=http%3A%2F%2Fwenwen.soso.com%2Fp%2F20110825%2F20110825115928-858187777.jpg";
        }
    }
}
