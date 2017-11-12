using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler {

    // Can use system.enum.parse to serialize these
    public KeyCode MOVE_LEFT = KeyCode.A;
    public KeyCode MOVE_RIGHT = KeyCode.D;
    public KeyCode JUMP_1 = KeyCode.W;
    public KeyCode JUMP_2 = KeyCode.Space;
    public KeyCode MELEE = KeyCode.F;
    public KeyCode INTERACT = KeyCode.E;
    public KeyCode CYCLE_ITEMS = KeyCode.Z;
    public KeyCode USE_ITEM = KeyCode.X;
    public KeyCode SHOOT = KeyCode.Mouse0;
    public KeyCode SHIELD = KeyCode.Mouse1;

    public KeyCode SHOW_HIDE_MENU = KeyCode.Escape;

    private GameManager gameManager;
    
    public InputHandler(GameManager manager)
    {
        gameManager = manager;
    }
    
    public void ProcessInput()
    {
        if (Input.GetKeyDown(JUMP_1) || Input.GetKeyDown(JUMP_2)) gameManager.JumpPressed();
        if (Input.GetKeyUp(JUMP_1) || Input.GetKeyUp(JUMP_2)) gameManager.JumpReleased();

        if (Input.GetKeyDown(SHIELD)) gameManager.ShieldPressed();
        if (Input.GetKeyUp(SHIELD)) gameManager.ShieldReleased();

        if (Input.GetKeyDown(MOVE_LEFT)) gameManager.MoveLeftPressed();
        if (Input.GetKeyDown(MOVE_RIGHT)) gameManager.MoveRightPressed();
        if (Input.GetKeyUp(MOVE_RIGHT) || Input.GetKeyUp(MOVE_LEFT))
        {
            if (!Input.GetKey(MOVE_LEFT) && !Input.GetKey(MOVE_RIGHT))
            {
                gameManager.MoveReleased();
            }
        }
        
        if (Input.GetKey(MELEE)) gameManager.MeleePressed();
        if (Input.GetKey(INTERACT)) gameManager.InteractPressed();
        if (Input.GetKey(CYCLE_ITEMS)) gameManager.CycleItemsPressed();
        if (Input.GetKey(USE_ITEM)) gameManager.UseItemPressed();
        if (Input.GetKeyDown(SHOOT)) gameManager.ShootPressed();


        if (Input.GetKeyDown(SHOW_HIDE_MENU)) gameManager.ToggleMenu();
    }
    
}
