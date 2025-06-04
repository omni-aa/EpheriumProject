using System;
using UnityEngine;
using UnityEngine.Events;

namespace uMMORPG
{
    [Serializable] public class UnityEventBool : UnityEvent<bool> { }
    //[Serializable] public class UnityEventInt : UnityEvent<int> { }
    //[Serializable] public class UnityEventString : UnityEvent<string> { }
    public class GameEventListener : MonoBehaviour
    {
        public GameEvent gameEvent;
        public UnityEvent onEventTriggered;
        public UnityEventBool onTriggeredBool;
        public UnityEventInt onTriggeredInt;
        public UnityEventString onTriggeredString;
    
        void OnEnable()
        {
            gameEvent.AddListener(this);
        }
        void OnDisable()
        {
            gameEvent.RemoveListener(this);
        }
        public void OnEventTriggered()
        {
            onEventTriggered.Invoke();
        }
    
        public void OnEventTriggeredBool(bool value)
        {
            onTriggeredBool.Invoke(value);
        }
        public void OnEventTriggeredInt(int value)
        {
            onTriggeredInt.Invoke(value);
        }
        public void OnEventTriggeredString(string value)
        {
            onTriggeredString.Invoke(value);
        }
    }
    
}
