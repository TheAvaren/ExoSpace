using System.Collections;
using System;
using System.Collections.Generic;

namespace Evarentech.Timers
{
    public class Timer
    {
        /// <summary>
        /// Time since event started.
        /// </summary>
        public float eventTime;

        /// <summary>
        /// Time it will take to reset the event;
        /// </summary>
        public float eventResetTime;

        /// <summary>
        /// Whether or not the event is currently active.
        /// </summary>
        public bool active { get; private set; }

        public Guid id { get; private set; }

        public string name { get; private set; }

        public bool HasAction
        {
            get
            {
                return hasAction;
            }

            private set
            {
                hasAction = value;
            }
        }

        private bool hasAction = false;

        private Actions actions;

        #region constructors
        public Timer(float eventResetTime, string name)
        {
            this.eventResetTime = eventResetTime;
            this.name = name;
            id = Guid.NewGuid();
            TimerController.instance.AddTimer(this, actions);
        }

        public Timer(string name, float eventResetTime, Func<ActionState> endAction) : this(eventResetTime, name)
        {
            hasAction = true;
            actions = new Actions();
            this.actions.endAction = endAction;
        }

        public Timer(string name, float eventResetTime, Action startAction, Func<ActionState> endAction) : this(name, eventResetTime, endAction)
        {
            this.actions.startAction = startAction;
        }

        public Timer(string name, float eventResetTime, Action startAction, Func<ActionState> endAction, Action customAction, Func<bool> customActionExpression) : this(name, eventResetTime, startAction, endAction)
        {
            this.actions.customAction = customAction;
            this.actions.customActionExpression = customActionExpression;
        }
        #endregion

        #region equality
        public override bool Equals(object obj)
        {
            var timer = obj as Timer;
            return timer != null &&
                   id.Equals(timer.id) &&
                   name == timer.name;
        }

        public override int GetHashCode()
        {
            var hashCode = -48284730;
            hashCode = hashCode * -1521134295 + EqualityComparer<Guid>.Default.GetHashCode(id);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(name);
            return hashCode;
        }
        #endregion

        private void Activate()
        {
            active = true;
            if (actions.startAction != null && hasAction)
            {
                actions.startAction.Invoke();
            }
        }

        private void Deactivate()
        {
            active = false;
            if (actions.endAction != null && hasAction)
            {
                actions.endAction.Invoke();
            }
        }

        private void CustomAction()
        {
            if (actions.customAction != null && hasAction)
            {
                actions.customAction.Invoke();
            }
        }

        private bool CustomExpression()
        {
            if (actions.customActionExpression != null && hasAction)
            {
                return actions.customActionExpression.Invoke();
            }
            return false;
        }

        public class Actions
        {
            #region actions

            /// <summary>
            /// Action that is ran everytime the timer starts.
            /// </summary>
            public Action startAction;

            /// <summary>
            /// Action that is ran everytime the timer ends.
            /// </summary>
            public Func<ActionState> endAction;

            /// <summary>
            /// Action that is ran everytime 'customActionExpression' evaluates true.
            /// </summary>
            public Action customAction;

            /// <summary>
            /// If action evaluates true then 'customAction' will Invoke.
            /// </summary>
            public Func<bool> customActionExpression;
            #endregion

            public Actions()
            {
            }
        }

        public enum ActionState
        {
            Nothing = 0,
            Destroy = 1,
            Restart = 2
        }
    }
}
