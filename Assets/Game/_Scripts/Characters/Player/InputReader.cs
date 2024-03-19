using System.Collections;
using System.Collections.Generic;
using Project_Anxiety.Game.Managers;
using Project_Anxiety.Game.Units;
using Project_Anxiety.Game.Utility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.DefaultInputActions;


namespace Project_Anxiety.Game.Units
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "Custom/InputReader")]
    public class InputReader : ScriptableObject, IPlayerActions
    {
        public event UnityAction<Vector2> Move = delegate { };
        public event UnityAction<bool> Run = delegate { };
        public event UnityAction<bool> Jump = delegate { };
        public event UnityAction Attack = delegate { };

        DefaultInputActions inputActions;

        public Vector3 Direction => inputActions.Player.Move.ReadValue<Vector2>();

        void OnEnable()
        {
            if (inputActions == null)
            {
                inputActions = new DefaultInputActions();
                inputActions.Player.SetCallbacks(this);
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            Move.Invoke(context.ReadValue<Vector2>());
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnFire(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

    }
}