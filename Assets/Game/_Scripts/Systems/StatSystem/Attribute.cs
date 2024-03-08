using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project_Anxiety.Game.Units
{
    [Serializable]
    public sealed class Attribute
    {
        private Stats _statsAttachedTo;
        public AttributeType AttributeType { get; private set; }
        [ShowInInspector] public int BaseValue { get; private set; } = 4;
        [ShowInInspector] public int FlatBonusValue { get; private set; } = 0;
        [ShowInInspector] public float PercentBonusValue { get; private set; } = 1f;
        [ShowInInspector] public int CombinedBonusValue { get; private set; }
        [ShowInInspector] public int TotalValue { get; private set; }

        public Attribute(AttributeType attributeType, Stats statsAttachedTo)
        {
            AttributeType = attributeType;
            _statsAttachedTo = statsAttachedTo;
        }

        public int ModifyBaseValue(int amount)
        {
            return BaseValue += amount;
        }

        public int ModifyFlatBonusValue(int amount)
        {
            return FlatBonusValue += amount;
        }

        public float ModifyPercentBonusValue(float amount)
        {
            return PercentBonusValue += amount;
        }

        public void RecalculateValues()
        {
            CombinedBonusValue = CalculateCombinedValue();
            TotalValue = CalculateTotalValue();
        }
        
        private int CalculateCombinedValue()
        {
            if (PercentBonusValue > 1)
            {
                return Mathf.RoundToInt(FlatBonusValue + BaseValue * PercentBonusValue);
            }
            else
            {
                return FlatBonusValue;
            }
        }
    
        private int CalculateTotalValue()
        {
            return Mathf.RoundToInt(BaseValue + CombinedBonusValue);
        }

    }
}