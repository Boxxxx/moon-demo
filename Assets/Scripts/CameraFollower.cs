using UnityEngine;
using System.Collections;

namespace Moon.Demo {
    public class CameraFollower : MonoBehaviour {
        public Transform target;

        public float smoothTimeX;
        public float smoothTimeY;

        private Vector2 velocity_;

        void Update() {
            float pos_x = Mathf.SmoothDamp(transform.position.x, target.transform.position.x, ref velocity_.x, smoothTimeX);
            float pos_y = Mathf.SmoothDamp(transform.position.y, target.transform.position.y, ref velocity_.y, smoothTimeY);
            transform.position = new Vector3(pos_x, pos_y, transform.position.z);
        }
    }
}