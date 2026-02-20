using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserFreakingVision : MonoBehaviour
{
    private LineRenderer _line1;
    private LineRenderer _line2;

    [SerializeField] ParticleSystem laserHitEffectPrefab;
    [SerializeField] float laserDamage = 0.1f;
    [SerializeField] AudioClip laserSFX;
    
    // Assign whichever camera is rendering your scene (usually Main Camera)
    public Camera playerCamera;
    private AudioSource _laserAudioSource;
    void Start()
    {
        _line1 = transform.Find("Line1").GetComponent<LineRenderer>();
        _line2 = transform.Find("Line2").GetComponent<LineRenderer>();

        _line1.positionCount = 2;
        _line2.positionCount = 2;

        _line1.enabled = false;
        _line2.enabled = false;
        
        _laserAudioSource = gameObject.AddComponent<AudioSource>();
        _laserAudioSource.clip = laserSFX;
        _laserAudioSource.loop = true;
        _laserAudioSource.playOnAwake = false;
    }

    void Update()
    {
        if (!GameManager.Instance.playerPaused)
        {
            HandleLaser();
        }
        else
        {
            DisableLaser();
        }

    }

    private void HandleLaser()
    {
        if (Input.GetMouseButtonDown(0))
        {
            EnableLaser();
        }

        if (Input.GetMouseButtonUp(0))
        {
            DisableLaser();
        }


        if (_line1.enabled && _line2.enabled)
        {
            ShootLaserAtMouse();

        }
    }

    private void ShootLaserAtMouse()
    {
        Vector3 mouseWorldPosition = GetMouseWorldPosition();

        _line1.SetPosition(0, _line1.transform.InverseTransformPoint(transform.position));
        _line1.SetPosition(1, _line1.transform.InverseTransformPoint(mouseWorldPosition));

        _line2.SetPosition(0, _line2.transform.InverseTransformPoint(transform.position));
        _line2.SetPosition(1, _line2.transform.InverseTransformPoint(mouseWorldPosition));

        //TODO: ADD SFX HERE


        if (laserHitEffectPrefab)
        {
            // spawn in an instance of the laser hit effect prefab at the mouse world position, so that its normal against the surface it hit
            ParticleSystem temp = Instantiate(laserHitEffectPrefab, mouseWorldPosition, Quaternion.LookRotation(playerCamera.transform.forward));
            Destroy(temp.gameObject, 0.35f);
        }
    }

    private void DisableLaser()
    {
        _line1.enabled = false;
        _line2.enabled = false;
        // cancel the laser sound
        _laserAudioSource.Stop();
    }

    private void EnableLaser()
    {
        _line1.enabled = true;
        _line2.enabled = true;
        _laserAudioSource.Play();
    }

    private Vector3 GetMouseWorldPosition()
    {
        // Cast a ray from the camera through the mouse position into the scene
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

        // If the ray hits something, use that hit point
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green);
            // check if the thing we hit is a subclass of DestructibleObject
            if (hit.collider.GetComponent<DestructibleObject>() != null)
            {
                hit.collider.GetComponent<DestructibleObject>().TakeDamage(laserDamage);
            }
            return hit.point;
        }
        // draw a debug ray for visualization
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);

        // If nothing is hit, project the laser out a long distance
        return ray.GetPoint(100f);
    }
}