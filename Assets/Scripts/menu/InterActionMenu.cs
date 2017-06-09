using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using UnityEngine.UI;
using netty;
using System.Collections.Generic;

/// <summary>
/// TIps面板
/// </summary>
public class InterActionMenu : MonoBehaviour
{
    // 抢地主相关按钮
    public Button disgrabBtn;
    public Button grabBtn1;
    public Button grabBtn2;
    public Button grabBtn3;

    // 出牌相关按钮
    public Button promptBtn;
    public Button passBtn;
    public Button discardBtn;

    private HomeController controller = null;
    // 屏蔽点击事件panel
    public GameObject RaycastTargetPanel;
    // playerselfSprite
    public GameObject slefPlayer;

    private ClientPlayerInfo playerInfo = null;

    public ClientPlayerInfo SetPlayerInfo
    {
        set { playerInfo = value; }
    }

    // 抢地主
    public GameObject btngroup1;
    // 出牌
    public GameObject btngroup2;

    // Use this for initialization
    void Start()
    {
        disgrabBtn.onClick.AddListener(Disgrab);
        grabBtn1.onClick.AddListener(Grab1);
        grabBtn2.onClick.AddListener(Grab2);
        grabBtn3.onClick.AddListener(Grab3);
        promptBtn.onClick.AddListener(Promp);
        passBtn.onClick.AddListener(Pass);
        discardBtn.onClick.AddListener(Discard);

        controller = GameObject.Find("UIRoot").GetComponent<HomeController>();
    }

    /// <summary>
    /// 不抢地主
    /// </summary>
    void Disgrab()
    {
        Grab(0);
    }
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
    /// <summary>
    /// 出牌
    /// </summary>
    void Discard()
    {
        Discard(true);
    }
    /// <summary>
    /// 不出
    /// </summary>
    void Pass()
    {
        Discard(false);
    }

    /// <summary>
    /// 提示
    /// </summary>
    void Promp()
    {
        //transform.gameObject.SetActive(false);
    }

    /// <summary>
    /// 抢地主
    /// </summary>
    /// <param name="type"> 0--不抢 1--抢地主1分  2--抢地主2分  3--抢地主3分 </param>
    private void Grab(int type)
    {
        MessageInfo req = new MessageInfo();
        req.messageId = MESSAGE_ID.msg_GrabHostReq;
        GrabHostReq grabreq = new GrabHostReq();
        grabreq.playerId = controller.PlayerId;
        grabreq.type = type;
        req.grabHostReq = grabreq;

        PPSocket.GetInstance().SendMessage(req);

        RaycastTargetPanel.SetActive(true);
        btngroup1.SetActive(false);
        btngroup2.SetActive(false);
    }

    private void Discard(bool discard)
    {
        CardsType type = CardsType.None;
        DiscardReq discardreq = new DiscardReq();
        if (discard)
        {
            List<int> cardIds = playerInfo.GetSelectedCardIds();
            //  没有选择出牌不处理
            if (cardIds.Count == 0)
            {
                controller.ShowTips("你没有选择出的牌！！！");
                return;
            }

            if (CheckPlayCards(cardIds, out type))
            {
                foreach (int cardId in cardIds)
                {
                    discardreq.cardIds.Add(cardId);
                }
            }
            else
            {
                // 如果无法出牌则不处理。
                return;
            }
        }
        MessageInfo req = new MessageInfo();
        req.messageId = MESSAGE_ID.msg_DiscardReq;
        discardreq.playerId = controller.PlayerId;
        discardreq.cardsType = (int)type;
        req.discardReq = discardreq;

        PPSocket.GetInstance().SendMessage(req);
    }
    public void OnPostGrabHost(int type)
    {
        switch (type)
        {
            case 3:
                grabBtn3.gameObject.SetActive(false);
                grabBtn2.gameObject.SetActive(false);
                grabBtn1.gameObject.SetActive(false);
                break;
            case 2:
                grabBtn3.gameObject.SetActive(true);
                grabBtn2.gameObject.SetActive(false);
                grabBtn1.gameObject.SetActive(false);
                break;
            case 1:
                grabBtn3.gameObject.SetActive(true);
                grabBtn2.gameObject.SetActive(true);
                grabBtn1.gameObject.SetActive(false);
                break;
            default:
                grabBtn3.gameObject.SetActive(true);
                grabBtn2.gameObject.SetActive(true);
                grabBtn1.gameObject.SetActive(true);
                break;
        }
    }

    /// <summary>
    /// 检测玩家出牌
    /// </summary>
    /// <param name="selectedCardsList"></param>
    /// <param name="selectedSpriteList"></param>
    bool CheckPlayCards(List<int> selectedCardsList, out CardsType type)
    {
        CardRules.SortCards(selectedCardsList);
        CardRules.FinalClassification(selectedCardsList);
        HomeController controller = GameObject.Find("UIRoot").GetComponent<HomeController>();
        int[] selectedCardsArray = selectedCardsList.ToArray();
        //检测是否符合出牌规则
        if (CardRules.PopEnable(selectedCardsArray, out type))
        {
            if (type > CardsType.Boom && controller.LastCardType != type && controller.LastCardType != CardsType.None)
            {
                controller.ShowTips("非炸弹，并且不是自己首发牌，且牌型与上家不一致！！！ 上家牌型：" + controller.LastCardType + " 自己牌型：" + type.ToString());
                return false;
            }
            if (CardRules.GetWeight(selectedCardsArray, type) > controller.LastWeight)
            {
                return true;
            }
            controller.ShowTips("牌不能压制上家！！！牌型为 ： " + type.ToString());
        }
        else
        {
            controller.ShowTips("牌型不正确！！！");
        }
        return false;
    }
}
