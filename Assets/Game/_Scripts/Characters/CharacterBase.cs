using UnityEngine;
using Project_Anxiety;

namespace Project_Anxiety.Game.Units
{
    public abstract class CharacterBase : MonoBehaviour
    {
        private Animator _animator;
        private Rigidbody2D _rb;
    
        public Animator Animator => _animator;
        public Rigidbody2D RB => _rb;

        public virtual void Awake()
        {
            _animator = GetComponent<Animator>();
            _rb = GetComponent<Rigidbody2D>();
        }
    }
}
