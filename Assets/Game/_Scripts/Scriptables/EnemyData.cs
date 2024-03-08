using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "New EnemyData", menuName = "EnemyData")]
[InlineEditor]
public sealed class EnemyData : ScriptableObject
{
    [field:SerializeField] public int EnemyLevel { get; private set; } = 1;
    [field:SerializeField] public int ExpToGiveOnDeath { get; private set; }

    public float walkSpeed = 3f;
    public float walkStopRate = 0.05f;
}
