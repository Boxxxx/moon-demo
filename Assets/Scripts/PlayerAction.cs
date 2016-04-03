using UnityEngine;
using System.Collections.Generic;
using System;

namespace Moon.Demo {
    public class ActionStep {
        private System.Action<bool> done_cb_;

        public virtual void Start(System.Action<bool> done_cb) {
            done_cb_ = done_cb;
        }

        public virtual void Update() { }

        public void Done(bool succeed) {
            if (done_cb_ != null) {
                done_cb_(succeed);
            }
        }
    }

    public class MoveActionStep : ActionStep {
        private PolyNavAgent agent_;
        private Vector2 target_;

        public MoveActionStep(PolyNavAgent agent, Vector2 target) {
            agent_ = agent;
            target_ = target;
        }

        public override void Start(Action<bool> done_cb) {
            base.Start(done_cb);

            agent_.SetDestination(target_, Done);
        }
    }

    public class WorkActionStep : ActionStep {
        private PlayerController player_;
        private MoveDirection direction_;
        private float delta_;
        private FieldWidget target_field_;

        public WorkActionStep(PlayerController player, MoveDirection face, FieldWidget target_field, float delta) {
            player_ = player;
            direction_ = face;
            delta_ = delta;
            target_field_ = target_field;
        }

        public override void Start(Action<bool> done_cb) {
            base.Start(done_cb);

            player_.moveDir = direction_;
            if (player_.isWorking) {
                Done(false);
            } else {
                player_.Work(delta_, () => {
                    target_field_.WorkedOut();
                    Done(true);
                });
            }
        }
    }

    public class ActionList {
        public enum StatusEnum {
            kWaiting,
            kRunning,
            kFinish
        }

        private List<ActionStep> steps_ = new List<ActionStep>();
        private StatusEnum status_ = StatusEnum.kWaiting;
        private int current_index_ = 0;

        public System.Action<bool> onAllDone;

        public bool isRunning {
            get { return status_ == StatusEnum.kRunning; }
        }
        
        public void Add(ActionStep step) {
            if (status_ == StatusEnum.kWaiting) {
                steps_.Add(step);
            } else {
                Debug.LogError("[ActionList] You can only add step before Start().");
            }
        }

        public void Start() {
            status_ = StatusEnum.kRunning;
            if (steps_.Count == 0) {
                AllDown(true);
            } else {
                current_index_ = 0;
                steps_[current_index_].Start(OnDone);
            }
        }

        public void Cancel() {
            AllDown(false);
        }

        public void Update() {
            if (isRunning) {
                steps_[current_index_].Update();
            }
        }

        public void OnDone(bool succeed) {
            if (!isRunning) {
                // Canceld.
                return;
            }
            if (succeed) {
                if (++current_index_ >= steps_.Count) {
                    AllDown(true);
                } else {
                    steps_[current_index_].Start(OnDone);
                }
            } else {
                AllDown(false);
            }
        }

        private void AllDown(bool succeed) {
            status_ = StatusEnum.kFinish;
            if (onAllDone != null) {
                onAllDone(succeed);
            }
        }
    }
}