using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WinScore : MonoBehaviour
{
    public Image Symbol;
    public Image score1;
    public Image score2;
    public Image score3;
    public GameObject scoreObj1;
    public GameObject scoreObj2;
    public GameObject scoreObj3;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void SetScore(int score)
    {
        string path = "settlement";
        if (score > 0)
        {
            path += "/win/";
        }
        else
        {
            score *= -1;
            path += "/lose/";
        }

        Symbol.sprite = Resources.Load<Sprite>(path + "symbol");

        int num100 = score / 100;
        int num10 = score % 100 / 10;
        int mum1 = score % 10;

        if (num100 > 0)
        {
            score1.sprite = Resources.Load<Sprite>(path + num100);
            score2.sprite = Resources.Load<Sprite>(path + num10);
            score3.sprite = Resources.Load<Sprite>(path + mum1);
            scoreObj1.SetActive(true);
            scoreObj2.SetActive(true);
            scoreObj3.SetActive(true);
        }
        else if (num10 > 0)
        {
            score1.sprite = Resources.Load<Sprite>(path + num10);
            score2.sprite = Resources.Load<Sprite>(path + mum1);
            score3.sprite = null;
            scoreObj1.SetActive(true);
            scoreObj2.SetActive(true);
            scoreObj3.SetActive(false);

        }
        else
        {
            score1.sprite = Resources.Load<Sprite>(path + mum1);
            score2.sprite = null;
            score3.sprite = null;
            scoreObj1.SetActive(true);
            scoreObj2.SetActive(false);
            scoreObj3.SetActive(false);
        }
    }
}
