﻿using MageRunner.Dialogues;
using UnityEngine;

namespace MageRunner.FTUE
{
    public class Ftue_2_DoubleJump : FtueSection
    { 
        [SerializeField] private DialogueController _firstStepDialogue;

        // Dialogue to start FTUE
        public override void FirstStep() => _firstStepDialogue.StartDialogue();
    }
}
