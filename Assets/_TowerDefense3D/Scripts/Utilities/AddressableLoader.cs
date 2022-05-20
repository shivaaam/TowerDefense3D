using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TowerDefense3D
{
    public static class AddressableLoader
    {
        [System.Serializable]
        public class AssetInfo
        {
            public object assetReferenceId;
            public GameObject prefabAsset;
            public List<GameObject> instanceAssets;

            public AssetInfo(object assetRefId, GameObject asset, GameObject instanceAsset)
            {
                instanceAssets = new List<GameObject>();
                assetReferenceId = assetRefId;
                prefabAsset = asset;
                instanceAssets.Add(instanceAsset);
            }

            public void AddInstance(GameObject instanceAsset)
            {
                instanceAssets.Add(instanceAsset);
            }

            public void RemoveInstance(GameObject instanceAsset)
            {
                instanceAssets.Remove(instanceAsset);

                if (IsAssetEmpty())
                {
                    assetReferenceId = null;
                    prefabAsset = null;
                }
            }

            public bool IsAssetEmpty()
            {
                return instanceAssets.Count <= 0;
            }
        }

        public static List<AssetInfo> assetInfos = new List<AssetInfo>();

        public static GameObject InstantiateAddressable(AssetReference assetReference)
        {
            GameObject Obj = null;
            if (assetInfos.Exists(x => x.assetReferenceId == assetReference.RuntimeKey))
            {
                var info = assetInfos.FirstOrDefault(x => x.assetReferenceId == assetReference.RuntimeKey);
                Obj = MonoBehaviour.Instantiate(info.prefabAsset);
                info.AddInstance(Obj);
            }
            else
            {
                var loadObj = Addressables.LoadAssetAsync<GameObject>(assetReference);
                GameObject loadedOb = loadObj.WaitForCompletion();
                Obj = MonoBehaviour.Instantiate(loadedOb);
                AssetInfo assetInfo = new AssetInfo(assetReference.RuntimeKey, loadedOb, Obj);
                assetInfos.Add(assetInfo);
            }
            return Obj;
        }

        public static GameObject InstantiateAddressable(object assetReferenceKey)
        {
            GameObject Obj = null;
            if (assetInfos.Exists(x => x.assetReferenceId == assetReferenceKey))
            {
                var info = assetInfos.FirstOrDefault(x => x.assetReferenceId == assetReferenceKey);
                Obj = MonoBehaviour.Instantiate(info.prefabAsset);
                info.AddInstance(Obj);
            }
            else
            {
                var loadObj = Addressables.LoadAssetAsync<GameObject>(assetReferenceKey);
                GameObject loadedOb = loadObj.WaitForCompletion();
                Obj = MonoBehaviour.Instantiate(loadedOb);
                AssetInfo assetInfo = new AssetInfo(assetReferenceKey, loadedOb, Obj);
                assetInfos.Add(assetInfo);
            }
            return Obj;
        }

        public static void DestroyAndReleaseAddressable(GameObject addressable)
        {
            if (addressable)
            {
                var info = assetInfos.FirstOrDefault(x => x.instanceAssets.Contains(addressable));
                if (info != null)
                {
                    GameObject loadedOb = info.prefabAsset;

                    info.RemoveInstance(addressable);
                    MonoBehaviour.Destroy(addressable);

                    if (info.IsAssetEmpty())
                    {
                        ReleaseAddressable(loadedOb);
                        assetInfos.Remove(info);
                    }
                }
                else
                {
                    MonoBehaviour.Destroy(addressable);
                }
            }

        }

        public static void ReleaseAddressable(GameObject addressable)
        {
            if (addressable)
                Addressables.Release(addressable);
        }
    }
}
