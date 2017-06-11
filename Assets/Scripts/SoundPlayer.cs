using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using netty;

public class SoundPlayer : MonoBehaviour
{
    //将准备好的MP3格式的背景声音文件拖入此处
    public AudioClip welcomeMusic;
    public AudioClip NNBackgroundMusic;
    public AudioClip DDZBackgroundMusic;

    //按钮音效  
    public AudioClip OnButtonClickSound;

    //用于控制声音的AudioSource组件  
    public AudioSource Music;
    public AudioSource Sound;

    //是否播放游戏背景音乐  
	private bool isPlayMusic = true;

    //是否播放按键音效  
    private bool isPlayButtonSound = true;

    public AudioSource nnPlayerSelf;
    public AudioSource nnPlayer2;
    public AudioSource nnPlayer3;
    public AudioSource nnPlayer4;
    public AudioSource nnPlayer5;

    private Dictionary<int, AudioClip> chatSounds = new Dictionary<int, AudioClip>();
    private Dictionary<NNType, AudioClip> nnSounds = new Dictionary<NNType, AudioClip>();

    void Awake()
    {
        // 加载牛牛配音资源
        for(NNType i=NNType.NNT_NONE;i<NNType.NNT_SPECIAL_BOMEBOME;++i)
        {
            nnSounds.Add(i, Resources.Load<AudioClip>("sound/nn/" + i.ToString()));
        }

        for (int i = 1; i < 6; ++i )
        {
            chatSounds.Add(i, Resources.Load<AudioClip>("sound/nn/chat/" + i.ToString()));
        }

            //设置循环播放  
            Music.loop = true;

        //设置音量为最大，区间在0-1之间  
        Music.volume = 1.0f;
        Sound.volume = 1.0f;

        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            Music.volume = PlayerPrefs.GetFloat("MusicVolume");
        }

        if (PlayerPrefs.HasKey("SoundVolume"))
        {
            Sound.volume = PlayerPrefs.GetFloat("SoundVolume");
        }
        SetSoundVolume(Sound.volume);

        Music.Play();
    }

    void Update()
    {
        //如果isPlayMusic为false,则暂停播放背景音乐  
        if (isPlayMusic == false) Music.Pause();
    }

    void OnGUI()
    {
    }

    // 播放按键音
    public void PlayButtonClickSound()
    {
        if (isPlayButtonSound)
        {
            Sound.PlayOneShot(OnButtonClickSound);
            Debug.Log("播放按钮点击音效!!!");
        }
    }

    /// <summary>
    /// 播放欢迎背景音乐
    /// </summary>
    public void PlayWelcomeMusic()
    {
        Music.clip = welcomeMusic;
        Music.Play();
    }

    /// <summary>
    /// 播放斗地主背景音乐
    /// </summary>
    public void PlayDDZBackgroundMusic()
    {
        Music.clip = DDZBackgroundMusic;
        Music.Play();
    }

    /// <summary>
    /// 播放牛牛背景音乐
    /// </summary>
    public void PlayNNBackgroundMusic()
    {
        Music.clip = NNBackgroundMusic;
        Music.Play();
    }

    /// <summary>
    /// 音乐音量调节
    /// </summary>
    /// <param name="value"></param>
    public void OnMusicVolumeChanged(Slider slider)
    {
        Music.volume = slider.value;
        Debug.Log("当前音乐声音大小：" + slider.value);

        PlayerPrefs.SetFloat("MusicVolume", slider.value);
    }

    /// <summary>
    /// 音乐音量调节
    /// </summary>
    /// <param name="value"></param>
    public void OnSoundVolumeChanged(Slider slider)
    {
        SetSoundVolume(slider.value);
        Debug.Log("当前音效声音大小：" + slider.value);

        PlayerPrefs.SetFloat("SoundVolume", slider.value);
    }

    public void OnPostSendSoundResp(int playerOrder, int soundId)
    {
        switch(playerOrder)
        {
            case 1:
                nnPlayerSelf.clip = chatSounds[soundId];
                nnPlayerSelf.Play();
                break;
            case 2:
                nnPlayer2.clip = chatSounds[soundId];
                nnPlayer2.Play();
                break;
            case 3:
                nnPlayer3.clip = chatSounds[soundId];
                nnPlayer3.Play();
                break;
            case 4:
                nnPlayer4.clip = chatSounds[soundId];
                nnPlayer4.Play();
                break;
            case 5:
                nnPlayer5.clip = chatSounds[soundId];
                nnPlayer5.Play();
                break;
            default:
                break;
        }
    }

    public void PlayeNNSound(int playerOrder, NNType nnType)
    {
        switch (playerOrder)
        {
            case 1:
                nnPlayerSelf.clip = nnSounds[nnType];
                nnPlayerSelf.Play();
                break;
            case 2:
                nnPlayer2.clip = nnSounds[nnType];
                nnPlayer2.Play();
                break;
            case 3:
                nnPlayer3.clip = nnSounds[nnType];
                nnPlayer3.Play();
                break;
            case 4:
                nnPlayer4.clip = nnSounds[nnType];
                nnPlayer4.Play();
                break;
            case 5:
                nnPlayer5.clip = nnSounds[nnType];
                nnPlayer5.Play();
                break;
            default:
                break;
        }
    }

    private void SetSoundVolume(float value)
    {
        Sound.volume = value;
        nnPlayerSelf.volume = value;
        nnPlayer2.volume = value;
        nnPlayer3.volume = value;
        nnPlayer4.volume = value;
        nnPlayer5.volume = value;
    }
}