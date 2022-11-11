using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMonobehaviour<T> : MonoBehaviour where T:MonoBehaviour
{
    public static T Instance {
        get {
            if (SingletonMonobehaviour<T>.instance == null) {
                SingletonMonobehaviour<T>.instance = (T)(UnityEngine.Object.FindObjectOfType(typeof(T)));
            }
            return SingletonMonobehaviour<T>.instance;
        }
    }

    protected bool CheckInstance() {
        if (this != SingletonMonobehaviour<T>.Instance) {
            UnityEngine.Object.Destroy(base.gameObject);
            return false;
        }
        return true;
    }

    protected void ReleaseInstance() {
        SingletonMonobehaviour<T>.instance = default(T);
    }

    private static T instance;
}
