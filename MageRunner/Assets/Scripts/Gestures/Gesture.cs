using System;
using System.Collections.Generic;
using GestureRecognizer;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct Gesture
{
    public EPlayerSpells spell;
    public EGestureDifficulty difficulty;
    public SpriteRenderer iconRenderer;
    [NonSerialized] public GesturePattern pattern;
    [NonSerialized] public Transform holder;
    [NonSerialized] public bool isBossGesture;

    public Gesture(EPlayerSpells spell, EGestureDifficulty difficulty, SpriteRenderer iconRenderer, GesturePattern pattern, Transform holder, bool isBossGesture)
    {
        this.spell = spell;
        this.difficulty = difficulty;
        this.iconRenderer = iconRenderer;
        this.pattern = pattern;
        this.holder = holder;
        this.isBossGesture = isBossGesture;
    }
}