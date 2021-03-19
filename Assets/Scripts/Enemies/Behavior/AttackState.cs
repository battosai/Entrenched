using System;
using UnityEngine;

public class AttackState : BaseState
{
    public AttackState(Enemy e) : base(e) {}
    
    public override Type Tick()
    {
        if(me.isDead)
            return null;

        if(!me.isAttacking)
        {
            if(me.InRange())
            {
                me.rb.velocity = Vector3.zero;
                me.isAttacking = true;

                // NOTE: Originally would've had different state for this,
                // no longer in the plan so just use the same state. Animators
                // will have a state that isn't used, just FYI.
                // if(me.isMelee)
                me.anim.SetTrigger("Attack");
                // else
                //     me.anim.SetTrigger("Shoot");
            }
            else
            {
                return typeof(ChaseState);
            }
        }
        return null;
    }
}