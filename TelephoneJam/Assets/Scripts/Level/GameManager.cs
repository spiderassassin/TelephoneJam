using System.Collections;
using System.Collections.Generic;
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

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
