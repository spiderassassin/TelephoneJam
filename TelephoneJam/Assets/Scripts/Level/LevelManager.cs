using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Current Stats")]
    public int racesFinished = 0;
    public int targetsDestroyed = 0;
    public bool conversationFinished = false;

    [Header("Win Conditions")]
    public int racesFinishedToWin = 1;
    public int targetsDestroyedToWin = 0;
    public bool conversationFinishedToWin = true;



    void Update()
    {
        //Just to make it work I added it here, if you want it to be more efficient just check whenever any condition changes.
        CheckWinningConditions();
        
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
