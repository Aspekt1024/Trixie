using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerInputHandler {

    // Can use system.enum.parse to serialize these
    // These key bindings are for windows
    // see http://wiki.unity3d.com/index.php?title=Xbox360Controller
    public string HORIZONTAL_AXIS = "Horizontal";
    public string SHOOT = "Fire1";
    public KeyCode JUMP_1 = KeyCode.Joystick1Button0;
    public KeyCode MELEE = KeyCode.Joystick1Button5;
    public KeyCode INTERACT = KeyCode.E;
    public KeyCode CYCLE_ITEMS = KeyCode.Z;
    public KeyCode USE_ITEM = KeyCode.X;
    //public KeyCode SHOOT = KeyCode.Joystick1Button2;
    public KeyCode SHIELD = KeyCode.Joystick1Button4;
    public KeyCode CYCLE_SHIELD_COLOUR = KeyCode.Joystick1Button3;

    public string AIM_AXIS_X = "AimX";
    public string AIM_AXIS_Y = "AimY";

    public KeyCode SHOW_HIDE_MENU = KeyCode.Joystick1Button7;

    private GameManager gameManager;

    private Vector2 aimDirection;

    public ControllerInputHandler(GameManager manager)
    {
        gameManager = manager;
        aimDirection = Vector2.right;
    }

    public void ProcessInput()
    {
        if (Input.GetKeyDown(JUMP_1)) gameManager.JumpPressed();
        if (Input.GetKeyUp(JUMP_1)) gameManager.JumpReleased();

        if (Input.GetAxis(HORIZONTAL_AXIS) > 0.5f)
        {
            gameManager.MoveRightPressed();
        }
        else if (Input.GetAxis(HORIZONTAL_AXIS) < -0.5f)
        {
            gameManager.MoveLeftPressed();
        }
        else
        {
            gameManager.MoveReleased();
        }

        if (Input.GetAxis(SHOOT) > 0.5f)
        {
            gameManager.ShootPressed();
        }

        if (Input.GetKeyDown(SHIELD)) gameManager.ShieldPressed();
        if (Input.GetKeyUp(SHIELD)) gameManager.ShieldReleased();

        if (Input.GetKey(MELEE)) gameManager.MeleePressed();
        if (Input.GetKey(INTERACT)) gameManager.InteractPressed();
        if (Input.GetKey(CYCLE_ITEMS)) gameManager.CycleItemsPressed();
        if (Input.GetKey(USE_ITEM)) gameManager.UseItemPressed();
        if (Input.GetKeyDown(CYCLE_SHIELD_COLOUR)) gameManager.CycleShieldColourPressed();


        if (Input.GetKeyDown(SHOW_HIDE_MENU)) gameManager.ToggleMenu();
    }

    public bool ReceivingInput()
    {
        return Input.GetKey(JUMP_1);
    }

    public Vector2 GetAimDirection()
    {
        float xVal = Input.GetAxis(AIM_AXIS_X);
        float yVal = Input.GetAxis(AIM_AXIS_Y);

        if (Mathf.Abs(xVal) + Mathf.Abs(yVal) > 0.5f)
        {
            aimDirection = new Vector2(xVal, -yVal);
        }
        aimDirection = new Vector2(xVal, -yVal);

        return aimDirection + (Vector2)Player.Instance.transform.position;
    }
}
