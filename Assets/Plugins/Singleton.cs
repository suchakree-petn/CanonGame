using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public bool CarryToOtherScene;
    static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
                instance = (T)FindObjectOfType(typeof(T));

            return instance;
        }
        protected set => instance = value;
    }

    protected virtual void Awake()
    {
        if (instance != null && instance != this)
        {
            DestroyImmediate(gameObject);
            return;
        }
        if (CarryToOtherScene)
        {
            transform.SetParent(null);
            DontDestroyOnLoad(transform.root);
        }

        instance = GetComponent<T>();
        InitAfterAwake();
    }

    protected abstract void InitAfterAwake();

}
