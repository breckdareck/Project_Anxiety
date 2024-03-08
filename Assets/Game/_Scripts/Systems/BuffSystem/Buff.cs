using System;
using System.Collections;
using System.Collections.Generic;
using Project_Anxiety.Game.Units;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Project_Anxiety.Game.Units
{

    [Serializable]
    public abstract class Buff : IEquatable<Buff>
    {
        [SerializeField] protected string buffName;
        [SerializeField] protected float modifier;
        [SerializeField] protected float duration;
        [SerializeField] protected bool expires;
        [SerializeField] protected bool canStack = false;
        [ShowInInspector, ReadOnly] protected float TimeElapsed;

        public string BuffName => buffName;
        public bool IsExpired => TimeElapsed >= duration;
        public float Duration => duration;
        public bool Expires => expires;
        public bool CanStack => canStack;

        public abstract void ApplyBuff(Player player);
        public abstract void RemoveBuff(Player player);
        
        public void TickBuff(float deltaTime)
        {
            if (!Expires) return;
            TimeElapsed += deltaTime;
        }

        public void ResetTimeElapsed()
        {
            TimeElapsed = 0;
        }

        public bool Equals(Buff other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return buffName == other.buffName && modifier.Equals(other.modifier) && duration.Equals(other.duration) && expires == other.expires;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Buff)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(buffName, modifier, duration, expires);
        }
    }

    
    [Serializable]
    public sealed class JumpHeightBuff : Buff
    {
        public override void ApplyBuff(Player Player) => Player.Stats.SetBonusJumpImpulse(modifier);
        public override void RemoveBuff(Player Player) => Player.Stats.SetBonusJumpImpulse(-modifier);
    }


    [Serializable]
    public sealed class MoveSpeedBuff : Buff
    {
        public override void ApplyBuff(Player Player) => Player.Stats.SetBonusMoveSpeed(modifier);
        public override void RemoveBuff(Player Player) => Player.Stats.SetBonusMoveSpeed(-modifier);
    }
    
    [Serializable]
    public class FlatBonusStrengthBuff : Buff
    {
        public override void ApplyBuff(Player Player) => Player.Stats.Attributes.Find(x => x.AttributeType == AttributeType.Strength).ModifyFlatBonusValue((int)modifier);
        public override void RemoveBuff(Player Player) => Player.Stats.Attributes.Find(x => x.AttributeType == AttributeType.Strength).ModifyFlatBonusValue(-(int)modifier);
    }

    [Serializable]
    public class FlatBonusDexterityBuff : Buff
    {
        public override void ApplyBuff(Player Player) => Player.Stats.Attributes.Find(x => x.AttributeType == AttributeType.Dexterity).ModifyFlatBonusValue((int)modifier);
        public override void RemoveBuff(Player Player) => Player.Stats.Attributes.Find(x => x.AttributeType == AttributeType.Dexterity).ModifyFlatBonusValue(-(int)modifier);
    }

    [Serializable]
    public class FlatBonusIntelligenceBuff : Buff
    {
        public override void ApplyBuff(Player Player) => Player.Stats.Attributes.Find(x => x.AttributeType == AttributeType.Intelligence).ModifyFlatBonusValue((int)modifier);
        public override void RemoveBuff(Player Player) => Player.Stats.Attributes.Find(x => x.AttributeType == AttributeType.Intelligence).ModifyFlatBonusValue(-(int)modifier);
    }

    [Serializable]
    public class FlatBonusLuckBuff : Buff
    {
        public override void ApplyBuff(Player Player) => Player.Stats.Attributes.Find(x => x.AttributeType == AttributeType.Luck).ModifyFlatBonusValue((int)modifier);
        public override void RemoveBuff(Player Player) => Player.Stats.Attributes.Find(x => x.AttributeType == AttributeType.Luck).ModifyFlatBonusValue(-(int)modifier);
    }
    
    
    
    [Serializable]
    public class PercentBonusStrengthBuff : Buff
    {
        public override void ApplyBuff(Player Player) => Player.Stats.Attributes.Find(x => x.AttributeType == AttributeType.Strength).ModifyPercentBonusValue(modifier/100);
        public override void RemoveBuff(Player Player) => Player.Stats.Attributes.Find(x => x.AttributeType == AttributeType.Strength).ModifyPercentBonusValue(-modifier/100);
    }

    [Serializable]
    public class PercentBonusDexterityBuff : Buff
    {
        public override void ApplyBuff(Player Player) => Player.Stats.Attributes.Find(x => x.AttributeType == AttributeType.Dexterity).ModifyPercentBonusValue(modifier/100);
        public override void RemoveBuff(Player Player) => Player.Stats.Attributes.Find(x => x.AttributeType == AttributeType.Dexterity).ModifyPercentBonusValue(-modifier/100);
    }

    [Serializable]
    public class PercentBonusIntelligenceBuff : Buff
    {
        public override void ApplyBuff(Player Player) => Player.Stats.Attributes.Find(x => x.AttributeType == AttributeType.Intelligence).ModifyPercentBonusValue(modifier/100);
        public override void RemoveBuff(Player Player) => Player.Stats.Attributes.Find(x => x.AttributeType == AttributeType.Intelligence).ModifyPercentBonusValue(-modifier/100);
    }

    [Serializable]
    public class PercentBonusLuckBuff : Buff
    {
        public override void ApplyBuff(Player Player) => Player.Stats.Attributes.Find(x => x.AttributeType == AttributeType.Luck).ModifyPercentBonusValue(modifier/100);
        public override void RemoveBuff(Player Player) => Player.Stats.Attributes.Find(x => x.AttributeType == AttributeType.Luck).ModifyPercentBonusValue(-modifier/100);
    }
    
}