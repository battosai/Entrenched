using System;
using UnityEngine;

public abstract class BaseState
{
    protected Enemy me;

    protected int mask = LayerMask.GetMask("Player");

    public BaseState(Enemy e)
    {
        me = e;
    }

    public abstract Type Tick();
}