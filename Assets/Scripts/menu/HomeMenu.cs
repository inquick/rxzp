﻿using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using UnityEngine.UI;
using netty;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;

/// <summary>
/// 主场景菜单面板
/// </summary>
public class HomeMenu : MonoBehaviour, IPointerClickHandler
{
    private HomeController controller = null;
    private ThirdParty thirdParty = null;

    public Button quit;
    public GameObject shareTypePanel = null;
    public Button shareToFriends = null;
    public Button shareToCircle = null;

    // Use this for initialization
    void Start()
    {
        transform.Find("ddz/Create").gameObject.GetComponent<Button>().onClick.AddListener(CreateDDZ);
        transform.Find("ddz/Enter").gameObject.GetComponent<Button>().onClick.AddListener(EnterDDZ);

        transform.Find("nn/Create").gameObject.GetComponent<Button>().onClick.AddListener(CreateNN);
        transform.Find("nn/Enter").gameObject.GetComponent<Button>().onClick.AddListener(EnterNN);

        transform.Find("player_info/room_card/BuyCard").gameObject.GetComponent<Button>().onClick.AddListener(Shop);
        transform.Find("top_right/shop").gameObject.GetComponent<Button>().onClick.AddListener(Shop);

        transform.Find("top_right/vip").gameObject.GetComponent<Button>().onClick.AddListener(VipBtn);

        transform.Find("left/Share").gameObject.GetComponent<Button>().onClick.AddListener(ShareBtn);
        transform.Find("left/Extension").gameObject.GetComponent<Button>().onClick.AddListener(ExtensionBtn);
        transform.Find("left/Other").gameObject.GetComponent<Button>().onClick.AddListener(OtherBtn);
        transform.Find("left/Help").gameObject.GetComponent<Button>().onClick.AddListener(HelpBtn);
        transform.Find("left/Set").gameObject.GetComponent<Button>().onClick.AddListener(SetBtn);


        quit.gameObject.GetComponent<Button>().onClick.AddListener(Quit);

        controller = GameObject.Find("UIRoot").GetComponent<HomeController>();
        thirdParty = GameObject.Find("UIRoot").GetComponent<ThirdParty>();

        shareToFriends.onClick.AddListener(ShareToFriends);
        shareToCircle.onClick.AddListener(ShareToTimeline);
    }

    /// <summary>
    /// 创建斗地主房间
    /// </summary>
	void CreateDDZ()
    {
        controller.ShowTips(Strings.SS_NOT_OPEN);
        //controller.OpenWindow(WINDOW_ID.WINDOW_ID_CREATE_DDZ);
	}

	/// <summary>
    /// 进入斗地主房间
	/// </summary>
	void EnterDDZ()
    {
        controller.ShowTips(Strings.SS_NOT_OPEN);
        //controller.OpenWindow(WINDOW_ID.WINDOW_ID_ENTER_DDZ);
        //GameObject.Find("UIRoot/WndEnter").GetComponent<EnterMenu>().SetGameType = GameType.GT_DDZ;
    }

    /// <summary>
    /// 创建斗牛房间
    /// </summary>
    void CreateNN()
    {
        controller.OpenWindow(WINDOW_ID.WINDOW_ID_CREATE_NN);
    }

    /// <summary>
    /// 进入斗牛房间
    /// </summary>
    void EnterNN()
    {
        controller.OpenWindow(WINDOW_ID.WINDOW_ID_ENTER_NN);
        GameObject.Find("UIRoot/WndEnter").GetComponent<EnterMenu>().SetGameType = GameType.GT_NN;
    }

    /// <summary>
    /// 购买房卡
    /// </summary>
    void Shop()
    {
        controller.ShowTips(Strings.SS_BUY_ROOM_CARDS);
        //controller.OpenWindow(WINDOW_ID.WINDOW_ID_SHOP);
    }


    /// <summary>
    /// VIP信息
    /// </summary>
    void VipBtn()
    {
        controller.ShowTips(Strings.SS_NOT_OPEN);
    }
    
    /// <summary>
    /// 分享
    /// </summary>
    void ShareBtn()
    {
        shareTypePanel.SetActive(true);
    }

    void ShareToFriends()
    {
        shareTypePanel.SetActive(false);
        thirdParty.ThirdPartyShare(Strings.SS_GAME_INVITE, String.Format(Strings.SS_INVITE_INFOS, controller.PlayerName, controller.PlayerId), 1);
    }
    void ShareToTimeline()
    {
        shareTypePanel.SetActive(false);
        thirdParty.ThirdPartyShare(Strings.SS_GAME_INVITE, String.Format(Strings.SS_INVITE_INFOS, controller.PlayerName, controller.PlayerId), 0);
    }
    
    /// <summary>
    /// 推广
    /// </summary>
    void ExtensionBtn()
    {
        controller.ShowTips(Strings.SS_NOT_OPEN);
    }
    
    /// <summary>
    /// 其他
    /// </summary>
    void OtherBtn()
    {
        controller.ShowTips(Strings.SS_NOT_OPEN);
    }
     
    /// <summary>
    /// 帮助
    /// </summary>
    void HelpBtn()
    {
        controller.OpenWindow(WINDOW_ID.WINDOW_ID_HELP);
    }
     
    /// <summary>
    /// 设置
    /// </summary>
    void SetBtn()
    {
        controller.OpenWindow(WINDOW_ID.WINDOW_ID_SETTING);
    }

    void Quit()
    {
        controller.ShowDialog(Strings.SS_ASK_QUIT, controller.Quit);
    }

    public void HideShareBtn()
    {
        shareTypePanel.SetActive(false);
    }

    public void OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
    {
        HideShareBtn();
        //throw new NotImplementedException();
    }
}
