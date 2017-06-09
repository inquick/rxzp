using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class AsyncImageDownload : MonoBehaviour
{
    private Sprite[] wxPic;                              //存在内存中的头像数组  
    Dictionary<int, int> code2PicID;                     //url的hashcode -> spriteID  
    private static int spriteCnt;

    private static AsyncImageDownload _instance = null;
    public static AsyncImageDownload GetInstance() { return Instance; }
    public static AsyncImageDownload Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("AsyncImageDownload");
                _instance = obj.AddComponent<AsyncImageDownload>();
                DontDestroyOnLoad(obj);
                _instance.Init();
            }
            return _instance;
        }
    }

    public bool Init()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/ImageCache/"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/ImageCache/");
        }
        wxPic = new Sprite[55];                  //默认不超过55个url  
        spriteCnt = 0;
        code2PicID = new Dictionary<int, int>();
        return true;
    }

    public void SetAsyncImage(string url, Image image)
    {
        //开始下载图片前，将UITexture的主图片设置为占位图  
        int code = url.GetHashCode();
        if (code2PicID.ContainsKey(code))
        {
            image.sprite = wxPic[code2PicID[code]];
        }
        else
        {
            // 如果之前不存在缓存中  就用WWW类下载
            if (spriteCnt >= 54)
                return;
            StartCoroutine(DownloadImage(url, image, code));
        }
    }

    IEnumerator DownloadImage(string url, Image image, int code)
    {
        WWW www = new WWW(url);
        yield return www;

        Texture2D tex2d = www.texture;
        //将图片保存至缓存路径  
        //        byte[] pngData = tex2d.EncodeToPNG();                                 不存图片了..  
        //      File.WriteAllBytes(path + url.GetHashCode(), pngData);  

        Sprite m_sprite = Sprite.Create(tex2d, new Rect(0, 0, tex2d.width, tex2d.height), new Vector2(0, 0));
        image.sprite = m_sprite;
        code2PicID[code] = spriteCnt;
        wxPic[spriteCnt] = m_sprite;
        ++spriteCnt;
    }

    IEnumerator LoadLocalImage(string url, Image image)
    {
        string filePath = "file:///" + path + url.GetHashCode();
        //     Debug.Log("getting local image:" + filePath);  
        WWW www = new WWW(filePath);
        yield return www;

        Texture2D texture = www.texture;
        Sprite m_sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
        image.sprite = m_sprite;
    }

    public string path
    {
        get
        {
            //pc,ios //android :jar:file//  
            return Application.persistentDataPath + "/ImageCache/";
        }
    }
}