using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuControl : MonoBehaviour {

    public RectTransform MainMenuPanel;
    public RectTransform ControlsPanel;

    private CanvasGroup mainCanvasGroup;

    //private Vector3 activePanelPosition;
    //private Vector3 inactivePanelPosition;

    private enum States
    {
        None, MainMenu, ControlsMenu, SequenceMenu
    }
    private States state;

    private void Awake()
    {
        mainCanvasGroup = GetComponent<CanvasGroup>();

        //activePanelPosition = new Vector3(Screen.width / 2f, Screen.height / 2f);
        //inactivePanelPosition = new Vector3(Screen.width * 2f, 0f, 0f);

        DisableMainMenu();
    }

    public void EnableMainMenu()
    {
        state = States.MainMenu;
        mainCanvasGroup.alpha = 1f;
        mainCanvasGroup.blocksRaycasts = true;
    }

    public void DisableMainMenu()
    {
        state = States.None;
        mainCanvasGroup.alpha = 0f;
        mainCanvasGroup.blocksRaycasts = false;
    }

    public void RestartButtonPressed()
    {
        GameManager.RestartGame();
    }

    public void ExitButtonPressed()
    {
        Application.Quit();
    }

    public bool MainMenuIsActive()
    {
        return state != States.None;
    }
}
