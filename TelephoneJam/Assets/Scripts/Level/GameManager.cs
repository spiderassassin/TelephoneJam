using System;
using System.Collections;
using System.Collections.Generic;
using HisaGames.CutsceneManager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    // public GameObject levelManagerPrefab;
    // public LevelManager levelManager;

    // Intended for pausing the player controls.
    // (There might be some cases where you want to pause the movement AND the UI AND the game physics, but this is not inteded for that)
    [SerializeField] public bool playerPaused = false;

    [SerializeField] public int racesFinished = 0;
    // [SerializeField] public bool playerPaused = false;

    [SerializeField] public int currentLevel = 1;

    void Awake()
    {
        // Make Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    public void PausePlayerControls()
    {
        playerPaused = true;
    }
    public void UnpausePlayerControls()
    {
        playerPaused = false;
    }

    public void PlayNextSequence()
    {
        EcCutsceneManager.instance.PlayNextSequence();
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void NextLevel()
    {
        currentLevel++;
        Debug.Log("Load Level " + currentLevel);
        ChangeScene("Level " + currentLevel);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void WinLevel()
    {
        GameWinUI.Instance.Show();
    }
}
