using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using Project_Anxiety.Game.Interfaces;
using Project_Anxiety.Game.Managers;
using Project_Anxiety.Game.Utility;
using Unity.VisualScripting;
using Random = UnityEngine.Random;

namespace Project_Anxiety.Game.Units
{
    public sealed class PlayerCombatSystem : MonoBehaviour
    {
        private CharacterBase _characterBase;
        private Player Player => (Player)_characterBase;
        [SerializeField] private ContactFilter2D enemyFilter;
        [SerializeField] private BoxCollider2D _abilityHitBox;
    
        private int _cachedAttackDamage = 0;
        private bool _damageDirtyValue = true;
        [ShowInInspector, TabGroup("Attack", TextColor = "orange", Order = 2)] public int AttackDamage
        {
            get
            {
                if (_damageDirtyValue)
                {
                    _cachedAttackDamage = CalculateAttackDamage();
                    _damageDirtyValue = false;
                }

                return _cachedAttackDamage;
            } 
            
        }
        [ShowInInspector, TabGroup("Attack")] public float AttackSpeed { get; private set; } = 1;
        [ShowInInspector, TabGroup("Attack"), ReadOnly] public float BaseCritRate { get; private set; } = 0.05f;
        [ShowInInspector, TabGroup("Attack")] public float BonusCritRate { get; private set; }
        [ShowInInspector, TabGroup("Attack")] public float TotalCritRate => BaseCritRate + BonusCritRate;
        [ShowInInspector, TabGroup("Attack")] public float BonusCritDamage { get; private set; }
        [ShowInInspector, TabGroup("Attack")] public int MinDamage => (int)Mathf.Round(AttackDamage * .8f);
        [ShowInInspector, TabGroup("Attack")] public int MaxDamage => (int)Mathf.Round(AttackDamage * 1.2f);
        [ShowInInspector, TabGroup("Attack")] public int MinCritDamage => Mathf.RoundToInt((1.2f + BonusCritDamage) * MinDamage);
        [ShowInInspector, TabGroup("Attack")] public int MaxCritDamage => Mathf.RoundToInt((1.5f + BonusCritDamage) * MaxDamage);
    
        private void SetAttackDamageDirty() => _damageDirtyValue = true;

        private int CalculateAttackDamage()
        {
            try
            {
                Player player = _characterBase != null ? (Player)_characterBase : GetComponent<Player>();

                if (player != null || player.JobSystem.CurrentJob != null || player.JobSystem.CurrentJob?.JobData != null)
                {
                    int main = 0;
                    int secondary = 0;

                    main = player.Stats.Attributes.Find(x => x.AttributeType == player.JobSystem.CurrentJob.JobData.JobMainAttribute).TotalValue;
                    secondary = player.Stats.Attributes.Find(x => x.AttributeType == player.JobSystem.CurrentJob.JobData.JobSecondaryAttribute)
                        .TotalValue;

                    float weaponMultiplier = 1.2f;
                    int attValue = main * 4 + secondary;
                    int damage = Mathf.RoundToInt(weaponMultiplier * attValue);

                    return main > 0 ? damage : 0;
                }
            }
            catch
            {
                return 0;
            }

            return 0;
        }
        
    
        private void Awake()
        {
            _characterBase = GetComponent<CharacterBase>();
        }

        private void OnEnable()
        {
            PlayerEventManager.Instance.OnPlayerAttributeValuesChangedEvent += SetAttackDamageDirty;
            EventLogger.Instance.LoggedEventActions.Add(new EventLogInfo(this,"OnPlayerAttributeValuesChangedEvent","SetAttackDamageDirty"));
        }

        private void OnDisable()
        {
            PlayerEventManager.Instance.OnPlayerAttributeValuesChangedEvent -= SetAttackDamageDirty;
            EventLogger.Instance.LoggedEventActions.Remove(new EventLogInfo(this, "OnPlayerAttributeValuesChangedEvent", "SetAttackDamageDirty"));
        }

        private void Update()
        {
            // Update the cooldowns for the abilities associated with the current job.
            UpdateActiveAbilityCooldowns();
        }

        private bool CanUseActiveAbility(ActiveAbilityData abilityData)
        {
            return abilityData != null && abilityData.CanUse && ((Player)_characterBase).Health.CurrentMp >= abilityData.ManaCost && !((Player)_characterBase).Health.IsDead;
        }

        public void UseActiveAbility(ActiveAbilityData abilityData)
        {
            if (abilityData == null)
            {
                Debug.Log("No ability found");
                return;
            }
            
            if (CanUseActiveAbility(abilityData))
            {
                // Perform any other checks or logic related to using the ability.
                if (abilityData.CooldownTime > 0f)
                {
                    abilityData.CooldownTimeLeft = abilityData.CooldownTime;
                    abilityData.ModifyIsOnCooldown(true);
                }
                
                ((Player)_characterBase).Health.ModifyMana(-abilityData.ManaCost);
                if(abilityData.Buffs.Count > 0) ((Player)_characterBase).BuffSystem.ApplyAbilityBuffs(abilityData);
                if(abilityData.AbilityAnimation != null) Player.Animator.Play(abilityData.AbilityAnimation.name);
            }
            else
            {
                // The player cannot use the ability at the moment (e.g., on cooldown or not enough mana).
                Debug.Log("Cannot use the ability right now.");
            }
        }

        public void UsePassiveAbility(PassiveAbilityData abilityData)
        {
            if (abilityData == null)
            {
                Debug.Log("No ability found");
                return;
            }
            if(abilityData.Level <= 0) return;
            if(abilityData.Buffs.Count > 0) ((Player)_characterBase).BuffSystem.ApplyAbilityBuffs(abilityData);
        }
        
        private void UpdateActiveAbilityCooldowns()
        {
            foreach (var ability in ((Player)_characterBase).JobSystem.CurrentJob.JobData.JobAbilities)
            {
                if (ability is ActiveAbilityData abil && abil.CooldownTimeLeft > 0f)
                {
                    abil.CooldownTimeLeft -= Time.deltaTime;
                    if (!(abil.CooldownTimeLeft <= 0f)) continue;
                    abil.CooldownTimeLeft = 0f;
                    abil.ModifyIsOnCooldown(false);
                }
            }
        }
        
        private bool IsAttackCritHit()
        {
            int critCheck = Random.Range(0, 101);
            return  critCheck <= (int)(TotalCritRate * 100) && critCheck > 0;
        }
        
        
    
        private void Attack(ActiveAbilityData abilityData)
        {
            Debug.Log("Attack: " + abilityData.Name);
            
            switch (abilityData.AbilityAttackType)
            {
                case AbilityAttackType.Melee:
                    MeleeAttack(abilityData);
                    break;
                
                case AbilityAttackType.Ranged:
                    RangedAttack(abilityData);
                    break;

                case AbilityAttackType.Magic:
                    MagicAttack(abilityData);
                    break;
            }
        }
        
        private void MeleeAttack(ActiveAbilityData abilityData)
        {
            var results = ReturnHitResults(abilityData);
            results.ForEach(x => DamageHitObject(abilityData, x));
        }
        
        private void RangedAttack(ActiveAbilityData abilityData)
        {
            if (abilityData.ProjectileAsset == null) return;
            
            var results = ReturnHitResults(abilityData);

            if (results.Count == 0)
            {
                var projectile = CreateProjectile(abilityData);
                var localScale = transform.localScale;
                StartCoroutine(LerpProjectile(projectile, projectile.position + new Vector3(localScale.x * 7,0,0)));
                var rb = projectile.GetComponent<Rigidbody2D>();
                rb.velocityX = localScale.x * 2;
            }
            else
            {
                foreach (var result in results)
                {
                    var projectileTransform = CreateProjectile(abilityData);
                    StartCoroutine(LerpProjectile(projectileTransform, result.transform, abilityData, result));
                }
            }
        }
        
        private void MagicAttack(ActiveAbilityData abilityData)
        {
            var results = ReturnHitResults(abilityData);
            results.ForEach(x => DamageHitObject(abilityData, x));
        }

        private List<Collider2D> ReturnHitResults(ActiveAbilityData abilityData)
        {
            var results = new List<Collider2D>();
            _abilityHitBox.Overlap(enemyFilter, results);
            results.RemoveAll(x => x.GetComponent<Health>().CanBeDamaged == false);

            results = results.OrderBy(x => (int)Vector3.Distance(x.transform.position, this.transform.position))
                .ToList();
            
            if (results.Count > abilityData.TargetAmount)
            {
                results.RemoveRange(abilityData.TargetAmount, results.Count - abilityData.TargetAmount);
            }

            return results;
        }
        
        private Transform CreateProjectile(ActiveAbilityData abilityData)
        {
            var projectile = Instantiate(abilityData.ProjectileAsset, _abilityHitBox.transform.position, Quaternion.identity);
            return projectile.transform;
        }

        private IEnumerator LerpProjectile(Transform projectileTransform, Vector3 toPosition)
        {
            float timeElapsed = 0;
            float lerpDuration = .2f;
            Vector3 startingTransform = projectileTransform.position;
            
            while (timeElapsed < lerpDuration)
            {
                projectileTransform.position = Vector3.Lerp(startingTransform, toPosition, timeElapsed/lerpDuration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            projectileTransform.position = toPosition;
            
            Destroy(projectileTransform.gameObject, .2f);
        }

        private IEnumerator LerpProjectile(Transform projectileTransform, Transform toTransform, ActiveAbilityData abilityData, Collider2D hit)
        {
            float timeElapsed = 0;
            float lerpDuration = .2f;
            Vector3 startingTransform = projectileTransform.position;
            
            while (timeElapsed < lerpDuration)
            {
                projectileTransform.position = Vector3.Lerp(startingTransform, toTransform.position, timeElapsed/lerpDuration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            projectileTransform.position = toTransform.position;

            DamageHitObject(abilityData, hit);
            
            Destroy(projectileTransform.gameObject, .2f);
        }
        
        private void DamageHitObject(ActiveAbilityData abilityData, Collider2D hitCollider)
        {
            if (!hitCollider.TryGetComponent(out IDamageable damageable)) return;
            List<KeyValuePair<int, bool>>  hitsAmounts = new();
            for (int i = 0; i < abilityData.HitAmount; i++)
            {
                int minDamage = 0, maxDamage = 0;
                int damageToSend = 0;
                bool isCrit = IsAttackCritHit();

                if (isCrit)
                {
                    minDamage = Mathf.RoundToInt(MinCritDamage * abilityData.DamagePercent / 100f);
                    maxDamage = Mathf.RoundToInt(MaxCritDamage * abilityData.DamagePercent / 100f);
                }
                else
                {
                    minDamage = Mathf.RoundToInt(MinDamage * abilityData.DamagePercent / 100f);
                    maxDamage = Mathf.RoundToInt(MaxDamage * abilityData.DamagePercent / 100f);
                }

                damageToSend = Random.Range(minDamage, maxDamage);
                hitsAmounts.Add(new KeyValuePair<int, bool>(damageToSend, isCrit));
            }

            damageable.TakeDamage(hitsAmounts, _characterBase);
            //damageable.TakeDamage(minDamage, maxDamage, isCrit, ability.HitAmount, _characterBase);
        }
        
    
    }
}