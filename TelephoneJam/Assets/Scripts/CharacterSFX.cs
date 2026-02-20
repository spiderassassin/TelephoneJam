using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSFX : MonoBehaviour
{
    private GameObject _player;
    private AudioSource audioSource;
    public AudioClip[] audioClips;
    public float volume = 1f;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        audioSource = _player.GetComponent<AudioSource>();
    }

    public void PlayRandomDialogueSFX()
    {
        if(audioClips.Length <= 0) return;
        AudioClip chosenAudio = audioClips[Random.Range(0, audioClips.Length -1)];
        PlayDialogueSFX(chosenAudio);
    }

    public void PlayDialogueSFX(AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip, volume);
    }
    public void StopAudio()
    {
        audioSource.Stop();
    }

}
