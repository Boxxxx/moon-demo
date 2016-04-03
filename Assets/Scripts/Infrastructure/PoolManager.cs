using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Moon.Demo {
    public class PoolManager : SingletonMonoBehaviour<PoolManager> {
        [Tooltip("If set to false, then the allocate & deallocate will be deliver to unity directly.")]
        public bool usePoolManager = true;
        public List<ObjectPool.Options> poolOptions = new List<ObjectPool.Options>();
        public bool loadWhenAwake = false;
        public bool showDebugLog = false;
        public bool dontDestroyOnLoad = false;
        public bool autoAddMissingPrefabPool = true;
        public int capacityOfMissingPool = 10;

        private bool is_loaded_ = false;
        private Dictionary<string, ObjectPool> pools_ = new Dictionary<string, ObjectPool>();
        private Dictionary<GameObject, ObjectPool> instance_to_pool_map = new Dictionary<GameObject, ObjectPool>();

        public ObjectPool NewPool(ObjectPool.Options options) {
            options.Preprocess();
            if (pools_.ContainsKey(options.name)) {
                Debug.LogError(string.Format("There is already a ObjectPool of {0}, you must use unique name.", options.name));
                return null;
            } else if (options.prefab == null) {
                Debug.LogError("Prefab of options must be set.");
                return null;
            }

            Transform folder;
            if (options.poolFolder != null) {
                folder = options.poolFolder;
            } else {
                folder = new GameObject(options.name).transform;
                folder.parent = transform;
                folder.position = Vector3.zero;
            }

            var object_pool = new ObjectPool(options, folder);
            pools_[options.name] = object_pool;

            return object_pool;
        }
        public ObjectPool NewPool(GameObject prefab) {
            ObjectPool.Options options = new ObjectPool.Options();
            options.prefab = prefab;
            options.maxCapacity = capacityOfMissingPool;
            return NewPool(options);
        }

        private GameObject AllocateInternal(string name, Vector3 position, Quaternion rotation) {
            if (showDebugLog) {
                Debug.Log("Allocate object " + name);
            }

            ObjectPool pool;
            if (!pools_.TryGetValue(name, out pool)) {
                Debug.LogError(string.Format("There is no object named {0} in PoolManager", name));
                return null;
            }

            return pool.Allocate(position, rotation);
        }
        private GameObject AllocateInternal(GameObject prefab, Vector3 position, Quaternion rotation) {
            ObjectPool pool;
            if (!pools_.TryGetValue(prefab.name, out pool)) {
                if (autoAddMissingPrefabPool) {
                    NewPool(prefab);
                } else {
                    Debug.LogError("There is no object pool for " + prefab.name);
                    return null;
                }
            }

            return AllocateInternal(prefab.name, position, rotation);
        }

        private bool DeallocateInternal(GameObject obj) {
            if (showDebugLog) {
                Debug.Log("Deallocate object " + obj.name);
            }

            ObjectPool pool;
            if (instance_to_pool_map.TryGetValue(obj.gameObject, out pool)) {
                return pool.Deallocate(obj);
            } else {
                return false;
            }
        }

        public ObjectPool GetPool(string name) {
            ObjectPool pool;
            if (pools_.TryGetValue(name, out pool)) {
                return pool;
            } else {
                return null;
            }
        }

        public GameObject CreateNew(GameObject prefab, ObjectPool pool) {
            GameObject new_obj = GameObject.Instantiate(prefab) as GameObject;
            instance_to_pool_map[new_obj] = pool;
            new_obj.transform.parent = pool.parentTransform;
            new_obj.gameObject.SetActive(false);
            return new_obj;
        }

        void Awake() {
            if (Instance != this) {
                // If it is not the singleton instance, destroy self.
                GameObject.Destroy(gameObject);
                return;
            }

            if (dontDestroyOnLoad) {
                DontDestroyOnLoad(gameObject);
            }

            if (loadWhenAwake) {
                is_loaded_ = true;
                for (int i = 0; i < poolOptions.Count; i++) {
                    NewPool(poolOptions[i]);
                }
            }
        }

        public IEnumerator Load() {
            if (!is_loaded_) {
                is_loaded_ = true;
                for (int i = 0; i < poolOptions.Count; i++) {
                    NewPool(poolOptions[i]);
                }
            }
            yield return null;
        }

        /// <summary>
        // Since all active objects will be destroyed by Unity when the scene is destroyed.
        // We should preload new asset and cleanup old ones when the level was loaded.
        /// </summary>
        void OnLevelWasLoaded(int unused_level) {
            foreach (var pool_pair in pools_) {
                pool_pair.Value.RecoverObjects(true);
            }
        }

        #region Static interfaces
        public static void Init() {
            var unused_instance = Instance;
            // no-op, just used to create the singleton instance.
        }
        public static GameObject Allocate(string name, Vector3 position, Quaternion rotation) {
            if (!Instance.usePoolManager) {
                Debug.LogError("Can't allocate by name when usePoolManager=false");
                return null;
            }
            return Instance.AllocateInternal(name, position, rotation);
        }
        public static GameObject Allocate(string name) {
            return Allocate(name, Vector3.zero, Quaternion.identity);
        }
        public static T Allocate<T>(string name, Vector3 position, Quaternion rotation) where T : Component {
            var game_object = Allocate(name, position, rotation);
            if (game_object != null) {
                return game_object.GetComponent<T>();
            } else {
                return null;
            }
        }
        public static T Allocate<T>(string name) where T : Component {
            return Allocate<T>(name, Vector3.zero, Quaternion.identity);
        }

        public static GameObject Allocate(GameObject prefab, Vector3 position, Quaternion rotation) {
            if (!Instance.usePoolManager) {
                return GameObject.Instantiate(prefab, position, rotation) as GameObject;
            }
            return Instance.AllocateInternal(prefab, position, rotation);
        }
        public static GameObject Allocate(GameObject prefab) {
            return Allocate(prefab, Vector3.zero, Quaternion.identity);
        }
        public static T Allocate<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component {
            var game_object = Allocate(prefab.gameObject, position, rotation);
            if (game_object != null) {
                return game_object.GetComponent<T>();
            }
            else {
                return null;
            }
        }
        public static T Allocate<T>(T prefab) where T : Component {
            return Allocate<T>(prefab, Vector3.zero, Quaternion.identity);
        }

        public static bool Deallocate(GameObject obj) {
            if (!Instance.usePoolManager) {
                GameObject.Destroy(obj);
                return true;
            }
            return Instance.DeallocateInternal(obj);
        }
        public static bool Deallocate(Component comp) {
            return Instance.DeallocateInternal(comp.gameObject);
        }
        #endregion
    }
}
