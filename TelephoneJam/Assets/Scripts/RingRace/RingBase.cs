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
        [SerializeField] private RingType _ringType = RingType.Start;
        [SerializeField] private int raceID = 0; public int GetRaceID() { return raceID; }
        [SerializeField] private float RaceTimeLimit = 10f; public float GetRaceTimeLimit() { return RaceTimeLimit; }

        // Only show this section in the editor, if the ring type is a checkpoint, as it will be used to determine the order of checkpoints
        
        [SerializeField, Tooltip("ONLY SET THIS IF ITS A CHECKPOINT")] private int checkpointID = 0; public int GetCheckpointID() { return checkpointID; }
        
        private MeshCollider _collider;
        
        private void Start()
        {
            _collider = GetComponent<MeshCollider>();
            _collider.isTrigger = true;

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
        }

        private void HandleCheckpointRing()
        {
            RingRaceManager.Instance.CheckpointReached(raceID, checkpointID);
        }

        private void HandleFinishRing()
        {
            RingRaceManager.Instance.FinishedReached(this);
        }
                

        private void OnDrawGizmos()
        {
            GUIStyle style = new GUIStyle
            {
                normal = { textColor = Color.white },
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                fontSize = 24
            };

            // set the label text based on the ring type
            string label = _ringType.ToString();
            Handles.Label(transform.position, label, style);
        }
    }
}