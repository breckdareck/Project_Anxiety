using System;
using System.Collections;
using System.Runtime.InteropServices;
using Project_Anxiety.Game.Managers;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Project_Anxiety.Game.Units
{
    [System.Serializable]
    public sealed class Job
    {
        [field:SerializeField, ShowInInspector, PropertyOrder(0)] public JobData JobData { get; private set; }
        [field:SerializeField, ShowInInspector, FoldoutGroup("EXP")] public int JobLevel { get; private set; } = 1;
        [field:SerializeField, ShowInInspector, FoldoutGroup("EXP")] public int MaxJobLevel { get; private set; } = 100;
        [field:SerializeField, ShowInInspector, FoldoutGroup("EXP")] public int CurrentExp { get; private set; }

        [field:SerializeField, ShowInInspector, FoldoutGroup("EXP")] public AnimationCurve ExpCurve;
    
        [ShowInInspector, FoldoutGroup("EXP")] public int ExpToNextLevel => (int)ExpCurve.Evaluate(JobLevel);

        public Job(JobData jobData, AnimationCurve expCurve)
        {
            JobData = jobData;
            ExpCurve = expCurve;
        }
        
        [Button, FoldoutGroup("EXP")]
        public void AddExp(int expToGive)
        {
            CurrentExp += expToGive;
            PlayerEventManager.Instance.InvokePlayerExpChanged();
        }
        
        [Button, FoldoutGroup("EXP")]
        public void SetLevel(int level)
        {
            if (level <= JobLevel) return;
            if (level > MaxJobLevel)
            {
                Debug.LogWarning(
                    $"Level can only be set to {MaxJobLevel}. {level} is out of Scope. Setting to {MaxJobLevel}.");
                level = MaxJobLevel;
            }

            while (JobLevel != level)
            {
                GainLevel();
            }
        }

        public void CheckIfLeveled()
        {
            if (JobLevel >= MaxJobLevel)
            {
                CurrentExp = (int)ExpCurve.Evaluate(JobLevel);
                return;
            }

            if (CurrentExp < (int)ExpCurve.Evaluate(JobLevel)) return;
            while (CurrentExp >= (int)ExpCurve.Evaluate(JobLevel))
            {
                CurrentExp -= (int)ExpCurve.Evaluate(JobLevel);
                GainLevel();
            }
        }

        private void GainLevel()
        {
            JobLevel++;
            PlayerEventManager.Instance.InvokePlayerLevelChanged();
        }
    }
}