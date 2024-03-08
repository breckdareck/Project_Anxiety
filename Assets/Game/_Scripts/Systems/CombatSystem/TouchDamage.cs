using System;
using System.Collections;
using System.Collections.Generic;
using Project_Anxiety.Game.Units;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Project_Anxiety.Game
{
    public sealed class TouchDamage : MonoBehaviour
    {
        private Units.CharacterBase _characterBase;

        [SerializeField] private int minDamage;
        [SerializeField] private int maxDamage;
        [SerializeField] private int hitAmount;

        [SerializeField] private Vector2 knockBackVector;

        private void Awake()
        {
            _characterBase = GetComponentInParent<Units.CharacterBase>();
        }

        private IEnumerator OnTriggerEnter2D(Collider2D other)
        {
            if (!enabled) yield break;
            StartCoroutine(KnockBack(other));
            yield return new WaitForSeconds(1);

        }

        private IEnumerator OnTriggerStay2D(Collider2D other)
        {
            if (!enabled) yield break;
            StartCoroutine(KnockBack(other));
            yield return new WaitForSeconds(1);

        }

        private IEnumerator KnockBack(Collider2D other)
        {
            if (other.TryGetComponent(out Player player))
            {
                if (!player.Health.CanBeDamaged || player.Health.IsDead) {yield break;}
                player.KnockedBack = true;
                var damage = Random.Range(minDamage, maxDamage);
                var hit = new List<KeyValuePair<int, bool>>();
                hit.Add(new KeyValuePair<int, bool>(damage, false));
                player.Health.TakeDamage(hit, _characterBase);
                player.RB.AddRelativeForce(
                    player.transform.localScale.x > 0
                        ? new Vector2(-knockBackVector.x, knockBackVector.y)
                        : new Vector2(knockBackVector.x, knockBackVector.y), ForceMode2D.Impulse);
                yield return new WaitForSeconds(.3f);
                player.KnockedBack = false;
            }
        }
    }
}
