using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.Scripts;
using System;

public class SelfInfos : MonoBehaviour
{
    // 微信头像
    public Image headIcon;
    // 名字
    public Text playerName;
    // 玩家id
    public Text playerId;
    // 房卡数量
    public Text cardNum;

    private ClientPlayerInfo playerInfo = null;

    public void RefreshSelfInfos(ClientPlayerInfo cpInfo)
    {
        playerInfo = cpInfo;

        playerName.text = playerInfo.Name;
        playerId.text = playerInfo.PlayerId.ToString();
        cardNum.text = playerInfo.LeaveCardCount.ToString();
        if (playerInfo.HeadIcon != null)
        {
            headIcon.sprite = playerInfo.HeadIcon;
            headIcon.gameObject.SetActive(true);
        }
        else if (playerInfo.HeadIconUrl != "")
        {
            StartCoroutine(DownloadImage(playerInfo.HeadIconUrl, headIcon));
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
        image.gameObject.SetActive(true);
        playerInfo.HeadIcon = sprite;
    }
}
