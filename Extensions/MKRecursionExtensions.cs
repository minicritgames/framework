using UnityEngine;

namespace Minikit
{
    public static class MKRecursionExtensions
    {
        public static void SetLayerRecursive(this GameObject _gameObject, int _layer, bool _recursivelyOnAllChildren = false)
        {
            _gameObject.layer = _layer;

            if (_recursivelyOnAllChildren)
            {
                foreach (Transform child in _gameObject.transform)
                {
                    child.gameObject.SetLayerRecursive(_layer, true);
                }
            }
        }

        public static void SetActiveRecursive(this GameObject _gameObject, bool _active)
        {
            _gameObject.SetActive(_active);

            foreach (Transform child in _gameObject.transform)
            {
                child.gameObject.SetActiveRecursive(_active);
            }
        }

        public static void SetCollidersEnabledRecursive(this GameObject _gameObject, bool _enabled)
        {
            foreach (Collider collider in _gameObject.GetComponents<Collider>())
            {
                collider.enabled = _enabled;
            }

            foreach (Transform child in _gameObject.transform)
            {
                child.gameObject.SetCollidersEnabledRecursive(_enabled);
            }
        }
    }
} // Minikit namespace
