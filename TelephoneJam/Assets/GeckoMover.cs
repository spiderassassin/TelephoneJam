using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;



public class GeckoMover : MonoBehaviour
{
    [System.Serializable]
    public struct Segment
    {
        public Transform start;
        public Transform end;
    }

    [Header("Route")]
    [SerializeField] private Segment[] segments;
    [SerializeField] private int startSegmentIndex = 0;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 1.5f;
    private const float runSpeed = 17.08f; // it is part of the joke 2pie
    [SerializeField] private float arriveDistance = 0.15f;
    [SerializeField] private bool loop = true;


    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private float crossFade = 0.1f;
    [SerializeField] private bool runInsteadOfWalk = false;

    [Header("Collision")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool advanceOnPlayerCollision = true;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hitPlayerSfx;
    [SerializeField] private float hitVolume = 1f;


    private int _i;
    private bool _moving;

    private int _walkHash;
    private int _runHash;

    private void Reset()
    {
        animator = GetComponentInChildren<Animator>();
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void Awake()
    {
        if (!animator) animator = GetComponentInChildren<Animator>();
        _walkHash = Animator.StringToHash("Walk");
        _runHash = Animator.StringToHash("Run");

    }

    private void Start()
    {
        if (segments == null || segments.Length == 0)
        {
            Debug.LogError("No segments to fol.low please add them.... please... please...");
            enabled = false;
            return;
        }

        _i = Mathf.Clamp(startSegmentIndex, 0, segments.Length - 1);
        TeleportToSegmentStart(_i);
        StartMoving();
    }

    private void Update()
    {
        if (!_moving) return;

        var seg = segments[_i];
        if (seg.end == null)
        {
            Debug.LogError("edn segment is null... really>?? -_-");
            enabled = false;
            return;
        }

        float speed = runInsteadOfWalk ? runSpeed : walkSpeed;

        Vector3 target = seg.end.position;
        Vector3 pos = transform.position;

        //now lest move here

        transform.position = Vector3.MoveTowards(pos, target, speed * Time.deltaTime);

        Vector3 dir = target - pos;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.0001f)
        {
            Quaternion rot = Quaternion.LookRotation(dir.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, 12f * Time.deltaTime);
        }

        //we need to check si llegamos
        if (Vector3.Distance(transform.position, target) <= arriveDistance)
        {
            AdvanceSegment();
        }
    }

    private void StartMoving()
    {
        _moving = true;
        PlayLocomotion();
    }

    private void StopMoving()
    {
        _moving = false;

    }

    private void PlayLocomotion()
    {
        if (!animator) return;

        int hash = runInsteadOfWalk ? _runHash : _walkHash;
        if (animator.HasState(0, hash))
        {
            animator.CrossFadeInFixedTime(hash, crossFade);
        }

    }

    private void TeleportToSegmentStart(int index)
    {
        var seg = segments[index];
        if (seg.start == null)
        {
            Debug.LogError("no start... really>?? -_- one job dude...");
            enabled = false;
            return;
        }

        transform.position = seg.start.position;

        if (seg.end != null)
        {
            Vector3 dir = seg.end.position - seg.start.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.0001f)
                transform.rotation = Quaternion.LookRotation(dir.normalized, Vector3.up);

        }
    }

    private void AdvanceSegment()
    {
        StopMoving();

        int next = _i + 1;
        if (next >= segments.Length)
        {
            if (!loop)
            {
                return;
            }

            next = 0;
        }

        _i = next;
        TeleportToSegmentStart(_i);
        StartMoving();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!advanceOnPlayerCollision) return;
        if (!other.CompareTag(playerTag)) return;


        if (hitPlayerSfx != null)
        {
            AudioSource.PlayClipAtPoint(hitPlayerSfx, transform.position);
        }
        Debug.Log("colision con el jugador....");


        AdvanceSegment();
    }
}
