using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public class LoadManager : MonoBehaviour
{
    public AssetLabelReference assetLabel;

    private IList<IResourceLocation> locations;             // ��� ĳ��
    private List<GameObject> gameObjects = new List<GameObject>();          // Destroy�� ���� ���� ĳ��

    public void GetLocations()
    {
        Addressables.GetDownloadSizeAsync(assetLabel.labelString).Completed +=
            (handle) =>
            {
                Debug.Log($"size : {handle.Result}");
            };
    }

    public void Instantiate()
    {
        var location = locations[Random.Range(0, locations.Count)];

        Addressables.InstantiateAsync(location, Vector3.one, Quaternion.identity).Completed +=
            (handle) =>
            {
                gameObjects.Add(handle.Result);
            };
    }

    public void Destroy()
    {
        if (gameObjects.Count == 0)
            return;

        var idx = gameObjects.Count - 1;

        Addressables.ReleaseInstance(gameObjects[idx]);
        gameObjects.RemoveAt(idx);
    }
}
