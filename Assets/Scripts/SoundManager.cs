using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip[] destroyNoise;
    public AudioSource audioSource;

    public void PlayRandomDestroyNoise()
    {
        //Choose a random number
        int clipToPlay = Random.Range(0, destroyNoise.Length);
        //play that clip
        audioSource.clip = destroyNoise[clipToPlay];
        audioSource.Play();
    }

    
}
