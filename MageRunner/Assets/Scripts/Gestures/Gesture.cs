using System;
using GestureRecognizer;
using MageRunner.Player;
using UnityEngine;

namespace MageRunner.Gestures
{
    [System.Serializable]
    public struct Gesture
    {
        public EPlayerSpells spell;
        public EGestureDifficulty difficulty;
        public SpriteRenderer iconRenderer;
        [NonSerialized] public GesturePattern pattern;
        [NonSerialized] public GameObject holder;
        [NonSerialized] public bool isBossGesture;

        public Gesture(EPlayerSpells spell, EGestureDifficulty difficulty, SpriteRenderer iconRenderer, GesturePattern pattern, GameObject holder, bool isBossGesture)
        {
            this.spell = spell;
            this.difficulty = difficulty;
            this.iconRenderer = iconRenderer;
            this.pattern = pattern;
            this.holder = holder;
            this.isBossGesture = isBossGesture;
        }
    }
}