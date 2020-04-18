using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GestureRecognizer;

public class Player : MonoBehaviour
{
    public int mana;
    public List<PlayerSpells.Spell> spellsData;

    private PlayerSpells _spells;

    private void Awake()
    {
        _spells = new PlayerSpells(this);
    }

    public void BeginCastingSpell(string id)
    {
        _spells.CastSpell(id);
    }

    public void CastingSpellFailed()
    {
        print("Casting spell failed");
    }
}
