using System;
using UnityEngine;

public class ChaseState : BaseState
{
    public ChaseState(Enemy e) : base(e) {}

    public override Type Tick()
    {
        if (me.isDead == true)
        {
            return null;
        }

        if (me.InRange() == true)
        {
            return typeof(AttackState);
        }

        me.rb.linearVelocity = Vector2.left * me.moveSpeed;
        return null;
    }
}