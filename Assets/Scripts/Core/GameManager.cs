using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

    #region PlayerInputs
    public void ToggleMenu()
    {
        if (state == States.InMenu)
        {
            state = States.Playing;
            MenuControl.DisableMainMenu();
        }
        else
        {
            state = States.InMenu;
            MenuControl.EnableMainMenu();
        }
    }
    public void SetPlayMode()
    {
        state = States.Playing;
    }

    public void MoveLeftPressed()
    {
        if (state == States.Playing)
        {
            Player.MoveLeft();
        }
    }
    public void MoveRightPressed()
    {
        if (state == States.Playing)
        {
            Player.MoveRight();
        }
    }
    public void MoveReleased()
    {
        if (state == States.Playing)
        {
            Player.MoveReleased();
        }
    }
    public void JumpPressed()
    {
        if (state == States.Playing)
        {
            Player.Jump();
        }
    }

    public void JumpReleased()
    {
        if (state == States.Playing)
        {
            Player.JumpReleased();
        }
    }
    public void MeleePressed()
    {

    }
    public void InteractPressed()
    {

    }
    public void CycleItemsPressed()
    {

    }
    public void UseItemPressed()
    {

    }
    public void ShootPressed()
    {
        if (state == States.Playing)
        {
            Player.Shoot();
        }
    }
    public void ShieldPressed()
    {
        if (state == States.Playing)
        {
            Player.ShieldPressed();
        }
    }
    public void ShieldReleased()
    {
        if (state == States.Playing)
        {
            Player.ShieldReleased();
        }
    }
    public void CycleShieldColourPressed()
    {
        if (state == States.Playing)
        {
            Player.CycleShieldColourPressed();
        }
    }
    
    #endregion


    public static Vector2 GetAimDirection()
    {
        return Instance.inputHandler.GetAimDirection();
    }

    public static void RespawnPlayerStart()
    {
        Instance.RespawnHandler.SetObjectToPoint(Instance.Player.transform);
    }
}
