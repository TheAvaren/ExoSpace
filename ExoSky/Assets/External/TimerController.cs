using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace Evarentech.Timers
{
    [DefaultExecutionOrder(-100)]
    public class TimerController : MonoBehaviour {

        public static TimerController instance { get; private set; }

        private Dictionary<Timer, Timer.Actions> timers = new Dictionary<Timer, Timer.Actions>();

        void Awake()
        {
            instance = this;
        }

        public void AddTimer(Timer timer, Timer.Actions actions)
        {
            timers.Add(timer, actions);
        }

        public void RemoveTimer(Timer timer)
        {
            timers.Remove(timer);
        }

        // Update is called once per frame
        void Update() {
            Timer timer;
            Timer.Actions actions;
            //Update the times on all of our timers.
            for (int i = 0; i < timers.Count; i++)
            {
                timer = timers.ElementAt(i).Key;
                actions = timers[timer];
                timer.eventTime += Time.deltaTime;

                if (timer.eventTime >= timer.eventResetTime)
                {
                    Timer.ActionState actionState = Timer.ActionState.Nothing;
                    if (actions.endAction != null)
                    {
                        actionState = actions.endAction();

                        if (actionState == Timer.ActionState.Destroy)
                        {
                            timers.Remove(timer);
                        }
                        else if (actionState == Timer.ActionState.Restart)
                        {
                            timer.eventTime = 0;
                            if (actions.startAction != null)
                            {
                                actions.startAction();
                            }
                        }
                    }
                }

                if (actions.customAction != null && actions.customActionExpression != null)
                {
                    if (actions.customActionExpression.Invoke())
                    {
                        actions.customAction.Invoke();
                    }
                }
            }
        }
    }
}