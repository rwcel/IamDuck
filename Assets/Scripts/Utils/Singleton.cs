using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    [SerializeField] bool isDontdestroy;

    private static T instance = null;
    public static T Instance
    {
        get
        {
            instance = instance ?? (FindObjectOfType(typeof(T)) as T);
            instance = instance ?? new GameObject(typeof(T).ToString(), typeof(T)).GetComponent<T>();
            return instance;
        }
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this as T;
        }

        if(isDontdestroy)
        {
            var obj = FindObjectsOfType<T>();
            if (obj.Length == 1)
                DontDestroyOnLoad(gameObject);
            else
            {
                Destroy(gameObject);
            }
        }

        AwakeInstance();
    }

    private void OnDestroy()
    {
        DestroyInstance();
        instance = null;
    }

    protected abstract void AwakeInstance();
    protected abstract void DestroyInstance();

    private void OnApplicationQuit()
    {
        instance = null;
    }
}