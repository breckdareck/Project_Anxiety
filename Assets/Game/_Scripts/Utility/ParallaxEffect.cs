using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project_Anxiety.Game.Utility
{
    public class ParallaxEffect : MonoBehaviour
    {
        public float parallaxEffectMultiplier;
        private float length, startPosition;
        private GameObject camera;

        void Start ()
        {
            startPosition = transform.position.x;
            length = GetComponent<SpriteRenderer>().bounds.size.x;
            camera = GameObject.Find("Main Camera"); 
        }

        void Update ()
        {
            float temp = (camera.transform.position.x * (1 - parallaxEffectMultiplier));
            float distance = (camera.transform.position.x * parallaxEffectMultiplier);

            transform.position = new Vector3(startPosition + distance, transform.position.y, transform.position.z);

            if (temp > startPosition + length/2) startPosition += length;
            else if (temp < startPosition - length/2) startPosition -= length;
        }
    }
}
