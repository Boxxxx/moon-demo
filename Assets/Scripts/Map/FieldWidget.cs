using UnityEngine;
using System.Collections;
using System;

namespace Moon.Demo {
    public class FieldWidget : MapWidget {
        public enum FieldType {
            kOcuppied,
            kPlowed,
            kSeed,
            kFlower,
            kEmpty
        }

        public enum OccupiedType {
            kGrass,
            kWood,
            kStone,
            kNone
        }


        public SpriteRenderer graphics;
        public SpriteRenderer highlight;
        public PolyNavObstacle obstacle;

        public FieldType fieldType = FieldType.kEmpty;
        public OccupiedType occupiedType = OccupiedType.kNone;
        private bool is_obstacle_ = false;

        public override bool isObstacle {
            get {
                return is_obstacle_;
            }
        }

        public void SetData(FieldType field_type, OccupiedType occupied_type = OccupiedType.kNone) {
            fieldType = field_type;
            occupiedType = field_type == FieldType.kOcuppied ? occupied_type : OccupiedType.kNone;
            is_obstacle_ = false;
            switch (fieldType) {
                case FieldType.kOcuppied:
                    switch (occupiedType) {
                        case OccupiedType.kGrass:
                            ChangeGraphics(MapManager.Instance.mapSprites.grass);
                            break;
                        case OccupiedType.kStone:
                            ChangeGraphics(MapManager.Instance.mapSprites.stone);
                            is_obstacle_ = true;
                            break;
                        case OccupiedType.kWood:
                            ChangeGraphics(MapManager.Instance.mapSprites.wood);
                            is_obstacle_ = true;
                            break;
                        default:
                            // kNone
                            ChangeGraphics(null);
                            break;
                    }
                    break;
                case FieldType.kPlowed:
                    ChangeGraphics(MapManager.Instance.mapSprites.plowed);
                    break;
                case FieldType.kSeed:
                    ChangeGraphics(MapManager.Instance.mapSprites.seed);
                    break;
                case FieldType.kFlower:
                    ChangeGraphics(MapManager.Instance.mapSprites.flower);
                    break;
                default:
                    // kEmpty
                    ChangeGraphics(null);
                    break;
            }

            obstacle.gameObject.SetActive(is_obstacle_);
        }

        public bool IsValidTool(ToolEnum current_tool) {
            switch (fieldType) {
                case FieldType.kEmpty:
                    return current_tool == ToolEnum.kHoe;
                case FieldType.kPlowed:
                    return current_tool == ToolEnum.kNone;
                case FieldType.kSeed:
                    return current_tool == ToolEnum.kKettle;
                case FieldType.kOcuppied:
                    switch (occupiedType) {
                        case OccupiedType.kGrass:
                            return current_tool == ToolEnum.kGrassHook;
                        case OccupiedType.kStone:
                            return current_tool == ToolEnum.kHammer;
                        case OccupiedType.kWood:
                            return current_tool == ToolEnum.kAxe;
                    }
                    break;
            }
            return false;
        }

        public void WorkedOut() {
            switch (fieldType) {
                case FieldType.kOcuppied:
                    SetData(FieldType.kEmpty);
                    break;
                case FieldType.kEmpty:
                    SetData(FieldType.kPlowed);
                    break;
                case FieldType.kPlowed:
                    SetData(FieldType.kSeed);
                    break;
                case FieldType.kSeed:
                    SetData(FieldType.kFlower);
                    break;
            }
        }

        public override bool IsOperale(PlayerController player) {
            return IsValidTool(player.currentTool);
        }

        public override void SetHighlight(bool is_highlight, bool has_op) {
            if (is_highlight) {
                highlight.gameObject.SetActive(true);
                if (has_op) {
                    highlight.color = new Color(0, 1, 0, 0.5f);
                } else {
                    highlight.color = new Color(1, 1, 0, 0.5f);
                }
            } else {
                highlight.gameObject.SetActive(false);
            }
        }

        private void ChangeGraphics(SpriteRenderer new_graphics) {
            if (graphics != null) {
                if (!PoolManager.Deallocate(graphics)) {
                    Destroy(graphics);
                }
                graphics = null;
            }
            if (new_graphics != null) {
                graphics = PoolManager.Allocate(new_graphics);
                graphics.transform.parent = transform;
                graphics.transform.localPosition = Vector3.zero;
            }
        }

        void Start() {
            // RefreshSelf
            SetData(fieldType, occupiedType);
        }
    }
}