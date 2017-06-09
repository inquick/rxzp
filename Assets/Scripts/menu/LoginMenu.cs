using UnityEngine;
using System.Collections;
using System.Text;
using UnityEngine.UI;

/// <summary>
/// 登录菜单面板
/// </summary>
public class LoginMenu : MonoBehaviour
{
    public ThirdParty thirdParty;
    public Button loginBtn;
    // Use this for initialization
    void Start()
    {
        //int playerid = PlayerPrefs.GetInt("PlayerId");
        //if (playerid > 0)
        //{
        //    thirdParty.NomalLogin(playerid);
        //}
        //else
        //{
            loginBtn.onClick.AddListener(LoginGame);
        //}
    }

    /// <summary>
    /// 选择简单模式
    /// </summary>
    void LoginGame()
    {
        thirdParty.ThirdPartyLogin();
    }
}
