using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameFlowController : MonoBehaviour
{
    [Header("Title UI")]
    public CanvasGroup titleGroup;
    public Button playButton;

    [Header("HUD")]
    public GameObject playerUI;

    [Header("Cameras")]
    public GameObject freeLookCamera; 

    [Header("Player")]
    public MonoBehaviour playerController; 

    [Header("HERO CARD OML")]
    public RectTransform icon;
    public Vector2 offscreenBottom = new Vector2(0, -380);
    public Vector2 coverCenter = new Vector2(0, -5.5f);
    public Vector2 offscreenTop = new Vector2(0, 900);

    public float flyInTime = 0.35f;
    public float holdTime = 0.10f;
    public float flyOutTime = 0.35f;

    [Header("Audio - SFX")]
    public AudioSource audioSource;
    public AudioClip batman;

    [Header("Audio - Music")]
    public AudioSource musicSource;
    public AudioClip backingTrack;
    public float musicDelay = 1.0f;

    bool started;

    void Awake()
    {
        // Title screen mode
        if (playerUI) playerUI.SetActive(false);
        if (freeLookCamera) freeLookCamera.SetActive(false);
        if (playerController) playerController.enabled = false;

        if (icon) icon.anchoredPosition = offscreenBottom;

        if (titleGroup)
        {
            titleGroup.alpha = 1f;
            titleGroup.interactable = true;
            titleGroup.blocksRaycasts = true;
        }

        if (musicSource && backingTrack)
        {
            musicSource.clip = backingTrack;
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }
    }

    public void OnPlayPressed()
    {
        if (started) return;
        started = true;

        if (playButton) playButton.interactable = false;

        // thank you for tween :D
        var seq = DOTween.Sequence().SetUpdate(true);

        seq.AppendCallback(() =>
        {
            if (audioSource && batman) audioSource.PlayOneShot(batman);
            if (musicSource && backingTrack) musicSource.PlayDelayed(musicDelay);
        });

        seq.Append(icon.DOAnchorPos(coverCenter, flyInTime).SetEase(Ease.OutCubic));

        seq.AppendCallback(() =>
        {
            if (titleGroup)
            {
                titleGroup.alpha = 0f;
                titleGroup.interactable = false;
                titleGroup.blocksRaycasts = false;
            }
        });

        seq.AppendInterval(holdTime);

        seq.Append(icon.DOAnchorPos(offscreenTop, flyOutTime).SetEase(Ease.InCubic));

        seq.OnComplete(() =>
        {
            if (playerUI) playerUI.SetActive(true);
            if (freeLookCamera) freeLookCamera.SetActive(true);
            if (playerController) playerController.enabled = true;
        });
    }
}

