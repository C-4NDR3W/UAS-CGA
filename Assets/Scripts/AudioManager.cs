using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource music;

    [SerializeField]
    private AudioClip background;

    [SerializeField]
    private AudioSource click;

    [SerializeField]
    private AudioClip clickSound;

    [SerializeField]
    private Slider musicVolumeSlider;

    [SerializeField]
    private Slider soundVolumeSlider;

    public float musicVolume = 0.3f;
    public float soundVolume = 0.3f;

    private void Start()
    {
        music.clip = background;
        music.volume = musicVolume;
        music.Play();

        musicVolumeSlider.value = musicVolume;
        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChange);

        soundVolumeSlider.value = musicVolume;
        soundVolumeSlider.onValueChanged.AddListener(OnSoundVolumeChange);
    }

    private void Update()
    {
        music.volume = musicVolume;
    }

    private void OnMusicVolumeChange(float value)
    {
        musicVolume = value;
    }

    private void OnSoundVolumeChange(float value)
    {
        soundVolume = value;
        if (click != null)
        {
            click.volume = soundVolume;
        }
    }

    public void PlayClickSound()
    {
        click.PlayOneShot(clickSound);
    }
}
