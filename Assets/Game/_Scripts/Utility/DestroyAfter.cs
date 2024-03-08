using UnityEngine;

namespace Project_Anxiety.Game.Utility
{
    public class DestroyAfter : MonoBehaviour
    { 
        [SerializeField] private float timeToDestroy = 60;
        private void Awake()
        {
            Destroy(this.gameObject, timeToDestroy);
        }
    }
}
