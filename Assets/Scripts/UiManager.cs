using UnityEngine;
using System.Collections;

namespace Moon.Demo {
    public class UiManager : SingletonMonoBehaviour<UiManager> {
        public Camera uiCamera;

        private bool is_pressing = false;

        public void OnToolChanged(UIToolButton selected_tool) {
            if (selected_tool.GetComponent<UIToggle>().value) {
                GameManager.Instance.OnToolChanged(selected_tool.toolEnum);
            }
        }

        public void OnTouchPress() {
            GameManager.Instance.OnTouch(uiCamera.WorldToScreenPoint(UICamera.lastWorldPosition), !is_pressing);
            is_pressing = true;
        }
        public void OnTouchRelease() {
            is_pressing = false;
        }

        void Update() {
            if (is_pressing) {
                GameManager.Instance.OnTouch(uiCamera.WorldToScreenPoint(UICamera.lastWorldPosition), false);
            }
        }
    }
}