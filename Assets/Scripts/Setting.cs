using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    public Slider music;
    public Slider sound;
    void Start()
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            music.value = PlayerPrefs.GetFloat("MusicVolume");
        }
        if (PlayerPrefs.HasKey("SoundVolume"))
        {
            sound.value = PlayerPrefs.GetFloat("SoundVolume");
        }
	}

	// Update is called once per frame
    void Update()
    {
	}
}
