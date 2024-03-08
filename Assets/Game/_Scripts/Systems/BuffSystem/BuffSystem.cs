using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Project_Anxiety.Game.Managers;
using Project_Anxiety.Game.Units;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Project_Anxiety.Game.Units
{
    public class BuffSystem : MonoBehaviour
    {
        private CharacterBase _characterBase;
        
        [SerializeField] private GameObject _buffBar;
        [SerializeField] private GameObject _buffIconTemplate;
        private ConcurrentBag<GameObject> _buffIcons;
        
        [SerializeReference, ReadOnly] private List<Buff> _currentBuffs = new ();
        
        
        private void Awake()
        {
            _buffIcons = new ConcurrentBag<GameObject>();
            _characterBase = GetComponent<CharacterBase>();
        }

        private void Update()
        {
            RefreshBuffBar();
            TickBuffs(Time.deltaTime);
        }
        
        private void RefreshBuffBar()
        {
            if (_characterBase is not Player) return;
            if (_buffIcons.Count <= 0) return;
            foreach (GameObject iconGO in _buffIcons)
            {
                BuffIcon buffIcon = iconGO.GetComponent<BuffIcon>();
                if (buffIcon.Lifetime > 0)
                {
                    buffIcon.Lifetime -= Time.deltaTime;
                    buffIcon.LifetimeText.text = Mathf.RoundToInt(buffIcon.Lifetime).ToString();
                }
                else
                {
                    List<GameObject> buffIcons = _buffIcons.ToList();
                    buffIcons.Remove(iconGO);
                    _buffIcons = new ConcurrentBag<GameObject>(buffIcons);
                    Destroy(iconGO);
                }
            }
        }

        public void ApplyAbilityBuffs(AbilityData abilityData)
        {
            // Apply buffs from the ability.
            foreach (Buff buff in abilityData.Buffs)
            {
                // If the abilities buff can't stack and the buff is already active reset its timer
                if (!buff.CanStack && _currentBuffs.Exists(x => x.Equals(buff)))
                {
                    _currentBuffs.Find(x => x.Equals(buff)).ResetTimeElapsed();
                }
                else // Add the buff
                {
                    buff.ApplyBuff((Player)_characterBase); // Apply buff to the player.
                    _currentBuffs.Add((Buff)SerializationUtility.CreateCopy(buff)); // Add the buff to the list of active buffs.
                }

                if (buff.Expires && _characterBase is Player)
                {
                    CreateBuffIcon(buff, buff.CanStack);
                    PlayerEventManager.Instance.InvokePlayerAttributeValuesChanged();
                }
            }
        }
        
        public void CreateBuffIcon(Buff buff, bool canStack)
        {
            List<GameObject> buffIcons = _buffIcons.ToList();
        
            GameObject buffIconExists =
                buffIcons.Find(x => x.GetComponent<BuffIcon>().TempBuffNameText.text == buff.BuffName);
        
            if (buffIconExists != null && !canStack)
            {
                buffIcons.Remove(buffIconExists);
                _buffIcons = new ConcurrentBag<GameObject>(buffIcons);

                Destroy(buffIconExists);
            }
        
            GameObject buffIconGO = Instantiate(_buffIconTemplate, _buffBar.transform);
            BuffIcon buffIcon = buffIconGO.GetComponent<BuffIcon>();
            buffIcon.Lifetime = buff.Duration;
            buffIcon.TempBuffNameText.text = buff.BuffName;
            buffIcon.LifetimeText.text = ((int)buff.Duration).ToString();
            _buffIcons.Add(buffIconGO);
        }

        private void TickBuffs(float deltaTime)
        {
            if(_currentBuffs.Count <= 0) return;
            // Tick down the durations of all active buffs for the player.
            for (int i = _currentBuffs.Count - 1; i >= 0; i--)
            {
                Buff buff = _currentBuffs[i];
                buff.TickBuff(deltaTime);
            
                // Remove the buff if its duration is over.
                if (buff.IsExpired && _characterBase is Player player)
                {
                    buff.RemoveBuff(player); // Remove the buff effect from the player.
                    _currentBuffs.RemoveAt(i); // Remove the buff from the list of active buffs.
                    PlayerEventManager.Instance.InvokePlayerAttributeValuesChanged();
                }
            }
        }

    }
}


