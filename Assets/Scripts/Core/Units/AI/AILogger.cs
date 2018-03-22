using UnityEngine;

namespace Aspekt.AI
{
    public static class AILogger
    {

        public static void CreateMessage(string message, AIAgent agent)
        {
            if (agent.LoggingEnabled)
            {
                Debug.Log(message);
            }
        }
    }

}