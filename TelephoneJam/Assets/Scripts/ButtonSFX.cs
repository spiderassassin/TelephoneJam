using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSFX : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Sounds for buttons")]
    public AudioClip buttonHoverSFX;
    public AudioClip buttonHoldFX;
    public AudioClip buttonClickSFX;
    public float volume = 1f;
    private GameObject _player;
    private AudioSource audioSource;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        audioSource = _player.GetComponent<AudioSource>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        audioSource.PlayOneShot(buttonHoverSFX, volume);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        audioSource.PlayOneShot(buttonHoldFX, volume);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        audioSource.PlayOneShot(buttonClickSFX, volume);
    }
}
