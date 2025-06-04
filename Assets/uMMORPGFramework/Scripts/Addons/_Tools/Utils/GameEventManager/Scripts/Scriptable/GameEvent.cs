using System.Collections.Generic;
using UnityEngine;

namespace uMMORPG
{
    [CreateAssetMenu(menuName = "MMO-Indie/Tools/Game Event/New Game Event", order = 0)]
    public class GameEvent : ScriptableObject
    {
        private List<GameEventListener> listeners = new List<GameEventListener>();
        public void TriggerEvent()
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                listeners[i].OnEventTriggered();
            }
        }
        public void TriggerEventBool(bool value)
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                listeners[i].OnEventTriggeredBool(value);
            }
        }
    
        public void TriggerEventInt(int value)
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                listeners[i].OnEventTriggeredInt(value);
            }
        }
    
        public void TriggerEventString(string value)
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                listeners[i].OnEventTriggeredString(value);
            }
        }
        public void AddListener(GameEventListener listener)
        {
            listeners.Add(listener);
        }
        public void RemoveListener(GameEventListener listener)
        {
            listeners.Remove(listener);
        }
    }
    
}
