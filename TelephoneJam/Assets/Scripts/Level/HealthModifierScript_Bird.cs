using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HealthModifierScript_Bird : HealthModifierScript
{
    private Vector3 _initialLocalPos;
    private Quaternion _initialLocalRot;
    private bool _initialized;


    protected override void Start()
    {


        _initialLocalPos = transform.localPosition;
        _initialLocalRot = transform.localRotation;
        _initialized = true;

        if (GetRingRaceID() != -1)
        {
            gameObject.SetActive(false);
        }
    }

    private void Awake()
    {

        if (!_initialized)
        {
            _initialLocalPos = transform.localPosition;
            _initialLocalRot = transform.localRotation;
        }
    }

    private void OnEnable()
    {
        if (!_initialized)
        {
            _initialLocalPos = transform.localPosition;
            _initialLocalRot = transform.localRotation;
            _initialized = true;
        }


        transform.localPosition = _initialLocalPos;
        transform.localRotation = _initialLocalRot;


        DOTween.Kill(gameObject);

        PlayIdleAnimationStable();
    }

    private void OnDisable()
    {
        DOTween.Kill(gameObject);
    }

    private void PlayIdleAnimationStable()
    {

        if (GetShouldSpin())
        {
            transform.DORotate(new Vector3(0, 360, 0), 360f / GetRotationSpeed(), RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart)
                .SetLink(gameObject);
        }
    }
}
