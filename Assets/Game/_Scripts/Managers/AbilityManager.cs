using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project_Anxiety.Game.Units
{
    public class AbilityManager : MonoBehaviour
    {
        public static AbilityManager Instance;
        
        [ShowInInspector, ReadOnly] private Dictionary<int, AbilityData> _abilityLookup = new ();
        
        private void Awake()
        {
            
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            
            // Load all Ability assets from the Resources/Abilities folder
            AbilityData[] abilities = Resources.LoadAll<AbilityData>("ScriptableObjects/Abilities");

            foreach (AbilityData ability in abilities)
            {
                _abilityLookup[ability.ID] = ability;
            }
        }

        public AbilityData GetAbilityByID(int abilityID)
        {
            if (_abilityLookup.TryGetValue(abilityID, out AbilityData ability))
            {
                return ability;
            }

            Debug.LogWarning($"Ability with ID {abilityID} not found.");
            return null;
        }
    }
}