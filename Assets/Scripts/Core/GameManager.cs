using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TrixieCore.UI;
using Aspekt.PlayerController;

namespace TrixieCore
{
    public class GameManager : MonoBehaviour
    {
        public Camera MainCamera;
        public MenuControl MenuControl;
        public RespawnHandler RespawnHandler;
        public PathfinderHandler PathfinderHandler;

        public Player Player;

        [HideInInspector] public static GameManager Instance;

        private enum States
        {
            None, Playing, Paused, InMenu
        }
        private States state;

        #region LifeCycle
        private void Awake()
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
            
            state = States.Playing;
        }
        #endregion

        #region GameEvents
        public static void RespawnPlayerStart()
        {
            Instance.RespawnHandler.SetObjectToPoint(Instance.Player.transform);
        }
        #endregion
        
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

        public static void RestartGame()
        {
            RespawnPlayerStart();
            Instance.ToggleMenu();
        }

    }
}

