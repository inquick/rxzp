using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class Clock : MonoBehaviour
{
    // 计时秒的个、十、百位
    public Image second1;
    public Image second10;
    public Image second100;

    private DateTime begineTime;
    private int nowSecond;

	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        int time = (int)(System.DateTime.Now - begineTime).TotalSeconds;
	    if(time != nowSecond)
        {
            nowSecond = time;
            second1.sprite = ResourceManager.Instance.GetClockTimeSprite(nowSecond % 10);
            second10.sprite = ResourceManager.Instance.GetClockTimeSprite(nowSecond / 10 % 10);
            second100.sprite = ResourceManager.Instance.GetClockTimeSprite(nowSecond / 100 % 10);
        }
	}

    public void ShowClock(bool show)
    {
        this.gameObject.SetActive(show);
        if (show)
        {
            begineTime = System.DateTime.Now;
            nowSecond = 0;
            second1.sprite = ResourceManager.Instance.GetClockTimeSprite(0);
            second10.sprite = ResourceManager.Instance.GetClockTimeSprite(0);
            second100.sprite = ResourceManager.Instance.GetClockTimeSprite(0);
        }
    }
}
