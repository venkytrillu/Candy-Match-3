using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    public static AudioController instacne;

    private AudioSource audioSource;
    public Sprite MusicOnSprite, MusicOffSprite;
    private static bool isMusic;

    private void Awake()
    {
        if(instacne==null)
        {
            instacne = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
       
    }
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    

    public void MusicOnOff(Image sprite)
    {
        if (!isMusic)
        {
            sprite.sprite = MusicOffSprite;
            audioSource.enabled = false;
            isMusic = true;
        }
        else if (isMusic)
        {
            sprite.sprite = MusicOnSprite;
            audioSource.enabled = true;
            isMusic = false;
        }
    }


}//class


























