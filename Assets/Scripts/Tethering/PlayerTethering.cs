using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerTethering : MonoBehaviour
{
    [SerializeField] Rigidbody2D PlayerRB;
    [SerializeField]Tether LeftTether;
    bool LeftTetherTriggerPressed = false;
    [SerializeField]Tether RightTether;
    bool RightTetherTriggerPressed = false;
    PlayerTetherState currentState;
    [SerializeField]SOEvent JoinObjectsEnabled;// invoked when the two tethers are active, to enable linking them
    [SerializeField]SOEvent JoinObjectsDisabled;//invoked when a tethered is freed

    List<Tether> ExternalTethers;//active tethers between other objects
    [SerializeField]int ExternalTethersLimit = 2;//how many pair of objects can the player attach together

    private void Start()
    {
        LeftTether.AnchorStartTo(PlayerRB);
        RightTether.AnchorStartTo(PlayerRB);
        ChangeState(new InactiveTetherState(this));

    }

    private void ChangeState(PlayerTetherState newTetherState)
    {
        currentState?.Exit();
        currentState = newTetherState;
        currentState.Enter();
    }

    private void OnRightArmControl(UnityEngine.InputSystem.InputValue value)
    {
        RightTetherTriggerPressed = value.isPressed;

        currentState.HandleControlRight(value.isPressed);
    }
    private void OnLeftArmControl(InputValue value)
    {
        LeftTetherTriggerPressed = value.isPressed;
        currentState.HandleControlLeft(value.isPressed);
    }
    private void OnRightGrabToggle(InputValue value)
    {
        if (value.isPressed) currentState.HandleGrabRightToggle();
    }
    private void OnLeftGrabToggle(InputValue value)
    {
        if (value.isPressed) currentState.HandleGrabLeftToggle();
    }
    private void OnMoveTether(InputValue value)
    {
        var axis = value.Get<Vector2>();
        currentState.HandleStickInput(axis);
    }
    #region states
    protected abstract class PlayerTetherState
    {
        protected PlayerTethering Parent;
        internal PlayerTetherState(PlayerTethering parent)
        {
            Parent = parent;
        }
        internal abstract void Enter();
        internal abstract void Exit();
        internal abstract void HandleControlLeft(bool isButtonDown);
        internal abstract void HandleControlRight(bool isButtonDown);
        internal abstract void HandleStickInput(Vector2 value);
        internal virtual void HandleGrabLeftToggle()
        {
            Parent.LeftTether.ToggleGrab();
        }
        internal virtual void HandleGrabRightToggle()
        {
            Parent.RightTether.ToggleGrab();
        }
    }
    protected class LeftTetherActiveState : PlayerTetherState
    {
        internal LeftTetherActiveState(PlayerTethering parent) : base(parent)
        {
        }

        internal override void Enter()
        {
            Parent.LeftTether.EnterControlMode();
        }

        internal override void Exit()
        {
            Parent.LeftTether.ExitControlMode();
        }
        internal override void HandleControlLeft(bool isButtonDown)
        {
            if (!isButtonDown)
            {
                if (Parent.RightTetherTriggerPressed) Parent.ChangeState(new RightTetherActiveState(Parent));
                else Parent.ChangeState(new InactiveTetherState(Parent));
            }
            else
            {
                Debug.LogWarning("button press detected for left tether control while controlling left tether");
            }
        }

        internal override void HandleControlRight(bool isButtonDown)
        {
            //ignore
        }

        internal override void HandleStickInput(Vector2 value)
        {
            Parent.LeftTether.RecieveMoveInput(value);
        }
    }
    protected class RightTetherActiveState : PlayerTetherState
    {
        public RightTetherActiveState(PlayerTethering parent) : base(parent)
        {
        }

        internal override void Enter()
        {
            Parent.RightTether.EnterControlMode();
        }

        internal override void Exit()
        {
            Parent.RightTether.ExitControlMode();
        }

        
        internal override void HandleControlLeft(bool isButtonDown)
        {
            //ignore
        }

        internal override void HandleControlRight(bool isButtonDown)
        {
            if (!isButtonDown)
            {
                if (Parent.LeftTetherTriggerPressed) Parent.ChangeState(new LeftTetherActiveState(Parent));
                else Parent.ChangeState(new InactiveTetherState(Parent));
            }
            else{
                Debug.LogWarning("Button press for controlling right tether recieved while controlling right tether");
            }
        }

        internal override void HandleStickInput(Vector2 value)
        {
            Parent.RightTether.RecieveMoveInput(value);
        }
    }
    protected class InactiveTetherState : PlayerTetherState
    {
        public InactiveTetherState(PlayerTethering parent) : base(parent)
        {
        }

        internal override void Enter()
        {
            
        }

        internal override void Exit()
        {
            
        }

        

        internal override void HandleControlLeft(bool isButtonDown)
        {
            if (isButtonDown) Parent.ChangeState(new LeftTetherActiveState(Parent));
            else Debug.LogWarning("Button press detected in unexpectd state");
        }

        internal override void HandleControlRight(bool isButtonDown)
        {
            if (isButtonDown) Parent.ChangeState(new RightTetherActiveState(Parent));
            else Debug.LogWarning("Button press detected in unexpected state");
        }

        internal override void HandleStickInput(Vector2 value)
        {
            //probably fine to do nothing
        }
    }
    #endregion
}

public abstract class Tetherable : MonoBehaviour
{
    Tether currentTether;//should be limited to one, having two does open possibilities
}
