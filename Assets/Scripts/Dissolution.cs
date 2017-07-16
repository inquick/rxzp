using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using netty;

public class Dissolution : MonoBehaviour
{
    // 倒计时时间条
    public RectTransform LeaveTime;

    Vector2 _size = new Vector2(350, 5);

    public Button agreeBtn;
    public Button refuseBtn;

    public GameObject buttonContainer;

    public AgreeOrRefuse aor1;
    public AgreeOrRefuse aor2;
    public AgreeOrRefuse aor3;
    public AgreeOrRefuse aor4;
    public AgreeOrRefuse aor5;

    public NNRoomSprite room;

    private Dictionary<int, AgreeOrRefuse> aors = new Dictionary<int, AgreeOrRefuse>();

    private DateTime begineTime;

    private int agreeCnt = 1;
    private int refuseCnt = 0;

    private int totalPlayerNum = 0;

    private int time = 60;

	// Use this for initialization
    void Start()
    {
        agreeBtn.onClick.AddListener(Agree);
        refuseBtn.onClick.AddListener(Refuse);
	}
	
	// Update is called once per frame
    void Update()
    {
        _size.x = 350 * (time - (int)(System.DateTime.Now - begineTime).TotalSeconds) / time;

        if (_size.x < 0)
        {
            Show(false);
            return;
        }

        LeaveTime.sizeDelta = _size;
	}

    private void Agree()
    {
        MessageInfo req = new MessageInfo();
        req.messageId = MESSAGE_ID.msg_NNAnswerDissolutionReq;
        NNAnswerDissolutionReq answerDisReq = new NNAnswerDissolutionReq();
        answerDisReq.playerId = room.playerSelf.PlayerInfo.PlayerId;
        answerDisReq.isAgree = true;
        req.nnAnswerDissolutionReq = answerDisReq;

        PPSocket.GetInstance().SendMessage(req);
        // 隐藏掉按钮
        buttonContainer.SetActive(false);
    }

    private void Refuse()
    {
        MessageInfo req = new MessageInfo();
        req.messageId = MESSAGE_ID.msg_NNAnswerDissolutionReq;
        NNAnswerDissolutionReq answerDisReq = new NNAnswerDissolutionReq();
        answerDisReq.playerId = room.playerSelf.PlayerInfo.PlayerId;
        answerDisReq.isAgree = false;
        req.nnAnswerDissolutionReq = answerDisReq;

        PPSocket.GetInstance().SendMessage(req);
        // 隐藏掉按钮
        buttonContainer.SetActive(false);
    }

    public void Show(bool show)
    {
        if (show)
        {
            time = 60;
            if (aors.Count == 0)
            {
                aors.Add(0, aor1);
                aors.Add(1, aor2);
                aors.Add(2, aor3);
                aors.Add(3, aor4);
                aors.Add(4, aor5);
            }
            this.gameObject.SetActive(true);
            begineTime = System.DateTime.Now;
            agreeCnt = 1;
            refuseCnt = 0;
            for(int i=1; i<5; ++i)
            {
                aors[i].SetState(-1);
            }
            agreeCnt = 1;
            totalPlayerNum = 0;
            if(room.playerSelf.PlayerInfo != null)
            {
                ++totalPlayerNum;
            }
            if (room.player2.PlayerInfo != null)
            {
                ++totalPlayerNum;
            }
            if (room.player3.PlayerInfo != null)
            {
                ++totalPlayerNum;
            }
            if (room.player4.PlayerInfo != null)
            {
                ++totalPlayerNum;
            }
            if (room.player5.PlayerInfo != null)
            {
                ++totalPlayerNum;
            }
        }
        else
        {
            buttonContainer.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }

    public void OnPostAnswerDissolution(PostAnswerDissolutionResult msg)
    {
        if (msg.agreeCnt > agreeCnt)
        {
            agreeCnt = msg.agreeCnt;
            aors[agreeCnt + refuseCnt - 1].SetState(1);
        }
        else if (msg.disagreeCnt > refuseCnt)
        {
            refuseCnt = msg.disagreeCnt;
            aors[agreeCnt + refuseCnt - 1].SetState(0);
        }

        if (msg.agreeCnt + msg.disagreeCnt == totalPlayerNum)
        {
            time = (int)(System.DateTime.Now - begineTime).TotalSeconds + 1;
        }
    }

    public void OnReConnect(RoomInfo rinfo)
    {
        agreeCnt = 0;
        refuseCnt = 0;
        buttonContainer.SetActive(true);
        foreach (int agreeId in rinfo.agreePlayerIds)
        {
            aors[agreeCnt].SetState(1);
            ++agreeCnt;

            if(agreeId == room.playerSelf.PlayerInfo.PlayerId)
            {
                buttonContainer.SetActive(false);
            }
        }
        foreach (int refuseId in rinfo.refusePlayerIds)
        {
            aors[agreeCnt + refuseCnt].SetState(0);
            ++refuseCnt;

            if (refuseId == room.playerSelf.PlayerInfo.PlayerId)
            {
                buttonContainer.SetActive(false);
            }
        }
        System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
        begineTime = startTime.AddSeconds(rinfo.startDisbandTime);
        //TimeSpan.FromSeconds(rinfo.startDisbandTime).Ticks;
        time = (int)(System.DateTime.Now - begineTime).TotalSeconds + 1;
        if(time > 60)
        {
            // 大于60秒不显示
            buttonContainer.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }
}
