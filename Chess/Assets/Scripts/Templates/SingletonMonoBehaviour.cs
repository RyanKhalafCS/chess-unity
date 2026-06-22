using UnityEngine;


//https://csharpindepth.com/articles/singleton
public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static bool _quitting = false;

    public static T Instance
    {
        get
        {
            if (_quitting)
            {
                Debug.LogWarning($"[Singleton] Instance '{typeof(T)}' already destroyed on application quit.");
                return null;
            }

            if (_instance == null)
            {
                _instance = FindAnyObjectByType<T>();

                if (_instance == null)
                {
                    //Be careful, auto object creation is being done. This is a design choice.
                    //Good for purely code objects, objects not connected to anything in the Unity inspector.
                    //Example use cases: Event bus, Save Manager, Global Config.
                    //Be careful if using for managers requiring inspector references.
                    GameObject singletonObj = new(typeof(T).Name);
                    _instance = singletonObj.AddComponent<T>();
                }

                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);//remove
        }
        else if (_instance != this)
        {
            Debug.LogWarning(
                $"Duplicate singleton {typeof(T).Name} found on {name}. Destroying duplicate.");
            Destroy(gameObject); // Ensure only one instance exists
        }
    }


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        _instance = null;
        _quitting = false;
    }


    protected virtual void OnApplicationQuit()
    {
        _quitting = true;
    }

    protected virtual void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}