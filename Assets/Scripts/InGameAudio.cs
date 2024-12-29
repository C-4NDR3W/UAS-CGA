using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameAudio : MonoBehaviour
{
    public static InGameAudio Instance;

    [SerializeField]
    private AudioSource music;

    [SerializeField]
    private AudioClip background;

    [SerializeField]
    private AudioSource click;

    [SerializeField]
    private AudioClip clickSound;

    public float musicVolume = 0.3f;
    public float soundVolume = 0.3f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayMusicForLevel();
    }

    private void Update()
    {
        if (background != GetLevelMusic())
        {
            PlayMusicForLevel();
        }
    }

    public void PlayClickSound()
    {
        click.PlayOneShot(clickSound);
    }

    public void PlayMusicForLevel()
    {
        background = GetLevelMusic();
        if (background != null)
        {
            music.clip = background;
            music.volume = musicVolume;
            music.Play();
        }
    }

    private AudioClip GetLevelMusic()
    {
        switch (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex)
        {
            case 1:
                return Resources.Load<AudioClip>("Audio/Music/Level1");
            case 2:
                return Resources.Load<AudioClip>("Audio/Music/Level2");
            case 3:
                return Resources.Load<AudioClip>("Audio/Music/Level3");
            case 4:
                return Resources.Load<AudioClip>("Audio/Music/LevelBoss");
            default:
                return Resources.Load<AudioClip>("Audio/Music/Main Menu");
        }
    }
}
