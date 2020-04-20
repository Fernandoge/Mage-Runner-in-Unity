using GestureRecognizer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spell", menuName = "PlayerSpellsData")]
public class PlayerSpellsData : ScriptableObject
{
    public class BaseSpell
    {
        public string name;
        public int mana;
        public GesturePattern gesture;
    }


    [System.Serializable]
    public class HighJump : BaseSpell
    {
        public float jumpSpeed;
        public float glideSpeed;
    }

    [System.Serializable]
    public class FastFall : BaseSpell
    {
        public float fallSpeed;
    }

    public HighJump highJump;
    public FastFall fastFall;
}

  
