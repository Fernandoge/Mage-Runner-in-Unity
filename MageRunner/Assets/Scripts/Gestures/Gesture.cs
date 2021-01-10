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
    [NonSerialized] public bool isBossGesture;
}