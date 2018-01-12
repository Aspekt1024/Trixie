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
		switch(state)
        {
            case States.None:
                break;
            case States.Playing:
                inputHandler.ProcessInput();
                break;
            case States.Paused:
                break;
            case States.InMenu:
                inputHandler.ProcessInput();
                break;
        }
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
            return Vector2.zero;
        }
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
                if (action.Method.Name == "ToggleMenu")
                {
                    action.Invoke();
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
    public void MeleePressed() { Player.Melee(); }
    public void InteractPressed() { }
    public void CycleItemsPressed() { }
    public void UseItemPressed() { }
    public void ShootPressed() { Player.Shoot(); }
    public void ShieldPressed() { Player.ShieldPressed(); }
    public void ShieldReleased() { Player.ShieldReleased(); }
    public void CycleShieldColourPressed() { Player.CycleShieldColourPressed(); }
    
#endregion

}
