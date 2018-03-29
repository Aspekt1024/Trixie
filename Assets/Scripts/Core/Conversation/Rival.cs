using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrixieCore.UI;

namespace TrixieCore.Units
{
    public class Rival : MonoBehaviour
    {
        public string ConversationText;

        private enum States
        {
            OutOfRange, InRange, ShowingText
        }
        private States state;

        private void Update()
        {
            switch (state)
            {
                case States.OutOfRange:
                    break;
                case States.InRange:
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        state = States.ShowingText;
                        GameUIManager.ShowConversation(ConversationText);
                    }
                    break;
                case States.ShowingText:
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        state = States.InRange;
                        GameUIManager.HideConversation();
                    }
                    break;
                default:
                    break;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                state = States.InRange;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                if (state == States.ShowingText)
                {
                    state = States.OutOfRange;
                    GameUIManager.HideConversation();
                }

                if (state == States.InRange)
                {
                    state = States.OutOfRange;
                }
            }
        }
    }
}
