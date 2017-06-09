using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.Scripts;
using System;

public class PlayerSprite : MonoBehaviour {

    public GameObject identityObj;

    public GameObject discardPanel;

    public GameObject leaveCardPanel;

    public GameObject cardsStart;

    public Clock clock;

    public Image headIcon;

    public Image identity;

    public Text name;

    public Text score;

    public Text leaveCount;

    public Transform leavePokesPanel;

    private ClientPlayerInfo playerInfo = null;

    public bool isSelf;

    public ClientPlayerInfo  SetPlayerInfo
    {
        set
        {
            playerInfo = value;
        }
    }

    public Sprite SetIdentity
    {
        set
        {
            if (value)
            {
                identity.sprite = value;
                identityObj.SetActive(true);
            }
            else
            {
                identityObj.SetActive(false);
            }
        }
    }

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void InitUserUI()
    {
        name.text = playerInfo.Name;
        if (playerInfo.HeadIconUrl == "")
        {
            playerInfo.HeadIconUrl = "https://timgsa.baidu.com/timg?image&quality=80&size=b9999_10000&sec=1492235773979&di=42b5ddb3d50d6ea32fafee903833c44c&imgtype=0&src=http%3A%2F%2Fwenwen.soso.com%2Fp%2F20110825%2F20110825115928-858187777.jpg";
        }
        StartCoroutine(DownloadImage(playerInfo.HeadIconUrl, headIcon));
    }
    public void RefreshScoreUI()
    {
        if (playerInfo == null)
        {
            Debug.LogError("PlayerData is null!");
            return;
        }

        score.text = playerInfo.Score.ToString();
        leaveCount.text = String.Format("剩<color=#F7EC19FF>{0}</color>张", playerInfo.LeaveCardCount);
    }

    /// <summary>
    /// 刷新手牌
    /// </summary>
    public void RefreshCardsUI()
    {
        // 清空旧牌组
        CardSprite[] cardSprites = leaveCardPanel.GetComponentsInChildren<CardSprite>();
        for (int i = 0; i < cardSprites.Length; i++)
        {
            cardSprites[i].Destroy();
        }
        // 刷新新牌组
        if (isSelf)
        {
            Vector3 position = Vector3.zero;
            foreach (int cardId in playerInfo.GetLeavePokeList())
            {
                GameObject poker = Instantiate(ResourceManager.Instance.GetPokerPrefab());
                poker.transform.SetParent(leaveCardPanel.transform);
                poker.transform.localScale = Vector3.one;
                poker.GetComponent<RectTransform>().anchoredPosition3D = position;
                CardSprite cardSprite = poker.GetComponent<CardSprite>();
                cardSprite.CardId = cardId;
                cardSprite.ShowBigSuits = true;
                cardSprite.SetPlayerInfo = playerInfo;

                position.x += 75;
            }
        }
        else
        {
            Vector3 position = Vector3.zero;
            position.y = (playerInfo.LeaveCardCount - 1) * 16 + 40;
            Vector2 anchors = Vector2.zero;
            anchors.x = 0.5f;
            for (int i=0; i< playerInfo.LeaveCardCount; ++i)
            {
                GameObject poker = Instantiate(ResourceManager.Instance.GetPokerPrefab());
                poker.transform.SetParent(leaveCardPanel.transform);
                poker.GetComponent<CardSprite>().CardId = 0;
                RectTransform rect = poker.GetComponent<RectTransform>();
                rect.anchorMin = anchors;
                rect.anchorMax = anchors;
                rect.pivot = anchors;
                rect.anchoredPosition3D = position;

                position.y -= 16;
            }
            RefreshScoreUI();
        }

    }

    public void ClearDiscardUI()
    {
        // 清空旧牌组
        CardSprite[] cardSprites = discardPanel.GetComponentsInChildren<CardSprite>();
        for (int i = 0; i < cardSprites.Length; i++)
        {
            cardSprites[i].Destroy();
        }
        cardsStart.SetActive(false);
    }

    public void RefreshDiscardUI()
    {
        // 清空旧牌组
        CardSprite[] cardSprites = discardPanel.GetComponentsInChildren<CardSprite>();
        for (int i = 0; i < cardSprites.Length; i++)
        {
            cardSprites[i].Destroy();
        }

        Vector3 position = Vector3.zero;
        Vector2 anchors = Vector2.zero;
        anchors.x = 0.5f;
        foreach (int cardId in playerInfo.GetDiscardList())
        {
            GameObject poker = Instantiate(ResourceManager.Instance.GetPokerPrefab());
            poker.transform.SetParent(discardPanel.transform);
            RectTransform rect = poker.GetComponent<RectTransform>();
            rect.anchoredPosition3D = position;
            CardSprite cardSprite = poker.GetComponent<CardSprite>();
            cardSprite.CardId = cardId;
            cardSprite.ShowBigSuits = true;

            position.x += 48;
        }

        if(playerInfo.GetDiscardList().Count == 0)
        {
            cardsStart.SetActive(true);
        }
        else
        {
            cardsStart.SetActive(false);
        }
    }

    public bool ShowIdentity
    {
        set
        {
            if (value)
            {
                SetIdentity = ResourceManager.Instance.GetIdentitySprite(playerInfo.IsDZ);
            }
            identityObj.SetActive(value);
        }
    }

    public void ShowClock(bool show)
    {
        clock.ShowClock(show);
    }


    /// <summary>
    /// 根据url获取玩家微信头像
    /// </summary>
    /// <param name="url"></param>
    /// <param name="image"></param>
    /// <returns></returns>
    IEnumerator DownloadImage(string url, Image image)
    {
        WWW www = new WWW(url);
        yield return www;

        Texture2D tex2d = www.texture; 

        Sprite sprite = Sprite.Create(tex2d, new Rect(0, 0, tex2d.width, tex2d.height), new Vector2(0, 0));
        image.sprite = sprite;
        playerInfo.HeadIcon = sprite;
    }
}
