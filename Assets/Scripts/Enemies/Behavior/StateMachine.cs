using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public Dictionary<Type, BaseState> states {get; private set;}
    public BaseState currentState {get; private set;}
    public event Action<BaseState> OnStateChanged;
    private bool pause;

    public void SetStates(Dictionary<Type, BaseState> states)
    {
        this.states = states;
    }

    public void Reset()
    {
        pause = false;
        currentState = states.Values.First();
    }

    private void Update()
    {
        if(pause)
            return;

        if(currentState == null)
            currentState = states.Values.First();
        // Debug.Log($"Current State: {currentState}");
        Type nextStateType = currentState?.Tick();
        if(nextStateType != null && nextStateType != currentState?.GetType())
            SwitchState(nextStateType);
    }

    public void Pause()
    {
        pause = true;
    }

    private void SwitchState(Type nextStateType)
    {
        currentState = states[nextStateType];
        OnStateChanged?.Invoke(currentState);
    }
}