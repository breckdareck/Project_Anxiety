using System;
using UnityEngine;
using Project_Anxiety.Game.Utility;
using UnityEditor;
using UnityEngine.Serialization;

namespace Project_Anxiety.Game.Units
{
    public class TouchingDirections : MonoBehaviour
    {
        private Rigidbody2D rb;
        private Animator animator;
        private Collider2D touchingCol;
        private RaycastHit2D[] groundHits = new RaycastHit2D[5];
        private RaycastHit2D[] platformHits = new RaycastHit2D[5];
        private RaycastHit2D[] wallHits = new RaycastHit2D[5];
        private RaycastHit2D[] ceilingHits = new RaycastHit2D[5];

        [FormerlySerializedAs("castFilter")] public ContactFilter2D groundCastFilter;
        public ContactFilter2D platformCastFilter;
        public ContactFilter2D wallCastFilter;
        
        public float groundDistance = 0.05f;
        public float wallDistance = 0.2f;
        public float ceilingDistance = 0.05f;

        private Vector2 wallCheckDirection => gameObject.transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        [SerializeField] private bool _isGrounded = true;
        public bool IsGrounded
        {
            get { return _isGrounded;}
            set
            {
                _isGrounded = value;
                animator.SetBool(AnimationStrings.isGrounded, value);
            }
        }
    
        [SerializeField] private bool _isOnPlatform = true;
        public bool IsOnPlatform
        {
            get { return _isOnPlatform;}
            set
            {
                _isOnPlatform = value;
            }
        }
        
        [SerializeField] private bool _isOnWall = true;
        public bool IsOnWall
        {
            get { return _isOnWall;}
            set
            {
                _isOnWall = value;
                animator.SetBool(AnimationStrings.isOnWall, value);
            }
        }

    
        [SerializeField] private bool _isOnCeiling = true;
        public bool IsOnCeiling
        {
            get { return _isOnCeiling;}
            set
            {
                _isOnCeiling = value;
                animator.SetBool(AnimationStrings.isOnCeiling, value);
            }
        }

    
        private void Awake()
        {
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();
            touchingCol = GetComponent<Collider2D>();
        }
        
        private void FixedUpdate()
        {
            IsGrounded = touchingCol.Cast(Vector2.down, groundCastFilter, groundHits, groundDistance) > 0;
            IsOnPlatform = touchingCol.Cast(Vector2.down, platformCastFilter, platformHits, groundDistance) > 0;
            IsOnWall = touchingCol.Cast(wallCheckDirection, wallCastFilter, wallHits, wallDistance) > 0;
            IsOnCeiling = touchingCol.Cast(Vector2.up, groundCastFilter, ceilingHits, ceilingDistance) > 0;
        }

        private void OnDrawGizmosSelected()
        {
            var position = transform.position;
            Gizmos.DrawLine(position, position + (Vector3)wallCheckDirection);
        }
    }
}
