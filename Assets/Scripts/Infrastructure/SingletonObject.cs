using UnityEngine;
using System.Collections;

namespace Moon.Demo {
    public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour {
        private static T instance_;

        public static T Instance {
            get {
                if (instance_ == null) {
                    T[] instances = FindObjectsOfType<T>();
                    if (instances.Length > 1) {
                        Debug.LogError(string.Format("there should be never more than one singleton {0} in this scene.", typeof(T).Name));
                    } else if (instances.Length == 1) {
                        instance_ = instances[0];
                    } else {
                        var obj = new GameObject("_" + typeof(T).Name);
                        instance_ = obj.AddComponent<T>();
                    }
                }
                return instance_;
            }
        }
    }
}
