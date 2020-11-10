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

                Attack();
                me.isAttacking = true;

                if(me.isMelee)
                    me.anim.SetTrigger("Attack");
                else
                    me.anim.SetTrigger("Shoot");
            }
            else
            {
                return typeof(ChaseState);
            }
        }
        return null;
    }

    private void Attack()
    {
        float range = me.isMelee ? me.meleeRange : me.rangedRange;

        RaycastHit2D hit = Physics2D.Raycast(
            me.transform.position, 
            Vector2.left,
            range,
            mask);
        
        if(hit.collider != null)
            Krieger.OnWounded?.Invoke();
    }
}