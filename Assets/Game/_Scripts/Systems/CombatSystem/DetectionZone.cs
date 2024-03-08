using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Project_Anxiety.Game.Units
{
    public sealed class DetectionZone : MonoBehaviour
    {
        public UnityEvent noCollidersRemain;
        
        public List<Collider2D> detectedColliders = new List<Collider2D>();
        private Collider2D col;

        private void Awake()
        {
            col = GetComponent<Collider2D>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            detectedColliders.Add(other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            detectedColliders.Remove(other);

            if (detectedColliders.Count <= 0)
            {
                noCollidersRemain.Invoke();
            }
        }
    }
}

