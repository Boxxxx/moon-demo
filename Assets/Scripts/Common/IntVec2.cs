using UnityEngine;
using System.Collections;

namespace Moon.Demo {
    [System.Serializable]
    public struct IntVec2 {
        public int x;
        public int y;

        public int width {
            get { return x; }
        }
        public int height {
            get { return y; }
        }

        public IntVec2(int x, int y) {
            this.x = x;
            this.y = y;
        }
    }
}