using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler {

    private enum InputMode
    {
        Keyboard, Controller
    }
    private InputMode mode;

    private GameManager gameManager;
    private KeyboardInputHandler keyboardInput;
    private ControllerInputHandler controllerInput;
    
    public InputHandler(GameManager manager)
    {
        gameManager = manager;
        keyboardInput = new KeyboardInputHandler(manager);
        controllerInput = new ControllerInputHandler(manager);
    }
    
    public void ProcessInput()
    {
        keyboardInput.ProcessInput();
        controllerInput.ProcessInput();
    }

    public Vector2 GetAimDirection()
    {
        return controllerInput.GetAimDirection();
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
    
}
