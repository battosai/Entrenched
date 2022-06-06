using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

        #if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
            Dictionary<string, Button> controlToButtons = GameState.instance.ui.touchControlToButtons;
            foreach (KeyValuePair<string, Button> pair in controlToButtons)
            {
                string name = pair.Key;
                Button button = pair.Value;
                EventTrigger trigger = button.GetComponent<EventTrigger>();
                switch (name)
                {
                    case "Move":
                        EventTrigger.Entry moveDown = new EventTrigger.Entry();
                        EventTrigger.Entry moveUp = new EventTrigger.Entry();
                        moveDown.eventID = EventTriggerType.PointerDown;
                        moveUp.eventID = EventTriggerType.PointerUp;

                        moveDown.callback.AddListener((data) => 
                        {
                            OnMoveDown((PointerEventData)data);
                        });

                        moveUp.callback.AddListener((data) => 
                        {
                            OnMoveUp((PointerEventData)data);
                        });

                        trigger.triggers.Add(moveDown);
                        trigger.triggers.Add(moveUp);
                        break;

                    case "Crouch":
                        EventTrigger.Entry crouchDown = new EventTrigger.Entry();
                        EventTrigger.Entry crouchUp = new EventTrigger.Entry();
                        crouchDown.eventID = EventTriggerType.PointerDown;
                        crouchUp.eventID = EventTriggerType.PointerUp;

                        crouchDown.callback.AddListener((data) => 
                        {
                            OnCrouchDown((PointerEventData)data);
                        });

                        crouchUp.callback.AddListener((data) => 
                        {
                            OnCrouchUp((PointerEventData)data);
                        });

                        trigger.triggers.Add(crouchDown);
                        trigger.triggers.Add(crouchUp);
                        break;

                    case "Attack":
                        EventTrigger.Entry attackDown = new EventTrigger.Entry();
                        EventTrigger.Entry attackUp = new EventTrigger.Entry();
                        attackDown.eventID = EventTriggerType.PointerDown;
                        attackUp.eventID = EventTriggerType.PointerUp;

                        attackDown.callback.AddListener((data) => 
                        {
                            OnAttackDown((PointerEventData)data);
                        });

                        attackUp.callback.AddListener((data) => 
                        {
                            OnAttackUp((PointerEventData)data);
                        });

                        trigger.triggers.Add(attackDown);
                        trigger.triggers.Add(attackUp);
                        break;

                    case "Reload":
                        button.onClick.AddListener(OnReload); 
                        break;

                    case "SwitchWeapon":
                        button.onClick.AddListener(OnSwitchWeapon); 
                        break;

                    default:
                        break;
                } 
            }
        #endif
    }

    /// <summary>
    /// Reads input from the respective platform.
    /// </summary>
    public void Read()
    {
        #if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
            // TouchReader();
            ButtonReader();
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
    /// Touch button input.
    /// Most of the changes will be handled via listeners.
    /// Move = hold to move
    /// Crouch = hold to crouch
    /// Reload = tap
    /// Attack = tap
    /// Switch = tap
    /// </summary>
    private bool switchPressed;
    private bool reloadPressed;
    private bool attackPressed;
    private bool attackReleased;
    private void ButtonReader()
    {
        // _move will keep its value
        // _crouch will keep its value
        _switch = switchPressed;
        _reload = reloadPressed;
        _attackDown = attackPressed;
        // _attackHold will keep its value
        _attackRelease = attackReleased;

        switchPressed = reloadPressed = attackPressed = attackReleased = false;
    }

    /// <summary>
    /// Move button press down listener.
    /// </summary>
    private void OnMoveDown(PointerEventData data)
    {
        _move = true;
    }

    /// <summary>
    /// Move button release listener.
    /// </summary>
    private void OnMoveUp(PointerEventData data)
    {
        _move = false;
    }

    /// <summary>
    /// Crouch button press down listener.
    /// </summary>
    private void OnCrouchDown(PointerEventData data)
    {
        _crouch = true;
    }

    /// <summary>
    /// Crouch button release listener.
    /// </summary>
    private void OnCrouchUp(PointerEventData data)
    {
        _crouch = false;
    }

    /// <summary>
    /// Attack button press down listener.
    /// </summary>
    private void OnAttackDown(PointerEventData data)
    {
        _attackHold = true;
        attackPressed = true;
    }

    /// <summary>
    /// Attack button release listener.
    /// </summary>
    private void OnAttackUp(PointerEventData data)
    {
        _attackHold = false;
        attackReleased = true;
    }

    /// <summary>
    /// Reload button click listener.
    /// </summary>
    private void OnReload()
    {
        reloadPressed = true;
    }

    /// <summary>
    /// Switch Weapon button click listener.
    /// </summary>
    private void OnSwitchWeapon()
    {
        switchPressed = true;
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