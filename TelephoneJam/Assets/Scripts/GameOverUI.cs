using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class GameOverUI : MonoBehaviour
{
    public static GameOverUI Instance;

    [Header("UI")]
    [SerializeField] CanvasGroup canvasGroup;   
    [SerializeField] RectTransform sheet;       
    [SerializeField] Button retryButton;

    [Header("GAME OVER movement")]
    [SerializeField] Vector2 offscreenBottom = new Vector2(0, -400);
    [SerializeField] Vector2 onScreenCenter = new Vector2(-130.8f, 35);
    [SerializeField] float slideTime = 1.2f;


    bool shown;

    void Awake()
    {
        Instance = this;

        if (!canvasGroup) canvasGroup = GetComponent<CanvasGroup>();

        HideInstant();

        if (retryButton)
            retryButton.onClick.AddListener(Retry);
    }

    public void Show()
    {
        if (shown) return;
        shown = true;

        Time.timeScale = 0f;

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;

        sheet.DOKill();
        sheet.anchoredPosition = offscreenBottom;

        sheet.DOAnchorPos(onScreenCenter, slideTime)
             .SetEase(Ease.OutCubic)
             .SetUpdate(true);
    }

    void HideInstant()
    {
        shown = false;

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        if (sheet)
            sheet.anchoredPosition = offscreenBottom;
    }

    public void Retry()
    {
        Time.timeScale = 1f;

        DOTween.KillAll();
        GameManager.Instance.ReloadScene();
    }
}
