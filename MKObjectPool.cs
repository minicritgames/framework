using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Minikit
{
    public interface IMKObjectPool
    {
        void Return(MKPooledObject _pooledObject);
    }

    public class MKObjectPool<T> : IMKObjectPool where T : Component
    {
        private static readonly ProfilerMarker createMarker = new("MK.Pools.Create");

        private readonly T prefab;
        private readonly Transform idleRoot;
        private readonly int maxIdle;
        private readonly Stack<MKPooledObject> idle = new();
        private int instanceCount;


        public MKObjectPool(T _prefab, Transform _idleRoot, int _maxIdle)
        {
            prefab = _prefab;
            idleRoot = _idleRoot;
            maxIdle = _maxIdle;
        }


        public T Get(Transform _parent = null)
        {
            Transform prefabTransform = prefab.transform;
            MKPooledObject pooledObject = TakeIdle() ?? Create();
            Place(pooledObject, _parent);
            pooledObject.transform.SetLocalPositionAndRotation(prefabTransform.localPosition, prefabTransform.localRotation);

            return Activate(pooledObject);
        }

        public T Get(Vector3 _position, Quaternion _rotation, Transform _parent = null)
        {
            MKPooledObject pooledObject = TakeIdle() ?? Create();
            Place(pooledObject, _parent);
            pooledObject.transform.SetPositionAndRotation(_position, _rotation);

            return Activate(pooledObject);
        }

        public void Prewarm(int _count)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            int target = Mathf.Min(_count, maxIdle);
            while (instanceCount < target)
            {
                idle.Push(Create());
            }
        }

        public void Return(MKPooledObject _pooledObject)
        {
            if (!_pooledObject)
            {
                return;
            }

            _pooledObject.SetInUse(false);
            _pooledObject.gameObject.SetActive(false);

            if (idle.Count >= maxIdle)
            {
                instanceCount--;
                Object.Destroy(_pooledObject.gameObject);
                return;
            }

            _pooledObject.transform.SetParent(idleRoot, false);
            idle.Push(_pooledObject);
        }

        private MKPooledObject TakeIdle()
        {
            while (idle.Count > 0)
            {
                MKPooledObject pooledObject = idle.Pop();
                if (pooledObject)
                {
                    return pooledObject;
                }

                instanceCount--;
            }

            return null;
        }

        private MKPooledObject Create()
        {
            using ProfilerMarker.AutoScope createScope = createMarker.Auto();

            GameObject instanceGO = Object.Instantiate(prefab.gameObject, idleRoot);
            instanceGO.SetActive(false);

            if (!instanceGO.TryGetComponent(out MKPooledObject pooledObject))
            {
                pooledObject = instanceGO.AddComponent<MKPooledObject>();
            }
            pooledObject.Bind(this, instanceGO.GetComponent<T>());
            pooledObject.SetInUse(false);

            instanceCount++;
            return pooledObject;
        }

        private void Place(MKPooledObject _pooledObject, Transform _parent)
        {
            Transform pooledTransform = _pooledObject.transform;
            if (_parent)
            {
                pooledTransform.SetParent(_parent, false);
            }
            else
            {
                pooledTransform.SetParent(null, false);
                SceneManager.MoveGameObjectToScene(_pooledObject.gameObject, SceneManager.GetActiveScene());
            }
            pooledTransform.localScale = prefab.transform.localScale;
        }

        private T Activate(MKPooledObject _pooledObject)
        {
            _pooledObject.SetInUse(true);
            _pooledObject.gameObject.SetActive(true);

            return (T)_pooledObject.pooledComponent;
        }
    }
}
