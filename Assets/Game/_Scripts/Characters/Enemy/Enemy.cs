using System;
using System.Collections.Generic;
using System.Linq;
using Project_Anxiety.Game.Interfaces;
using UnityEngine;
using Project_Anxiety.Game.Utility;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Project_Anxiety.Game.Units
{
    public sealed class Enemy : CharacterBase
    {
        [FormerlySerializedAs("_enemyInfo")] [SerializeField] private EnemyData enemyData;
        public EnemyData EnemyData => enemyData;
        
        [SerializeField] private DetectionZone attackZone;
        [SerializeField] private DetectionZone cliffDetectionZone;
        private Rigidbody2D rb;
        private TouchingDirections touchingDirections;
        private Animator animator;

        public ObjectPool<Enemy> assignedObjectPool;

        public enum WalkableDirection{Right,Left}
        private WalkableDirection _walkDirection;
        private Vector2 walkDirectionVector = Vector2.right;
        public WalkableDirection WalkDirection
        {
            get { return _walkDirection; }
            set
            {
                if (_walkDirection != value)
                {
                    var localScale = transform.localScale;
                    localScale = new Vector2(localScale.x * -1,
                        localScale.y);
                    transform.localScale = localScale;
                    if (value == WalkableDirection.Right)
                    {
                        walkDirectionVector = Vector2.right;
                    }else if (value == WalkableDirection.Left)
                    {
                        walkDirectionVector = Vector2.left;
                    }
                }
                _walkDirection = value;
            }
        }


        public bool _hasTarget = false;
        public bool HasTarget
        {
            get { return _hasTarget;}
            private set
            {
                _hasTarget = value;
                animator.SetBool(AnimationStrings.hasTarget, value);
            }
        }
        
        public float AttackCooldown
        {
            get
            {
                return animator.GetFloat(AnimationStrings.attackCooldown);
            }
            set
            {
                animator.SetFloat(AnimationStrings.attackCooldown, MathF.Max(value, 0));
            }
        }

        public bool CanMove => animator.GetBool(AnimationStrings.canMove);


        public override void Awake()
        {
            base.Awake();
            rb = GetComponent<Rigidbody2D>();
            touchingDirections = GetComponent<TouchingDirections>();
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            HasTarget = attackZone.detectedColliders.Count(x => !x.GetComponent<Health>().IsDead) > 0;
            if (AttackCooldown > 0)
            {
                AttackCooldown -= Time.deltaTime;
            }
        }
        
        private void FixedUpdate()
        {
            if (touchingDirections.IsGrounded && touchingDirections.IsOnWall)
            {
                FlipDirections();
            }

            if (CanMove)
                rb.velocity = new Vector2(enemyData.walkSpeed * walkDirectionVector.x, rb.velocity.y);
            else
                rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, enemyData.walkStopRate), rb.velocity.y);
        }

        private void FlipDirections()
        {
            if (WalkDirection == WalkableDirection.Right)
            {
                WalkDirection = WalkableDirection.Left;
            }
            else if (WalkDirection == WalkableDirection.Left)
            {
                WalkDirection = WalkableDirection.Right;
            }
        }
        
        public void Attack()
        {
            foreach (var collider in attackZone.detectedColliders)
            {
                if (!collider.GetComponent<Health>().IsDead)
                {
                    var damage = Random.Range(1, 20);
                    var hit = new List<KeyValuePair<int, bool>>();
                    hit.Add(new KeyValuePair<int, bool>(damage, false));
                    collider.GetComponent<IDamageable>().TakeDamage(hit, this);
                }
            }
        }

        public void OnNoGroundDetected()
        {
            if (touchingDirections.IsGrounded)
            {
                FlipDirections();
            }
        }
        
    }
}
