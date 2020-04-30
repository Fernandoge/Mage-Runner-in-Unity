using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GestureSpells
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
    private GameObject _spellToShoot;
    private float _shootSpeed;
    
    public GestureSpells(PlayerController player)
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

    public void ShootSpell()
    {
        GameObject shootedSpell = UnityEngine.Object.Instantiate(_spellToShoot, _player.spellShooter.transform.position, _player.spellShooter.transform.rotation);
        shootedSpell.GetComponent<Rigidbody2D>().velocity = _player.spellShooter.transform.up * _shootSpeed;
        UnityEngine.Object.Destroy(shootedSpell, 10f);
        _player.readyToShoot = false;
        _player.animator.SetBool("ReadyToShoot", false);
    }

    public void LoadSpells()
    {
        for (int i = 0; i < _player.spellsAmount; i++)
        {
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
                    if (_player.isGrounded)
                    {
                        _player.rigidBody.velocity = Vector2.up * highJump.jumpSpeed;
                        _player.glideSpeed = highJump.glideSpeed;
                        _player.isGliding = true;
                        _player.animator.SetBool("HighJump", true);
                        _player.animator.SetBool("Running", false);
                    }
                };
                return new Spell(highJump.gesture.id, highJump.name, highJump.mana, highJumpCast);
           
            // Fast Fall
            case 1:
                PlayerSpellsData.FastFall fastFall = _player.spellsData.fastFall;
                Action fastFallCast = () =>
                {
                    if (_player.isGrounded == false)
                    {
                        _player.isGliding = false;
                        _player.rigidBody.gravityScale = _player.originalGravity;
                        _player.rigidBody.velocity = Vector2.down * fastFall.fallSpeed;
                        _player.animator.SetBool("FastFall", true);
                        _player.animator.SetBool("HighJump", false);
                    }
                };
                return new Spell(fastFall.gesture.id, fastFall.name, fastFall.mana, fastFallCast);

            // Fireball
            case 2:
                PlayerSpellsData.FireBall fireball = _player.spellsData.fireball;
                Action fireballCast = () =>
                {
                    _spellToShoot = fireball.spellObject;
                    _shootSpeed = fireball.speed;
                    _player.readyToShoot = true;
                    _player.animator.SetBool("ReadyToShoot", true);
                };
                return new Spell(fireball.gesture.id, fireball.name, fireball.mana, fireballCast);

            default:
                Debug.LogError("There are missing spells for the gesture patterns");
                return new Spell();
        }
    }
}
