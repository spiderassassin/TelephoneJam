using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HealthModifierScript : MonoBehaviour
{

    public enum EffectType { Heal, Damage, SetMaxHealth }
    [SerializeField] EffectType effectType = EffectType.Heal;

    
    [SerializeField] int amount = 1;
    [SerializeField] bool destroyOnUse = true; // if you want to destroy it after onTrigger


    [SerializeField] float bobbingHeight = 0.3f;
    [SerializeField] float bobbingDuration = 0.8f;
    [SerializeField] int shakeVibrato = 10;

    void Start()
    {
        PlayIdleAnimation();
    }

    void PlayIdleAnimation()
    {
        transform.DOLocalMoveY(transform.localPosition.y + bobbingHeight, bobbingDuration)
            .SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).SetLink(gameObject); // bobbing effect
        
        transform.DOShakeRotation(bobbingDuration, new Vector3(0, 0, 10f), shakeVibrato, 90)
            .SetLoops(-1, LoopType.Restart).SetLink(gameObject); // Rotating shake
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        // damaging or healing player
        PlayerStat playerStat = other.GetComponent<PlayerStat>();

        switch (effectType)
        {
            case EffectType.Heal:
                playerStat.HealHealth(amount);
                break;
            case EffectType.Damage:
                playerStat.ReduceHealth(amount);
                break;
            case EffectType.SetMaxHealth:
                playerStat.SetMaxHealth(amount);
                break;
        }
        

        if (destroyOnUse)
            Destroy(gameObject);
    }
}
