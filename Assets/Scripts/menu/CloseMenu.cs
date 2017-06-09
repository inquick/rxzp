using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// TIps面板
/// </summary>
public class CloseMenu : MonoBehaviour
{
    public Button closeBtn;
    // Use this for initialization
    void Start()
    {
        if (closeBtn != null)
        {
            closeBtn.onClick.AddListener(CloseWnd);
        }
    }

    /// <summary>
    /// 关闭Tips窗口
    /// </summary>
    public void CloseWnd()
    {
        transform.gameObject.SetActive(false);
    }
}
