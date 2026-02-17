using TMPro;
using UnityEngine;

namespace RingRace
{
    /// <summary>
    /// PLEASE NOTE: This is the font that was used in this
    /// https://www.dafont.com/oups.font?text=Time+Remaining%3A+00%3A00&back=theme
    /// </summary>
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
        private TMP_Text _timeRemainingShadowText;
        
        private void Start()
        {
            // find the child of this object with the name "TMP_TimeRemaining" and get the TMP_Text component from it, and store it in _timeRemainingText
            _timeRemainingText = transform.Find("TMP_TimeRemaining").GetComponent<TMP_Text>();
            _timeRemainingShadowText = transform.Find("TMP_TimeRemaining_Shadow").GetComponent<TMP_Text>();
            _timeRemainingText.text = "";
            _timeRemainingShadowText.text = "";
        }
        
        public void UpdateTimeRemaining(float timeRemaining)
        {
            if (timeRemaining == -1){ _timeRemainingText.text = ""; _timeRemainingShadowText.text = ""; return; }
            _timeRemainingText.text = $"Time Remaining: {timeRemaining:F2}s";
            _timeRemainingShadowText.text = $"Time Remaining: {timeRemaining:F2}s";
        }

    }
}
