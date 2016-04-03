using UnityEngine;
using System.Collections;

namespace Moon.Demo {
    public static class InputEvents {
        public static readonly string kHorizontal = "Horizontal";
        public static readonly string kVertical = "Vertical";
    }

    public enum MoveDirection {
        kLeft,
        kUp,
        kRight,
        kDown,
        kNone
    }

    public enum ToolEnum {
        kHoe, kGrassHook, kAxe, kHammer, kKettle, kNone
    }
}