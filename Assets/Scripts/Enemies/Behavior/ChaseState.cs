using System;
using UnityEngine;

public class ChaseState : BaseState
{
    public ChaseState(Enemy e) : base(e) {}

    public override Type Tick()
    {
        if(me.isDead)
            return null;

        if(me.InRange())
            return typeof(AttackState);
        else
        {
            me.rb.velocity = Vector2.left * me.moveSpeed;
            return null;
        }
    }
}