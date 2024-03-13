using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project_Anxiety.Game.Utility
{
    public class ObjectPool<T> where T : MonoBehaviour
    {
        readonly Stack<T> objectList = new Stack<T>();
        readonly T objectPrefab;
        readonly Transform parent;
        
        public Action<T> OnObjectPulled = delegate { };
        public Action<T> OnObjectReturned = delegate { };
        
        public ObjectPool(T prefab, int size, Transform parentTransform)
        {
            objectPrefab = prefab;
            parent = parentTransform;
            
            for (int i = 0; i < size; i++)
            {
                AddObjectToPool();
            }
        }
        
        T AddObjectToPool()
        {
            T obj = GameObject.Instantiate(objectPrefab, parent);
            obj.gameObject.SetActive(false);
            objectList.Push(obj);
            return obj;
        }

        public T GetPooledObject()
        {
            if (objectList.Count > 0)
            {
                T obj = objectList.Pop();
                obj.gameObject.SetActive(true);
                OnObjectPulled.Invoke(obj);
                return obj;
            }

            T newObj = AddObjectToPool();
            OnObjectPulled.Invoke(newObj);
            return newObj;
        }

        public void ReturnObjectToPool(T obj)
        {
            obj.gameObject.SetActive(false);
            objectList.Push(obj);
            OnObjectReturned.Invoke(obj);
        }

        public int GetObjectListCount() => objectList.Count;
    }
}