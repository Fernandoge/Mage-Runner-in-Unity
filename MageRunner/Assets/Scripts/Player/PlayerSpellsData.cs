using GestureRecognizer;
using UnityEngine;

namespace MageRunner.Player
{
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
    
        [System.Serializable] public class Block : BaseSpell { public float duration; }
        
        [System.Serializable] public class Dash : BaseSpell { public float speed; public float duration; }

        [System.Serializable] public class HighJump : BaseSpell { public float jumpSpeed; public float glideSpeed; }

        [System.Serializable] public class FastFall : BaseSpell { public float fallSpeed; }
    
        [System.Serializable] public class AttackSpell : BaseSpell { public float speed; public GameObject spellObject; }
    
        [Header("Basic Spells")]
        public Block block;
        public Dash dash;
        public HighJump highJump;
        public FastFall fastFall;
    
        [Header("Attacks")]
        public AttackSpell fireball;
        public AttackSpell fireballHeavy;
        public AttackSpell ice;
        public AttackSpell iceHeavy;
        public AttackSpell wind;
        public AttackSpell windHeavy;
        public AttackSpell earth;
        public AttackSpell earthHeavy;
        public AttackSpell lightning;
        public AttackSpell lightningHeavy;
        public AttackSpell nature;
        public AttackSpell natureHeavy;
        public AttackSpell water;
        public AttackSpell waterHeavy;
    }
}

  
