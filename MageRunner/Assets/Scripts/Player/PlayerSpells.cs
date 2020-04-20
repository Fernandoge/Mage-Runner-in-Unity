using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GestureRecognizer;
using System;

public class PlayerSpells
{
    #region Spell struct
    [System.Serializable]
    public struct Spell
    {
        [System.NonSerialized]
        public string id;
        public string name;
        public int mana;
        public GesturePattern gesture;
        public Action castSpell;

        public Spell(string id, string name, int mana, GesturePattern gesture, Action castSpell)
        {
            this.id = id;
            this.name = name;
            this.mana = mana;
            this.gesture = gesture;
            this.castSpell = castSpell;
        }
    }
    #endregion Spell struct

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
        foreach(Spell spell in _player.spellsData)
        {
            string spellId = spell.gesture.id;
            Spell newSpell = new Spell(spellId, spell.name, spell.mana, spell.gesture, GetSpellAction(spellId));
            _spellsDict.Add(newSpell.id, newSpell);
        }
    }

    public Action GetSpellAction(string id)
    {
        switch (id)
        {
            // High Jump
            case "verticalUp":
                return () =>
                {
                    MonoBehaviour.print("Jumping very high");
                    _player.transform.position = new Vector2(_player.transform.position.x, _player.transform.position.y + 1);
                };
           
            // Fast Fall
            case "verticalDown":
                return () =>
                {
                    MonoBehaviour.print("Falling quickly to the ground");
                    _player.transform.position = new Vector2(_player.transform.position.x, _player.transform.position.y - 1);
                };

            default:
                return () => Debug.LogError("Missing action for spell id: " + id);
        }
    }
}
