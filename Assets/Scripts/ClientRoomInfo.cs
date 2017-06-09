using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using netty;

public class ClientRoomInfo
{
    private int roomId;
    private int multiple;
    private int playedGames;
    private int remainderGames;
    private ClientPlayerInfo selfPlayer;
    private ClientPlayerInfo leftPlayer;
    private ClientPlayerInfo rightPlayer;
    // 当前说话者
    private int currentPlayerId;

    public int RoomId
    {
        set { roomId = value; }
        get { return roomId; }
    }
    public int Multiple
    {
        set { multiple = value; }
        get { return multiple; }
    }
    public int PlayedGames
    {
        set { playedGames = value; }
        get { return playedGames; }
    }
    public int RemainderGames
    {
        set { remainderGames = value; }
        get { return remainderGames; }
    }
    public int CurrentPlayerId
    {
        set { currentPlayerId = value; }
        get { return currentPlayerId; }
    }

    public ClientRoomInfo()
    {
        selfPlayer = new ClientPlayerInfo(true);
        leftPlayer = new ClientPlayerInfo(false);
        rightPlayer = new ClientPlayerInfo(false);
    }

    public void OnPostEntry(MessageInfo msg)
    {
        if (msg.postEntryRoom.player.order - selfPlayer.Order == 1)
        {
            rightPlayer.CopyFrom(msg.postEntryRoom.player);
        }
        else
        {
            leftPlayer.CopyFrom(msg.postEntryRoom.player);
        }
    }

    /// <summary>
    /// 玩家进入房间
    /// </summary>
    public void Enter(MessageInfo msg)
    {
        roomId = msg.entryRoomResp.roomInfo.roomId;
        selfPlayer.Order = msg.entryRoomResp.order;
        multiple = msg.entryRoomResp.roomInfo.multiple;
        playedGames = msg.entryRoomResp.roomInfo.playedGames;
        remainderGames = msg.entryRoomResp.roomInfo.totalGames;
            
        foreach (Player p in msg.entryRoomResp.roomInfo.players)
        {
            if (msg.entryRoomResp.order - p.order == 1)
            {
                leftPlayer.CopyFrom(p);
            }
			else if (msg.entryRoomResp.order != p.order)
            {
                rightPlayer.CopyFrom(p);
            }
        }
    }

    public int GetPlayerNum()
    {
        int count = 0;

        if (leftPlayer.PlayerId > 0)
            ++count;

        if (rightPlayer.PlayerId > 0)
            ++count;

        return count;
    }

    public ClientPlayerInfo GetPlayer(int id)
    {
        if (leftPlayer.PlayerId == id)
        {
            return leftPlayer;
        }
        if (rightPlayer.PlayerId == id)
        {
            return rightPlayer;
        }
		if (selfPlayer.PlayerId == id)
		{
			return selfPlayer;
		}

        return null;
    }

    public ClientPlayerInfo LeftPlayer
    {
        get { return leftPlayer; }
    }

    public ClientPlayerInfo RightPlayer
    {
        get { return rightPlayer; }
    }

    public ClientPlayerInfo SelfPlayer
    {
        get { return selfPlayer; }
    }

    /// <summary>
    /// 出牌成功后同步本地数据
    /// </summary>
    /// <param name="msg"></param>
    /// <returns> 是否该自己出手 </returns>
    public void OnDiscard(MessageInfo msg)
    {
        if (leftPlayer.PlayerId == msg.postDiscard.playerId)
            leftPlayer.OnDiscard(msg);

        if (rightPlayer.PlayerId == msg.postDiscard.playerId)
            rightPlayer.OnDiscard(msg);

    }

    public int GetPlayerOrder(int playerId)
    {
        if (leftPlayer.PlayerId == playerId)
            return leftPlayer.Order;

        if (rightPlayer.PlayerId == playerId)
            return rightPlayer.Order;

        return 0;
    }

    public void SetDizhu(int dizhuId)
    {
        if (leftPlayer.PlayerId == dizhuId)
        {
            leftPlayer.IsDZ = true;
            leftPlayer.LeaveCardCount = 20;
            leftPlayer.Player_Sprite.RefreshScoreUI();
        }
        else
        {
            leftPlayer.IsDZ = false;
        }

        if (rightPlayer.PlayerId == dizhuId)
        {
            rightPlayer.IsDZ = true;
            rightPlayer.LeaveCardCount = 20;
            rightPlayer.Player_Sprite.RefreshScoreUI();
        }
        else
        {
            rightPlayer.IsDZ = false;
        }
    }

    public void SetPlayerSprite(PlayerSprite self, PlayerSprite left, PlayerSprite right)
    {
        if (selfPlayer.Player_Sprite == null)
        {
            selfPlayer.Player_Sprite = self;
        }
        if (leftPlayer.Player_Sprite == null)
        {
            leftPlayer.Player_Sprite = left;
        }
        if (rightPlayer.Player_Sprite == null)
        {
            rightPlayer.Player_Sprite = right;
        }
    }

    /// <summary>
    /// 发牌完毕，刷新其他两家牌数
    /// </summary>
    public void OnDealOver()
    {
        leftPlayer.LeaveCardCount = 17;
        rightPlayer.LeaveCardCount = 17;
        leftPlayer.Player_Sprite.RefreshCardsUI();
        rightPlayer.Player_Sprite.RefreshCardsUI();
    }

    /// <summary>
    /// 切换玩家时钟显示
    /// </summary>
    public void ChangeClock()
    {
        if (currentPlayerId == leftPlayer.PlayerId)
        {
            leftPlayer.Player_Sprite.ShowClock(true);
            rightPlayer.Player_Sprite.ShowClock(false);
            selfPlayer.Player_Sprite.ShowClock(false);
        }
        else if (currentPlayerId == rightPlayer.PlayerId)
        {
            rightPlayer.Player_Sprite.ShowClock(true);
            leftPlayer.Player_Sprite.ShowClock(false);
            selfPlayer.Player_Sprite.ShowClock(false);
        }
        else
        {
            leftPlayer.Player_Sprite.ShowClock(false);
            rightPlayer.Player_Sprite.ShowClock(false);
            selfPlayer.Player_Sprite.ShowClock(true);
        }
    }
    public void InitSelfUI()
    {
        selfPlayer.Player_Sprite.InitUserUI();
    }
    /// <summary>
    /// 显示玩家头像地主标记
    /// </summary>
    /// <param name="show"></param>
    public void ShowIdentity(bool show)
    {
        selfPlayer.Player_Sprite.ShowIdentity = show;
        leftPlayer.Player_Sprite.ShowIdentity = show;
        rightPlayer.Player_Sprite.ShowIdentity = show;
    }

    /// <summary>
    /// 重置牌桌出牌信息
    /// </summary>
    public void RefreshUI()
    {
        ShowIdentity(false);
        // 关闭所有闹钟显示
        selfPlayer.Player_Sprite.ShowClock(false);
        leftPlayer.Player_Sprite.ShowClock(false);
        rightPlayer.Player_Sprite.ShowClock(false);
        // 清空所有出牌显示
        selfPlayer.Player_Sprite.ClearDiscardUI();
        leftPlayer.Player_Sprite.ClearDiscardUI();
        rightPlayer.Player_Sprite.ClearDiscardUI();
    }
}
