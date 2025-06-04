using System.Collections.Generic;
using UnityEngine;
using Action = System.Action;

namespace uMMORPG
{
    
    // Thread MANAGER
    // The ThreadManager will moves things to another thread to process, then bring them back to main thread.
    // Very useful for SQLite so you can send a save/whatever to another thread without blocking the main
    // Unity thread.
    
    public class ThreadManager : MonoBehaviour
    {
        public interface IThread
        {
            void QueueOnMainThread(Action action);
        }
    
        private static readonly NullThread _nullThread = new();
        private static ThreadDispatcher _thread;
    
        public static IThread MThread
        {
            get
            {
                if (_thread != null)
                {
                    return _thread as IThread;
                }
                return _nullThread as IThread;
            }
        }
    
        private void Awake()
        {
            _thread = new ThreadDispatcher();
        }
    
        private void OnDestroy()
        {
            _thread = null;
        }
    
        private void Update()
        {
            if (Application.isPlaying && _thread != null)
            {
                _thread.Update();
            }
        }
    
        private class NullThread : IThread
        {
            public void QueueOnMainThread(Action action)
            {
            }
        }
    
        private class ThreadDispatcher : IThread
        {
            private readonly List<Action> actions = new();
    
            public void QueueOnMainThread(Action action)
            {
                lock (actions)
                {
                    actions.Add(action);
                }
            }
    
            public void Update()
            {
                // Pop the actions from the synchronized list
                Action[] actionsToRun = null;
                lock (actions)
                {
                    actionsToRun = actions.ToArray();
                    actions.Clear();
                }
                // Run each action
                foreach (Action action in actionsToRun)
                {
                    action();
                }
            }
        }
    }
    
}
