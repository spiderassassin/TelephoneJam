using UnityEngine;

namespace RingRace
{
    /// <summary>
    /// Singleton to handle the race state, player progress, and overall game management for the game
    /// </summary>
    public class RingRaceManager : MonoBehaviour
    {
        public static RingRaceManager Instance { get; private set; }

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

        public void RequestStartRace(RingBase startRing)
        {
            // get access to all of the RingBase components in the scene (search inactive too), that having a matching raceID, and then sort them by their checkpointID
            // where the Start will have a checkpointID of 0, and the finish will have a checkpoint ID of -1.
            // so the end order of the rings will be Start (0), Checkpoint 1 (1), Checkpoint 2 (2), Checkpoint 3 (3), Finish (max value)
            RingBase[] rings = FindObjectsOfType<RingBase>(true);
            RingBase[] raceRings = System.Array.FindAll(rings, ring => ring.GetRaceID() == startRing.GetRaceID());
            
            // Sort the rings based on their checkpointID
            System.Array.Sort(raceRings, (a, b) => a.GetCheckpointID().CompareTo(b.GetCheckpointID()));

            
            startRing.gameObject.SetActive(false);
            raceRings[1].gameObject.SetActive(true);
        }


        public void CheckpointReached(int raceID, int checkpointID)
        {
            // get access to all of the RingBase components in the scene (search inactive too), that
            // find the ring with the matching raceID and checkpointI +1, and then enable it
                RingBase[] rings = FindObjectsOfType<RingBase>(true);
                RingBase[] raceRings = System.Array.FindAll(rings, ring => ring.GetRaceID() == raceID);
                RingBase nextRing = System.Array.Find(raceRings, ring => ring.GetCheckpointID() == checkpointID + 1);
                if (nextRing != null)
                {
                    nextRing.gameObject.SetActive(true);
                }
                else
                {
                    // if we dont find one, that means we are at the goal!
                    // find the goal with the matching race ID and checkpointID of int.MaxValue, and enable it
                    RingBase goalRing = System.Array.Find(raceRings, ring => ring.GetCheckpointID() == int.MaxValue);
                    if (goalRing != null)
                    {
                        goalRing.gameObject.SetActive(true);
                    }
                }
                
                // disable the current checkpoint
                RingBase currentRing = System.Array.Find(raceRings, ring => ring.GetCheckpointID() == checkpointID);
                if (currentRing != null)
                {
                    currentRing.gameObject.SetActive(false);
                }

        }
        
        public void FinishedReached(RingBase finishRing)
        {
            // get access to all of the RingBase components in the scene (search inactive too), that
            Debug.Log("Finished");
        }
    }
}