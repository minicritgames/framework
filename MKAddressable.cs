using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
#endif // UNITY_EDITOR

namespace Minikit
{
    public static class MKAddressable
    {
        public static string GetAddressableKeyFromPrefab(GameObject _prefab)
        {
#if UNITY_EDITOR
            string addressableKey = string.Empty;
            
            if (AssetDatabase.GetAssetPath(_prefab) is string assetPath
                && AssetDatabase.AssetPathToGUID(assetPath) is string guid
                && AddressableAssetSettingsDefaultObject.Settings is AddressableAssetSettings settings
                && settings.FindAssetEntry(guid) is AddressableAssetEntry entry)
            {
                addressableKey = entry.address;
            }
            
            return addressableKey;
#else
            Debug.LogError("You cannot get addressable keys from a prefab outside of the editor");
            return string.Empty;
#endif // UNITY_EDITOR
        }

        public static T LoadAssetSync<T>(string _addressableKey)
        {
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(_addressableKey);
            handle.WaitForCompletion();
            return handle.Result;
        }
        
        public static AsyncOperationHandle<T> LoadAssetAsync<T>(string _addressableKey)
        {
            return Addressables.LoadAssetAsync<T>(_addressableKey);
        }
        
        public static async Task<T> LoadAssetAsyncTask<T>(string _addressableKey)
        {
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(_addressableKey);
            await handle.Task;

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"Failed to load Addressable asset with key: {_addressableKey}");
                return default;
            }
            
            return handle.Result;
        }
        
        public static void LoadAssetAsyncCallback<T>(string _addressableKey, Action<T> _callback = null)
        {
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(_addressableKey);
            handle.Completed += _handle =>
            {
                if (_handle.Status != AsyncOperationStatus.Succeeded)
                {
                    Debug.LogError($"Failed to load addressable with key: {_addressableKey}");
                    return;
                }
                
                _callback?.Invoke(_handle.Result);
            };
        }
    }
} // Minikit namespace
