using UnityEngine;
using System.Collections;

namespace Moon.Demo {
    public class GameManager : SingletonMonoBehaviour<GameManager> {
        public Camera mainCamera;
        public PlayerController player;

        private MapWidget current_selected = null;

        public ToolEnum currentTool {
            get { return player.currentTool; }
        }

        public void OnTouch(Vector2 screen_pos, bool new_down) {
            if (!new_down && player.isInAction) {
                return;
            } else {
                var world_pos = mainCamera.ScreenToWorldPoint(screen_pos);

                MapWidget new_selected = MapManager.Instance.SelectWidget(world_pos);
                if (current_selected != null && current_selected != new_selected) {
                    current_selected.SetHighlight(false, false);
                }
                current_selected = new_selected;
                if (new_selected != null) {
                    if (new_selected.IsOperale(player)) {
                        current_selected.SetHighlight(true, true);
                        player.MoveAndOperate(current_selected);
                    }
                    else {
                        current_selected.SetHighlight(true, false);
                        player.MoveTo(world_pos);
                    }
                }
            }
        }

        public void OnToolChanged(ToolEnum tool) {
            player.SetTool(tool);
            Debug.Log("[GameManager] change tool into " + tool);
        }
    }
}