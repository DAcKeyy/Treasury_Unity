﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Treasury.Miscellaneous.Optimization.System
{
    public abstract class MonoAllocation : MonoBehaviour
    {
        private Dictionary<int, Component> _get;
        private Dictionary<int, Component[]> _gets;
        private Dictionary<int, Component> _childrenGet;
        private Dictionary<int, Component[]> _childrenGets;
        private Dictionary<int, Component> _parentGet;
        private Dictionary<int, Component[]> _parentGets;
        private Dictionary<int, Component> _find;
        private Dictionary<int, Component[]> _finds;

        public int GetID() => cachedInstanceId ??= GetInstanceID();
        private int? cachedInstanceId;

        public GameObject CachedGameObject => cachedGameObject ??= gameObject;
        private GameObject cachedGameObject;

        public Transform CachedTransform => cachedTransform ??= transform;
        private Transform cachedTransform;

        private bool allocationEnabled = true;

        public void EnableAllocation()
        {
            allocationEnabled = true;
        }

        public void DisableAllocation()
        {
            allocationEnabled = false;
        }

        public T GetComponentCached<T>() => GetComponent<T>();
        
        public T[] GetComponentsCached<T>() => GetComponents<T>();
        
        public T GetComponentInChildrenCached<T>() => GetComponentInChildren<T>();
        
        public T[] GetComponentsInChildrenCached<T>() => GetComponentsInChildren<T>();
        
        public T GetComponentInParentCached<T>() => GetComponentInParent<T>();
        
        public T[] GetComponentsInParentCached<T>() => GetComponentsInParent<T>();
        
        public T FindObjectOfTypeCached<T>() where T : Object => FindObjectOfType<T>();
        
        public T[] FindObjectsOfTypeCached<T>() where T : Object => FindObjectsOfType<T>();
        

        public T GetCached<T>() where T : Component
        {
            return GetSingleCached(_get, GetComponent<T>);
        }

        public T[] GetsCached<T>() where T : Component
        {
            return GetManyCached(_gets, GetComponents<T>);
        }

        public T ChildrenGetCached<T>() where T : Component
        {
            return GetSingleCached(_childrenGet, GetComponentInChildren<T>);
        }

        public T[] ChildrenGetsCached<T>() where T : Component
        {
            return GetManyCached(_childrenGets, GetComponentsInChildren<T>);
        }

        public T ParentGetCached<T>() where T : Component
        {
            return GetSingleCached(_parentGet, GetComponentInParent<T>);
        }

        public T[] ParentGetsCached<T>() where T : Component
        {
            return GetManyCached(_parentGets, GetComponentsInParent<T>);
        }

        public T FindCached<T>() where T : Component
        {
            return GetSingleCached(_find, FindObjectOfType<T>);
        }

        public T[] FindsCached<T>() where T : Component
        {
            return GetManyCached(_finds, FindObjectsOfType<T>);
        }
        
        private T GetSingleCached<T>(Dictionary<int, Component> storage, Func<T> getMethod) where T : Component
        {
            var index = GetInfo<T>.Index;

            if (allocationEnabled)
            {
                storage ??= new Dictionary<int, Component>(16);

                if (storage.TryGetValue(index, out var component))
                {
                    return (T) component;
                }
            }

            var instance = getMethod?.Invoke();

            if (allocationEnabled && instance != null)
            {
                storage.Add(index, instance);
            }

            return instance;
        }

        private T[] GetManyCached<T>(Dictionary<int, Component[]> storage, Func<T[]> getsMethod) where T : Component
        {
            var index = GetInfo<T>.Index;

            if (allocationEnabled)
            {
                storage ??= new Dictionary<int, Component[]>(16);

                if (storage.TryGetValue(index, out var components))
                {
                    return (T[]) components;
                }
            }

            var instances = getsMethod?.Invoke();

            if (allocationEnabled && instances != null)
            {
                storage.Add(index, instances);
            }

            return instances;
        }
    }
}