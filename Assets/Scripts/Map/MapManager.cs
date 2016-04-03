using UnityEngine;
using System.Collections.Generic;

namespace Moon.Demo {
    public class MapManager : SingletonMonoBehaviour<MapManager> {
        [System.Serializable]
        public struct MapSprites {
            public SpriteRenderer grass;
            public SpriteRenderer stone;
            public SpriteRenderer wood;
            public SpriteRenderer plowed;
            public SpriteRenderer seed;
            public SpriteRenderer flower;
        }

        public IntVec2 size;
        public Vector2 unitSize;

        public MapSprites mapSprites;

        public Transform widgetFolder;
        public FieldWidget fieldPrefab;

        private float map_width_;
        private float map_height_;

        private MapWidget[,] map_slots_;

        void Awake() {
            map_slots_ = new MapWidget[size.height, size.width];
            map_width_ = size.width * unitSize.x;
            map_height_ = size.height * unitSize.y;
        }

        void Start() {
            for (int i = 0; i < size.height; i++) {
                for (int j = 0; j < size.width; j++) {
                    if (map_slots_[i, j] == null) {
                        var field = PoolManager.Allocate(fieldPrefab);
                        field.name = string.Format("Slot({0}, {1})", j, i);
                        field.transform.parent = widgetFolder;
                        field.transform.localPosition = new Vector3(j, i, 0);
                        field.transform.localScale = Vector3.one;
                        field.RegisterSelf();
                    }
                }
            }
        }

        void Update() {
            for (int i = 0; i <= size.width; i++) {
                float offset_x = unitSize.x * i + transform.position.x;
                Debug.DrawLine(
                    new Vector2(offset_x, transform.position.y),
                    new Vector2(offset_x, map_height_ + transform.position.y), Color.red);
            }
            for (int i = 0; i <= size.height; i++) {
                float offset_y = unitSize.y * i + transform.position.y;
                Debug.DrawLine(
                    new Vector2(transform.position.x, offset_y),
                    new Vector2(map_width_ + transform.position.x, offset_y),
                    Color.red);
            }
        }

        public bool IsValidTilePos(IntVec2 pos) {
            return pos.x >= 0 && pos.x < size.x && pos.y >= 0 && pos.y < size.y;
        }
        public IntVec2 WorldPosToTilePos(Vector2 world_pos) {
            Vector2 offset = world_pos - (Vector2)transform.position;
            return new IntVec2(Mathf.FloorToInt(offset.x / unitSize.x), Mathf.FloorToInt(offset.y / unitSize.y));
        }
        public Vector2 TilePosToWorldPos(IntVec2 pos) {
            return (Vector2)transform.position + new Vector2(pos.x * unitSize.x + unitSize.x * 0.5f, pos.y * unitSize.y + unitSize.y * 0.5f);
        }
        public MapWidget SelectWidget(Vector2 world_pos) {
            IntVec2 tile_pos = WorldPosToTilePos(world_pos);
            if (!IsValidTilePos(tile_pos)) {
                return null;
            }
            return map_slots_[tile_pos.y, tile_pos.x];
        }
        public List<Vector2> GetValidAdjancentTile(MapWidget widget) {
            IntVec2[] dir_offsets = new IntVec2[] {
                new IntVec2(-1, 0),
                new IntVec2(0, 1),
                new IntVec2(1, 0),
                new IntVec2(0, -1)
            };
            List<Vector2> candidates = new List<Vector2>();
            foreach (var tile_pos in widget.TileList) {
                foreach (var offset in dir_offsets) {
                    var candidate_pos = new IntVec2(tile_pos.x - offset.x, tile_pos.y + offset.y);
                    if (IsValidTilePos(candidate_pos)) {
                        var slot = map_slots_[candidate_pos.y, candidate_pos.x];
                        if (slot != widget && !slot.isObstacle) {
                            candidates.Add(TilePosToWorldPos(candidate_pos));
                        }
                    }
                }
            }
            return candidates;
        }

        public IntVec2[] RegisterObject(MapWidget map_obj) {
            var min_pos = WorldPosToTilePos(map_obj.bound.min);
            var max_pos = WorldPosToTilePos(map_obj.bound.max);
            min_pos = new IntVec2(Mathf.Max(0, min_pos.x), Mathf.Max(0, min_pos.y));
            max_pos = new IntVec2(Mathf.Min(size.width - 1, max_pos.x), Mathf.Min(size.height - 1, max_pos.y));

            List<IntVec2> occupied_tiles = new List<IntVec2>();
            for (int index_y = min_pos.y; index_y <= max_pos.y; index_y++) {
                for (int index_x = min_pos.x; index_x <= max_pos.x; index_x++) {
                    if (map_slots_[index_y, index_x] != null) {
                        Debug.LogErrorFormat("[MapManager] position ({0}, {1}) is already occupied by {2}", index_x, index_y, map_slots_[index_y, index_x].name);
                        continue;
                    }
                    map_slots_[index_y, index_x] = map_obj;
                    occupied_tiles.Add(new IntVec2(index_x, index_y));
                }
            }
            return occupied_tiles.ToArray();
        }

        private void InsertIfEmpty(IntVec2 pos) {

        }
    }
}