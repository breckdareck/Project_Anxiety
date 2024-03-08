using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Project_Anxiety.Game.Managers
{
    public sealed class PlayerEventManager : MonoBehaviour
    {
        public static PlayerEventManager Instance;

        // Player Events
        public event Action OnPlayerAttributeValuesChangedEvent;
        public event Action OnPlayerLevelChangedEvent;
        public event Action OnPlayerExpChangedEvent;
        public event Action OnPlayerIsDeadEvent;

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
    
        public void InvokePlayerAttributeValuesChanged()
        {
            Debug.Log("Invoking OnAttributesChanged");
            OnPlayerAttributeValuesChangedEvent?.Invoke();
        }
    
        public void InvokePlayerLevelChanged()
        {
            Debug.Log("Invoking OnLevelChanged");
            OnPlayerLevelChangedEvent?.Invoke();
        }
    
        public void InvokePlayerExpChanged()
        {
            Debug.Log("Invoking OnExpChanged");
            OnPlayerExpChangedEvent?.Invoke();
        }
        
        public void InvokePlayerIsDead()
        {
            Debug.Log("Invoking OnPlayerIsDead");
            OnPlayerIsDeadEvent?.Invoke();
        }
    }
}

