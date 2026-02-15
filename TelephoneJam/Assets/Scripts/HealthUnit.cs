using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;


public class HealthUnit : MonoBehaviour
{
    [SerializeField] float spawnScaleUp = 1.2f;
    [SerializeField] float spawnDuration = 0.5f;


    [SerializeField] float iniShakeDuration = 0.5f;
    [SerializeField] float iniShakeStrength = 0.3f;
    [SerializeField] int iniShakeVibrato = 20;


    [SerializeField] float shakeDuration = 0.5f;
    [SerializeField] float shakeStrength = 0.3f;
    [SerializeField] int shakeVibrato = 20;

    [SerializeField] float punchScale = 1.4f;
    [SerializeField] float scaleDuration = 0.2f;

    [SerializeField] Color hitColor = Color.red;
    [SerializeField] float colorDuration = 0.3f;


    Image image;
    void Awake()
    {
        image = GetComponent<Image>();
    }
    void Start()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(spawnScaleUp, spawnDuration).SetEase(Ease.OutBack));
        seq.Append(transform.DOScale(1f, spawnDuration).SetEase(Ease.InBack));
        seq.OnComplete(() =>
        {
            transform.DOShakePosition(iniShakeDuration, iniShakeStrength, iniShakeVibrato)
            .SetLoops(-1, LoopType.Restart).SetLink(gameObject);
        });

    }
    public void DestroyObject()
    {
        Sequence seq = DOTween.Sequence();

        // Punch scale up and color change
        seq.Append(transform.DOScale(punchScale, scaleDuration).SetEase(Ease.OutBack));
        seq.Join(image.DOColor(hitColor, colorDuration));

        // Shake/vibrate and then destroy
        seq.Append(transform.DOShakePosition(shakeDuration, shakeStrength, shakeVibrato));
        seq.Append(transform.DOScale(0f, 0.15f).SetEase(Ease.InBack));
        seq.OnComplete(() => Destroy(gameObject));
    }
}
