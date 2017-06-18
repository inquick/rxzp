using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using netty;

public class SettlementFinal : MonoBehaviour
{
    public Image winSprite;
    public SettlementPlayerInfo player1;
    public SettlementPlayerInfo player2;
    public SettlementPlayerInfo player3;
    public Button closeBtn;
    public HomeController controller;

    private SettlementPlayerInfo[] players;
	// Use this for initialization
	void Start () {
        players = new SettlementPlayerInfo[3];
        players[0] = player1;
        players[1] = player2;
        players[2] = player3;

        closeBtn.onClick.AddListener(CloseWnd);
	}
	
	// Update is called once per frame
	void Update () {
	}

    /// <summary>
    /// 关闭窗口
    /// </summary>
    void CloseWnd()
    {
        transform.gameObject.SetActive(false);
        controller.BackHome();
    }

    public void SetSettlementUI(ClientRoomInfo room, SettlementInfo info)
    {
        bool selfWin = false;

        foreach (SettlementData data in info.players)
        {
            if (data.ID == room.SelfPlayer.PlayerId)
            {
                selfWin = data.isWin;
            }
            room.GetPlayer(data.ID).Score = data.finalscore;
        }
        int index = 0;
        foreach(SettlementData data in info.players)
        {
            players[index].SetSettlementUI(data.finalscore, room.GetPlayer(data.ID), selfWin);
            index++;
        }
    }
}
