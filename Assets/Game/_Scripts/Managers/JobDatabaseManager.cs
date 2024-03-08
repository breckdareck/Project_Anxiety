using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Project_Anxiety.Game.Units;
using UnityEngine;
using UnityEngine.Serialization;

namespace Project_Anxiety.Game.Managers
{
   public sealed class JobDatabaseManager : MonoBehaviour
   {
      public static JobDatabaseManager Instance;
      
      [SerializeField] public List<JobData> JobDatas = new ();
       
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
      }



      public JobData GetJobDataByName(string jobName)
      {
         if (JobDatas.Exists(x => x.JobName == jobName))
         {
            return JobDatas.Find(x => x.JobName == jobName);
         }

         Debug.LogWarning($"JobData with name {jobName} not found.");
         return null;
      }
      
      
   }
}

