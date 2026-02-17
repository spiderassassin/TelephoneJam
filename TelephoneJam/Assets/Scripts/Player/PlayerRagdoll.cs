using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRagdoll : MonoBehaviour
{
    
    public static PlayerRagdoll Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private GameObject _player;
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        // Disable all colliders and rigidbodies in the player hierarchy
        foreach (Collider col in _player.GetComponentsInChildren<Collider>())
        {
            // skip root collider
            if (col.gameObject == _player) continue;
            col.enabled = false;
        }
        foreach (Rigidbody rb in _player.GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = true;
        }
    }
    
    public void ActivateRagdoll()
    {
        foreach (Collider col in _player.GetComponentsInChildren<Collider>())
        {
            if (col.gameObject == _player) continue;
            col.enabled = true;
        }
        foreach (Rigidbody rb in _player.GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = false;
        }

        var controller = PlayerController.Instance;

        // Disable flight & input
        controller.enabled = false;

        // Disable the CharacterController so it doesn't fight the ragdoll physics
        controller.cc.enabled = false;

        // Disable animator so it doesn't override ragdoll bone positions
        controller.animator.enabled = false;
        
        // provide a forece to the ragdoll based on the player's current velocity, so it has some momentum when it first activates
        Vector3 forwardVelocity = controller.transform.forward * controller.GetVelocity();
        foreach (Rigidbody rb in _player.GetComponentsInChildren<Rigidbody>())
        {
            rb.AddForce(forwardVelocity, ForceMode.VelocityChange);
        }
        
    }


}
