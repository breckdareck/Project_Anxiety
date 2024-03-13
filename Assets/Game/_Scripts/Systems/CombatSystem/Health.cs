using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Project_Anxiety.Game.Interfaces;
using Project_Anxiety.Game.Managers;
using Project_Anxiety.Game.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEditor;
using Random = UnityEngine.Random;

namespace Project_Anxiety.Game.Units
{
    public sealed class Health : MonoBehaviour, IDamageable
    {
        private CharacterBase _characterBase;

        [SerializeField] private DamageText _damageText;

        [ShowInInspector, SerializeField, TabGroup("HP", TextColor = "red", Order = 0)]
        private int _currentHp = 50;
        public int CurrentHp
        {
            get { return _currentHp;}
            private set
            {
                _currentHp = value;
                if (_currentHp <= 0)
                {
                    IsDead = true;
                    _currentHp = 0;
                }
                else
                {
                    if (IsDead == false) return;
                    IsDead = false;
                }
            } 
        }
        [field:SerializeField, TabGroup("HP")] public int MaxHp { get; private set; } = 50;
        [field: SerializeField, TabGroup("MP", TextColor = "blue", Order = 1)] public int CurrentMp { get; private set; }
        [field: SerializeField, TabGroup("MP")] public int MaxMp { get; private set; } = 5;
    
        [field: SerializeField, TabGroup("Defense", TextColor = "white")] public int PhysicalDefense { get; private set; } = 5;
        [field: SerializeField, TabGroup("Defense")] public int MagicDefense { get; private set; } = 5;
    
        [field: SerializeField, TabGroup("Defense")] public int BonusPhysicalDefense { get; private set; }
        [field: SerializeField, TabGroup("Defense")] public int BonusMagicDefense { get; private set; }

        [ShowInInspector, TabGroup("Defense")] public int TotalPhysicalDefense => PhysicalDefense + BonusPhysicalDefense;
        [ShowInInspector, TabGroup("Defense")] public int TotalMagicDefense => MagicDefense + BonusMagicDefense;

        private bool _canBeDamaged = true;

        [ShowInInspector]
        public bool CanBeDamaged
        {
            get
            {
                return _canBeDamaged;
            }
            private set
            {
                _canBeDamaged = value;
            }
        }
        
        private bool _isDead = false;

        [ShowInInspector]
        public bool IsDead
        {
            get
            {
                return _isDead;
            }
            private set
            {
                _isDead = value;
                CanBeDamaged = !value;
                _characterBase.Animator.SetBool(AnimationStrings.isDead, value);
                
                if (_characterBase is Player p)
                {
                    if (value)
                    {
                        PlayerEventManager.Instance.InvokePlayerIsDead();
                    }
                }

                if (_characterBase is Enemy enemy)
                {
                    // Disable all components except the Health Component
                    foreach (var component in gameObject.GetComponentsInChildren<MonoBehaviour>())
                    {
                        if (component != this)
                            component.enabled = !value;
                    }

                    if (!value || _damagedBy == null) return;
                    foreach (var character in _damagedBy)
                    {
                        if (character is not Player player) continue;
                        int expToGive = enemy.EnemyData.ExpToGiveOnDeath;
                        if (player != null) player.JobSystem.CurrentJob.AddExp(expToGive);
                        StartCoroutine(SetInactiveOnDead());
                    }
                }
            }
        }
        
        [ShowInInspector, ReadOnly] private List<CharacterBase> _damagedBy = new();
        
        private void Awake()
        {
            if(_characterBase == null) _characterBase = GetComponent<CharacterBase>();
            CurrentHp = MaxHp;
            CurrentMp = MaxMp;
        }
    
        private void OnEnable()
        {
            if (_characterBase.CompareTag("Player"))
            {
                PlayerEventManager.Instance.OnPlayerLevelChangedEvent += OnLevelChanged;
                EventLogger.Instance.LoggedEventActions.Add(new EventLogInfo(this,"OnPlayerLevelChangedEvent","SetHealthOnPlayerLevel"));
                PlayerEventManager.Instance.OnPlayerIsDeadEvent += OnPlayerIsDead;
                EventLogger.Instance.LoggedEventActions.Add(new EventLogInfo(this,"OnPlayerIsDeadEvent","OnPlayerIsDead"));
            }
        }

        private void OnDisable()
        {
            if (_characterBase.CompareTag("Player"))
            {
                PlayerEventManager.Instance.OnPlayerLevelChangedEvent -= OnLevelChanged;
                EventLogger.Instance.LoggedEventActions.Remove(new EventLogInfo() {Component = this, EventName = "OnPlayerLevelChangedEvent", FunctionName = "SetHealthOnPlayerLevel" });
                PlayerEventManager.Instance.OnPlayerIsDeadEvent -= OnPlayerIsDead;
                EventLogger.Instance.LoggedEventActions.Remove(new EventLogInfo(this,"OnPlayerIsDeadEvent","OnPlayerIsDead"));
            }
            
        }
        
        private void OnPlayerIsDead()
        {
            if (_characterBase.CompareTag("Player"))
            {
                Debug.Log($"Player: {((Player)_characterBase).PlayerName} is Dead");
            }
        }

        private void OnLevelChanged()
        {
            MaxHp = Mathf.FloorToInt((MaxHp * 1.024f) + 13);
            MaxMp = Mathf.FloorToInt((MaxMp * 1.018f) + 11);
        
            CurrentHp = MaxHp;
            CurrentMp = MaxMp;
        }

        public void ModifyMana(int amount)
        {
            CurrentMp += amount;
        }
    
        public void TakeDamage(List<KeyValuePair<int, bool>> hits, CharacterBase fromCharacterBase)
        {
            // TODO - Calculate the actual damage to be taken based off of Defense Stats.
            if(!_canBeDamaged || IsDead) return;
            _characterBase.Animator.SetTrigger(AnimationStrings.hitTrigger);
            if(_characterBase is Enemy) _damagedBy.Add(fromCharacterBase);
            Vector3 currentPos = transform.position;

            var lookup = hits.ToLookup(kvp => kvp.Key, kvp => kvp.Value);
            
            for (int i = 0; i < hits.Count; i++)
            {
                Debug.Log(this.gameObject.name + " took " + hits[i].Key + " damage.");
                if (CurrentHp > 0)
                {
                    CurrentHp -= hits[i].Key;
                }
                if(_characterBase is Player) StartCoroutine(StartIframes());
                StartCoroutine(StartDamageText(i, hits[i].Key, hits[i].Value, currentPos));
            }
        }
    
        private IEnumerator StartDamageText(int i, int damage, bool isCrit, Vector3 currentPos)
        {
            yield return new WaitForSeconds(i*.1f);
            var top = transform.GetComponent<Collider2D>().bounds.size.y;
            float randomXOffset = Random.Range(-.12f, .12f);
        
            var tmpdmgTextGo = Instantiate(_damageText, currentPos + new Vector3(randomXOffset,i * _damageText.spacingScale + top,-i), Quaternion.identity);
            tmpdmgTextGo.SetDamageText(damage.ToString(), isCrit);
        }
        
        IEnumerator StartIframes()
        {
            _canBeDamaged = false;
        
            yield return new WaitForSeconds(1f);

            _canBeDamaged = true;
        }

        [Button]
        public void Revive()
        {
            CurrentHp = MaxHp;
            CurrentMp = MaxMp;

            IsDead = false;
            
            _damagedBy = new();
        }

        private IEnumerator SetInactiveOnDead()
        {
            yield return new WaitForSeconds(3);
            if (_characterBase is Enemy enemy)
            {
                enemy.assignedObjectPool.ReturnObjectToPool(enemy);
            }
            gameObject.SetActive(false);
        }
    
    }
}
