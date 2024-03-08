using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Project_Anxiety.Game.Managers;
using Project_Anxiety.Game.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Project_Anxiety.Game.Units
{
    [RequireComponent(typeof(CharacterBase))]
    public sealed class JobSystem : MonoBehaviour
    {
        public CharacterBase CharacterBase { get; private set; }
        [ShowInInspector, ReadOnly] public Job CurrentJob { get; private set; }
        [field:SerializeField, ShowInInspector] public List<Job> JobList { get; private set; } = new ();

        [SerializeField] private AnimationCurve expCurve = new();

        private void Awake()
        {
            CharacterBase = GetComponent<CharacterBase>();
            if (JobList == null || JobList.Count < 1)
            {
                JobList = new();
                foreach (var jobData in JobDatabaseManager.Instance.JobDatas)
                {
                    JobList.Add(new Job(jobData, expCurve));
                }
            }
            if (CurrentJob == null || CurrentJob.JobData == null) CurrentJob = JobList[0];
        }

        private void OnEnable()
        {
            PlayerEventManager.Instance.OnPlayerExpChangedEvent += CurrentJob.CheckIfLeveled;
            EventLogger.Instance.LoggedEventActions.Add(new EventLogInfo(this,"OnPlayerExpChangedEvent","CheckIfLeveled"));
        }

        private void OnDisable()
        {
            PlayerEventManager.Instance.OnPlayerExpChangedEvent -= CurrentJob.CheckIfLeveled;
            EventLogger.Instance.LoggedEventActions.Remove(new EventLogInfo() {Component = this, EventName = "OnPlayerExpChangedEvent", FunctionName = "CheckIfLeveled"});
        }

        private void Start()
        {
            foreach (var ability in GetAbilitiesFromCurrentJob().Where(ability => ability is PassiveAbilityData && !ability.Buffs.Exists(x => x.CanStack)))
            {
                ((Player)CharacterBase).PlayerCombatSystem.UsePassiveAbility(ability as PassiveAbilityData);
            }
        }

        public List<AbilityData> GetAbilitiesFromCurrentJob()
        {
            if (JobList == null)
            {
                Debug.LogError("No Job is not set!");
                return new List<AbilityData>();
            }

            return CurrentJob.JobData.JobAbilities;
        }

        [Button]
        public void SwapJobs(string jobToSwapTo)
        {
            CurrentJob = JobList.Find(x => x.JobData.JobName == jobToSwapTo);
        }
    }
}
