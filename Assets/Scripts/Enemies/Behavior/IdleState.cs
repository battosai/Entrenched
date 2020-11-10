using System;
using UnityEngine;

//Before being aggroed
public class IdleState : BaseState
{
    public IdleState(Enemy e) : base(e) {}

    public override Type Tick()
    {
        if(me.isDead)
            return null;

        if(IsAlerted())
        {
            return typeof(AlertState);
        }
        return null;
    }

    private bool IsAlerted()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            me.transform.position,
            Vector2.left,
            me.aggroRange,
            mask);
        
        return hit.collider != null;
    }
}