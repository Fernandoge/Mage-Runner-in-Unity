using System;
using UnityEngine;

namespace MageRunner.Dialogues
{
    [Serializable]
    public struct Message
    {
        [TextArea] public string text;
        public float secondsToContinue;
        public bool waitForAction;
        public bool enablePlayerIdleCast;
        public bool changeLevelMovingState;

        public Message (string text, float secondsToContinue, bool waitForAction, bool enablePlayerIdleCast, bool changeLevelMovingState)
        {
            this.text = text;
            this.secondsToContinue = secondsToContinue;
            this.waitForAction = waitForAction;
            this.enablePlayerIdleCast = enablePlayerIdleCast;
            this.changeLevelMovingState = changeLevelMovingState;
        }
    }
}
