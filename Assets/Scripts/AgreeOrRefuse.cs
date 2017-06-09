using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class AgreeOrRefuse : MonoBehaviour
{
    // 同意
    public GameObject agree;
    // 拒绝
    public GameObject refuse;

	// Use this for initialization
    void Start()
    {
	}
	
	// Update is called once per frame
    void Update()
    {
	}

    public void SetState(int state)
    {
        if (state == 1)
        {
            agree.SetActive(true);
            refuse.SetActive(false);
        }
        else if (state == 0)
        {
            agree.SetActive(false);
            refuse.SetActive(true);
        }
        else
        {
            agree.SetActive(false);
            refuse.SetActive(false);
        }
    }
}
