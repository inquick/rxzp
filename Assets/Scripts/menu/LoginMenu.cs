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
    public GameObject input;
    public GameObject chooseServer;
    public Toggle chooseHeartBeat;
    public Text version;
    // Use this for initialization
    void Start()
    {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        input.SetActive(true);
        chooseServer.SetActive(true);
        chooseHeartBeat.gameObject.SetActive(true);

        if (PlayerPrefs.HasKey("TestHeartBeat"))
        {

            chooseHeartBeat.isOn = PlayerPrefs.GetInt("TestHeartBeat") == 1;
        }
#endif
        //int playerid = PlayerPrefs.GetInt("PlayerId");
        //if (playerid > 0)
        //{
        //    thirdParty.NomalLogin(playerid);
        //}
        //else
        //{
            loginBtn.onClick.AddListener(LoginGame);
        //}

            version.text = Strings.SS_VERSION + Application.version;
    }

    /// <summary>
    /// 选择简单模式
    /// </summary>
    void LoginGame()
    {
        thirdParty.ThirdPartyLogin();
    }
}
