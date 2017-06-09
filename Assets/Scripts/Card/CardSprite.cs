using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// 用于卡片的显示
/// </summary>
public class CardSprite : MonoBehaviour
{
    /// <summary>
    /// 牌字
    /// </summary>
    public Image weight;

    /// <summary>
    /// 花色-小
    /// </summary>
    public Image SmallSuits;

    /// <summary>
    /// 花色-大
    /// </summary>
    public Image BigSuits;

    /// <summary>
    /// 癞子，地主标记
    /// </summary>
    public Image Flag;

    public GameObject BigSuitsObj;
    public GameObject FlagObj;
    public GameObject BackObj;
    public GameObject NomalObj;
    public GameObject LJokerObj;
    public GameObject SJokerObj;

    public bool isNN = false;

    private int cardId;
    private bool isSelected;

    public FlagType flagType = FlagType.None;

    private bool isLaizi = false;

    private ClientPlayerInfo playerInfo;

    public bool ShowBigSuits
    {
        set { BigSuitsObj.SetActive(value); }
    }

    public ClientPlayerInfo SetPlayerInfo
    {
        set { playerInfo = value; }
    }

    public bool IsLaizi
    {
        set { isLaizi = value; }
        get { return isLaizi; }
    }

    void Start()
    {
    }

    /// <summary>
    /// 是否被点击中
    /// </summary>
    public bool Select
    {
        set { isSelected = value; }
        get { return isSelected; }
    }

    /// <summary>
    /// 设置UI对应的卡牌Id
    /// </summary>
    public int CardId
    {
        set 
        {
            cardId = value;
            if (cardId > 0)
            {
                BackObj.SetActive(false);
                if (!isNN)
                {
                    Button btn = this.gameObject.GetComponent<Button>();
                    if (btn == null)
                    {
                        this.gameObject.AddComponent<Button>().onClick.AddListener(OnClick);
                    }
                }

                weight.sprite = ResourceManager.Instance.GetCardWeightSprite(cardId);
                // 获取到的牌字资源为空，则是大小王
                if (weight.sprite == null)
                {
                    NomalObj.SetActive(false);
                    if (cardId == 54)
                    {
                        // 大王
                        LJokerObj.SetActive(true);
                        SJokerObj.SetActive(false);
                    }
                    else if (cardId == 53)
                    {
                        // 小王
                        SJokerObj.SetActive(true);
                        LJokerObj.SetActive(false);
                    }
                }
                else
                {
                    LJokerObj.SetActive(false);
                    SJokerObj.SetActive(false);
                    NomalObj.SetActive(true);
                    SmallSuits.sprite = ResourceManager.Instance.GetSuitsSprite(cardId, SuitsSpriteSize.sss_small);
                    BigSuits.sprite = ResourceManager.Instance.GetSuitsSprite(cardId, SuitsSpriteSize.sss_small);
                }
            }
            else
            {
                ShowBack();
            }
        }
        get
        {
            return cardId;
        }
    }

    /// <summary>
    /// 显示卡牌
    /// </summary>
    public void Show()
    {
        BackObj.SetActive(false);
    }

    /// <summary>
    /// 卡牌点击
    /// </summary>
    public void OnClick()
    {
        if (isSelected)
        {
            transform.localPosition -= Vector3.up * 40;
			isSelected = false;
			playerInfo.RemoveDiscard(cardId);
        }
        else
        {
            transform.localPosition += Vector3.up * 40;
			isSelected = true;
			playerInfo.AddDiscard(cardId);
        }
    }

    public void ShowBack()
    {
        BackObj.SetActive(true);
    }

    public void Destroy()
    {
        Destroy(this.gameObject);
    }
}
