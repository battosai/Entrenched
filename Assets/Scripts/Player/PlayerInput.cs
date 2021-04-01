using System;
using UnityEngine;

public class PlayerInput
{
    //functional input
    public bool _move {get; private set;}
    public bool _crouch {get; private set;}
    public bool _switch {get; private set;}
    public bool _reload {get; private set;}
    public bool _attackDown {get; private set;}
    public bool _attackHold {get; private set;}
    public bool _attackRelease {get; private set;}

    public PlayerInput()
    {
        hasLeftTouch = hasRightTouch = false;
    }

    /// <summary>
    /// Reads input from the respective platform.
    /// </summary>
    public void Read()
    {
        #if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
            TouchReader();
        #else
            KeyboardReader();
        #endif
    }

    /// <summary>
    /// Keyboard input.
    /// Move = D
    /// Crouch = S
    /// Reload = R
    /// Attack = J
    /// Switch = K
    /// </summary>
    private void KeyboardReader()
    {
        _move = Input.GetKey(KeyCode.D);
        _crouch = Input.GetKey(KeyCode.S);
        _switch = Input.GetKeyDown(KeyCode.K);
        _reload = Input.GetKeyDown(KeyCode.R);
        _attackDown = Input.GetKeyDown(KeyCode.J);
        _attackHold = Input.GetKey(KeyCode.J);
        _attackRelease = Input.GetKeyUp(KeyCode.J);
    }

    //TODO:
    //mobile input reading
    //NEED: do it
    /// <summary>
    /// Touch input. Will only allow one touch input at a time on either side.
    /// Move = tap and/or hold left side
    /// Crouch = swipe down + down left side
    /// Reload = swipe up left side
    /// Attack = tap and/or hold right side
    /// Switch = swipe right side
    /// </summary>
    private void TouchReader()
    {
        _move = false;
        _crouch = false;
        _switch = false;
        _reload = false;
        _attackDown = false;
        _attackHold = false;
        _attackRelease = false;

        float screenHorizontalMiddle = Screen.width/2f;
        foreach(Touch touch in Input.touches)
        {
            //left
            if(touch.position.x < screenHorizontalMiddle)
            {
                if(hasLeftTouch)
                    continue;

                if(touch.phase == TouchPhase.Began)
                {
                    hasLeftTouch = true;
                    leftTouch = touch;
                    leftTouchStart = touch.position;
                }
            }

            //right
            else
            {
                if(hasRightTouch)
                    continue;

                if(touch.phase == TouchPhase.Began)
                {
                    hasRightTouch = true;
                    rightTouch = touch;
                    rightTouchStart = touch.position;
                }
            }
        }

        ProcessTouch(
            ref hasLeftTouch,
            ref usedLeftSwipe,
            ref leftTouch,
            ref leftTouchStart);

        ProcessTouch(
            ref hasRightTouch,
            ref usedRightSwipe,
            ref rightTouch,
            ref rightTouchStart);
    }

    /// <summary>
    /// Helper function that maps touch to functional inputs.
    /// </summary>
    private bool hasLeftTouch;
    private bool usedLeftSwipe;
    private Touch leftTouch;
    private Vector2 leftTouchStart;
    private bool hasRightTouch;
    private bool usedRightSwipe;
    private Touch rightTouch;
    private Vector2 rightTouchStart;
    private void ProcessTouch(
        ref bool hasTouch,
        ref bool usedSwipe,
        ref Touch touch,
        ref Vector2 touchStart)
    {
        if(!hasTouch)
            return;

        bool isLeft = (touch.position == leftTouch.position);

        switch(touch.phase)
        {
            case TouchPhase.Began:
                touchStart = touch.position;
                if(!isLeft)
                    _attackDown = true;
                break;

            case TouchPhase.Stationary:
                touchStart = touch.position;
                usedSwipe = false;
                if(isLeft)
                    _move = true;
                else
                {
                    _attackHold = true;
                }
                break;

            case TouchPhase.Moved:
                if(!usedSwipe && IsSwipe(touch, touchStart))
                {
                    usedSwipe = true;
                    if(isLeft)
                    {
                        if(touch.position.y > touchStart.y)
                            _reload = true;
                        else
                            _crouch = true;
                    }
                    else
                        _switch = true;
                }
                break;

            case TouchPhase.Ended:
                if(!IsSwipe(touch, touchStart))
                {
                    if(!isLeft)
                        _attackRelease = true;
                }
                goto case TouchPhase.Canceled;

            case TouchPhase.Canceled:
                hasTouch = false;
                usedSwipe = false;
                break;
        }
    }

    /// <summary>
    /// Returns true if up, false if down.
    /// </summary>
    private bool IsSwipeUp(
        Touch touch,
        Vector2 startPos)
    {
        return touch.position.y > startPos.y;
    }

    /// <summary>
    /// Decides if touch performed a swipe.
    /// </summary>
    private bool IsSwipe(
        Touch touch,
        Vector2 startPos)
    {
        float distanceThreshold = 20f;
        Vector2 diff = touch.position - startPos;
        return (diff.sqrMagnitude < Mathf.Pow(distanceThreshold, 2));
    }
}