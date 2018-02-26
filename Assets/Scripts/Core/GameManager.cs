using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TrixieInput;

public class GameManager : MonoBehaviour {
    
    public Camera MainCamera;
    public MenuControl MenuControl;
    public RespawnHandler RespawnHandler;

    public Player Player;

    [HideInInspector] public static GameManager Instance;

    private enum States
    {
        None, Playing, Paused, InMenu
    }
    private States state;

    private InputHandler inputHandler;

#region LifeCycle
    private void Awake ()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        inputHandler = new InputHandler(this);
        state = States.Playing;
	}
	
	private void Update ()
    {
        inputHandler.ProcessInput();
    }
#endregion

#region GameEvents
    public static Vector2 GetAimDirection()
    {
        if (Instance.state == States.Playing)
        {
            return Instance.inputHandler.GetAimDirection();
        }
        else
        {
            Debug.Log("if the game wasnt paused, look at this");
            return Vector2.zero;
        }
    }

    public static Vector2 GetMoveDirection()
    {
        if (Instance.state == States.Playing)
        {
            return Instance.inputHandler.GetMoveDirection();
        }
        return Vector2.zero;
    }

    public static void RespawnPlayerStart()
    {
        Instance.RespawnHandler.SetObjectToPoint(Instance.Player.transform);
    }
#endregion

#region PlayerInputs
    public void DirectInput(Action action)
    {
        switch (state)
        {
            case States.None:
                break;
            case States.Playing:
                action.Invoke();
                break;
            case States.Paused:
                break;
            case States.InMenu:
                switch (action.Method.Name)
                {
                    case "ToggleMenu":
                        action.Invoke();
                        break;
                    case "MoveReleased":
                        action.Invoke();
                        break;
                    case "ShieldReleased":
                        action.Invoke();
                        break;
                    case "ShootReleased":
                        action.Invoke();
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
    }
    
    public void ToggleMenu()
    {
        if (state == States.InMenu)
        {
            SetPlayMode();
            Time.timeScale = 1f;
            MenuControl.DisableMainMenu();
        }
        else
        {
            state = States.InMenu;
            Time.timeScale = 0f;
            MenuControl.EnableMainMenu();
        }
    }

    public void SetPlayMode()
    {
        state = States.Playing;
    }

    public void MoveLeftPressed() { Player.MoveLeft(); }
    public void MoveRightPressed() { Player.MoveRight(); }
    public void MoveReleased() { Player.MoveReleased(); }
    public void JumpPressed() { Player.Jump(); }
    public void JumpReleased() { Player.JumpReleased(); }
    public void MeleePressed() { Player.MeleePressed(); }
    public void MeleeReleased() { Player.MeleeReleased(); }
    public void InteractPressed() { Player.RangedAttackPressed(); }
    public void InteractReleased() { Player.RangedAttackReleased(); }
    public void CycleItemsPressed() { }
    public void UseItemPressed() { }
    public void ShootPressed() { Player.ShootPressed(); }
    public void ShootReleased() { Player.ShootReleased(); }
    public void ShieldPressed() { Player.ShieldPressed(); }
    public void ShieldReleased() { Player.ShieldReleased(); }
    public void CycleShieldColourPressed() { Player.CycleShieldColourPressed(); }
    
#endregion

    public static void RestartGame()
    {
        RespawnPlayerStart();
        Instance.ToggleMenu();
    }

}
