using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Project_Anxiety.Game.Utility
{
    public sealed class EventLogger : MonoBehaviour
    {
        public static EventLogger Instance;
    
        [ShowInInspector, ReadOnly] public List<EventLogInfo> LoggedEventActions = new();
    
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
        }
    }

    public struct EventLogInfo
    {
        public Component Component;
        public String EventName;
        public String FunctionName;

        public EventLogInfo(Component component, string eventName, string functionName)
        {
            Component = component;
            EventName = eventName;
            FunctionName = functionName;
        }
    }
}

