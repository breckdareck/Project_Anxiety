using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project_Anxiety.Game.Units
{
    [CreateAssetMenu(fileName = "New JobData", menuName = "JobData")]
    [InlineEditor]
    public sealed class JobData : ScriptableObject
    {
        [ShowInInspector] public string JobName => name;
        [field:SerializeField] public List<AbilityData> JobAbilities { get; private set; }
        [field:SerializeField] public AttributeType JobMainAttribute { get; private set; }
        [field:SerializeField] public AttributeType JobSecondaryAttribute { get; private set; }
    }
}
