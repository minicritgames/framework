using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Minikit
{
    public static class MKPools
    {
        public const int defaultMaxIdle = 100;

        private static readonly Dictionary<(GameObject, Type), IMKObjectPool> poolsByPrefab = new();
        private static Transform root;


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            poolsByPrefab.Clear();
            root = null;
        }

        public static MKObjectPool<T> GetPool<T>(GameObject _prefab, int _maxIdle = defaultMaxIdle) where T : Component
        {
            (GameObject, Type) key = (_prefab, typeof(T));
            if (poolsByPrefab.TryGetValue(key, out IMKObjectPool pool))
            {
                return (MKObjectPool<T>)pool;
            }

            if (!_prefab.TryGetComponent(out T prefabComponent))
            {
                throw new ArgumentException($"{_prefab.name} has no {typeof(T).Name} on its root to pool");
            }

            MKObjectPool<T> newPool = new(prefabComponent, CreateIdleRoot(_prefab.name), _maxIdle);
            poolsByPrefab.Add(key, newPool);
            return newPool;
        }

        public static T Spawn<T>(GameObject _prefab, Transform _parent = null) where T : Component
        {
            return GetPool<T>(_prefab).Get(_parent);
        }

        public static T Spawn<T>(GameObject _prefab, Vector3 _position, Quaternion _rotation, Transform _parent = null) where T : Component
        {
            return GetPool<T>(_prefab).Get(_position, _rotation, _parent);
        }

        public static GameObject Spawn(GameObject _prefab, Vector3 _position, Quaternion _rotation, Transform _parent = null)
        {
            return Spawn<Transform>(_prefab, _position, _rotation, _parent).gameObject;
        }

        public static void Prewarm<T>(GameObject _prefab, int _count) where T : Component
        {
            if (_prefab)
            {
                GetPool<T>(_prefab).Prewarm(_count);
            }
        }

        public static void Prewarm(GameObject _prefab, int _count)
        {
            Prewarm<Transform>(_prefab, _count);
        }

        public static void Release(GameObject _gameObject)
        {
            if (_gameObject.TryGetComponent(out MKPooledObject pooledObject))
            {
                pooledObject.Release();
            }
            else
            {
                Object.Destroy(_gameObject);
            }
        }

        public static void ReleaseAfter(GameObject _gameObject, float _delay)
        {
            if (_gameObject.TryGetComponent(out MKPooledObject pooledObject))
            {
                pooledObject.ReleaseAfter(_delay);
            }
            else
            {
                Object.Destroy(_gameObject, _delay);
            }
        }

        private static Transform CreateIdleRoot(string _name)
        {
            if (!root)
            {
                GameObject rootGO = new("Pools");
                rootGO.SetActive(false);
                Object.DontDestroyOnLoad(rootGO);
                root = rootGO.transform;
            }

            GameObject idleRootGO = new(_name);
            idleRootGO.transform.SetParent(root, false);
            return idleRootGO.transform;
        }
    }
}
