using GestureRecognizer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spell", menuName = "PlayerSpellsData")]
public class PlayerSpellsData : ScriptableObject
{
    [System.Serializable]
    public class BaseSpell
    {
        public string name;
        public int mana;
        public GesturePattern gesture;
    }

    [System.Serializable]
    public class Aura : BaseSpell
    {
        public float duration;
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

    [System.Serializable]
    public class FireBall: BaseSpell
    {
        public GameObject spellObject;
        public float damage;
        public float speed;
    }

    public BaseSpell block;
    public Aura reflect;
    public HighJump highJump;
    public FastFall fastFall;
    public FireBall fireball;
    public BaseSpell ice;
    public BaseSpell wind;
    public BaseSpell earth;
    public BaseSpell lightning;
}

  
