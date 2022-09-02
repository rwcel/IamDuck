using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct FPool
{
    public int _Count;
    public GameObject _Obj;
    public Transform parent;   // *개별 부모가 필요한 경우 넣기 null인경우 poolParent로
}

// **여기 등록해서 쓰는 방식이 아니라 LevelData에서 넣고
// 게임매니저에서 호출해서 사용해야 하지 않나? 프리팹 2개나 들어가 있음
public class PoolingManager : Singleton<PoolingManager>
{
    [SerializeField] Transform poolParent;
    [SerializeField] FPool[] pools;

    private Dictionary<string, Queue<GameObject>> poolDictionary;
    private Dictionary<string, Transform> poolParents;

    System.Action OnAllEnqueue;

    protected override void AwakeInstance()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        poolParents = new Dictionary<string, Transform>();

        GeneratePrefab();
        ResourcePrefab();
    }

    protected override void DestroyInstance() { }

    private void GeneratePrefab()
    {
        foreach (var item in pools)
        {
            Queue<GameObject> poolQueue = new Queue<GameObject>();
            Transform parent;
            if (item.parent == null)
            {
                parent = new GameObject(item._Obj.name + " Pool").transform;
                parent.SetParent(poolParent);
            }
            else
            {
                parent = item.parent;
            }
            for (int i = 0, length = item._Count; i < length; i++)
            {
                GameObject obj = Instantiate(item._Obj, parent);
                obj.name = item._Obj.name + "_" + i.ToString();
                obj.SetActive(false);

                poolQueue.Enqueue(obj);
            }
            poolDictionary.Add(item._Obj.name, poolQueue);
            poolParents.Add(item._Obj.name, parent);
            // Debug.Log(item._Obj.name);
        }
    }

    private void ResourcePrefab()
    {
        // Debug.Log("FloorMap : " + LevelData.Instance.FloorMap.Count);

        foreach (var item in LevelData.Instance.FloorMap)
        {
            Queue<GameObject> poolQueue = new Queue<GameObject>();
            Transform parent = new GameObject(item.Key + " Pool").transform;
            parent.SetParent(poolParent);

            for (int i = 0, length = item.Value.pool; i < length; i++)
            {
                GameObject obj = Instantiate(item.Value.prefab, parent);
                obj.name = item.Key + "_" + i.ToString();
                obj.SetActive(false);

                poolQueue.Enqueue(obj);
            }
            poolDictionary.Add(item.Key.ToString(), poolQueue);
            poolParents.Add(item.Key.ToString(), parent);
            // Debug.Log(item._Obj.name);
        }

        foreach (var item in LevelData.Instance.InGameItemMap)
        {
            Queue<GameObject> poolQueue = new Queue<GameObject>();
            Transform parent = new GameObject(item.Key + " Pool").transform;
            parent.SetParent(poolParent);

            for (int i = 0, length = item.Value.pool; i < length; i++)
            {
                GameObject obj = Instantiate(item.Value.prefab, parent);
                obj.name = item.Key + "_" + i.ToString();
                obj.SetActive(false);

                poolQueue.Enqueue(obj);
            }
            poolDictionary.Add(item.Key.ToString(), poolQueue);
            poolParents.Add(item.Key.ToString(), parent);
            // Debug.Log(item._Obj.name);
        }

        foreach (var item in LevelData.Instance.ObstacleMap)
        {
            Queue<GameObject> poolQueue = new Queue<GameObject>();
            Transform parent = new GameObject(item.Key + " Pool").transform;
            parent.SetParent(poolParent);

            for (int i = 0, length = item.Value.pool; i < length; i++)
            {
                GameObject obj = Instantiate(item.Value.prefab, parent);
                obj.name = item.Key + "_" + i.ToString();
                obj.SetActive(false);

                poolQueue.Enqueue(obj);
            }
            poolDictionary.Add(item.Key.ToString(), poolQueue);
            poolParents.Add(item.Key.ToString(), parent);
            // Debug.Log(item._Obj.name);
        }
    }

    public GameObject Dequeue(string name, Vector3 pos, Quaternion rot, bool isLocal = false)
    {
        if (name == Values.Key_Null)
            return null;

        var obj = poolDictionary[name].Dequeue();
        if (poolDictionary[name].Count <= 0)
        {
            CreateObjPool(name, obj);
        }

        if (isLocal)
        {
            obj.transform.localPosition = pos;
            obj.transform.localRotation = rot;
        }
        else
        {
            obj.transform.position = pos;
            obj.transform.rotation = rot;
        }
        obj.SetActive(true);

        OnAllEnqueue += () => Enqueue(obj, true);

        return obj;
    }

    public GameObject Dequeue(string name, Transform parent)
    {
        if (name == Values.Key_Null)
            return null;

        // Debug.Log($"{name} Pool : {poolDictionary[name].Count}");

        var obj = poolDictionary[name].Dequeue();
        if (poolDictionary[name].Count <= 0)
        {
            Debug.LogWarning($"비었음 : {name}");
            CreateObjPool(name, obj);
        }

        obj.transform.position = Vector2.zero;
        obj.transform.SetParent(parent, false);
        obj.SetActive(true);

        OnAllEnqueue += () => Enqueue(obj, true);

        return obj;
    }

    public void Enqueue(GameObject obj, bool isSetParent = false)
    {
        obj.SetActive(false);

        string name = obj.name.Split('_')[0];
        if (isSetParent)
        {
            obj.transform.SetParent(poolParents[name]);
        }

        poolDictionary[name].Enqueue(obj);

        OnAllEnqueue -= () => Enqueue(obj, true);
    }

    void CreateObjPool(string name, GameObject item)
    {
        Transform parent = item.transform.parent;
        for (int i = 1; i <= 5; i++)
        {
            GameObject obj = Instantiate(item, parent);
            obj.name = name + "_" + i.ToString();

            poolDictionary[name].Enqueue(obj);
        }
    }

    public void AllEnqueueObjects()
    {
        OnAllEnqueue?.Invoke();
        OnAllEnqueue = null;
    }
}