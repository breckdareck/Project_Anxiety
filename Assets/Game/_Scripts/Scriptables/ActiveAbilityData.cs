using System.Collections;
using System.Collections.Generic;
using Project_Anxiety.Game.Units;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "New Active Ability", menuName = "Active Ability")]
public class ActiveAbilityData : AbilityData
{
    [SerializeField] private AbilityAttackType _abilityAttackType;
    [SerializeField] private int _damagePercent = 100;
    [SerializeField] private int _targetAmount = 1;
    [SerializeField] private int _hitAmount = 1;
    [SerializeField] private int _manaCost = 0;
    [SerializeField] private float _cooldownTime = 1;
    [SerializeField] private Target _target;
    [SerializeField] private bool _isOnCooldown = false;
    [SerializeField] private AnimationClip _abilityAnimation;
    [SerializeField] private GameObject _projectileAsset;

    public AbilityAttackType AbilityAttackType => _abilityAttackType;
    public int DamagePercent => _damagePercent;
    public int TargetAmount => _targetAmount;
    public int HitAmount => _hitAmount;
    public int ManaCost => _manaCost;
    public float CooldownTime => _cooldownTime;
    [ShowInInspector, ReadOnly] public float CooldownTimeLeft { get; set; } = 0f;
    public Target Target => _target;
    public bool CanUse => !_isOnCooldown;
    public AnimationClip AbilityAnimation => _abilityAnimation;
    public GameObject ProjectileAsset => _projectileAsset;
        
    public void ModifyIsOnCooldown(bool value)
    {
        _isOnCooldown = value;
    }
}
