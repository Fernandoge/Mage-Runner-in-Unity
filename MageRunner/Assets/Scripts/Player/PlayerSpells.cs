using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerSpells
{
    private struct Spell
    {
        public string id;
        public string name;
        public int mana;
        public Action castSpell;

        public Spell(string id, string name, int mana, Action castSpell)
        {
            this.id = id;
            this.name = name;
            this.mana = mana;
            this.castSpell = castSpell;
        }
    }


    private Dictionary<string, Spell> _spellsDict = new Dictionary<string, Spell>();
    private PlayerController _player;
    
    public PlayerSpells(PlayerController player)
    {
        _player = player;
        LoadSpells();
    }
    
    public void CastSpell(string id)
    {
        Spell spell = _spellsDict[id];
        _player.totalMana -= spell.mana;
        spell.castSpell();
    }

    public void LoadSpells()
    {
        for (int i = 0; i < _player.spellsAmount; i++)
        {
            Debug.LogError("loading spell");
            Spell newSpell = GetSpellData(i);
            _spellsDict.Add(newSpell.id, newSpell);
        }
    }

    private Spell GetSpellData(int spellNumber)
    {
        switch (spellNumber)
        {
            // High Jump
            case 0:
                PlayerSpellsData.HighJump highJump = _player.spellsData.highJump;
                Action highJumpCast = () =>
                {
                    MonoBehaviour.print("Jumping very high");
                    _player.transform.position = new Vector2(_player.transform.position.x, _player.transform.position.y + 1);
                };
                return new Spell(highJump.gesture.id, highJump.name, highJump.mana, highJumpCast);
           
            // Fast Fall
            case 1:
                PlayerSpellsData.FastFall fastFall = _player.spellsData.fastFall;
                Action fastFallCast = () =>
                {
                    MonoBehaviour.print("Falling quickly to the ground");
                    _player.transform.position = new Vector2(_player.transform.position.x, _player.transform.position.y - 1);
                };
                return new Spell(fastFall.gesture.id, fastFall.name, fastFall.mana, fastFallCast);

            default:
                Debug.LogError("There are missing spells for the gesture patterns");
                return new Spell();
        }
    }
}
