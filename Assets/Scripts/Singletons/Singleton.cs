using UnityEngine;

public class Singleton<T> : MonoBehaviour where T: MonoBehaviour
{

    protected virtual void Awake()
    {
        _instance = GetComponent<T>();
    }

    static bool err;

    private static T _instance;
    public static T Instance
    {
        get
        {
            _instance ??= (T)FindObjectOfType(typeof(T));
            if (_instance == null && !err)
            {
                err = true;
                Debug.LogError($"An instance of {typeof(T)} is needed in the scene, but was not found!");
            }
            return _instance;
        }
    }
    
}
