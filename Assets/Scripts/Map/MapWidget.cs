using UnityEngine;
using System.Collections;

namespace Moon.Demo {
    public abstract class MapWidget : MonoBehaviour {
        public IntVec2[] TileList {
            get; private set;
        }
        public IntVec2 TilePos {
            get { return TileList[0]; }
        }
        public Rect bound {
            get; private set;
        }
        public abstract bool isObstacle {
            get;
        }

        public bool registerOnAwake = false;

        private Collider2D collider_;

        public void RegisterSelf() {
            float tiny_size = 0.1f;
            collider_ = GetComponent<Collider2D>();
            bound = new Rect(
                transform.position.x - tiny_size * 0.5f, transform.position.y - tiny_size * 0.5f,
                tiny_size, tiny_size);

            TileList = MapManager.Instance.RegisterObject(this);
            if (TileList.Length == 0) {
                Debug.LogError("[MapObject] the position of map object " + name + " is invalid.");
            }
        }

        public abstract void SetHighlight(bool is_highlight, bool has_op);
        public abstract bool IsOperale(PlayerController player);

        void Awake() {
            if (registerOnAwake) {
                RegisterSelf();
            }
        }
    }
}