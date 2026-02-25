using System;
using System.Collections;
using System.Collections.Generic;
using RingRace;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    // To get some better winning conditions and to add steps, another Dev could add a Scriptable Object with
    // each different set of conditions. And after every set of conditions is done, you could do GameManager.Instance.PlayNextSequence()
    // to trigger the next dialogue. Of course, this is just an idea, do whatever seems the most fun:)
    [Header("Current Stats")]
    public int racesFinished = 0;
    public int targetsDestroyed = 0;
    public bool conversationFinished = false;

    [Header("Win Conditions")]
    public int racesFinishedToWin = 1;
    public int targetsDestroyedToWin = 0;
    public bool conversationFinishedToWin = true;

    [Header("lizard finish")]
    [SerializeField] private GeckoCounterSO geckoCounter;
    [Tooltip("drag a hidden finish ring here because this was easy and it is 3am ")]
    [SerializeField] private RingBase finishRing;


    private bool _finishTriggered;

    private void OnEnable()
    {
        if (geckoCounter != null)
            geckoCounter.OnGoalReached += HandleGeckoGoalReached;
    }

    private void OnDisable()
    {
        if (geckoCounter != null)
            geckoCounter.OnGoalReached -= HandleGeckoGoalReached;
    }

    private void Start()
    {
        _finishTriggered = false;

        if (geckoCounter != null)
            geckoCounter.ResetCount();

        /*if (RingRaceManager.Instance != null)
            RingRaceManager.Instance.ResetAllRaces();
        else
            Debug.LogWarning("LevelManager: RingRaceManager.Instance is null at Start().");*/

        if (finishRing == null)
            Debug.LogWarning($"{name}: Finish ring is not assigned .....");
    }


    void Update()
    {
        //Just to make it work I added it here, if you want it to be more efficient just check whenever any condition changes.
        CheckWinningConditions();

    }

    private void HandleGeckoGoalReached()
    {
        if (_finishTriggered) return;
        _finishTriggered = true;

        if (finishRing == null)
        {
            Debug.LogError($"{name}: Gecko goal reached but finishRing is null. ~_~");
            return;
        }

        if (RingRaceManager.Instance == null)
        {
            Debug.LogError($"{name}: Gecko goal reached but RingRaceManager.Instance is null. whyy you make me suffer T_T ");
            return;
        }


        RingRaceManager.Instance.FinishedReached(finishRing);
    }

    public GameManager GetGameManager()
    {
        return GameManager.Instance;
    }

    public void UnpausePlayerControls()
    {
        GameManager.Instance.UnpausePlayerControls();
    }

    public void PausePlayerControls()
    {
        GameManager.Instance.PausePlayerControls();
    }

    public void PlayNextSequence()
    {
        GameManager.Instance.PlayNextSequence();
    }


    public void CheckWinningConditions()
    {
        if ((racesFinished >= racesFinishedToWin)
            && (targetsDestroyed >= targetsDestroyedToWin)
            && (conversationFinished == conversationFinishedToWin))
        {
            WinLevel();
        }

    }

    //You can call this if you want to instantly win the level.
    public void WinLevel()
    {
        GameManager.Instance.WinLevel();
    }

    public void FinishedCutscenes()
    {
        GameManager.Instance.FinishedCutscenes();
    }

}
