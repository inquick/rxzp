using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using netty;

/// <summary>
/// 资源管理
/// </summary>
public class ResourceManager
{
    private static ResourceManager instance;
    /// <summary>
    ///  扑克牌资源
    /// </summary>
    private readonly Dictionary<Weight, Sprite> poker_num_black = new Dictionary<Weight, Sprite>();
    private readonly Dictionary<Weight, Sprite> poker_num_red = new Dictionary<Weight, Sprite>();
    private readonly Dictionary<Weight, Sprite> poker_num_laizi = new Dictionary<Weight, Sprite>();
    private readonly Dictionary<Suits, Sprite> poker_suits_big = new Dictionary<Suits, Sprite>();
    private readonly Dictionary<Suits, Sprite> poker_suits_small = new Dictionary<Suits, Sprite>();
    private readonly Dictionary<NNType, Sprite> niuniuTypes = new Dictionary<NNType, Sprite>();
    private readonly Dictionary<int, Sprite> stakes = new Dictionary<int, Sprite>();

    // 玩家头像地主农民标记
    private Sprite Farmer_player;
    private Sprite Landlord_player;
    // 
    private GameObject pokerPrefab;

    private Weight m_laize = Weight.LJoker;

    private Material m_greyMaterial = null;

    /// <summary>
    /// 计时器数字
    /// </summary>
    private readonly Dictionary<int, Sprite> clock_num = new Dictionary<int, Sprite>();

    public Weight CurrentLaizi
    {
        set { m_laize = value; }
        get { return m_laize; }
    }

    public static ResourceManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ResourceManager();
            }
            return instance;
        }
    }
    /// <summary>
    /// 私有构造
    /// </summary>
    private ResourceManager()
    {
    }

    public void LoadGameResources()
    {
        // 加载扑克牌数字
        for (Weight i = Weight.Three; i < Weight.SJoker; ++i)
        {
            poker_num_black.Add(i, Resources.Load<Sprite>("Cards/num_black/" + i.ToString()));
            poker_num_red.Add(i, Resources.Load<Sprite>("Cards/num_red/" + i.ToString()));
            poker_num_laizi.Add(i, Resources.Load<Sprite>("Cards/num_laizi/" + i.ToString()));
        }

        // 加载扑克牌花色
        for (Suits i = Suits.Diamond; i < Suits.None; i++)
        {
            poker_suits_big.Add(i, Resources.Load<Sprite>("Cards/flower_big/" + i.ToString()));
            poker_suits_small.Add(i, Resources.Load<Sprite>("Cards/flower_small/" + i.ToString()));
        }

        Farmer_player = Resources.Load<Sprite>("ddz/nongming");
        Landlord_player = Resources.Load<Sprite>("ddz/dizhu");

        pokerPrefab = Resources.Load<GameObject>("prefab/poker");

        for (int i = 0; i < 10; ++i)
        {
            clock_num.Add(i, Resources.Load<Sprite>("ddz/clock/" + i));
        }

        for (NNType i = NNType.NNT_NONE; i < NNType.NNT_SPECIAL_BOMEBOME; ++i)
        {
            niuniuTypes.Add(i, Resources.Load<Sprite>("nn/" + i.ToString()));
        }

        for (int i = 1; i < 6; ++i)
        {
            stakes.Add(i, Resources.Load<Sprite>("nn/stake" + i));
        }

        m_greyMaterial = Resources.Load<Material>("material/Grey");
    }

    public Sprite GetCardWeightSprite(int cardId)
    {
        Card card = CardsLibrary.Instance[cardId];
        if (card.GetCardWeight == m_laize && m_laize != Weight.LJoker)
        {
            return poker_num_laizi[card.GetCardWeight];
        }

        switch(card.GetCardSuit)
        {
            case Suits.Club:
            case Suits.Spade:
                return poker_num_black[card.GetCardWeight];
            case Suits.Heart:
            case Suits.Diamond:
                return poker_num_red[card.GetCardWeight];
            case Suits.None:
                return null;
            default:
                return null;
        }
    }

    public Sprite GetSuitsSprite(int cardId, SuitsSpriteSize size)
    {
        Card card = CardsLibrary.Instance[cardId];
        switch(size)
        {
            case SuitsSpriteSize.sss_small:
                if (card.GetCardWeight == m_laize && m_laize != Weight.LJoker)
                {
                    return poker_suits_small[Suits.Laizi];
                }
                return poker_suits_small[card.GetCardSuit];
            case SuitsSpriteSize.sss_big:
                if (card.GetCardWeight == m_laize && m_laize != Weight.LJoker)
                {
                    return poker_suits_big[Suits.Laizi];
                }
                return poker_suits_big[card.GetCardSuit];
            default:
                return null;
        }
    }

    public GameObject GetPokerPrefab()
    {
        return pokerPrefab;
    }

    public Sprite GetIdentitySprite(bool isLandlord)
    {
        if (isLandlord)
        {
            return Landlord_player;
        }

        return Farmer_player;
    }

    public Sprite GetClockTimeSprite(int num)
    {
        return clock_num[num];
    }

    public Sprite GetNiuNiuTypeSprite(NNType type)
    {
        return niuniuTypes[type];
    }

    public Sprite GetStakeSprite(int stake)
    {
        return stakes[stake];
    }

    public Material GreyMeterial
    {
        get { return m_greyMaterial; }
    }
}
