using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] 
    private AudioSource music;

    [SerializeField]
    private AudioClip background;

    public float volume = 0.3f;

    private void Start()
    {
        music.clip = background;
        music.volume = volume;
        music.Play();
    }

    private void Update()
    {
        music.volume = volume;
    }
}
