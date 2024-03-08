using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Project_Anxiety;
using Project_Anxiety.Game.Managers;
using Project_Anxiety.Game.Utility;

namespace Project_Anxiety.Game.Units
{
    [RequireComponent(typeof(CharacterBase))]
    public sealed class Stats : MonoBehaviour
    {
        private CharacterBase _characterBase;
    

        [ShowInInspector, TabGroup("Tabs","Attri&Ability", TextColor = "purple")] public int AttributePoint { get; private set; }
        [ShowInInspector, TabGroup("Tabs","Attri&Ability")] public int AbilityPoint { get; private set; }
        [ShowInInspector, TabGroup("Tabs", "Attri&Ability")] public List<Attribute> Attributes { get; private set; }
        [ShowInInspector, TabGroup("Tabs","Movement", TextColor = "green")] public float BonusMoveSpeed { get; private set; } = 0;
        [ShowInInspector, TabGroup("Tabs","Movement")] public float BonusJumpImpulse { get; private set; } = 0;
        public void SetBonusMoveSpeed(float bonus) => BonusMoveSpeed += bonus;
        public void SetBonusJumpImpulse(float bonus) => BonusJumpImpulse += bonus;

        private void Awake()
        {
            if(_characterBase == null) _characterBase = GetComponent<CharacterBase>();
            if(Attributes == null) Attributes = new(new[]
            {
                new Attribute(AttributeType.Strength, this), new Attribute(AttributeType.Dexterity, this),
                new Attribute(AttributeType.Intelligence, this), new Attribute(AttributeType.Luck, this)
            });
        }

        private void OnEnable()
        {
            if (_characterBase.CompareTag("Player"))
            {
                PlayerEventManager.Instance.OnPlayerLevelChangedEvent += SetPointsOnPlayerLevel;
                EventLogger.Instance.LoggedEventActions.Add(new EventLogInfo(this,"OnPlayerLevelChangedEvent","SetPointsOnPlayerLevel"));
            }
            PlayerEventManager.Instance.OnPlayerAttributeValuesChangedEvent += CalculateAllTotalAttributeValues;
            EventLogger.Instance.LoggedEventActions.Add(new EventLogInfo(this,"OnPlayerAttributeValuesChangedEvent","CalculateAllTotalAttributeValues"));
        }

        private void OnDisable()
        {
            if (_characterBase.CompareTag("Player"))
            {
                PlayerEventManager.Instance.OnPlayerLevelChangedEvent -= SetPointsOnPlayerLevel;
                EventLogger.Instance.LoggedEventActions.Remove(new EventLogInfo() {Component = this, EventName = "OnPlayerLevelChangedEvent", FunctionName = "SetPointsOnPlayerLevel"});
            }
            PlayerEventManager.Instance.OnPlayerAttributeValuesChangedEvent -= CalculateAllTotalAttributeValues;
            EventLogger.Instance.LoggedEventActions.Remove(new EventLogInfo() {Component = this, EventName = "OnPlayerAttributeValuesChangedEvent", FunctionName = "CalculateAllTotalAttributeValues"});
        }

        private void Start()
        {
            PlayerEventManager.Instance.InvokePlayerAttributeValuesChanged();
        }
        
        private void SetPointsOnPlayerLevel()
        {
            foreach (var attribute in Attributes)
            {
                attribute.ModifyBaseValue(5);
            }
            AttributePoint+=5;
            AbilityPoint+=3;
            PlayerEventManager.Instance.InvokePlayerAttributeValuesChanged();
        }

        [Button, TabGroup("Tabs","Attri&Ability")]
        public void UseAttributePoint(AttributeType type, int amount)
        {
            if(AttributePoint <= 0 || AttributePoint - amount < 0) return;
            Attributes.Find(x => x.AttributeType == type).ModifyBaseValue(amount);
            AttributePoint-=amount;
            PlayerEventManager.Instance.InvokePlayerAttributeValuesChanged();
        }
    
        private void CalculateAllTotalAttributeValues()
        {
            Attributes.ForEach(x => x.RecalculateValues());
        }

    }
}