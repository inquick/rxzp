using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using UnityEngine.UI;
using netty;
using System.Collections.Generic;

/// <summary>
/// 菜单面板
/// </summary>
public class EnterMenu : MonoBehaviour
{
    public HomeController controller;

    private Dictionary<int, Sprite> NumImages;
    private int[] roomNums;
    private Dictionary<int, Image> roomNumImages;
    private Dictionary<int, GameObject> roomNumObjects;

    private int currentNumIndex = -1;
    private bool beginEnterRoom = false;

    private GameType m_gameType;

    public GameType SetGameType
    {
        set { m_gameType = value; }
    }

    // Use this for initialization
    void Start()
    {
        Debug.Log("EnterMenu Start!");
        NumImages = new Dictionary<int, Sprite>();
        roomNumImages = new Dictionary<int, Image>();
        roomNumObjects = new Dictionary<int, GameObject>();
        roomNums = new int[4];
        for (int i = 0; i < 10; ++i)
        {
            NumImages.Add(i, transform.Find("Numbers/" + i).GetComponent<Image>().sprite);
        }

        roomNumObjects.Add(0, transform.Find("bg/RoomID/Num1/Num").gameObject);
        roomNumObjects.Add(1, transform.Find("bg/RoomID/Num2/Num").gameObject);
        roomNumObjects.Add(2, transform.Find("bg/RoomID/Num3/Num").gameObject);
        roomNumObjects.Add(3, transform.Find("bg/RoomID/Num4/Num").gameObject);

        roomNumImages.Add(0, transform.Find("bg/RoomID/Num1/Num").GetComponent<Image>());
        roomNumImages.Add(1, transform.Find("bg/RoomID/Num2/Num").GetComponent<Image>());
        roomNumImages.Add(2, transform.Find("bg/RoomID/Num3/Num").GetComponent<Image>());
        roomNumImages.Add(3, transform.Find("bg/RoomID/Num4/Num").GetComponent<Image>());
        Reset();

        transform.Find("bg/Close").gameObject.GetComponent<Button>().onClick.AddListener(Close);

        transform.Find("bg/KeyBoard/KeyReset").gameObject.GetComponent<Button>().onClick.AddListener(Reset);
        transform.Find("bg/KeyBoard/KeyDelet").gameObject.GetComponent<Button>().onClick.AddListener(Delet);

        transform.Find("bg/KeyBoard/Key1").gameObject.GetComponent<Button>().onClick.AddListener(Key1);
        transform.Find("bg/KeyBoard/Key2").gameObject.GetComponent<Button>().onClick.AddListener(Key2);
        transform.Find("bg/KeyBoard/Key3").gameObject.GetComponent<Button>().onClick.AddListener(Key3);
        transform.Find("bg/KeyBoard/Key4").gameObject.GetComponent<Button>().onClick.AddListener(Key4);
        transform.Find("bg/KeyBoard/Key5").gameObject.GetComponent<Button>().onClick.AddListener(Key5);
        transform.Find("bg/KeyBoard/Key6").gameObject.GetComponent<Button>().onClick.AddListener(Key6);
        transform.Find("bg/KeyBoard/Key7").gameObject.GetComponent<Button>().onClick.AddListener(Key7);
        transform.Find("bg/KeyBoard/Key8").gameObject.GetComponent<Button>().onClick.AddListener(Key8);
        transform.Find("bg/KeyBoard/Key9").gameObject.GetComponent<Button>().onClick.AddListener(Key9);
        transform.Find("bg/KeyBoard/Key0").gameObject.GetComponent<Button>().onClick.AddListener(Key0);

        Debug.Log("EnterMenu Start success!");
    }

    void Close()
    {
		beginEnterRoom = false;
		Reset ();
        transform.gameObject.SetActive(false);
    }

    void Reset()
    {
        for (int i = 0; i < 4; ++i )
        {
            roomNumObjects[i].SetActive(false);
            roomNums[i] = -1;
        }
		currentNumIndex = -1;
    }

    void Delet()
    {
        if (currentNumIndex > -1)
        {
            roomNums[currentNumIndex] = -1;
            roomNumObjects[currentNumIndex].SetActive(false);
            --currentNumIndex;
        }
    }

    void Key1()
    {
        KeyDown(1);
    }
    void Key2()
    {
        KeyDown(2);
    }
    void Key3()
    {
        KeyDown(3);
    }
    void Key4()
    {
        KeyDown(4);
    }
    void Key5()
    {
        KeyDown(5);
    }
    void Key6()
    {
        KeyDown(6);
    }
    void Key7()
    {
        KeyDown(7);
    }
    void Key8()
    {
        KeyDown(8);
    }
    void Key9()
    {
        KeyDown(9);
    }
    void Key0()
    {
        KeyDown(0);
    }

    void KeyDown(int key)
    {
        if (currentNumIndex < 3)
        {
            currentNumIndex++;
            roomNums[currentNumIndex] = key;
            roomNumImages[currentNumIndex].sprite = NumImages[key];
            roomNumObjects[currentNumIndex].SetActive(true);

            if (currentNumIndex == 3 && !beginEnterRoom)
            {
                beginEnterRoom = true;
                MessageInfo req = new MessageInfo();
                if (m_gameType == GameType.GT_DDZ)
                {
                    EntryRoomReq entryRoom = new EntryRoomReq();
                    req.messageId = MESSAGE_ID.msg_EntryRoomReq;
                    entryRoom.roomId = roomNums[0] * 1000 + roomNums[1] * 100 + roomNums[2] * 10 + roomNums[3];
                    entryRoom.playerId = controller.PlayerId;
                    req.entryRoomReq = entryRoom;
                    controller.RoomId = entryRoom.roomId;
                }
                else if (m_gameType == GameType.GT_NN)
                {
                    EntryNNRoomReq entryRoom = new EntryNNRoomReq();
                    req.messageId = MESSAGE_ID.msg_EntryNNRoomReq;
                    entryRoom.roomId = roomNums[0] * 1000 + roomNums[1] * 100 + roomNums[2] * 10 + roomNums[3];
                    entryRoom.playerId = controller.PlayerId;
                    req.entryNNRoomReq = entryRoom;
                    controller.RoomId = entryRoom.roomId;
                }

                PPSocket.GetInstance().SendMessage(req);

                controller.LoadingStart();
                Close();
            }
        }
    }
}
