using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Project_Anxiety.Game.Managers
{
    public class HotBarManager : MonoBehaviour
    {
        public static HotBarManager Instance;

        [SerializeField] private PlayerInput _playerInput;
    
        public List<HotBarSlot> HotBarSlots = new();
      
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

        private void Start()
        {
            for (var index = 0; index < HotBarSlots.Count; index++)
            {
                var slot = HotBarSlots[index];
                slot.GetComponentInChildren<TMP_Text>().text = $"{_playerInput.actions[$"HotBarAction{index+1}"].controls[0].name.ToUpper()}";
            }
        }
    }
}

