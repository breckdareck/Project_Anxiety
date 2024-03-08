using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;

namespace Project_Anxiety.Game.Units
{
    public abstract class AbilityData : ScriptableObject
    {
        [SerializeField] private int _id = 0;
        [SerializeField] private string _name = "New Ability";
        [SerializeField] private string _description;
        [SerializeField] private Sprite _sprite;
        [SerializeField] private int _level = 1;
        [SerializeReference] private List<Buff> _buffs;

        public int ID => _id;
        public string Name => _name;
        public string Description => _description;
        public Sprite Sprite => _sprite;
        public int Level => _level;
        public List<Buff> Buffs => _buffs;
    }

    public enum AbilityAttackType : byte
    {
        Melee,
        Ranged,
        Magic
    }
    
    public enum Target : byte
    {
        Self,
        Enemies,
        Friendlies
    }
}