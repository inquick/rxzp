using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShopItemMenu : MonoBehaviour
{
    public Button buyBtn;

    public int itemIndex;

	// Use this for initialization
    void Start()
    {
        buyBtn.onClick.AddListener(BuyCards);
	}

    /// <summary>
    /// 关闭窗口
    /// </summary>
    void BuyCards()
    {
        
    }
}
