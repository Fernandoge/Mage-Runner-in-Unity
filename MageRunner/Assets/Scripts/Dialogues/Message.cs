using System;
using UnityEngine;
using UnityEngine.Events;

namespace MageRunner.Dialogues
{
    [Serializable]
    public struct Message
    {
        [TextArea] public string text;
        public float secondsToStart;
        public float secondsToContinue;
        public bool waitForAction;
        public bool enablePlayerIdleCast;
        public bool changeLevelMovingState;
        public UnityEvent onTextShowed;

        public Message (string text, float secondsToStart, float secondsToContinue, bool waitForAction, bool enablePlayerIdleCast, bool changeLevelMovingState, UnityEvent onTextShowed)
        {
            this.text = text;
            this.secondsToStart = secondsToStart;
            this.secondsToContinue = secondsToContinue;
            this.waitForAction = waitForAction;
            this.enablePlayerIdleCast = enablePlayerIdleCast;
            this.changeLevelMovingState = changeLevelMovingState;
            this.onTextShowed = onTextShowed;
        }
    }
}
