using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HealthPopup : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private RectTransform rect;
    [SerializeField] private CanvasGroup group;
    [SerializeField] private Image img;
    [SerializeField] private AudioSource audioSource;

    [Header("HERO CARDS OML")]
    public Sprite healIcon;
    public Sprite damageIcon;

    [Header("Movement")]
    public Vector2 hiddenOffset = new Vector2(0, -300);
    public float inTime = 0.12f;
    public float holdTime = 0.20f;
    public float outTime = 0.15f;

    private Vector2 basePos;
    private Sequence seq;

    void Awake()
    {
        if (!rect) rect = GetComponent<RectTransform>();
        if (!group) group = GetComponent<CanvasGroup>();
        if (!img) img = GetComponent<Image>();
        if (!audioSource) audioSource = GetComponent<AudioSource>();

        basePos = rect.anchoredPosition;

        group.alpha = 0f;
        rect.anchoredPosition = basePos + hiddenOffset;
        gameObject.SetActive(false);
    }

    public void ShowHeal() => Show(healIcon);
    public void ShowDamage() => Show(damageIcon);

    private void Show(Sprite sprite)
    {
        if (!sprite) return;

        gameObject.SetActive(true);
        img.sprite = sprite;

        seq?.Kill();

        group.alpha = 0f;
        rect.anchoredPosition = basePos + hiddenOffset;

        seq = DOTween.Sequence().SetUpdate(true);
        seq.Append(group.DOFade(1f, inTime));
        seq.Join(rect.DOAnchorPos(basePos, inTime).SetEase(Ease.OutBack));
        seq.AppendInterval(holdTime);
        seq.Append(group.DOFade(0f, outTime));
        seq.Join(rect.DOAnchorPos(basePos + hiddenOffset, outTime).SetEase(Ease.InCubic));
        seq.OnComplete(() => gameObject.SetActive(false));
    }
}

