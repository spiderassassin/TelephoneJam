using System;
using UnityEditor;
using UnityEngine;

namespace RingRace
{
    [Serializable]
    public enum RingType
    {
        Start,
        Checkpoint,
        Finish
    }

    public class RingBase : MonoBehaviour
    {
        [Header("Ring Settings")]
        [SerializeField] private RingType _ringType = RingType.Start; public RingType GetRingType() { return _ringType; }
        [SerializeField] private int raceID = 0; public int GetRaceID() { return raceID; }
        [SerializeField] private float RaceTimeLimit = 10f; public float GetRaceTimeLimit() { return RaceTimeLimit; }

        // Only show this section in the editor, if the ring type is a checkpoint, as it will be used to determine the order of checkpoints
        
        [SerializeField, Tooltip("ONLY SET THIS IF ITS A CHECKPOINT")] private int checkpointID = 0; public int GetCheckpointID() { return checkpointID; }
        
        private MeshCollider _collider;
        private GameObject _player;
        private AudioSource audioSource;

        [Header("Audio")]
        public AudioClip raceStartSFX;
        public float volume = 1f;

        private void Start()
        {
            _collider = GetComponent<MeshCollider>();
            _collider.isTrigger = true;
            
            _player = GameObject.FindGameObjectWithTag("Player");
            audioSource = _player.GetComponent<AudioSource>();

            if (_ringType == RingType.Checkpoint)
            {
                gameObject.SetActive(false);
            }

            if (_ringType == RingType.Finish)
            {
                gameObject.SetActive(false);
                checkpointID = int.MaxValue;
            }
            if (_ringType == RingType.Start)
            {
                gameObject.SetActive(true);
                checkpointID = int.MinValue;
            }
        }



        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            
            // Check the type of ring, as it will do different things
            switch (_ringType)
            {
                case RingType.Start:
                    HandleStartRing();
                    break;
                case RingType.Checkpoint:
                    HandleCheckpointRing();
                    break;
                case RingType.Finish:
                    HandleFinishRing();
                    break;
            }


        }

        private void HandleStartRing()
        {
            RingRaceManager.Instance.RequestStartRace(this);
            audioSource.PlayOneShot(raceStartSFX, volume);
        }

        private void HandleCheckpointRing()
        {
            RingRaceManager.Instance.CheckpointReached(raceID, checkpointID);
        }

        private void HandleFinishRing()
        {
            RingRaceManager.Instance.FinishedReached(this);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            MainCamera.Instance.SetFreeFlightMode(false);
            MainCamera.Instance.SetMouseLook(false);
        }
                

        /*private void OnDrawGizmos()
        {
            GUIStyle style = new GUIStyle
            {
                normal = { textColor = Color.white },
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                fontSize = 12
            };

            string label = "Type: " + _ringType.ToString() + "\nRaceId: " + raceID.ToString() +
                           (_ringType == RingType.Checkpoint ? ("\nCheckpointID: " + checkpointID.ToString()) : "");
            Handles.Label(transform.position, label, style);

            RingBase[] rings = FindObjectsOfType<RingBase>(true);
            RingBase nextRing = null;

            if (_ringType == RingType.Start)
            {
                nextRing = Array.Find(rings, ring => ring.GetRaceID() == raceID && ring.GetCheckpointID() == 1);
            }
            else if (_ringType == RingType.Checkpoint)
            {
                nextRing = Array.Find(rings,
                    ring => ring.GetRaceID() == raceID && ring.GetCheckpointID() == checkpointID + 1);
            }

            // Fallback to Finish ring if no next checkpoint found
            if (nextRing == null && _ringType != RingType.Finish)
            {
                nextRing = Array.Find(rings,
                    ring => ring.GetRaceID() == raceID && ring.GetRingType() == RingType.Finish);
            }

            // Draw the line
            if (nextRing != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, nextRing.transform.position);
            }
        }*/
        

        
    }
}