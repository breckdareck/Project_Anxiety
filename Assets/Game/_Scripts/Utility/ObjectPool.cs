using System.Collections.Generic;
using UnityEngine;

namespace Project_Anxiety.Game.Utility
{
    public class ObjectPool<T> where T : MonoBehaviour
    {
        List<T> objectList;
        T objectPrefab;
        int poolSize;
        Transform parent;
        
        public ObjectPool(T prefab, int size, Transform parentTransform)
        {
            objectList = new List<T>();
            objectPrefab = prefab;
            poolSize = size;
            parent = parentTransform;
            FillPool();
        }

        private void FillPool()
        {
            for (int i = 0; i < poolSize; i++)
            {
                T obj = GameObject.Instantiate(objectPrefab, parent);
                obj.gameObject.SetActive(false);
                objectList.Add(obj);
            }
        }

        public T GetPooledObject()
        {
            foreach (T obj in objectList)
            {
                if (!obj.gameObject.activeSelf)
                    return obj;
            }
            return null;
        }
    }
}