using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChooseServer : MonoBehaviour {

    public HomeController controller;
	// Use this for initialization
	void Start () {
        transform.GetComponent<Dropdown>().onValueChanged.AddListener(OnValueChanged);
        if (PlayerPrefs.HasKey("ChooseServer"))
        {
            transform.GetComponent<Dropdown>().value = PlayerPrefs.GetInt("ChooseServer");
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnValueChanged(int value)
    {
        controller.ipIndex = (ServerIp)value;
        PlayerPrefs.SetInt("ChooseServer", value);
    }
}
