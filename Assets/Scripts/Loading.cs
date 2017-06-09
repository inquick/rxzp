using UnityEngine;
using System.Collections;

public class Loading : MonoBehaviour {

    public RectTransform progress;

    Vector2 _size = new Vector2(28, 84);
	// Use this for initialization
    void Start()
    {
	}

    private void init()
    {
    }

    public void StartLoading()
    {
        _size.x = 28;
        this.gameObject.SetActive(true);
        Debug.Log("<color=#00ff00ff>StartLoading!!!</color>");
    }

    public void EndLoading()
    {
        _size.x = 1109;
        Debug.Log("<color=#00ff00ff>EndLoading!!!</color>");
    }
	
	// Update is called once per frame
    void Update()
    {
        if (_size.x < 1090)
        {
            _size.x += Random.Range(1, 8);
            if (_size.x > 1109)
            {
                _size.x = 1109;
            }
            progress.sizeDelta = _size;
        }
        else if (_size.x == 1109)
        {
            this.gameObject.SetActive(false);
        }
        //Debug.Log("<color=#00ff00ff>Loading Length = " + _size.x + "</color>");
	}
}
