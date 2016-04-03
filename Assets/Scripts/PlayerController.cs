using UnityEngine;
using System.Collections;

namespace Moon.Demo {
    public class PlayerController : MonoBehaviour {
        public static class AnimatorParameters {
            public static readonly string MoveDir = "move_dir";
            public static readonly string IsMoving = "is_moving";
        }

        public Animator animator;
        public Vector2 moveSpeed = new Vector2(1, 1);
        public float workTime = 0.5f;

        private Vector2 move_dir_ = Vector2.zero;
        private Rigidbody2D rigidbody2d_;
        private PolyNavAgent agent_;
        private MoveDirection move_dir_enum_ = MoveDirection.kNone;
        private bool is_working_ = false;
        private float rest_worktime = 0;
        private System.Action work_finish_cb_;

        private ActionList current_actionlist_ = null;

        public MoveDirection moveDir {
            get { return move_dir_enum_; }
            set {
                if (move_dir_enum_ != value) {
                    move_dir_enum_ = value;
                    animator.SetInteger(AnimatorParameters.MoveDir, (int)value);
                }
            }
        }
        public bool isMoving {
            get;
            private set;
        }
        public bool isInAction {
            get { return current_actionlist_ != null; }
        }

        public bool isWorking {
            get {
                return is_working_;
            }
        }
        public ToolEnum currentTool {
            get;
            private set;
        }

        /// <summary>
        /// Move to a target position, without doing any operation.
        /// </summary>
        public void MoveTo(Vector2 world) {
            if (is_working_) {
                return;
            }

            agent_.SetDestination(world);
        }

        public void MoveAndOperate(MapWidget target_widget) {
            if (is_working_) {
                return;
            }

            var candidate_pos = MapManager.Instance.GetValidAdjancentTile(target_widget);
            var current_pos = (Vector2)transform.position;
            candidate_pos.Sort((lhs, rhs) => {
                float val = (lhs - current_pos).sqrMagnitude - (rhs - current_pos).sqrMagnitude;
                return val < 0 ? -1 : (val > 0 ? 1 : 0);
            });

            var adjacent_pos = candidate_pos[0];
            MoveDirection face = GetMoveDirection((Vector2)target_widget.transform.position - adjacent_pos);

            if (current_actionlist_ != null && current_actionlist_.isRunning) {
                current_actionlist_.Cancel();
            }

            current_actionlist_ = new ActionList();
            current_actionlist_.Add(new MoveActionStep(agent_, adjacent_pos));
            current_actionlist_.Add(new WorkActionStep(this, face, target_widget as FieldWidget, workTime));
            current_actionlist_.Start();
        }

        public void Work(float worktime, System.Action finish_cb) {
            if (is_working_) {
                return;
            }
            agent_.Stop();
            is_working_ = true;
            rest_worktime = worktime;
            work_finish_cb_ = finish_cb;
        }

        public void SetTool(ToolEnum new_tool) {
            currentTool = new_tool;
        }

        private void Awake() {
            rigidbody2d_ = GetComponent<Rigidbody2D>();
            agent_ = GetComponent<PolyNavAgent>();
        }

        private void Start() {
            moveDir = MoveDirection.kDown;
            currentTool = ToolEnum.kNone;
        }

        private void Update() {
            if (is_working_) {
                rest_worktime -= Time.deltaTime;
                if (rest_worktime <= 0) {
                    if (work_finish_cb_ != null) {
                        work_finish_cb_();
                    }
                    is_working_ = false;
                }
            }
            if (isInAction) {
                current_actionlist_.Update();
            }
        }

        private void FixedUpdate() {
            MoveUpdate();
        }

        private void OnGUI() {
            if (is_working_) {
                GUI.color = Color.red;
                GUI.Label(new Rect(50, 50, 300, 50), string.Format("Working, finish in {0}...", rest_worktime));
            }
        }

        private void MoveUpdate() {
            UpdateMoveDirction(agent_.movingDirection);
        }
        private void UpdateMoveDirction(Vector2 direction) {
            move_dir_ = direction;
            isMoving = direction.sqrMagnitude > Utils.kLargeEps;
            animator.SetBool(AnimatorParameters.IsMoving, isMoving);
            if (isMoving) {
                moveDir = GetMoveDirection(move_dir_);
            }
        }

        private static MoveDirection GetMoveDirection(Vector2 move_dir) {
            if (move_dir.x < -Utils.kLargeEps && -move_dir.x >= Mathf.Abs(move_dir.y)) {
                return MoveDirection.kLeft;
            }
            else if (move_dir.x > Utils.kLargeEps && move_dir.x >= Mathf.Abs(move_dir.y)) {
                return MoveDirection.kRight;
            }
            else if (move_dir.y > Utils.kLargeEps) {
                return MoveDirection.kUp;
            }
            else if (move_dir.y < -Utils.kLargeEps) {
                return MoveDirection.kDown;
            }
            else {
                return MoveDirection.kNone;
            }
        }
    }
}