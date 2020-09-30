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

    public GestureSpells(PlayerController player)
    {
        _player = player;
        LoadSpells();
    }
    
    public void CastSpell(string id)
    {
        Spell spell = _spellsDict[id];
        if (_player.manaController.currentMana >= spell.mana)
        {
            _player.manaController.UpdateMana(-spell.mana);
            // _player.companion.StartChat(spell.name, 0.5f);
            spell.castSpell();
        }
        else
        {
            _player.manaController.NoManaFeedback();
            Debug.Log("Not enough mana");
        }
    }

    public void ShootSpell()
    {
        LevelController currentLevel = GameManager.Instance.level;
        Transform spellParent = currentLevel.movingObjects;
        GameObject shootedSpell = UnityEngine.Object.Instantiate(_spellToShoot, _player.spellShooter.transform.position, _player.spellShooter.transform.rotation, spellParent);
        Attack shootedSpellComponent = shootedSpell.GetComponent<Attack>();
        float speedReduction = _player.isMoving == false ? 0f : (shootedSpellComponent.speed.Equals(0f) ? currentLevel.movingSpeed : currentLevel.movingSpeed / 2);
        shootedSpellComponent.rigBody.velocity = _player.spellShooter.transform.right * (shootedSpellComponent.speed - speedReduction);
        shootedSpellComponent.shooterLayer = _player.gameObject.layer;
        _player.stateHandler.DisableState(EPlayerState.ReadyToShoot);
    }

    private void LoadSpells()
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
                        _player.stateHandler.EnableState(EPlayerState.HighJumping);
                        _player.rigidBody.velocity = Vector2.up * highJump.jumpSpeed;
                        _player.glideSpeed = highJump.glideSpeed;
                    }
                };
                return new Spell(highJump.gesture.id, highJump.name, highJump.mana, highJumpCast);
           
            // Fast Fall
            case 1:
                PlayerSpellsData.FastFall fastFall = _player.spellsData.fastFall;
                Action fastFallCast = () =>
                {
                    if (_player.groundCollider == null || _player.groundLayer != LayerMask.NameToLayer("BottomGround"))
                    {
                        _player.stateHandler.EnableState(EPlayerState.FastFall);
                        _player.rigidBody.velocity = Vector2.down * fastFall.fallSpeed;

                        if (_player.groundCollider == null)
                            return;
                        
                        fastFallGroundCollider = _player.groundCollider;
                        fastFallGroundCollider.enabled = false;
                    }
                };
                return new Spell(fastFall.gesture.id, fastFall.name, fastFall.mana, fastFallCast);

            // Block
            case 2:
                PlayerSpellsData.BaseSpell block = _player.spellsData.block;
                Action blockCast = () =>
                {
                    if (lockedCasting == false)
                    {
                        _player.stateHandler.EnableState(EPlayerState.Blocking);
                    }
                };
                return new Spell(block.gesture.id, block.name, block.mana, blockCast);

            // Reflect
            case 3:
                PlayerSpellsData.Aura reflect = _player.spellsData.reflect;
                Action reflectCast = () =>
                {
                    if (lockedCasting == false)
                    {
                        _player.stateHandler.EnableState(EPlayerState.Reflecting);
                        _player.reflectingDuration = reflect.duration;
                    }
                };
                return new Spell(reflect.gesture.id, reflect.name, reflect.mana, reflectCast);
            
            // Fireball
            case 4:
                PlayerSpellsData.AttackSpell fireball = _player.spellsData.fireball;
                Action fireballCast = () => { PrepareAttack(fireball, EAttackSpellType.Projectile); };
                return new Spell(fireball.gesture.id, fireball.name, fireball.mana, fireballCast);

            // Ice
            case 5:
                PlayerSpellsData.AttackSpell ice = _player.spellsData.ice;
                Action iceCast = () => { PrepareAttack(ice, EAttackSpellType.Projectile); };
                return new Spell(ice.gesture.id, ice.name, ice.mana, iceCast);

            // Earth
            case 6:
                PlayerSpellsData.AttackSpell earth = _player.spellsData.earth;
                Action earthCast = () => { PrepareAttack(earth, EAttackSpellType.Projectile); };
                return new Spell(earth.gesture.id, earth.name, earth.mana, earthCast);

            // Wind
            case 7:
                PlayerSpellsData.AttackSpell wind = _player.spellsData.wind;
                Action windCast = () => { PrepareAttack(wind, EAttackSpellType.Projectile); };
                return new Spell(wind.gesture.id, wind.name, wind.mana, windCast);

            // Lighting
            case 8:
                PlayerSpellsData.AttackSpell lightning = _player.spellsData.lightning;
                Action boltCast = () => { PrepareAttack(lightning, EAttackSpellType.Instant); };
                return new Spell(lightning.gesture.id, lightning.name, lightning.mana, boltCast);
            
            // Nature
            case 9:
                PlayerSpellsData.AttackSpell nature = _player.spellsData.nature;
                Action natureCast = () => { PrepareAttack(nature, EAttackSpellType.Projectile); };
                return new Spell(nature.gesture.id, nature.name, nature.mana, natureCast);
            
            // Water
            case 10:
                PlayerSpellsData.AttackSpell water = _player.spellsData.water;
                Action waterCast = () => { PrepareAttack(water, EAttackSpellType.Projectile); };
                return new Spell(water.gesture.id, water.name, water.mana, waterCast);

            default:
                Debug.LogError("There are missing spells for the gesture patterns");
                return new Spell();
        }
    }

    private void PrepareAttack(PlayerSpellsData.AttackSpell attackSpell, EAttackSpellType spellType)
    {
        if (lockedCasting == false)
        {
            _spellToShoot = attackSpell.spellObject;
            _player.drawArea.raycastTarget = false;
            _player.spellToShootType = spellType;
            _player.stateHandler.EnableState(EPlayerState.ReadyToShoot);
        }
    }
}