using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using netty;

/// <summary>
/// TIps面板
/// </summary>
public class ChatChooseMenu : MonoBehaviour
{
    public Button chatSoundBtn1;
    public Button chatSoundBtn2;
    public Button chatSoundBtn3;
    public Button chatSoundBtn4;
    public Button chatSoundBtn5;

    public NNRoomSprite room;

    // Use this for initialization
    void Start()
    {
        chatSoundBtn1.onClick.AddListener(CheckChatSound1);
        chatSoundBtn2.onClick.AddListener(CheckChatSound2);
        chatSoundBtn3.onClick.AddListener(CheckChatSound3);
        chatSoundBtn4.onClick.AddListener(CheckChatSound4);
        chatSoundBtn5.onClick.AddListener(CheckChatSound5);
    }

    /// <summary>
    /// 关闭Tips窗口
    /// </summary>
    public void CloseWnd()
    {
        transform.gameObject.SetActive(false);
    }

    void CheckChatSound1()
    {
        SendSound(1);
    }
    void CheckChatSound2()
    {
        SendSound(2);
    }
    void CheckChatSound3()
    {
        SendSound(3);
    }
    void CheckChatSound4()
    {
        SendSound(4);
    }
    void CheckChatSound5()
    {
        SendSound(5);
    }

    private void SendSound(int soundId)
    {
        MessageInfo req = new MessageInfo();
        req.messageId = MESSAGE_ID.msg_SendSoundReq;
        SendSoundReq chatReq = new SendSoundReq();
        chatReq.playerId = room.playerSelf.PlayerInfo.PlayerId;
        chatReq.soundId = soundId;
        req.sendSoundReq = chatReq;

        PPSocket.GetInstance().SendMessage(req);
    }
}
