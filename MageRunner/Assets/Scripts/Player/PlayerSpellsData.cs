using GestureRecognizer;
using UnityEngine;

[CreateAssetMenu(menuName = "PlayerSpellsData")]
public class PlayerSpellsData : ScriptableObject
{
    [System.Serializable]
    public class BaseSpell
    {
        public string name;
        public int mana;
        public GesturePattern gesture;
    }
    
    [System.Serializable] public class Aura : BaseSpell { public float duration; }

    [System.Serializable] public class HighJump : BaseSpell { public float jumpSpeed; public float glideSpeed; }

    [System.Serializable] public class FastFall : BaseSpell { public float fallSpeed; }
    
    [System.Serializable] public class AttackSpell : BaseSpell { public GameObject spellObject; }
    
    [Header("Basic Spells")]
    public BaseSpell block;
    public Aura reflect;
    public HighJump highJump;
    public FastFall fastFall;
    
    [Header("Attacks")]
    public AttackSpell fireball;
    public AttackSpell ice;
    public AttackSpell wind;
    public AttackSpell earth;
    public AttackSpell lightning;
    public AttackSpell nature;
    public AttackSpell water;
}

  
