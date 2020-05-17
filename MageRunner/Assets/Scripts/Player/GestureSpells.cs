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

    public bool lockedCasting;
    public Collider2D fastFallGroundCollider;

    private Dictionary<string, Spell> _spellsDict = new Dictionary<string, Spell>();
    private PlayerController _player;
    private GameObject _spellToShoot;
    private float _spellSpeed;
    
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
        shootedSpell.GetComponent<Rigidbody2D>().velocity = _player.spellShooter.transform.right * _spellSpeed;
        UnityEngine.Object.Destroy(shootedSpell, 10f);
        _player.readyToShoot = false;
        _player.animator.SetBool("ReadyToShoot", false);
        _player.animator.SetInteger("StateNumber", 6);
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
                    if (_player.jumpAvailable)
                    {
                        _player.rigidBody.velocity = Vector2.up * highJump.jumpSpeed;
                        _player.glideSpeed = highJump.glideSpeed;
                        _player.isHighJumping = true;
                        _player.jumpAvailable = false;
                    }
                };
                return new Spell(highJump.gesture.id, highJump.name, highJump.mana, highJumpCast);
           
            // Fast Fall
            case 1:
                PlayerSpellsData.FastFall fastFall = _player.spellsData.fastFall;
                Action fastFallCast = () =>
                {
                    if (_player.groundCollider == null || _player.groundCollider.CompareTag("BottomGround") == false)
                    {
                        _player.isHighJumping = false;
                        _player.jumpAvailable = false;
                        _player.rigidBody.gravityScale = _player.originalGravity;
                        _player.rigidBody.velocity = Vector2.down * fastFall.fallSpeed;
                        _player.animator.SetInteger("StateNumber", 4);
                        if (_player.groundCollider != null)
                        {
                            fastFallGroundCollider = _player.groundCollider;
                            fastFallGroundCollider.enabled = false;
                        }
                    }
                };
                return new Spell(fastFall.gesture.id, fastFall.name, fastFall.mana, fastFallCast);

            // Fireball
            case 2:
                PlayerSpellsData.FireBall fireball = _player.spellsData.fireball;
                Action fireballCast = () =>
                {
                    if (lockedCasting == false)
                    {
                        _spellToShoot = fireball.spellObject;
                        _spellSpeed = fireball.speed;
                        _player.readyToShoot = true;
                        _player.animator.SetInteger("StateNumber", 5);
                        _player.animator.SetBool("ReadyToShoot", true);
                    }
                };
                return new Spell(fireball.gesture.id, fireball.name, fireball.mana, fireballCast);

            // Block
            case 3:
                PlayerSpellsData.BaseSpell block = _player.spellsData.block;
                Action blockCast = () =>
                {
                    if (lockedCasting == false)
                    {
                        lockedCasting = true;
                        _player.animator.SetInteger("StateNumber", 8);
                        _player.animator.SetBool("Blocking", true);
                    }
                };
                return new Spell(block.gesture.id, block.name, block.mana, blockCast);

            // Reflect
            case 4:
                PlayerSpellsData.Aura reflect = _player.spellsData.reflect;
                Action reflectCast = () =>
                {
                    if (lockedCasting == false)
                    {
                        lockedCasting = true;
                        _player.reflectingDuration = reflect.duration;
                        _player.reflectAura.SetActive(true);
                        _player.isReflecting = true;
                        _player.animator.SetInteger("StateNumber", 7);
                        _player.animator.SetBool("Reflecting", true);
                    }
                };
                return new Spell(reflect.gesture.id, reflect.name, reflect.mana, reflectCast);

            // Ice
            case 5:
                PlayerSpellsData.BaseSpell ice = _player.spellsData.ice;
                Action iceCast = () =>
                {
                    Debug.Log("ice");
                };
                return new Spell(ice.gesture.id, ice.name, ice.mana, iceCast);

            // Earth
            case 6:
                PlayerSpellsData.BaseSpell earth = _player.spellsData.earth;
                Action earthCast = () =>
                {
                    Debug.Log("earth");
                };
                return new Spell(earth.gesture.id, earth.name, earth.mana, earthCast);

            // Wind
            case 7:
                PlayerSpellsData.BaseSpell wind = _player.spellsData.wind;
                Action windCast = () =>
                {
                    Debug.Log("wind");
                };
                return new Spell(wind.gesture.id, wind.name, wind.mana, windCast);

            // Lighting
            case 8:
                PlayerSpellsData.BaseSpell lightning = _player.spellsData.lightning;
                Action boltCast = () =>
                {
                    Debug.Log("lightning");
                };
                return new Spell(lightning.gesture.id, lightning.name, lightning.mana, boltCast);

            default:
                Debug.LogError("There are missing spells for the gesture patterns");
                return new Spell();
        }
    }
}
