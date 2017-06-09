using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class SettlementOne : MonoBehaviour
{
    public Image winSprite;
    public WinScore winScore;
    public HomeController conroller;

    private DateTime begineTime;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
    void Update()
    {
        if((System.DateTime.Now - begineTime).TotalSeconds > 5)
        {
            transform.gameObject.SetActive(false);
            conroller.SettlementOneClosed();
        }
	}

    public void SetSettlementUI(int score)
    {
        begineTime = System.DateTime.Now;
        string path = "settlement/";
        if (score > 0)
        {
            path += "win/text";
        }
        else
        {
            path += "lose/text";
        }
        winSprite.sprite = Resources.Load<Sprite>(path);
        winScore.SetScore(score);
    }
}
