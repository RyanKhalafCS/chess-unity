using UnityEngine;

public class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.FindObjectsOfTypeAll<T>()[0];
                if (_instance == null)
                {
                    Debug.LogError($"[ScriptableObjectSingleton] No instance of {typeof(T)} found.");
                }
            }
            return _instance;
        }
    }
}