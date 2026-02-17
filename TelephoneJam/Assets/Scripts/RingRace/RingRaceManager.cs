using System.Collections;
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
        
        
        // Global things for each of the races / rings
        [SerializeField] AudioClip RaceStartSFX;
        [SerializeField] AudioClip CheckpointSFX;
        [SerializeField] AudioClip FinishWinSFX;
        [SerializeField] AudioClip FinishLoseSFX;
        
        private AudioSource audioSource;
        private GameObject _player;
        
        private int _currentRaceID = -1;
        


        private void Start()
        {
            // get the player
            _player = GameObject.FindGameObjectWithTag("Player");
            audioSource = _player.GetComponent<AudioSource>();
            
        }
        
        public void RequestStartRace(RingBase startRing)
        {
            
            // if we are already in a race, we dont want to start another
            if (_currentRaceID != -1)
            {
                Debug.LogWarning("Already in a race, cannot start another!");
                return;
            }

            // get access to all of the RingBase components in the scene (search inactive too), that having a matching raceID, and then sort them by their checkpointID
            // where the Start will have a checkpointID of 0, and the finish will have a checkpoint ID of -1.
            // so the end order of the rings will be Start (0), Checkpoint 1 (1), Checkpoint 2 (2), Checkpoint 3 (3), Finish (max value)
            RingBase[] rings = FindObjectsOfType<RingBase>(true);
            RingBase[] raceRings = System.Array.FindAll(rings, ring => ring.GetRaceID() == startRing.GetRaceID());
            
            // Sort the rings based on their checkpointID
            System.Array.Sort(raceRings, (a, b) => a.GetCheckpointID().CompareTo(b.GetCheckpointID()));

            
            startRing.gameObject.SetActive(false);
            raceRings[1].gameObject.SetActive(true);
            HandlePickupsForRace(startRing.GetRaceID(), true);
            
            // Start a timer for the race using the time limit from the start ring, and display it on the UI
            StartCoroutine(RaceTimer(startRing.GetRaceTimeLimit()));
            
            if (RaceStartSFX != null)
            {
                audioSource.PlayOneShot(RaceStartSFX);
            }
            _currentRaceID = startRing.GetRaceID();
            
            
        }

        private void HandlePickupsForRace(int raceID, bool enable)
        {
            // loop through the world, and find all of the HealthModifierScript that have a matching ringRaceID, and enable them
            HealthModifierScript[] pickups = FindObjectsOfType<HealthModifierScript>(true);
            foreach (HealthModifierScript pickup in pickups)
            {
                if (pickup.GetComponent<HealthModifierScript>().GetRingRaceID() == raceID)
                {
                    pickup.gameObject.SetActive(enable);
                }
            }
        }

        private IEnumerator RaceTimer(float timeLimit)
        {
            float timeRemaining = timeLimit;
            while (timeRemaining > 0)
            {
                // update the UI with the time remaining
                RingRaceUIManager.Instance.UpdateTimeRemaining(timeRemaining);
                yield return null;
                timeRemaining -= Time.deltaTime;
            }

            // if we reach here, that means the player has run out of time, so we need to end the
            EndRace(false);
        }

        private void EndRace(bool finished)
        {
            if (finished)
            {
                if (FinishWinSFX != null) { audioSource.PlayOneShot(FinishWinSFX); } 
                _player.GetComponent<PlayerStat>().HealHealth(1);
            }
            else
            {
                if (FinishLoseSFX != null) { audioSource.PlayOneShot(FinishLoseSFX); }
                // Damage the player here, and reset the Start ring
                _player.GetComponent<PlayerStat>().ReduceHealth(2);
                ResetRace(_currentRaceID);
            }
            HandlePickupsForRace(_currentRaceID, false);
            RingRaceUIManager.Instance.UpdateTimeRemaining(-1);
            _currentRaceID = -1;
        }

        private void ResetRace(int raceID)
        {
            // get access to all of the RingBase components in the scene (search inactive too), that have the same race id, and are of type Start
            RingBase[] rings = FindObjectsOfType<RingBase>(true);
            // get all rings with the same raceID
            RingBase[] raceRings = System.Array.FindAll(rings, ring => ring.GetRaceID() == raceID);
            foreach (RingBase ring in raceRings)
            {
                if (ring.GetCheckpointID() == int.MinValue)
                {
                    ring.gameObject.SetActive(true);
                }
                else
                {
                    ring.gameObject.SetActive(false);
                }
            }
            
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
                
                if (CheckpointSFX != null)
                {
                    audioSource.PlayOneShot(CheckpointSFX);
                }

        }
        
        public void FinishedReached(RingBase finishRing)
        {
            // stop the timer and end the race
            StopAllCoroutines();
            EndRace(true);
            
            finishRing.gameObject.SetActive(false);
        }
    }
}