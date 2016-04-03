using System.Collections.Generic;

namespace Moon.Demo {
    public enum Events {
        OnEnemyBorn,
        OnEnemyInView,
        OnEnemyAction,
        OnEnemyDestroy
    }

    public class EventPool : SingletonMonoBehaviour<EventPool> {
        public delegate void EventCallbackDelegate(object[] args);

        private Dictionary<Events, HashSet<EventCallbackDelegate>> eventCB = new Dictionary<Events, HashSet<EventCallbackDelegate>>();
        private Dictionary<Events, HashSet<EventCallbackDelegate>> onceEventCB = new Dictionary<Events, HashSet<EventCallbackDelegate>>();

        public void On(Events ev, EventCallbackDelegate cb) {
            EnsureEvents(eventCB, ev).Add(cb);
        }

        public void Once(Events ev, EventCallbackDelegate cb) {
            EnsureEvents(onceEventCB, ev).Add(cb);
        }

        public void Remove(EventCallbackDelegate cb) {
            foreach (var cbs in eventCB.Values) {
                cbs.Remove(cb);
            }
            foreach (var cbs in onceEventCB.Values) {
                cbs.Remove(cb);
            }
        }

        public void Remove(Events ev, EventCallbackDelegate cb) {
            EnsureEvents(eventCB, ev).Remove(cb);
            EnsureEvents(onceEventCB, ev).Remove(cb);
        }

        public void Emit(Events ev, params object[] args) {
            foreach (var cb in EnsureEvents(eventCB, ev)) {
                cb(args);
            }
            foreach (var cb in EnsureEvents(onceEventCB, ev)) {
                cb(args);
            }
            onceEventCB.Remove(ev);
        }

        private HashSet<EventCallbackDelegate> EnsureEvents(Dictionary<Events, HashSet<EventCallbackDelegate>> events, Events ev) {
            if (!events.ContainsKey(ev)) {
                events[ev] = new HashSet<EventCallbackDelegate>();
            }
            return events[ev];
        }
    }

}