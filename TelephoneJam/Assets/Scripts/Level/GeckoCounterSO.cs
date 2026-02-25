using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Game/Gecko Counter", fileName = "GeckoCounter")]
public class GeckoCounterSO : ScriptableObject
{
    [SerializeField] public int _required = 5;
    [SerializeField] public int _collected = 0;

    // Fires exactly once when collected reaches required.
    public event System.Action OnGoalReached;

    private bool _goalFired;

    public int Required => _required;
    public int Collected => _collected;
    public bool HasMetGoal => _collected >= _required;

    public void ResetCount()
    {
        _collected = 0;
        _goalFired = false;
    }

    public void AddOne()
    {
        _collected++;

        if (!_goalFired && _collected >= _required)
        {
            _goalFired = true;
            OnGoalReached?.Invoke();
        }
    }


}
