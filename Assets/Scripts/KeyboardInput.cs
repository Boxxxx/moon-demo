using UnityEngine;
using System.Collections;

namespace Moon.Demo {
    public class KeyboardInput : MonoBehaviour {
        public PlayerController controller;

        void Update() {
            float horiz_axis = Input.GetAxis(InputEvents.kHorizontal);
            float vert_axis = Input.GetAxis(InputEvents.kVertical);
            //controller.SetMove(MathUtils.ToIntSign(horiz_axis), MathUtils.ToIntSign(vert_axis));
        }
    }
}
