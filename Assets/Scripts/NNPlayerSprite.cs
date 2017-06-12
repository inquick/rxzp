using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.Scripts;
using System;
using netty;

public class NNPlayerSprite : MonoBehaviour
{
    public GameObject pokerContainer;
    public Text playerName;
    public Text playerId;
    public Text playerScore;
    public Image playerIcon;
    public GameObject banker;
    public Image nnType;
    public Image stake;
    public GameObject ok;
    public GameObject offline;

    private Dictionary<int, CardSprite> pokers = new Dictionary<int, CardSprite>();

    private ClientPlayerInfo playerInfo;

    public ClientPlayerInfo PlayerInfo
    {
        set
        {
            if (playerInfo == null)
            {
                pokers.Clear();
                for (int i = 1; i < 6; ++i)
                {
                    pokers.Add(i, transform.FindChild("Pokers/poker" + i).GetComponent<CardSprite>());
                }
            }
            playerInfo = value;
            if (playerInfo != null)
            {
                this.gameObject.SetActive(true);

                playerName.text = playerInfo.Name;
                playerId.text = "ID:" + playerInfo.PlayerId;
                playerScore.text = "0";
                if (playerInfo.HeadIconUrl == null || playerInfo.HeadIconUrl.Length == 0)
				{
					playerInfo.HeadIconUrl = "https://timgsa.baidu.com/timg?image&quality=80&size=b9999_10000&sec=1492235773979&di=42b5ddb3d50d6ea32fafee903833c44c&imgtype=0&src=http%3A%2F%2Fwenwen.soso.com%2Fp%2F20110825%2F20110825115928-858187777.jpg";
                }
				StartCoroutine(DownloadImage(playerInfo.HeadIconUrl, playerIcon));
                banker.SetActive(false);
                playerIcon.gameObject.SetActive(false);
                pokerContainer.SetActive(false);
                nnType.gameObject.SetActive(false);
                stake.gameObject.SetActive(false);

                this.gameObject.SetActive(true);

                offline.SetActive(!playerInfo.GetIsOnline);
            }
        }
        get { return playerInfo; }
    }

	// Use this for initialization
	void Start () {
        pokerContainer.SetActive(false);
        nnType.gameObject.SetActive(false);
        stake.gameObject.SetActive(false);
        offline.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void OnPostDealResp(PostNNDealResp msg)
    {
        pokerContainer.SetActive(true);
        int index = 1;
        foreach (int cardId in msg.pokers)
        {
            pokers[index].CardId = cardId;
            ++index;
        }
        
        nnType.gameObject.SetActive(false);
        stake.gameObject.SetActive(false);
    }

    // 显示玩家UI信息
    public void ShowCardBack()
    {
        if (playerInfo != null)
        {
            pokerContainer.SetActive(true);
            pokers[1].CardId = 0;
            pokers[2].CardId = 0;
            pokers[3].CardId = 0;
            pokers[4].CardId = 0;
            pokers[5].CardId = 0;
        }
    }

    public void OnShowCards(NNShowCardsResp msg)
    {
        int index = 4;
        foreach (int cardId in msg.pokers)
        {
            pokers[index].CardId = cardId;
            ++index;
        }

        nnType.sprite = ResourceManager.Instance.GetNiuNiuTypeSprite(msg.nntype);
        nnType.SetNativeSize();
        nnType.gameObject.SetActive(true);
    }

    public void OnPostShowCards(PostNNShowCards msg)
    {
        int index = 1;
        Debug.Log(pokers.ToString());
        foreach (int cardId in msg.pokers)
        {
            Debug.Log("index = " + index);
            pokers[index].CardId = cardId;
            ++index;
        }

        nnType.sprite = ResourceManager.Instance.GetNiuNiuTypeSprite(msg.nntype);
        nnType.SetNativeSize();
        nnType.gameObject.SetActive(true);
    }

    public void ShowStake(int _stake)
    {
        stake.sprite = ResourceManager.Instance.GetStakeSprite(_stake);
        stake.gameObject.SetActive(true);
    }
    // 显示隐藏ok手势
    public void ShowOk(bool show)
    {
        ok.SetActive(show);
    }

    public void Again()
    {
        if (playerInfo != null)
        {
            pokers[1].CardId = 0;
            pokers[2].CardId = 0;
            pokers[3].CardId = 0;
            pokers[4].CardId = 0;
            pokers[5].CardId = 0;
            ShowOk(false);
            stake.gameObject.SetActive(false);
            banker.SetActive(false);
            pokerContainer.SetActive(false);
            nnType.gameObject.SetActive(false);
            offline.SetActive(false);
        }
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
        playerIcon.gameObject.SetActive(true);
        playerInfo.HeadIcon = sprite;
    }

    public void OnPostUnusualQuit()
    {
        offline.SetActive(true);
    }

    public void OnPostPlayerOnline()
    {
        offline.SetActive(false);
    }

    public void ShowBanker(int banderid)
    {
        if (banderid == playerInfo.PlayerId)
        {
            banker.SetActive(true);
        }
        else
        {
            banker.SetActive(false);
        }
    }
}
