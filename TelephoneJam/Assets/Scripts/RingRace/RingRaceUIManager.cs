using TMPro;
using UnityEngine;

namespace RingRace
{
    public class RingRaceUIManager : MonoBehaviour
    {
        public static RingRaceUIManager Instance { get; private set; }
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
        
        
        // --
        private TMP_Text _timeRemainingText;
        
        private void Start()
        {
            // find the child of this object with the name "TMP_TimeRemaining" and get the TMP_Text component from it, and store it in _timeRemainingText
            _timeRemainingText = transform.Find("TMP_TimeRemaining").GetComponent<TMP_Text>();
            _timeRemainingText.text = "Time Remaining: 0.00s";
        }

    }
}
