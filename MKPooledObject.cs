using UnityEngine;

namespace Minikit
{
    [DisallowMultipleComponent]
    public class MKPooledObject : MonoBehaviour
    {
        public bool inUse { get; private set; }

        internal Component pooledComponent { get; private set; }

        private IMKObjectPool pool;
        private float timeOfRelease;


        private void Update()
        {
            if (Time.time >= timeOfRelease)
            {
                Release();
            }
        }


        public void Release()
        {
            if (!inUse)
            {
                return;
            }

            pool.Return(this);
        }

        public void ReleaseAfter(float _delay)
        {
            timeOfRelease = Time.time + _delay;
            enabled = true;
        }

        internal void Bind(IMKObjectPool _pool, Component _pooledComponent)
        {
            pool = _pool;
            pooledComponent = _pooledComponent;
        }

        internal void SetInUse(bool _inUse)
        {
            inUse = _inUse;
            enabled = false;
        }
    }
}
