using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using RingRace;
using UnityEditor;

public class HealthModifierScript : MonoBehaviour
{

    public enum EffectType { Heal, Damage, SetMaxHealth }
    [SerializeField] EffectType effectType = EffectType.Heal;

    
    [SerializeField] int amount = 1;
    [SerializeField] bool destroyOnUse = true; // if you want to destroy it after onTrigger
    [SerializeField] bool shouldSpin = true; // if you want it to have a spinning animation
    [SerializeField] float rotationSpeed = 50f; // the speed of the spinning animation


    [SerializeField] float bobbingHeight = 0.3f;
    [SerializeField] float bobbingDuration = 0.8f;
    [SerializeField] int shakeVibrato = 10;
    
    [Header("Ring Race Specifics")]
    [SerializeField, Tooltip("If you want this to appear/dissapear during a race. -1 means no race")] int ringRaceID = -1; public int GetRingRaceID() { return ringRaceID; }

    void Start()
    {
        PlayIdleAnimation();
        if (ringRaceID != -1)
        {
            gameObject.SetActive(false);
        }
    }

    void PlayIdleAnimation()
    {
        transform.DOLocalMoveY(transform.localPosition.y + bobbingHeight, bobbingDuration)
            .SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).SetLink(gameObject); // bobbing effect
        
        transform.DOShakeRotation(bobbingDuration, new Vector3(0, 0, 10f), shakeVibrato, 90)
            .SetLoops(-1, LoopType.Restart).SetLink(gameObject); // Rotating shake
        
        if (shouldSpin)
        {
            Transform spinTarget = transform.parent != null ? transform.parent : transform;
            spinTarget.DORotate(new Vector3(0, 360, 0), 360f / rotationSpeed, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart).SetLink(gameObject);
        }
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
        {
            // switching to disabling for race ones
            if (ringRaceID != -1)
            {
                gameObject.SetActive(false);
                return;
            }

            Destroy(gameObject);
        }

    }
    
    // we will draw gizmos above pickups to show what raceID they belong to (only if its not -1)
    private void OnDrawGizmos()
    {
        if (ringRaceID == -1){ return;}
        GUIStyle style = new GUIStyle
        {
            normal = { textColor = Color.white },
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold,
            fontSize = 12
        };

        string label = "RaceId: " + ringRaceID.ToString();
        Handles.Label(transform.position + Vector3.up * 0.75f, label, style);
        
        // draw a line between the pickup, and the Start ring of the raceID it belongs to for better visualization
        HealthModifierScript[] pickups = FindObjectsOfType<HealthModifierScript>(true);
        foreach (HealthModifierScript pickup in pickups)
        {
            if (pickup.GetRingRaceID() == ringRaceID)
            {
                RingBase[] rings = FindObjectsOfType<RingBase>(true);
                RingBase closestRing = null;
                float closestDistance = float.MaxValue;

                foreach (RingBase ring in rings)
                {
                    if (ring.GetRaceID() != ringRaceID) continue;

                    float distance = Vector3.Distance(pickup.transform.position, ring.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestRing = ring;
                    }
                }

                if (closestRing != null)
                {
                    float alpha = Mathf.Clamp01(1f - (closestDistance / 40f));
                    Handles.color = new Color(1f, 1f, 1f, alpha);
                    Handles.DrawLine(pickup.transform.position, closestRing.transform.position);
                }
            }
        }
    }
}
