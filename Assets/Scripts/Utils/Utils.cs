using UnityEngine;

namespace Moon.Demo {
    public static class Utils {
        public const float kLargeEps = 1e-5f;

        public static int ToIntSign(float value) {
            return Mathf.Abs(value) < kLargeEps ? 0 : (value > 0 ? 1 : -1);
        }

        public static Vector3 RandomVector(Vector2 fluctuation) {
            return new Vector3(Random.Range(-fluctuation.x, fluctuation.x), Random.Range(-fluctuation.y, fluctuation.y), 0);
        }

        public static Bounds CalculateOrthographicBounds(Camera camera) {
            float screen_aspect = Screen.width / (float)Screen.height;
            float camera_height = camera.orthographicSize * 2;
            Bounds bounds = new Bounds() {
                center = new Vector3(camera.transform.position.x, camera.transform.position.y, 0),
                size = new Vector3(camera_height * screen_aspect, camera_height, 0)
            };
            return bounds;
        }

        public static Vector2 WorldPosToUI(Vector3 position) {
            Vector2 pos = GameManager.Instance.mainCamera
                .WorldToViewportPoint(position);
            return UiManager.Instance.transform.InverseTransformPoint(
                UiManager.Instance.uiCamera.ViewportToWorldPoint(pos));
        }

        public static Vector3 UILocalToWorld(Vector2 ui_pos) {
            return UiManager.Instance.transform.TransformPoint(ui_pos);
        }
    }
}