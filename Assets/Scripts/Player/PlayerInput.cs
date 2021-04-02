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

    /// <summary>
    /// Touch input. Will only allow one touch input at a time on either side.
    /// Move = tap and/or hold left side
    /// Crouch = vertical swipe left side
    /// Reload = horizontal swipe left side
    /// Attack = tap and/or hold right side
    /// Switch = swipe right side
    /// </summary>
    private void TouchReader()
    {
        //_crouch will keep its value
        _move = false;
        _switch = false;
        _reload = false;
        _attackDown = false;
        _attackHold = false;
        _attackRelease = false;

        bool foundLeft = false;
        bool foundRight = false;
        float screenHorizontalMiddle = ((float)Screen.width)/2f;
        foreach(Touch touch in Input.touches)
        {
            if(foundLeft && foundRight)
                break;

            if(hasLeftTouch &&
                !foundLeft && 
                touch.fingerId == leftTouch.fingerId)
            {
                foundLeft = true;
                leftTouch = touch;
                continue;
            }

            if(hasRightTouch &&
                !foundRight &&
                touch.fingerId == rightTouch.fingerId)
            {
                foundRight = true;
                rightTouch = touch;
                continue;
            }

            //left
            if(touch.position.x < screenHorizontalMiddle)
            {
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
                if(touch.phase == TouchPhase.Began)
                {
                    hasRightTouch = true;
                    rightTouch = touch;
                    rightTouchStart = touch.position;
                }
            }
        }

        ProcessTouch(
            isLeft:true,
            ref hasLeftTouch,
            ref usedLeftSwipe,
            ref leftTouch,
            ref leftTouchStart);

        ProcessTouch(
            isLeft:false,
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
        bool isLeft,
        ref bool hasTouch,
        ref bool usedSwipe,
        ref Touch touch,
        ref Vector2 touchStart)
    {
        if(!hasTouch)
            return;

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
                bool isVertical;
                if(IsSwipe(out isVertical, touch, touchStart))
                {
                    if(!usedSwipe)
                    {
                        usedSwipe = true;
                        if(isLeft)
                        {
                            if(isVertical)
                                _crouch = touch.position.y < touchStart.y;
                            else
                                _reload = true;
                        }
                        else
                            _switch = true;
                    }
                }
                else
                {
                    if(isLeft)
                        _move = true;
                }
                break;

            case TouchPhase.Ended:
                if(!IsSwipe(touch, touchStart))
                    if(!isLeft)
                        _attackRelease = true;
                goto case TouchPhase.Canceled;

            case TouchPhase.Canceled:
                hasTouch = false;
                usedSwipe = false;
                break;
        }
    }

    /// <summary>
    /// Decides if touch performed a swipe.
    /// Output parameter to know if horizontal/vertical.
    /// </summary>
    private bool IsSwipe(
        out bool isVertical,
        Touch touch,
        Vector2 startPos)
    {
        float distanceThreshold = ((float)Screen.height)/4f;
        Vector2 diff = touch.position - startPos;
        isVertical = Mathf.Abs(diff.y) > Mathf.Abs(diff.x);
        return (diff.sqrMagnitude > Mathf.Pow(distanceThreshold, 2));
    }

    /// <summary>
    /// Decides if touch performed a swipe.
    /// </summary>
    private bool IsSwipe(
        Touch touch,
        Vector2 startPos)
    {
        float distanceThreshold = ((float)Screen.height)/6f;
        Vector2 diff = touch.position - startPos;
        return (diff.sqrMagnitude > Mathf.Pow(distanceThreshold, 2));
    }
}