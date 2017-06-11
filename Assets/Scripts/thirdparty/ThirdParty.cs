using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using netty;
using UnityEngine.UI;

public class ThirdParty : MonoBehaviour
{
    public HomeController controller = null;

    private int playerId = 1001;

#if UNITY_IPHONE
    [DllImport("__Internal")]
    private static extern void weixinLoginByIos();

    /// <summary>
    /// 分享图片链接
    ///         需要注意的是，分享缩略图固定为应用icon图标
    /// </summary>
    /// <param name="url"> 链接地址 </param>
    /// <param name="title"> 标题 </param>
    /// <param name="des"> 描述</param>
    [DllImport("__Internal")]
    private static extern void WXShareToFriend(string url, string title, string des);

    /// <summary>
    /// 截图分享
    /// </summary>
    /// <param name="imagepath">分享的图片本地路径</param>
    [DllImport("__Internal")]
    private static extern void WXShareScreenshot(string imagepath);
#elif UNITY_ANDROID
    private void AndroidLogin()
    {
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        jo.Call("weiLogin");
    }
    private void WXShareToFriend(string url, string title, string des)
    {
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        // 参数  
        string[] mObject = new string[3];
        mObject[0] = url;
        mObject[1] = title;
        mObject[2] = des;
        jo.Call("WXShareToFriend", mObject);
    }

    private void WXShareScreenshot(string imagepath)
    {
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        // 参数  
        string[] mObject = new string[1];
        mObject[0] = imagepath;
        jo.Call("WXShareScreenshot", mObject);
    }
#endif

    public void NomalLogin(string token = "")
    {
        if (PPSocket.GetInstance().Connect(controller))
        {
            MessageInfo req = new MessageInfo();
            LoginReq login = new LoginReq();
            PlayerBaseInfo playerBaseInfo = new PlayerBaseInfo();
            req.messageId = MESSAGE_ID.msg_LoginReq;
            login.code = "";
            if (token.Length > 0)
            {
                login.code = token;
            }
            else
            {
                login.playerid = playerId;
            }
            req.loginReq = login;

            PPSocket.GetInstance().SendMessage(req);

            // 打开主界面
            controller.OpenWindow(WINDOW_ID.WINDOW_ID_HOME);
            // 关闭登录界面
            controller.CloseWindow(WINDOW_ID.WINDOW_ID_LOGIN);
            // 加载条
            controller.LoadingStart();
        }
        else
        {
            controller.ShowTips("网络连接失败，请检查网络是否打开！");
            Debug.LogError("链接服务器失败！");
        }
    }
    public void ThirdPartyLogin()
    {
#if UNITY_IPHONE 
        Debug.logger.Log("IOS准备拉取微信授权登录！");
        weixinLoginByIos();
#elif UNITY_ANDROID
        Debug.logger.Log("Android准备拉取微信授权登录！");
        AndroidLogin();
#elif UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        NomalLogin();
#endif
    }

    public void ThirdPartyShare(string title, string des)
    {
#if UNITY_IPHONE 
        Debug.logger.Log("IOS准备分享！");
        WXShareToFriend(controller.ShareUrl, title, des);
#elif UNITY_ANDROID
        Debug.logger.Log("Android准备分享！");
        WXShareToFriend(controller.ShareUrl, title, des);
#endif
    }

    public void ShareScreenshot(string imgPath)
    {
#if UNITY_IPHONE 
        Debug.logger.Log("IOS准备分享截屏！");
        WXShareScreenshot(imgPath);
#elif UNITY_ANDROID
        Debug.logger.Log("Android准备分享截屏！");
        WXShareScreenshot(imgPath);
#endif
    }

    public void WXCallBack(string param)
    {
        Debug.LogError("微信回调脚本成功！！ param=" + param);
        string[] result = param.Split(' ');
        int code = 0;
        int.TryParse(result[0], out code);
        if (code == 0)
        {
            // 登录成功
            if (PPSocket.GetInstance().IsConnected() || PPSocket.GetInstance().Connect(controller))
            {
                MessageInfo req = new MessageInfo();
                LoginReq login = new LoginReq();
                PlayerBaseInfo playerBaseInfo = new PlayerBaseInfo();
                req.messageId = MESSAGE_ID.msg_LoginReq;
                login.code = result[1];
                req.loginReq = login;

                PPSocket.GetInstance().SendMessage(req);

                // 打开主界面
                controller.OpenWindow(WINDOW_ID.WINDOW_ID_HOME);
                // 关闭登录界面
                controller.CloseWindow(WINDOW_ID.WINDOW_ID_LOGIN);
                // 加载条
                controller.LoadingStart();

#if UNITY_IPHONE 
                // 第三方登录
                controller.ShowTips("微信授权登录，token：" + result[1]);
#endif
            }
            else
            {
                controller.ShowTips("网络连接失败，请检查网络是否打开！");
                Debug.LogError("链接服务器失败！");
            }
        }
        else
        {
            // 登录失败
            controller.ShowTips("微信授权失败，错误码" + result[0]);
        }
    }

    public void WXShareCallBack(string param)
    {
        int code = 0;
        int.TryParse(param, out code);
        if (code == 0)
        {
            // 分享回调
            controller.ShowTips("分享成功");
        }
    }

    public void InputPlayerId(InputField input)
    {
        int.TryParse(input.text, out playerId);
    }
}
