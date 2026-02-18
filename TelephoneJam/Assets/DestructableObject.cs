using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This will be a class that will parent alot of other classes, that we want to be able to destroy with laser vision
/// </summary>
public class DestructibleObject : MonoBehaviour
{
    [Header("Desctructible Object Settings")]
    [SerializeField] AudioClip destructionSFX;
    [SerializeField] ParticleSystem destructionEffectPrefab;
    [SerializeField] float destructionEffectDuration = 1f;
    [SerializeField] float health = 3f;
    
    
    
    
    public virtual void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        
        if (health <= 0)
        {
            Die();
        }
    }

    protected virtual void Die(bool shouldDestroy = true)
    {
        if(destructionSFX != null)
        {
            AudioSource.PlayClipAtPoint(destructionSFX, transform.position);
        }
        if(destructionEffectPrefab != null)
        {
            ParticleSystem effect = Instantiate(destructionEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect.gameObject, destructionEffectDuration);
        }
        
        if (shouldDestroy)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
        
    }
    
}
