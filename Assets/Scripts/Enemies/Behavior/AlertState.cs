using System;
using UnityEngine;

public class AlertState : BaseState
{
    public AlertState(Enemy e) : base(e)
    {
        e.stateMachine.OnStateChanged += Alert;
    }

    /// <summary>
    /// Cleanup.
    /// </summary>
    ~AlertState()
    {
        me.stateMachine.OnStateChanged -= Alert;
    }

    public override Type Tick()
    {
        if(me.isDead)
            return null;

        if(!me.isAlerted)
            return typeof(ChaseState);
        return null;
    }

    private void Alert(BaseState state)
    {
        if(state is AlertState)
        {
            me.isAlerted = true;
            me.anim.SetTrigger("Alert");
            AudioManager.PlayOneClip(
                me.audioSource, 
                me.alerts,
                Enemy.volumeScale);
        }
    }
}