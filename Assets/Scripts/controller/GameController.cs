using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine.UI;

/// <summary>
/// 游戏界面按钮控制
/// </summary>
public class GameController : MonoBehaviour
{
    public Text roomId;

    public Text curCount;

    public Text totalCount;

    public Text curMultiple;

    public Text curBaseScore;

    public HomeController controller;

    public 
    void Start()
    {
        transform.Find("BackHome").gameObject.GetComponent<Button>().onClick.AddListener(BackHome);
    }

    /// <summary>
    /// 选择简单模式
    /// </summary>
    void BackHome()
    {
        controller.OpenWindow(WINDOW_ID.WINDOW_ID_HOME);
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// 刷新游戏信息
    /// </summary>
    public void RefreshGameInfo()
    {
        roomId.text = controller.RoomId.ToString();

        curCount.text = controller.RoomInfo.PlayedGames.ToString();

        totalCount.text = (controller.RoomInfo.PlayedGames + controller.RoomInfo.RemainderGames).ToString();

        curMultiple.text = controller.RoomInfo.Multiple.ToString();

        //curBaseScore.text = controller.RoomInfo.ToString();
    }
}
